# Better Prayer Stations — Faith Probe 0.1.1

Research-only, one occupied grave, **no faith payment and no prayer-station radius**. This is a separate diagnostic plugin, not a production release or an extension of the immutable Appearance Probe artifact. Canonical version: Plugin.cs Version. Installed `BetterPrayerStationsFaithProbe.dll`; handoff `BetterPrayerStationsFaithProbe-<version>.dll`. Exact source/hash/install state belongs to docs/TEST_BUILD_LOG.md. Do not run the old appearance console alongside it in this test; preserve its payload outside plugins before installing this probe while the game is closed.

## What the test actually changes

Only in-memory counts of conversion capacity, extra wreaths and an explicit active/dormant toggle. There are **no persistent writes**, no body/organ/item edits, no spending, no synthetic items and no station/coverage simulation presented as real geometry. Save/load invalidates all transient references/counts; the save retains ordinary physical data, not diagnostic improvement state. This session-only storage does not claim to test the future product's saved investments.

The quality transpiler projects only one native skull input and the grave branch's inventory-quality input. It leaves native no-body/generic/craft exclusion/floor/min paths intact; effect activation is deliberately limited to the selected, idle grave with the same body reference. The UI transpiler projects only the central diagram's skull input and completed two-card decoration sum. Shared Item/body-panel/item-quality methods are not patched. No real world-object parts are redrawn. Raw body panel and physical item cards remain native.

Converted whites obey native decay. Effective conversions cannot exceed actual red skulls, and unused capacity creates no extra whites. Extra wreaths add decoration capacity; the native white limit may make their immediate rating delta zero. The diagnostic count limit12 per action type bounds GUI object creation and is not product balance.

## Deterministic gameplay test

Panel is left, draggable/scrollable; F8 hides/shows without changing effects. Open one normal occupied buried grave, idle, with completed decorations. Prefer the earlier grave with raw3 red/3 white and decoration6, in healthy condition, for the concrete expected numbers below. Do not exhume, repair, remove decorations or change bodies during this narrow test.

1. **Выбрать открытую могилу**. Wait for the status check. At zero improvements, total and central diagram must match vanilla; panel of the body shows actual anatomy.
2. **Добавить один венок**. Central wreath column gains one. On the3red/3white/decoration6 example rating remains3: the white limit still caps it. Item-card quality must not change.
3. **Обнулить улучшения**. Then **Добавить одно превращение**. Example central result2 red/4 available white, rating4. Raw body panel remains3 red/3 white.
4. Add two more conversions, waiting for each action to finish. Healthy example becomes0 red/6 available white, rating6. Add a fourth conversion: no extra white is created, rating stays6 (stored diagnostic capacity4, effective3).
5. Add a wreath again with conversions still present; examine the extra central wreath and preserved limits. **Отключить эффекты**: original total/central diagram returns, counts are retained. **Включить эффекты**: the same improvements return. Actual body/organ/item data and currency remain unchanged.
6. Optionally close/reopen the same grave or use F8 to view the cemetery; then open the grave and **Повторно проверить рейтинг и шкалу**. Finish with **Очистить и завершить тест**. Native presentation returns. No save is required to clean these session-only settings.

A different body/decoration/decay state can produce different numbers; use the actual reported effective state, not force this example onto it. User provides visual outcome; accessible logs are collected by the agent. No manual calculations, log copying or DLL handling are required.

## Build, checks and evidence limits

`build.ps1 -GameRoot <local game root> -DotnetRoot <local SDK root> -ExpectedSource <exact SHA>` uses verified SDK6.0.406 Roslyn with installed GK1.407/BepInEx/Harmony/Unity references, warnings as errors, version checks and a clean source requirement for immutable numbered artifacts. `-CheckOnly` is development-only. No proprietary references are shipped. Build manifest records reference/source/artifact hashes.

FaithTests reads the actual installed native methods through Harmony's instruction reader, applies both real rewrite functions, checks preservation of the generic inventory branch/native floor/min/real item-card calls, rejects changed target shapes and validates incoming label transfer. The compiled conversion helper is compared with independent actual Item.GetBodySkulls reference fixtures in160 cases; unbound wrapper calls return native stats. These checks cannot prove live Harmony startup or instantiated UI.

Sparse `BPS_FAITH` log records READY/SELECT/REDRAW_BEGIN/REDRAW_END/OBSERVE/CLEANUP_DONE/RESET/FAILED/ROLLBACK. Before/after physical hashes cover the selected grave's recursive item IDs/counts, complete GameRes name/value lists and scalar fields, plus player money/faith; definition/reference caches and unrelated save objects are not a bytewise save-equivalence claim. Raw body/part skulls and durability are logged directly. Observation runs one Update after the real GUI redraw so native child destruction can settle; it checks total label, active central slot/wreath count, red bar visibility and visible-bar height, raw body panel and that all four projected seams were reached. Check success/visual outcome only after this exact candidate is exercised.

Every change runs outside OnGUI. Only one selected-grave GUI redraw per action; no world scans, recurring logs or global body patches. Unselected graves pass through before save-ownership/reflection checks. No zone-total getter or quest-mirror updates are invoked by this slice because those carry native notification side effects and are outside the requested rating/scale test.

Init checks exact host SHA/private fields and exact declared signatures; partial init failure unpatches only this plugin. Runtime validation failure disables effects, queues one vanilla GUI rollback and stops retrying if rollback fails; cleanup remains available. The last effective GUI presentation cannot be considered accepted if OBSERVE reports mismatch/skipped or a failure marker. A closed grave needs reopening for native visual observation; cleanup still clears transient state when its window is closed.

Limits: no payment, radius, tier, saved investments, exhumation/replacement/export, changed native crafts, zone/hint refresh, other-mod compatibility, full catalog or final UI acceptance. Native diagram behavior at extreme counts is outside the bounded test. Existing appearance results remain in their own frozen candidate/source records.

## Short corrective retest after 0.1.0

0.1.0 reached effective getter/label rating6 (native baseline3) with raw body unchanged, all6 actual slots and6 wreaths, then its combined red-bar check failed at zero red and triggered vanilla rollback. That log did not record actual bar height/visibility separately, so the exact failed conjunct remains unobserved. Native GraveGUI hides the bar at zero red; NGUI height setters can coerce requested dimensions. The corrected observer requires hidden state at zero red and does not assert invisible geometry. Positive-red height/state, all counts, raw anatomy and total checks remain mandatory. New fields slots_ok/wreaths_ok/red_bar_ok/red_bar_height/red_bar_min/red_bar_visible distinguish the actual failure if it persists. No input projection/effect formula/physical data/activation behavior changed.

No need to repeat earlier successful wreath-only/reset/one-conversion activation checks. On the same healthy3red/3white/decoration6 grave: select; click Add conversion **three times, one at a time**, waiting for each observation; expect4,5,6. Add a fourth: expect6 with no extra white. Add one wreath, disable/re-enable effects (3 then6); cleanup. This is a test of corrected observation and the previously unreached excess-conversion boundary, not a request to reproduce the same0.1.0 failure.
