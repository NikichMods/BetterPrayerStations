# Design — Better Prayer Stations

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

Input is the sum of grave contributions, not the whole zone total, the station itself, other prayer stations or unrelated decorations. No recursive station-to-station amplification. Using current effective grave quality (including faith improvements and native body/decoration limits) is the recommended interpretation; exact inclusion of empty/negative graves, rounding/clamping and preview settlement require an explicit final formula before runtime implementation.

Do not silently adopt the earlier zero-base proposal: whether the bonus adds to native station base quality or replaces it remains open. Wooden/stone passive-rate differentiation is not selected. Whether the same grave contributes to multiple overlapping stations is also open; naive reuse permits extra station placement to multiply this bonus without improving graves. Do not invent first-built assignment, distance splitting or overlap rules without settling this product choice. Requirements for source enumeration, native zone notification and bounded update ownership need technical evidence before a new passive-quality probe.

## Placement and coverage presentation — adopted 2026-10-23

Prayer stations should additionally be placeable on the ordinary grave-eligible ground of the cemetery, retaining normal footprint, occupancy and collision restrictions. This adds layout freedom alongside existing placement options; it does not permit overlap with existing graves or remove native construction costs. Native build-mask/config integration remains to be verified.

During construction placement preview, show how that proposed station would change nearby grave results before committing the build. This feedback is desired, not an error to remove. Distinguish previewed coverage/result from committed built-station coverage; cancellation must restore the actual scene's result, and completion/removal must refresh affected world quality badges without requiring the player to refocus a grave. Exact behavior during an unfinished construction project remains to be resolved with native lifecycle evidence; a registered preview must not be silently described as a completed station.

The prayer station's quality badge should sit near the center of its own visible sprite to avoid covering the grave behind it. This adjustment targets station badges, not a global shift of all grave labels. Exact anchor/offset needs native rendering inspection and visual acceptance. A radius overlay was absent from diagnostic0.3.0; no final circle presentation or overlay controls have been adopted yet.


Diagnostic-only addition2026-10-23: user authorizes a temporary visible radius to simplify move/cancel tests. This does not select final radius art/UI. While moving an existing station, previewed coverage replaces that station's original position; it is not an additional station. Cancel returns original coverage. New-construction preview is separately labelled and is not counted as built. Exact candidate/proof: Faith0.6.0 PLAN/README.

## Square influence and covered-grave feedback — adopted2026-10-23

Influence becomes an axis-aligned square centered on the prayer station, with dimensions tied to the actual cemetery construction lattice. This supersedes circular geometry as intended product behavior; frozen Faith0.6.1 remains an accepted circular diagnostic, not the new implementation. Specify dimensions as side length in small construction cells, avoiding ambiguous radius labels. Exact wooden/stone side lengths and logical station-center/edge alignment remain to be selected. Native BuildGrid uses32-world-unit cells; previous diagnostic unit96 equals three such cells. Center and bounds need verified footprint/grid alignment, not the tall sprite's apparent center.

User proposes translucent green grave rectangles during station movement/placement to identify affected graves; adopt this feedback direction. Highlight must match actual coverage and passive-quality membership, follow prospective station position, preserve the already accepted native world-quality preview, and restore actual coverage on cancel. Whether membership requires the whole footprint, center or another grid rule remains open; full-footprint containment is a candidate to avoid marking half-covered graves, not yet adopted. Do not silently enlarge eligibility solely to make highlights look complete. Full population enumeration/footprints and render/occlusion/input behavior still require research.

Working tier discussion favors different square size over wooden action caps, with identical actions/prices suggested in chat. The previous adopted cap table above is under review; no final cap-removal decision or new cap implementation is made in this design segment. Resolve this before the deferred tier-purchase probe. Square geometry, grid units and passive-divisor10 are adopted independently of that remaining tier choice.