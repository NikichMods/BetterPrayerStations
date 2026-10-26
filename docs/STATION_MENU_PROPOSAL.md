# Station menu — accepted presentation and native preview

Updated2026-10-26. Native-list structure, the following wording and exact icon trio are accepted. Current native preview is Menu0.5.0; visual return failed with rows shifted left/down in station and several ordinary construction menus. PROJECT_STATE/TEST_BUILD_LOG own the investigation; selected copy/artwork is unchanged. Historical drafts/reference searches are [archived](history/2026-10-26/STATION_MENU_PROPOSAL.md).

## Accepted text

| Element | Text |
| --- | --- |
| Title | Благословения |
| Permanent purchased count | Даровано благословений: P |
| Current station ceiling | Молельни позволяют использовать до C благословений одновременно. |
| Secondary counts | Действуют: A · Доступно: F; add Угасло: D when needed |
| Prayer action | Помолиться |
| Prayer description | Дарует ещё одно благословение. |
| Place action | Благословить место |
| Place description | На могиле превращает все красные черепа в белые. |
| Remove action | Снять благословение |
| Remove description | Снятое благословение можно использовать в другом месте. |

No permanent “blessings and price are shared” footer. Keep P and C separate, never a P/C fraction. Use short contextual refusal: insufficient faith states missing amount; insufficient station ceiling says «Постройте или улучшите молельню.» The probe's ten-price bound is still explained as a test limit, not final product behavior. Place rule remains visible throughout world placement; no grave-only restriction is introduced.

Editorial basis from DetailedTechnologyTooltips: communicate the main player capability/result with natural concise sentences, not telegraphic fragments; avoid duplicated context and internal bookkeeping language; vary phrasing by meaning rather than rotating templates; emphasize useful distinctions sparingly. This is wording reuse, not a new mechanics owner.

## Accepted icon files

Current exact sources are under ignored `artifacts/IconConcepts/`. Keep their bytes; do not regenerate them from descriptive prose. Local handoff bundle includes these inputs.

| Action | File | Raster | SHA256 |
| --- | --- | --- | --- |
| Помолиться | prayer-open-rosary-large-cross-nearest100-v1.png |100×100|0f9324e6b6c40607856e922b750963675723f0551ad9337a2706f37a106e06c7|
| Благословить место | bless-place-nearest37-v1.png |37×37|cdb90b9719f3efce7656b1b8403c4033eb407cf76dafa509b7a23919ff0adfbb|
| Снять благословение | remove-blessing-nearest37-v1.png |37×37|38c0dd4ec6a76170460e48227a60bb6a39631f6fd7db43b4276ba6401b46be4d|

Prayer: generated open book/rosary variant with enlarged gold cross, nearest-neighbor100px reduction. Graves: user-selected generated/edited base, directly nearest-reduced to37px, lit/unlit pair. These are concept-to-sprite results, not literal copied native sprites. Native book i_book_gold_1 was a palette/shape reference. Raw extracted game/reference files remain local and outside the software license; final production asset packaging/provenance still needs review.

User accepts slightly higher detail of the100px book in the mockup. Earlier hands, circle, arrow overlays, other books and manual variants are not current choices. Earlier symmetric-cross cleanup was discussed; the final displayed trio was accepted as shown, no additional retouch is inferred. Native source rasters vary17×20/32×32/37×37;37 is not a universal game requirement.

## Current native implementation

`research/MenuProbe/Plugin.cs`0.5.0 adds these texts to scoped GJL action keys and the native header. `StationMenuView.cs` decodes three pinned embedded PNGs once into owned Texture2D/Sprite objects. All three use37×37 native display bounds and Point filtering; source rasters remain100/37/37. Restore pooled row presentation on hide/reset, release owned textures/sprites on unload. No runtime asset-directory scan or external image file is required by the installed DLL.

Price uses the native faith ingredient icon and available/required counter, e.g.23/12. Native generic drawing can include interaction chests; the owned prayer counter/colors are corrected after drawing to CapacityPurchase.Count(player), matching actual carried inventory+bags. Custom CraftInterface retains the transaction; the native custom click branch skips ordinary crafting, preventing an extra material debit. No separate custom “Нужно/При себе” block was selected.

The browser mockup is illustrative, not a reproduction of actual NGUI font/frame/scale. Its40/56/72px controls are review settings, not final native size requirements. Local preview location is in `.local/WORKSPACE.md`.

## Readiness recorded before0.5.0 source mutation

Question/method: extend exact accepted0.4.0 probe, not a new framework. Existing project evidence establishes state/dispatch; pinned GK1.407 native IL establishes the following last writers. Only real native fitting requires a new live candidate.

- **Copy/header READY:** observable adopted names/counts; owner UniversalObjectInfoGUI/GJL action IDs, final RefreshMenu/native Redraw. Exact current menu/controller/card references bound the change; preserve payment/state/editor. Acceptance: opening, refresh, refusal and ordinary desk after close.
- **Icons READY:** native Redraw calls DrawItem/DrawIcon/MakePixelPerfect; UI2DSprite and empty_item_gfx are independent consumers. Assign owned sprites after Redraw; restore pooled state on hide. Only the three owned cards, no shared prefab/global icon registry change. Acceptance: selected bytes/dimensions plus actual native repeated redraw/readability/unrelated desk.
- **Geometry READY:** native Redraw IL0559–0638 reads description.height-20 after anchor/text update, then mutates parent bottomAnchor and hgt_widget. Prefix resets captured own-row geometry/description sizing; native final collider/anchors and outer UITable consume it. No fixed absolute card offsets. Acceptance: repeated purchases/reopen show stable rows without overlap. Static evidence alone does not prove visual correction.
- **Faith display READY:** one transient faith need for native rendering, refreshed before Redraw. Scoped final counter uses actual payer funds; custom action is still the only debit. Place/remove have no needs. Acceptance: actual price/remaining count/shortage and exact transaction; earlier payment/save tests retained.

Compiler/headless checks, installed identity and the failed visual return are in TEST_BUILD_LOG. Diagnose shared menu displacement before another candidate. The preparation references above to pooled rows describe the original assumption: returned host inspection establishes fresh row creation/destruction on this CraftGUI path, so that assumption does not explain cross-menu displacement. World heavenly-light rendering is separately deferred.
