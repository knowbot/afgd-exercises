// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace AfGD
// {
//     [ExecuteInEditMode]
//     public class DebugCurve : MonoBehaviour
//     {
//
//         // TODO exercise 2.3
//         // you will want to have more than one CurveSegment when creating a cyclic path
//         // you can consider a List<CurveSegment>. 
//         // You may also want to add more control points, and "lock" the CurveType, since 
//         // different curve types make curves in different ranges 
//         // (e.g. Catmull-rom and B-spline make a curve from cp2 to cp3, Hermite and Bezier from cp1 to cp4)
//         CurveSegment curve;
//         // must be assigned in the inspector
//         [Tooltip("curve control points/vectors")]
//         public Transform cp1, cp2, cp3, cp4;
//         [Tooltip("Set the curve type")]
//         public CurveType curveType = CurveType.BEZIER;
//
//
//         // these variables are only used for visualization
//         [Header("Debug varaibles")]
//         [Range(2, 100)]
//         public int debugSegments = 20;
//         public bool drawPath = true;
//         public Color pathColor = Color.magenta;
//         public bool drawTangents = true;
//         public Color tangentColor = Color.green;
//
//
//         bool Init()
//         {
//             // initialize curve if all control points are valid
//             if (cp1 == null || cp2 == null || cp3 == null || cp4 == null)
//                 return false;
//             curve = new CurveSegment(cp1.position, cp2.position, cp3.position, cp4.position, curveType);
//             return true;
//         }
//
//
//
//         public static void DrawCurveSegments(CurveSegment curve,
//             Color color, int segments = 50)
//         {
//             // TODO exercise 2.2
//             // evaluate the curve from start to end (range [0, 1])
//             // and you draw a number of line segments between 
//             // consecutive points
//             
//
//         }
//
//         public static void DrawTangents(CurveSegment curve,
//             Color color, int segments = 50, float scale = 0.1f)
//         {
//             // TODO exercise 2.2
//             // evaluate the curve and tangent from start to end (range [0, 1])
//             // and draw the tangent as a line from the current curve point
//             // to the current point + the tangent vector 
//
//         }
//
//         // Start is called before the first frame update
//         void Start()
//         {
//             Init();
//         }
//
//         // Update is called once per frame
//         void Update()
//         {
//             if (Application.isEditor)
//             {
//                 // reinitialize if we change somethign while not playing
//                 // this is here so we can update the debug draw of the curve
//                 // while in edit mode
//                 if (!Init())
//                     return;
//             }
//
//             if(curveType == CurveType.HERMITE)
//             {
//                 // Hermite spline has control vectors besides start and end points
//                 Debug.DrawLine(cp1.position, cp2.position);
//                 Debug.DrawLine(cp4.position, cp3.position);
//             }
//             else
//             {
//                 // line connecting control points
//                 Debug.DrawLine(cp1.position, cp2.position);
//                 Debug.DrawLine(cp2.position, cp3.position);
//                 Debug.DrawLine(cp3.position, cp4.position);
//             }
//
//             // draw the debug shapes
//             if (drawPath)
//                 DrawCurveSegments(curve, pathColor, debugSegments);
//             if (drawTangents)
//                 DrawTangents(curve, tangentColor, debugSegments);
//
//         }
//     }
// }