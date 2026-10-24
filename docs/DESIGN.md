# Design — Better Prayer Stations

## Current design revision — blessing field, 2026-10-24

Status: **adopted core concept; balance, detailed presentation and native feasibility remain open**. This section takes precedence over the earlier combined product concept below where they conflict. Earlier research and bounded runtime acceptance remain evidence for their original scenarios, not acceptance of this revised product.

The user agrees to simplify the former multi-system product. The product centers on visible blessings. **Passive station-quality aggregation is excluded from this version.** Do not carry the former organ inspection, incremental paid skull conversion, repeatable grave-upgrade or appearance-commission services into the revised scope without a new decision.

Adopted interaction: a prayer station acts conceptually as a construction station through which the player places a one-square blessing field. The field's position anchors the visual effect. An actively blessed grave **converts all its effective red skulls to white skulls for the burial calculation, with no separate +1 quality bonus**. This replaces the earlier +1 proposal. The effect is nonstacking: one active field is sufficient; additional fields cannot repeat conversion or add quality. Original body/organ data remain unchanged; this is not an exportable body/organ/zombie improvement. Native decoration and decay limits still matter; conversion does not repair durability. The spatial field owns the local effect, but is not individually purchased: faith buys fungible cemetery-wide capacity. This is a product model, not a verified native construction/save implementation.

The proposal includes faith payment and a light beam. **Adopted: faith price progression is shared across the whole cemetery, not reset per station.** Exact price curve remains open. Do not infer that this field is the former station coverage square or inherits its radius/nearest-station ownership rules.

Full effective red-to-white conversion is now explicitly adopted. Merely ignoring red penalties is not the selected effect. This deliberately supersedes the earlier partial-conversion limit: blessing replaces red-skull cleanup for the burial result while preserving the physical body and native decoration/decay constraints. Adopted design is not installed-runtime acceptance.

### Adopted purchased capacity and free field placement

There is **no station influence radius**. Completed prayer stations add to one cemetery-wide capacity ceiling. Initial in-game tuning values are **wooden +5 / stone +10**, now selected for trial rather than final balance. Fields and purchases are not assigned to an individual station; any station provides access to the same purchased capacity and shared price progression.

Faith buys **one more simultaneous beam slot**, not a particular positioned beam. Track permanent purchased capacity P, station-supplied ceiling C, and placed field locations separately. Usable simultaneous capacity is min(P,C); a new purchase increases P by1 and is available only while P<C. Placing/removing a field is free and does not advance/reduce P or the shared price progression. There is no individual paid-beam identity or reserve-entry selection to preserve.

Demolishing stations does not erase P or saved field locations. With C=0, all beams go dark and their grave effects cease. Rebuilding sufficient station capacity restores support without payment or another purchase. P>C is a valid temporary state, not grounds for deleting investment. Dormant fields remain removable in management mode. **Adopted priority: earliest placed fields stay active; latest placed fields go dormant first.** Increasing support restores fields in the same placement order. Removing and placing a field again makes it the newest/last field; placement order persists through save/load. Do not preserve fictional per-field purchase identities.

User authorizes Menu0.3.0 persistence prototype for this global P/position/order state after bounded0.2.0 effect acceptance. Use the native per-save carrier under GLOBAL_BEAM_PERSISTENCE.md; support/active flags remain derived. No faith debit or visual polish is part of this slice. Diagnostic schema selection is not a final production migration-format promise.

Neither the old nearest-station rule nor the earlier per-station beam ownership proposal applies.

### Adopted free placement and reusable paid beams

The player may place fields freely on cemetery ground, including locations without graves; placement is not restricted to a grave-only target picker. This is product intent, not permission to bypass every native collision/terrain rule without host inspection. Multiple fields affecting the same grave do not stack their numerical effect; exact same-cell occupancy is not selected here.

During placement the rule must remain visibly available throughout, rather than appearing only in a one-time explanation: **"When placed on a grave, converts all red skulls to white."** Russian working wording: **"При размещении на могиле превращает все красные черепа в белые."** The rule explains the functional use without removing decorative placement freedom. Exact text/layout/localization and active/dormant contextual feedback require native UI iteration.

