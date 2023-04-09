using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
public partial struct SpritePositionUpdate : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        SpriteSheetCache.cache = new Dictionary<int, SpriteSheetDrawInfo>();
        SpriteSheetCache.cachedSpriteSheets = new Dictionary<Texture, Sprite[]>();
    }

    public void OnDestroy(ref SystemState state)
    {
        SpriteSheetCache.ClearCache();
    }

    public void OnUpdate(ref SystemState state)
    {
        new SpritePositionUpdateJob{}.ScheduleParallel();
    }
}


[BurstCompile]
public partial struct SpritePositionUpdateJob : IJobEntity {

    [BurstCompile]
    public void Execute(ref LocalToWorld localToWorld, ref SpriteSheetAnimationData data) {
        data.instanceData.worldMatrix = localToWorld.Value;
        data.instanceData.worldMatrixInverse = Matrix4x4.Inverse(localToWorld.Value);
    }
}