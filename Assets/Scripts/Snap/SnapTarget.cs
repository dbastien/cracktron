using System.Collections.Generic;
using UnityEngine;

public class SnapTarget : MonoBehaviour
{
    private List<GameObject> lineObjects;

    public void Awake()
    {
        var snapPoints = this.GetSnapPoints();

        lineObjects = new List<GameObject>(snapPoints.Length);

        foreach (var snapPoint in snapPoints)
        {
            for (int d = 0; d < 3; ++d)
            {
                var go = new GameObject();
                go.transform.parent = this.transform;
                go.AddComponent<LineRenderer>();

                var lr = go.GetComponent<LineRenderer>();
                //todo: pull material from snapmanager
                //todo: custom shader that fades lines further away from you
                lr.material = new Material(Shader.Find("Unlit/Color"));
                lr.SetWidth(SnapManager.Instance.GridLineWidth, SnapManager.Instance.GridLineWidth);

                Vector3 offset = Vector3.zero;
                offset[d] = SnapManager.Instance.GridLineLength * 0.5f;
                lr.material.color = new Color(offset.x, offset.y, offset.z, 0.5f);
                Vector3 start = snapPoint - offset;
                Vector3 end = snapPoint + offset;

                lr.SetPosition(0, start);
                lr.SetPosition(1, end);

                lr.useWorldSpace = false;
            }
        }
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void OnEnable()
    {
        SnapManager.Instance.SnapTargets.Add(this);
    }

    void OnDisable()
    {
        SnapManager.Instance.SnapTargets.Remove(this);
    }

    public Vector3[] GetSnapPoints()
    {
        //TODO: requirecomponent? cache?
        var boxCollider = this.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            return new Vector3[] { boxCollider.bounds.min, boxCollider.bounds.max };
        }

        return new Vector3[] { this.transform.position };
    }
}
