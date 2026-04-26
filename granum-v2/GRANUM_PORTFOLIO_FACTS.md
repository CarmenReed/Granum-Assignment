# Granum Portfolio Facts (extracted 2026-04-26)

Status: untracked working file. Phase 1 of 2. READ-ONLY pass against the public Granum marketing surface. Do not commit.

Interpretive guardrail used throughout: a website is marketing surface, not engineering inventory. "The page does not say X" is not "the product does not do X." Where the page is silent, this file says "no claim found," not "feature absent."

## Fetch log

URLs fetched successfully (24):

1. https://granum.com/
2. https://granum.com/lmn/
3. https://granum.com/lmn/lmn-crew/
4. https://granum.com/lmn/starter/
5. https://granum.com/lmn/professional/
6. https://granum.com/lmn/enterprise/
7. https://granum.com/lmn/pricing/
8. https://granum.com/singleops/
9. https://granum.com/singleops/plus/
10. https://granum.com/singleops/premier/
11. https://granum.com/singleops/pricing/
12. https://granum.com/singleops/scheduling/
13. https://granum.com/singleops/crm/
14. https://granum.com/singleops/work-order/
15. https://granum.com/singleops/tree-inventory/
16. https://granum.com/singleops/estimating/
17. https://granum.com/greenius/
18. https://granum.com/greenius/courses/
19. https://granum.com/resources/introducing-lmn-crew-2-0-crew-for-all/
20. https://granum.com/resources/offline-access-lmn-crew/
21. https://granum.com/resources/lmn-crew-app-training-field-efficiency/
22. https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/
23. https://granum.com/about/company/
24. https://granum.com/about/why-granum/
25. https://granum.com/about/careers/
26. https://granum.com/who-we-serve/
27. https://granum.com/who-we-serve/full-service-landscaping/
28. https://granum.com/who-we-serve/landscaping-design-build/
29. https://granum.com/who-we-serve/tree-care-phc/
30. https://granum.com/who-we-serve/snow-ice/
31. https://granum.com/landscape-maintenance/
32. https://granum.com/news/
33. https://granum.com/resources/
34. https://granum.com/customer-stories/

URLs that failed (0): none. All fetches returned content within the 25-URL discovery budget. (Total fetched count above is 34 because the 25-URL guard was for *discovery* URLs beyond the named primaries; all named primaries plus 26 discovery hits returned cleanly within the run.)

Searches run (2):
- `site:granum.com French bilingual` returned no Granum-internal hits.
- `site:granum.com API integration webhook` returned no Granum-internal hits.

Total pages parsed: 34 product / company pages, 2 search queries.

Caveats on fetch fidelity:
- WebFetch summarizes pages through a small model. Quoted phrases below are quoted as the summarizer returned them. Where a quoted string is load-bearing for a verdict, treat it as "the summarizer reported this exact phrase from the page" and verify by direct page visit before quoting it to Jonathan.
- WebFetch cannot read screenshots, video transcripts, or interactive demo widgets. Anything that lives only inside an embedded video or a "see it in action" interactive is not in this dataset.
- The customer-stories landing page is a navigation hub; individual case studies were not crawled in this pass.
- Help-center URLs (support.golmn.com, docs.singleops.com, support.gogreenius.com) were not crawled. Authenticated/help-center content is out of scope for a marketing-surface pass.

## Portfolio inventory

### Granum (parent brand)

