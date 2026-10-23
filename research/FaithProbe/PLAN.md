# Faith Probe0.3.0 — research-method checkpoint

Question: can actual prayer-station coverage control already-saved grave input improvements through move-out/return, native demolition/replacement and menu-load, without deleting C/L/N or changing anatomy?

Existing paths: accepted0.2.0 proves native saved-grave lifecycle and rating projection, but has synthetic activation only. Static native grid_pos/pos/RefreshPositionCache, SetObject->WorldMap.OnAddNewWGO and DestroyMe plus exact Move Objects0.6.0 transform/grid/zone path establish bounded owners. No authored prayer-station radius exists. Native AuraEmitter.ProcessAurasCalculation traverses all emitters/receiver subsystem; supplying new aura definitions/receivers would introduce separate host-buff state for a new boolean coverage requirement. A separate coordinate-keyed save ledger is unnecessary.

Choice: retain the known artifact's implementation and extend only activation input. Discover real station refs in the existing post-load pass; register later additions through exact native world registration, verify actual membership to exclude rejected preview duplicates. Poll only this bounded station list/current grave every0.25s for support observations; actual getter/UI consult current native positions. This is simpler than patching arbitrary Transform setters or depending on one movement plugin's private internals, and avoids repeated world scans. No native movement, transform, zone or object mutation is performed. Exact distance uses inclusive circle radius3 native96-unit cells between WGO origins; purely diagnostic. Busy-station exclusion and highest-tier reporting are provisional eligibility, not production balance/purchase commitments.

| Case | Native path and independent evidence | Cleanup |
| --- | --- | --- |
|Cover/uncover/return | Real Move Objects commit; log raw transform XY separately from pos distance; actual native getter and explicit GUI OBSERVE | C/L/N remain |
|Demolish/rebuild | Native craft/DestroyMe/world registration; removed reference pruned, new registered station discovered | Ordinary native costs remain |
|Wood/stone overlap | Maximum reported tier; any remaining covering station activates full saved inputs | No stacked modifier |
|Menu-load | Existing native scene recovery and newly discovered live stations; recovered grave ID/C/L/N | Clear diagnostic fields then save |

Offline arithmetic/signature/native projection/save gates run before frozen handoff. The user performs genuine placement/movement/demolition and visual checks; no probe action teleports/spawns/despawns a station to manufacture its own acceptance. Full user steps are in README, exact identity in TEST_BUILD_LOG. Zone/hint refresh, payment/tier caps, arbitrary move mods and final radius/UI remain separate questions.
