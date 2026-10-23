# Test-build log

## Appearance Probe 0.1.0 — failed initialization; preserved

Research-only plugin, not production Better Prayer Stations or a stable release. Compiled/installed on 2026-10-23; user launch reported the disabled-panel message. Current session log confirms INIT_FAILED before READY, so no rendering/save/preview scenario was exercised or accepted.

- Exact clean source: `4d2e1bc786858978f179ee215e92d47c15fc85d8`.
- Frozen reference: `probe/appearance-0.1.0`.
- Numbered artifact: `BetterPrayerStationsAppearanceProbe-0.1.0.dll`.
- Canonical installed basename: `BetterPrayerStationsAppearanceProbe.dll`, isolated plugin directory.
- Artifact/installed SHA-256: `fea781138d693630d9bb96b44f711d40c53ae6b2d15860de31eaec7da258cc3b` (identical).
- Canonical version owner: `research/AppearanceProbe/Plugin.cs` Version; assembly/file versions derived and checked as 0.1.0.0.
- Build: `research/AppearanceProbe/build.ps1 -GameRoot <local game root> -DotnetRoot <local SDK root> -ExpectedSource <exact SHA>`; Windows SDK6.0.406 Roslyn, warnings as errors, locally installed host references. Full local reference hashes are in the ignored build-identity manifest.
- Gate PASS: compilation, assembly/file version identity and real native WOPPrefabName-based helper tests for names/cache, stage/suffix/herb preservation, unchanged source descriptor, unrelated fence, construction and empty/unsupported slots. These tests do not instantiate a Unity scene or prove native close/load behavior.
- Installation: exact artifact verified, no prior same-probe payload found, one active copy installed while host closed. Previous ordinary game log preserved privately under ignored evidence storage. Local installation manifest identifies paths/time/hash; no machine paths or raw host logs are published here.

Question/matrix: [PLAN](../research/AppearanceProbe/PLAN.md). Gameplay procedure and save-state rules: [probe README](../research/AppearanceProbe/README.md).

Pending user scenario: choose one completed occupied grave -> commit wood/stone gravestone look -> ordinary redraw -> native save/menu/load -> fit alternative -> close native grave window -> observe committed look restored -> cleanup and save. Accessible LogOutput.log is collected by the agent after the run.

Expected evidence: BPS_PROBE READY/SELECT/REDRAW_BEGIN/REDRAW_END with actual part names and unchanged snapshots; LOAD recovery; CANCEL reason=grave-close plus cancel redraw; CLEANUP. Record loader failures, failed/absent seams and user-visible outcome separately from compile success.

Save classification: two explicit namespaced diagnostic fields are save-persistent; preview is not saved. Cleanup zeroes only these two fields, returns native appearance and requires one ordinary save to persist cleanup. No faith, money, physical body or decoration data is intentionally changed.

No source or numbered artifact may be overwritten under this handed identity. Any corrective candidate after handoff gets a new number; the production appearance gate remains open.

### Observed 0.1.0 failure and narrow correction

Installed 0.1.0 hash matches its exact artifact, and the game assembly hash matches the inspected host. Log snapshot SHA-256: `dffc024dd93a5461bc83f0b7d9576fbe8cb0f2f24bc9fb04fb2f392173bf035a`, retained privately in ignored evidence storage. Harmony reports `Parameter "grave_wgo" not found in method virtual void BaseGUI::Open()` while installing the grave-open postfix. The old name-only AccessTools lookup selected an inherited parameterless BaseGUI method, rather than the native declared GraveGUI.Open(WorldGameObject) supported by installed metadata/source. This is a probe binding defect, not evidence of an unsupported game build.

Correction scope: resolve all five targets by declared owner, exact argument types, return type and implemented body before patching; reject inherited/no-argument grave Open; bind the grave postfix argument by index. Improve the panel's generic initialization failure wording so it does not misdiagnose every patch failure as an environment mismatch. No product mechanics, save fields, looks, UI workflow or rendering algorithm changed. The descriptor mechanism remains justified; this failure occurred before testing it.