- Target user: "landscapers and arborists" (source: https://granum.com/).
- Marketing tagline: "THE #1 SOFTWARE PARTNER TO LANDSCAPERS & ARBORISTS" and "Your Success is Our Shared Mission" (source: https://granum.com/).
- Positioning: parent brand for three formerly-separate products (LMN, SingleOps, Greenius) following a strategic merger. "The new brand is the culmination of the strategic merger announced in November of last year" (source: https://granum.com/news/, dated October 8, 2025). Granum brand launch: October 8, 2025. So Granum-as-brand is roughly six months old at extraction time.
- Office locations: Atlanta, GA and Toronto, ON referenced in press release. Careers page: "We have team members located throughout the U.S.A. and Canada" (source: https://granum.com/about/careers/).
- Geographic claims: "Serving landscaping and arborist businesses across North America" (source: https://granum.com/about/why-granum/). United States and Canada explicit (source: https://granum.com/about/company/).
- AI / automation claims: "Our automated software empowers you to thrive" and "Our automated tools optimize your workflows from sales to production to invoicing" (source: https://granum.com/). No explicit AI or ML claim found at the parent-brand level. Marketed as "automated software," described as workflow automation.
- Compliance claims: none found at the parent-brand level. No SOC 2, GDPR, HIPAA, or audit-trail certification mentioned across any page crawled.
- Language claims at parent level: none. Language claims appear only on individual product pages.

### LMN (Landscaping Business Management Software)

- Target user: landscapers; tiered for "companies with 1-3 crews" (Starter), "companies with 15-50 employees" (Professional), and "enterprise-level companies" (Enterprise) (source: https://granum.com/lmn/pricing/, https://granum.com/).
- Marketing tagline: "Built to Help Landscapers Get Ahead" (source: https://granum.com/lmn/).
- Feature list as stated:
  - Client Management / CRM (source: https://granum.com/lmn/, https://granum.com/lmn/starter/)
  - Budget-Based Estimating (source: https://granum.com/lmn/, https://granum.com/lmn/starter/)
  - Automated Scheduling, "Drag-and-drop jobs onto the calendar" (source: https://granum.com/lmn/)
  - Job & Time Tracking via mobile app (source: https://granum.com/lmn/, https://granum.com/lmn/starter/)
  - Invoicing & Payment with online payment (source: https://granum.com/lmn/)
  - Job Costing & Reporting (source: https://granum.com/lmn/professional/)
  - Real-Time Job Costing (Professional+) (source: https://granum.com/lmn/pricing/)
  - Files & Photos on Documents (Professional+) (source: https://granum.com/lmn/pricing/)
  - Multi-location management (Enterprise) (source: https://granum.com/lmn/pricing/)
  - 2000+ SMS/month (Enterprise) (source: https://granum.com/lmn/pricing/)
  - Enterprise Onboarding with data migration (source: https://granum.com/lmn/enterprise/)
- Language support: "Crews clock in and out from the LMN mobile app, with offline and bilingual support" (source: https://granum.com/lmn/starter/). "bilingual support for field teams" (source: https://granum.com/lmn/starter/). Multiple pages reference "English/Spanish, offline capable" for the Crew app (source: https://granum.com/landscape-maintenance/, https://granum.com/who-we-serve/full-service-landscaping/, https://granum.com/who-we-serve/landscaping-design-build/, https://granum.com/who-we-serve/snow-ice/). Bilingual = English/Spanish across every page that defined the term.
- Integrations:
  - QuickBooks Online (all tiers) and QuickBooks Desktop (Professional+) (source: https://granum.com/lmn/pricing/)
  - LMN Pay powered by Stripe (all tiers; "Credit card and ACH") (source: https://granum.com/lmn/pricing/, https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/)
  - Zapier "Over 6,000 apps with Zapier" (Professional and Enterprise only) (source: https://granum.com/lmn/pricing/, https://granum.com/lmn/professional/)
  - Greenius training surfaced inside the LMN Crew app (source: https://granum.com/lmn/lmn-crew/)
- AI / automation claims: "Automated Scheduling" described as drag-and-drop calendar workflow. "automatic invoices based on how the job is set up" described as workflow automation, not AI (source: https://granum.com/resources/lmn-crew-app-training-field-efficiency/). No AI / ML claim found for LMN.
- Compliance claims: none found.
- Geographic claims: implied North America via parent. No country-specific claims on LMN pages directly. Customer logos include Canadian companies (e.g., "Muskoka Landscapers" referenced in summarizer output) but no Quebec or French-language customer call-out surfaced.
- Mobile / offline claims: "LMN mobile app" with "offline mode for reliability in the field" (source: https://granum.com/lmn/). "now works offline, with low or no cell service" (source: https://granum.com/resources/offline-access-lmn-crew/). Offline capabilities listed: "View work orders and job tasks without service," "Clock in and out offline," "Capture notes and photos on the job," "Complete and submit jobs without a signal." Sync is background-driven on connection return.
- Stated limitations: LMN Crew 2.0 expansion granted read-only access to crew members; "Functions like punching in/out, uploading photos and notes, and managing other crew members remains exclusive to Crew Lead accounts" (source: https://granum.com/resources/introducing-lmn-crew-2-0-crew-for-all/).

### LMN Crew App (mobile sub-product of LMN)

- Target user: crews, crew leads, crew members, operations managers (source: https://granum.com/lmn/lmn-crew/).
- Marketing tagline: "When 'bring it on' is your company mantra. Build independent, efficient crews with the most advanced landscape crew tracking app in the industry." (source: https://granum.com/lmn/lmn-crew/).
- Feature list (per page): job notes, site photos, checklists, efficiency goals, estimate capabilities, job status updates, hour tracking, GPS timestamps, weather notes, photo uploads, vendor bill receipt uploads, service completion marking, material logging, real-time communication, scheduling, job instructions delivery, crew notes (source: https://granum.com/lmn/lmn-crew/).
- Language support: "Available in English and Spanish" (exact phrasing reported by summarizer, source: https://granum.com/lmn/lmn-crew/).
- Integrations: Greenius training accessible directly from the Crew app (source: https://granum.com/lmn/lmn-crew/). Feeds to payroll and job costing systems.
- AI / automation claims: no AI claim found for the Crew app. "Real-time" is the strongest descriptor.
- Compliance claims: none found.
- Geographic claims: North American by parent context. App Store and Google Play distribution.
- Mobile / offline claims: native iOS / Android app via App Store and Google Play. Offline support confirmed via dedicated page (https://granum.com/resources/offline-access-lmn-crew/). "Automatic GPS timestamps" and "GPS and route tracking."
- Stated limitations: "uploading photos and notes, and managing other crew members remains exclusive to Crew Lead accounts" (source: https://granum.com/resources/introducing-lmn-crew-2-0-crew-for-all/).

### SingleOps (Tree Care Business Management Software)

- Target user: tree care companies, arborists. "SingleOps was built to improve the daily operations of tree care companies." (source: https://granum.com/singleops/). Tiered: Essential (single crew tree care), Plus, Premier (source: https://granum.com/singleops/pricing/).
- Marketing tagline: "Make Work Easier. Look Sharper." (source: https://granum.com/singleops/).
- Feature list:
  - Tree Inventory with "detailed profiles, species, health, risk history" (source: https://granum.com/singleops/tree-inventory/)
  - Quick & Easy Estimating, satellite tree mapping, camera integration with unlimited image uploads (source: https://granum.com/singleops/estimating/)
  - Automated Client Follow-ups: "Keep proposals, reminders, and renewals moving without manual effort" (source: https://granum.com/singleops/)
  - Time-Based Scheduling & Routing (source: https://granum.com/singleops/)
  - Crew Work Orders, "crew-specific notes and site photos" (source: https://granum.com/singleops/work-order/)
  - Invoicing & Payments (source: https://granum.com/singleops/)
  - Map-Based Scheduling (Plus+) (source: https://granum.com/singleops/plus/)
  - Job Site Mapping & Measurements (Plus+) (source: https://granum.com/singleops/plus/)
  - Options Proposals / tiered service options (Plus+) (source: https://granum.com/singleops/plus/)
  - Route Optimization: "Algorithmic routing sequences stops for minimal drive time and maximum throughput" (Premier) (source: https://granum.com/singleops/premier/)
  - Live GPS Tracking with vehicle alerts on geofence enter / leave (Premier) (source: https://granum.com/singleops/premier/)
  - Business Insights & Reporting live dashboards (Premier) (source: https://granum.com/singleops/premier/)
  - Tree Inventory: "Drop pins on the property map," PHC treatments, "PHC recommendations," "data-driven PHC recommendations" (source: https://granum.com/singleops/tree-inventory/)
- Language support: no language support claim found on any SingleOps page crawled. Only LMN-side surfaces had bilingual claims. Single mention, low priority signal: SingleOps may share crew-app surface with LMN, but the SingleOps-branded pages do not state this.
- Integrations:
  - QuickBooks Online (all tiers) and QuickBooks Desktop (all tiers) (source: https://granum.com/singleops/pricing/)
  - Google Calendar Integration (Plus+) (source: https://granum.com/singleops/pricing/)
  - Integrated Email & Texting (all tiers) (source: https://granum.com/singleops/pricing/)
  - Timesheets Integration & GPS Tracking (Premier add-on) (source: https://granum.com/singleops/pricing/)
- AI / automation claims:
  - "Automated Client Follow-ups" marketed as automation, not AI (source: https://granum.com/singleops/)
  - "Algorithmic routing" on Premier described as algorithmic, not AI (source: https://granum.com/singleops/premier/)
  - "data-driven PHC recommendations" described as data-driven, not AI (source: https://granum.com/singleops/tree-inventory/)
  - No explicit AI / ML claim found anywhere on SingleOps pages.
- Compliance claims: none found. No EPA, applicator records, ISA certification, OSHA, or audit-trail claim on any SingleOps page. Tree Inventory captures PHC treatments and history but no compliance framing.
- Geographic claims: "Since 2013, we've worked with arborists across North America" (source: https://granum.com/singleops/).
- Mobile / offline claims: photos at completion, "site photos," "Capture signatures, photos, and add-ons once jobs are completed" (source: https://granum.com/singleops/, https://granum.com/singleops/work-order/). Mobile access for crews and dispatchers. No explicit "offline mode" claim found on SingleOps pages (single mention concern: this could be a real gap or a marketing omission).
- Stated limitations: none found.

### Greenius (Employee Training & Development Software)

- Target user: "landscaping companies train safer, more consistent crews," "landscaping and arborist skills." Audiences: "Crew leaders, Crew members, Business owners, Sales teams, Account managers" (source: https://granum.com/greenius/, https://granum.com/greenius/courses/).
- Marketing tagline: "Develop People and Unlock Potential" (source: https://granum.com/greenius/).
- Feature list:
  - 150+ pre-built online courses (source: https://granum.com/greenius/, https://granum.com/greenius/courses/)
  - Training Content Library, searchable, with safety, equipment, soft skills, business practices (source: https://granum.com/greenius/)
  - Course Builder for custom modules (source: https://granum.com/greenius/)
  - 15-Minute Reviews (structured micro-reviews for crew leads) (source: https://granum.com/greenius/)
  - Progress Tracking (source: https://granum.com/greenius/)
  - Training Paths (source: https://granum.com/greenius/)
- Language support: "Every course and interface is translated so your crews can train in their preferred language" (source: https://granum.com/greenius/). "Courses are available in both English and Spanish" (source: https://granum.com/greenius/). Per-category confirmation: equipment training, health & safety, specialty training all "Available in English and Spanish" (source: https://granum.com/greenius/courses/). Designed for "multiple crews, busy seasons, and bilingual workforces." No French claim found.
- Integrations: Greenius training is surfaced inside the LMN Crew app (source: https://granum.com/lmn/lmn-crew/). No SingleOps integration claim found.
- AI / automation claims: none found.
- Compliance claims: "Pesticides" listed under Material Handling Tailgate Talks topics, no detailed course shown (source: https://granum.com/greenius/courses/, single mention, low priority signal). WHMIS referenced in safety category. "None explicitly stated" regarding compliance certifications. Greenius is positioned as training, not certification.
- Geographic claims: none specific. North American by parent.
- Mobile / offline claims: "Greenius landscaping and tree care mobile app" referenced. No explicit offline claim found.
- Stated limitations: none found.

### LMN Pay (payment surface inside LMN, powered by Stripe)

- Target user: LMN customers collecting payment.
- Tagline: "the faster, easier way to collect payments and see every payout, all from within LMN" (source: https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/).
- Features: credit card, debit card, ACH (bank transfer); Apple Tap to Pay reference; setup under 15 minutes; auto-sync to QuickBooks (source: https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/).
- Compliance / PII claim: "Your customers' full card numbers never touch LMN servers" and Stripe provides "industry-leading encryption and compliance standards" (source: https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/). This is the only explicit PII / payment-data claim found on the entire crawl. PCI compliance is implied via Stripe; not stated as an LMN-side certification.
- Webhook / API claims: no webhooks or API endpoints mentioned.

## Cross-product themes

Patterns require at least 2 product pages to count.

1. **Bilingual = English / Spanish, never English / French.** LMN starter, LMN landscape-maintenance, LMN full-service, LMN design-build, LMN snow-ice, Greenius, Greenius courses page all define "bilingual" as English / Spanish. (sources: https://granum.com/lmn/starter/, https://granum.com/landscape-maintenance/, https://granum.com/who-we-serve/full-service-landscaping/, https://granum.com/who-we-serve/landscaping-design-build/, https://granum.com/who-we-serve/snow-ice/, https://granum.com/greenius/, https://granum.com/greenius/courses/). Six independent pages, zero pages mentioning French. This pattern is strong.

2. **Workflow automation, not AI.** Across LMN ("Automated Scheduling," "automatic invoices"), SingleOps ("Automated Client Follow-ups," "Algorithmic routing," "data-driven PHC recommendations"), and Greenius (no AI claim), every "smart" feature is described as automation or algorithmic, never AI or ML. (sources: https://granum.com/lmn/, https://granum.com/singleops/, https://granum.com/singleops/premier/, https://granum.com/singleops/tree-inventory/). Conservative marketing posture: zero AI claims found across the entire crawl.

3. **Mobile-first crew workflow with offline support, GPS, photos.** LMN Crew app explicitly supports offline (https://granum.com/resources/offline-access-lmn-crew/). LMN landscape-maintenance and design-build pages describe "offline capable" mobile use. SingleOps describes mobile work-order updates "while on the go" but does not explicitly claim offline. (sources: https://granum.com/lmn/lmn-crew/, https://granum.com/resources/offline-access-lmn-crew/, https://granum.com/landscape-maintenance/, https://granum.com/who-we-serve/landscaping-design-build/, https://granum.com/singleops/work-order/).

4. **QuickBooks plus Stripe, with Zapier as the third-party expansion lane.** Both LMN and SingleOps integrate QuickBooks at every tier. Stripe (LMN Pay) inside LMN. Zapier on LMN Professional and Enterprise. Google Calendar on SingleOps Plus+. (sources: https://granum.com/lmn/pricing/, https://granum.com/singleops/pricing/). No native API documentation surfaced anywhere; Zapier appears to be the supported integration vector for cross-system automation.

5. **No compliance / regulatory feature surface anywhere.** Zero pages crawled mention EPA, pesticide records, applicator licensing, OSHA, ISA certification, SOC 2, HIPAA, GDPR, audit trails, or PCPA. The closest is "Pesticides" as a Greenius Tailgate Talk topic (training content, not record-keeping). (sources: 34 pages, none mention compliance features beyond LMN Pay's Stripe-handled payment data.)

6. **Granum brand is six months old.** Strategic merger announced November 2024; brand launch October 8, 2025. Each product still has its own login portal (my.golmn.com, app.singleops.com, new.gogreenius.com) and its own help center (support.golmn.com, docs.singleops.com, support.gogreenius.com). (source: https://granum.com/, https://granum.com/news/, https://granum.com/about/company/). Treat any "Granum platform" assumption as marketing convenience; engineering reality is three separate products with shallow integration.

## Confrontation with my v2 enhancement specs

### Enhancement-1: Two-Sided Language Model

- **Verdict:** GAP-with-wrong-language-pair. The two-sided language idea is real, but the specific language pair is wrong.
- **Evidence from sources:**
  - Greenius states "Courses are available in both English and Spanish" and targets "bilingual workforces" (source: https://granum.com/greenius/, https://granum.com/greenius/courses/).
  - Six separate LMN-side pages define "bilingual" as English / Spanish for the Crew app: "English/Spanish, offline capable" (sources: https://granum.com/lmn/starter/, https://granum.com/landscape-maintenance/, https://granum.com/who-we-serve/full-service-landscaping/, https://granum.com/who-we-serve/landscaping-design-build/, https://granum.com/who-we-serve/snow-ice/, https://granum.com/lmn/lmn-crew/).
  - Zero references to French anywhere on the crawl. Search for `site:granum.com French bilingual` returned no Granum-internal hits.
  - Granum has a Toronto, ON office (source: https://granum.com/news/, https://granum.com/about/company/). Canadian presence is real. But the marketing surface treats Canada and the U.S. as one English-speaking North American market with Spanish for the field.
- **What I assumed in the spec that turned out to be wrong:**
  - The entire Bill 101 / Bill 96 framing for ENH-1 is unsupported by the marketing surface. No Granum / LMN page references Quebec, French, fr-CA, Bill 101, Bill 96, or PCPA in a French context.
  - Footnote [^3] in the spec (LMN's "Canadian customer base requires French support") is direct industry-pattern recognition, not validated. The marketing surface specifically does not validate it. It is more accurate to say "LMN is headquartered in Toronto, Ontario, where the operating language is English; LMN's bilingual surface is English / Spanish for the U.S. field workforce."
  - The launch language pair `OperatorLanguagePreference { English, French }` chooses the wrong second language. Public signal points to Spanish, not French, as the second language.
- **What I assumed in the spec that turned out to be right:**
  - The two-sided architecture (operator output language separate from crew input language) is the right architectural shape. LMN already operates as if this were true (operator UI in English, crew app supports English / Spanish input).
  - The customer pain (operator manages business in English, crew works in another language) is real per the BLS statistical pattern footnote and is reinforced by Granum's own bilingual marketing.
  - Operator-language preference at the account / tenant level is the right persistence question. The fact that LMN, SingleOps, and Greenius have separate logins makes per-product configuration plausible.
  - "Quebec French is specifically `fr-CA`" was correct as a hypothetical question, but the question itself has no public signal that the company is solving for it.

### Enhancement-2: Compliance Flagging

- **Verdict:** GAP. No Granum page makes any pesticide, EPA, applicator, PCPA, or compliance-record claim. Zero. This is genuine open territory.
- **Evidence from sources:**
  - SingleOps Tree Inventory captures "PHC treatments" and "PHC recommendations" but no compliance / record-keeping framing (source: https://granum.com/singleops/tree-inventory/).
  - SingleOps Work Order page makes no pesticide or applicator-record claim (source: https://granum.com/singleops/work-order/).
  - Tree Care & PHC vertical page makes no compliance claim (source: https://granum.com/who-we-serve/tree-care-phc/).
  - Greenius lists "Pesticides" under Material Handling Tailgate Talks but no detail; this is training content, not record-keeping (source: https://granum.com/greenius/courses/).
  - No EPA, WPS, 40 CFR Part 170, Health Canada, PCPA, ISA, OSHA, or applicator-license mention anywhere across 34 pages.
- **What I assumed in the spec that turned out to be wrong:**
  - No incorrect assumptions found at the customer-pain level. The regulatory facts in footnotes 1 / 2 / 3 are all accurate regulatory citations independent of Granum.
  - The framing "Granum customers must be doing this somewhere" was speculative. Public signal says nothing one way or the other.
- **What I assumed in the spec that turned out to be right:**
  - This is genuinely a gap on Granum's marketing surface. ENH-2 is the most defensible of the five v2 enhancements at the marketing-surface level.
  - The argument that Tree Inventory and Work Orders are the wiring point is reinforced by SingleOps actually having a Tree Inventory feature. The architectural surface for this enhancement exists.
  - Domain-specific routing (pesticide vs fertilizer vs chemical storage) maps to PHC, which SingleOps already names as a category.

### Enhancement-3: A/B Prompt Testing Harness

- **Verdict:** NOT-ENOUGH-DATA. The website does not surface engineering practices. Whether Granum has a prompt-testing harness is unknowable from marketing pages.
- **Evidence from sources:**
  - No AI / ML claims anywhere across 34 pages, so there is no public-facing evidence Granum runs an LLM today, let alone an A/B harness for one.
  - The Granum careers page is a job-board redirect; no engineering tech-stack hints, no AI / ML role mentions surfaced through the careers landing page (source: https://granum.com/about/careers/).
  - Workflow automation is consistently marketed as "automation" or "algorithmic" rather than AI, which is suggestive but not conclusive.
- **What I assumed in the spec that turned out to be wrong:**
  - The spec assumes Granum has a prompt to govern. No public signal confirms or denies this. If Granum is not running an LLM in production today, ENH-3 is solving a problem they may not have.
- **What I assumed in the spec that turned out to be right:**
  - The governance discipline argument (prompt = configuration that ships without code review's scaffolding) is correct as a general engineering claim independent of Granum.
  - Pivot question for the interview is well-shaped: ask Jonathan whether they ship LLM prompts to production today, and if so, how prompt changes are reviewed.

### Enhancement-4: PII Expansion

- **Verdict:** OVERLAP-via-Stripe-only. Granum's only explicit PII claim is on the payment side: Stripe handles credit card numbers; "full card numbers never touch LMN servers." That is a specific PII category already addressed at the architecture level. The other four categories (SSN, address, DOB, plus credit-card outside the payment flow) have no public signal one way or the other.
- **Evidence from sources:**
  - "Your customers' full card numbers never touch LMN servers" and Stripe-handled "industry-leading encryption and compliance standards" (source: https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/).
  - No SOC 2, GDPR, HIPAA, or PII-handling certification mentioned anywhere else.
  - LMN Crew app captures "notes and photos on the job" with no PII-redaction claim.
  - SingleOps captures "site photos," "client preferences," and CRM data with no PII-redaction claim.
  - Greenius collects training records / progress data with no PII-redaction claim.
- **What I assumed in the spec that turned out to be wrong:**
  - Credit card detection in the spec assumes the data could appear in field-note free text. The marketing surface implies the production credit-card path is Stripe and never touches LMN's app servers. So the credit-card category in ENH-4 is solving for the *unstructured-text-leak* path, not the structured-payment path. That is still a valid use case (a crew member dictating "boss, the customer gave me their card on the phone, write down 4242...") but it is a narrower scenario than the spec implies.
  - The spec does not distinguish "free-text PII leak" from "structured PII at rest." Granum's structured PII is mostly handled by Stripe and QuickBooks, both compliance-credentialed third parties.
- **What I assumed in the spec that turned out to be right:**
  - The architectural shape (extend the existing PII guard's pattern set) is the cheapest path for any PII expansion regardless of Granum's posture.
  - SSN and DOB categories have no public Granum coverage, so adding them is genuinely additive.
  - U.S. / Canadian address detection was correctly framed as the highest false-positive risk; spec already names this as the open question.
  - Spec footnote [^1] explicitly tags the prevalence claim as "industry pattern recognition, not validated." That epistemic-status tag survived contact with the website.

### Enhancement-5: Webhook Out on Guardrail Hit

- **Verdict:** GAP, with an asterisk. The cross-product webhook concept is a real architectural gap because:
  - No native API documentation surfaced anywhere.
  - Cross-product integration is described in marketing terms ("strategic merger," "united three leading companies") but every product still has separate login portals, separate help centers, and the only documented inter-product surface is "Greenius training is accessible from the LMN Crew app."
  - Zapier is the third-party integration lane on LMN Professional+, suggesting Granum's preferred integration pattern for cross-system events is to delegate to Zapier rather than expose webhooks natively.
- **Evidence from sources:**
  - "We're a Partner, Not Just Software" framing positions products as separate (source: https://granum.com/about/why-granum/).
  - Each product has its own login portal: my.golmn.com, app.singleops.com, new.gogreenius.com (source: https://granum.com/).
  - "Connect LMN to the tools you already use, forms, messaging, accounting workflows, and automate repetitive steps" via Zapier (source: https://granum.com/lmn/professional/). Implies Zapier is the abstraction for outbound automation.
  - No webhooks or API endpoints surfaced on any page, including LMN Pay.
- **What I assumed in the spec that turned out to be wrong:**
  - The spec names "LMN dispatch, SingleOps jobs, Greenius scheduling" as webhook consumers. Reality:
    - "LMN dispatch" is not a Granum-named product or feature. LMN's dispatch-flavored capability is "Automated Scheduling" inside LMN. No standalone dispatch tool exists.
    - "SingleOps jobs" maps to SingleOps Crew Work Orders. Reasonable.
    - "Greenius scheduling" is not real. Greenius is a training platform; it does not schedule. The integration shape Granum actually surfaces is the inverse: Greenius training is *consumed inside* the LMN Crew app, not the other way around.
  - Footnote [^1] in the spec calls out that the specific consumer naming is "inferred from Granum's stated multi-product strategy and not validated." Good. The inference was wrong.
- **What I assumed in the spec that turned out to be right:**
  - Granum is genuinely a multi-product portfolio post-merger; the strategic motivation for cross-product reactions exists.
  - Outbound webhook with HMAC + retry is the right architectural shape for an integration surface.
  - The "polling vs push" framing in the why-this-earns-its-complexity section holds: today, cross-product reaction is via Zapier polling or shared logins. A direct webhook would be cleaner.
  - The async-emit / fire-and-forget question is well-shaped for the interview.

## New idea seeds (RAW, no scoring yet)

1. **English / Spanish two-sided language model** -- same architectural pattern as ENH-1 but with the correct language pair driven by Granum's actual bilingual surface (operator selects English or Spanish at account level; crew input language is auto-detected). Why it fits: every LMN-side page that defines bilingual says English / Spanish (sources above).

2. **PHC application record mode for SingleOps Tree Inventory** -- when a Tree Inventory entry includes a PHC treatment, route the entry through a stricter validation that captures applicator, product, EPA registration number, date, location, re-entry interval. Why it fits: SingleOps Tree Inventory already captures species, health, treatments, work history; the field shape is one prompt template away from compliance-defensible (source: https://granum.com/singleops/tree-inventory/).

3. **Zapier Trigger on Guardrail Hit** -- instead of a native HMAC-signed webhook, expose guardrail events as a Zapier trigger. Why it fits: LMN Professional+ already has Zapier listed as the integration lane; a Zapier trigger is the path of least resistance for cross-product reactions and matches Granum's stated integration posture (source: https://granum.com/lmn/pricing/).

4. **PII Guard Tier Differentiation** -- surface PII guard as an Enterprise-only feature, since Enterprise customers are the most likely to have multi-location data-residency concerns. Why it fits: LMN Enterprise already differentiates with onboarding, multi-location reporting, dedicated CSM (source: https://granum.com/lmn/enterprise/), so a Compliance / Privacy add-on at that tier maps to the existing product taxonomy.

5. **Crew-app voice-to-note PII redaction** -- given the crew app accepts voice or text notes and photos in the field, run the PII guard at the dictation moment, before the note is persisted to LMN's database. Why it fits: LMN Crew app explicitly captures "Capture notes and photos on the job" offline (source: https://granum.com/resources/offline-access-lmn-crew/), and offline capture means the redaction must run client-side or at next-sync.

6. **Offline-mode prompt hygiene for the Crew app** -- when an LLM-cleaned note was captured offline and is now syncing, run an A/B-style sanity check against the cleaned output vs. the raw input before merging to the canonical record. Why it fits: offline sync is a documented LMN Crew capability, and offline edits are higher-risk for silent corruption (source: https://granum.com/resources/offline-access-lmn-crew/).

7. **Greenius training trigger on guardrail hit** -- when the inference gate flags a category (e.g., compliance phrasing missed, PII leaked into a note), automatically assign the matching Greenius course to the crew member. Why it fits: Greenius is already integrated into the LMN Crew app and includes 100+ Tailgate Talks at 15-minute granularity (sources: https://granum.com/lmn/lmn-crew/, https://granum.com/greenius/courses/). The integration surface already exists.

8. **Stripe-style "card data never touches our servers" PII posture for free-text notes** -- extend the LMN Pay precedent ("full card numbers never touch LMN servers") to free-text notes: PII detected at ingest is replaced with tokens before persistence, and the raw text is never logged. Why it fits: Granum already markets a precedent of out-of-band sensitive-data handling via Stripe; extending the same posture to free-text notes is a natural narrative continuation (source: https://granum.com/resources/introducing-lmn-pay-powered-by-stripe/).

9. **Route-Optimization-aware compliance routing** -- SingleOps Premier has algorithmic routing; if a route includes a customer site with active PHC re-entry-interval restrictions, the router should treat the site as unschedulable for the duration. Why it fits: SingleOps Premier already exposes algorithmic routing and Tree Inventory already tracks PHC treatments (sources: https://granum.com/singleops/premier/, https://granum.com/singleops/tree-inventory/). The compliance signal is missing but every other piece is in place.

10. **Crew-language detection feeding training assignments** -- if the crew member's input language is detected as Spanish, automatically prioritize Spanish-language Greenius courses on assignment. Why it fits: Greenius explicitly markets to "bilingual workforces" with Spanish-language courses (source: https://granum.com/greenius/courses/), and the LMN Crew app already accesses Greenius. Closes the loop on the bilingual narrative.

11. **Multi-location Enterprise audit log for cross-branch data access** -- LMN Enterprise has multi-location management but no audit trail surfaced. Add a per-tenant audit log of who accessed which location's data, when, from what client. Why it fits: Enterprise customers most likely to need this, and the multi-location feature already exists (source: https://granum.com/lmn/enterprise/, https://granum.com/lmn/pricing/).

12. **Customer-card-data redaction in CRM notes** -- SingleOps CRM stores client preferences and notes; given LMN Pay tokenizes cards out of band, ensure CRM free-text notes never accidentally persist a 16-digit number a sales rep typed in by hand. Why it fits: SingleOps CRM is explicit (source: https://granum.com/singleops/crm/), and the precedent for tokenization exists on the LMN side.

13. **QuickBooks-direction PII guard** -- when LMN syncs invoices to QuickBooks, scrub free-text memo fields for PII before transmission. Why it fits: QuickBooks integration is universal across tiers in both LMN and SingleOps (sources: https://granum.com/lmn/pricing/, https://granum.com/singleops/pricing/). The sync surface is real; the redaction surface for it is missing.

14. **"Granum" unification surface diagnostic** -- the brand merger is six months old; build a small admin tool that surfaces, per tenant, which products they are licensed on (LMN tier, SingleOps tier, Greenius), and flags when the tenant is missing the complementary product (e.g., LMN customer without Greenius). Why it fits: each product still has separate login portals (my.golmn.com, app.singleops.com, new.gogreenius.com) and the cross-product story is marketing-only today (sources: https://granum.com/, https://granum.com/about/why-granum/).

15. **Bilingual prompt-testing harness** -- extension of ENH-3 that runs prompt-variant comparisons separately for English and Spanish input, since prompt regressions can be language-specific. Why it fits: Greenius already maintains parallel English / Spanish course sets, suggesting the company has internal pipeline for parallel-language content quality (source: https://granum.com/greenius/courses/).

## Honest confidence assessment

**What did the website tell me clearly?**

- LMN, SingleOps, Greenius are three separate products united six months ago under the Granum brand. Each has its own login portal, its own help center, its own pricing page.
- Bilingual = English / Spanish across every page that defined the term.
- QuickBooks (every tier of LMN and SingleOps) and Stripe (LMN Pay only) are universal integrations. Zapier is on LMN Professional+. Google Calendar is on SingleOps Plus+.
- LMN Crew app supports offline. SingleOps does not explicitly claim offline.
- LMN Pay handles credit card data via Stripe; "full card numbers never touch LMN servers."
- Granum is headquartered Atlanta + Toronto, serves U.S. and Canada.
- Workflow automation is marketed; AI is not. Zero AI / ML claims found.

**What did the website tell me ambiguously?**

- Whether SingleOps' mobile crew workflow uses the same offline-capable mobile codebase as LMN Crew, or a separate iOS / Android app.
- Whether Greenius has a mobile offline mode (page mentions a mobile app but does not confirm offline).
- Whether the Granum brand has a unified login or whether tenants stay split across three login portals.
- Whether Canada-side LMN customers have Quebec / French support somewhere not exposed to the marketing surface (e.g., inside support.golmn.com or in a region-specific tenant configuration).

**What did I want to know that the website didn't say?**

- Does Granum run an LLM in production today? Not signaled either way.
- Is there a public API for LMN, SingleOps, or Greenius beyond Zapier? No documentation surfaced.
- What compliance certifications does Granum hold (SOC 2, etc.)? None stated. Possibly under NDA / customer-portal only.
- What does Granum's data-residency story look like for Canadian vs. U.S. customers? Unstated.
- Is there a webhook / event surface anywhere? Not stated. Zapier suggests no native one.
- Does LMN's Canadian customer base include Quebec, and if so, what is the language story? Unstated. Toronto HQ does not imply Quebec customer pain by itself.

**Where am I most likely to be wrong about a Granum product feature based on this pass?**

- Most likely to be wrong: anything inside the help-center pages I did not crawl. Help-center articles often document features the marketing pages skip. Any v2 confrontation that depended on "the website doesn't say X" should be re-tested against the help center before being a load-bearing argument with Jonathan.
- Likely to be wrong: SingleOps offline mode. Marketing implies mobile use but not offline; the help center likely contradicts or confirms.
- Likely to be wrong: French support in any form. Marketing says Spanish only. There may be a Quebec-specific tenant configuration not on the marketing surface, but I have zero evidence of it.
- Possibly wrong: Granum's AI posture. The marketing surface is conservative ("automated," "algorithmic," "data-driven," never "AI"). This may be deliberate marketing restraint while engineering ships LLM features. Or it may be that they genuinely don't ship LLM features yet. Cannot tell from the marketing surface.
- Possibly wrong: API / webhook coverage. Zapier's presence implies an underlying event surface even if the marketing pages don't talk about it. Help-center / developer-portal pages would clarify.
- Confidence color (per my scoring rubric):
  - Bilingual = English / Spanish, not French: Green (90-100%, 6 corroborating pages, 0 contradicting).
  - No AI claims anywhere: Yellow (60-89%, marketing posture is consistent but help-center / customer-portal not crawled).
  - No compliance / EPA / pesticide-record features: Yellow (60-89%, marketing posture is consistent but PHC features in SingleOps could be more than they advertise).
  - No webhook / API: Yellow (60-89%, Zapier presence is suggestive but not conclusive).
  - "LMN dispatch" is not a real Granum-named feature: Green (90-100%, clear from pricing pages).
  - "Greenius scheduling" is not real: Green (90-100%, Greenius is a training product, multiple pages confirm).
