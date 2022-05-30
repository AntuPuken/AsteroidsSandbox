using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WorldBordersRespawnSystem : SystemBase
{
    private float sideBuffer = 0.5f; 
    protected override void OnUpdate()
    {

        var SideBuffer = sideBuffer;
        var cam = Camera.main;
        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));
        float2 bottomR = (float2)bottomRight;
        float2 bottomL = (float2)bottomLeft;
        float2 topL = (float2)topLeft;
        float2 topR = (float2)topRight;
       //Getting screen borders
        
        float dt = Time.DeltaTime;
        //and elapsed time since last frame
        
        Entities.
             WithAny<PlayerTag>().
              WithAny<AsteroidTag>().
                WithAny<BulletTag>().
                WithAny<EnemyTag>().
             ForEach((ref Translation translation) => {
                 //if the current entity is outside the screen borders
                 if(translation.Value.x < topL.x - SideBuffer)
                 {
                     //teleport it to the opposite border 
                     translation.Value = new float3(topR.x + SideBuffer, translation.Value.y, translation.Value.z);
                 }
                 if (translation.Value.x > topR.x + SideBuffer)
                 {
                    translation.Value = new float3(topL.x - SideBuffer, translation.Value.y, translation.Value.z);
                 }
                 if (translation.Value.y > topL.y + SideBuffer)
                 {
                     translation.Value = new float3(translation.Value.x, bottomL.y - SideBuffer, translation.Value.z);
                 }
                 if (translation.Value.y <  bottomL.y - SideBuffer)
                 {
                     translation.Value = new float3(translation.Value.x, topR.y + SideBuffer, translation.Value.z);
                 }

             }).ScheduleParallel();
             //Scheduling for paralel worker threads

    }

}
