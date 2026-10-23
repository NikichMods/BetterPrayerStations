# Better Prayer Stations — Faith Probe0.4.0

Research-only paid-action test, one grave under an existing completed prayer station. This build REALLY consumes player-carried faith. Test prices1,2,3… are not final balance. No placement, radius-overlay, world-badge or product-UI work is included. Version owner Plugin.cs Version; exact source/artifact/install identity: docs/TEST_BUILD_LOG.md. Older numbered artifacts remain immutable.

## Короткий игровой сценарий

Нужна занятая могила с минимум двумя красными черепками и готовая молельня в трёх тестовых клетках. Можно использовать могилу1060 из прошлого теста, если её вложения очищены. Для трёх покупок достаточно6 веры в инвентаре героя/его сумках; сундуки не учитываются. Не нужно переносить молельню или снова менять тело.

1. Откройте могилу и нажмите «Выбрать открытую могилу». Для чистого теста должно быть0 превращений/0 венков, общий счётчик0, следующая цена1. Если остались прежние бесплатные тестовые вложения, очистите их, снова выберите могилу; очистка не возвращает веру.
2. Один раз купите превращение за1 веру. Дождитесь проверки: C1/L0/N1, следующая цена2. Затем один раз купите венок за2 веры: C1/L1/N2, следующая цена3. Вера уменьшилась суммарно на3. Венок может не повысить рейтинг из-за ограничения белыми черепками — это нормально.
3. Нажмите «Проверить нехватку: цена = весь запас + 1». Пробник подставит явно тестовую цену больше реального количества веры и вызовет ту же ветку покупки. Должен быть отказ insufficient; вера, C/L/N и обычная следующая цена3 не меняются. Перекладывать или искусственно обнулять веру не нужно.
4. Сохраните игру обычным способом, выйдите в меню и загрузитесь. Откройте ту же могилу, нажмите «Повторно проверить рейтинг и шкалу» и «Записать этап…». Должны восстановиться C1/L1/N2, следующая цена3 и остаток веры после двух покупок. Для чистого сравнения не выполняйте между замерами другие траты/получение веры.
5. Купите второе превращение за3 веры. Ожидаются C2/L1/N3, следующая цена4; суммарная трата за этот сценарий6 веры. Затем «Очистить вложения и завершить тест» и сохраните игру. Очистка удаляет диагностические улучшения, но НЕ возвращает6 потраченной веры.

Счётчик, запас, цену и результат показывает диагностическая панель. Логи агент собирает сам; вручную считать или копировать журнал не нужно. Панель F8/перетаскивание/прокрутка сохранены. Служебные строки покрытия из0.3.0 ещё не переделаны — визуальные исправления отложены. Не нужно повторять весь сценарий движения/сноса.

## Transaction and evidence contract

FaithPurchase.Count uses actual player Item.GetTotalCount("faith",true), matching native root/bag removal scope. Secondary inventories and nearby/world-zone/church storage are not spend sources. Native debit is Item.RemoveItem("faith",cost,null); both its boolean and exact counted delta must pass. Reentry guard, one pending console action and quoted shared count/cost validation prevent a pending stale action from charging at a different price. Consecutive intentional clicks are separate actions and each advances price; no blanket double-click refund is promised.

Before mutation, preserve root/bag list contents, reference/order/quantities and the three owned grave values. On debit/commit exception or wrong delta, restore those bounded inputs and verify them, then fail closed. This is synchronous main-thread in-memory containment, not disk crash atomicity or a rollback of arbitrary third-party side effects. Backend test-only delegate/checkpoint injection is not exposed as a live fault button and does not replace the installed native debit.

Once native debit and saved-state commit have succeeded, subsequent presentation refresh is a separate phase: a later GUI failure disables effects but does not undo the valid paid investment or retry charging. Existing per-GUI-action hash is measured AFTER the payment; separate PURCHASE record checks unchanged recursive physical grave data/money and logs faith/action deltas. Only the three diagnostic root keys are excluded from physical grave hashes. Rollback restores owned values semantically; it may leave inert zero owned keys where they were previously absent. No anatomy/decor/money mutation or item creation is intended.

Purchases require current save/player ownership, open selected idle occupied grave, current station coverage and no native build/floating-preview mode. This deliberately tests under an existing completed station; adopted construction-preview feedback remains intact, but purchase-from-preview is refused. Conversion when no further actual red can be converted is rejected without charge in this diagnostic. Wooden purchase caps, final no-gain eligibility, storage scope beyond this test and production schema/prices remain unselected. Shared N is C+L, not a separately decremented balance or per-action price series; legacy diagnostic counts, if retained, remain counts and influence the quote.

Sparse BPS_FAITH PURCHASE fields: action/result/quote_kind/cost/faith_before/after/actions_before/after/C/L/physical/money checks. RECOVER and CHECKPOINT log actual player/bag faith plus next price. PURCHASE_ABORT records cost/current faith/actions and rollback outcome before circuit breaker/one vanilla GUI fallback. Refusal has no debit; insufficient test quote is exactly actual available faith+1 and never a fake inventory balance.

## Checks and limits

Build requires exact clean source and canonical version with verified local SDK6.0.406 Roslyn/host refs. Preserved native-IL projection,160-case skull oracle,2700 getter cases, binary lifecycle and coverage-geometry gates run. Compiled payment adapter is checked to call native RemoveItem/GetTotalCount. PurchaseTests executes the real transaction backend with deliberately simulated debit, actual native counting and SmartSerializer: shared sequence, root/bag/exact/zero funds, stale/wrong quote, actual shortage, reentry, limit, seven injected rollback classes and synthetic payer/grave serialization.

These tests do NOT execute actual RemoveItem, real GameSave restoration, Unity UI or foreign mod debit hooks. Live scenario above closes those normal installed paths; no stable production acceptance follows. Coverage preview/construction classification/world badge refresh remain as documented in COVERAGE_RESEARCH.md; no unrelated correction is bundled into this candidate.
