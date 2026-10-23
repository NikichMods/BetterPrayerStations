# Better Prayer Stations — Faith Probe 0.3.0

Research-only coverage test, one saved-investment grave. No faith payment, purchase caps or production release. Plugin.cs Version owns numbering. Installed BetterPrayerStationsFaithProbe.dll; exact clean source/artifact/install identity belongs to docs/TEST_BUILD_LOG.md. Previous0.2.0 remains frozen with its accepted persistence return.

## Observable change

Existing grave-owned C/L/schema storage and native rating/diagram input projection are retained. Manual activation is replaced by actual registered prayer-station positions. Any covering station activates all stored effects; highest tier is reported, never stacked. Outside coverage, native rating returns while C/L/N remain saved. Appearances are outside this separate plugin and are not switched off by coverage.

Diagnostic geometry: inclusive XY circle, radius3 native grid cells (288 world units), measured between native WGO origins. WorldGameObject.grid_pos divides pos by96 in the verified host. This radius/anchor/metric is a test convention, not adopted product balance or a promise about visual sprite bounds. Wooden pray_stand_wd and stone pray_stand_stn count; unknown/placeholder/removed/destroyed objects and stations currently is_crafting do not. In particular, marking a station for removal can disable it before demolition finishes. Eligibility during construction/removal is provisional research policy.

One existing WorldMap.objs traversal after game-started restoration also discovers stations. A declared WorldMap.OnAddNewWGO postfix registers new/transformed stations only if the actual world list contains the instance, excluding duplicate-ID preview instances rejected by native registration. Current cached station refs/coordinates are read directly; removed refs are pruned. No recurring full-world scan, Resources enumeration or new transform/zone writes. Getter/UI inputs consult current coverage; a0.25-second main-thread check updates the panel and logs only coverage-tier/count transitions. Actual transform coordinates are logged separately from native cached position/distance for audit. The probe does not invoke Move Objects or replace its algorithm.

The installed Move Objects0.6.0 exact c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0 selected path moves the existing transform then synchronizes grid/zone state. Compatibility with actual station selection/preview/cancel/commit still needs this live test.

## Короткий игровой сценарий

Панель слева можно перетащить/прокрутить; F8 скрывает её. Никакие монеты или вера пробником не списываются. Перенос, строительство и снос выполняются обычными средствами игры/Move Objects и имеют обычные последствия.

1. Откройте занятую могилу с красными черепками, выберите её кнопкой. Добавьте три превращения и один венок (если сохранились прежние, не добавляйте повторно). Это бесплатная тестовая настройка, а не проверка покупки. При необходимости переставьте существующую молельню ближе. В панели должно быть «покрывают:1» или больше; уровень1 — дерево,2 — камень. Нажмите «Повторно проверить рейтинг и шкалу».
2. Перенесите молельню дальше трёх клеток. Если поблизости есть другие молельни, они тоже должны оказаться вне радиуса, иначе эффекты правильно продолжат работать. После подтверждения переноса подождите секунду, откройте могилу и нажмите проверку рейтинга. Рейтинг/шкала должны стать обычными, но C3/L1 останутся. Ближайшее расстояние и число покрывающих объектов видны в панели.
3. Верните молельню внутрь радиуса, снова откройте могилу и проверьте рейтинг. Все сохранённые эффекты должны вернуться без повторных улучшений. По возможности сравните позицию немного внутри/снаружи границы; точную дробную координату выставлять не нужно.
4. Снесите эту молельню обычным способом. Если это была последняя покрывающая, эффекты отключаются, а вложения остаются. Откройте могилу и проверьте рейтинг. Затем поставьте/перенесите другую молельню внутрь круга: эффекты должны восстановиться. Можно использовать деревянную даже при трёх сохранённых превращениях — ограничения покупок здесь не проверяются.
5. Сохраните игру и загрузитесь из меню с восстановленным покрытием. Откройте ту же могилу, проверьте рейтинг и нажмите «Записать этап…». Затем «Очистить вложения и завершить тест» и сохраните игру.

Если рядом уже стоят деревянная и каменная: необязательная короткая проверка перекрытия — убрать только каменную. При оставшейся деревянной уровень должен снизиться2->1, но все эффекты сохраняются. Специально строить две для этого необязательно.

Не нужны повторные эксгумации или косметические проверки. Для другой анатомии/разложения ожидаемое число рейтинга может отличаться; сравниваем покрытие/его отсутствие на одном и том же теле. Пользователь оценивает визуальный результат; агент самостоятельно читает журнал. После каждого переноса/сноса открывайте исходную могилу и нажимайте проверку, чтобы получить автоматическое OBSERVE, а не только визуальный отчёт.

## Checks, logging and boundaries

Build gate retains native IL shape/exact signatures/card/native cap/label tests,160 compiled/native skull cases,2700 native getter cases and actual SmartSerializer synthetic lifecycle. New compiled geometry checks include exact radius/diagonal/outside, wood/stone IDs, removed/busy/placeholder exclusion and out/back. These prove arithmetic, not Unity registration/movement/destruction.

BPS_FAITH COVERAGE records selected grave ID, tier, covering count, nearest registered station/distance, actual transform XY positions and C/L/N. CHECKPOINT role=coverage-transition records actual getter baseline/result and raw anatomy. Explicit rating button still asserts actual native label/diagram, all four input seams and immediate before/after physical hash. Only the three owned root-grave parameters are excluded; recursive anatomy/physical items/currency remain included. The hash does not compare native construction/removal costs across gameplay. A missed/skipped OBSERVE is not a GUI pass. A mismatch disables effects with one native UI rollback attempt; cleanup remains available.

Limits: one selected grave; no production purchase constraints or faith debit, free body-inspection UI, cosmetic integration, radius balancing, zone/hint notification timing, arbitrary clone/delete/recreate mods, background registry-recovery scan, full restart/multi-save compatibility or final UI. Coverage for ordinary gameplay reads is current, but the probe does not force native zone/quest notifications. A displayed cemetery total is not the primary assertion. Native movement/registration/demolition/load and overlay usability remain unaccepted until this exact artifact returns.
