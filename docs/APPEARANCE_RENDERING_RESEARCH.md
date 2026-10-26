> Historical combined-concept research. Its evidence remains reusable, but its services and old “current/next” statements are not the current product or work queue. See [PROJECT_STATE](PROJECT_STATE.md) and [history map](history/README.md).

# Grave-owned appearance rendering — 2026-10-23

Status: **bounded static source/IL and asset metadata plus passing bounded installed 0.1.1/0.1.2 scenarios**. One grave passed gravestone projection/close-cancel; two graves subsequently passed independent slots, removal/reinstallation, distinguishable gravestone exchange, menu-load and cleanup with preserved measured cosmetic-redraw state. No production patch or full appearance acceptance exists. Product ownership is accepted in `DESIGN.md`; native persistence carrier/fixture are documented in `PERSISTENCE_RESEARCH.md`.

## Question and hypothesis

Can a grave-specific appearance replace only the rendered decoration choice, preserving installed items, native quality, damage stages and cancellation behavior?

Hypothesis supported by the inspected code: project the native grave visual descriptors after they are built; let the native drawer instantiate and maintain the world parts. A temporary fitting choice can feed the same projection without writing persistent grave state. The research probe now verifies this mechanism for one gravestone and native grave-window closing; the full product and other lifecycle paths remain unverified.

## Evidence identity

Same 1.407 assembly/resource hashes as `PERSISTENCE_RESEARCH.md`, rechecked. Pinned supporting source `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`: `CustomDrawers`, `WOPPrefabName`, `WorldGameObject`, `WorldObjectPart`, `SmartDrawer`, `GraveGUI`. Installed IL corroborates GetNewWOPPrefabsNames/NeedRedrawGrave/OnDrawGrave and its native drawer calls. Asset inspection reads named GameObject/component metadata only; no game artwork is copied here.

## Native owner and final visual consumer

`WorldGameObject.Redraw -> CustomDrawers.OnObjectRedraw -> OnDrawGrave` owns the selected grave visual route. OnDrawGrave handles `grave_ground` only. It gets a list from private `GetNewWOPPrefabsNames`, compares/caches it, removes old additional WOPs, and resolves each descriptor through `WorldGameObject.RedrawPart` under `objects/grave parts/`.

The builder uses actual grave inventory and native ItemType (GraveStone=101, GraveFence=102, GraveCover=103). An incomplete installation/removal state has a native construction placeholder based on the actual item-ID parameter. A completed part gets `part_1 = actual_item_id + "_stg_"`, damage stage from actual item durability, and the grave vegetation flag. Do not infer slot from `grave_top`/`grave_bot`: e.g. the sarcophagus definition is a fence.

`WOPPrefabName.EqualsTo` compares part_1, part_2, stage and need_herb; a changed projected name can participate in native redraw detection. Its GetName composes the prefab name. If a requested damage-stage prefab fails to load, OnDrawGrave tries descending positive stages. It then optionally tries the corresponding `_herb` part. RedrawPart loads/instantiates native WorldObjectPart and uses the prefab's local offset; it is more than assigning a SpriteRenderer sprite.

The quality getter reads real inventory quality and body skulls, not WOPPrefabName. A descriptor-only change therefore has a static separation from numerical quality; live unchanged-score behavior is confirmed for the narrow 0.1.1 case, not the full catalog/lifecycle. Do not temporarily replace Item.id, definitions, durability or shared balance data to obtain a visual.

The native builder also removes inventory items whose definitions are null. It is not a pure read helper to invoke repeatedly from a picker. Let native drawing invoke it; do not manufacture preview by repeatedly calling it as a catalog/eligibility query.

## Candidate seam and preserved invariants

Candidate: a tightly scoped postfix/result projection on `CustomDrawers.GetNewWOPPrefabsNames`, only for the current valid grave. Match a returned **completed** descriptor to its actual supported inventory item/slot, then replace its visual name with the saved or temporary choice. Keep native construction placeholders, stage, vegetation and unsupported entries unchanged. No item mutation or generic resource-path interception is needed.

Because native descriptors originate from actual installed items, no descriptor means no displayed decoration: a saved choice cannot conjure an empty slot. Missing/invalid/unsupported saved choices should leave the real item's native descriptor intact, without deleting the paid choice. Exact validation/catalog policy remains to be selected; never use list order as a persisted appearance ID.

