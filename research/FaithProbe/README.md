# Better Prayer Stations — Faith Probe 0.2.0

Research-only saved-investment test. No faith payment, prayer-station radius or production release. Version owner: Plugin.cs Version; installed BetterPrayerStationsFaithProbe.dll. Exact source/artifact/install identity: docs/TEST_BUILD_LOG.md. Earlier numbered artifacts remain immutable; do not run Appearance Probe alongside this test.

## What changes

Three diagnostic GameRes parameters on the selected grave Item: nikichmods.bps.faith_probe.schema (1), .conversions (C), .wreaths (L). Successful-action count N is derived as C+L. This is a diagnostic format, not a production save-format promise. Counts are bounded at12 each for test/UI safety, not balance. Actual body, organs, durability, decorations, faith and money are never edited by the probe. Native exhumation and construction still have their ordinary costs/effects.

The same accepted native rating/central-diagram input projection remains. Conversion uses min(C,current red), follows native decay for converted whites, and extra wreaths retain native white caps. Empty graves keep their native result. Investment remains on the grave when the body changes; an unselected destination receives no effect. Activation is an explicit session flag, defaults enabled after load and does not simulate real coverage.

A one-time pass of existing WorldMap.objs after OnGameStartedPlaying restores the single marked grave. Multiple marked graves, unknown schemas or invalid counts disable the probe rather than erase data. No recurring world scan. Cleanup clears only the three diagnostic parameters and MUST be followed by an ordinary save. Removing the DLL alone leaves inert saved parameters.

## Short live test: removal, replacement and two loads

Use the earlier grave A if convenient. Panel is draggable/scrollable; F8 hides it.

1. Open occupied idle A and choose **Выбрать открытую могилу**. Add three conversions and one wreath, waiting after each click. These are stored C3/L1, N4; effective numbers depend on the current body. Use **Записать этап: могила и другое открытое захоронение**.
2. Through ordinary game actions, remove decorations as needed and exhume A's body. Bury it in another, unmarked grave B. Do not select B in the probe. Open B and **Записать этап…**: logs capture both empty invested A and ordinary B. Save normally, return to menu and load. Open B and record another checkpoint. A must retain C3/L1/N4 while empty; B must use ordinary anatomy/rating.
3. Bury a different body in A (an existing exhumed body is fine); reinstall ordinary decorations. Open A, **Повторно проверить рейтинг и шкалу**, then record a checkpoint. Investment must remain C3/L1/N4, with conversion bounded by this body's actual red skulls and native decay. Save, menu/load, open A and repeat rating/checkpoint. Body panel must stay raw; central diagram reflects grave improvements.
4. **Очистить вложения и завершить тест**, then save normally. Native presentation returns. Cleanup does not undo the real body/decoration movements or native costs.

No need to repeat appearance fitting or earlier full-conversion/toggle tests. The user checks visuals; the agent collects accessible logs. Do not force the old healthy-body rating onto a different replacement. If the probe disables or recovery is missing, preserve the log before further loading and report the point reached.

## Independent checks and limits

build.ps1 requires exact clean source for immutable numbered artifacts, SDK6.0.406 Roslyn and verified installed references; -CheckOnly is development-only. FaithTests retains exact native target/IL shape/label/card/floor/min checks,160 native skull-oracle cases and hidden-red-bar assertions. LifecycleTests executes actual native get_quality IL on synthetic native objects:2700 combinations compared with independently transformed native fixtures. It exercises the shared saved-state backend, actual SmartSerializer map roundtrips, empty/replaced graves, raw-body non-export, native zero-param pruning, cleanup and invalid-state rejection.

Headless execution substitutes explicit test authorization for live selection/coverage. Synthetic inventory relocation is not native exhumation/SetObject, Unity scene restoration or live GUI acceptance. These are the purpose of the live test above. Payment, radius/tier, zone/hint notification, movement compatibility and production migration remain open.

Sparse BPS_FAITH records add RECOVER and CHECKPOINT to existing rating/observation records. Raw body and each part report their own probe fields (expected zero). Per-action physical hashes exclude ONLY the three owned parameters on the root grave; recursive anatomy/items and money/faith remain included. They compare immediately around diagnostic actions, not across ordinary gameplay. Native serialization tests compare original body bytes independently. A failed assertion disables effects and requests one vanilla UI rollback; saved investment is not automatically destroyed.
