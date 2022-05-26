using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PowerUpLifeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimECB;
    protected override void OnCreate()
    {
        m_EndSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();



    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimECB.CreateCommandBuffer().AsParallelWriter();
      //  GetBufferFromEntity<PowerUpDataBuffer>();
    //    BufferFromEntity<PowerUpDataBuffer> lookup = GetBufferFromEntity<PowerUpDataBuffer>();
      


        float dt = Time.DeltaTime;
        Entities.
        ForEach((Entity entity, int nativeThreadIndex,ref Translation translation, ref DynamicBuffer<PowerUpDataBuffer> powerUpDataBuffer, in Rotation rotation) => {
            //var buffer = lookup[e];
            for (var i = 0; i < powerUpDataBuffer.Length; i++)
            {
                var powerUpDataBufferInst = powerUpDataBuffer[i];
                PowerUpData powerUp = powerUpDataBufferInst.PowerUpDataSet;
                powerUp.currentTime += dt;
               
                if (powerUp.currentTime > powerUp.time)
                {
                  
                     if (powerUp.type == 'F')
                    {
                        bool shouldRemove = true;
                        for (var x = 0; x < powerUpDataBuffer.Length; x++)
                        {
                            var powerUpDataBufferInst2 = powerUpDataBuffer[x];
                            PowerUpData powerUp2 = powerUpDataBufferInst2.PowerUpDataSet;

                            if ((powerUp2.type == 'F') && x != i)
                            {
                                shouldRemove = false;
                            }

                        }
                        if ( shouldRemove)
                        {
                            commandBuffer.AddComponent(nativeThreadIndex, entity, new RemoveFasterBulletsPowerTag { }); //Schedule for next frame

                        }
                    }
                
                    if (powerUp.type == 'S')
                    {
                        bool shouldRemove = true;
                        for (var x = 0; x < powerUpDataBuffer.Length; x++)
                            {
                                var powerUpDataBufferInst2 = powerUpDataBuffer[x];
                                PowerUpData powerUp2 = powerUpDataBufferInst2.PowerUpDataSet;

                                if ((powerUp2.type == 'S') && x != i)
                                {
                                    shouldRemove = false;
                                }
                            }
                        if (shouldRemove)
                        {
                            commandBuffer.RemoveComponent<ShieldPowerTag>(nativeThreadIndex, entity);

                        }
                    }
                    powerUpDataBuffer.RemoveAt(i);
                }
                else
                {
                    powerUpDataBufferInst.PowerUpDataSet = powerUp;
                    powerUpDataBuffer[i] = powerUpDataBufferInst;
                }

            }


        }).ScheduleParallel();
        m_EndSimECB.AddJobHandleForProducer(this.Dependency);
    }
    /*
   private bool  hasPowerUpOfSameType(DynamicBuffer<PowerUpDataBuffer> powerUpDataBuffer, int i, char type)
    {
        for (var x = 0; x < powerUpDataBuffer.Length; x++)
        {
            var powerUpDataBufferInst2 = powerUpDataBuffer[x];
            PowerUpData powerUp2 = powerUpDataBufferInst2.PowerUpDataSet;

            if ((powerUp2.type == type) && x != i)
            {
                return true;
               
            }
        }
        return false;
    }*/
}