The native redraw loop handles stage fallback and grass. Preserve it rather than duplicate all construction/damage logic. Changing to an appearance that lacks a requested stage can fall back to its available stage; this is native visual fallback, not a durability/quality repair.

Alternatives considered: swapping actual item IDs would leak into inventory, installation and quality; replacing the entire grave drawer duplicates native state/damage/fallback behavior; ad-hoc SpriteRenderer writes risk being overwritten and omit composite parts/materials. Descriptor projection currently has the narrowest evidenced scope. Reopen if actual prefab/lifecycle checks reveal unwanted behavior.

## Bounded asset check

For the 42 checked GraveStone/GraveFence definitions, named stage-1 GameObjects with WorldObjectPart exist for all 42; 29 have all three named stages and 13 have only stage 1 in this bounded name check. Nine have at least one named herb part. This is **not** proof that every native Resources path resolves or that every look is unlocked/compatible/rendered correctly.

Stage-1 hierarchy inspection of all 42 found Transform, SpriteRenderer, WorldObjectPart and selected native rendering/sorting/shadow/ground helpers. Every hierarchy also contains BoxCollider2D; the checked collider metadata is identical and disabled (layer 0, same size/offset, non-trigger). Do not equate a WorldObjectPart with a single sprite or infer all later stages/runtime collider states from stage 1. Live interaction, sorting, transparency and collateral behavior need a representative check.

## Fitting, save and close lifecycle

Proposed state: the grave stores committed gravestone/fence codes; an in-memory fitting context holds a target belonging to the current loaded save/session plus temporary slot choices. Selection changes only that context and requests a bounded visual refresh. Confirm validates target/slot/price and commits the grave code/payment; cancellation clears the context and redraws from committed state. No temporary code is placed in grave parameters, so saving during fitting must persist the committed choice, not the preview.

`GraveGUI.Hide(bool)` exists as a concrete native close seam; it refreshes the nearest interaction hint, disables the selection frame and calls base.Hide. Other close/load/target-loss paths and UI restoration still require inspection/execution. A stale unique ID alone cannot authorize a preview on another save; current live target/save ownership must be revalidated.

WorldGameObject.Redraw refreshes more than grave parts (component/craft lists, docks, transparency, ground sprites and buff checks). A direct sub-drawer refresh may omit those integrations. Select the actual refresh boundary after checking callers/cache ownership; do not add a per-frame full redraw or claim preview is fully solved by finding the descriptor method.

## Readiness and next evidence

The installed 0.1.1 diagnostic passes the first narrow slice: committed wooden-cross projection, explicit redraw, native menu-load recovery, temporary stone-cross fitting, close cancellation and cleanup on one occupied completed grave. Actual WOP names changed as expected, while all measured native quality/item/body/currency snapshots stayed identical. The user confirmed the visual sequence. Exact source/artifact/log identity and limitations are owned by [TEST_BUILD_LOG.md](TEST_BUILD_LOG.md).

The installed 0.1.2 extension also passes independent gravestone/fence choices on two occupied graves: native removal leaves no ghost part, ordinary reinstallation restores the retained choices, exchanged distinct gravestones adopt the destination grave's look, and both code pairs survive menu-load. Passive checkpoints observe native craft redraws without forcing refresh. Returned physical items carry no probe parameters. All eight own redraws preserve measured state; cleanup restores current physical-item looks. Both physical fences were the same ID/durability, so individual fence provenance and a different-ID fence exchange are not established. Exact evidence and native-log watch items are in TEST_BUILD_LOG.md.

**Production gate remains BLOCKED for the full feature.** The core grave-owned appearance model is supported in this bounded lifecycle, not universally verified. Remaining distinct boundaries include movement/exhumation, full restart, save/load during fitting, target loss, damaged/construction runtime visuals, catalog policy and payment/coverage/native picker behavior. No production code exists. Do not repeat the passed healthy occupied-grave removal/menu-load loop without a new reason. Resolve the remaining boundary required by the next concrete production slice; full Redraw is sufficient for the probe, not yet selected as the minimal production refresh strategy. General native log errors have unassigned ownership and must not be described as either proved probe defects or proved unrelated.
