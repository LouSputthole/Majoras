using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Majoras.Editor
{
    /// <summary>
    /// Additive authored details measured against the local US-N64 reference.json.
    /// No original meshes, materials, transforms, collision or scene settings are modified.
    /// Coordinates already use Unity X=N64 X, Y=N64 Y, Z=-N64 Z, divided by 100.
    /// The caller owns scene/prefab saving and AssetDatabase.SaveAssets after Build.
    /// </summary>
    public static class CityArchitecture
    {
        public const string DefaultAssetFolder = "Assets/LocalReference/SouthClockTown/Enhanced/Geometry";

        public const string ConstructionNotes =
            "Authored additive refinement, not original game geometry. Source clock perimeter is twelve-sided " +
            "(radius 2.4), not octagonal. Two narrow bevel beads retain that contour and leave the original " +
            "painted ring and face visible. Entrance coping retains the source six-point ledge contour. " +
            "New chamfered jamb courses, brass fittings, two warm entrance lamps, five dimensional stall " +
            "posts and under-roof bracing are deliberate modern additions. Source geometry is retained; " +
            "no collision is added. The movable counterweight and original cloth remain source assets.";

        /// <summary>
        /// Creates one separate local-identity root and persists its batched meshes in the ignored folder.
        /// stone/wood/metal/trim remain caller-owned; trim is the restrained teal accent material.
        /// The existing root must be removed by the caller before rebuilding a scene to avoid duplicates.
        /// </summary>
        public static GameObject Build(Transform parent, Material stone, Material wood,
            Material metal, Material trim, string assetFolder = DefaultAssetFolder)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (stone == null || wood == null || metal == null || trim == null)
                throw new ArgumentException("CityArchitecture requires all four authored materials.");
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
                throw new InvalidOperationException("Build architectural details outside Play Mode after compilation.");
            assetFolder = ValidateFolder(assetFolder);
            EnsureFolder(assetFolder);

            var root = new GameObject("South Clock Town - authored architectural refinement");
            root.transform.SetParent(parent, false);
            var clock = new Group("Clock perimeter - source twelve-sided contour", root.transform, assetFolder);
            var portal = new Group("Clock entrance - supported stone trim", root.transform, assetFolder);
            var stalls = new Group("Market stalls - measured supports and joinery", root.transform, assetFolder);
            var lamps = new Group("Clock entrance - mounted lanterns", root.transform, assetFolder);

            BuildClock(clock, metal, trim);
            BuildEntrance(portal, stone, metal, trim);
            BuildStalls(stalls, stone, wood, metal);
            var amber = LanternMaterial(assetFolder, metal);
            BuildLantern(lamps, new Vector3(-4.27f, 1.82f, 7.87f), metal, amber, "West");
            BuildLantern(lamps, new Vector3(-1.33f, 1.82f, 7.87f), metal, amber, "East");

            clock.Persist("clock");
            portal.Persist("entrance");
            stalls.Persist("stalls");
            lamps.Persist("lanterns");
            return root;
        }

        static void BuildClock(Group group, Material brass, Material teal)
        {
            // Measured source bounds: center (-2.8,5.55), outer R2.4, inner R1.89;
            // painted face R1.8; original rim nearest Z7.907. At noon the ring is unrotated.
            var center = new Vector3(-2.8f, 5.55f, 0);
            AddPolygonBead(group.Surface(brass), center, 2.401f, .057f, 7.872f, 7.966f, .014f);
            // The source annulus slopes back to Z8.162 at its inner radius. This bead must
            // follow that recessed edge, rather than float at the outer edge's Z7.907 plane.
            AddPolygonBead(group.Surface(brass), center, 1.906f, .032f, 8.115f, 8.190f, .008f);

            // Twelve very small joint pins sit on the outside edge, never over the face/motifs.
            for (int side = 0; side < 12; side++)
            {
                float angle = side * Mathf.PI / 6;
                var direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                var position = center + direction * 2.403f + Vector3.forward * 7.855f;
                AddTurnedPin(group.Surface(brass), position, .028f, .032f);
                // One small inset per quarter turn; no replacement graphic over the source design.
                if (side % 3 == 0)
                    AddTurnedPin(group.Surface(teal), position - Vector3.forward * .020f, .012f, .008f);
            }
        }

        static void BuildEntrance(Group group, Material stone, Material brass, Material teal)
        {
            // Original jambs occupy X[-4,-3.6] / [-2,-1.6], Y[0,1.4]; leaves stay unobstructed.
            foreach (float x in new[] { -3.8f, -1.8f })
            {
                AddBeam(group.Surface(stone), new Vector3(x, -.01f, 7.43f),
                    new Vector3(x, .15f, 7.43f), .42f, .34f, .025f);
                for (int course = 0; course < 4; course++)
                {
                    float low = .16f + course * .315f;
                    AddBeam(group.Surface(stone), new Vector3(x, low, 7.435f),
                        new Vector3(x, low + .304f, 7.435f), .314f, .242f, .016f);
                }
                AddBeam(group.Surface(stone), new Vector3(x - .22f, 1.43f, 7.40f),
                    new Vector3(x + .22f, 1.43f, 7.40f), .34f, .13f, .022f);
            }

            AddBeam(group.Surface(stone), new Vector3(-4.025f, 1.825f, 7.42f),
                new Vector3(-1.575f, 1.825f, 7.42f), .41f, .13f, .023f);
            // Source fascia front Z7.28; the 3.5cm strip reaches back to Z7.29 (1cm embed).
            AddBeam(group.Surface(brass), new Vector3(-3.87f, 1.672f, 7.2725f),
                new Vector3(-1.73f, 1.672f, 7.2725f), .035f, .032f, .008f);
            foreach (float x in new[] { -3.82f, -1.78f })
            {
                AddBeam(group.Surface(teal), new Vector3(x, 1.522f, 7.265f),
                    new Vector3(x, 1.725f, 7.265f), .043f, .036f, .009f);
            }

            // Exact source ledge XZ outline from room 0073E8, slot12; adds a 9cm coping only.
            var ledge = new[] {
                new Vector2(-3.6f,7.28f), new Vector2(-3.2f,6.8f),
                new Vector2(-2.4f,6.8f), new Vector2(-2f,7.28f),
                new Vector2(-2f,8.26f), new Vector2(-3.6f,8.26f) };
            AddCoping(group.Surface(stone), ledge, 2.395f, .09f, .018f);
        }

        static void BuildStalls(Group group, Material stone, Material wood, Material metal)
        {
            // Original room 00D990 slots29-33 are flat post faces. New bevelled sections enclose
            // those same faces with depth; old faces remain inside, so they are not coplanar.
            var posts = new[] {
                new Vector3(-6.27f,0,2.09f), new Vector3(-4.44f,0,3.15f),
                new Vector3(-4.43f,0,.86f), new Vector3(-1.43f,0,3.08f),
                new Vector3(-1.43f,0,.81f) };
            foreach (var post in posts)
            {
                AddBeam(group.Surface(wood), post + Vector3.up * .045f,
                    post + Vector3.up * 1.365f, .145f, .145f, .013f);
                AddBeam(group.Surface(stone), post - Vector3.up * .015f,
                    post + Vector3.up * .075f, .215f, .215f, .016f);
                AddBeam(group.Surface(metal), post + Vector3.up * .087f,
                    post + Vector3.up * .162f, .163f, .163f, .008f);
                AddBeam(group.Surface(metal), post + Vector3.up * 1.229f,
                    post + Vector3.up * 1.284f, .157f, .157f, .007f);
            }

            // Source left canopy ridge (-5.04,1.8,2.04) and eaves, room slot25.
            var leftRidge = new Vector3(-5.04f, 1.744f, 2.04f);
            AddBeam(group.Surface(wood), new Vector3(-4.44f,1.325f,3.15f),
                new Vector3(-4.43f,1.325f,.86f), .095f, .105f, .012f);
            AddBeam(group.Surface(wood), new Vector3(-6.27f,1.32f,2.09f), leftRidge, .085f,.09f,.011f);
            AddBeam(group.Surface(wood), new Vector3(-4.44f,1.35f,3.15f), leftRidge, .075f,.08f,.01f);
            AddBeam(group.Surface(wood), new Vector3(-4.43f,1.35f,.86f), leftRidge, .075f,.08f,.01f);
            AddKnees(group.Surface(wood), new Vector3(-4.44f,0,3.15f), Vector3.back);
            AddKnees(group.Surface(wood), new Vector3(-4.43f,0,.86f), Vector3.forward);

            // Right canopy, room slot23: front is lower, rear joins the existing X0 wall.
            AddBeam(group.Surface(wood), new Vector3(-1.43f,1.325f,3.08f),
                new Vector3(-1.43f,1.325f,.81f), .095f,.105f,.012f);
            foreach (float z in new[] { .81f, 3.08f })
                AddBeam(group.Surface(wood), new Vector3(-1.43f,1.35f,z),
                    new Vector3(-.015f,1.755f,z), .075f,.08f,.01f);
            AddKnees(group.Surface(wood), new Vector3(-1.43f,0,3.08f), Vector3.back);
            AddKnees(group.Surface(wood), new Vector3(-1.43f,0,.81f), Vector3.forward);
        }

        static void AddKnees(Draft draft, Vector3 post, Vector3 alongBeam)
        {
            AddBeam(draft, post + Vector3.up * .99f,
                post + Vector3.up * 1.325f + alongBeam * .34f, .065f,.065f,.008f);
        }

        static void BuildLantern(Group group, Vector3 mount, Material metal, Material amber, string label)
        {
            var casing = group.Surface(metal);
            // The plate meets the measured tower surface Z7.87; arm and diagonal stay attached.
            AddBeam(casing, mount + new Vector3(0,-.12f,-.035f),
                mount + new Vector3(0,.12f,-.035f), .105f,.075f,.012f);
            var tip = mount + new Vector3(0,.06f,-.55f);
            AddBeam(casing, mount + new Vector3(0,.06f,-.04f), tip, .045f,.045f,.008f);
            AddBeam(casing, mount + new Vector3(0,-.10f,-.045f),
                tip + Vector3.forward * .045f, .035f,.035f,.006f);
            AddBeam(casing, tip, tip + Vector3.down * .105f, .028f,.028f,.005f);
            var center = tip + Vector3.down * .29f;

            // Square taper, chamfered caps and four real cage uprights; no primitive cube lantern.
            AddBeam(casing, center + Vector3.down * .19f,
                center + Vector3.down * .145f, .255f,.255f,.018f);
            AddBeam(casing, center + Vector3.up * .14f,
                center + Vector3.up * .19f, .265f,.265f,.02f);
            AddBeam(casing, center + Vector3.up * .19f,
                center + Vector3.up * .235f, .16f,.16f,.018f);
            foreach (float x in new[] { -.101f, .101f })
                foreach (float z in new[] { -.101f, .101f })
                    AddBeam(casing, center + new Vector3(x,-.15f,z),
                        center + new Vector3(x,.15f,z), .021f,.021f,.004f);
            AddBeam(group.Surface(amber), center + Vector3.down * .128f,
                center + Vector3.up * .128f, .176f,.176f,.026f);

            var lightObject = new GameObject(label + " entrance lantern - warm local fill");
            lightObject.transform.SetParent(group.Root, false);
            lightObject.transform.localPosition = center + Vector3.back * .045f;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = Amber;
            light.intensity = .55f;
            light.range = 2.35f;
            light.shadows = LightShadows.None;
            light.renderMode = LightRenderMode.Auto;
            light.bounceIntensity = 0;
        }

        static readonly Color Amber = new Color(1f, .61f, .27f, 1f);

        static Material LanternMaterial(string folder, Material fallback)
        {
            var shader = Shader.Find("Standard");
            var material = new Material(shader != null ? shader : fallback.shader) { name = "Architectural lantern amber" };
            if (material.HasProperty("_Color")) material.SetColor("_Color", Amber);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", Amber * 1.65f);
                material.EnableKeyword("_EMISSION");
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            }
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", .32f);
            string path = folder + "/lantern_amber.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing == null) { AssetDatabase.CreateAsset(material, path); return material; }
            EditorUtility.CopySerialized(material, existing);
            UnityEngine.Object.DestroyImmediate(material);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        // The 96 perimeter intervals subdivide the twelve straight source edges; they do not
        // turn its polygonal design into a generic circular torus. Bevel profile is closed.
        static void AddPolygonBead(Draft draft, Vector3 center, float radius, float width,
            float front, float back, float bevel)
        {
            float inner = radius - width * .5f, outer = radius + width * .5f;
            var profile = new[] { new Vector2(inner,back), new Vector2(inner,front+bevel),
                new Vector2(inner+bevel,front), new Vector2(outer-bevel,front),
                new Vector2(outer,front+bevel), new Vector2(outer,back) };
            const int samples = 96;
            for (int i = 0; i < samples; i++)
            {
                Vector2 p = PolygonDirection(i, samples), q = PolygonDirection(i + 1, samples);
                var radial = new Vector3(p.x + q.x, p.y + q.y, 0).normalized;
                for (int j = 0; j < profile.Length; j++)
                {
                    var a = profile[j]; var b = profile[(j + 1) % profile.Length];
                    var hint = radial * (b.y - a.y) - Vector3.forward * (b.x - a.x);
                    draft.Quad(center + RingPoint(p,a), center + RingPoint(q,a),
                        center + RingPoint(q,b), center + RingPoint(p,b), hint);
                }
            }
        }

        static Vector2 PolygonDirection(int sample, int samples)
        {
            float edge = (sample % samples) * 12f / samples;
            int corner = Mathf.FloorToInt(edge);
            float a = corner * Mathf.PI / 6, b = (corner + 1) * Mathf.PI / 6;
            return Vector2.Lerp(new Vector2(Mathf.Cos(a),Mathf.Sin(a)),
                new Vector2(Mathf.Cos(b),Mathf.Sin(b)), edge - corner);
        }

        static Vector3 RingPoint(Vector2 direction, Vector2 profile)
        { return new Vector3(direction.x * profile.x, direction.y * profile.x, profile.y); }

        static void AddTurnedPin(Draft draft, Vector3 center, float radius, float depth)
        {
            // A small chamfered twelve-sided pin, pointing towards the street (-Z).
            const int sides = 12;
            for (int i = 0; i < sides; i++)
            {
                float a = i * Mathf.PI * 2 / sides, b = (i + 1) * Mathf.PI * 2 / sides;
                var ra = new Vector3(Mathf.Cos(a),Mathf.Sin(a),0);
                var rb = new Vector3(Mathf.Cos(b),Mathf.Sin(b),0);
                var frontCenter = center - Vector3.forward * depth * .5f;
                var backCenter = center + Vector3.forward * depth * .5f;
                draft.Triangle(frontCenter, frontCenter + ra * radius * .70f,
                    frontCenter + rb * radius * .70f, Vector3.back);
                draft.Quad(frontCenter + ra * radius * .70f, frontCenter + rb * radius * .70f,
                    center + rb * radius, center + ra * radius, (ra+rb) - Vector3.forward);
                draft.Quad(center + ra * radius, center + rb * radius,
                    backCenter + rb * radius, backCenter + ra * radius, ra+rb);
                draft.Triangle(backCenter, backCenter + rb * radius, backCenter + ra * radius, Vector3.forward);
            }
        }

        static void AddBeam(Draft target, Vector3 from, Vector3 to, float width, float depth, float bevel)
        {
            float length = Vector3.Distance(from,to);
            if (length < .001f) return;
            bevel = Mathf.Min(bevel, Mathf.Min(length, Mathf.Min(width,depth)) * .24f);
            var local = new Draft();
            var sections = new Vector2[4][];
            sections[0] = ChamferedRectangle(width*.5f-bevel,depth*.5f-bevel,bevel*.35f);
            sections[1] = ChamferedRectangle(width*.5f,depth*.5f,bevel);
            sections[2] = sections[1]; sections[3] = sections[0];
            var zs = new[] {-length*.5f,-length*.5f+bevel,length*.5f-bevel,length*.5f};
            for (int layer = 0; layer < 3; layer++)
                for (int side = 0; side < 8; side++)
                {
                    int next = (side + 1) % 8;
                    var a = V(sections[layer][side],zs[layer]);
                    var b = V(sections[layer][next],zs[layer]);
                    var c = V(sections[layer+1][next],zs[layer+1]);
                    var d = V(sections[layer+1][side],zs[layer+1]);
                    local.Quad(a,b,c,d,new Vector3((a.x+b.x)*.5f,(a.y+b.y)*.5f,0));
                }
            for (int i = 0; i < 8; i++)
            {
                int n = (i+1)%8;
                local.Triangle(new Vector3(0,0,zs[0]),V(sections[0][i],zs[0]),
                    V(sections[0][n],zs[0]),Vector3.back);
                local.Triangle(new Vector3(0,0,zs[3]),V(sections[3][i],zs[3]),
                    V(sections[3][n],zs[3]),Vector3.forward);
            }
            var direction = (to-from)/length;
            var up = Mathf.Abs(Vector3.Dot(direction,Vector3.up)) > .95f ? Vector3.forward : Vector3.up;
            target.Append(local,Matrix4x4.TRS((from+to)*.5f,Quaternion.LookRotation(direction,up),Vector3.one));
        }

        static Vector2[] ChamferedRectangle(float x, float y, float bevel)
        {
            return new[] {new Vector2(-x+bevel,-y),new Vector2(x-bevel,-y),new Vector2(x,-y+bevel),
                new Vector2(x,y-bevel),new Vector2(x-bevel,y),new Vector2(-x+bevel,y),
                new Vector2(-x,y-bevel),new Vector2(-x,-y+bevel)};
        }
        static Vector3 V(Vector2 v,float z) { return new Vector3(v.x,v.y,z); }

        static void AddCoping(Draft draft, Vector2[] outline, float bottom, float height, float bevel)
        {
            Vector2 center = Vector2.zero;
            foreach (var point in outline) center += point;
            center /= outline.Length;
            var inset = new Vector2[outline.Length];
            for (int i=0;i<outline.Length;i++) inset[i] = Vector2.MoveTowards(outline[i],center,bevel);
            for (int i=0;i<outline.Length;i++)
            {
                int n=(i+1)%outline.Length;
                var outward = new Vector3((outline[i].x+outline[n].x)*.5f-center.x,0,
                    (outline[i].y+outline[n].y)*.5f-center.y);
                draft.Quad(XZ(outline[i],bottom),XZ(outline[n],bottom),
                    XZ(outline[n],bottom+height-bevel),XZ(outline[i],bottom+height-bevel),outward);
                draft.Quad(XZ(outline[i],bottom+height-bevel),XZ(outline[n],bottom+height-bevel),
                    XZ(inset[n],bottom+height),XZ(inset[i],bottom+height),outward+Vector3.up);
                draft.Triangle(XZ(center,bottom+height),XZ(inset[i],bottom+height),
                    XZ(inset[n],bottom+height),Vector3.up);
                draft.Triangle(XZ(center,bottom),XZ(outline[n],bottom),XZ(outline[i],bottom),Vector3.down);
            }
        }
        static Vector3 XZ(Vector2 point,float y) { return new Vector3(point.x,y,point.y); }

        sealed class Group
        {
            public Transform Root { get; private set; }
            readonly string folder;
            readonly List<Material> materials = new List<Material>();
            readonly List<Draft> drafts = new List<Draft>();
            public Group(string label,Transform parent,string assetFolder)
            {
                var go=new GameObject(label); Root=go.transform; Root.SetParent(parent,false); folder=assetFolder;
            }
            public Draft Surface(Material material)
            {
                int index=materials.IndexOf(material);
                if(index>=0)return drafts[index];
                materials.Add(material); var draft=new Draft(); drafts.Add(draft); return draft;
            }
            public void Persist(string prefix)
            {
                // Batch by family/material: details do not introduce one renderer per rivet/beam.
                for(int i=0;i<drafts.Count;i++)
                {
                    var mesh=drafts[i].ToMesh(prefix+"_"+i.ToString("D2",CultureInfo.InvariantCulture));
                    string path=folder+"/"+mesh.name+".asset";
                    var stored=AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    if(stored==null){AssetDatabase.CreateAsset(mesh,path);stored=mesh;}
                    else{EditorUtility.CopySerialized(mesh,stored);UnityEngine.Object.DestroyImmediate(mesh);EditorUtility.SetDirty(stored);}
                    var go=new GameObject(meshName(prefix,i)); go.transform.SetParent(Root,false); go.isStatic=true;
                    go.AddComponent<MeshFilter>().sharedMesh=stored;
                    var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=materials[i];
                    renderer.shadowCastingMode=ShadowCastingMode.On;renderer.receiveShadows=true;
                }
            }
            static string meshName(string prefix,int index){return prefix+" surface "+index;}
        }

        sealed class Draft
        {
            readonly List<Vector3> vertices=new List<Vector3>();
            readonly List<Vector3> normals=new List<Vector3>();
            readonly List<Vector2> uv=new List<Vector2>();
            readonly List<int> indices=new List<int>();
            public void Triangle(Vector3 a,Vector3 b,Vector3 c,Vector3 hint)
            {
                if(Vector3.Dot(Vector3.Cross(b-a,c-a),hint)<0){var swap=b;b=c;c=swap;}
                var normal=Vector3.Cross(b-a,c-a).normalized;
                if(normal.sqrMagnitude<.5f)return;
                int start=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);
                normals.Add(normal);normals.Add(normal);normals.Add(normal);
                uv.Add(Vector2.zero);uv.Add(new Vector2((b-a).magnitude,0));
                uv.Add(new Vector2(0,(c-a).magnitude));
                indices.Add(start);indices.Add(start+1);indices.Add(start+2);
            }
            public void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d,Vector3 hint)
            {
                if(Vector3.Dot(Vector3.Cross(b-a,c-a),hint)<0){var swap=b;b=d;d=swap;}
                var normal=Vector3.Cross(b-a,c-a).normalized;
                if(normal.sqrMagnitude<.5f)return;
                int start=vertices.Count;vertices.Add(a);vertices.Add(b);vertices.Add(c);vertices.Add(d);
                for(int i=0;i<4;i++)normals.Add(normal);
                float u=(b-a).magnitude,v=(d-a).magnitude;
                uv.Add(Vector2.zero);uv.Add(new Vector2(u,0));uv.Add(new Vector2(u,v));uv.Add(new Vector2(0,v));
                indices.Add(start);indices.Add(start+1);indices.Add(start+2);
                indices.Add(start);indices.Add(start+2);indices.Add(start+3);
            }
            public void Append(Draft source,Matrix4x4 transform)
            {
                int offset=vertices.Count;
                for(int i=0;i<source.vertices.Count;i++)
                {vertices.Add(transform.MultiplyPoint3x4(source.vertices[i]));normals.Add(transform.MultiplyVector(source.normals[i]).normalized);uv.Add(source.uv[i]);}
                foreach(int index in source.indices)indices.Add(index+offset);
            }
            public Mesh ToMesh(string name)
            {
                var mesh=new Mesh{name=name,indexFormat=vertices.Count>65535?IndexFormat.UInt32:IndexFormat.UInt16};
                mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);
                var colors=new Color[vertices.Count];for(int i=0;i<colors.Length;i++)colors[i]=Color.white;
                mesh.colors=colors;mesh.RecalculateBounds();mesh.RecalculateTangents();return mesh;
            }
        }

        static string ValidateFolder(string path)
        {
            if(string.IsNullOrEmpty(path))throw new ArgumentException("A generated asset folder is required.");
            path=path.Replace('\\','/').TrimEnd('/');
            if(path!=DefaultAssetFolder&&!path.StartsWith(DefaultAssetFolder+"/",StringComparison.Ordinal))
                throw new ArgumentException("Architectural meshes must remain under the ignored "+DefaultAssetFolder+" directory.");
            foreach(string segment in path.Split('/'))
                if(segment==".."||segment=="."||segment.Length==0)
                    throw new ArgumentException("Generated asset folder contains invalid path segments.");
            return path;
        }
        static void EnsureFolder(string path)
        {
            if(AssetDatabase.IsValidFolder(path))return;
            int split=path.LastIndexOf('/');var parent=path.Substring(0,split);EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent,path.Substring(split+1));
        }
    }
}
