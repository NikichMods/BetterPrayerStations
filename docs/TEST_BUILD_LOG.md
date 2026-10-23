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

## Faith Probe 0.1.1 — bounded native rating/diagram/activation PASS

- Exact clean source: `7818cb977ab151fae8cdc893eb5ba94987eb914e`; frozen `probe/faith-0.1.1`.
- Numbered artifact: `BetterPrayerStationsFaithProbe-0.1.1.dll`; installed basename `BetterPrayerStationsFaithProbe.dll`.
- Artifact/installed SHA-256: `edad3c9e500a152cf31e97e4d5f73eae2e87b55a860ffd325607192fa17f8cc2` (identical); canonical Plugin.Version0.1.1, assembly/file0.1.1.0.
- Build: exact clean source, research/FaithProbe/build.ps1, SDK6.0.406 Roslyn, installed references, warnings as errors. Build manifest retained with local reference hashes.
- PASS: previous installed-IL shape/projection/native-cap/card/label/signature gates, new hidden/visible red-bar criterion tests, compiled/native160-case skull oracle and unbound pass-through. Live startup/UI not established by these checks.
- Installation2026-10-23: host closed, verified prior0.1.0 backed up outside active plugins, candidate installed at canonical basename and hash verified; one active BPS probe. Original numbered bytes and frozen refs unchanged. Private installation manifest records paths/time/hash.

Scope: only zero-red hidden-bar observer semantics and diagnostic detail changed. Zero-red must still hide the bar; positive-red still needs correct visible height. Slot/wreath counts, raw body panel, physical hash, total and seam assertions remain unchanged. No new game behavior, saved field, payment, radius or native redraw strategy.

Runtime status: requested short scenario passed with fresh log and user visual confirmation; detailed return below. The reproduced zero-red widget is hidden with height/minimum18, explaining the obsolete exact7 observer assumption. Prior0.1.0 did not record its actual dimensions; preserve that historical limitation. No full product acceptance follows.

### Installed Faith0.1.1 return — 2026-10-23

User confirms three conversions, fourth conversion, extra wreath and off/on with no automatic reset and expected visuals. Fresh READY identifies0.1.1 and expected host; installed DLL matches the frozen artifact hash above. Preserved private log SHA-256: `104be9513c25fc8d87212ef0ab2154137455d3710d37f33147ea49da8cf8c38d`. Nine REDRAW_END records all physical_unchanged=True; nine OBSERVE records pass slots_ok/wreaths_ok/red_bar_ok/raw_panel/chart/total and reach all four seams. No probe failure, skipped observation or rollback occurs. The identical physical hash on all nine actions is `f32d0f340f0d658e55e3d76529ec3d16d4b33c03ce735036410fc731a64748d6`, also matching the preceding session's baseline.

| Step | Stored C/L and activation | Native observed result |
| --- | --- | --- |
| Select | 0/0 active | rating/label3, raw3red/3white, central3red/3available,6 wreaths |
| Conversions1/2/3 | 1/0,2/0,3/0 active | ratings4/5/6; effective2/4,1/5,0/6; actual slots6 and wreaths6 throughout |
| Excess conversion | 4/0 active | rating6, effective0red/6available; no seventh white |
| Extra wreath | 4/1 active | actual wreaths7, slots6, rating6 (white cap preserved) |
| Disable | 4/1 dormant | native rating3, central3red/3available, actual wreaths6 |
| Re-enable | 4/1 active | rating6, central0red/6available, actual wreaths7 |
| Cleanup | 0/0 dormant | native rating3, central3red/3available, wreaths6; CLEANUP_DONE persistent_writes=0 |

Raw body3/3/3 and durability0.9621399, original organ contributions, recursive physical grave/item GameRes and money/faith remain unchanged under the measured hash. This is the defined selected-grave/currency snapshot, not bytewise equality of an entire save or unrelated objects. Source has no persistent writes; cleanup clears only transient state and needs no save.

