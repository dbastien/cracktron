using UnityEngine;

public class Snap : MonoBehaviour
{
    public Vector3 Position;

    void Start()
    {
        Position = transform.position;
    }

    void Update()
    {
    }

    void OnMouseDrag()
    {
        float screenZ = Camera.main.WorldToScreenPoint(Position).z;

        screenZ += Input.mouseScrollDelta.y * 0.15f;

        Position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenZ));

        transform.position = SnapManager.Instance.GetSnapPoint(this);
    }

    //relative to transform position
    public Vector3[] GetSnapPoints()
    {
        //TODO: requirecomponent? cache?
        var boxCollider = this.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            return new Vector3[] { boxCollider.bounds.min - this.transform.position, boxCollider.bounds.max - this.transform.position };
        }

        return new Vector3[] { Vector3.zero };
    }
}