The player uses **Place / Remove**, not a dedicated movement mechanic. Removing a field frees space within the purchased simultaneous capacity; it does not refund faith or reset/reduce the shared price progression. A field can then be placed elsewhere for free. No Move Objects dependency or persistent individual paid-beam reserve UI is required. This supersedes the previous per-entitlement purchase-order model.

Exhumation leaves the spatial field at its location; an empty grave receives no numerical benefit. A replacement body at the same blessed grave receives the selected conversion. The field does not travel with an exhumed body or a moved grave; applicability is spatial. Exact membership/footprint detection across native states still needs evidence.

Adopted membership: **any native construction cell occupied by the grave qualifies**; no special central-cell requirement. Host evidence and remaining runtime limits are recorded in `BLESSING_MEMBERSHIP_RESEARCH.md`. The user authorizes the next session-only full-conversion prototype, without payment, persistence or visual/menu polish. Adoption is not installed effect acceptance.

### Construction-style management direction

Interact with any prayer station to open a shared construction-style management interface. It must offer: buy +1 capacity with the next faith price; place a field; remove a field. Show station ceiling, purchased capacity and remaining placement availability distinctly. Placement exposes the cemetery grid; removal exposes existing field positions. Field markers must remain clearly visible over graves in these management modes, including dormant fields, without requiring permanent ordinary-gameplay dormant markers. Exact native menu reuse/customization and grid feasibility are unverified. This is the agreed interaction direction, not a selected native implementation.

### Initial trial prices

The user wants an inexpensive first trial rather than the former meaningful-first-sacrifice policy. **Adopted first-ten trial prices: 1,2,3,4,6,9,12,15,18,21 faith**, totals16 for the first5 and91 for the first10. These are selected for in-game tuning, not accepted final balance. Continuation beyond10 is not selected. Earlier alternatives in conversation are not concurrent requirements. The user's approximate10–15 faith per weekly sermon at first station access is a scenario assumption, not established timing/income evidence. Exact decoration faith costs and marginal quality comparisons require canonical economy data before using them as balance evidence.

### Audience and balance hypotheses to validate

Intended overlapping motivations: less red-skull cleanup for burial, expressive cemetery lighting, and experimenting with anatomy under a new burial calculation. Broad usefulness to all players, sustained late-game demand and a new optimal late-game anatomy route are hypotheses, not established audience/runtime evidence. For a healthy blessed body, effective white capacity becomes the clamped native red-plus-white total, still limited by decoration. Adding reds alone can therefore be beneficial, but exchanging white for red at equal total is not an improvement; specific native procedures/availability/costs must be checked before promising progression or a new maximum. A permanent reusable purchase is a finite faith expenditure, not an ongoing upkeep or guaranteed endless resource sink. Check whether stronger early conversions become the obvious preferred route, rather than assuming a price per beam equals a price per quality point.

The bounded static blind balance screen2026-10-24 found no severe issue requiring immediate concept revision. Keep the selected concept/trial balance pending real gameplay; this does not establish strategy nondominance or final tuning. `EVIDENCE_AND_ACCEPTANCE.md` owns the nonspoiler evidence boundary; do not disclose detailed anatomy results in ordinary discussion/recovery.

### Remaining balance, interaction and feasibility work

- Balance/access: validate initial5/10 capacity in game; select faith price curve and whether existing station unlocks suffice.
- Interaction: exact placed/dormant count and placement availability display; placement/cancel/confirmation flow. The functional rule must remain visible during placement.
- Presentation: permanent light intensity at single/many-field density, deferred while mechanics are settled.
- Host feasibility and edge cases: precise spatial grave membership (one square is not yet a verified native cell/footprint), terrain/occupancy restrictions, native management/grid lifecycle, persistence/uninstall and supported compatibility. Native UI/visual proof remains an implementation acceptance task, not a missing gameplay system.

## Earlier combined concept — retained pending design reconciliation

Status: **adopted product concept**, 2026-10-23. These requirements describe intended behavior, not implemented or runtime-verified features. This document is the product owner; the earlier research idea bank is background, not a second contract.

## Purpose and boundaries

Give the player optional, costly ways to improve a desired cemetery through existing prayer stations. Faith pays for functional improvements to one grave; money pays for preferred decoration appearance without reducing efficiency. The mod intentionally provides a **partial** alternative to repetitive anatomy work, preserving anatomy's broader role.

