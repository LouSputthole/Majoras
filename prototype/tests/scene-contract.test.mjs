import test from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import {
  validateScene,
  toThreePosition,
  fromThreePosition,
  toThreeYaw,
} from "../../shared/scene-contract.mjs";
const scene = JSON.parse(
  readFileSync(
    new URL(
      "../../shared/scenes/south-clock-town.blockout.json",
      import.meta.url,
    ),
  ),
);
test("shared scene is well formed with unique stable asset IDs", () =>
  assert.deepEqual(validateScene(scene), []));
test("right-handed conversion is reversible and preserves meters", () => {
  const p = [8, 3, 14];
  assert.deepEqual(toThreePosition(p), [8, 3, -14]);
  assert.deepEqual(fromThreePosition(toThreePosition(p)), p);
  assert.equal(toThreeYaw(90), -Math.PI / 2);
});
test("invalid geometry and reference mistakes are rejected", () => {
  const data = structuredClone(scene);
  data.nodes[0].size[0] = -1;
  data.nodes[1].id = data.nodes[0].id;
  data.nodes[2].material = "missing";
  data.nodes[3].position[1] = NaN;
  data.nodes[4].collision = "yes";
  const errors = validateScene(data);
  for (const prefix of [
    "Invalid size",
    "Missing or duplicate ID",
    "Unknown material",
    "Invalid position",
    "Missing collision intent",
  ])
    assert.ok(
      errors.some((e) => e.startsWith(prefix)),
      prefix,
    );
});
test("south spawn is on the ground footprint and outside solid proxy envelopes", () => {
  const spawn = scene.markers.find((m) => m.kind === "spawn");
  assert.ok(spawn);
  assert.equal(spawn.position[1], 0);
  for (const n of scene.nodes.filter(
    (n) => n.collision && n.kind === "box" && n.id !== "sct.env.ground",
  )) {
    const [x, y, z] = spawn.position,
      [cx, cy, cz] = n.position,
      [w, h, d] = n.size;
    assert.ok(
      !(
        Math.abs(x - cx) < w / 2 + 0.3 &&
        Math.abs(z - cz) < d / 2 + 0.3 &&
        y + 1.6 > cy - h / 2 &&
        y < cy + h / 2
      ),
      `Spawn overlaps ${n.id}`,
    );
  }
});
test("proxy status cannot be confused with measured original data", () => {
  assert.equal(scene.stage, "proxy-unmeasured");
  assert.ok(scene.nodes.every((n) => n.provenance === "proxy-unmeasured"));
  assert.ok(scene.warning.includes("NOT extracted"));
});