Observed bar heights are46,33,20 for3/2/1 red. Atzero red: red_bar_visible=False, red_bar_height=18, red_bar_min=18. This directly confirms native minimum-height coercion in the repeated condition; the corrected semantic observer accepts the hidden widget while retaining all positive-red checks. It explains why requiring7 was invalid; the earlier artifact's unlogged actual field values are not retroactively asserted.

Accepted boundary: one healthy occupied grave, effective-only conversion/wreath inputs, native rating/central-diagram agreement, excess capacity, synthetic activation off/on, raw anatomy/item/currency preservation and cleanup, with user visual confirmation.0.1.0's earlier successful wreath-only/reset checks remain separate supporting evidence. Production payment/radius/tier/save-investment, live decay below90%, exhumation/replacement/export, other grave states, native craft/zone refresh, broad compatibility and final UI remain unaccepted. No production merge/release is authorized by this diagnostic return. Next useful investigation is grave-owned saved investment and real body removal/replacement without export, before adding spending/coverage policy.

## Faith Probe 0.2.0 — saved grave investment candidate, live return pending

- Exact clean source:6540406a0f2da0e4df57f284be18fc8f0df1d84d; frozen tag probe/faith-0.2.0.
- Artifact: artifacts/FaithProbe-0.2.0/BetterPrayerStationsFaithProbe-0.2.0.dll; canonical installed basename BetterPrayerStationsFaithProbe.dll.
- Artifact/installed SHA-256:f0d81fada80dc02a940f0f08d21c3ecc6b453ab9dac4206e73124b7df78620b2. Assembly/file version0.2.0.0.
- Build: research/FaithProbe/build.ps1, clean exact source, SDK6.0.406 Roslyn, verified installed references, warnings as errors; local build-identity.json retains reference hashes.
- PASS: existing exact-target/native-IL/card/cap/label and red-bar tests;160-case native skull oracle;2700 actual native getter IL cases with shared saved-state backend; actual SmartSerializer occupied/empty/replaced synthetic map lifecycle, non-export, current-red/decay, pruning, cleanup and malformed-state rejection.
- Installation2026-10-23: host closed, old0.1.1 hash verified and backed up outside plugins, current log preserved, new canonical DLL copied and installed hash verified. One active BPS probe. Private install.json records exact path/time/hashes. No game startup or live return for0.2.0 yet.

Scope: diagnostic schema/C/L on root grave Item, derived N=C+L, dynamic current body, single-target one-time post-start recovery, passive source/destination checkpoints and saved cleanup. Three owned root fields are excluded from immediate per-action physical hashes; recursive body/organ/decoration parameters and currency remain included. Raw body/part logs explicitly show whether diagnostic fields leaked. No faith payment/radius; session activation defaults on after load. Format/count limits are diagnostic, not settled production design.

Headless actual-getter execution uses synthetic owners/definitions and explicit test authorization; list relocation is not actual exhumation/SetObject or scene restoration. Remaining live test: C3/L1 on A -> ordinary exhume/rebury body into unmarked B -> checkpoint and load with A empty -> different body in A, checkpoint and load -> cleanup and ordinary save. Full steps: research/FaithProbe/README.md. Prior accepted0.1.1 and immutable older artifacts remain intact; no main promotion or production release.

### Installed Faith0.2.0 return — bounded saved investment PASS

User confirms native body transfer, load with original grave empty, replacement body burial and second load, with expected visuals. READY identifies0.2.0 and expected host; installed SHA-256 matches frozen f0d81fada80dc02a940f0f08d21c3ecc6b453ab9dac4206e73124b7df78620b2. Private log snapshot SHA-256:16c8f1bba3dc991f84a7dc6b7f6ef25115879cd1dfac2c55b56800f0ee24ea73.

Five REDRAW_END and five OBSERVE records pass with unchanged measured physical hash f32d0f340f0d658e55e3d76529ec3d16d4b33c03ce735036410fc731a64748d6. Initial native rating3 progresses4/5/6 for C1/2/3; L1 gives7 central wreaths while white-capped rating remains6. Raw original body3/3/3 and durability0.9621399 remain unchanged. No FAILED/ROLLBACK/SKIPPED or failed observation occurs.

