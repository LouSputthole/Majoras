"""Run inside the source-import Blender process. Exports local reference data only."""
import bpy, json
from pathlib import Path

DEST = PROJECT/'Unity/Assets/LocalReference/SouthClockTown'
DEST.mkdir(parents=True, exist_ok=True)
(DEST/'Textures').mkdir(exist_ok=True)

def vec(v): return {'x':v[0], 'y':v[1], 'z':v[2]}
def rgba(c): return {'r':c[0], 'g':c[1], 'b':c[2], 'a':c[3]}
def uv(v): return {'x':v[0], 'y':v[1]}
def texture_size(t):
    return t.tex.size[:] if t.tex else t.tex_reference_size[:] if t.use_tex_reference else (32,32)
def tile(t):
    return {'size':uv(texture_size(t)), 'shift':uv((t.S.shift,t.T.shift)), 'low':uv((t.S.low,t.T.low))}
materials=[]
meshes=[]
images={}
used=[]
source_objects=[ob for ob in bpy.context.scene.objects if not ob.name.startswith('fast64_f3d_material_library')]
for ob in source_objects:
    if ob.type=='MESH' and len(ob.data.polygons) and getattr(ob,'ignore_collision',False):
        for m in ob.data.materials:
            if m and m not in used: used.append(m)

def tex(t):
    im=t.tex
    if not im: return ''
    if im.name not in images:
        name=f'texture_{len(images):03d}.png'
        im.filepath_raw=str(DEST/'Textures'/name)
        im.file_format='PNG'
        im.save()
        images[im.name]=name
    return images[im.name]

for m in used:
    f=m.f3d_mat
    materials.append({'name':m.name,'texture0':tex(f.tex0),'texture1':tex(f.tex1),
        'sourceTexture0':f.tex0.tex.name if f.tex0.tex else '', 'sourceTexture1':f.tex1.tex.name if f.tex1.tex else '',
        'prim':rgba(f.prim_color),'env':rgba(f.env_color),
        'primLodFrac':f.prim_lod_frac,
        'uvBasisSize':uv(texture_size(f.tex0) if f.tex0.tex or f.tex0.use_tex_reference else texture_size(f.tex1)),
        'texScale':uv(f.tex_scale),'tile0':tile(f.tex0),'tile1':tile(f.tex1),
        'combiner1':[getattr(f.combiner1,k) for k in ['A','B','C','D','A_alpha','B_alpha','C_alpha','D_alpha']],
        'combiner2':[getattr(f.combiner2,k) for k in ['A','B','C','D','A_alpha','B_alpha','C_alpha','D_alpha']],
        'twoCycle':f.rdp_settings.g_mdsft_cycletype=='G_CYC_2CYCLE',
        'lighting':f.rdp_settings.g_lighting,
        'transparent':f.draw_layer.oot=='Transparent',
        'cullBack':f.rdp_settings.g_cull_back,
        'wrap0':[('Clamp' if a.clamp else 'Mirror' if a.mirror else 'Repeat') for a in [f.tex0.S,f.tex0.T]],
        'wrap1':[('Clamp' if a.clamp else 'Mirror' if a.mirror else 'Repeat') for a in [f.tex1.S,f.tex1.T]]})

for ob in source_objects:
    if ob.type!='MESH' or not len(ob.data.polygons): continue
    me=ob.data
    me.calc_loop_triangles()
    collision=not getattr(ob,'ignore_collision',False)
    positions=[]; colors=[]; normals=[]; uvs=[]; triangles=[]; submeshes=[]
    col=me.color_attributes.get('Col')
    alpha=me.color_attributes.get('Alpha')
    uvl=me.uv_layers.active
    for mi in range(max(1,len(me.materials))):
        indices=[]
        for tri in me.loop_triangles:
            if tri.material_index!=mi: continue
            for li in tri.loops:
                vi=me.loops[li].vertex_index
                p=ob.matrix_world @ me.vertices[vi].co
                n=ob.matrix_world.to_3x3().inverted().transposed() @ me.corner_normals[li].vector
                n.normalize()
                # Blender X-right/Z-up to Unity X-right/Y-up; mirror winding below.
                positions.append(vec((p.x,p.z,p.y)))
                normals.append(vec((n.x,n.z,n.y)))
                c=col.data[li if col.domain=='CORNER' else vi].color if col else (1,1,1,1)
                a=alpha.data[li if alpha.domain=='CORNER' else vi].color[0] if alpha else 1
                colors.append(rgba((c[0],c[1],c[2],a)))
                uvs.append(uv(uvl.data[li].uv if uvl else (0,0)))
                indices.append(len(positions)-1)
        for i in range(0,len(indices),3): indices[i+1],indices[i+2]=indices[i+2],indices[i+1]
        if indices:
            submeshes.append({'material':used.index(me.materials[mi]) if not collision and me.materials[mi] in used else -1,'indices':indices})
    meshes.append({'name':ob.name,'collision':collision,'vertices':positions,'normals':normals,'colors':colors,'uv':uvs,'submeshes':submeshes})
doc={'version':1,'coordinateSystem':'Unity X=N64 X, Y=N64 Y, Z=-N64 Z; 100 source units per Unity unit (provisional physical scale)', 'materials':materials,'meshes':meshes}
(DEST/'reference.json').write_text(json.dumps(doc,separators=(',',':')))
(DEST/'texture-sources.json').write_text(json.dumps(images,indent=2))
print('REFERENCE_EXPORT',json.dumps({'meshes':len(meshes),'materials':len(materials),'textures':len(images),'triangles':sum(len(s['indices'])//3 for m in meshes for s in m['submeshes'])}),flush=True)
