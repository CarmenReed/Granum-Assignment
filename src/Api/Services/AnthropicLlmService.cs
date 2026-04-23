using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Api.Services;

public class AnthropicLlmService : ILlmService
{
    private const int MaxTokens = 1024;
    private const int TimeoutSeconds = 30;
    private const int StreamStallTimeoutSeconds = 30;
    private const string AnthropicVersion = "2023-06-01";

    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _systemPrompt;

    public AnthropicLlmService(HttpClient http)
    {
        _http = http;
        _apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
            ?? throw new InvalidOperationException("ANTHROPIC_API_KEY is not set.");
        _model = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL")
            ?? "claude-haiku-4-5-20251001";

        var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "system_prompt.txt");
        _systemPrompt = File.Exists(promptPath) ? File.ReadAllText(promptPath) : string.Empty;
    }

    public async Task<LlmResult> EnhanceAsync(string rawNote, CancellationToken ct = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
        linked.CancelAfter(TimeSpan.FromSeconds(TimeoutSeconds));

        var sw = Stopwatch.StartNew();
        using var req = BuildRequest(rawNote, stream: false);
        using var resp = await _http.SendAsync(req, linked.Token);
        resp.EnsureSuccessStatusCode();
        var payload = await resp.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken: linked.Token);
        sw.Stop();

        if (payload is null)
            throw new InvalidOperationException("Anthropic response was empty.");

        var text = string.Concat(payload.content.Select(c => c.text ?? string.Empty));
        var input = payload.usage?.input_tokens ?? 0;
        var output = payload.usage?.output_tokens ?? 0;
        return new LlmResult(text, payload.model ?? _model, input, output, input + output, sw.ElapsedMilliseconds);
    }

    public async IAsyncEnumerable<string> EnhanceStreamAsync(
        string rawNote,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var req = BuildRequest(rawNote, stream: true);
        using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await ReadLineWithStallGuardAsync(reader, ct);
            if (line is null) break;
            if (!line.StartsWith("data: ", StringComparison.Ordinal)) continue;

            var json = line.Substring(6).Trim();
            if (string.IsNullOrEmpty(json)) continue;

            var chunk = TryExtractTextDelta(json);
            if (!string.IsNullOrEmpty(chunk)) yield return chunk;
        }
    }

    private static async Task<string?> ReadLineWithStallGuardAsync(StreamReader reader, CancellationToken ct)
    {
        using var stallCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        stallCts.CancelAfter(TimeSpan.FromSeconds(StreamStallTimeoutSeconds));
        try
        {
            return await reader.ReadLineAsync(stallCts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"LLM stream stalled for {StreamStallTimeoutSeconds}s without a new chunk.");
        }
    }

    private HttpRequestMessage BuildRequest(string rawNote, bool stream)
    {
        var body = new
        {
            model = _model,
            max_tokens = MaxTokens,
            system = _systemPrompt,
            stream,
            messages = new[] { new { role = "user", content = rawNote } }
        };

        var req = new HttpRequestMessage(HttpMethod.Post, "/v1/messages");
        req.Headers.Add("x-api-key", _apiKey);
        req.Headers.Add("anthropic-version", AnthropicVersion);
        req.Content = JsonContent.Create(body);
        return req;
    }

    private static string? TryExtractTextDelta(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("type", out var typeEl)) return null;
            if (typeEl.GetString() != "content_block_delta") return null;
            if (!root.TryGetProperty("delta", out var delta)) return null;
            if (!delta.TryGetProperty("type", out var dType) || dType.GetString() != "text_delta") return null;
            return delta.TryGetProperty("text", out var textEl) ? textEl.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private record AnthropicResponse(string? model, AnthropicContent[] content, AnthropicUsage? usage);
    private record AnthropicContent(string? type, string? text);
    private record AnthropicUsage(int input_tokens, int output_tokens);
}
