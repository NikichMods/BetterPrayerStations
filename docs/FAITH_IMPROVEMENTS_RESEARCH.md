# Grave-owned faith improvements — technical research, 2026-10-23

Status: static/native arithmetic evidence plus accepted bounded Faith0.1.1 rating/UI,0.2.0 grave persistence/replacement and0.3.0 numeric wooden coverage/load returns. Full product remains unready; preview/construction classification and world-badge refresh are open. Paid-action candidate0.4.0 is owned by FAITH_PAYMENT_RESEARCH.md; real installed payment/player-save acceptance is pending. Exact artifact/return identities remain in TEST_BUILD_LOG.md.

## Outcome and selected research direction

A grave stores purchased conversion capacity C and extra wreaths L, with one shared successful-action progression N. The physical body, organs, their native skull fields, durability and real decorations remain unchanged. Coverage gates effective use; no body means no faith contribution. Removing coverage or replacing a body never deletes investment. Unused C does not create white skulls after eligible red skulls run out.

User decision in this segment: **converted white skulls obey the native body-decay limit**. They do not cure decay. Apply conversion to total effective red/white counts before calculating available white skulls. Existing price-led access, wooden first-N purchase cap, strongest-tier purchase permission and any-station activation remain unchanged; prices/N/radius are not settled here.

Research direction: narrowly project two inputs at the **grave branch** of WorldGameObject.quality, leaving its native branches, craft ignore-item rule, floor and min calculation intact. Feed corresponding effective inputs to the native central grave diagram. Keep shared Item.GetBodySkulls, BodyPanelGUI and physical item getters unpatched globally. This is a supported candidate mechanism, not a production implementation; the research probe compiles the transpilers and validates their rewrite against the installed native instruction lists, with live startup/central-diagram agreement now confirmed for the bounded0.1.1 slice, not full production acceptance.

## Evidence identity

GK 1.407 Assembly-CSharp.dll SHA-256 rechecked: `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`. Same installed references and pinned supporting source `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9` as the existing persistence/rendering research: WorldGameObject, Item, GraveGUI. Fresh installed IL inspection adds BodyPanelGUI.Draw, WorldZone.GetTotalQuality/GetQualityString/RedrawQualities and selected Item.RemoveItem methods. No copied host code/assets are stored in Git.

## Native owner / final consumer

| Boundary | Verified host behavior | Consequence |
| --- | --- | --- |
| WorldGameObject.get_quality | Grave branch gets the current body and its native skull totals; result is min(floor(GetInventoryQuality(ignore)) - red, availableWhite) | Effective values must enter before this cap; adding a number to the final result is not equivalent |
| No body | Returns the definition's authored quality before skull/inventory grave calculation | Preserve this return; no empty-grave bonus |
| In-progress set_/rem_ | Selects the current craft input/output ID as ignore argument before GetInventoryQuality | Preserve native exclusion instead of copying an idle-only formula |
| Generic quality modifiers | quality_k and generic quality expression/decay/multiplier arithmetic occur in the non-Grave branch; the early approximately-zero quality_multiplier guard still precedes both branches | Those parameters are not a ready extension point for these two effects |
| Item.GetBodySkulls | Sums body and contained-part skulls, clamps negative totals to0; uses ceil(durability*100), promotes percent>90 to100, floors white*percent/100 | Get native anatomy totals first; only eligible effective reds can become whites; use same decay leaf for increased white total |
| Item.GetInventoryQuality | Traverses inventory and recurses into Body items; ordinary decoration quality is durability-sensitive | Use the actual native input, not a guessed universal sum of two item definitions |
| GraveGUI.Redraw | Draws raw body panel and reads WGO quality for the displayed total | Raw anatomy panel and effective burial result have separate owners |
| GraveGUI.RedrawGraveInventory | Separately sums two DrawGravePart results, calls body.GetBodySkulls and builds central slots/wreaths/scroll bounds | A rating-only patch leaves the diagram inconsistent; do not attach extra wreaths to one real item's card |
| BodyPanelGUI.Draw | Calls shared body.GetBodySkulls and renders raw totals/durability | Keep actual anatomy visible and Cultist disclosure unchanged |
| WorldZone.GetTotalQuality | Enumerates its member WGOs, skipping ignore_counting_at_zone, and calls each WGO.quality; may emit OnQualityComputed | Real rating seam feeds zone totals; getter has a notification side effect and is not a pure diagnostic query |
| WorldZone.GetQualityString | Reads live totals, with additional player-parameter substitutions depending on format | Existing authored zone-quality mirrors are not a new storage owner; UI/quest refresh timing still needs runtime evidence |

