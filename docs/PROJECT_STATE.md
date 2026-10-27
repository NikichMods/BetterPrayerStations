# Project state — Better Prayer Stations

Updated2026-10-27. This is the current entry point, not a chronological chat log. Product rules belong to DESIGN; exact historical runtime evidence belongs to TEST_BUILD_LOG.

## Where work lives

- Owning repository: NikichMods/BetterPrayerStations.
- Active branch: `codex/persistence-research`. At handoff, main still contains the early bootstrap; do not switch to it to “start fresh”.
- Current implementation: `research/MenuProbe`. No production source/release exists yet.
- GraveyardKeeperResearch is the shared host-knowledge repository, not this mod's production owner. Its app project was used for this long conversation; a dedicated BetterPrayerStations app project must point at the owning checkout.
- Portable decisions/source are committed to this branch. Game binaries/references, raw logs, screenshots, draft/generated PNGs and the browser preview are local ignored inputs. `.local/WORKSPACE.md` and `.local/handoff/2026-10-26/` preserve this machine's recovery pointers and selected-input bundle. Do not equate Git push with backup of ignored files.

## Current candidate

| Item | Exact state |
| --- | --- |
| Installed probe | Menu0.5.1 — read-only layout diagnostic;0.5.0 geometry retained |
| Frozen source | `42513e21faa69687fc9f931e6001e001e7045f24` |
| Frozen tag | `probe/menu-0.5.1` |
| Artifact | `artifacts/MenuProbe-0.5.1/BetterPrayerStationsMenuProbe-0.5.1.dll` |
| SHA256 | `9d62888681b57d7b2b03a15f19e5a41f578982131c45b8a2f08411e1993cfa9d` |
| Installed identity | Same hash; canonical BetterPrayerStationsMenuProbe.dll |
| Previous payload |0.5.0 backed up outside loader tree; earlier0.4.0 backup retained; see TEST_BUILD_LOG/local WORKSPACE |
| Readiness |0.5.1 diagnostic return captured: shared scroll X6->-26 / clipOffsetX0->32 after station refresh; scoped corrective gate READY, no visual fix implemented |

0.5.0 adds native menu presentation to the accepted0.4.0 payment/persistence/effect;0.5.1 adds read-only layout tracing only. Real faith spending remains enabled, though the current short diagnostic requires no purchase/save changes. F8 diagnostic panel starts hidden; its existing grants/cleanup still exist. No final heavenly world-light effect is implemented.

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

The0.5.1 return localizes horizontal drift to shared scroll.localX6->-26 with clipOffsetX0->32 on the frame after synchronous station refresh. Hide retains it; the next ordinary desk opening restores Y only. Pinned native CraftGUI.LateUpdate calls a confinement overload that enables both axes even for vertical views. BUILD_MENU_RESEARCH owns a READY gate for restricting that call to the vertical axis only during the exact owned station session, preserving ordinary dispatch. Next implementation is this scoped correction, followed by the short desk->station->desk return; no0.5.2 source/artifact exists yet. Station header-background fitting remains separate. Do not rebuild frozen candidates or repeat the accepted entire payment/save/support scenarios.

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