It primarily serves players still interested in their cemetery. It does not promise an endless resource sink, new goals for every late-game player, or demand for every production chain.

No surrounding-grave quality gate, arbitrary set of three special graves, ingredient-preparation ceremony or compulsory new content is part of this design.

## Inspection and faith actions

A grave covered by a prayer station provides free inspection of the body's actual contained items and quantities, including non-organ contents when present. Inspection does not disclose numerical skull contributions of individual organs; it must preserve the intended role of Cultist. Example inventory lists are not assumptions about normal native bodies.

For the selected grave, the player chooses either:

- Convert one effective red skull to one effective white skull.
- Add one effective decoration/wreath point without replacing physical decoration.

Both are complementary options in **one escalating payment sequence per grave**. Each paid action advances the same counter. Switching action, replacing a body or rebuilding a station never resets it. Buying cheap first steps on more graves versus expensive later steps on one grave is an intended allocation choice.

The first payment must be a meaningful early sacrifice; 10 faith is an illustrative user proposal. Subsequent prices, curve, caps and income calibration remain open. The preview's 10/20/40 sequence is not a final balance formula. Do not present a faith amount as a fixed number of weeks independent of sermon/income assumptions.

The action changes the **effective burial result**, not the physical body, organs or original decoration. It does not create independent white skull capacity after all eligible red skulls are exhausted. Available white capacity, decoration, decay and the native rating formula must remain distinct; converted white skulls obey the same native durability/decay limit as original white skulls (explicitly adopted 2026-10-23). Conversion is applied to effective total whites before available-white rounding; it does not restore durability.

## Coverage and station tiers

| Coverage of a grave | Existing paid faith effects | Further purchases |
| --- | --- | --- |
| None | Stored, dormant | Unavailable |
| Wooden only | All active, even beyond wooden purchase cap | First N total paid actions for that grave |
| Stone, including overlap with wood | All active | No station-tier purchase cap |

The highest covering tier governs purchase permission only. Any station activates **all** stored effects. No first-built priority, partial activation order or permanent assignment to a station is needed. N and radius are unbalanced; two actions is an example, not a fixed number. Lack of a stone-tier cap does not remove anatomy/rating limits.

Moving or demolishing a station re-evaluates current coverage. It does not delete investments, refund them or reset price progression. Returning any station restores all stored effects. Wooden stations therefore remain useful for maintaining graves developed under stone coverage.

Inspection also requires coverage. Purchased appearances remain on the grave and do not switch off outside coverage.

## Ownership, exhumation and replacement

| State | Owner | Effect of exhumation/removal |
| --- | --- | --- |
| Original body and native anatomy/embalming | Body | Leaves with body |
| Bought conversion capacity, extra wreaths, shared action count | Grave | Remains stored at grave |
| Paid gravestone and fence appearances | Grave, separate choice for each slot | Remain at grave; removed items are ordinary |

An empty grave contributes no bonus from faith investments. A replacement body uses the grave's stored investments when covered. Effective conversions are bounded by the actual eligible red skulls: unused capacity cannot manufacture extra white skulls for an already red-free body. The same investment can yield a different rating for another body.

Exhumation exports no grave modifier to another grave, organs or zombies. There is no automatic refund or count reset. True body improvements made through native systems still follow the body normally.

Full grave deletion, third-party grave movement, identity replacement and mod uninstall behavior require explicit supported-case rules after host inspection; no claim of compatibility or impossibility of grave deletion is established.

## Appearance commissions and fitting

The first version uses **existing available game appearances** for gravestones and fences. The player pays craftspeople to customize a specific grave. The grave stores separate gravestone/fence appearance choices; quality/function still comes from the physical item installed in each slot. A wooden-looking decoration can retain the original high-quality item's effectiveness. No mandatory best-looking tier or quality penalty for visual preference.

Ownership revised and accepted 2026-10-23: **appearance belongs to the grave; quality belongs to the installed item**. Removing a decoration returns an ordinary item and retains the appearance choice at the grave. An empty slot renders no paid decoration. Installing another compatible item uses the saved grave appearance with the new item's actual quality. On a different grave, the moved item uses that destination's own appearance choice, or its normal look if none is selected. Body replacement/exhumation does not reset either appearance.

