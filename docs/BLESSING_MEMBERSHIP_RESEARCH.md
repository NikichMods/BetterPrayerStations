# Spatial blessing membership —2026-10-24

Status: **any occupied grave cell adopted; session-only effect prototype authorized, installed effect acceptance pending**. Menu0.1.1 grid/actions are accepted within their recorded scope; menu polish remains deferred.

## Menu0.2.0 readiness checkpoint

READY for a research-only joint field/effect candidate. Observable: active occupied-cell fields convert all effective reds to whites, nonstacking, restoring native result when the last qualifying field deactivates. Owners/final consumers: current WGO.wop layer23 placement box -> native32-unit occupied-cell convention; WorldGameObject.get_quality skull inputs -> unchanged native floor/subtract/min; GraveGUI.RedrawGraveInventory skull inputs -> unchanged diagram loops. Reuse the previously tested GridFootprint shape projection and guarded native skull-call rewrite, without decoration projection, persisted upgrades or old radius/passive systems. Initialise optimized colliders only on current-part change, cache per owner/part, derive bounds from runtime transforms on use; no global recurring world scans or dependence on _cells readiness. The native box-overlap tolerance supplies the same discrete footprint policy as prior native occupancy evidence. Unsupported geometry fails the probe rather than guessing.

Blast radius is owned grave calculations/central diagram only, current-save/cache guarded. Shared Item.GetBodySkulls and body-panel rendering are not patched. Raw items, decorations, currency and saves are preserved. Initial grave registry shares the existing game-ready world scan and native add/definition-change event; bounded cached refresh detects active-set/body/value changes and updates native badges/zone feedback. Reset clears activation before refresh/old-session release. Cheapest checks: compiled native rewrite guards/branch metadata, full-conversion native Item oracle across decay thresholds, geometry fixtures and existing state caps/order gates; installed boundary cells, effect on/off/duplicates, diagram/zone, cleanup and reload are the new acceptance envelope. One joint candidate is simpler than an extra geometry-only installation; no UI polish, payment or persistence.

## Proposed player rule

**An active light placed in any construction cell occupied by a grave blesses that grave.** A special central cell, nearest-grave radius or percentage coverage is unnecessary. A neighbouring cell does not qualify merely because a tall gravestone sprite extends over it. Multiple qualifying active fields still produce one conversion, as already adopted. Field priority/capacity remains independent of target-grave lookup.

Empty graves retain the spatial relation but have no body benefit; a replacement body at that location receives the effect. A field stays at its coordinates if the grave moves or is removed. These consequences reuse adopted spatial/empty/replacement semantics, not new payment or ownership policy. Free ground placement remains decorative when no grave occupies the selected cell. This paragraph does not select behavior for unsupported overlapping/rotated graves created by other mods.

Recommendation: match the host construction-cell occupancy test, rather than sprite bounds, transform distance, centre-only targeting, or tiny physical-edge intersection between two arbitrary rectangles. One discrete cell is either a grave cell or it is not. This makes edge handling consistent with the native placement grid without teaching the player collision tolerances.

## Verified host evidence and limits

GK1.407 Assembly-CSharp SHA256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`, rechecked; resources.assets `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`, checked by local inspector. Shared canonical facts: GraveyardKeeperResearch/docs/GRAVEYARD_RUNTIME.md, small construction cells and placement-box sections.

- Native small cells are32 world units. The MenuProbe point indices already refer to those cell centres; WGO.grid_pos uses a different96-unit scale and must not substitute for them.
- WorldGameObject.RecalculateGridShape initializes optimized colliders, filters relevant colliders with BuildGrid.SkipCollider, and populates _cells using BuildGrid.IsCellBusy at32-unit centres. DoesIncludeGridPos only searches that cached list.
- BuildGrid.IsCellBusy queries a28.8×28.8 physics box and optionally filters hits to the supplied collider list. With a build collider present, SkipCollider selects layer23 rather than arbitrary visual/body colliders. This is a construction occupancy contract, not pixel-perfect sprite hit testing.
- Fresh bounded resource inspection confirms matching layer23 box position/offset/size for grave_ground, grave_empty and grave_corp, despite different visual subtrees. Raw prefab geometry is not world-space dimensions: use actual runtime transforms. No claim of a fixed cell count is established here. The inspector did not establish full grave_exhume geometry, so do not infer it lacks a footprint from absence of one serialized BoxCollider2D alone; optimized collider initialization is a separate host path.
- Earlier installed evidence found empty cached _cells on an initially loaded grave; a direct DoesIncludeGridPos false is therefore not sufficient proof of spatial exclusion. Reusing the cache unconditionally would introduce false negatives. Current WGO.wop owns the current part; native RedrawPart replaces it, so shape discovery must follow that owner and invalidate on replacement/movement. Earlier GridFootprint code supplies relevant mechanism evidence, not automatic acceptance for this new point-membership rule.

Ignored local selected-shape report: `.local/evidence/native-blessing-menu-2026-10-24/grave-footprints.json`, SHA256 `adbdf217fd35085b4e0c295c7457fc1ac69a0c3da68f97da152b0f660534c432`. Original inspector and bounded IL dumps remain local; no copied game assets/source are persisted publicly.

## Next implementation evidence after agreement

Derive discrete membership from current grave placement geometry using the native occupancy convention; do not depend on an already-populated _cells cache, sprite silhouette or hardcoded prefab coordinates. Keep current-part initialization/lifecycle and active-field lookup separate from the effective-skull projection. The exact adapter is not declared production-ready by this proposal.

In the next session-only effect candidate, verify an interior cell, each relevant edge and adjacent outside cells; multiple lights on one grave do not stack; empty/replacement grave states retain the spatial rule; loading and current-part replacement do not produce stale membership. Prefer this bounded joint effect check over another geometry-only installation. No anatomy recipes/costs are involved or disclosed. Payment/save changes remain later, and no further menu polish is required first.
