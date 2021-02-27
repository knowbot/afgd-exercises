using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DeCasteljau : MonoBehaviour
{
    [Tooltip("curve control points/vectors")]
    public Transform cp1, cp2, cp3, cp4;
    public float segments = 50;

    Vector3 EvaluateAt(float u)
    {
        Vector3 mid1 = Vector3.Lerp(cp1.position, cp2.position, u);
        Vector3 mid2 = Vector3.Lerp(cp2.position, cp3.position, u);
        Vector3 mid3 = Vector3.Lerp(cp3.position, cp4.position, u);

        Vector3 mid1half = Vector3.Lerp(mid1, mid2, u);
        Vector3 mid2half = Vector3.Lerp(mid2, mid3, u);

        return Vector3.Lerp(mid1half, mid2half, u);
    }

    void DebugDrawCurve()
    {
        float interval = 1.0f / segments;
        for(int i = 1; i < segments; i++)
        {
            float start_u = i * interval;
            float end_u = (i+1) * interval;

            Vector3 startPoint = EvaluateAt(start_u);
            Vector3 endPoint = EvaluateAt(end_u);
            Debug.DrawLine(startPoint, endPoint, Color.yellow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cp1 == null || cp2 == null || cp3 == null || cp4 == null)
            return;
        DebugDrawCurve();
    }
}