Relevant installed IL anchors, not a proposed offset-only patch: the grave getter calls GetBodySkulls at011A, GetInventoryQuality(ignore) at0126, then Floor/sub/Min. The non-Grave branch has another GetInventoryQuality call at0163: do not replace every matching call indiscriminately. RedrawGraveInventory calls DrawGravePart twice (0084/0096), finishes its decoration accumulator at009C and calls GetBodySkulls at00E4 before all diagram/scroll loops. Actual transpilers must validate exact owners/signatures, branch and expected call/accumulator shape, preserve labels/exception blocks, and fail closed on mismatch. These anchors describe this inspected binary only.

## Effective model

Inputs: native clamped body totals R/W, actual body durability d, native inventory-quality input D (including the getter's ignore argument), stored C/L and current activation. With body and coverage:

- c = min(C, R).
- effectiveRed = R - c; effectiveWhite = W + c.
- p = ceil(100*d); if p>90, p=100.
- effectiveAvailableWhite = floor(effectiveWhite*p/100).
- effectiveDecoration = D + L.
- Native grave result = min(floor(effectiveDecoration) - effectiveRed, effectiveAvailableWhite).

Without coverage, use native inputs unchanged; without a body, preserve the native no-body result. Counts stay saved, not clamped/deleted when current R is smaller. C/L must be validated nonnegative integer counts before use; integer overflow/float-backed save precision must be bounded by the eventual schema. N can be persisted with a consistency check or derived from successful C+L purchases; exact schema is not selected. No new cap or pricing formula is adopted.

| Healthy body / actual decoration | Action | Effective result | Rating |
| --- | --- | --- | --- |
| R3/W3, D6 | None | R3/W3, D6 | 3 |
| Same | One conversion | R2/W4, D6 | 4 |
| Same | Three conversions | R0/W6, D6 | 6 |
| Same | One extra wreath only | R3/W3, D7 | 3: white capacity already limits |
| R0/W6, D3 | One extra wreath | R0/W6, D4 | 4 |
| R3/W3 at80%, D6 | Three conversions | R0/W6, availableW4 | 4 |

An extra wreath adds decoration capacity, not a guaranteed graveyard-quality point. A conversion on a decayed body can also yield no immediate score increase because of rounding. Show native-before/effective-after and the real current delta; whether to allow a zero-current-gain purchase for future replacement capacity remains an explicit action-eligibility/UX decision, not silently disabled or adopted here.

## Mechanism checkpoint / alternatives

1. **Generic WGO buffs/quality_k/definition expression:** insufficient on the inspected body-present Grave branch. Inserting hidden physical items or changing organs would alter ownership/removal/native consumers and violate the requested invariant.
2. **Global Item.GetBodySkulls or GetItemQuality patch:** too broad. Those readers serve body/anatomy/worker/UI paths without a grave owner; body reference alone cannot establish current coverage. A dynamic global context stack adds reentrancy/lifetime dependencies unnecessarily.
3. **Getter postfix reconstructs effective final quality:** possible, but must duplicate the grave branch and transient craft filtering. A flat delta on the already capped result is wrong when white/decor limits change.
4. **Selected candidate: call-site input projection:** at the two semantic calls in the grave branch, wrappers receive the native input plus current WGO, invoke native skull/inventory readers normally, then return effective numbers without writing Items. Preserve the rest of the host calculation. Only the tiny available-white decay leaf needs explicit calculation after increasing white total; it is independently compared with native GetBodySkulls below. Do not create a temporary/shadow body in production.

The central UI needs the same c/effective-red/effective-available values and +L on its existing decoration accumulator, before the native diagram and scroll loops. Raw body panel and real item cards remain native. Its two-card sum and the getter's full inventory sum are distinct native inputs; do not silently normalize vanilla differences for unusual inventories/other mods. A short explanation separates body facts, stored improvements and effective active/dormant result. Native UI layout, controller behavior and large counts remain unverified.

## Offline check and limits

Original research script: `research/faith/native-skull-oracle.ps1 -ManagedDirectory <installed managed path>`. It creates synthetic uninitialized native Items/definitions with explicit cached definitions, avoids Unity Resources constructors and invokes the actual installed Item.GetBodySkulls. A separate synthetic native body with transformed reference counts provides an independent oracle for the proposed decay leaf. This is test data, never live body mutation or a recommended production shadow-body implementation.

PASS: **160 cases**, eight durability values including0/80/90/90.01/97/100%, five red/white inputs including negative-total clamping and red-free bodies, four stored capacities including excess. Checks: native available-white agreement, eligible-red bound, no white creation on red-free bodies, unchanged original synthetic raw totals/available whites/durability. Explicit 6-white limits:80%->4,90%->5,90.01%->6. Unity Mathf floor/ceil are managed in these inspected references and were also called directly offline. No native WGO/Harmony/GUI/zone/payment/real save/exhumation path is exercised. This is arithmetic/source evidence, not installed faith-feature acceptance.

## Faith purchase / refresh boundaries still open

The existing native single-item RemoveItem checks quantity then removes; RemoveItems iterates independent removals, so its boolean is not a general multi-resource transaction. Do not equate sufficiency with successful exact charging or use unrelated nearby/church storage just because an interaction MultiInventory exposes it. Faith source inventory scope, count/debit agreement, one successful payment/action, reentry, failure rollback and save commit need their own focused check. Price remains unsettled; no faith is spent by this research.

After an accepted action, only the selected grave's effective state/UI and relevant quality display should be refreshed. Zone GetTotalQuality reads live WGO quality, so no parallel additive zone ledger is needed. Coverage geometry/invalidation, native hint/zone UI refresh timing, compatibility with other rating transpilers and future saved-state schema remain open. No per-frame world scan or global body patch is justified.

## Readiness and next smallest evidence

**BLOCKED for the full production feature**: native owners/seams, arithmetic and the bounded one-grave native call-site/GUI/synthetic-activation slice are established. Grave-owned saved investment, body replacement/non-export, actual coverage and payment, adjacent consumers and supported edge cases remain open. The subsequent authorized `research/FaithProbe` candidate supplies these inputs; see its PLAN/README and TEST_BUILD_LOG.md. Numbering and acceptance are independent of the appearance probe.

The completed bounded installed experiment is an isolated grave-effect slice: one opted-in grave, explicit diagnostic C/L plus active/dormant input (clearly synthetic until real coverage is implemented), native rating and central diagram, unchanged raw body/organ/durability/item snapshots, and cleanup. Live zone-total notifications/refresh are deliberately deferred from this first user-requested rating/scale slice because the getter has notification side effects. Do not bundle unverified payment/coverage geometry into this first proof or repeat accepted cosmetic tests. Then establish body removal/replacement/non-export and persistence through real paths before paid production actions. Temporary diagnostic activation must never be presented as proof of prayer-station radius or spending behavior. Exact scenario/guards must be recorded before any numbered handoff.

## First installed return — bounded, not complete

Faith Probe0.1.0 initialized and demonstrated rating/central-diagram agreement for zero/one/two conversions, extra wreaths and off/on at C1/L1 with unchanged complete measured physical parameters. Third conversion reached native rating/label6 and correct total slot/wreath counts, but the combined zero-red-bar observer failed, triggered effect disable/vanilla rollback and required corrective observation. See TEST_BUILD_LOG.md for exact identity/evidence. The underlying raw body/organ/decoration/currency hash stayed identical for all11 redraws. Native hidden-bar height is not a meaningful zero-red visual invariant; corrected0.1.1 retains visibility and positive-red height checks and adds separate actual bar telemetry. The subsequent0.1.1 return passes full conversion/excess conversion, wreaths, off/on and cleanup. It observes the hidden bar atzero red with height/minimum18, confirming the native-coercion explanation for the invalid exact7 check. All nine physical hashes match, and raw anatomy remains unchanged. No radius/payment/save-investment acceptance follows. The previous0.1.0 record remains a historical partial/failing run.

## Faith0.2.0 saved-investment evidence

Authorized next slice uses three diagnostic root-grave Item parameters (schema1, conversion capacity C, extra wreaths L); N=C+L is the shared successful-action sum. It is not a final production schema or price implementation. Existing accepted rating/diagram projection now reads the current body dynamically; empty graves retain native behavior. Post-start one-time recovery finds the single marked grave, validates state and logs its identity/counts. Explicit activation remains session-only and resets enabled on load; no payment or radius is implied.

New original LifecycleTests execute actual installed WorldGameObject.get_quality IL on synthetic owners/definitions, applying the real rewrite and shared saved-state backend with explicit headless authorization. PASS2700 combinations of skulls, decay, decoration, C/L and activation against independent synthetic native reference fixtures; original getter equivalence, no-body/generic/native craft exclusions and original body bytes checked. Actual SmartSerializer map roundtrips preserve occupied/empty/replaced grave IDs and investment; synthetic list relocation shows no body/organ export, current-red/decay recalculation, shared N retention, native positive-parameter pruning, cleanup and unrelated data preservation. Unknown schema and invalid counts are rejected.

Limits: synthetic definitions/owners and relocation, no Unity scene, actual exhumation/SetObject or live recovery/UI/save acceptance. Installed0.2.0 now confirms same grave47567/C3/L1/N4 after both empty and replacement-body menu-loads; empty rating remains native -2, decayed replacement raw4/3/0 at durability0.305293947 yields native -4/effective0. User confirms visuals. Initial five diagnostic actions preserve physical hash and pass native GUI observation; replacement GUI has no separate automated observation. Body/part probe fields on replacement are zero. No destination checkpoint was recorded, so live destination non-export is visually supported but not instrumented. Cleanup is followed by successful native save; no cleanup reload recorded. Exact identity/limits: TEST_BUILD_LOG.md installed return. Prior0.1.1 remains accepted for its bounded session-only behavior. Full production, payment, coverage, zone/hint refresh and migration remain open.