Show the actual installed item and its quality separately from the grave's chosen appearance. No cosmetic tags, special inventory icons, item-level appearance tooltips or metadata transfer through removal recipes are required. Inventory/storage continue to show ordinary items. This supersedes the former item-owned model; it is an intentional product simplification, not a claim that item metadata is impossible.

Adopted fitting flow:

1. Select the installed decoration/slot and browse available looks.
2. Temporarily preview the look on the **actual grave in the actual cemetery**, with surrounding context visible.
3. Confirm the displayed price or cancel.

Browsing and cancellation spend nothing and do not commit item changes. Confirmation commits the grave-slot appearance choice and payment together. Closing/interruption must restore committed presentation; exact handling belongs to implementation acceptance. A separate scene/view rendered inside a window is not required. The earlier schematic illustration was an interaction aid, not a proposed second game renderer.

The exact meaning of available, gravestone/fence compatibility, prices, repeat-commission payment, the price/access for clearing a saved override and ordering-access requirements remain open. A missing override uses the installed item's ordinary appearance. Restricting to learned decoration technologies was suggested but **not adopted**.

## Information and interface

Distinguish actual body state, grave investments and effective current result. A compact presentation is accepted, for example:

> Body: 3 red, 3 white.
> Grave improvements: 3 conversions -> effective 0 red, 6 white.

Show decoration plus extra wreaths separately; also show whether faith effects are active or stored/dormant, current covering tier, paid action count, next shared price and actual expected rating change. Individual organ-quality contributions remain hidden by this feature.

Tabs are a possible layout, not a final UI decision. An early interactive browser example compared tabs and disclosures; neither establishes native NGUI fit or controller acceptance. Use a separate iteration to settle concrete native presentation.

Proposals for compact exhumation consequences and affected-grave/quality-loss warnings before station removal are useful follow-ups, not approved mandatory new dialog flows. Avoid unnecessary nested confirmations.

## Outside initial scope

- Complete anatomy replacement or independent white-skull creation.
- Additional sermons, faster corpse deliveries, calendar skipping and fishing services.
- Technology-point sinks or a compulsory new research tree.
- Sarcophagus rewards/coin investment, columbaria extensions, cookies, candles and perfume systems.
- New Gothic/nonwestern/carved/ivy/metal decoration art. These remain possible later cosmetic content, not first-version promises.

## Remaining decisions

Product direction is complete enough to start technical planning. Numeric balance, radius, exact appearance availability/payment/access and supported edge cases remain explicit design tasks. Architecture, persistence and host UI feasibility are unverified implementation questions, not permission to silently remove adopted behavior.

### Progression policy — price-led access; milestone gates deferred

The user explicitly chose to keep access price-led for now, without aristocrat-paper or cathedral hard gates. Free body-content inspection remains available from early station access. Aristocrat papers (the previously discussed 12-gold milestone) for appearance commissions and cathedral progression for faith/skull services are retained only as deferred alternatives, not implementation requirements. Do not interpret this as removal of the agreed inspection radius. Exact native progression flags and document possession/consumption semantics need verification only if this direction is selected.

Tradeoff: explicit milestones communicate intended late-game timing and guarantee delayed access, but remove the deliberate option to spend scarce early faith. A cathedral gate also changes the role of the wooden station's first-N paid actions from an early working version to a post-unlock purchase cap. These consequences must be settled before adopting either gate. Appearance unlock and functional unlock need not use the same policy. The exact action scope of a proposed cathedral gate (conversion only or both faith actions) remains unspecified.

Current access policy is explicit: price rather than those hard progression gates. Exact prices and appearance-catalog availability remain open. No gate is added to Appearance Probe 0.1.0, whose frozen source/artifact and requested runtime scenario remain unchanged. Reopen milestone gating only on a later explicit product decision.
### Station quality from surrounding graves — adopted direction, initial divisor10

The user now adopts a passive prayer-station quality bonus from surrounding grave quality, with an initial threshold of10 instead of the earlier illustrative5: +1 station quality per10 total grave quality in its own influence area.10 is a first tuning value, not final balance. This is a benefit of improving nearby graves, not a minimum-quality gate for inspection or faith actions.

