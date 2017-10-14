using UnityEngine;

public class PrefabDistributionSpherical : MonoBehaviour 
{
    public AnimationCurve PositionDistribution;
    public int PositionDistributionSubdivions;

    public AnimationCurve SizeDistribution;

    public GameObject Prefab;

    public int Amount = 100;

    void Awake()
    {
        SpawnAll();
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        
    }

    void SpawnAll()
    {
        for (var i = 0; i < Amount; ++i)
        {
            // var pos = new Vector3(CurveXPosition.Evaluate(Random.value),
            //                       CurveYPosition.Evaluate(Random.value),
            //                       CurveZPosition.Evaluate(Random.value));

            // var p = Instantiate(Prefab, transform.position + pos * 10, Quaternion.identity, transform);
            // var scale = SizeDistribution.Evaluate(Random.value);
            // p.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
    
    void OnDisable()
    {
        
    }
    
    void Update() 
    {
        
    }
}