Local regression gate passes exact bindings for CustomDrawers.GetNewWOPPrefabsNames(), GraveGUI.Open(WorldGameObject), GraveGUI.Hide(bool), MainGame.OnGameLoaded(GameSave), MainGame.OnGameStartedPlaying(), plus inherited-overload rejection and indexed argument contract. Existing descriptor/version tests also pass. This is not yet a successful installed Harmony startup.
## Appearance Probe 0.1.1 — narrow installed scenario PASS

Corrective research candidate for the 0.1.0 initialization failure above. No production release or full product acceptance is implied.

- Exact clean source: `f2857b63ad8655f4fc17892f3e1ba00215681257`.
- Frozen reference: `probe/appearance-0.1.1`.
- Numbered artifact: `BetterPrayerStationsAppearanceProbe-0.1.1.dll`.
- Canonical installed basename: `BetterPrayerStationsAppearanceProbe.dll`.
- Artifact/installed SHA-256: `3d65bf6d32ed485fdc9efe6eeee2b1297cd8329383e5ee9d35fa5dcbd1cef36d` (identical).
- Canonical version owner: Plugin.cs Version; assembly/file versions checked as 0.1.1.0.
- Build gate: clean exact source, SDK6.0.406 Roslyn compilation with warnings as errors, five exact declared patch targets, inherited Open() rejection, indexed argument binding contract, native descriptor projection and version identity tests all PASS.
- Installation on 2026-10-23: host process absent immediately before replacement; prior 0.1.0 DLL backed up outside the plugin directory; exact new artifact copied to the canonical basename and hash verified. Private installation manifest retained in ignored evidence storage. Numbered 0.1.0 bytes remain unchanged.
- Runtime status: installed startup and the requested narrow scenario passed on 2026-10-23; details below. This is research acceptance, not stable promotion or full product acceptance.

### Installed return — 2026-10-23

User reports every visual step matched the procedure and supplied a native grave-window screenshot. Fresh session log identifies 0.1.1 and READY with the expected game assembly hash; installed DLL still matches the artifact hash above. Preserved private log SHA-256: `b8d41ce97045c3c1db922f7a4695d8e88c44677c4c30a8ffec388588cc0dd23f`. Screenshot SHA-256: `923520be3ac4be3248749194ad4b4f548e52d0cb2b8c137f7f8cab551479b2c8`. Raw log/image are retained only in ignored local evidence, not public assets.

Selected grave 47567 has actual `grave_top_stn_plate_2` and `grave_bot_stn_2`, native quality 3. Six REDRAW_END records all report invariants=True; all recorded state strings are identical across SELECT and before/after actions: native quality, actual item IDs/counts/durability, direct body contents, player money and faith. This is bounded instrumentation, not a bytewise comparison of every save field.

| Step | Recorded world result |
| --- | --- |
| Commit wooden cross | Actual WOP becomes `grave_top_wd_cross_1_stg_1`; stone fence retained |
| Explicit redraw | Same wooden cross and fence |
| Native menu/save-load | LOAD recovers grave 47567 with committed=1; wooden cross restored |
| Temporary stone-cross fitting | `grave_top_stn_cross_1_stg_1`; committed remains 1 |
| Close grave window | CANCEL reason=grave-close; persisted_look_unchanged=true; wooden cross restored |
| Cleanup | Original `grave_top_stn_plate_2_stg_1` restored; active_keys=0, inert_zero_keys=2 |

No probe FAILED/INIT_FAILED or invariant violation appears in this snapshot. Other host errors exist; do not call the entire session error-free. In particular, a missing `grave_top_stn_plate_2_stg_2` resource is logged at line731, before initial target selection at line1043. The tested redraws all instantiate stage1; this record is not a failed probe redraw, and its independent cause/owner is not established here.

The screenshot records native UI quality3, body3 red/3 white at97%, and the original physical gravestone/fence names and icons, each quality3. It is a useful native UI baseline, not a screenshot proving every world appearance or a finalized cosmetic picker design.

Limits: one occupied, completed grave; gravestone only, two test looks. No fence-choice mutation, removal/reinstallation, physical item transfer, empty/construction slot runtime test, exhumation, movement, interrupted fitting load/save, full application restart or production payment/coverage/UI acceptance. Cleanup was observed in memory; a subsequent save/load proving persistent cleanup is not recorded. Next useful lifecycle slice is two independent grave-slot choices through decoration removal/reinstallation and destination-grave transfer; do not repeat the passed basic redraw/close scenario without a new reason.

