# Better Prayer Stations — Working Contract

This project follows [NikichMods/DevRules](https://github.com/NikichMods/DevRules). Start with `WORKING_RULES.md` and its action-based reading map; local work also follows `CODEX_WORKFLOW.md`. This file adds project-specific requirements only.

## Identity and current phase

- Project: **Better Prayer Stations**.
- Repository: `NikichMods/BetterPrayerStations`.
- Initial target: Graveyard Keeper **1.407**, PC, BepInEx environment. Exact loader/runtime/build references remain to be verified.
- Purpose: useful prayer stations, optional faith spending on individual graves, and paid decoration appearance changes preserving quality.
- Current milestone: adopted product concept; first technical persistence research is recorded in `docs/PERSISTENCE_RESEARCH.md`. No production implementation or release exists. The user authorized a narrow research-only appearance probe; its version/build/install state is owned by `docs/TEST_BUILD_LOG.md`, not a production release. The narrow 0.1.1 gravestone render/redraw/menu-load/close-cancel/cleanup scenario passed in the installed game with user visual confirmation; full product acceptance remains open. The 0.1.2 installed two-grave/two-slot removal/reinstallation/exchange/menu-load/cleanup scenario also passed with user visual confirmation; identical physical fence instances remain indistinguishable in that exchange. Full product acceptance remains open.

## Sources of truth and recovery

Inspect actual checkout/branch/changes and read this contract, `docs/DESIGN.md`, `docs/IMPLEMENTATION_PLAN.md` and `docs/EVIDENCE_AND_ACCEPTANCE.md` and `docs/PERSISTENCE_RESEARCH.md` and `docs/APPEARANCE_RENDERING_RESEARCH.md` and `docs/FAITH_IMPROVEMENTS_RESEARCH.md` before substantive work. Update owning documents when accepted decisions or evidence change.

Shared host knowledge: [NikichMods/GraveyardKeeperResearch](https://github.com/NikichMods/GraveyardKeeperResearch), starting with `docs/RESEARCH_INDEX.md`. In particular consult `GRAVEYARD_RUNTIME.md`, `PERK_MECHANICS.md`, `CRAFTING_INVENTORY_AND_TRADING.md` and `UI_INPUT_TIME_AND_ENVIRONMENT.md` before new host inspection. Product/economy context is linked in the evidence document.

This repository owns this mod's product behavior, architecture, balance and acceptance. Shared research owns reusable host facts. Look up project docs -> shared facts -> accepted history -> fresh static inspection -> narrow probe only when needed. Do not duplicate a competing host knowledge base here.

## Product constraints

`docs/DESIGN.md` owns the adopted mechanics. Appearance belongs to the grave; quality belongs to the physically installed item. No cosmetic item metadata or inventory-icon changes are required. Do not silently change ownership, shared price progression, coverage activation, partial anatomy bypass or appearance/stat separation. UI tabs, prices, radius and unlock details remain open. Do not promote illustrative preview values into requirements.

## Engineering and evidence

- Follow `ENGINEERING_RULES.md` before technical mechanism selection; apply `RUNTIME_INTEGRATION.md` to host integration and `RUNTIME_TEST_HARNESS.md` when supplying an installed probe.
- Prefer verified host seams; preserve native anatomy and original body data. A grave modifier must not become exported organ/zombie power.
- Never guess identifiers, serialization, installation/removal paths, rendering ownership or compatibility. Static plausibility and browser mockups are not installed-game proof.
- Save data must preserve per-grave investment/count and independent grave-owned gravestone/fence appearance choices across supported lifecycles. Stable identity, migration and safe removal behavior need evidence before implementation is declared ready.
- Appearance fitting is a temporary view of the actual selected world grave, with cancellation restoring committed presentation and no payment until confirmation. No separate rendered scene is required by the product.
- No broad recurring world scans, reflection, logging or visual object recreation without verified need. Re-evaluate coverage on appropriate verified changes; exact mechanism remains open.
- Do not commit game binaries, extracted assets or copied/decompiled game source. Preserve minimal derived evidence and original code only.

## Git, builds and acceptance

- `main`: documented accepted product state and, later, accepted runtime source. Initial documentation can go directly to `main`.
- Use `codex/<topic>` for implementation/research branches. Unaccepted runtime work stays off `main`.
- Follow global immutable numbered-build/source/hash rules and explicit installed-runtime acceptance before stable promotion. GitHub Releases will be the stable binary download surface.
- Intended installed filename: `BetterPrayerStations.dll`; numbered handoff: `BetterPrayerStations-<version>.dll`. Select canonical semantic-version owner with the actual build system, before first binary; no version is consumed by bootstrap.
- Faith diagnostic: `research/FaithProbe/Plugin.cs` Version is its canonical version owner; build gate `research/FaithProbe/build.ps1` validates exact native IL rewrites and compiled conversion against the native oracle. Installed `BetterPrayerStationsFaithProbe.dll`; handoff `BetterPrayerStationsFaithProbe-<version>.dll`. Independent research numbering; no production version consumed. Read its PLAN/README and TEST_BUILD_LOG before handoff. The first authorized slice is session-only, one grave, explicit activation, no payment/radius; Faith Probe0.1.1 passes this bounded native rating/diagram/activation/cleanup scenario with user visual confirmation. Faith0.2.0 now confirms saved investment through empty/replacement body menu-loads, replacement decay calculation and saved cleanup. Destination non-export has no captured live checkpoint; Bounded0.3.0 numeric wooden coverage/load and0.4.0 native debit/shared sequence/player recovery now pass; final coverage lifecycle/tier/payment policy remain open.
- Research probe build gate: `research/AppearanceProbe/build.ps1`, exact clean source for numbered artifacts, version assertions and native descriptor tests. Compiler: verified Windows SDK 6.0.406 Roslyn with local host references. Hosted CI is unselected because proprietary local references are already available for the meaningful local gate; do not upload them. Reassess production build/CI separately.
- Diagnostic identity exception: canonical probe version is `research/AppearanceProbe/Plugin.cs` constant Version; installed `BetterPrayerStationsAppearanceProbe.dll`, handoff `BetterPrayerStationsAppearanceProbe-<version>.dll`. A probe number does not consume the production mod version.
- Read `research/AppearanceProbe/PLAN.md` and README plus `docs/TEST_BUILD_LOG.md` before diagnostic handoff/testing; follow the local installed-runtime loop and never overwrite a loaded binary.
- MPL-2.0: see `LICENSE` and `LICENSING.md`; add SPDX notices when original source files are introduced.

Faith Probe0.2.0 extends the accepted input projection with diagnostic grave-owned C/L/schema fields and one-time post-start recovery. Read research/FaithProbe/README.md and PLAN.md for cleanup/save requirements and live acceptance boundaries. SavedFaithState.cs owns the diagnostic backend; LifecycleTests.cs verifies actual native getter IL and SmartSerializer on synthetic owners. Do not confuse offline relocation with native exhumation or this diagnostic format with a production schema. Exact artifact state remains in TEST_BUILD_LOG.md.

Coverage research/candidate owner: docs/COVERAGE_RESEARCH.md and Faith Probe0.3.0 README/PLAN. PrayerCoverage.cs provides diagnostic three-cell actual-station activation; no payment/tier cap or final radius is selected. Preserve0.2.0 accepted persistence and all older frozen identities.

Faith payment research/candidate: docs/FAITH_PAYMENT_RESEARCH.md and Faith Probe0.4.0 README/PLAN. FaithPurchase.cs owns native carried-faith transaction and bounded rollback; PurchaseTests.cs uses simulated debit explicitly. Test price N+1 is not balance. This probe spends real faith; cleanup does not refund it. Bounded installed0.4.0 native debit/shared sequence/player recovery passes; full limits and exact return are in TEST_BUILD_LOG.md.

Two-grave research/candidate: docs/TWO_GRAVE_ZONE_RESEARCH.md, Faith Probe0.5.1 README/PLAN. TwoGraves.cs bounds current owners at two; selection is editing only. Explicit closed-window native zone sample observes real OnQualityComputed, never a disabled-baseline zone event. Installed0.5.1 bounded two-owner prices/closed-zone sum/event now pass; paired live-load evidence and transient preview station-count defect remain open. Exact return: TEST_BUILD_LOG.md. No visual/balance scope added.

Current diagnostic Faith0.6.0: narrow one-station move/cancel correction with temporary radius contour and optional free C1/L1 test setup. See COVERAGE_RESEARCH.md and research/FaithProbe/README.md; old two-grave payment/zone acceptance retained. No product radius/visual/balance decision or paired-live-load acceptance implied.
