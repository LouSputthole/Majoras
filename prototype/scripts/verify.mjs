import { chromium } from "@playwright/test";
import { mkdir, readFile, writeFile } from "node:fs/promises";
import assert from "node:assert/strict";
import path from "node:path";
import { fileURLToPath } from "node:url";
const root = path.resolve(
  path.dirname(fileURLToPath(import.meta.url)),
  "../..",
);
const output = path.resolve(root, process.env.VERIFY_OUTPUT || "evidence");
await mkdir(output, { recursive: true });
await writeFile(
  path.join(output, "verification.json"),
  JSON.stringify(
    { status: "started", timestamp: new Date().toISOString() },
    null,
    2,
  ) + "\n",
);
const browser = await chromium.launch(
  process.env.CI ? { headless: true } : { channel: "chrome", headless: true },
);
const page = await browser.newPage({
  viewport: { width: 1440, height: 960 },
  deviceScaleFactor: 1,
});
const errors = [];
page.on("pageerror", (e) => errors.push(e.message));
page.on("console", (msg) => {
  if (msg.type() === "error") errors.push(msg.text());
});
try {
  await page.goto(process.env.PREVIEW_URL || "http://127.0.0.1:4173", {
    waitUntil: "networkidle",
  });
  await page.waitForFunction(
    () => window.__MAJORAS__?.getState().triangles > 0,
  );
  assert.equal(await page.locator("#error").isVisible(), false);
  assert.match(
    await page.locator(".notice").innerText(),
    /Unmeasured blockout/,
  );
  const initial = await page.evaluate(() => window.__MAJORAS__.getState());
  assert.equal(initial.nodeCount, 17);
  await page.screenshot({
    path: path.join(output, "south-clock-town-overview.png"),
  });
  for (const view of ["approach", "tower", "plan", "overview"]) {
    await page.locator(`[data-view="${view}"]`).click();
    assert.equal(
      await page.locator(`[data-view="${view}"]`).getAttribute("class"),
      "selected",
    );
  }
  for (const light of ["day", "night", "dusk"])
    await page.locator(`[data-light="${light}"]`).click();
  await page.locator("#wireframe").check();
  assert.equal(
    await page.evaluate(() => window.__MAJORAS__.getState().wireframe),
    true,
  );
  await page.locator("#wireframe").uncheck();
  await page.locator("#markers").uncheck();
  assert.equal(
    await page.evaluate(() => window.__MAJORAS__.getState().markers),
    false,
  );
  await page.locator("#markers").check();
  await page.locator("#grid").check();
  assert.equal(
    await page.evaluate(() => window.__MAJORAS__.getState().grid),
    true,
  );
  await page.locator("#grid").uncheck();
  const downloadPromise = page.waitForEvent("download");
  await page.locator("#export").click();
  const download = await downloadPromise;
  const glbPath = path.join(output, download.suggestedFilename());
  await download.saveAs(glbPath);
  const bytes = await readFile(glbPath);
  assert.equal(bytes.readUInt32LE(0), 0x46546c67);
  assert.equal(bytes.readUInt32LE(4), 2);
  assert.equal(bytes.readUInt32LE(8), bytes.length);
  const gltf = JSON.parse(
    bytes.subarray(20, 20 + bytes.readUInt32LE(12)).toString(),
  );
  assert.ok(gltf.nodes.some((n) => n.name === "sct.env.clock_tower"));
  assert.ok(!gltf.nodes.some((n) => n.name?.includes("Inspection helpers")));
  const tower = gltf.nodes.find((n) => n.name === "sct.env.clock_tower");
  assert.deepEqual(tower.translation, [0, 5, -15]);
  await page.setViewportSize({ width: 700, height: 850 });
  await page.screenshot({
    path: path.join(output, "south-clock-town-compact.png"),
  });
  assert.equal(await page.locator("#export").isVisible(), true);
  assert.deepEqual(errors, []);
  const result = {
    status: "passed",
    timestamp: new Date().toISOString(),
    sourceRevision: process.env.GITHUB_SHA || null,
    platform: process.platform,
    nodeVersion: process.version,
    browserVersion: await browser.version(),
    checks: [
      "WebGL render",
      "view presets",
      "lighting controls",
      "wireframe",
      "markers",
      "meter grid",
      "GLB binary and hierarchy",
      "Z reflection in exported tower",
      "compact layout",
    ],
    scene: initial,
    glbBytes: bytes.length,
    consoleErrors: errors,
    limitations: [
      "Unity not executed",
      "No original gameplay",
      "Proxy layout unmeasured",
      "No finished Link model",
      "Not a performance benchmark",
    ],
  };
  await writeFile(
    path.join(output, "verification.json"),
    JSON.stringify(result, null, 2) + "\n",
  );
  console.log(JSON.stringify(result, null, 2));
} catch (error) {
  await writeFile(
    path.join(output, "verification.json"),
    JSON.stringify(
      {
        status: "failed",
        timestamp: new Date().toISOString(),
        error: error.message,
        consoleErrors: errors,
      },
      null,
      2,
    ) + "\n",
  );
  throw error;
} finally {
  await browser.close();
}