## Appearance Probe 0.1.2 — bounded two-slot lifecycle PASS

Research-only extension requested after the passing 0.1.1 slice; not production or stable promotion. Previous 0.1.0/0.1.1 numbered artifacts remain unchanged.

- Exact clean source: `990b5038bfefe135f31d498ac970f7dee2e5e077`.
- Frozen reference: `probe/appearance-0.1.2`.
- Numbered artifact: `BetterPrayerStationsAppearanceProbe-0.1.2.dll`.
- Installed basename: `BetterPrayerStationsAppearanceProbe.dll`; single active payload in the isolated probe directory.
- Artifact/installed SHA-256: `9b3b520b07afb8ab52a4a276bddfaf7cce07977461f5c9f0fcf2bdf174c5b938` (identical).
- Canonical version owner Plugin.cs Version; assembly/file versions derive from it and passed 0.1.2.0 identity checks.
- Build: `research/AppearanceProbe/build.ps1 -GameRoot <local game root> -DotnetRoot <local SDK root> -ExpectedSource <exact SHA>`, SDK6.0.406 Roslyn with installed host references, clean source, warnings as errors. Local build-identity manifest includes all reference hashes.
- Gate PASS: compilation/version, four exact declared implemented patch signatures, inherited parameterless Open rejection/indexed argument, real native WOP name/cache preservation, independent top/fence projection, missing individual slots, unfinished descriptor, stage/herb/suffix preservation and invalid saved-code rejection. These are not Unity/craft/load acceptance.
- Installation 2026-10-23: game process absent immediately before replacement; verified prior 0.1.1 payload and latest ordinary log preserved outside plugin directory; exact candidate copied and installed hash rechecked. Private paths/manifest/log remain ignored local evidence.

Scope: at most one marked A and B, three diagnostic fields each (marker/top/fence); two independently selectable top/fence choices. Same descriptor projection/native full-redraw mechanism. Previous fitting UI/Hide hook excluded from this new slice; its accepted behavior stays frozen in 0.1.1. No native item/craft/transfer patches, item generation/mutation, payment or coverage mechanics. One recovery pass after load, redraws paced one grave per frame. Explicit passive checkpoints do not force redraw and record both graves plus player's direct decoration inventory, including any unintended diagnostic metadata on items.

Requested procedure and save classification: [README](../research/AppearanceProbe/README.md); question/matrix/why this extension: [PLAN](../research/AppearanceProbe/PLAN.md). A = wooden cross/stone fence I; B = stone cross/stone fence II. Record baseline; native top-only removal; fence removal/pickup; native reinstallation; empty both and exchange actual ordinary decorations; save/menu-load; cleanup both and save. Prefer distinct physical item IDs at A/B so quality/identity transfer is visible. Normal item removal/replacement can change grave quality; unchanged measured state is asserted only immediately across the probe's own cosmetic redraws.

Runtime status: installed READY, native slot removal/reinstallation, distinguishable gravestone exchange, empty slots, two-grave menu-load recovery and cleanup passed with user visual confirmation; see the bounded return below. Fallback execution and cleanup persistence after another load remain unverified. Entire-grave movement, exhumation, full restart, fitting interruptions, all styles and final production UI/payment remain outside this candidate.

### Installed 0.1.2 return — 2026-10-23

User confirms removal, reinstallation, exchange and loading visually matched the procedure. Fresh loader/READY identifies 0.1.2 with the expected host hash; installed DLL still matches the frozen artifact hash. Preserved private session log SHA-256: `72710cba9034a89647dc014f99ca80a57fda7e24529587d6d2bcf1d3a0a8d423`. Ten numbered checkpoints each record both graves and player decoration inventory; all captured craft states are idle. Eight own cosmetic/load/cleanup redraws report invariants=True, with no probe FAILED/PROJECTION_FAILED/INIT_FAILED. All logged diagnostic parameter triplets on body/contents/decoration items are `0,0,0`.

