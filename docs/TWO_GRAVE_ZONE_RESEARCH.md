# Two-grave and native zone integration research — 2026-10-23

Status: research-only0.5.0 candidate, offline gates pass; live simultaneous/native-zone/closed-window return pending. DESIGN.md owns mechanics. No final prices/radius/caps or product visual work is selected here.

## Exact question and native owners

Single-owner0.4.0 acceptance is reused. It intentionally authorizes only a selected grave, so it does not establish production-like simultaneous operation.0.5.0 bounds the next step at two saved marks and makes selection only an edit/purchase address. Actual quality/diagram inputs use each grave's body, root Item C/L and actual coverage; per-grave N=C+L prices retain native payment backend.

Installed GK1.407 Assembly-CSharp remains e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8. Fresh IL corroborates WorldZone.GetTotalQuality: None returns0; Sum iterates native _wgos, skips obj_def.ignore_counting_at_zone and adds each WGO.quality; Average additionally divides by member count. It invokes static OnQualityComputed(id,result) before returning. GetZoneWGOs simply returns _wgos. Native WGO._zone supplies cached membership owner; WGO.GetMyWorldZone instead performs Physics2D.OverlapPointAll, and WorldZone.GetZoneOfObject iterates all zones. Use verified cached owner/member list at explicit checks, not recurring physics/global searches.

Readiness for this bounded research: owner membership, getter/diagram inputs, payment and native sum/event are established. Production remains open beyond two graves, final saved format/tier/preview lifecycle and related UI. No public game code/binaries are imported.

## Mechanism and observation

TwoGraves explicitly holds up to two current WGO refs with reference-based membership, duplicate-live-ID rejection and stable ID order. Register only marked graves; unmarked ones pass through. Same native root fields, no new coordinate/ID save ledger. Existing loaded-world traversal recovers marks/stations; not a recurring scan. Native current save/player readiness retained; unknown/third mark rejects rather than discards. All effects run without GUI. Purchases remain selected-open/current/covered; record unchanged other-owner state and physical data.

Closed check reads native individual vanilla qualities with projection disabled only for those leaf reads (no zone callback), then restores projection, computes the two deltas and calls real GetTotalQuality once with effects on. Its real result must equal native baseline sum+two deltas and deliver both marked input seams; a temporary listener must observe native event/current total. Never raise OnQualityComputed manually or call GetTotalQuality on a fake disabled baseline. Real native quality mirrors may update normally. Reflection validates cached owner field; verified public GetZoneWGOs reads members. Only Sum zone supported in this diagnostic.

User requests a sample through console; it waits until native windows and floating preview close. Sample restores no fabricated scene or anatomy and triggers no artificial purchase. After load with two marks, one sample is requested automatically, again waiting for closed windows. Logs include per-owner C/L/N/price/raw/effective/coverage, full native total and exact unchanged-physical/currency assertions. Subsequent world badge freshness/placement rendering is outside scope.

## Offline evidence and live boundary

Actual TwoGraves identity/order/cap/duplicate handling passes; headless native getter IL shows both positive owners and independent dormant selection, both-owner native map serialization including mixed C/L. Actual native WorldZone.GetTotalQuality executes on raw synthetic native fixtures, ignores a marked-as-ignored third object and emits its actual result once. Separate simulated-debit/native-count tests show A1/A2/B1 prices and independent restored counters. These are intentionally separate boundaries, not a synthetic replacement for the combined live zone calculation.

Pending live scenario: two marked healthy graves in one native Sum zone, A C1/L1/N2 (next3), B C1/L0/N1 (next2), total cost4; closed sample -> move station so only one loses coverage -> closed sample -> restore -> closed sample -> save/menu-load both -> automatic closed sample and individual native GUI checks -> cleanup both/save. Exact steps README, frozen identity TEST_BUILD_LOG. No stable promotion or final balance acceptance.
