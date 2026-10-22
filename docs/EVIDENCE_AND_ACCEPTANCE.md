# Evidence and acceptance

## Current evidence boundary

As of 2026-10-23: adopted concept, bounded shared static host evidence and schematic browser interaction checks. **No production game implementation, installed UI/persistence acceptance or compatibility acceptance exists.** The research-only appearance probe is tracked separately in [the test-build log](TEST_BUILD_LOG.md); its compiled/installed state must not be called runtime acceptance. The subsequent [persistence research](PERSISTENCE_RESEARCH.md) adds bounded static lifecycle findings and a passing offline native map/item binary fixture. Appearance ownership was subsequently revised to grave-owned slot choices; item-owned cosmetic round trips are historical research, not current requirements. This does not establish installed-game persistence or cosmetic rendering.

## Shared evidence owners

Project origin: [GraveyardKeeperResearch](https://github.com/NikichMods/GraveyardKeeperResearch) branch `codex/graveyard-design-candidates`, source snapshot `5846cac5dc62578166c741e43894d8e7558024c7`. Shared documents below are pinned for bootstrap provenance; consult their current canonical versions before fresh technical work.

- [Graveyard runtime](https://github.com/NikichMods/GraveyardKeeperResearch/blob/5846cac5dc62578166c741e43894d8e7558024c7/docs/GRAVEYARD_RUNTIME.md): GK 1.407 prayer-station IDs, native rating and bounded cosmetic seams. Same host inputs identified by hashes in that owner document. Does not prove the proposed mod.
- [Player needs](https://github.com/NikichMods/GraveyardKeeperResearch/blob/5846cac5dc62578166c741e43894d8e7558024c7/docs/GRAVEYARD_PLAYER_NEEDS.md): qualitative anatomy/aesthetic motivations; no measured prevalence or proven market size, and no exact buried-organ-inspection demand established.
- [Economy](https://github.com/NikichMods/GraveyardKeeperResearch/blob/5846cac5dc62578166c741e43894d8e7558024c7/docs/ECONOMY_PROGRESSION_RESEARCH.md): income/resource progression assumptions. No universal surplus or fixed calendar-to-faith conversion.
- [Cultist](https://github.com/NikichMods/GraveyardKeeperResearch/blob/5846cac5dc62578166c741e43894d8e7558024c7/docs/PERK_MECHANICS.md): native organ quality visibility boundaries.
- [Research index](https://github.com/NikichMods/GraveyardKeeperResearch/blob/5846cac5dc62578166c741e43894d8e7558024c7/docs/RESEARCH_INDEX.md): inventory, UI/input/time and other reusable host facts.

Bounded host findings: native grave rating depends on decoration, red and available white skulls; grave-only modifiers must not be presented as physical body edits. Grave Item parameters are the current carrier for both faith investment and two grave-owned appearance choices. The updated offline native map fixture passes for independent occupied/empty grave choices and ordinary decorations. Item-owned cosmetic transfer is superseded. [Rendering research](APPEARANCE_RENDERING_RESEARCH.md) identifies a descriptor-projection seam; native refresh/close/load behavior and installed visuals remain unverified.

The earlier browser sketch exercised shared prices/caps, no-coverage dormancy, wooden reactivation above cap, free fitting/cancel and paid appearance changes preserving score. It used schematic shapes and fabricated example state. It proves only local interaction logic, not game rendering, serialization, Cultist behavior or mod compatibility. It is not shipped source or a native UI specification.

## Required representative acceptance, when implemented

| Property | Representative evidence |
| --- | --- |
| Free contents inspection | Actual buried inventory shown in radius; no exhumation/payment and no added per-organ skull disclosure |
| Shared progression | Both action types advance one grave counter; rebuild/action switch/body replacement do not reset |
| Effective ownership | Raw body/organs unchanged; moving body exports no grave modifier; empty grave has no bonus contribution |
| Replacement bounds | New body uses stored investment; insufficient/no red skulls do not become free white capacity |
| Coverage | None -> dormant; either tier -> all active; wood above purchase cap still activates all; strongest overlap permits purchases |
| Faith spending | Exact displayed successful action charged once; failure/cancel does not consume; actual before/after rating agrees |
| Save lifecycle | Investment/count, grave identity and appearance survive supported save/load transitions |
| Grave-owned appearance | Two slot choices survive grave save/load and decoration removal/reinstallation; an empty slot stays empty; moved ordinary items use destination grave choices; quality follows the actual installed item |
| World fitting | Actual target changes temporarily; cancellation/closing/interruption restores; neighbors and original item data unaffected |
| Cosmetic payment | Confirmation commits grave-slot look and correct cost once; browsing/cancel is free; physical item data and effectiveness unchanged |
| UI/input | Chosen native layout readable and operable for agreed input devices, including dormant-state explanations |
| Integration | Relevant native life cycles preserve adjacent behavior; inspect concrete Move Object/stacking mods before claiming support |

Before supplying any numbered binary, create a test-build record with semantic version, exact source SHA/frozen ref, artifact hash, build command/toolchain and requested scenario. Keep logs bounded and support-useful. Explicit user acceptance is required for stable promotion under DevRules. No candidate/release version is assigned now.
