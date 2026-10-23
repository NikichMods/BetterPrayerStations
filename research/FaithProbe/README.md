# Better Prayer Stations — Faith Probe0.5.0

Research-only two-grave integration test. Real faith is spent, test prices N+1 and radius3 cells are unchanged diagnostic values. No product visual/placement/overlay/balance work. Version owner Plugin.cs Version; exact frozen source/artifact/install: docs/TEST_BUILD_LOG.md. Existing0.4.0 normal native payment acceptance is reused.

## Что меняется

Можно отметить две могилы одной нативной зоны. Их вложения работают одновременно, даже когда окна закрыты и выбрана другая могила. Выбор в панели определяет только адрес покупки. У каждой могилы свои C/L/N и следующая цена. Панель показывает обе по постоянным номерам; порядок номеров стабилен, названия А/Б ниже — просто удобные обозначения выбранных вами объектов.

Проверка общего качества вызывает настоящий WorldZone.GetTotalQuality один раз при закрытых игровых окнах. Чтобы не посылать игре искусственное «качество без улучшений», контрольный фон считается отдельными нативными оценками объектов; GetTotalQuality никогда не вызывается с временно отключёнными улучшениями. Журнал проверяет реальную сумму, вклад обеих могил, нативное событие с этой суммой, сохранность тела/украшений/денег/веры. Это точечный замер по кнопке, не постоянный пересчёт зоны.

## Короткий сценарий — всего4 веры

Нужны две здоровые занятые могилы с хотя бы одним красным черепком каждая, на одной стороне кладбища/в одной нативной зоне, обе под молельней. Можно использовать1060 и другую рядом. Данные прошлого пробника должны быть очищены; если остались, кнопка завершения очищает ОБЕ отмеченные могилы без возврата потраченной веры, после чего можно отметить их снова.

1. По очереди откройте А и Б, каждый раз нажимая «Отметить / выбрать открытую могилу». Теперь обе строки должны быть C0/L0/N0, цена1. Чтобы купить на конкретной могиле, откройте её и снова нажмите выбор — без очистки другой.
2. На А купите превращение за1 и венок за2: А=C1/L1/N2, следующая цена3. На Б купите превращение за её собственную цену1: Б=C1/L0/N1, следующая цена2. Общая трата4 веры. После каждой покупки дождитесь завершения проверки; строки другой могилы должны оставаться прежними.
3. Нажмите «Проверить зону после закрытия игровых окон», затем закройте окно могилы обычным способом. Когда останется игровой мир с диагностической панелью, проверка выполнится один раз. Статус подтвердит сумму и добавку; в журнале ZONE closed=True/both_inputs=True. Обе могилы продолжают работать, хотя ни одна не открыта.
4. Перенесите молельню так, чтобы А оказалась вне всех радиусов, а Б осталась под покрытием (можно наоборот). Завершите режим переноса/строительства. В строках панели одна могила станет неактивной, другая останется активной; C/L/N и цены не меняются. Снова нажмите проверку зоны, оставив игровые окна закрытыми. Если рядом есть другие молельни, они могут законно сохранять покрытие.
5. Верните покрытие обеим и повторите закрытую проверку зоны. Затем сохраните игру, выйдите в меню и загрузитесь. Пробник восстановит обе могилы и автоматически запросит один закрытый замер, как только игровые окна/загрузка завершатся. Проверьте строки А=C1/L1/N2/цена3 и Б=C1/L0/N1/цена2. По очереди откройте/выберите обе и нажмите «Повторно проверить рейтинг и шкалу», затем при желании повторите закрытый замер кнопкой.
6. «Очистить ОБЕ могилы и завершить тест», затем сохраните игру. Очистка убирает диагностические улучшения обеих могил, не возвращая4 веры и не отменяя обычный перенос молельни.

Не нужно считать общую сумму вручную или переписывать лог. Обычные значки над могилами могут сохранять прежнюю задержку обновления: их визуальное исправление сюда не входит. Первичные критерии — настоящая сумма зоны в журнале и нативное окно могилы по кнопке проверки. F8/перетаскивание/прокрутка панели сохранены.

## Implementation and independent observation

TwoGraves stores at most two actual WGO references, validates duplicate identity and orders by unique_id. Existing namespaced schema/C/L stay on each grave; no external save ledger or coordinate ownership. The existing post-start pass restores both marks (including zero-count marked graves) and station refs. Third marked grave/unknown data disables rather than silently discards state. Cached scene/player ownership guards and cleanup/save rules remain.

Native getter/UI projection uses the actual grave argument and its current body/state/coverage. Editing target is never a quality authorization gate. Each PURCHASE additionally compares other grave state/physical snapshot; other_unchanged must be true. Payment backend from0.4.0 stays unchanged. Coverage polls only registered station refs for two grave positions; logs per-owner transitions. Cached text is refreshed on changes, not by writing/reading host state during OnGUI.

Closed check requires two idle occupied marked members in the same native Sum zone and no massive native window/floating construction preview. Native cached WGO._zone is type-checked; actual members are read through verified GetZoneWGOs (returns native list). Individual native vanilla leaf sum plus the two actual effective-minus-native deltas is compared with the actual zone result. No replacement zone ledger/result or manually raised event. A temporary OnQualityComputed listener observes only the one actual call and is detached in finally. Both grave input seams must be hit inside that call. No recurring full-world/zone scan: the normal post-load discovery plus explicit checkpoint calculation only. The real zone callback may update native quality mirrors normally; diagnostic false-baseline callbacks are avoided.

ZONE logs closed/zone/baseline_leaf_sum/grave_delta/expected/actual/native_events/event_value/both_inputs/total_ok/event_ok/physical_money_faith_unchanged. ZONE_GRAVE and CHECKPOINT role=zone-closed log each grave's identity, selected state, current native/effective value, C/L/N/quote/faith/raw anatomy. RECOVER followed by both CHECKPOINT owners proves restoration. A skipped check is not a pass. Failure disables effects; native world-badge/zone notification fallback beyond the recorded real call is not claimed.

## Local gates and limits

All prior compiled native projection/skull/diagram/payment/serializer gates remain. New checks exercise actual two-owner set (reference membership, duplicate/cap rejection), simultaneous projected native getter IL, selective dormant state and both-owner native serialization including mixed C/L; independent price sequence A1/A2/B1 and restored counters; actual native WorldZone Sum/ignored member/native callback on synthetic objects. Native zone fixture uses unpatched raw leaf getters; synthetic projection uses explicit headless authorization. Thus live combined Harmony+Unity+closed-window zone integration remains to be accepted on this artifact. Purchase rollback tests still use simulated debit; prior installed native normal debit is accepted separately.

No final wooden/stone purchase cap, full preview/construction policy, production migration, arbitrary grave moves/deletion, all-save/restart compatibility, bag-only native debit, appearance integration or new visual behavior is claimed. This is a bounded two-grave test, not a production implementation.
