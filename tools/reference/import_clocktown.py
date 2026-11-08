import bpy, sys, json, re, math
from pathlib import Path
from mathutils import Matrix

PROJECT = Path(__file__).resolve().parents[2]
ROOT = PROJECT / 'local-reference'
parser_path=ROOT/'fast64/fast64_internal/f3d/f3d_parser.py'
parser_source=parser_path.read_text(encoding='utf-8')
old='print("gsSPPopMatrix not handled.")'
new='self.currentTransformName = self._reference_matrix_stack.pop()'
if old in parser_source:
    parser_path.write_text(parser_source.replace(old,new),encoding='utf-8')
elif new not in parser_source:
    raise RuntimeError('Fast64 parser differs from the pinned revision; review matrix handling.')
sys.path.insert(0, str(ROOT))
import fast64
# MM importer is present upstream but its UI selector is disabled there.
fast64.gameEditorEnum += (("MM", "MM", "Majora's Mask", 4),)
fast64.register()
bpy.context.scene.gameEditorMode = 'MM'
from fast64.fast64_internal.f3d.f3d_parser import F3DContext
from fast64.fast64_internal.z64.importer import scene_header
from fast64.fast64_internal.f3d.f3d_parser import parseMatrices as upstream_parse_matrices
# Upstream's importer did not pop matrix state and repeatedly applied scene
# scale during billboard matrix multiplication. Keep source matrices unscaled;
# each display list's root already applies the one unit conversion.
scene_header.parseMatrices = lambda data,ctx,scale: upstream_parse_matrices(data,ctx,1)
def reference_matrix(self,name,flags='G_MTX_NOPUSH | G_MTX_LOAD | G_MTX_MODELVIEW'):
    if not hasattr(self,'_reference_matrix_stack'): self._reference_matrix_stack=[]
    if 'G_MTX_PUSH' in flags: self._reference_matrix_stack.append(self.currentTransformName)
    if 'G_MTX_MUL' in flags:
        key=name+'_reference_'+str(len(self.matrixData))
        self.addMatrix(key,self.matrixData[self.currentTransformName] @ self.matrixData[name])
        self.currentTransformName=key
    else: self.currentTransformName=name
F3DContext.setCurrentTransform=reference_matrix
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
scene = bpy.context.scene
scene.ootDecompPath = str(ROOT / 'mm-decomp')
scene.ootBlenderScale = 100.0
settings = scene.ootSceneImportSettings
settings.isCustomDest = True
settings.destPath = str(ROOT / 'mm-decomp/extracted/n64-us/assets/scenes')
settings.name = 'Z2_CLOCKTOWER'
settings.includeMesh = True
settings.includeCollision = False
for key in ['includeActors','includeCutscenes','includeLights','includeCameras','includePaths','includeWaterBoxes','includeAnimatedMats']:
    setattr(settings, key, False)
print('FAST64_REGISTERED_MM', flush=True)
source = Path(settings.destPath) / settings.name / (settings.name + '.c')
if not source.exists():
    raise FileNotFoundError(f'Extract the scene with ZAPD first: {source}')
