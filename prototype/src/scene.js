import * as THREE from "three";
import {
  toThreePosition,
  toThreeYaw,
  validateScene,
} from "../../shared/scene-contract.mjs";

// Visualization only: no gameplay, final art, or extracted game assets.
export function createBlockout(data) {
  const errors = validateScene(data);
  if (errors.length) throw new Error(errors.join("\n"));
  const root = new THREE.Group();
  root.name = data.id;
  root.userData = {
    stage: data.stage,
    units: data.units,
    source: "shared/scenes/south-clock-town.blockout.json",
    coordinates: "glTF right-handed Y up; authoring Z reflected once",
  };
  const mats = Object.fromEntries(
    data.materials.map((m) => [
      m.id,
      new THREE.MeshStandardMaterial({
        color: m.color,
        roughness: m.roughness,
      }),
    ]),
  );
  const allMaterials = Object.values(mats);
  function mesh(geometry, material, parent, pos = [0, 0, 0]) {
    const m = new THREE.Mesh(geometry, material);
    m.position.set(...pos);
    m.castShadow = true;
    m.receiveShadow = true;
    parent.add(m);
    return m;
  }
  const box = (size, mat, parent, pos) =>
    mesh(new THREE.BoxGeometry(...size), mat, parent, pos);
  for (const n of data.nodes) {
    const group = new THREE.Group();
    group.name = n.id;
    group.position.set(...toThreePosition(n.position));
    group.rotation.y = toThreeYaw(n.yawDegrees);
    group.userData = {
      assetId: n.id,
      label: n.name,
      collisionIntent: n.collision,
      provenance: n.provenance,
    };
    root.add(group);
    const [w, h, d] = n.size,
      mat = mats[n.material];
    if (n.kind === "box") {
      box(n.size, mat, group);
      if (n.id.includes("perimeter"))
        box([w + 0.04, 0.22, d + 0.04], mats.trim, group, [0, h / 2 - 0.65, 0]);
      if (n.id === "sct.prop.construction_platform") {
        const height = n.position[1] - h / 2;
        for (const x of [-w * 0.43, w * 0.43])
          for (const z of [-d * 0.43, d * 0.43])
            box([0.26, height, 0.26], mats.wood, group, [
              x,
              -h / 2 - height / 2,
              z,
            ]);
      }
    } else if (n.kind === "cylinder")
      mesh(new THREE.CylinderGeometry(w / 2, w / 2, h, 8), mat, group);
    else if (n.kind === "clockFace") {
      const disk = mesh(
        new THREE.CylinderGeometry(w / 2, w / 2, d, 12),
        mat,
        group,
      );
      disk.rotation.x = Math.PI / 2;
      const inner = mesh(
        new THREE.CylinderGeometry(w * 0.435, w * 0.435, d + 0.03, 64),
        mats.clock,
        group,
      );
      inner.rotation.x = Math.PI / 2;
      for (let i = 0; i < 12; i++) {
        const a = (i / 12) * Math.PI * 2;
        const mark = box([0.16, 0.6, 0.1], mats.brass, group, [
          Math.sin(a) * w * 0.38,
          Math.cos(a) * w * 0.38,
          d / 2 + 0.05,
        ]);
        mark.rotation.z = -a;
      }
      box([0.16, w * 0.3, 0.12], mats.brass, group, [
        0,
        w * 0.15,
        d / 2 + 0.13,
      ]);
      box([w * 0.23, 0.16, 0.14], mats.brass, group, [
        w * 0.115,
        0,
        d / 2 + 0.16,
      ]);
    } else if (n.kind === "shelter") {
      for (const x of [-w * 0.4, w * 0.4])
        for (const z of [-d * 0.4, d * 0.4])
          box([0.23, h - 0.7, 0.23], mats.wood, group, [x, (h - 0.7) / 2, z]);
      const roof = mesh(new THREE.ConeGeometry(1, 1, 4), mat, group, [
        0,
        h - 0.55,
        0,
      ]);
      roof.rotation.y = Math.PI / 4;
      roof.scale.set(w * 0.79, 1.7, d * 0.79);
    } else if (n.kind === "bunting") {
      const line = new THREE.CatmullRomCurve3([
        new THREE.Vector3(-w / 2, 0, 0),
        new THREE.Vector3(0, -0.65, 0),
        new THREE.Vector3(w / 2, 0, 0),
      ]);
      mesh(new THREE.TubeGeometry(line, 32, 0.025, 5, false), mats.wood, group);
      for (let i = 0; i < 18; i++) {
        const p = line.getPoint((i + 0.5) / 18);
        box(
          [0.68, 0.9, 0.035],
          i % 3 === 0 ? mats.trim : i % 3 === 1 ? mat : mats.thatch,
          group,
          [p.x, p.y - 0.48, p.z],
        );
      }
    }
  }
  const helpers = new THREE.Group();
  helpers.name = "Inspection helpers (excluded from export)";
  const humanMat = new THREE.MeshStandardMaterial({
    color: 0xd8cba8,
    roughness: 0.9,
  });
  allMaterials.push(humanMat);
  const figure = new THREE.Group();
  figure.name = "1.6 m scale mannequin — not Link";
  figure.position.set(
    ...toThreePosition(data.markers.find((m) => m.kind === "spawn").position),
  );
  mesh(
    new THREE.CapsuleGeometry(0.22, 0.93, 4, 8),
    humanMat,
    figure,
    [0, 0.75, 0],
  );
  mesh(new THREE.SphereGeometry(0.14, 12, 8), humanMat, figure, [0, 1.46, 0]);
  helpers.add(figure);
  const markerGroup = new THREE.Group();
  helpers.add(markerGroup);
  for (const m of data.markers) {
    const dot = mesh(
      new THREE.CylinderGeometry(0.6, 0.6, 0.06, 24),
      m.kind === "spawn" ? mats.brass : mats.trim,
      markerGroup,
      toThreePosition([m.position[0], 0.05, m.position[2]]),
    );
    dot.name = m.id;
    dot.userData = {
      assetId: m.id,
      label: m.label,
      provenance: "proxy-unmeasured",
    };
  }
  return { root, helpers, markerGroup, allMaterials };
}