| Checkpoint | A=47567 | B=37815 |
| --- | --- | --- |
| 1, native baseline | physical plate II + fence II, quality3, codes0/0 | physical stone cross + fence II, quality4, codes0/0 |
| 3, choices set | wood-cross/fence-I looks, codes1/1, physical originals unchanged | stone-cross/fence-II looks, codes2/2, physical originals unchanged |
| 4, A top removed | fence-I look only, codes1/1, quality0; ordinary plate in inventory | unchanged |
| 5, A both removed | no parts, codes1/1, quality-3; ordinary plate/fence in inventory | unchanged |
| 6, A reinstalled | both A looks return, codes1/1, quality3 | unchanged |
| 7, both empty | no parts, codes1/1, quality-3 | no parts, codes2/2, quality-2 |
| 8, incoming items at A | actual stone cross/fence II; still A looks | empty with B choices retained |
| 9, exchange complete | actual stone cross/fence II; A looks, quality3 | actual plate II/fence II; B looks, quality4 |
| 10, after menu-load | same exchanged physical data and A looks/codes | same exchanged physical data and B looks/codes |

Body contents/durability and money/faith remain identical in the recorded snapshots; quality responds to actual removal rather than the chosen looks. Natural native crafts, not checkpoint-triggered redraws, produced the observed parts at checkpoints4–9. Cleanup logs both code pairs zero and restores the current exchanged physical items' ordinary appearances, with CLEANUP_DONE active_keys=0. It does not undo the exchange.

Boundaries: both physical fences are `grave_bot_stn_2`, count1/durability1. The log confirms fence removal/reinstallation and independent per-grave fence appearances, but cannot distinguish which identical fence instance moved where or prove a different-ID fence exchange. Gravestone IDs differ and their exchange is unambiguous. Grave scores3/4 after exchange are capped/body-dependent, not evidence that every incoming decoration quality causes a score delta. Only tested healthy stage1 styles and occupied graves are accepted here. No empty-body/exhumation/whole-grave movement, damaged construction runtime path, full restart, interrupted fitting, arbitrary catalog, production UI/payment or broad compatibility acceptance. No subsequent reload after cleanup is recorded.

The overall host log is not error-free: it contains native resource/GUI/World Object not found messages, including specific missing world IDs during load. Their ownership/cause is not established by this run; the absence of probe failures does not prove universal save integrity or attribute those messages to another mod. Preserve this as a watch item if production redraw strategy or save-integrity symptoms are investigated, rather than infer causality from neighboring lines.

## Faith Probe 0.1.0 — partial runtime pass; zero-red observer failure

Separate research-only artifact, independently numbered from Appearance Probe. No stable release or production mechanics are accepted. User authorized one-grave conversion/wreath effects with explicit activation and no spending or station radius.

- Exact clean source: `dd53a045f1346780c113963e4b03f1e8093db6f3`.
- Frozen reference: `probe/faith-0.1.0`.
- Numbered artifact: `BetterPrayerStationsFaithProbe-0.1.0.dll`.
- Canonical installed basename: `BetterPrayerStationsFaithProbe.dll`, own isolated plugin directory.
- Artifact/installed SHA-256: `21b391c3c317f186db90a6f600228da62431247c13beec451fb864614ab0aedf` (identical).
- Canonical version owner: research/FaithProbe/Plugin.cs Version; assembly/file identity checked as0.1.0.0.
- Build: research/FaithProbe/build.ps1 with exact ExpectedSource, installed local GameRoot and SDK6.0.406 Roslyn DotnetRoot. Clean source, warnings as errors, local reference hashes in ignored build manifest. Proprietary references not distributed.
- PASS: compile/version; four exact targets and inherited Open rejection; both actual installed-native-method IL rewrites, retained generic inventory branch/native floor/min/item-card calls, changed-shape rejection and branch-label transfer; compiled conversion versus independent native skull oracle160 cases; unbound skull wrappers pass through actual original stats. This is not live Harmony/UI acceptance.
- Installation on2026-10-23: game process absent; verified Appearance Probe0.1.2 and latest log preserved outside plugin directory; new faith payload verified before removing only the old appearance DLL from active plugins. One BPS diagnostic DLL active; original numbered artifacts untouched. Private installation manifest/evidence retain local paths.

Method/matrix: [Faith PLAN](../research/FaithProbe/PLAN.md). Gameplay steps/safety/limits: [Faith README](../research/FaithProbe/README.md). Four hooks: WorldGameObject.get_quality and GraveGUI.RedrawGraveInventory input transpilers; declared GraveGUI.Open postfix; MainGame.OnGameLoaded reset prefix. Raw Item/body-panel/global item-quality methods are not patched. No full world redraw or zone-total notification is invoked.

