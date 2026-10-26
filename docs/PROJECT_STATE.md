# Project state — Better Prayer Stations

Updated2026-10-26. This is the current entry point, not a chronological chat log. Product rules belong to DESIGN; exact historical runtime evidence belongs to TEST_BUILD_LOG.

## Where work lives

- Owning repository: NikichMods/BetterPrayerStations.
- Active branch: `codex/persistence-research`. At handoff, main still contains the early bootstrap; do not switch to it to “start fresh”.
- Current implementation: `research/MenuProbe`. No production source/release exists yet.
- GraveyardKeeperResearch is the shared host-knowledge repository, not this mod's production owner. Its app project was used for this long conversation; a dedicated BetterPrayerStations app project must point at the owning checkout.
- Portable decisions/source are committed to this branch. Game binaries/references, raw logs, screenshots, draft/generated PNGs and the browser preview are local ignored inputs. `.local/WORKSPACE.md` and `.local/handoff/2026-10-26/` preserve this machine's recovery pointers and selected-input bundle. Do not equate Git push with backup of ignored files.

## Current candidate

| Item | Exact state |
| --- | --- |
| Installed probe | Menu0.5.0 |
| Frozen source | `0b5b838e1889819645e9b2e897bef43f292fbb61` |
| Frozen tag | `probe/menu-0.5.0` |
| Artifact | `artifacts/MenuProbe-0.5.0/BetterPrayerStationsMenuProbe-0.5.0.dll` |
| SHA256 | `f624ed052f681ad57b26558c2a9a38d5988cee7422f3611dcacdc83bba9b5b55` |
| Installed identity | Same hash; canonical BetterPrayerStationsMenuProbe.dll |
| Previous payload |0.4.0 backed up outside loader tree; see TEST_BUILD_LOG/local WORKSPACE |
| Readiness | Builds/headless checks pass; installed visual check failed: user reports rows shifted left/down in station and several ordinary construction menus |

0.5.0 adds only native menu presentation to the accepted0.4.0 payment/persistence/effect. Real faith spending remains enabled. F8 diagnostic panel starts hidden; its existing grants/cleanup still exist. No final heavenly world-light effect is implemented.

## Acceptance map

| Area | Current evidence |
| --- | --- |
| Station entry, native three actions, grid under camera pan | Bounded installed acceptance from Menu0.1.x and later returns |
| Any occupied grave-cell conversion, nonstacking and removal restoration | Bounded installed Menu0.2.0 acceptance; physical body unchanged |
| Shared P/points/order persistence, support loss/recovery | Bounded installed Menu0.3.1/0.4.0 acceptance |
| Real shared faith price/debit, shortage, A/B/A isolation, saved cleanup/full restart | Requested Menu0.4.0 checks accepted; exact limits in TEST_BUILD_LOG |
| New text and icon selection | Accepted by user in design/mockup |
| Actual0.5.0 native layout/readability/ordinary-desk regression | Failed user return: rows shifted left/down in station and several ordinary construction menus; actions remain usable; cause under investigation |
| Final balance/world rays/production release | Not complete |

Handoff live-log snapshot shows READY0.5.0, recovered C10/P0/points0/faith50/next1, repeated station opens/closes and no own FAILED/INIT_FAILED markers in that snapshot. It is a snapshot of a still-running session, not a final visual result. Exact snapshot identity is in TEST_BUILD_LOG; no purchase is observed in this snapshot. Game was running during handoff, and was not stopped or modified.

## Immediate next action

Investigate the returned0.5.0 layout regression before further features. User reports actions/recipe rows shifted left/down in station and three or four other construction menus, while actions work. Preserved session and numeric return are in TEST_BUILD_LOG. Obtain whole-window station/ordinary-menu screenshots to distinguish list displacement from row-content displacement; actual final writer/cause is not established. Static host inspection shows Hide destroys the old rows before its close callback and subsequent openings create fresh rows, so direct reuse of modified row instances is not an established explanation. Inspect shared window/header/list geometry and relevant mod interactions without assuming a culprit. If a correction is needed, establish readiness and issue a new immutable candidate; do not rebuild0.5.0 or repeat the accepted entire payment/save/support scenarios.

After menu acceptance, proceed by IMPLEMENTATION_PLAN: world-light visual trial, remaining targeted functional/compatibility checks, explicit balance decisions and production consolidation. The handoff itself authorizes none of those as already accepted.

## Document ownership

| Question | Owner |
| --- | --- |
| Current state and next action | This document |
| Product mechanics/scope/defaults | [DESIGN](DESIGN.md) |
| Forward work sequence | [IMPLEMENTATION_PLAN](IMPLEMENTATION_PLAN.md) |
| Accepted menu copy/icons and native presentation gates | [STATION_MENU_PROPOSAL](STATION_MENU_PROPOSAL.md) |
| Acceptance summary and blind-review boundary | [EVIDENCE_AND_ACCEPTANCE](EVIDENCE_AND_ACCEPTANCE.md) |
| Exact frozen builds, logs and historical returns | [TEST_BUILD_LOG](TEST_BUILD_LOG.md) |
| Geometry / native menu | [BLESSING_MEMBERSHIP_RESEARCH](BLESSING_MEMBERSHIP_RESEARCH.md), [BUILD_MENU_RESEARCH](BUILD_MENU_RESEARCH.md) |
| Save carrier / payment transaction | [GLOBAL_BEAM_PERSISTENCE](GLOBAL_BEAM_PERSISTENCE.md), [GLOBAL_CAPACITY_PAYMENT](GLOBAL_CAPACITY_PAYMENT.md) |
| Former combined-concept work and old copy iterations | [history map](history/README.md) |

For a new chat use [CHAT_START_PROMPT](CHAT_START_PROMPT.md). Recovery is from these files and actual source, not a retelling of the entire conversation.
