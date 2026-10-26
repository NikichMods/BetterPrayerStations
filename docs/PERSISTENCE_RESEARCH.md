> Historical combined-concept research. Its evidence remains reusable, but its services and old “current/next” statements are not the current product or work queue. See [PROJECT_STATE](PROJECT_STATE.md) and [history map](history/README.md).

# Persistence research — 2026-10-23

Current global blessing-capacity/position/order persistence is owned by `GLOBAL_BEAM_PERSISTENCE.md` (Menu0.3.0 native player-root schema, offline gates and pending live acceptance). The grave-owned appearance/C-L carriers below remain historical/reusable evidence for their own ownership, not the global field format or a migration source.

Status: **bounded static evidence plus passing offline native binary fixture**. Subsequent installed 0.1.1/0.1.2 probes confirm grave-owned appearance through menu-load, close-cancel, independent top/fence removal/reinstallation and distinguishable gravestone exchange on two occupied graves; their diagnostic fields were explicitly written and cleaned in the user save sessions. No production patch or full lifecycle acceptance exists. `DESIGN.md` owns product semantics; reusable host facts remain in [shared research](https://github.com/NikichMods/GraveyardKeeperResearch/blob/codex/graveyard-design-candidates/docs/GRAVEYARD_RUNTIME.md#grave-and-decoration-persistence--2026-10-23).

## Current question and result

Can one grave retain faith investment and two independent appearance choices while physical decorations remain ordinary items with native quality?

**Accepted ownership revision:** gravestone and fence appearances belong to the grave. Quality belongs to the actual installed item. This supersedes item-owned cosmetics and removes the need for custom item icons, tooltip badges or cosmetic transfer through removal recipes.

Native grave Item parameters remain the first mechanism to pursue. A separate coordinate-keyed ledger, cosmetic item-definition IDs or an item-owned appearance registry are not currently justified. The exact schema/code mapping is provisional; this is not a completed save-compatible mod.

## Evidence identity and method

- Target: GK 1.407 PC.
- `Assembly-CSharp.dll` SHA-256: `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`.
- `Assembly-CSharp-firstpass.dll` SHA-256: `9dc6def3b7715dd27eeb168ddc0af47e31c6f38d3fbee24bf592899392026498`.
- `resources.assets` SHA-256: `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`.
- Supporting source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, selected Item/GameRes/SerializableWGO/SerializableGameMap/WorldGameObject/CraftComponent/ResModificator/MultiInventory/GraveGUI and grave FlowCanvas nodes. Selected local IL corroborates ownership, restoration and transfer paths. Rendering investigation is recorded separately in `APPEARANCE_RENDERING_RESEARCH.md`.
- Prior native Item binary evidence from ResearchableAlchemyRecipes was reused. This project's fixture adds the actual map/WGO/grave/decoration container boundary; it does not replace the serializer.

## Grave storage and identity

`SerializableGameMap.SaveSceneToMe -> SerializableWGO.FromWGO` captures `unique_id` and `item=wgo.data`. Scene restore creates new Unity objects and restores their saved ID/Item before SetObject. A live Unity reference is transient; the saved grave Item can carry its own state without an external ID ledger.

`ReplaceWithObject -> SetObject -> InitNewObject` reuses the current WGO and Item. Updating Item definition ID/capacity does not clear parameters. The inspected chain is `grave_empty -> grave_corp -> grave_ground -> grave_exhume -> grave_empty`; exhumation drops/removes the body's inventory item, not grave data. Craft End removes zero parameters, not positive custom fields.

Research direction: namespaced schema marker, conversion/wreath/shared-payment counts and separate stable appearance codes for gravestone/fence on grave data. Missing/zero appearance means use the actual installed item's ordinary look. Keep coverage and effective score derived, and never store a station ID as investment ownership. Exact keys, code values, validation/migration and limits are not final.

An empty decoration slot retains its saved choice but displays nothing. A replacement item uses that choice with its own native quality. Moving an ordinary item to another grave does not export a cosmetic preference. Exhumation leaves both choices at the source grave. Installed 0.1.2 confirms retained choices across empty decoration slots, native removal/reinstallation, distinct gravestone exchange and menu-load on two occupied graves. Identical physical fences prevent individual provenance verification; exhumation and broader lifecycles remain untested.

## Decoration lifecycle under revised ownership

| Boundary | Current requirement |
| --- | --- |
| Install/reinstall | Keep ordinary item identity/quality; render that grave's choice only for a present supported slot |
| Remove | Native ordinary item result; grave choices remain; no cosmetic metadata transfer |
| Chest/other inventory | Ordinary items and native inventory presentation |
| Another grave | Destination grave choice applies, or ordinary item look if unset |
| Save/load | Preserve both grave choices even without installed decorations; no cross-save cache leakage |

Historical finding: all 42 checked GraveStone/GraveFence definitions have stack_count=1 and native set/remove recipes; removal generates a base-item template and does not copy arbitrary custom parameters. That is useful shared host evidence but **no longer a product blocker**. The former proposed removal-result interception and pending cosmetic-transfer state are superseded, not queued for implementation.

Historical trap: active craft `_current_item` restores from ID/durability, while consumed-items data is separate. It still matters to mods that depend on that service field; this product no longer needs it to preserve appearance. Prior item-owned fixture state/results are archived at source `b86ff41`, not current requirements or public save compatibility obligations.

## Movement boundary

Reused exact Move Objects 0.6.0 structural checkpoint, DLL SHA-256 `c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0`: its selected route moves the existing WGO. Grave-attached state therefore avoids a coordinate-keyed identity problem. Grave eligibility, cancellation, redraw, radius invalidation and move/save behavior remain untested; no blanket move-mod compatibility is claimed. Deletion/recreation and uninstall policy remain separate open cases.

## Current offline executable check

Original script: `research/persistence/native-serialization.ps1`, fresh PowerShell process, `-ManagedDirectory <local game Managed folder>`. No DLL, game process manipulation or save-file reads/writes.

Method: synthetic uninitialized Items avoid Unity Resources constructor calls. Native GameRes access and SmartSerializer round-trip a native SerializableGameMap with two WGO records: an occupied grave with investment and two appearance codes, and a separate empty grave with different codes. Ordinary decoration/body data and a chest Item are included. The generic serializer requires a class root, so the actual native containing map is used.

PASS: WGO ID; faith counters; independent gravestone/fence choices for both occupied and empty graves; no appearance on the ordinary decoration; chest decoration stays ordinary; durability and body separation; restored graph independence; missing-choice default. It proves the carrier can represent the revised ownership. It does not execute removal/replacement/movement, native visual rendering or a full live save cycle.

## Next useful slice

The bounded installed ownership/rendering/removal/reinstallation/menu-load slice now passes; exact limits belong to APPEARANCE_RENDERING_RESEARCH.md and TEST_BUILD_LOG.md. Do not repeat the same healthy occupied-grave loop or reopen item-owned metadata transfer. Select the next still-open lifecycle only when required by a concrete production slice (for example exhumation, whole-grave movement or fitting interruption). Full production readiness and broad save/compatibility acceptance remain open.

## Faith0.2.0 saved-investment evidence

Authorized next slice uses three diagnostic root-grave Item parameters (schema1, conversion capacity C, extra wreaths L); N=C+L is the shared successful-action sum. It is not a final production schema or price implementation. Existing accepted rating/diagram projection now reads the current body dynamically; empty graves retain native behavior. Post-start one-time recovery finds the single marked grave, validates state and logs its identity/counts. Explicit activation remains session-only and resets enabled on load; no payment or radius is implied.

New original LifecycleTests execute actual installed WorldGameObject.get_quality IL on synthetic owners/definitions, applying the real rewrite and shared saved-state backend with explicit headless authorization. PASS2700 combinations of skulls, decay, decoration, C/L and activation against independent synthetic native reference fixtures; original getter equivalence, no-body/generic/native craft exclusions and original body bytes checked. Actual SmartSerializer map roundtrips preserve occupied/empty/replaced grave IDs and investment; synthetic list relocation shows no body/organ export, current-red/decay recalculation, shared N retention, native positive-parameter pruning, cleanup and unrelated data preservation. Unknown schema and invalid counts are rejected.

Limits: synthetic definitions/owners and relocation, no Unity scene, actual exhumation/SetObject or live recovery/UI/save acceptance. Installed0.2.0 now confirms same grave47567/C3/L1/N4 after both empty and replacement-body menu-loads; empty rating remains native -2, decayed replacement raw4/3/0 at durability0.305293947 yields native -4/effective0. User confirms visuals. Initial five diagnostic actions preserve physical hash and pass native GUI observation; replacement GUI has no separate automated observation. Body/part probe fields on replacement are zero. No destination checkpoint was recorded, so live destination non-export is visually supported but not instrumented. Cleanup is followed by successful native save; no cleanup reload recorded. Exact identity/limits: TEST_BUILD_LOG.md installed return. Prior0.1.1 remains accepted for its bounded session-only behavior. Full production, payment, coverage, zone/hint refresh and migration remain open.