Two automatic post-load checkpoints recover the same grave unique_id47567 and C3/L1/N4:

| Loaded state | Native/effective rating | Direct body evidence |
| --- | --- | --- |
|grave_empty | -2/-2 | body=none; no bonus or conjured body |
|grave_ground with replacement | -4/0 | raw4red/3white/0available, durability0.305293947; original parts logged; body_probe and every part_probe0,0,0 |

The replacement exercises live decay below90%: three effective conversions leave1red/6white and native decay permits1available white. Actual native effective rating0 is observed, not a healthy-body6 expectation. C/L and derived N survive both boundaries. Neither checkpoint is a post-replacement GUI OBSERVE: central diagram after replacement has user visual confirmation, while automated slot/bar assertions cover the initial five redraws only. No manual CHECKPOINT/other-opened record was emitted; destination grave ID/rating and the transferred original body's complete post-transfer parameters are not instrumented. Therefore live cross-grave non-export has user visual support plus prior source/offline evidence, not a captured destination checkpoint. Do not claim every old/new-body parameter or currency is invariant across ordinary native gameplay.

CLEANUP_DONE grave_params=cleared save_required=true ui_refresh=false occurs, followed by native SaveGame/ToBinary/serialization completion in the same snapshot. No post-cleanup reload is recorded. Existing host/resource/other-plugin warnings/errors are present with unassigned ownership; absence of probe failure is not a globally error-free-session claim.

Accepted boundary: stored grave investment and shared derived count through native empty/replacement state transitions and menu-load, native empty result, replacement-body/decay calculation, user-confirmed visuals and saved cleanup. No need to repeat this whole sequence. Remaining live evidence gaps: instrumented destination non-export and automated replacement GUI observation if needed by a later slice; actual payment/coverage, movement, zone/hint refresh, production schema/migration and broader compatibility remain open. No production release/main promotion follows.

## Faith Probe0.3.0 — actual prayer-station coverage candidate

- Exact clean source:ba304df6f47cbd48827ba89b224c02c722b0df26; frozen probe/faith-0.3.0.
- Artifact:artifacts/FaithProbe-0.3.0/BetterPrayerStationsFaithProbe-0.3.0.dll; installed BetterPrayerStationsFaithProbe.dll.
- Artifact/installed SHA-256:66c78d87e6ed4b9829cd650f1887a21cc874f28520ed9005e39daedfb4bdf118; assembly/file0.3.0.0.
- Build: exact clean source, SDK6.0.406 Roslyn, verified local host references, warnings as errors. Manifest retains reference hashes.
- PASS: exact six targets (including static native WorldMap.OnAddNewWGO), real native input IL rewrites/labels/card/cap preservation, red-bar observation contracts,160-case skull oracle,2700 actual getter IL cases and native binary synthetic lifecycle. New compiled coverage boundary/diagonal/ID/busy/removed/movement-out/back checks pass; no Unity registry/transform acceptance follows offline.
- Installation2026-10-23: game closed; previous0.2.0 hash verified and DLL/log retained privately outside plugins. One BPS payload found. New canonical installed file hash matches artifact. Private install.json records exact path/time/old/new hashes. No live0.3.0 return yet.

Changed activation only: registered real wood/stone station refs and current native positions; inclusive diagnostic radius3 cells/288 units. Highest tier reported; any covering station activates all saved C/L. No payment, production cap, anatomy/item edit, movement invocation or new save field. Existing load pass discovers stations, exact native registration tracks additions with world-membership check,0.25s polling inspects only cached station refs. Removed refs are pruned; busy station excluded (including removal marking), a provisional policy. COVERAGE logs raw transform coordinates, nearest distance, count/tier and retained C/L/N. Explicit native rating/diagram OBSERVE and physical hash remain the acceptance consumers. Manual toggle removed.

