// Unity-style +Z north reflects once into right-handed Three.js/glTF.
export const toThreePosition = ([x, y, z]) => [x, y, -z];
export const fromThreePosition = toThreePosition;
export const toThreeYaw = (degrees) => (-degrees * Math.PI) / 180;
export function validateScene(scene) {
  const errors = [];
  const finite3 = (v) =>
    Array.isArray(v) && v.length === 3 && v.every(Number.isFinite);
  if (scene.schemaVersion !== 1) errors.push("Unsupported schemaVersion");
  if (scene.units !== "meters") errors.push("Scene units must be meters");
  if (
    !Array.isArray(scene.nodes) ||
    !Array.isArray(scene.markers) ||
    !Array.isArray(scene.materials)
  )
    return [...errors, "Missing scene arrays"];
  const materials = new Set(scene.materials.map((m) => m.id)),
    ids = new Set(),
    kinds = new Set(["box", "cylinder", "clockFace", "shelter", "bunting"]);
  for (const n of [...scene.nodes, ...scene.markers]) {
    if (!n.id || ids.has(n.id)) errors.push(`Missing or duplicate ID: ${n.id}`);
    ids.add(n.id);
    if (!finite3(n.position)) errors.push(`Invalid position: ${n.id}`);
  }
  for (const n of scene.nodes) {
    if (!finite3(n.size) || n.size.some((v) => v <= 0))
      errors.push(`Invalid size: ${n.id}`);
    if (!Number.isFinite(n.yawDegrees)) errors.push(`Invalid yaw: ${n.id}`);
    if (!kinds.has(n.kind)) errors.push(`Unsupported kind: ${n.id}`);
    if (!materials.has(n.material)) errors.push(`Unknown material: ${n.id}`);
    if (typeof n.collision !== "boolean")
      errors.push(`Missing collision intent: ${n.id}`);
    if (!n.provenance) errors.push(`Missing provenance: ${n.id}`);
  }
  return errors;
}
