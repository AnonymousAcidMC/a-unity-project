using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public struct RenderComponent : IComponentData
{
    public int materialIndex;
}