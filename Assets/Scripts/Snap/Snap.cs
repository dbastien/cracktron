using UnityEngine;

public class Snap : MonoBehaviour
{
    public Vector3 Position;

    public void Start()
    {
        this.Position = this.transform.position;
    }

    public void OnMouseDrag()
    {
        var screenZ = Camera.main.WorldToScreenPoint(this.Position).z;

        screenZ += Input.mouseScrollDelta.y * 0.15f;

        this.Position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenZ));

        this.transform.position = SnapManager.Instance.GetSnapPoint(this);
    }

    //relative to transform position
    public Vector3[] GetSnapPoints()
    {
        //TODO: requirecomponent? cache?
        var boxCollider = this.GetComponent<BoxCollider>();
        return boxCollider != null ? new Vector3[] { boxCollider.bounds.min - this.transform.position, boxCollider.bounds.max - this.transform.position } : new Vector3[] { Vector3.zero };
    }
}
