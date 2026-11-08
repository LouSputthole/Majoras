# Working on Majoras

Read README.md and PROJECT_PLAN.md first. The current milestone is groundwork, not a finished or playable remake.

- Unity is the production gameplay target. Keep prototype/ a thin asset/layout reviewer; do not implement quests, player physics, saves or schedules in JavaScript.
- The user selected a faithful stylized remake. N64 layout/gameplay is the working baseline; 3DS references must be labeled. Do not silently mix versions.
- Current transforms are proxy-unmeasured. Never promote them to measured/final merely because they render.
- Keep shared asset IDs stable. Update the shared contract and both consumers together when it changes.
- Never check in game dumps, extracted assets, saves, credentials, caches or build output. Record upstream sources and source commits when using their code.
- Preserve unrelated/dirty work. Each change must distinguish authored code, source inspection, automated tests, Unity execution and user visual review.
- Run npm test and npm run build in prototype/ for changes to the viewer/shared data. With the local preview server running, npm run verify exercises rendering and export. Unity changes require real editor/play-mode validation after activation; source review is not a substitute.
- Do not attempt authentication or claim a Unity build while licensing is unavailable.
- Delegation follows the user's authorization. For this planning/groundwork work the user requested GPT-6.1 Sol agents; use explicit non-overlapping file ownership and review their outputs before integration.
