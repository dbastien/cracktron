using UnityEngine;
using System.Collections.Generic;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    public float GridLineLength = 2.0f;
    public float GridLineWidth = 0.02f;

    public List<SnapTarget> SnapTargets;

    public float SnapDistance = 0.1f;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances created");
        }

        Instance = this;
    }


    void Start()
    {
    }

    void Update()
    {
    }

    public Vector3 GetSnapPoint(Snap snap)
    {
        var snapPoints = snap.GetSnapPoints();

        if (SnapTargets == null || snapPoints.Length <= 0 || SnapTargets.Count <= 0)
        {
            return snap.Position;
        }

        Vector3 bestDistance = new Vector3(SnapDistance, SnapDistance, SnapDistance);
        Vector3 snappedPosition = snap.Position;

        for (int snapPointIndex = 0; snapPointIndex < snapPoints.Length; ++snapPointIndex)
        {
            var snapPoint = snapPoints[snapPointIndex];

            foreach (var snapTarget in SnapTargets)
            {
                if (snapTarget.gameObject == snap.gameObject)
                {
                    continue;
                }

                var snapTargetPoints = snapTarget.GetSnapPoints();

                foreach (var snapTargetPoint in snapTargetPoints)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        float distance = Mathf.Abs((snapPoint[i] + snap.Position[i]) - snapTargetPoint[i]);

                        if (distance < bestDistance[i])
                        {
                            snappedPosition[i] = snapTargetPoint[i] - snapPoint[i];
                            bestDistance[i] = distance;
                        }
                    }
                }
            }
        }

        return snappedPosition;
    }
}
