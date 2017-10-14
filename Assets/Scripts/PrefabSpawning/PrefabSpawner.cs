using UnityEngine;

public class PrefabSpawner : MonoBehaviour 
{
    public GameObject Prefab;

    public SpawnShape PositionShape = SpawnShape.Sphere;
    public Vector3 PositionScale = Vector3.one;

    public SpawnShape VelocityShape = SpawnShape.None;

    public int InitialAmount = 100;

    private Vector3Provider positionProvider;

    void Spawn(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            var pos = positionProvider.Next();

            var go = Instantiate(Prefab,
                                 transform.position + pos * 10,
                                 Quaternion.identity,
                                 transform);

            // var scale = SizeDistribution.Evaluate(Random.value);
            // p.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    void Awake()
    {
        positionProvider = new RandomSphere();
        Spawn(InitialAmount);
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        
    }
    
    void OnDisable()
    {
        
    }
    
    void Update() 
    {
        
    }
}