Requested short scenario in research/FaithProbe/README.md: select occupied grave C3/L1 -> actual station inside/outside/return with explicit grave check after each -> native demolition of last covering station -> other/new station restores -> menu-load covered state -> checkpoint -> cleanup and save. Optional existing wood/stone overlap2->1 preserves full effects; do not require unnecessary construction. Real station relocation/construction/removal costs are native, not part of probe payment invariants. Exact movement artifact is Move Objects0.6.0 c126964ca13e474770254f998fb912deca14d6c51ce933dce5ded8c7cdbcfdf0.

Status: prepared/installed only. Final radius/anchor, construction/removal eligibility, native zone/hint notification, payment/caps, free-inspection/final UI and arbitrary movement compatibility remain unaccepted. Older artifact identities/accepted bounded returns remain intact. No production release or main promotion.

### Installed Faith0.3.0 return — numeric coverage slice passes; presentation/lifecycle refinements open

Exact source ba304df6f47cbd48827ba89b224c02c722b0df26 and installed DLL SHA-25666c78d87e6ed4b9829cd650f1887a21cc874f28520ed9005e39daedfb4bdf118 match frozen candidate. Private log snapshot SHA-2568783c97d1679eea79122f0aa67076923cc7174e91e6fabe053e4eea8ab9a4bdb; three supplied screenshots retained privately alongside it. User reports completing requested sequence including native movement/building and visual checks; also identifies preview participation and stale world quality badges.

Twelve REDRAW_END and twelve OBSERVE pass, no probe FAILED/ROLLBACK/SKIPPED or failed predicate. All immediate per-action physical hashes identical:e65d6e15e78e0f1090084f4645de46b1ad6575b4a2659c012b614bc4958cef4d. Grave1060 raw body2red/2white/2available, durability1 and body/part probe fields zero. C1 produces rating3, C2/C3 rating4; L1 gives5 actual central wreaths but remains white-capped4. After setup, stored C3/L1/N4 persist through subsequent coverage records and load;23 coverage records total include the initial zero-investment selection. Under no coverage, explicit n10 rating/label2 and native central2red/2available/4wreaths; under coverage n11/n12 rating/label4 and0red/4available/5wreaths. All four input seams reached.

Actual position samples repeatedly switch at2.748737 cells (tier1, effective4) vs3.0731802 cells (tier0, effective2), consistent with diagnostic radius3. Earlier registered instances221602/221688 appear during BuildModeGUI/ShowBuildGrid events with covering count2 then1 as their coordinates change; user explicitly observes construction-preview quality feedback. Busy instance221396 at0.745356cells gives covering0/native2. Later instance221786 is recovered after menu-load at0.942809045cells with C3/L1/N4/native2/effective4. Cleanup occurs and native save follows. No wood/stone overlap or stone-tier runtime case is recorded.

Do not equate every coverage transition with a completed station move/build/demolition: logs do not label preview versus committed instances or each work-completion boundary. The user observes new construction and eventual completion; source's cached registry/current coordinates can include native registered previews. Numeric activation, preservation and loaded wooden coverage pass the bounded slice, but final preview/construction classification and world-badge refresh remain open. Overall host/resource/other-plugin errors exist with unassigned ownership; no global clean-session claim.

Screenshot1 has no selected grave, coverageTier=-1 and nearest default0/0; it exposes internal sentinel/uninitialized presentation, not a real negative station tier. Registered count2 is shown before selected-target evaluation/pruning; exact two entries were not logged before selection, so do not definitively attribute the extra to a specific preview. After selection: count1, covering1, tier1, nearest221396 at0.94279cells.

