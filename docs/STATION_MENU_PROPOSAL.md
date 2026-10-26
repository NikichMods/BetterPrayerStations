# Station menu presentation —2026-10-26

Status: **Родной список layout and lore copy adopted; icon choices proposed**. User approved the native list, then explicitly asked to take the prayer/blessing wording and update its preview. Beam art remains deferred. Native rendering/runtime implementation has not been accepted or changed.

## Adopted presentation

Header: «Благословения». Ledger: «Даровано благословений: P» and «Молельни: до C благословений». Secondary line: «Действуют: A · Доступно: F», adding «Угасло: D» when positioned fields lack support. Do not combine P/C into a fraction: P>C is valid. Price and carried faith belong to the prayer row. Footer: «Благословения и цена молитвы общие для кладбища»; under lost support, explain that the last placed blessing fades first.

Three actions and descriptions:

- «Помолиться» — «Дарует ещё одно благословение.»
- «Благословить место» — «На могиле превращает все красные черепа в белые.»
- «Снять благословение» — «Дарованное благословение сохраняется.»

«Место» preserves adopted free ground placement; wording does not introduce grave-only targeting, a new grace resource or upkeep. Insufficient faith retains price and gives the missing amount; insufficient station ceiling says to build/upgrade a prayer station. Removing dormant fields remains available. The conversion rule must also remain visible throughout actual world placement.

## Native icon inspection and proposal

Verified installed GK1.407 resources.assets SHA256215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca. Inspect existing EasySpriteCollection catalog/NGUI atlas before inventing new artwork. Selected original Sprite images were extracted only into ignored local evidence for visual inspection and private preview; no proprietary bitmap/atlas is committed.

| Proposed use | Actual sprite ID | Native dimensions | Observed appearance |
| --- | --- | --- | --- |
| Prayer | icon_pray_bubble |17×20| framed praying hands |
| Bless place | icon_faith |12×12| luminous golden faith orb |
| Remove blessing | icon_stop |22×22| simple red prohibition/cancel mark |

These three original images are embedded in the updated private native-list preview, with pixelated scaling. No suitable disappearing-light symbol was found in the inspected candidate set; this is not proof of absence from every game asset. i_b_remove exists32×32 but visually depicts explosive demolition; prefer icon_stop to avoid implying grave destruction. i_b_holy_cross/02/03 are construction blueprints for crosses, not blessing glyphs. icon_faith_ol is another available faith-orb variant. The faith ItemDefinition has icon="" in the inspected data: do not assume that raw field is a usable sprite name in future menu code; selected explicit IDs are evidenced. Exact native icon consumption/rendering still needs the narrow implementation check.

## Preview and next boundary

Only the adopted native-list layout is shown now. It retains ordinary, insufficient-faith, insufficient-support, all-places-occupied and initial-state controls plus ordinary/compact density. Local prayer simulation updates illustrative faith/P; bless/remove show only the cemetery-grid transition boundary. Preview QA passes initial paint, local paid update, shortage, support/dormant removal, transition/back, all three image decodes and no desktop/mobile horizontal overflow. This proves the preview only. Frame/font remain an approximation; actual implementation should reuse native frame/font/faith imagery and address the known repeated-redraw row-height defect under its own readiness/visual acceptance.

Pending feedback: accept or replace the three native icon candidates and refine density if needed. No runtime/DLL/version change, no beam-art work or stable promotion.
