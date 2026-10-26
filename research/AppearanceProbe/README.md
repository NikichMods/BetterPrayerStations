> Historical experiment, not the current installed prototype. Start at [PROJECT_STATE](../../docs/PROJECT_STATE.md); do not install/resume this probe by default.

# Better Prayer Stations — Appearance Probe 0.1.2

Research-only, not a production mod/release. This slice tests **two occupied graves, each with independent gravestone and fence appearance choices**, through actual native decoration removal/reinstallation and exchange. Previous 0.1.1 redraw/menu-load/preview-close result remains preserved under its immutable source/artifact; fitting is deliberately absent from this lifecycle probe. No faith, fees, prayer coverage or final UI is implemented.

## Build and identity

Canonical version: Plugin.cs Version; assembly/file versions and artifact name derive from it. Installed `BetterPrayerStationsAppearanceProbe.dll`; handoff `BetterPrayerStationsAppearanceProbe-<version>.dll`. Exact source/hash/install state: [test-build log](../../docs/TEST_BUILD_LOG.md).

Build: `build.ps1 -GameRoot <local game root> -DotnetRoot <local SDK root> -ExpectedSource <exact SHA>`. SDK6.0.406 Roslyn, warnings as errors, installed proprietary host references only. Numbered builds require clean exact source and refuse artifact overwrite. `-CheckOnly` emits a non-handoff development check. Tests validate four exact declared signatures, inherited Open rejection, indexed argument, native WOP names/cache preservation, independent slots, missing slots, construction, suffix/stage/herb and invalid code rejection. They do not execute Unity or native crafts.

Four hooks: native descriptor result, GraveGUI.Open(WorldGameObject), MainGame.OnGameLoaded(GameSave), MainGame.OnGameStartedPlaying(). Exact host SHA guard retained because the probe writes diagnostic grave fields against inspected private save/GUI ownership. Native resource definitions/types and stage1 WOPs are checked before selecting looks. The previous Hide hook is unnecessary for this slice and removed.

## Gameplay procedure

Use two normal occupied buried graves, A and B, with both completed decoration slots and no active craft. Prefer **different physical gravestone and fence IDs** at the two graves so an exchange also demonstrates that quality follows the physical item. No mod movement command is needed: move the ordinary removed inventory items, not entire graves.

Panel: left, draggable, scrollable; F8 hides/shows. The A/B buttons register an opened grave once; later opening a registered grave selects it automatically. Do not reassign the letters during the test. The same grave cannot own both letters.

1. Open A -> **Запомнить открытую могилу как А** -> **Надгробие: дерево**, **Оградка: камень I**. Open B -> register as B -> **Надгробие: камень**, **Оградка: камень II**. Click **Записать состояние обеих могил и инвентаря**. This is the baseline. Cosmetic commits are free, save-persistent diagnostic settings.
2. At A, remove only the gravestone with the game's ordinary action and finish the work. Record a checkpoint: gravestone disappears, fence retains its chosen appearance; both saved codes remain.
3. Remove A's fence normally and finish. Pick up both resulting items into the player inventory and record another checkpoint. A should have neither decoration; ordinary inventory names/icons are expected.
4. Reinstall those two items at A with ordinary native actions, finish work, record a checkpoint. Both A looks should return without choosing them again.
5. Now remove both decorations from A and B, finish all work and collect the items. Keep track of which physical items came from each grave. Record the empty-grave checkpoint. Install A's former items at B and B's former items at A; finish work and record again. Each grave must retain **its own** appearance. Actual installed item names/icons and quality must follow the exchanged physical items; they need not match the chosen appearance. Quality changes caused by changed physical decoration are expected, not an invariant failure.
6. Save normally, return to menu and reload once; record another checkpoint and confirm both grave appearances. This specifically checks two choices on two modified inventories, not a repeat of 0.1.1's one-slot scenario. Then **Очистить обе могилы и вернуть исходный вид** and save normally. Cleanup changes only the diagnostic choices, not the installed physical items or their current positions.

No need to repeat fitting/cancel from 0.1.1. The agent collects accessible logs and installed identity; user reports visual outcome only. A checkpoint should be pressed **after work has completed**. It reads actual world parts and state without forcing redraw, so stale native redraws remain detectable rather than hidden by the harness.

## State, observation and containment

Exactly three namespaced parameters per grave: existing `selected` (1=A, 2=B), existing `look` (0=native,1=wood cross,2=stone cross), new `fence` (0=native,1=stone fence I,2=stone fence II), all under `nikichmods.bps.appearance_probe.`. Existing uncleaned 0.1.1 marker1/gravestone code is read as A with native fence0; no automatic unrelated migration or item writes. A duplicate/invalid positive marker fails closed; unrelated graves are untouched. No production schema is promised.

Persistent choices stay on grave Item, including while decoration slots are empty. The descriptor postfix matches only existing positive-stage native descriptors against actual inventory items; it never creates descriptors for absent slots, changes item IDs/durability/quality or patches native craft/transfer code. Test looks: `grave_top_wd_cross_1`, `grave_top_stn_cross_1`, `grave_bot_stn_1`, `grave_bot_stn_2`.

On-demand CHECKPOINT records both grave IDs, saved codes, native craft state, actual WOP names, measured quality/item/body/currency state and the player's direct decoration inventory. Items' three diagnostic parameters are included to detect unintended cosmetic metadata export. Checkpoints are observations, not automatic PASS assertions; ordinary gameplay changes between checkpoints are allowed. REDRAW invariants compare only immediately before/after the probe's own cosmetic refreshes, not across native item removal/exchange.

Only explicit cosmetic commits, load recovery and cleanup request a full redraw; one grave per queued Update. Native crafts must perform their own ordinary redraws. There is one existing WorldMap list pass after load, never recurring scans/polling or per-frame logging. Load invalidates old references before restoration, recovers at most one A and one B and redraws them on separate frames. Checkpoints enumerate only the two opted-in graves and direct player inventory.

Cleanup zeroes only these three diagnostic fields, leaves inert zero fields if not natively pruned, and requires an ordinary save. Without the DLL, vanilla ignores the fields and uses ordinary item appearance. No items are created, deleted, transferred, refunded or restored by the harness, including cleanup. Native item operations and saving remain real gameplay changes.

Failures disable projection and attempt bounded vanilla redraw on each known grave on separate frames, with no repeated retry after a failing fallback. Persistent choices remain for explicit cleanup; cleanup is accessible even if projection is disabled. Log errors and fallback markers must be reviewed before acceptance. No application restart, exhumation, whole-grave movement, all mods, full appearance catalog, payment/UI or other interruption path is proved by this candidate.
