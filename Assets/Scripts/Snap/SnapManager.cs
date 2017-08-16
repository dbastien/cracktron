using System.Collections.Generic;
using UnityEngine;

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    public float GridLineLength = 2.0f;
    public float GridLineWidth = 0.02f;

    public List<SnapTarget> SnapTargets;

    public float SnapDistance = 0.1f;

    public void Awake()
    {
        if (SnapManager.Instance != null)
        {
            Debug.LogError("Multiple instances created");
        }

        SnapManager.Instance = this;
    }

    public Vector3 GetSnapPoint(Snap snap)
    {
        var snapPoints = snap.GetSnapPoints();

        if (this.SnapTargets == null || snapPoints.Length <= 0 || this.SnapTargets.Count <= 0)
        {
            return snap.Position;
        }

        Vector3 bestDistance = new Vector3(this.SnapDistance, this.SnapDistance, this.SnapDistance);
        Vector3 snappedPosition = snap.Position;

        for (int snapPointIndex = 0; snapPointIndex < snapPoints.Length; ++snapPointIndex)
        {
            var snapPoint = snapPoints[snapPointIndex];

            foreach (var snapTarget in this.SnapTargets)
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
