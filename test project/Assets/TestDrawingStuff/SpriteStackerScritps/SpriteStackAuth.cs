using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public class SpriteStackAuth : MonoBehaviour
{
    public Sprite spriteSheet;
    public Material baseMaterial;

    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
    public bool recieveShadows;


    private InstanceDataClass instanceDataObj;
    private DrawData data;
    
    public static List<Texture2D> GetSlicedSpriteTextures(Sprite sprite)
    {
        List<Texture2D> textures = new List<Texture2D>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(sprite.texture.name);
        
        for(var i=0; i<sprites.Length; i++) {
            Sprite s = sprites[i];
            Rect sprRect = s.rect;
            Texture2D slicedTex = new Texture2D((int)sprRect.width, (int)sprRect.height);
            slicedTex.filterMode = sprite.texture.filterMode;
            
            Color[] colors = sprite.texture.GetPixels((int)sprRect.x, (int)sprRect.y, (int)sprRect.width, (int)sprRect.height);

            slicedTex.SetPixels(
                0, 0,
                (int)sprRect.width, (int)sprRect.height,
                colors
                );
            
            slicedTex.Apply();   
            slicedTex.name = s.name;    
            textures.Add(slicedTex);
        }
 
        return textures;
    }
}

public class SpriteStackBaker : Baker<SpriteStackAuth>
{
    public override void Bake(SpriteStackAuth authoring)
    {
        List<Texture2D> textures = SpriteStackAuth.GetSlicedSpriteTextures(authoring.spriteSheet);
        NativeList<DrawDataComponent> spriteDrawData = new NativeList<DrawDataComponent>(Allocator.Persistent);

        for(var i=0; i<textures.Count; i++) {
            Texture2D tex = textures[i];
            Material material = new Material(authoring.baseMaterial);
            material.mainTexture = tex;

            Vector3 offset = new Vector3(0, i*-0.02f, 0);

            //Cache the sprite slice
            DrawDataComponent component = InstancingCache.CacheSpriteEntity(
                material,
                DrawData.NewQuadMesh(),
                offset,
                Vector3.one,
                new Bounds(Vector3.zero, Vector3.one*10)
            );
            DrawData data = InstancingCache.cache[component.drawDataCacheIndex];
            data.shadowCastingMode = authoring.shadowCastingMode;
            data.receiveShadows = authoring.recieveShadows;
            
            spriteDrawData.Add(component);
        }

        AddComponent(new SpriteStackComponent {
            spriteDrawData = spriteDrawData
        });

    }
}