State: in-memory C/L counts0..12 and explicit synthetic activation on one selected grave with the same actual body reference. No saved fields, payment, item creation/edit, radius or tier. Load invalidates targets/counts. Cleanup returns native presentation and clears only memory; no save required. Body/organ/decoration recursive GameRes values/scalars, item IDs/counts and money/faith are hashed immediately before/after each GUI refresh. One next-Update OBSERVE checks actual native total text, central active slot/wreath counts, red-bar height/state, raw body panel and reachability of all four projection inputs. A mismatch disables effects and requests one native UI rollback; do not claim fallback executed before evidence arrives.

Requested scenario: select -> wreath alone -> reset -> one conversion -> three conversions -> excess conversion -> extra wreath -> disable/enable retained effects -> optional close/reopen same grave -> cleanup. Healthy reference example R3/W3/D6: native3, wreath-only still3, one conversion4, three conversions6, excess remains6. Body panel remains raw3/3; central diagram changes. Other body/decoration/decay states use their actual native bounds. No need to repeat cosmetic removal/exchange/load tests.

Runtime status: installed initialization and earlier combinations passed; third conversion reached rating6 but the combined red-bar observation failed and disabled effects. Rollback and cleanup completed. Detailed bounded return follows. No payment, coverage, saved investment, exhumation/replacement/export, construction/damage interaction, broad compatibility or final native UI acceptance follows from this candidate.

### Installed Faith0.1.0 return and narrow observer correction

User varied test order and reports automatic return to native presentation after three conversions. Fresh log identifies exact0.1.0 READY; installed hash still matches the frozen artifact. Preserved private snapshot SHA-256: `de3bc1a51bfac8f534ef12c1a2e8f8e616a41df2c017773f4bce0e47dfe4fd88`. Do not repeat this old binary merely to enforce the originally described order.

Eleven redraws all show physical_unchanged=True and the same physical hash `f32d0f340f0d658e55e3d76529ec3d16d4b33c03ce735036410fc731a64748d6`. The recursive grave/body/organ/item GameRes/currency measurement is unchanged; raw body remains3/3/3 atdurability0.9621399 and logged part skull contributions unchanged. Ten OBSERVE records pass all checks. Observed combinations: C0/L0 native3; C0/L1 decoration7, rating3; C1/L0 effective2red/4available, rating4; C1/L1 active4, dormant3, reactivated4; reset returns3; C2/L0 effective1red/5available, rating5.

At n11 (C3/L0), actual native getter/label both show6; projected diagram inputs0red/6available/decoration6, observed actual_slots6/actual_wreaths6, raw_panel=True,total=True and every seam reached. chart=False then invokes FAILED, followed by ROLLBACK_BEGIN/ROLLBACK_END fallback=vanilla-ui. CLEANUP_DONE persistent_writes=0 occurs later. This is the cause of the visible automatic reset; test ordering is not an identified trigger. Excess conversion and toggle at full conversion were not reached. Overall0.1.0 acceptance is not complete.

The code's remaining failed predicate is either exact bar height or bar visibility;0.1.0 logged neither separately, so the actual failing conjunct/dimension is not established. Fresh installed firstpass IL shows UIWidget.set_height clamps to virtual minHeight and may adjust through anchors; sprite minimums can depend on border/padding. Native GraveGUI.RedrawGraveInventory explicitly hides this widget atzero red before assigning height7. Requiring invisible geometry to equal7 is an unjustified observer condition, even though the specific actual height was not recorded. Corrective0.1.1 checks zero-red hidden state alone, retains positive-red geometry/state and all native count/raw/total checks, and logs every bar predicate. No production decision, effect calculation, native input rewrite or physical data path changes.

Local regression checks pass hidden bars at representative coerced heights, reject visible zero-red bars, reject hidden positive-red bars and wrong visible heights, and retain all installed-IL/native-oracle gates. Mechanism choice remains justified by earlier passing cases; this is a bounded observer correction, not evidence to replace native input projection. New numbered bytes/source must be frozen as0.1.1;0.1.0 preserved.
