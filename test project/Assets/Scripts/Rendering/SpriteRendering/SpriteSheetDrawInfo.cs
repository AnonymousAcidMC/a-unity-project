using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpriteSheetDrawInfo
{
    public Material spriteSheetMaterial;
    public Mesh mesh;

    public ComputeBuffer argsBuffer;
    public ComputeBuffer instancesBuffer;
    public Dictionary<int, InstanceData> instances;
    public Bounds renderBounds;
    public MaterialPropertyBlock materialPropertyBlock;
    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;
    private EntityManager entityManager;
    public Rect[] uvRects;

    #region constructors
    
    public SpriteSheetDrawInfo(Material material, Mesh mesh, Bounds renderBounds) {
        this.spriteSheetMaterial = material;
        this.mesh = mesh;
        this.renderBounds = renderBounds;
        materialPropertyBlock = new MaterialPropertyBlock();
        shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        recieveShadows = true;
        instances = new Dictionary<int, InstanceData> ();

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public SpriteSheetDrawInfo(RenderArgs args) {
        this.spriteSheetMaterial = args.material;
        this.materialPropertyBlock = args.materialPropertyBlock;
        this.mesh = args.mesh;
        this.recieveShadows = args.recieveShadows;
        this.renderBounds = args.renderBounds;
        this.shadowCastingMode = args.shadowCastingMode;
        this.instances = new Dictionary<int, InstanceData> ();

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    #endregion constructors

    #region buffer methods
    public void UpdateAllBuffers() {
        UpdateArgsBuffer();
        UpdateInstancesBuffer();//update instance buffer first, because material buffer relies on it.
        UpdateMaterialBuffer();
    }

    public void UpdateInstancesBuffer() {
        instancesBuffer?.Release();

        instancesBuffer = new ComputeBuffer(instances.Count, InstanceData.Size());
        instancesBuffer.SetData(new List<InstanceData>(instances.Values));
    }

    public void UpdateArgsBuffer() {
        argsBuffer?.Release();

        argsBuffer = new ComputeBuffer(1, sizeof(uint)*5, ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(new uint[] {
            mesh.GetIndexCount(0),
            (uint)instances.Count,
            mesh.GetIndexStart(0),
            mesh.GetBaseVertex(0),
            0
        });
    }

    public void UpdateMaterialBuffer() {
        spriteSheetMaterial.SetBuffer("_PerInstanceData", instancesBuffer);
    }

    public void DestroyBuffers() {
        argsBuffer?.Release();
        instancesBuffer?.Release();
    }

    #endregion buffer methods

    public void Draw() {
        if(instances.Count == 0)
            return;
        UpdateAllBuffers();

        Graphics.DrawMeshInstancedIndirect(
            mesh, 0,
            spriteSheetMaterial, renderBounds,
            argsBuffer, 0,
            materialPropertyBlock,
            shadowCastingMode,
            recieveShadows
        );
    }
}
