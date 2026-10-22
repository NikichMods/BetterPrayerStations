# Better Prayer Stations — Working Contract

This project follows [NikichMods/DevRules](https://github.com/NikichMods/DevRules). Start with `WORKING_RULES.md` and its action-based reading map; local work also follows `CODEX_WORKFLOW.md`. This file adds project-specific requirements only.

## Identity and current phase

- Project: **Better Prayer Stations**.
- Repository: `NikichMods/BetterPrayerStations`.
- Initial target: Graveyard Keeper **1.407**, PC, BepInEx environment. Exact loader/runtime/build references remain to be verified.
- Purpose: useful prayer stations, optional faith spending on individual graves, and paid decoration appearance changes preserving quality.
- Current milestone: adopted product concept; first technical persistence research is recorded in `docs/PERSISTENCE_RESEARCH.md`. No production implementation, runtime acceptance, build, version or release exists yet. Bootstrap authorization does not authorize automatic production implementation.

## Sources of truth and recovery

Inspect actual checkout/branch/changes and read this contract, `docs/DESIGN.md`, `docs/IMPLEMENTATION_PLAN.md` and `docs/EVIDENCE_AND_ACCEPTANCE.md` and `docs/PERSISTENCE_RESEARCH.md` before substantive work. Update owning documents when accepted decisions or evidence change.

Shared host knowledge: [NikichMods/GraveyardKeeperResearch](https://github.com/NikichMods/GraveyardKeeperResearch), starting with `docs/RESEARCH_INDEX.md`. In particular consult `GRAVEYARD_RUNTIME.md`, `PERK_MECHANICS.md`, `CRAFTING_INVENTORY_AND_TRADING.md` and `UI_INPUT_TIME_AND_ENVIRONMENT.md` before new host inspection. Product/economy context is linked in the evidence document.

This repository owns this mod's product behavior, architecture, balance and acceptance. Shared research owns reusable host facts. Look up project docs -> shared facts -> accepted history -> fresh static inspection -> narrow probe only when needed. Do not duplicate a competing host knowledge base here.

## Product constraints

`docs/DESIGN.md` owns the adopted mechanics. Do not silently change ownership, shared price progression, coverage activation, partial anatomy bypass or appearance/stat separation. UI tabs, prices, radius and unlock details remain open. Do not promote illustrative preview values into requirements.

## Engineering and evidence

- Follow `ENGINEERING_RULES.md` before technical mechanism selection; apply `RUNTIME_INTEGRATION.md` to host integration and `RUNTIME_TEST_HARNESS.md` when supplying an installed probe.
- Prefer verified host seams; preserve native anatomy and original body data. A grave modifier must not become exported organ/zombie power.
- Never guess identifiers, serialization, installation/removal paths, rendering ownership or compatibility. Static plausibility and browser mockups are not installed-game proof.
- Save data must preserve per-grave investment/count and per-decoration appearance across supported lifecycles. Stable identity, migration and safe removal behavior need evidence before implementation is declared ready.
- Appearance fitting is a temporary view of the actual selected world grave, with cancellation restoring committed presentation and no payment until confirmation. No separate rendered scene is required by the product.
- No broad recurring world scans, reflection, logging or visual object recreation without verified need. Re-evaluate coverage on appropriate verified changes; exact mechanism remains open.
- Do not commit game binaries, extracted assets or copied/decompiled game source. Preserve minimal derived evidence and original code only.

## Git, builds and acceptance

- `main`: documented accepted product state and, later, accepted runtime source. Initial documentation can go directly to `main`.
- Use `codex/<topic>` for implementation/research branches. Unaccepted runtime work stays off `main`.
- Follow global immutable numbered-build/source/hash rules and explicit installed-runtime acceptance before stable promotion. GitHub Releases will be the stable binary download surface.
- Intended installed filename: `BetterPrayerStations.dll`; numbered handoff: `BetterPrayerStations-<version>.dll`. Select canonical semantic-version owner with the actual build system, before first binary; no version is consumed by bootstrap.
- No build/CI scaffolding at this documentation-only phase. Establish useful compile/handoff gates when executable source/toolchain exist, under `CI_POLICY.md`.
- MPL-2.0: see `LICENSE` and `LICENSING.md`; add SPDX notices when original source files are introduced.