Input is the sum of positive current grave quality, not the whole zone total, the station itself, other prayer stations or unrelated decorations. User explicitly selects positive-only contribution and an initial +1 per complete10 quality. Nonpositive graves contribute0, never subtract from positive neighbors. Read current effective grave quality (including active faith improvements and native body/decoration/decay limits), consistent with the quality displayed to the player; no separate raw-skull/decor resource is introduced. Empty graves receive no special positive contribution: the same actual-quality condition applies. No recursive station-to-station amplification. Preview settlement/update ownership still require technical evidence.

User explicitly chooses to ADD the passive bonus to the station's ordinary native quality; the earlier zero-base/replacement proposal is not selected. Wooden/stone passive-rate differentiation is not selected. User prefers that one grave contribute to at most one station passive bonus, provided the mechanism stays simple/reliable. Naive reuse is rejected as the intended direction. Nearest eligible station with stable persistent-ID tie break is now accepted in direction following explicit agreement with the proposed mechanics; native geometry/update reliability remains to be verified. Service/faith-effect coverage must remain independent: any covering station activates all saved effects. Do not invent first-built assignment or distance splitting as requirements. Requirements for source enumeration, native zone notification and bounded update ownership need technical evidence before a new passive-quality probe.

## Placement and coverage presentation — adopted 2026-10-23

Prayer stations should additionally be placeable on the ordinary grave-eligible ground of the cemetery, retaining normal footprint, occupancy and collision restrictions. This adds layout freedom alongside existing placement options; it does not permit overlap with existing graves or remove native construction costs. Native build-mask/config integration remains to be verified.

During construction placement preview, show how that proposed station would change nearby grave results before committing the build. This feedback is desired, not an error to remove. Distinguish previewed coverage/result from committed built-station coverage; cancellation must restore the actual scene's result, and completion/removal must refresh affected world quality badges without requiring the player to refocus a grave. Exact behavior during an unfinished construction project remains to be resolved with native lifecycle evidence; a registered preview must not be silently described as a completed station.

The prayer station's quality badge should sit near the center of its own visible sprite to avoid covering the grave behind it. This adjustment targets station badges, not a global shift of all grave labels. Exact anchor/offset needs native rendering inspection and visual acceptance. A radius overlay was absent from diagnostic0.3.0; no final circle presentation or overlay controls have been adopted yet.


Diagnostic-only addition2026-10-23: user authorizes a temporary visible radius to simplify move/cancel tests. This does not select final radius art/UI. While moving an existing station, previewed coverage replaces that station's original position; it is not an additional station. Cancel returns original coverage. New-construction preview is separately labelled and is not counted as built. Exact candidate/proof: Faith0.6.0 PLAN/README.

## Square influence and covered-grave feedback — adopted2026-10-23

Influence becomes an axis-aligned square centered on the prayer station, with dimensions tied to the actual cemetery construction lattice. This supersedes circular geometry as intended product behavior; frozen Faith0.6.1 remains an accepted circular diagnostic, not the new implementation. Specify dimensions as side length in small construction cells, avoiding ambiguous radius labels. Exact wooden/stone side lengths and logical station-center/edge alignment remain to be selected. Native BuildGrid uses32-world-unit cells; previous diagnostic unit96 equals three such cells. Center and bounds need verified footprint/grid alignment, not the tall sprite's apparent center.

User proposes translucent green grave rectangles during station movement/placement to identify affected graves; adopt this feedback direction. Highlight must match actual coverage and passive-quality membership, follow prospective station position, preserve the already accepted native world-quality preview, and restore actual coverage on cancel. Center-point containment supersedes full-footprint containment: the square must include the geometric center of the grave ground footprint, including a point exactly on the boundary. The full grave need not fit inside. A dedicated central cell is not required; the point can lie between cells. Actual native grid/footprint origin, offset and stability across body/decoration states still require verification before implementation. Full population enumeration/footprints and render/occlusion/input behavior still require research.

Working tier discussion favors different square size over wooden action caps, with identical actions/prices suggested in chat. The previous adopted cap table above is under review; no final cap-removal decision or new cap implementation is made in this design segment. Resolve this before the deferred tier-purchase probe. Square geometry, grid units and passive-divisor10 are adopted independently of that remaining tier choice.
## Base-game-first support and station focus feedback — adopted direction

