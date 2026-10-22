# Better Prayer Stations — Appearance Probe

Research-only candidate. This is not the production mod or a release. Only one opted-in grave's **gravestone** and two existing looks are tested; fence customization, faith, fees, radius and final NGUI design are outside this slice.

## Preparation and identity

Canonical version owner: `Plugin.cs` constant `Version`, from which assembly/file versions and numbered output are derived/checked. Installed basename: `BetterPrayerStationsAppearanceProbe.dll`; numbered handoff: `BetterPrayerStationsAppearanceProbe-<version>.dll`. Exact source/artifact identity and installed status belong to `docs/TEST_BUILD_LOG.md`.

Build using `build.ps1 -GameRoot <local game root> -DotnetRoot <local .NET root> -ExpectedSource <exact SHA>`. Verified compiler: .NET SDK 6.0.406 Roslyn on Windows, targeting locally installed GK 1.407/BepInEx/Harmony/Unity references. No proprietary DLLs are copied to Git or included in the handoff. `-CheckOnly` permits development edits and produces an explicitly non-handoff output. Numbered builds require a clean exact checkout and refuse to overwrite an existing numbered artifact.

The local build gate compiles with warnings as errors, asserts assembly/file version identity and runs descriptor tests against the real native WOPPrefabName name/cache methods. Tests prove helper-level preservation of damage/herb/suffix, unrelated fence, construction and empty slots, not native loaded rendering or GUI lifecycle. `build-identity.json` records source, artifact hash, compiler and local reference hashes.

The loader checks the exact inspected Assembly-CSharp SHA-256 before installing its five narrow patches; failures disable the probe. Runtime known resources are checked before choosing a look. No global patch removal, full-world per-frame scans, per-frame redraws or recurring trace output.

## Gameplay test

1. Start the game and load a save. Open a normal occupied grave with a fully installed gravestone and no active craft. The research panel is initially at the left; it can be dragged or hidden/shown with **F8**.
2. Click **Выбрать открытую могилу**, then **Сохранить вид: деревянный крест** (use the stone button as well if the original was already wooden). Look at the actual grave.
3. Click **Проверить перерисовку**. The chosen look should remain. The ordinary game log records actual instantiated WOP names and before/after quality/items/body/currency snapshots.
4. Save through the game's ordinary sleep/save flow, return to menu and reload. The marked grave is recovered once after native restoration, its saved test look is redrawn, and the panel reports restoration. Confirm the grave still looks correct.
5. Open that grave again. Click **Примерить другой вид до закрытия окна**. Close the **native grave window** normally (not merely F8). The temporary look should revert to the committed one on the next frame. The journal must show `CANCEL reason=grave-close` and the subsequent redraw. A separate button also permits explicit cancellation.
6. Click **Очистить тест и вернуть исходный вид**, then save normally once more. This disables both test fields and returns native appearance. Test cleanup is not a removal/refund of any real decoration.

The user supplies only these gameplay/visual observations; accessible logs and installed DLL identity are collected by the agent. Do not ask the user to copy DLLs or manually interpret invariant traces when local tools can do so.

## Save state and failure behavior

Choosing the test grave and committing a look writes two namespaced **save-persistent diagnostic parameters** to that grave. It never writes body, decoration ID/durability/quality, currency or normal mod settings. No existing production save format is being promised. Fitting is in-memory only; loading invalidates old references before scene restoration and rebinds by the saved marker, never by coordinates alone.

Cleanup writes zero to only the two diagnostic fields. They remain inert if present, without trimming unrelated native zero parameters. Save after cleanup to preserve it. Without cleanup the test choice remains in the save, but vanilla ignores it when this diagnostic DLL is absent; normal appearance returns after restarting without the DLL. No paid improvements or item-owned cosmetics are involved.

Actions are queued out of panel drawing onto the next Update and perform one bounded real redraw. Each logs `REDRAW_BEGIN`/`REDRAW_END`, actual part names and unchanged-state checks. Closing clears temporary state immediately and queues one restore redraw. Native WGO.Redraw is deliberately the refresh boundary under investigation; this is not yet the optimized production renderer.

Unexpected failure disables projection and attempts a native-appearance redraw, with a bounded failure/rollback trace. Cleanup remains available. Multiple marked graves fail closed rather than choosing one silently. Native loaded behavior and visual acceptance remain pending until this exact candidate is exercised; do not mark a compiled probe accepted.
