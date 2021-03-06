using System;
using System.Collections.Generic;
using System.Numerics;
using AfGD.Assignment1;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using Vector3 = UnityEngine.Vector3;

namespace AfGD
{
    public class SplinePath : MonoBehaviour
    {
        private List<Vector3> path = new List<Vector3>();

        // we get the control points directly from the pathfinding object
        [Tooltip("Path holder")]
        [SerializeField] private PathFinding pathfinder;
        
        [Tooltip("Set the curve type")]
        [SerializeField] private CurveType curveType = CurveType.HERMITE;

        // we will need an array of curves for the path (instead of a single curve)
        public CurveSegment[] curves;
        
        [Range(0.0f, 1.0f)]
        [SerializeField] private float tightness;
        
        // stuff to be used for moving along the path
        // first off we need the normalized arclength "table"
        private float[] _normArcLengths;
         // then we need a parameter to decide how precise the approximation should be (aka how many entries in teh table)
         [Tooltip("Arc-length approximation precision")]
         [Range(2, 1000)]
         [SerializeField] private int arcLengthEntries = 200;
        

        // these variables are only used for visualization
        [Header("Debug variables")]
        [Range(2, 100)]
        public int debugSegments = 20;
        public bool drawPath = true;
        public Color pathColor = Color.magenta;
        public bool drawTangents = true;
        public Color tangentColor = Color.green;
        
        void Start()
        {
            Init();
            _normArcLengths = new float[arcLengthEntries];
            _normArcLengths[0] = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isEditor)
            {
                // reinitialize if we change something while not playing
                if (!Init())
                    return;
            }
            

            foreach (CurveSegment t in curves)
            {
                if (drawPath)
                    DrawCurveSegments(t, pathColor, debugSegments);
                if (drawTangents)
                    DrawTangents(t, tangentColor, debugSegments);
            }
        }

        public bool Init()
        {
            if (!pathfinder)
            {
                Debug.LogError("No pathfinder assigned to the curve!");
                return false;
            }
            
            path = pathfinder.m_Path;
            int points = path.Count;
            if (curves == null || curves.Length != points-1)
                curves = (points == 0) ? new CurveSegment[0] : new CurveSegment[points-1];

            // instantiate a curve segment for each valid sequence of points
            for (int i = 0; i < points - 1; i++)
            {
                Vector3 cp1 = Vector3.zero, cp2 = Vector3.zero , cp3 = Vector3.zero, cp4 = Vector3.zero;
                cp1 = path[i];
                cp4 = path[i + 1];
                cp2 = CardinalSplineTangent(cp1);
                cp3 = CardinalSplineTangent(cp4);
                curves[i] = new CurveSegment(cp1, cp2, cp3, cp4, (CurveType)curveType);
            }
            if(curves.Length > 0)
                UpdateArcLengthTable();
            
            foreach (CurveSegment t in curves)
            {
                if (drawPath)
                    DrawCurveSegments(t, pathColor, debugSegments);
                if (drawTangents)
                    DrawTangents(t, tangentColor, debugSegments);
            }
            return true;
        }
        
        // function that returns a point/vector along the spline
        public Vector3 Evaluate(float s, bool parametrizeByArcLength = false, int derivative = 0)
        {
            if ((curves == null && !Init()) || curves.Length == 0)
                 return Vector3.zero;
            
            // do we want to use arc-length parametrization?
            float u = parametrizeByArcLength ? ArcLengthParametrization(s) : s;

            // scale 'u' from [0,1] to [0, curves.Length]
            float pathU = u * curves.Length;
            // round down pathU to retrieve the curve segment ID
            int curveID = (int)pathU;
            // ensure that the curveID is in a valid range
            curveID = Mathf.Clamp(curveID, 0, curves.Length - 1);
            // 'u' in the selected curve segment
            float curveU = pathU - curveID;
            
            if (derivative == 1)
                return curves[curveID].EvaluateDv(curveU);
            if (derivative == 2)
                return curves[curveID].EvaluateDv2(curveU);

            Debug.Log("Onto curve " + curveID + " out of " + curves.Length);
            return curves[curveID].Evaluate(curveU);
        }

        private void UpdateArcLengthTable()
        {
            // calc intervals at which calculate arc-length
            float interval = 1.0f / arcLengthEntries;
            
            // reinit table
            _normArcLengths = new float[arcLengthEntries];
            _normArcLengths[0] = 0.0f;

            // get starting position on path
            Vector3 currPos = Evaluate(0);
            
            // build the table
            for (int i = 1; i < arcLengthEntries; i++)
            {
                float u = interval * i; // get current interval to examine
                Vector3 newPos = Evaluate(u); // evaluate new position at interval
                // compute cumulative distance
                _normArcLengths[i] = _normArcLengths[i - 1] + Vector3.Distance(currPos, newPos);
                // update current pos along path
                currPos = newPos;
            }
            
            // time to normalize all the distances!
            float pathArcLength = _normArcLengths[arcLengthEntries - 1];
            // avoid possible division by 0
            Assert.IsTrue(pathArcLength > float.Epsilon);
            // normalize
            for (int i = 1; i < arcLengthEntries; i++)
                _normArcLengths[i] /= pathArcLength;
        }


