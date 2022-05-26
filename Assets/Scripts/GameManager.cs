using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.UI;
using Unity.Transforms;
using Unity.Jobs;
public class GameManager : MonoBehaviour
{
    public static GameManager main;

    public GameObject AsteroidPrefab;
    Entity AsteroidEntityPrefab;
    public GameObject PlayerPrefab;
    Entity PlayerEntityPrefab;
    public GameObject BulletPrefab;
    Entity BulletEntityPrefab;

    EntityManager manager;
    
    BlobAssetStore blobAssetStore;

    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }

        
        main = this;
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        AsteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(AsteroidPrefab, settings);
        PlayerEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerPrefab, settings);
        BulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(BulletPrefab, settings);

    }
   
    void SpawnAsteroid()
    {
        Entity asteroid = manager.Instantiate(AsteroidEntityPrefab);
        Movable moveData = new Movable()
        {
            speed = 2,
           // direction = new float3(0f, 1f, 0f),
            forward = 0,
            turnDirection = 0,
            turningSpeed = 0,
        };
       
        manager.AddComponentData(asteroid, moveData);
        manager.AddComponentData(asteroid, new Translation { Value = new float3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), 0f)});
    }
    void SpawnBullet()
    {
        Entity bullet = manager.Instantiate(BulletEntityPrefab);
        Translation bulletPos = manager.GetComponentData<Translation>(PlayerEntityPrefab);
        Rotation bulletRot = manager.GetComponentData<Rotation>(PlayerEntityPrefab);
       manager.AddComponentData(bullet, bulletPos);
        manager.AddComponentData(bullet, bulletRot);
    }
    void SpawnPlayer()
    {
        Entity player = manager.Instantiate(PlayerEntityPrefab);
    
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
      //   SpawnPlayer();
        //SpawnAsteroid();


    }

    // Update is called once per frame
    void Update()
    {
       //SpawnBullet();
       // SpawnAsteroid();
    }
}
