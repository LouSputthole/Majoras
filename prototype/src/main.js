import "./style.css";
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GLTFExporter } from "three/addons/exporters/GLTFExporter.js";
import data from "../../shared/scenes/south-clock-town.blockout.json";
import { createBlockout } from "./scene.js";

try {
  const canvas = document.querySelector("#viewport");
  const renderer = new THREE.WebGLRenderer({
    canvas,
    antialias: true,
    powerPreference: "high-performance",
  });
  renderer.setPixelRatio(Math.min(devicePixelRatio, 2));
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;
  renderer.toneMapping = THREE.ACESFilmicToneMapping;
  renderer.toneMappingExposure = 1.15;
  const scene = new THREE.Scene();
  scene.background = new THREE.Color("#36484d");
  scene.fog = new THREE.Fog("#36484d", 90, 230);
  const camera = new THREE.PerspectiveCamera(43, 1, 0.1, 350);
  const controls = new OrbitControls(camera, canvas);
  controls.enableDamping = true;
  controls.minDistance = 4;
  controls.maxDistance = 140;
  controls.maxPolarAngle = Math.PI * 0.49;
  const blockout = createBlockout(data);
  scene.add(blockout.root, blockout.helpers);
  const hemisphere = new THREE.HemisphereLight("#c5dbe2", "#4c4438", 2);
  scene.add(hemisphere);
  const sun = new THREE.DirectionalLight("#ffe1ad", 3.1);
  sun.position.set(-28, 42, 18);
  sun.castShadow = true;
  sun.shadow.mapSize.set(2048, 2048);
  Object.assign(sun.shadow.camera, {
    left: -40,
    right: 40,
    top: 40,
    bottom: -40,
    near: 1,
    far: 120,
  });
  sun.shadow.bias = -0.0003;
  sun.shadow.normalBias = 0.05;
  scene.add(sun);
  const base = new THREE.Mesh(
    new THREE.CylinderGeometry(40, 43, 1.8, 80),
    new THREE.MeshStandardMaterial({ color: "#34413d", roughness: 1 }),
  );
  base.position.y = -1.3;
  base.receiveShadow = true;
  scene.add(base);
  const grid = new THREE.GridHelper(80, 80, "#b4b59d", "#61776f");
  grid.position.y = 0.025;
  grid.visible = false;
  scene.add(grid);
  const views = {
    overview: { pos: [60, 47, 69], target: [2, 6, 0] },
    approach: { pos: [2, 5.1, 30], target: [0, 11, -12] },
    tower: { pos: [19, 20, 13], target: [0, 15, -14] },
    plan: { pos: [0, 90, 0.01], target: [0, 0, 0] },
  };
  function setView(name) {
    const v = views[name];
    camera.position.set(...v.pos);
    controls.target.set(...v.target);
    controls.update();
    document
      .querySelectorAll("[data-view]")
      .forEach((b) => b.classList.toggle("selected", b.dataset.view === name));
  }
  function setLight(mode) {
    const p = {
      day: ["#869eaa", "#d5e8ef", "#fff0d0", 2.1, 3.4, 1.05],
      dusk: ["#36484d", "#bdc6df", "#ffc68b", 1.45, 3.2, 1.1],
      night: ["#111d32", "#738ab9", "#aec6ef", 0.7, 1.05, 1.05],
    }[mode];
    scene.background.set(p[0]);
    scene.fog.color.set(p[0]);
    hemisphere.color.set(p[1]);
    sun.color.set(p[2]);
    hemisphere.intensity = p[3];
    sun.intensity = p[4];
    renderer.toneMappingExposure = p[5];
    document
      .querySelectorAll("[data-light]")
      .forEach((b) => b.classList.toggle("selected", b.dataset.light === mode));
  }
  document
    .querySelectorAll("[data-view]")
    .forEach((b) => (b.onclick = () => setView(b.dataset.view)));
  document
    .querySelectorAll("[data-light]")
    .forEach((b) => (b.onclick = () => setLight(b.dataset.light)));
  document.querySelector("#wireframe").onchange = (e) =>
    blockout.allMaterials.forEach((m) => (m.wireframe = e.target.checked));
  document.querySelector("#markers").onchange = (e) =>
    (blockout.markerGroup.visible = e.target.checked);
  document.querySelector("#grid").onchange = (e) =>
    (grid.visible = e.target.checked);
  document.querySelector("#reset").onclick = () => setView("overview");
  const raycaster = new THREE.Raycaster(),
    pointer = new THREE.Vector2();
  let down = null;
  canvas.addEventListener(
    "pointerdown",
    (e) => (down = [e.clientX, e.clientY]),
  );
  canvas.addEventListener("pointerup", (e) => {
    if (!down || Math.hypot(e.clientX - down[0], e.clientY - down[1]) > 5)
      return;
    const r = canvas.getBoundingClientRect();
    pointer.set(
      ((e.clientX - r.left) / r.width) * 2 - 1,
      (-(e.clientY - r.top) / r.height) * 2 + 1,
    );
    raycaster.setFromCamera(pointer, camera);
    const hit = raycaster.intersectObjects(
      [blockout.root, blockout.helpers],
      true,
    )[0];
    if (!hit) return;
    let obj = hit.object;
    while (obj.parent && !obj.userData.assetId) obj = obj.parent;
    if (obj.userData.assetId)
      document.querySelector("#selection").textContent =
        `${obj.userData.label} · ${obj.userData.assetId} · ${obj.userData.provenance}`;
  });
  document.querySelector("#export").onclick = async () => {
    const button = document.querySelector("#export");
    button.disabled = true;
    button.textContent = "Exporting…";
    try {
      const content = await new GLTFExporter().parseAsync(blockout.root, {
        binary: true,
        trs: true,
        onlyVisible: true,
      });
      const url = URL.createObjectURL(
        new Blob([content], { type: "model/gltf-binary" }),
      );
      const a = document.createElement("a");
      a.href = url;
      a.download = "south-clock-town.proxy.glb";
      a.click();
      setTimeout(() => URL.revokeObjectURL(url), 2000);
    } catch (error) {
      showError(error);
    } finally {
      button.disabled = false;
      button.textContent = "Export scene .glb ↗";
    }
  };
  function resize() {
    renderer.setSize(innerWidth, innerHeight, false);
    camera.aspect = innerWidth / innerHeight;
    camera.updateProjectionMatrix();
  }
  addEventListener("resize", resize);
  resize();
  setView("overview");
  setLight("dusk");
  document.querySelector("#scene-state").textContent =
    `${data.nodes.length} proxy assemblies · no game data`;
  // Read-only diagnostics for regression verification, not a game control API.
  window.__MAJORAS__ = {
    stage: data.stage,
    nodeCount: data.nodes.length,
    scene,
    renderer,
    camera,
    getState: () => ({
      stage: data.stage,
      nodeCount: data.nodes.length,
      meshes: renderer.info.render.calls,
      triangles: renderer.info.render.triangles,
      wireframe: blockout.allMaterials[0].wireframe,
      markers: blockout.markerGroup.visible,
      grid: grid.visible,
    }),
  };
  renderer.setAnimationLoop(() => {
    controls.update();
    renderer.render(scene, camera);
  });
} catch (error) {
  showError(error);
}
function showError(error) {
  console.error(error);
  const panel = document.querySelector("#error");
  panel.hidden = false;
  panel.textContent = `The scene could not load: ${error.message}. Check WebGL support and the browser console.`;
}
