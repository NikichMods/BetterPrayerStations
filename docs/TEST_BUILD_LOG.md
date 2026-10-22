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