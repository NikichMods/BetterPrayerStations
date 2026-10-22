# Appearance probe — research-method checkpoint

Question: does one actual grave render a projected gravestone descriptor, retain the committed choice through native save/load, and restore it after fitting closes?

Existing evidence: pinned native source/IL, asset metadata and passing native binary fixture establish the carrier and descriptor owner. No existing exact mod implements the chosen behavior. A direct vanilla action cannot supply a projected descriptor. A small research-only BepInEx plugin is needed to supply that input and capture actual WOP names/unchanged item state; visual judgment and native save/load remain user actions.

This is research, not a READY production appearance feature. Only gravestone, two fixed existing looks and one opted-in grave are tested. Fence, costs, radius and final native UI are excluded.

| Case | Native path | Expected evidence | Cleanup |
| --- | --- | --- | --- |
| Commit look A/B | Native descriptor builder, native WGO redraw/resource loading | Actual additional WOP names change; quality/items/body/currency snapshot unchanged | Cleanup button |
| Explicit redraw | Same native redraw with committed choice | Same actual appearance and unchanged snapshot | None |
| Save/menu/load | Real game save map/restore/start callback | Marked grave recovered once; committed look applied; visual confirmation | Cleanup then save |
| Fit alternative then close grave | Temporary result projection, native GraveGUI.Hide | CANCEL reason; next-frame native redraw from committed choice; no saved look change | Automatic |
| Cleanup | Own two namespaced test parameters set to zero, native redraw | Normal appearance, no active saved marker | Save once to persist cleanup |

Input guards: current save/player inventory ownership, live selected grave with completed installed gravestone, no active craft, exact verified assembly identity and known resource paths. No world enumeration in recurring paths. Load invalidates all transient target references before native scene restoration; one existing WorldMap list pass after actual game-start recovers an opted-in marker. Multiple marked graves fail closed.

Save classification: explicit target selection/committed look is save-persistent, scoped to two `nikichmods.bps.appearance_probe.*` parameters. Preview is nonpersistent and never written to grave data. Cleanup leaves inert zero-valued namespaced fields, preserving all unrelated parameters; no global parameter trimming. Test marker remains after removing the DLL unless cleaned/saved; vanilla ignores those namespaced fields.

Full WGO.Redraw is the controlled refresh under test, not a claimed optimal production refresh. Bound one native call per action, queue panel actions onto Update, log before/after and inspect actual WOP instances. No per-frame redraw/logging. Native Hide schedules cancellation redraw for the next frame after clearing preview immediately.
