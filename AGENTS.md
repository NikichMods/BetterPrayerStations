# Better Prayer Stations — Working Contract

This repository follows [NikichMods/DevRules](https://github.com/NikichMods/DevRules). Read its WORKING_RULES.md and action-based reading map; local checkout work also follows CODEX_WORKFLOW.md. This file contains project-specific requirements, not a second global policy.

## Start and ownership

1. Inspect the actual checkout/branch/changes, then read [docs/PROJECT_STATE.md](docs/PROJECT_STATE.md).
2. Read [docs/DESIGN.md](docs/DESIGN.md) for product decisions and [docs/IMPLEMENTATION_PLAN.md](docs/IMPLEMENTATION_PLAN.md) for the active work sequence.
3. Read the task's owner document from PROJECT_STATE; use [docs/TEST_BUILD_LOG.md](docs/TEST_BUILD_LOG.md) for exact candidate/evidence identities. Do not treat historical “current/next” passages as today's work queue.
4. Consult the local ignored `.local/WORKSPACE.md` for this machine's game, compiler, artifact, preview and handoff paths. Missing private inputs are an explicit environment gap, not permission to regenerate accepted artwork or infer host APIs.

Project: NikichMods/BetterPrayerStations, initial target Graveyard Keeper1.407 Windows/BepInEx5. This repository owns product design, runtime source, assets selected for the mod, build identities and acceptance. Shared reusable host knowledge belongs to NikichMods/GraveyardKeeperResearch, starting at docs/RESEARCH_INDEX.md. Consult existing project owners, shared facts and accepted evidence before fresh research.

## Current work

- Active source is `research/MenuProbe`, not a released production plugin. Current installed0.5.0 adds accepted menu copy/icons to accepted0.4.0 mechanics. Visual return failed: rows shift left/down in station and several ordinary construction menus; PROJECT_STATE and TEST_BUILD_LOG own exact status.
- Next action is diagnose the returned0.5.0 layout regression, starting with whole-window screenshots and shared layout ownership. Do not start a new feature, rebuild a frozen version or assume functional actions mean visual acceptance.
- `research/AppearanceProbe` and `research/FaithProbe` are historical experiments for the former combined concept. Preserve them as evidence; do not install them or resume their old “next” scenarios by default.
- Use the current checkout/branch documented in PROJECT_STATE; do not switch to main just because a new chat starts. One active writer per checkout; use a separate worktree only when actual concurrent writing requires it.

## Product invariants

DESIGN is authoritative. No station radius or passive station-quality aggregation. Faith buys global reusable simultaneous capacity P; completed stations supply ceiling C; active capacity=min(P,C). Free spatial placement/removal preserves paid capacity and price progression. Earliest placed fields stay active first, newest go dormant first; remove/reinsert makes a point newest. Station demolition retains P and positions. Any occupied grave construction cell qualifies, effect is nonstacking. Convert all effective red skulls to white with native decoration/decay limits; no separate+1 quality and no physical body/organ/item mutation/export. Keep excluded inspection/cosmetic/per-grave paid services excluded unless explicitly reopened.

Trial defaults: wooden+5/stone+10 and first ten prices1,2,3,4,6,9,12,15,18,21. They are tuning values, not final balance. Price continuation beyond10 is unresolved; keep the current probe bound rather than inventing it. No paid reserve identity, dedicated move tool or Move Objects dependency.

## Evidence and spoiler boundary

- Never guess game IDs, formulas, schemas, render owners or lifecycle boundaries. Before host work consult shared GRAVEYARD_RUNTIME, CRAFTING_INVENTORY_AND_TRADING and UI_INPUT_TIME_AND_ENVIRONMENT as relevant; preserved source/runtime evidence outranks chat recollection.
- Follow ENGINEERING_RULES per-change readiness, RUNTIME_INTEGRATION and RUNTIME_TEST_HARNESS when relevant. Prefer narrow owned hooks; avoid recurring global scans, resource enumeration, reflective discovery and broad logging.
- Anatomy balance review is deliberately blind. Without explicit spoiler-disclosure authorization do not reveal recipes, organ combinations, costs, attainable maxima or reproduction instructions. Do not open the detailed ignored blind-review inputs during routine recovery. EVIDENCE_AND_ACCEPTANCE contains the safe verdict and limits.
- Browser mockup acceptance, source compilation, installed loading and user visual acceptance are separate evidence classes. Preserve earlier accepted tests instead of requesting complete repeats.

## Builds, installation and acceptance

- main is the stable/documentation baseline; unaccepted runtime work stays on codex branches. No stable promotion or Release without its explicit acceptance.
- Menu diagnostic canonical version: `research/MenuProbe/Plugin.cs` Version. Installed filename `BetterPrayerStationsMenuProbe.dll`; numbered handoff `BetterPrayerStationsMenuProbe-<version>.dll`. Numbered source/tag/binary bytes are immutable.
- Build gate `research/MenuProbe/build.ps1`: verified local SDK6.0.406 Roslyn, pinned native references, exact clean source and embedded-icon hashes. Current PNG inputs are intentionally ignored/local; see WORKSPACE/handoff bundle. Do not upload game references or extracted assets to hosted CI. Reassess production build/CI separately, not for documentation cleanup.
- For authorized local tests collect accessible logs and install exact verified DLLs directly under the DevRules local-runtime loop. Never replace a loaded binary or terminate/restart the game without authorization. Back up previous owned payload outside loader folders; preserve unrelated plugins/configs/saves. Keep one active BPS probe.
- Future production installed filename is `BetterPrayerStations.dll`; no production version has been consumed. Historical Appearance/Faith probe version owners remain in their source and TEST_BUILD_LOG.
- Original source uses MPL-2.0; see LICENSE/LICENSING. Do not commit game binaries, full decompilation, extracted sprites or private user context. Keep raw references and media scratch in ignored locations.
