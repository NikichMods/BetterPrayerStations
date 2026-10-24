# Prayer-station management through native UI — 2026-10-24

Status: **bounded static feasibility established for native menu reuse; session-only prototype installed, visual/runtime acceptance remains open**. This is research, not implemented production behavior. DESIGN.md owns gameplay; no production payment or persistence is introduced.

User-authorized next slice is now implemented as `research/MenuProbe`0.1.0, a separate session-only native-menu/editor diagnostic. `research/MenuProbe/PLAN.md` owns the bounded readiness/method, README the user scenario, and TEST_BUILD_LOG the exact frozen artifact/install identity. Local compile/compiled-policy/native-dispatch gates pass; live UI/input acceptance remains pending. The source adds a scoped suppression of the ordinary native removal card and uses BuildType.None for virtual action cards to avoid native physical-output description resolution. No faith debit, grave conversion or saved parameters are included.

## Question and method

Can one native construction-style window offer Buy capacity / Place / Remove, while faith purchases permanent capacity and world points are free to edit? Reused shared crafting/building/payment evidence, then inspected exact local native metadata/IL and only the relevant object definitions. No new runtime probe was needed for the route question; static inspection answers dispatch/ownership, not visible fit or input acceptance.

GK1.407 inputs: Assembly-CSharp.dll SHA256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`; resources.assets SHA256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`. Original local inspector/selected inputs are ignored under `.local/evidence/native-blessing-menu-2026-10-24/`. Thirteen route assertions pass; they inspect actual IL/data, not live UI. Route report SHA256 `fa01ec0ac485f156e718f91dc9352ca77e070db439fc587653e901450ab6b194`.

## What the host already provides

- The normal builder window is `CraftGUI`, reached via `MainGame.OpenBuildObjectGUI -> CraftGUI.OpenAsBuild`. Native cards, scrolling, selection and gamepad focus have existing host owners.
- `CraftGUI.custom_craft_interface` accepts `CraftInterface`. `CraftItemGUI.CanCraft` delegates availability to its CanCraft; `CraftItemGUI.OnCraftPressed` delegates execution to its OnCraft when supplied. The custom path disables native zombie batching, but definitions still need appropriate single-action settings and an idle context. It does not remove every earlier card/queue check.
- `CraftsInventory.additional_crafts` appends transient ObjectCraftDefinition records in GetObjectCraftsList. Native CommonOpen still applies craft visibility/type filtering. Unique action IDs and valid native card presentation are needed, but this route does not itself require publishing invented recipes to the global balance cache or saving them as completed crafts.
- The native synthetic `_remove_` card demonstrates an action card without a crafted output. Use distinct mod action IDs, not that reserved ID.
- `UniversalObjectInfoGUI` exposes header/description labels; a scoped header/counter addition is plausible. Exact layout, localization, update order and gamepad readability are not visually verified.

Important: **OpenAsBuild resets custom_craft_interface to null**. A custom opening path must install the controller at the verified point before cards evaluate availability; do not assign it and then call OpenAsBuild expecting it to survive. Normal closing/reopening also requires explicit session cleanup/restoration. Hide does not itself clear custom_craft_interface; its ON_CRAFT_GUI_HIDE callback is one-shot and must not accidentally reopen the menu during a transition into placement.

The two prayer-station definitions currently have interaction_type=0, has_craft=false and an empty zone_id. The native graveyard desk has interaction_type=4 and zone_id=graveyard. Thus adding a usable station entry/hint and explicitly choosing the cemetery build zone are necessary; simply calling OpenBuildObjectGUI on an unmodified prayer station cannot be advertised as sufficient. ComponentsManager constructs its component set separately; native CommonOpen requires a valid CraftComponent/context even for a custom controller. The actual selected station and native interaction route need a bounded live check.

## Why the ordinary build/remove actions are insufficient

| Action | Reusable native surface | Required mod behavior |
| --- | --- | --- |
| Buy +1 capacity | Card, faith ingredient display, enabled/disabled state | Revalidate quote and P<C; debit once and save P through the project-owned transaction. No output item, construction project or queue entry. Refresh price/counters after commit. Reuse bounded payment evidence, not a claim of a ready production transaction. |
| Place | Menu selection and zone/grid/camera facilities | Add a virtual field at a valid cemetery cell; check simultaneous capacity at commit; permit the adopted grave overlap. No material debit or physical construction stage. |
| Remove | Menu selection and mode presentation | Select only the project's field markers, including dormant points; remove one location, preserve P/price and update placement priority. Never pass a grave/station to native demolition. |

Native DoPlace removes materials, commits/possibly replaces a real floating WGO, performs quest/one-time bookkeeping and can prepare another preview. Native EnterRemoveMode marks ordinary removable cemetery objects; DoRemove calls WorldGameObject.MarkForRemoval. Those complete actions do not express a virtual light-field editor.

BuildGrid.RefreshGrid combines zone/subzone availability and occupancy. Reusing ordinary occupancy unchanged would conflict with placement over real graves. Preserve cemetery availability/locked-area boundaries while separately proving the light-field cell policy; a global IsCellBusy override is not justified. Coordinate/footprint evidence from prior square coverage does not prove new field membership or over-grave marker rendering.

The host has script-building callbacks, but UpdateWhileScriptBuilding refunds _cd.needs on cancellation, clears the build zone, runs the callback and exits on confirmation. It also reads _cd.has_variations, uses native global state and has a cancel path that reopens the native builder. This is not a ready repeatable Place/Remove controller, and EnterScriptBuilding alone is insufficient setup.

## Mechanism comparison and recommendation

1. **Unmodified native construction recipes for everything:** insufficient. It conflates permanent capacity with world-object crafts and exposes ordinary demolition/occupancy side effects.
2. **Native CraftGUI/cards through the existing custom interface, plus a scoped field-placement/removal controller:** recommended. Reuses the actual native window/selection system and keeps virtual points/payment semantics explicit. Reuse zone/grid/camera facilities where their verified contracts fit; choose exact editor integration after the narrow lifecycle/input check.
3. **Entirely custom window and world editor:** currently unjustified. Duplicates a proven card/focus/scroll surface without removing the world-mode integration work.

The recommendation is not permission to silently alter gameplay or globally replace crafting/building handlers. No production mutation readiness is asserted for the entire editor. Known blast radius: one native shared CraftGUI session, its own temporary action records/controller/labels, current build/grid/camera/input state and project-owned counters/field locations. Unrelated stations/crafting/removal must retain native behavior.

## Next evidence slice

A narrow in-game UI prototype should prove: open either completed station, show exactly three actions plus readable counts/price, exercise custom availability/click dispatch, leave/reopen without controller leakage, and transition into/out of a temporary grid mode with visible over-grave markers. Verify mouse and gamepad, back/cancel and repeated placement/removal, occupied and unavailable ground, capacity exhaustion and dormant point selection. Start with session-only counters/markers, no real faith debit or save changes: payment/persistence integration is a later slice using existing accepted seams. No final light art is needed for this check.

Static research does not establish the actual native layout, a safe final grid adapter, clean optional-mod compatibility or installed acceptance. No diagnostic was built/installed in this investigation.