User reports world grave badge stays stale after station placement and completion until interaction focus; open-grave OBSERVE is a different consumer and does not prove world badges. Screenshot3 shows station quality badge overlapping the grave above it. No circle overlay exists in0.3.0, so its absence is expected. Newly adopted design: keep useful pre-construction preview of affected grave results; expand station placement onto grave-eligible ground while retaining ordinary footprint/occupancy constraints; put the station's quality badge near its sprite center. Exact native placement/anchor and preview/commit/cancel/world-badge integration require evidence before a new runtime candidate. No new binary is issued by this return.

## Faith Probe0.4.0 — native carried-faith purchase candidate

- Exact clean source:8990010743af2c672fab75069b33bc404db454da; frozen probe/faith-0.4.0.
- Artifact:artifacts/FaithProbe-0.4.0/BetterPrayerStationsFaithProbe-0.4.0.dll; installed canonical BetterPrayerStationsFaithProbe.dll.
- Artifact/installed SHA-256:a87ccf8bae7078dd1f296965111f73ab79cbce0a703b9d4edd153c15006c26e1; assembly/file version0.4.0.0.
- Build: clean exact source, research/FaithProbe/build.ps1, verified SDK6.0.406 Roslyn/local installed references, warnings as errors. Local build-identity.json retains reference hashes.
- PASS: all prior native projection/signature/red-bar/160-skull/2700-getter/native binary lifecycle/coverage gates; compiled payment adapter calls actual Item.RemoveItem/GetTotalCount. Real transaction backend passes simulated-debit tests with actual native counting/serializer: mixed shared price sequence, exact/bag-only/zero funds, stale count/cost, true shortage without debit, reentry, bound and seven verified rollback failure classes.
- Offline limit: debit is deliberately simulated in transaction tests; actual native debit, Unity interaction and real GameSave/player restoration are NOT accepted by this build gate. Synthetic map pair is not a real player save.
- Installation2026-10-23: host closed, old0.3.0 hash verified and payload/log backed up outside plugins, one BPS payload present, new canonical DLL installed with matching hash. Private install.json records exact path/time/hash/source. No live0.4.0 return yet.

Scope: actual carried-player/root/bag faith debit, shared C+L quote and saved investment, synchronous bounded rollback, refusal/reentry guards, post-commit native rating observation. Test quote N+1, shortage-test quote actual balance+1; no fake balance or faith grants. Free add/reset actions replaced by purchases/shortage check; cleanup remains diagnostic and does NOT refund paid faith. Conversion without remaining effective red is refused for this test. Build/floating-preview purchase is refused, while desired preview feedback is preserved. No final price, wooden cap, source-scope expansion, placement/visual/world-badge correction or new saved schema.

Requested scenario: fresh grave (prefer1060 with two raw red skulls) under existing completed station, at least6 carried faith; conversion1 -> wreath2 -> shortage refusal unchanged -> ordinary save/menu-load verifies C1/L1/N2/next3 and faith remaining -> conversion3 gives C2/L1/N3/next4 -> cleanup -> save. Full steps: research/FaithProbe/README.md. Do not repeat prior movement/exhumation/cosmetic scenarios. Normal paid commit survives later GUI failure; no re-charge/retry, backend rollback and later presentation errors are separate log phases. Real payment/player save acceptance pending; no main promotion or production release.

### Installed Faith0.4.0 return — bounded native payment/load PASS

User confirms purchases/refusal/save-load and expected visuals, varying action order and trying extra actions. READY identifies0.4.0/expected host; installed DLL matches frozen a87ccf8bae7078dd1f296965111f73ab79cbce0a703b9d4edd153c15006c26e1 at source8990010743af2c672fab75069b33bc404db454da. Private runtime-return.log SHA-256:2d5aa47c2c8ad3072be8581c938c4ed6c690f8ebd5d6a5ffb3694bb7bb1cd0ef.

| Actual event | Faith | C/L/N and next price |
| --- | --- | --- |
|Conversion, cost1 |30->29 |1/0/1, next2 |
|Conversion, cost2 |29->27 |2/0/2, next3 |
|Two shortage-test attempts, cost28 each |27->27 both |2/0/2 unchanged |
|Menu-load recovery, grave1060 |27 |2/0/2, next3 |
|Conversion attempt with no remaining effective red |no payment record; rejection only |no counter advance; later checkpoint confirms faith27/N2 |
|Wreath, cost3 after load |27->24 |2/1/3, next4 |
|Cleanup |no refund |owned improvements cleared, native rating2 |

