# Test-build log

## Appearance Probe 0.1.0 — failed initialization; preserved

Research-only plugin, not production Better Prayer Stations or a stable release. Compiled/installed on 2026-10-23; user launch reported the disabled-panel message. Current session log confirms INIT_FAILED before READY, so no rendering/save/preview scenario was exercised or accepted.

- Exact clean source: `4d2e1bc786858978f179ee215e92d47c15fc85d8`.
- Frozen reference: `probe/appearance-0.1.0`.
- Numbered artifact: `BetterPrayerStationsAppearanceProbe-0.1.0.dll`.
- Canonical installed basename: `BetterPrayerStationsAppearanceProbe.dll`, isolated plugin directory.
- Artifact/installed SHA-256: `fea781138d693630d9bb96b44f711d40c53ae6b2d15860de31eaec7da258cc3b` (identical).
- Canonical version owner: `research/AppearanceProbe/Plugin.cs` Version; assembly/file versions derived and checked as 0.1.0.0.
- Build: `research/AppearanceProbe/build.ps1 -GameRoot <local game root> -DotnetRoot <local SDK root> -ExpectedSource <exact SHA>`; Windows SDK6.0.406 Roslyn, warnings as errors, locally installed host references. Full local reference hashes are in the ignored build-identity manifest.
- Gate PASS: compilation, assembly/file version identity and real native WOPPrefabName-based helper tests for names/cache, stage/suffix/herb preservation, unchanged source descriptor, unrelated fence, construction and empty/unsupported slots. These tests do not instantiate a Unity scene or prove native close/load behavior.
- Installation: exact artifact verified, no prior same-probe payload found, one active copy installed while host closed. Previous ordinary game log preserved privately under ignored evidence storage. Local installation manifest identifies paths/time/hash; no machine paths or raw host logs are published here.

Question/matrix: [PLAN](../research/AppearanceProbe/PLAN.md). Gameplay procedure and save-state rules: [probe README](../research/AppearanceProbe/README.md).

Pending user scenario: choose one completed occupied grave -> commit wood/stone gravestone look -> ordinary redraw -> native save/menu/load -> fit alternative -> close native grave window -> observe committed look restored -> cleanup and save. Accessible LogOutput.log is collected by the agent after the run.

Expected evidence: BPS_PROBE READY/SELECT/REDRAW_BEGIN/REDRAW_END with actual part names and unchanged snapshots; LOAD recovery; CANCEL reason=grave-close plus cancel redraw; CLEANUP. Record loader failures, failed/absent seams and user-visible outcome separately from compile success.

Save classification: two explicit namespaced diagnostic fields are save-persistent; preview is not saved. Cleanup zeroes only these two fields, returns native appearance and requires one ordinary save to persist cleanup. No faith, money, physical body or decoration data is intentionally changed.

No source or numbered artifact may be overwritten under this handed identity. Any corrective candidate after handoff gets a new number; the production appearance gate remains open.

### Observed 0.1.0 failure and narrow correction

Installed 0.1.0 hash matches its exact artifact, and the game assembly hash matches the inspected host. Log snapshot SHA-256: `dffc024dd93a5461bc83f0b7d9576fbe8cb0f2f24bc9fb04fb2f392173bf035a`, retained privately in ignored evidence storage. Harmony reports `Parameter "grave_wgo" not found in method virtual void BaseGUI::Open()` while installing the grave-open postfix. The old name-only AccessTools lookup selected an inherited parameterless BaseGUI method, rather than the native declared GraveGUI.Open(WorldGameObject) supported by installed metadata/source. This is a probe binding defect, not evidence of an unsupported game build.

Correction scope: resolve all five targets by declared owner, exact argument types, return type and implemented body before patching; reject inherited/no-argument grave Open; bind the grave postfix argument by index. Improve the panel's generic initialization failure wording so it does not misdiagnose every patch failure as an environment mismatch. No product mechanics, save fields, looks, UI workflow or rendering algorithm changed. The descriptor mechanism remains justified; this failure occurred before testing it.

Local regression gate passes exact bindings for CustomDrawers.GetNewWOPPrefabsNames(), GraveGUI.Open(WorldGameObject), GraveGUI.Hide(bool), MainGame.OnGameLoaded(GameSave), MainGame.OnGameStartedPlaying(), plus inherited-overload rejection and indexed argument contract. Existing descriptor/version tests also pass. This is not yet a successful installed Harmony startup.
## Appearance Probe 0.1.1 — narrow installed scenario PASS

