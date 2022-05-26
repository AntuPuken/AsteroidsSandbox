using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WorldBordersRespawnSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        var cam = Camera.main;
        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));
        float2 bottomR = (float2)bottomRight;
        float2 bottomL = (float2)bottomLeft;
        float2 topL = (float2)topLeft;
        float2 topR = (float2)topRight;
        float dt = Time.DeltaTime;
        Entities.
             WithAny<PlayerTag>().
              WithAny<AsteroidTag>().
                WithAny<BulletTag>().
                WithAny<EnemyTag>().
             ForEach((ref Translation translation) => {
            if(translation.Value.x < topL.x - 0.5f)
                {
                    translation.Value = new float3(topR.x + 0.5f, translation.Value.y, translation.Value.z);
                }
            if (translation.Value.x > topR.x + 0.5f)
                {
                    translation.Value = new float3(topL.x - 0.5f, translation.Value.y, translation.Value.z);
                }
                if (translation.Value.y > topL.y + 0.5f)
                 {
                     translation.Value = new float3(translation.Value.x, bottomL.y - 0.5f, translation.Value.z);
                 }
               if (translation.Value.y <  bottomL.y - 0.5f)
                 {
                     translation.Value = new float3(translation.Value.x, topR.y + 0.5f, translation.Value.z);
                 }

                 // float3 pos = Camera.main.WorldToViewportPoint(translation.Value);
             }).ScheduleParallel();
    }

}
