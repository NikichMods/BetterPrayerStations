# Test-build log

## Appearance Probe 0.1.0 — prepared and installed; runtime return pending

Research-only plugin, not production Better Prayer Stations or a stable release. Compiled and installed with the game closed on 2026-10-23; no launch, live rendering/load/close result or user acceptance is claimed.

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