Corrective research candidate for the 0.1.0 initialization failure above. No production release or full product acceptance is implied.

- Exact clean source: `f2857b63ad8655f4fc17892f3e1ba00215681257`.
- Frozen reference: `probe/appearance-0.1.1`.
- Numbered artifact: `BetterPrayerStationsAppearanceProbe-0.1.1.dll`.
- Canonical installed basename: `BetterPrayerStationsAppearanceProbe.dll`.
- Artifact/installed SHA-256: `3d65bf6d32ed485fdc9efe6eeee2b1297cd8329383e5ee9d35fa5dcbd1cef36d` (identical).
- Canonical version owner: Plugin.cs Version; assembly/file versions checked as 0.1.1.0.
- Build gate: clean exact source, SDK6.0.406 Roslyn compilation with warnings as errors, five exact declared patch targets, inherited Open() rejection, indexed argument binding contract, native descriptor projection and version identity tests all PASS.
- Installation on 2026-10-23: host process absent immediately before replacement; prior 0.1.0 DLL backed up outside the plugin directory; exact new artifact copied to the canonical basename and hash verified. Private installation manifest retained in ignored evidence storage. Numbered 0.1.0 bytes remain unchanged.
- Runtime status: installed startup and the requested narrow scenario passed on 2026-10-23; details below. This is research acceptance, not stable promotion or full product acceptance.

### Installed return — 2026-10-23

User reports every visual step matched the procedure and supplied a native grave-window screenshot. Fresh session log identifies 0.1.1 and READY with the expected game assembly hash; installed DLL still matches the artifact hash above. Preserved private log SHA-256: `b8d41ce97045c3c1db922f7a4695d8e88c44677c4c30a8ffec388588cc0dd23f`. Screenshot SHA-256: `923520be3ac4be3248749194ad4b4f548e52d0cb2b8c137f7f8cab551479b2c8`. Raw log/image are retained only in ignored local evidence, not public assets.

Selected grave 47567 has actual `grave_top_stn_plate_2` and `grave_bot_stn_2`, native quality 3. Six REDRAW_END records all report invariants=True; all recorded state strings are identical across SELECT and before/after actions: native quality, actual item IDs/counts/durability, direct body contents, player money and faith. This is bounded instrumentation, not a bytewise comparison of every save field.

| Step | Recorded world result |
| --- | --- |
| Commit wooden cross | Actual WOP becomes `grave_top_wd_cross_1_stg_1`; stone fence retained |
| Explicit redraw | Same wooden cross and fence |
| Native menu/save-load | LOAD recovers grave 47567 with committed=1; wooden cross restored |
| Temporary stone-cross fitting | `grave_top_stn_cross_1_stg_1`; committed remains 1 |
| Close grave window | CANCEL reason=grave-close; persisted_look_unchanged=true; wooden cross restored |
| Cleanup | Original `grave_top_stn_plate_2_stg_1` restored; active_keys=0, inert_zero_keys=2 |

No probe FAILED/INIT_FAILED or invariant violation appears in this snapshot. Other host errors exist; do not call the entire session error-free. In particular, a missing `grave_top_stn_plate_2_stg_2` resource is logged at line731, before initial target selection at line1043. The tested redraws all instantiate stage1; this record is not a failed probe redraw, and its independent cause/owner is not established here.

The screenshot records native UI quality3, body3 red/3 white at97%, and the original physical gravestone/fence names and icons, each quality3. It is a useful native UI baseline, not a screenshot proving every world appearance or a finalized cosmetic picker design.

Limits: one occupied, completed grave; gravestone only, two test looks. No fence-choice mutation, removal/reinstallation, physical item transfer, empty/construction slot runtime test, exhumation, movement, interrupted fitting load/save, full application restart or production payment/coverage/UI acceptance. Cleanup was observed in memory; a subsequent save/load proving persistent cleanup is not recorded. Next useful lifecycle slice is two independent grave-slot choices through decoration removal/reinstallation and destination-grave transfer; do not repeat the passed basic redraw/close scenario without a new reason.
