// ENHANCEMENT-5: Prompt A/B testing harness (CONDITIONAL)
// Spec: granum-v2/specs/05-prompt-ab-testing-harness.md
// Status: STUB ONLY. Not wired to runtime. Not part of build.

namespace Api.Models;

/// <summary>
/// One side of an A/B prompt comparison. Id is a stable identifier
/// (commit hash, version tag, or hand-assigned label); Name is the
/// human-readable label shown in diff reports; FilePath points to
/// the prompt template on disk.
/// </summary>
/// <param name="Id">Stable identifier for the variant.</param>
/// <param name="Name">Human-readable label.</param>
/// <param name="FilePath">Path to the prompt template file.</param>
public record PromptVariant(
    string Id,
    string Name,
    string FilePath);
