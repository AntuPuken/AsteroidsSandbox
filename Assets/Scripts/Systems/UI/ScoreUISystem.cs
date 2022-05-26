using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ScoreUISystem : SystemBase
{

    private Entity scoreTextEntity;
    private ScoreUIData scoreTextUI;
    private ScoreData scoreTextData;
    private Entity lifesTextEntity;
    private PlayerLifesData lifesTextData;
    private PlayerLifesUIData lifesTextUI;
    private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnStartRunning()
    {
        base.OnCreate();
        RequireSingletonForUpdate<PlayerLifesData>();
        RequireSingletonForUpdate<ScoreData>();

        scoreTextEntity = GetSingletonEntity<ScoreData>();
        scoreTextData = EntityManager.GetComponentData<ScoreData>(scoreTextEntity);
        scoreTextUI = EntityManager.GetComponentData<ScoreUIData>(scoreTextEntity);
        lifesTextEntity = GetSingletonEntity<PlayerLifesData>();
        lifesTextData = EntityManager.GetComponentData<PlayerLifesData>(lifesTextEntity);
        lifesTextUI = EntityManager.GetComponentData<PlayerLifesUIData>(lifesTextEntity);

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    }

    protected override void OnUpdate()
    {

        Entities.
       WithAny<DestroyAsteroidTag>().
      ForEach((Entity entity) => {
        scoreTextData.currentScore += 5;
        scoreTextUI.scoreText.text = $"Score : { scoreTextData.currentScore}";
    }).WithoutBurst().Run();
       
        
        Entities.
     WithAny<EnemyDestroyTag>().
    ForEach((Entity entity) => {
        scoreTextData.currentScore += 30;
        scoreTextUI.scoreText.text = $"Score : { scoreTextData.currentScore}";
    }).WithoutBurst().Run();
      
        
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Entities.
        WithAny<RefreshUIPlayerLifesTag>().
        ForEach((Entity entity, in PlayerData playerData) => {


            lifesTextData.lifesAmount = playerData.currentLifes;
                lifesTextUI.lifesText.text = $"Player Lifes: {    lifesTextData.lifesAmount }";

            ecb.RemoveComponent<RefreshUIPlayerLifesTag>(entity);


        }).WithoutBurst().Run();
       
        Entities.
        WithAny<ResetUIScoreTag>().
        ForEach((Entity entity) => {
            scoreTextData.currentScore = 0;
            scoreTextUI.scoreText.text = $"Score : { scoreTextData.currentScore}";
            ecb.DestroyEntity(entity);
        }).WithoutBurst().Run();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }

}
