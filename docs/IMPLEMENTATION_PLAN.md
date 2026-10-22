# Implementation plan

Status: planning only. No production code or user-operated probe is authorized by the bootstrap itself.

## First technical slice

Hypothesis: stable grave state plus a narrow native rating/UI extension can represent the agreed effective changes without modifying physical bodies; decoration instance metadata can preserve cosmetic identity across native item lifecycles.

Start with the shared evidence linked in `EVIDENCE_AND_ACCEPTANCE.md`. Verify the still-open owners rather than repeat the completed economy/player-needs research. Establish:

1. Stable grave identity and save/load lifecycle, including an empty/reused grave.
2. The narrow native effective-rating/display seam; actual body/organ data must remain untouched.
3. Prayer-station coverage geometry and reliable relevant placement/movement/removal updates, without a recurring full-world scan.
4. Decoration appearance state round trip: inventory -> chest -> save/load -> install -> remove -> another grave -> save/load. Native removal recipes may lose instance metadata unless handled.
5. Native world sprite owner and safe temporary fitting/rollback; interruptions and UI ownership must be understood before preview mutation.

If either identity or round-trip hypothesis fails, select another mechanism with explicit evidence rather than reduce the product requirement by accident. Do not design a generic persistence/UI framework before the concrete lifecycle is known.

## Subsequent slices

- Agree narrow native UI interaction and supported input devices through a focused preview iteration. Reuse existing grave presentation where appropriate; no separate rendered preview scene is required.
- Resolve numeric faith progression, wooden N, radius and appearance availability/prices after comparing representative early/mature economies. Early access plus expensive first payment is intentional.
- Implement one coherent vertical slice only after the engineering readiness gate: single grave, inspection, conversion/wreath persistence, coverage, one cosmetic commission with cancel/confirm.
- Establish actual build/version owner, verified BepInEx/runtime references and compile/handoff CI once source/toolchain exist. Keep proprietary host inputs outside public Git.
- Expand coverage to acceptance scenarios in `EVIDENCE_AND_ACCEPTANCE.md`; perform installed-runtime acceptance before stable promotion.

## Deferred concrete choices

Per-item parameter versus separate item IDs, storage location/schema, Harmony/native seam selection, UI tabs, camera behavior, keyboard/controller navigation, migration/uninstall behavior, mod-movement compatibility and exact body-decay interaction are not selected. A static serializable parameter is a candidate, not accepted architecture.
