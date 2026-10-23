# Faith Probe0.4.0 — research-method checkpoint

Question: can one grave convert actual player-carried faith into one persisted improvement, advancing the same C+L sequence regardless of action, with no debit on shortage/stale context and contained failure between debit and state commit?

Existing accepted0.2/0.3 proves saved grave inputs, arithmetic and bounded actual coverage, but all actions are free. Native single-resource Item.RemoveItem and GetTotalCount source/IL establish root/bag scope and direct list/value mutation; a boolean alone does not prove exact debit. Native crafting would add work/craft state and ingredient service owners beyond an instant optional grave action. A generic multi-resource transaction/framework is unnecessary. Existing exact baseline code is extended only with paid action, not reconstructed.

Choice: native single-resource debit + exact before/after count + existing grave saved-state write, all in one synchronous main-thread method. Small snapshot of only verified mutable root/bag lists/item quantities and owned grave values supports bounded rollback; no Item deep clone replacing native references, global player inventory replacement or asynchronous save machinery. No GUI call/yield inside the transaction; normal save later serializes current player/grave owners. Quoted count/cost and reentry guard are checked, failures disable rather than retry. Native getter/UI work follows a completed commit and is not a second debit phase.

Test-only N+1 prices and shortage quote(actual faith+1) are explicit diagnostic inputs, not balance or simulated funds. Conversion/wreath share N=C+L. Successful setup from zero and three purchases costs6 faith; cleanup removes owned improvements without refund. Probe UI/context excludes build/floating preview purchase; preview of affected grave results stays intact.

| Case | Independent evidence | Cleanup |
| --- | --- | --- |
|Conversion then wreath | Actual native faith delta and saved C/L/N, native rating/UI and physical grave hash | Explicit clear then save; paid faith stays spent |
|Insufficient | Actual balance vs larger diagnostic quote, unchanged faith/C/L/N, no debit call | No change |
|Save/menu-load + next conversion | Recovered native payer balance, same grave ID/C/L, derived price3 then4 | Ordinary save |
|Stale/reentrant/fault | Compiled real backend with simulated debit and seven fault classes; native counting/serializer retained | Snapshot verification |

Native debit itself and real player save restoration remain live-only; do not describe simulated debit tests as installed payment proof. No visual/placement/zone/coverage fix, final price/cap policy or production format change is included. Exact user steps README; immutable source/artifact/hash/runtime return TEST_BUILD_LOG.
