# Implementation plan

Status: technical research started at the user's request, 2026-10-23. [Persistence research](PERSISTENCE_RESEARCH.md) establishes native carriers. The accepted grave-owned appearance model removes the former need to transfer cosmetic metadata at decoration removal. No production implementation or user-operated runtime probe has been supplied.

## First technical slice

Hypothesis: stable grave state plus a narrow native rating/UI extension can represent the agreed effective changes without modifying physical bodies; two grave-owned appearance codes can drive world rendering independently of physical decoration quality.

Start with the shared evidence linked in `EVIDENCE_AND_ACCEPTANCE.md`. Verify the still-open owners rather than repeat the completed economy/player-needs research. Establish:

1. Stable grave identity and save/load lifecycle, including an empty/reused grave.
2. The narrow native effective-rating/display seam; actual body/organ data must remain untouched.
3. Prayer-station coverage geometry and reliable relevant placement/movement/removal updates, without a recurring full-world scan.
4. Grave-slot appearance persistence: set two appearances -> save/load -> remove/reinstall decorations -> move a physical decoration to a different grave. Source grave choices persist, empty slots stay empty, destination choices apply, and real item quality remains native.
5. [Native rendering owner](APPEARANCE_RENDERING_RESEARCH.md): descriptor projection is the supported research direction. Next establish the exact refresh/close/load rollback and loaded-prefab behavior before a production appearance mutation.

If the grave identity or appearance-rendering hypothesis fails, select another mechanism with explicit evidence rather than reduce the product requirement by accident. Do not design a generic persistence/UI framework before the concrete lifecycle is known.

## Subsequent slices

- Agree narrow native UI interaction and supported input devices through a focused preview iteration. Reuse existing grave presentation where appropriate; no separate rendered preview scene is required.
- Resolve numeric faith progression, wooden N, radius and appearance availability/prices after comparing representative early/mature economies. Early access plus expensive first payment is intentional.
- Implement one coherent vertical slice only after the engineering readiness gate: single grave, inspection, conversion/wreath persistence, coverage, one cosmetic commission with cancel/confirm.
- Establish actual build/version owner, verified BepInEx/runtime references and compile/handoff CI once source/toolchain exist. Keep proprietary host inputs outside public Git.
- Expand coverage to acceptance scenarios in `EVIDENCE_AND_ACCEPTANCE.md`; perform installed-runtime acceptance before stable promotion.

## Deferred concrete choices

Exact grave parameter schema/storage, stable appearance-code mapping, Harmony/native seam selection, UI tabs, camera behavior, keyboard/controller navigation, migration/uninstall behavior, mod-movement compatibility and exact body-decay interaction are not selected. Native grave Item parameters are the currently supported research direction, not a completed production save implementation. Per-item cosmetic metadata, new cosmetic item IDs and appearance transfer through removal recipes are superseded for this product.