Support the standard game without Move Objects; that mod is optional compatibility, not a product requirement. Ordinary new-construction preview, cancel, unfinished construction, completion and demolition must work with native mechanics. Current accepted0.6.1 movement is evidence only for the exact installed Move Objects0.6.0 environment; it does not establish a tested no-Move-Objects configuration. The probe's installed-payload offline regression fixture requirement is not a shipping/runtime dependency. Future probe plans must include native workflows and avoid silently requiring the move mod for core acceptance.

In ordinary gameplay, approaching/focusing a prayer station should reveal its square influence and useful grave-effect information; no building/moving mode is required. Use one focused station at a time as the proposed low-clutter presentation; exact native focus/accessibility/trigger lifecycle is unverified. Show its passive quality bonus and the underlying counted-grave total clearly. A progress-to-next-threshold display is optional/deferred, only if there is an unobtrusive place for it; native base quality must remain distinguishable from the added passive bonus.

User now adopts numeric committed-value plus signed-delta preview beside the existing grave quality number: for example3 +1, with positive delta green. Keep the old committed number unchanged during preview; do not print projected total, equals sign, duplicate labels or words such as total. On confirmation update the ordinary number to committed result and remove temporary delta; cancellation retains original number and removes delta. Negative delta is necessary for moving coverage away; muted red and hiding zero are proposed presentation details. No mandatory always-visible popup flow. Ordinary approach/focus retains the actual current total above each grave with no decomposition or prospective delta. It still reveals area/highlights and station information. Signed differences are shown only for native new-construction preview and optional movement preview. Exact label spacing and native input presentation remain UI implementation tasks.

Coverage highlighting and passive contribution ownership are different: a grave can receive services under overlapping squares but contribute passive quality to one station. A proposed small contribution marker plus station counts can express this; do not label every green covered grave as contributing to the focused station without computing ownership. Nearest-eligible ownership and stable tie-break are adopted in direction; implementation/acceptance remains open. No new runtime artifact or native visual acceptance follows from the explanatory mockup.
### Numeric baseline and center clarification —2026-10-23

The user explicitly replaces full-footprint inclusion with center-point inclusion and agrees to the other proposed mechanics/nuances, including single passive contributor via nearest eligible station. The native central-cell existence is unknown and unnecessary to the intended rule: use a verified logical footprint center, never the image center or an unverified transform origin. Same point rule drives service coverage, passive eligibility and green whole-grave highlight. A whole highlighted grave may therefore extend past the square boundary; this is intentional under the new center rule.

Construction/move preview must preserve the actual committed grave quality as baseline throughout the preview, including existing faith improvements and all real covering stations. Signed delta is proposed effective quality minus that committed baseline, not full saved bonus added again. Recompute projected result without overwriting baseline on each cursor step. Station bonus progress-to-next-threshold is not required; core station bonus and counted quality remain visible.

Resolved by explicit user reply: ordinary approach/focus leaves current total unchanged (for example3); do not split it into2 +1. Adjacent signed differences appear only while previewing construction or movement, compared with the committed pre-preview quality. Existing schematic already follows this choice. This closes the numeric-baseline clarification; no runtime acceptance or new build is implied.

### Passive quality formula resolved —2026-10-23

User selects positive-only grave quality, initial divisor10, and explicitly confirms adding the bonus to ordinary station quality. For each eligible completed station s:

`bonus(s) = floor(sum(max(0, current_grave_quality(g))) / 10)`

The sum includes only graves allocated to s by the adopted nearest-covering-station rule with stable persistent-ID tie-break. A grave contributes to at most one station. `station_quality(s) = ordinary_native_station_quality(s) + bonus(s)`. Examples: positive sums0..9 -> +0;10..19 -> +1;20..29 -> +2. Negative or zero graves do not reduce neighbors' contributions.10 remains initial tuning, not final balance. Ordinary native quality is retained; no stations or unrelated zone decorations enter the grave sum.

Interpretation of the user's quality wording is the actual current displayed/effective grave quality, including currently active faith improvements, rather than a new underlying raw-body-only quantity. This keeps the adopted grave-improvement -> station-benefit loop visible and respects native decay/caps. The implementation must keep native original station quality separate from its projected bonus to prevent self-reading. Center membership, exclusive passive allocation versus nonexclusive service coverage, and native-first/optional-Move support remain unchanged. No new binary is authorized or produced merely by this decision capture; next technical preparation must establish the real station-quality/zone consumer and recomputation/preview boundaries before a narrow passive-quality candidate.