        // map s to u according to the arc-length table
        private float ArcLengthParametrization(float s)
        {
            // check s range
            Mathf.Clamp(s, 0.0f, 1.0f);
            
            // find the entry closest to s with a binary search
            int min = 0;
            int max = arcLengthEntries - 1;
            int curr = max / 2;
            while (true)
            {
                // if min and max are neighbours, we found the approximation
                if (min == max - 1)
                {
                    float sMin = _normArcLengths[min];
                    float sMax = _normArcLengths[max];
                    float uMin = (float)min / (arcLengthEntries - 1);
                    float uMax = (float)max / (arcLengthEntries - 1);
                    // interpolate between the two entries to approximate for s
                    float ds = (s - sMin) / (sMax - sMin);
                    return Mathf.Lerp(uMin, uMax, ds);
                }

                // rescale search bounds
                if (s > _normArcLengths[curr])
                    min = curr;
                else
                    max = curr;
                curr = min + (max - min) / 2;
            }

        }

        public int GetCurveCount()
        {
            return curves.Length;
        }

        public List<Vector3> GetPointsOnCurveSegment(CurveSegment curve, int segments = 50)
        {
            List<Vector3> pointsOnCurveSegment = new List<Vector3>();
            float interval = 1.0f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float start_u = i * interval;

                Vector3 startPoint = curve.Evaluate(start_u);
                pointsOnCurveSegment.Add(startPoint);
            }
            return pointsOnCurveSegment;
        }


        public Vector3 GetSinglePointOnCurveSegment(CurveSegment curve, float t)
        {
            t = Mathf.Clamp01(t);
            return curve.Evaluate(t);
        }

        public Vector3 GetDirection(CurveSegment[] curves, float progress)
        {
            int curveIndex = 0;
            float t = 0;
            if (curves.Length > 0)
            {
                curveIndex = (int)progress % curves.Length;
                t = progress - curveIndex;
            }
            return curves[curveIndex].EvaluateDv(t);
        }

        public Vector3 GetPoint(float progress)
        {
            int curveIndex = 0;
            float t = 0;
            if (curves.Length > 0)
            {
                curveIndex = (int)progress % curves.Length;
                t = progress - curveIndex;
            }
            return GetSinglePointOnCurveSegment(curves[curveIndex], t);
        }

        private static void DrawCurveSegments(CurveSegment curve,
            Color color, int segments = 50)
        {
            float interval = 1.0f / segments;
            Vector3 lastPos = curve.Evaluate(0);
            for (int i = 1; i <= segments; i++)
            {
                float u = interval * (float)i;
                Vector3 pos = curve.Evaluate(u);

                UnityEngine.Debug.DrawLine(lastPos, pos, color);
                lastPos = pos;
            }
        }

        private static void DrawTangents(CurveSegment curve,
            Color color, int segments = 50, float scale = 0.1f)
        {
            float interval = 1.0f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float u = interval * (float)i;
                Vector3 pos = curve.Evaluate(u);
                Vector3 tangent = curve.EvaluateDv(u);

                UnityEngine.Debug.DrawLine(pos, pos + tangent * scale, color);
            }
        }
        
        // private Vector3 FiniteDifferenceTangent(Vector3 cp)
        // {
        //     Vector3 tangent = new Vector3();
        //     if (path.Contains(cp))
        //     {
        //         int index = path.IndexOf(cp);
        //         if (index == 0 && path.Count > 1)
        //         {
        //             tangent = 0.5f * (path[1] - path[index]) / (path[1].x - path[index].x);
        //         }  
        //         else if (index == path.Count - 1) 
        //         {
        //             tangent = 0.5f * (path[index] - path[index - 1]) / (path[index].x - path[index - 1].x);
        //         }
        //         else
        //         {
        //             tangent = 0.5f * ((path[1] - path[index]) / (path[1].x - path[index].x) + (path[index] - path[index - 1]) / (path[index].x - path[index - 1].x));
        //         }
        //     }
        //     return tangent;
        // }

        private Vector3 CardinalSplineTangent(Vector3 cp)
        {
            Vector3 tangent = new Vector3();
            if (path.Contains(cp))
            {
                Matrix3x2 counterClockwise2D = new Matrix3x2(1, 0, 0, 0, -1, 0);
                int index = path.IndexOf(cp);
                if (index == 0 && path.Count > 1)
                {
                    tangent = path[1] - path[index];
                }  
                else if (index == path.Count - 1) 
                {
                    tangent = path[index] - path[index - 1];
                }
                else
                {
                    tangent = tightness *
                              (path[index + 1] - path[index - 1]);
                }
            }
            // return tangent.normalized;
            return tangent;
        }

    }
}