Exactly three committed PURCHASE records, every exact faith delta equals cost and N advances by1. Total native debit6 equals original30 minus final24; no unexplained probe debit or duplicate commit appears. Both insufficient records preserve faith/actions and physical-grave/money checks. Extra conversion correctly logs PURCHASE_REJECT no-eligible-red; it is not a failed transaction. Switching to wreath after two conversions/load uses shared price3 rather than a separate first-action quote. Normal expected refusals are not circuit-breaker errors.

Eight REDRAW_END and eight OBSERVE records pass; no probe FAILED/INIT_FAILED/PURCHASE_ABORT/ROLLBACK/SKIPPED/failed predicate. All committed/refused purchase records report grave_physical_unchanged=True and money_unchanged=True. Raw body remains2red/2white/2available/durability1; body/part probe fields remain zero. Native ratings progress2->3->4, extra wreath gives central decoration5 while rating remains white-capped4; cleanup restores native2 and central2red/2available/decoration4. Native save occurs after cleanup. Existing host/other-plugin warnings/errors retain unassigned ownership; this is not a globally clean-session claim.

Accepted boundary: real installed native carried-faith debit, amount checks, shared mixed-action sequence, repeat shortage refusal, no-red rejection without charge, real player/grave recovery with correct next quote, paid action after load, native diagram/raw/physical separation and saved cleanup with no refund. Actual load was C2/L0 before the wreath purchase, not the originally requested mixed C1/L1. Thus simultaneous C+L>0 load is supported by earlier native synthetic serialization, not directly by this live return; no need to repeat the whole payment sequence. No post-cleanup reload, native live fault/reentry injection, bag-only source distribution, crash-atomicity, final balance/tier caps or broad compatibility acceptance follows. Previously passed simulated-debit rollback tests remain distinct. No new binary, production release or main promotion in this return.

## Faith Probe0.5.0 — independent two-grave/native closed-zone candidate

- Exact clean source:567be3b0629de6964308082dfed201b8fb552416; frozen probe/faith-0.5.0.
- Artifact:artifacts/FaithProbe-0.5.0/BetterPrayerStationsFaithProbe-0.5.0.dll; installed BetterPrayerStationsFaithProbe.dll.
- Artifact/installed SHA-256:6418c07547c62a2a48ccb9c74e84e65d4317e13d1e25c49f853abfbe4d920f80; assembly/file0.5.0.0.
- Build: exact clean source, SDK6.0.406 Roslyn/verified installed references, warnings as errors. Build manifest retains local reference hashes.
- PASS: all prior native IL/card/skull/red-bar/payment/serializer/coverage gates. New compiled observer has exactly one real native GetTotalQuality call plus GetZoneWGOs. Actual two-owner set identity/order/limit/duplicate tests pass; headless native getter projection applies independently to both owners and selective dormant inputs; both-owner native serialization includes mixed C/L. Actual native WorldZone Sum/ignored-member/callback executes on raw synthetic native fixtures. Simulated-debit/native-count test A1/A2/B1 preserves independent quotes and pair reload.
- Limits: native-zone fixture is raw, two-owner projection uses explicit headless authorization; combined live Harmony/Unity/two-owner/native-zone/closed-window behavior remains pending. Native0.4.0 payment acceptance is reused, not reproved by simulated debit tests.
- Installation2026-10-23: host closed; prior0.4.0 hash verified, DLL/log retained privately outside plugins; one active BPS DLL; new canonical installed hash matches artifact. Private install.json records path/time/identities. No live0.5.0 return yet.

