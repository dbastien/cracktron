using UnityEngine;

public class SimpleFirstPersonController : MonoBehaviour 
{
    public Vector2 Speed = Vector2.one;

    void Awake()
    {
        
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
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        var input = new Vector2(h, v);
        var scaledInput = Vector2.Scale(input, Speed) * Time.deltaTime;

        this.transform.position += new Vector3(scaledInput.x, 0.0f, scaledInput.y);
    }
}