else:
    from fast64.fast64_internal.z64.importer.scene import parseScene
    from fast64.fast64_internal.z64.importer import room_header
    # Static base-header study only. These are gameplay object-bank metadata,
    # not meshes; upstream still registers the OOT enum for MM banks.
    room_header.parseObjectList = lambda *args: None
    room_header.parseAlternateRoomHeaders = lambda *args: None
    scene_header.parseAlternateSceneHeaders = lambda *args: None
    area = next(a for a in bpy.context.screen.areas if a.type == 'VIEW_3D')
    region = next(r for r in area.regions if r.type == 'WINDOW')
    with bpy.context.temp_override(area=area, region=region):
        parseScene(settings, settings.option)
        from fast64.fast64_internal.f3d.f3d_parser import importMeshC
        from fast64.fast64_internal.z64.model_classes import OOTF3DContext
        from fast64.fast64_internal.f3d.f3d_gbi import get_F3D_GBI
        # Source actor transforms, static Day 1 noon. Model scale .1 and world
        # scale .01 are separate. Counterweight uses initialization phase.
        def actor(bank, dl, pos, yaw=0, name=None):
            path=ROOT/'mm-decomp/extracted/n64-us/assets/objects'/bank/(bank+'.c')
            ctx=OOTF3DContext(get_F3D_GBI(), [], str(ROOT/'mm-decomp'), context='dl')
            ctx.mat().prim_color=(1,1,1,1)
            ctx.mat().env_color=(1,1,1,1)
            ob=importMeshC(path.read_text(),dl,1000.0,True,True,'Opaque',ctx)
            ob.name=name or dl
            ob.ignore_collision=True
            ob.rotation_euler.z=math.radians(yaw)
            ob.location=(pos[0]/100,-pos[2]/100,pos[1]/100)
            return ob
        for dl in ['gClockTowerMinuteRingDL','gClockTowerClockCenterAndHandDL','gClockTowerClockFaceDL']:
            actor('object_obj_tokeidai',dl,(-280,555,-833))
        actor('object_obj_tokeidai','gClockTowerSunAndMoonPanelDL',(-280,443.8,-834.96))
        actor('object_obj_tokeidai','gClockTowerExteriorGearDL',(-492,555,-1012),270,'Clock_Gear_West')
        actor('object_obj_tokeidai','gClockTowerExteriorGearDL',(-68,555,-1012),90,'Clock_Gear_East')
        actor('object_obj_tokeidai','gClockTowerCounterweightDL',(-280,1103,-1020))
        actor('object_tokei_turret','gClockTownTurretPlatformBaseDL',(-290,0,160))
        actor('object_tokei_turret','gClockTownTurretPlatformTopDL',(-290,80,160))
        actor('object_tokei_step','gClocktowerPanelDL',(-280,255,-853))
        actor('object_tokei_tobira','object_tokei_tobira_DL_001108',(-360,0,-807))
        actor('object_tokei_tobira','object_tokei_tobira_DL_0013E8',(-200,0,-807))
    # Preserve collision geometry without assigning OOT surface enum values to
    # MM data. Surface gameplay flags remain in original extracted source.
    data=source.read_text()
    def entries(typ,suffix):
        body=re.search(typ+r'\s+Z2_CLOCKTOWERCollisionHeader_005560'+suffix+r'\[\]\s*=\s*\{(.*?)\};',data,re.S).group(1)
        return [[int(t.strip(),0) for t in item.split(',') if t.strip()] for item in re.findall(r'\{([^{}]+)\}',body)]
    verts=[(x/100,-z/100,y/100) for x,y,z in entries('Vec3s','Vertices')]
    faces=[tuple(v&0x1fff for v in row[1:4]) for row in entries('CollisionPoly','Polygons')]
    mesh=bpy.data.meshes.new('Source_Collision'); mesh.from_pydata(verts,[],faces); mesh.update()
    collision=bpy.data.objects.new('Source_Collision',mesh); scene.collection.objects.link(collision)
    collision.hide_render=True
    out = ROOT / 'south-clock-town'
    out.mkdir(exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(out/'south-clock-town-source.blend'))
    report = {'meshes': [{'name': o.name, 'vertices': len(o.data.vertices), 'polygons': len(o.data.polygons)} for o in bpy.data.objects if o.type == 'MESH'], 'materials': len(bpy.data.materials), 'images': [im.name for im in bpy.data.images]}
    (out/'import-report.json').write_text(json.dumps(report,indent=2))
    print('IMPORT_REPORT_SAVED', str(out/'import-report.json'), flush=True)
    exec(compile((PROJECT/'tools/reference/export_reference.py').read_text(),str(PROJECT/'tools/reference/export_reference.py'),'exec'))
