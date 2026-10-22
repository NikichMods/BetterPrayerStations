# Persistence research — 2026-10-23

Status: **bounded static evidence plus offline native binary serialization fixture**. No installed-game acceptance, production patches or changes to user saves. Product requirements remain in `DESIGN.md`; reusable host findings belong to [shared graveyard runtime research](https://github.com/NikichMods/GraveyardKeeperResearch/blob/codex/graveyard-design-candidates/docs/GRAVEYARD_RUNTIME.md#grave-and-decoration-persistence--2026-10-23).

## Question and result

Can grave investments stay with the grave while cosmetic identity follows a physical decoration through installation, removal, storage and loading?

The evidence supports native object/item parameters as the first implementation family to pursue. A separate coordinate-keyed ledger is not currently justified. A deliberate transfer of cosmetic metadata at decoration removal is necessary; native output generation does not automatically copy the installed item's custom fields.

This is a recommendation grounded in the inspected paths, not a completed save-compatible implementation or permission to change adopted product behavior.

## Evidence identity and method

- Game target: GK 1.407 PC.
- `Assembly-CSharp.dll` SHA-256: `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`.
- `Assembly-CSharp-firstpass.dll` SHA-256: `9dc6def3b7715dd27eeb168ddc0af47e31c6f38d3fbee24bf592899392026498`.
- `resources.assets` SHA-256: `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`.
- Supporting source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`, selected Item/GameRes/SerializableWGO/SerializableGameMap/WorldGameObject/CraftComponent/ResModificator/MultiInventory/GraveGUI and grave FlowCanvas node files. Local IL corroborated the selected ownership, transfer, removal and restoration methods.
- Prior research already demonstrated namespaced Item binary serialization in ResearchableAlchemyRecipes. Reused that finding; the new fixture specifically adds the composite native map/WGO/grave/nested-decoration boundary.
- Bounded native data inspection covered the four grave states, relevant scripts/recipes and all 42 ItemType.GraveStone/GraveFence definitions. No bulk proprietary payload is committed.

## Grave storage and identity

`SerializableGameMap.SaveSceneToMe` captures world objects through `SerializableWGO.FromWGO`. The record stores both `unique_id` and `item = wgo.data`. Restoration rebuilds scene objects, restores their saved ID and Item, then reconfigures the object definition. Thus a Unity object reference is transient across loading, while grave state can travel in the saved Item without a second identity ledger.

Normal grave state changes use `ReplaceWithObject -> SetObject -> InitNewObject` on the same WGO. `InitNewObject` changes the data Item's definition ID/capacity rather than replacing its Item; `SetItemID` does not clear parameters. The inspected burial/exhumation chain is `grave_empty -> grave_corp -> grave_ground -> grave_exhume -> grave_empty`. Exhumation's graph uses `Flow_ExhumeBodyFromWGO` to drop/remove the actual body from the grave inventory, not replace the grave data. Native craft End removes zero parameters; positive custom counts/schema markers survive that cleanup path.

Recommendation: store schema/version, conversion capacity, extra-wreath count and shared paid-action count on grave data under a project namespace. Zero/missing can mean an unpurchased default; positive schema markers can identify initialized state. Exact keys/migration and maximum valid values are not final. Keep effective coverage/score derived, rather than persist a particular station ID or active effect list. Body data remains untouched.

Coordinates are not a stable primary key under movement. The WGO ID can aid diagnostics and lifecycle correlation, but no global external registry is currently needed. Arbitrary deletion/recreation and other movement mods remain unsupported until inspected/tested.

## Decoration lifecycle

| Boundary | Evidence | Consequence |
| --- | --- | --- |
| Installation | GraveGUI passes the chosen instance; native transfer-needs branch inserts that instance for the selected single-item path | Cosmetic parameters have a carrier into grave inventory |
| Storage transfer | MultiInventory copies Item; Item copy constructor copies GameRes; non-stackable AddItem copies Item again as needed | Copied custom parameters can follow the decoration through the inspected normal transfer path |
| Save serialization | Native map/WGO and Item fixture retains nested appearance code and durability | Serialization can preserve item-owned metadata; not a live inventory or world test |
| Removal | Recipe output is copied from its definition; original installed item is removed later; native handling explicitly propagates durability, not arbitrary appearance keys | Add a narrow, validated transfer of the chosen cosmetic metadata before the generated result is dropped |
| Inventory/world rendering | Icons and world visuals resolve native definition data unless explicitly extended | A saved appearance key does not itself render the requested look |

The current native corpus has 42 GraveStone/GraveFence definitions, all stack_count=1, each with one matching grave installation recipe and one matching removal recipe producing one base-definition item. This includes definitions whose gameplay unlock/placement status is not proven; it is not the final available-look catalog. GraveCover and mod-added definitions are outside this corpus.

Potential removal seam: while the native removal output list is being generated, scope to the actual grave removal craft and exact installed slot item, then annotate its corresponding generated output before native DropItems. Preserve native quality, ID, durability, work, tools, scripts and output multiplicity. This candidate is **not READY**: all matching result-generation callers, interruptions and the exact drop/copy path still need a representative executable slice. Do not broadly override Item copying or globally rewrite ResModificator outputs.

Active native craft serialization has an additional trap: `_current_item` is restored from ID/durability, while `cur_craft_items_used` is stored separately. An in-memory captured Item reference cannot serve as the sole source across save/load. For removal, prefer the still-installed decoration in the saved grave inventory if its lifetime is verified, or deliberately persist narrowly scoped pending metadata. That choice remains open.

Cosmetic codes need a stable, versioned mapping independent of dictionary/list ordering. Separate new item-definition IDs are less attractive for the initial scope because native installation matches exact input IDs and loading requires those definitions. A separate JSON sidecar adds save pairing/identity/atomicity work without a demonstrated need. Reconsider if native metadata fails a required lifecycle.

## Movement-mod boundary

The installed Move Objects 0.6.0 DLL matches the already inspected SHA-256 `c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0`. Reused shared evidence: its selected `MoveExistingWGO` route moves the existing object instead of replacing it. This makes data-attached investments preferable to coordinate keys. It does **not** prove grave eligibility, cancellation, position persistence, redraw, radius invalidation or all movement tools. Do not claim full compatibility yet.

## Offline executable check

Original script: `research/persistence/native-serialization.ps1`. Run in a fresh PowerShell process with `-ManagedDirectory <local game Managed folder>`. No DLL is built, no game process is touched, and no save file is read or written.

Method: synthetic Items are allocated without constructors because native constructors consult Unity Resources. Native GameRes parameter access and **native SmartSerializer** serialize/deserialize a native SerializableGameMap containing a WGO record, grave Item, decoration and body; a separate chest Item is also round-tripped. The first direct struct call was rejected by the serializer's class generic constraint; the successful test uses its real map container, without replacing the serializer.

Result: PASS for saved WGO ID, conversion/wreath/shared-count fields, nested cosmetic code, original durability, absence of grave modifier on the body, chest contents, independent restored graph and fresh-grave default. These prove serialization representability and separation in the fixture, not installed persistence of a mod that does not exist yet. No copying/crafting/movement/rendering methods were simulated as proof of their native execution.

## Readiness and next test

- **Serialization carrier:** established by source/IL and bounded native fixture.
- **Full production persistence and cosmetic removal:** **BLOCKED for implementation acceptance**, pending exact mutation/drop boundary, interrupted removal and representative live cycle.
- **Rendering and coverage:** not investigated by this persistence slice.

Next useful slice: one grave and one ordinary decoration in an explicitly identified research candidate, using reversible markers only. Exercise install -> remove -> chest -> save/load -> install on another grave; separately save/load an invested grave, exhume/replace its body and move it through the selected Move Objects route. Compare actual metadata before/after and original body/decorative quality, including save/load during manual removal and cancellation. Select the narrow diagnostic method under DevRules before building that candidate; this document does not claim a probe has been supplied or the runtime gate has passed.
