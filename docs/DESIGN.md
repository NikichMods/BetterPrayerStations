# Design — Better Prayer Stations

Status2026-10-26: adopted current concept. Trial balance and accepted artwork are distinct from final production/runtime acceptance. See PROJECT_STATE for build status; earlier combined requirements are archived under history.

## Purpose

Make prayer stations useful through visible, reusable blessings placed around the cemetery. The player spends faith to unlock additional simultaneous blessings, then freely chooses where to use them. A blessing may be decorative or affect an occupied grave.

## Global capacity and placement

There is no station radius and no individual station ownership of fields/purchases. All cemetery prayer stations open one shared system.

| Value | Meaning |
| --- | --- |
| P | Permanent purchased blessing capacity in the current save |
| C | Current simultaneous ceiling supplied by completed eligible stations |
| N | Number of positioned fields |
| min(P,C) | Usable simultaneous capacity |
| A=min(N,P,C) | Number of active positioned fields |
| F=max(0,min(P,C)-N) | Additional fields that may currently be placed |

Initial tuning values: completed wooden station adds5 to C; stone adds10. Unfinished, removing or floating preview stations do not provide support in the current probe. A purchase adds1 to P only while P<C and the selected trial price is affordable.

Faith buys a reusable slot, not a particular field object. Placing/removing fields is free, does not change P or global price progression, and does not refund faith. Use Place/Remove; no separate move tool or per-paid-beam reserve screen. Remove and place elsewhere to reposition.

Demolishing stations retains P and field locations. P>C is valid. Oldest positioned fields stay active first; newest go dark first. Restoring C reactivates fields in the same order without repayment. Remove/reinsert makes a field newest. Persist placement order; derive active/dormant status from current support rather than saving it as authority.

## Spatial effect

- Free placement on available cemetery ground, including places without graves. Do not turn this into a grave-only picker or inherit a station radius.
- Any native construction cell occupied by a grave qualifies. Multiple fields on one grave do not stack. Current prototype rejects exact same-cell duplicate points.
- An actively blessed grave converts all effective red skulls to white in its burial calculation. No separate+1 quality bonus. Preserve native decoration caps and durability/decay; blessing does not repair the body.
- Physical body, organ, item and zombie data remain unchanged. The result is local to a supported grave and cannot be exported with a body.
- Fields are spatial. Exhumation leaves the field in place; an empty grave has no numerical benefit. A replacement body receives the effect at the same location. Moving the grave/body does not move the field with it.
- Field markers, including dormant ones, must be selectable/removable in management mode. A permanently visible dormant marker during ordinary play is not required.

The functional rule stays visible throughout placement: «На могиле превращает все красные черепа в белые.» Exact world-grid presentation still uses the technical probe.

## Prayer price

One shared progression for the cemetery, independent of which station is used. First ten trial prices: **1,2,3,4,6,9,12,15,18,21 faith**. Paid capacity is permanent; removal/support loss does not reset the sequence. Existing trial capacity is not charged retrospectively.

Continuation after10 is unresolved. Current probe stops purchases after that row; this is a diagnostic bound, not the selected final product limit. Do not invent later prices or silently remove the bound. Wooden/stone capacities and prices need ordinary gameplay tuning with user agreement. No new technology/unlock/upkeep requirement has been selected.

## Menu and artwork

Adopted: native construction-style window with three actions, «Помолиться», «Благословить место», «Снять благословение». Distinguish P, C, active/free counts and conditional dormant count. Faith cost belongs on the prayer row. Do not display P/C as a fraction because P>C is legitimate.

[STATION_MENU_PROPOSAL](STATION_MENU_PROPOSAL.md) owns exact accepted Russian copy and pinned icon files. Accepted trio: book/rosary/large cross100×100, lit grave37×37, unlit grave37×37. These source dimensions do not prescribe the physical native UI scale.

Menu0.5.0 implements a first native preview; artwork/copy acceptance does not imply actual NGUI layout acceptance. Current installed state belongs to PROJECT_STATE/TEST_BUILD_LOG.

World-light direction: restrained descending God Rays, brighter above and weaker below, related to the grave/icon reference. Final world art, density/contrast and inactive presentation are deferred; the current effect uses technical markers. Do not claim an approved final beam from an icon concept.

## Excluded and still open

Excluded from this version: passive station-quality aggregation, organ inventory inspection, individual incremental paid skull upgrades, separate wreath upgrades, commissioned gravestone/fence appearances, radius/nearest-station ownership, upkeep or new services. Historical Appearance/Faith probes do not reintroduce them.

Open: final world rays, final economy/continuation, production localization/packaging and targeted remaining lifecycle/input/compatibility acceptance. Base game is primary; Move Objects support is optional and must not become a runtime dependency.

The user wants to discover anatomy personally. The completed blind static screen found no severe issue requiring immediate concept revision; it does not prove optimal-strategy nondominance or final balance. Preserve the spoiler boundary in EVIDENCE_AND_ACCEPTANCE.