Changed boundary: up to two saved marked graves, quality authorization by membership/current grave argument rather than editing selection. C/L/N/test prices remain per-grave; no new save keys or external ledger. Other-owner state/physical snapshot must remain unchanged on purchase. Coverage transitions and cached diagnostic summary are per-owner; native getter/UI read current coverage without requiring an open window. Both marks recover in the existing one-time loaded-world pass; unknown/third mark rejects. Cleanup clears both, no faith refund, followed by save.

Closed-zone button requests one sample after native massive windows/floating preview close; two-owner load also requests one such sample automatically. Native cached WGO._zone and public GetZoneWGOs establish same Sum-zone membership. Baseline sums only native individual leaf getters with inputs disabled, then inputs restore and real GetTotalQuality runs once. A temporary native-event listener is attached only for this call/detached in finally. Both owner input hits, actual total vs baseline+two deltas, native event value and physical/money/faith invariance are asserted. No false-baseline zone call, manually emitted event, replacement total/ledger, world-part redraw or recurring zone scan. Ordinary native quality mirrors may update from the real call.

Requested bounded scenario: mark two healthy occupied graves in one native zone, both covered; A conversion1+wreath2 givesC1/L1/N2/next3, B conversion1 givesC1/L0/N1/next2; cost4 total. Closed native-zone sample -> relocate station so only one loses coverage -> closed sample -> restore coverage -> closed sample -> save/menu-load both -> automatic closed sample plus each real GUI check -> clear both/save. Steps: research/FaithProbe/README.md; mechanism/limits: docs/TWO_GRAVE_ZONE_RESEARCH.md. Final radius/caps, visuals/placement/badge refresh, production saved format and broader lifecycle compatibility remain open. No production/main promotion.

## Corrective0.5.1 — explicit saved roles and actual open context

The incomplete0.5.0 return contains only SELECT1060, conversion cost1 and wreath cost2 (faith24->21,N2,next3), three passing GUI observations and context-or-coverage rejection. No SELECT B, closed ZONE, load or post-purchase save. User exited without saving. Private runtime-incomplete.log SHA256 fa56282014a4c10cad53302aa4c9b9b5288e79cf8f7dc485698d1c3ee6b71831. No evidence of cross-grave price leakage; console retained selected A after another native grave opened, and guard prevented payment. Exact reason B was never registered is not logged.

0.5.1 adds explicit assign-opened-as-A/B buttons and optional grave-owned diagnostic slot, persistent across native serialization. Occupied role/two roles for one grave reject without mutation; legacy unlabelled marks need explicit assignment. Native Open uses actual owner, refreshes quote and purchase enablement, logs OPEN_CONTEXT, cancels pending old-owner work. Unregistered/unlabelled opened grave cannot buy. Role is cleared with improvements, never travels with body. No payment backend or zone formula change; no production visual/balance scope.

Offline role tests pass including ID-order independence, native map role/count reload, refusal invariance, duplicate/invalid roles and cleanup. Unity UI/open/load and simultaneous zone integration remain unaccepted. Repeat the README two-role setup and independent prices first, then previously unperformed closed-zone/move/load steps. No save-file editing or faith refund.
### Frozen Faith0.5.1 handoff

Exact clean source24d24b4c18b6863da5562056d5c65266b49a3612; tag probe/faith-0.5.1. Artifact artifacts/FaithProbe-0.5.1/BetterPrayerStationsFaithProbe-0.5.1.dll; SHA256 d5168025c6acb2e07908a64929a0223cd4a89806e0c859f2f33b8bc98037716b; assembly/file0.5.1.0. SDK6.0.406 clean-source build passes all prior gates and explicit saved-role tests. Installed2026-10-23 with host closed: previous0.5.0 hash verified and backed up outside plugins, canonical BetterPrayerStationsFaithProbe.dll matches new artifact hash. Private install.json preserves exact paths/time/previous identity. One active BPS payload. No installed0.5.1 behavior acceptance yet; scenario research/FaithProbe/README.md.
## Installed Faith0.5.1 return — bounded two-owner/closed-zone PASS, preview-count defect open

