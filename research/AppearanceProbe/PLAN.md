# Appearance probe — research-method checkpoint, 0.1.2

Question: do independent grave-owned top/fence appearances survive **native removal, reinstallation and exchange of ordinary physical decorations between two graves**, with empty slots invisible and actual item quality preserved?

Existing evidence: 0.1.1 exact artifact passed one occupied grave's gravestone projection, explicit redraw, menu-load, close-cancel and cleanup. Static/native-serializer findings establish two grave fields are representable and native removal generates ordinary base items. They do not prove installed two-slot lifecycle. Reusing 0.1.1 alone cannot supply fence projection or two live marked graves. Therefore extend its same narrow descriptor mechanism and add passive checkpoints; use direct native gameplay for the operations being tested. A craft/transfer emulator or direct inventory mutation would add assumptions and bypass the tested boundary, so neither is introduced. Full production appearance remains BLOCKED pending evidence.

| Case | Native path / observation | Expected | Cleanup |
| --- | --- | --- | --- |
| Register A/B and separate top/fence commits | Four known stage1 resources and native WGO redraw | Distinct committed grave choices; immediate redraw preserves measured actual state | Clear both |
| Remove top only, then fence | Ordinary native removal craft, pickup, passive checkpoint | Real slots disappear individually; saved codes stay; ordinary returned inventory items have no diagnostic fields | Reinstall |
| Reinstall at A | Native set recipes and their own redraw | Both original A choices return without another commit | Clear both |
| Empty both, exchange physical items | Native remove/set paths, actual inventory IDs/durability/quality, passive checkpoint | Source codes stay; destination preferences apply to incoming items; quality follows real items | Physical positions remain as player arranged |
| Save/menu-load modified inventories | Native map save/restore and one bounded recovery pass | Both IDs and all four appearance codes recover; expected WOPs and measured native state | Clear both then save |
| Cleanup | Zero only own three fields, one redraw per frame | Ordinary visuals for current physical items; no item undo/refund | Save |

No synthetic item setup, native craft patch, transfer interception, item-owned cosmetic metadata, per-frame observer or forced redraw at checkpoint. Scope: two occupied buried graves, completed native operations, two known top/fence choices. No new fitting behavior: previous exact 0.1.1 source/artifact preserves that accepted slice. This candidate removes its unused Hide hook. Invalid duplicate/out-of-range marker/code fails closed rather than guessing an owner.

Save classification: A/B registration and slot commits are save-persistent diagnostic parameters. Native removal/reinstallation/exchange and ordinary saves are real player actions. Checkpoint/panel/session references are nonpersistent. Existing 0.1.1 marker1/look is compatible as A/fence0; original bytes/number remain immutable. Exact current source/hash/handoff belongs to docs/TEST_BUILD_LOG.md; this plan does not claim runtime acceptance.
