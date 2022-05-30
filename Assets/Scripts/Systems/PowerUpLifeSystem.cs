using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PowerUpLifeSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimECB;
    //creating the Command buffer system variable

    protected override void OnCreate()
    {
        m_EndSimECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage




    }
    protected override void OnUpdate()
    {
        var commandBuffer = m_EndSimECB.CreateCommandBuffer().AsParallelWriter();
        // create new command buffer as paralel wirter for the end of every frame 



        float dt = Time.DeltaTime;
        //getting elapsed time since last frame

        Entities.
        ForEach((Entity entity, int nativeThreadIndex,ref DynamicBuffer<PowerUpDataBuffer> powerUpDataBuffer) => {
            //this will execute on all enities with a PowerUpDataBuffer AKA al players that have picked up a power up 
            for (var i = 0; i < powerUpDataBuffer.Length; i++)
            {
            //for each powerup
                var powerUpDataBufferInst = powerUpDataBuffer[i];
                PowerUpData powerUp = powerUpDataBufferInst.PowerUpDataSet;
                //get a reference to its data 

                powerUp.currentTime += dt;
               //add elapsed time since last frame to the total time
                if (powerUp.currentTime > powerUp.time)
                {
                //if the current time has passed the dessired time  


                    if (powerUp.type == 'F')
                    {
                    //if the type of powerUp is F, for FasterBullets
                        bool shouldRemove = true;

                        for (var x = 0; x < powerUpDataBuffer.Length; x++)
                        {
                        //we iterate trough the powerups again to check if theres not another poewr of the same type active at another position on the buffer
                            var powerUpDataBufferInst2 = powerUpDataBuffer[x];
                            PowerUpData powerUp2 = powerUpDataBufferInst2.PowerUpDataSet;

                            if ((powerUp2.type == 'F') && x != i)
                            {
                            //and if there is we shouldnt remove the powerUp
                                shouldRemove = false;
                                //so we flag it accordingly 
                            }

                        }
                        if ( shouldRemove)
                        {
                        //but if theres no other power up of same type present
                            commandBuffer.AddComponent(nativeThreadIndex, entity, new RemoveFasterBulletsPowerTag { }); //Schedule for next frame
                            //we add a Tag for the FasterBulletsSystem to remove the power
                        }
                    }
                
                    if (powerUp.type == 'S')
                    {
                    //if the type of powerUp is S, for Shield

                        bool shouldRemove = true;
                        for (var x = 0; x < powerUpDataBuffer.Length; x++)
                        {
                        //we iterate trough the powerups again to check if theres not another poewr of the same type active at another position on the buffer
                            
                                var powerUpDataBufferInst2 = powerUpDataBuffer[x];
                                PowerUpData powerUp2 = powerUpDataBufferInst2.PowerUpDataSet;

                                if ((powerUp2.type == 'S') && x != i)
                                {
                                //and if there is we shouldnt remove the powerUp
                                shouldRemove = false;
                                //so we flag it accordingly 
                              
                                }
                            }
                        if (shouldRemove)
                        {
                        //but if theres no other power up of same type present

                            commandBuffer.RemoveComponent<ShieldPowerTag>(nativeThreadIndex, entity);
                            //We remove the porwerUp 
                        }
                    }
                    powerUpDataBuffer.RemoveAt(i);
                    //always remove the powerup from the DynamicBuffer because its time has runned out
                }
                else
                {
                //if its time hasnt runned out
                    powerUpDataBufferInst.PowerUpDataSet = powerUp;
                    powerUpDataBuffer[i] = powerUpDataBufferInst;
                    //save the delta time changes to the DynamicBuffer
                }

            }


        }).ScheduleParallel();
        //Scheduling for paralel worker threads


        m_EndSimECB.AddJobHandleForProducer(this.Dependency);
        //This will add our dependency to be played back on the EndSimulationEntityCommandBuffer

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