User confirms independent A/B prices, zone/movement and visual behavior, and reports having checked loading. Later adds transient displayed station count up to eight while only one station exists. This return is therefore not an unconditional coverage/UI acceptance.

Identity: source24d24b4c18b6863da5562056d5c65266b49a3612; installed DLL matches frozen d5168025c6acb2e07908a64929a0223cd4a89806e0c859f2f33b8bc98037716b; READY0.5.1 expected GK1.407 host. Private runtime-return.log SHA256 a4a8df5cb9559751989ea5e5606102a11387c4db7f5d48109f1010915e83d959.

A1060/slot1 starts C0/L0, buys conversion1 (faith24->23), wreath2 (23->21), ends C1/L1/N2/next3. B1061/slot2 starts C0/L0, buys conversion1 (21->20), ends C1/L0/N1/next2. All three committed purchases report exact debit, unchanged physical/money/other-owner state. OPEN_CONTEXT switches repeatedly to actual A/B with correct quote; two SELECT identities and seven native GUI/diagram observations pass. Raw anatomy stays A2red/2white and B2red/4white, both durability1; no body/organ diagnostic fields. No FAILED/ABORT/ROLLBACK/SKIPPED or failed assertion in probe records.

Three real closed-window ZONE samples in graveyard have baseline144: actual146/delta2, then145/delta1, then146/delta2. Each logs both_inputs=True, total_ok=True, event_ok=True, one native event carrying actual result, physical_money_faith_unchanged=True. Both owners contribute without open grave windows regardless of selected editor. Selective loss sample is B moved to2176,-1120 at3.34995842cells from station221786 at2208,-1440: B native/effective4/4 while A stays2/3. B return to2176,-1184 at2.68741918cells restores effective5. Therefore this establishes selective coverage after actual grave relocation, not solely station relocation. Station221786 also changes coordinates earlier in the session; preview transitions below prevent blanket move-mod acceptance.

Station-count defect: user saw up to8; sampled COVERAGE records reach registered2/covering2, then return1. Different station IDs224608/224692/224748/224804/224902/225068 appear temporarily alongside persistent221786, often at the same coordinate. Thus duplicate temporary station candidates are actually fed to coverage, not only text. Candidate224804 alone covers B at2.68741918 while persistent221786 is3.018462cells away; preview affects activation, as intentionally desired by product. Multiple candidates do not multiply C/L: Evaluate uses boolean coverage/highest tier, not count-based bonus. But retaining old position plus proposed position can yield incorrect move preview; classification/cleanup and logical one-source replacement are not accepted. Count directly returns backing-list length, while invalid/dead entries are only pruned in Evaluate; displayed Count is not a reliable count of completed stations and can temporarily include stale references. Exact source/state of displayed8 was not logged and remains unproven; no claim of eight permanent stations or fixed defect.

Loading boundary: this snapshot contains only initial RECOVER target=none, no post-purchase RECOVER/RESET/Capture(load), and no restored pair/roles. Cleanup clears1060/slot1 and1061/slot2; then native GameSave.ToBinary/save and ReturnToMainMenu. User visual load report is preserved separately, but pair mixed-state native-load/automatic-zone evidence is absent here; no captured post-cleanup reload. Do not repeat paid/zone tests wholesale. Existing0.2.0 live carrier and0.4.0 payer recovery remain accepted, A/B-role native serialization offline only.

Accepted narrow result: actual A/B addressing/independent native prices, unchanged other owner, simultaneous real closed-window zone sum/event, selective dormant/reactivated grave projection and native GUI consistency; cleanup followed by native save. Still open: reliable committed/preview station count and move-preview replacement/cancel, paired mixed-state load/roles in live log, tier caps/overlap, full construction lifecycle, production schema, visuals/balance and broader compatibility. Next investigation should classify registered station candidates against verified native current-floater/move ownership and capture start/commit/cancel with one actual station; preserve desired preview feedback. No new binary, stable merge or release is implied by this return.