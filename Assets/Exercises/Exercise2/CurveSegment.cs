// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace AfGD
// {
//
//     /// <summary>
//     /// List the curve types
//     /// enum is explicit for readability
//     /// </summary>
//     public enum CurveType { HERMITE = 0, CATMULLROM = 1, BEZIER = 2, BSPLINE = 3 };
//
//     public static class CurveCoefficients
//     {
//         /// <summary>
//         /// coefficient matrices
//         /// </summary>
//         private static Matrix4x4[] matrices = {
//             // TODO exercise 2.1 
//             // create the coefficient matrices used to evaluate points in the curve
//             // replace the matrices below with their respective coefficients
//             // mind that each Vector4 is actuall a COLUMN in the matrix, and NOT A ROW
//                 new Matrix4x4(// Hermite
//                     new Vector4( 1, 0, 0, 0),  // column 0
//                     new Vector4( 0, 1, 0, 0),  // column 1
//                     new Vector4( 0, 0, 1, 0),  // column 2
//                     new Vector4( 0, 0, 0, 1)), // column 3
//                 new Matrix4x4(// CatmullRom
//                     new Vector4( 1, 0, 0, 0)/2.0f,  // column 0
//                     new Vector4( 0, 1, 0, 0)/2.0f,  // column 1
//                     new Vector4( 0, 0, 1, 0)/2.0f,  // column 2
//                     new Vector4( 0, 0, 0, 1)/2.0f), // column 3
//                 new Matrix4x4(// Bezier
//                     new Vector4( 1, 0, 0, 0),  // column 0
//                     new Vector4( 0, 1, 0, 0),  // column 1
//                     new Vector4( 0, 0, 1, 0),  // column 2
//                     new Vector4( 0, 0, 0, 1)), // column 3
//                 new Matrix4x4(// B-spline
//                     new Vector4( 1, 0, 0, 0)/6.0f,  // column 0
//                     new Vector4( 0, 1, 0, 0)/6.0f,  // column 1
//                     new Vector4( 0, 0, 1, 0)/6.0f,  // column 2
//                     new Vector4( 0, 0, 0, 1)/6.0f), // column 3
//                 };
//
//         /// <summary>
//         /// Return 4x4 coefficient matrix of type CurveType
//         /// </summary>
//         /// <param name="type"></param>
//         /// <returns></returns>
//         public static Matrix4x4 GetCoefficients(CurveType type)
//         {
//             return matrices[(int)type];
//         }
//     }
//
//
//
//     public class CurveSegment
//     {
//         /// <summary>
//         /// B contains the control parameters (points/vectors) of the curve
//         /// </summary>
//         public Matrix4x4 B;
//
//         /// <summary>
//         /// the M matrix contains the coefficients of the cubic polynomials used in the curve formulation
//         /// </summary>
//         private Matrix4x4 M;
//
//         private CurveType _type;
//         /// <summary>
//         /// Type is used to easily update the curve coefficients
//         /// </summary>
//         public CurveType Type
//         {
//             get { return _type; }
//             set
//             {
//                 _type = value;
//                 M = CurveCoefficients.GetCoefficients(_type);
//             }
//         }
//
//
//
//         /// <summary>
//         /// Curve segment contructor 
//         /// </summary>
//         /// <param name="cv1">control value 1</param>
//         /// <param name="cv2">control value 2</param>
//         /// <param name="cv3">control value 3</param>
//         /// <param name="cv4">control value 4</param>
//         /// <param name="type">curve type</param>
//         public CurveSegment(Vector4 cv1, Vector4 cv2, Vector4 cv3, Vector4 cv4, CurveType type = CurveType.BEZIER)
//         {
//             // curve type, it also set the matrix of coefficients
//             this.Type = type;
//             // TODO exercise 2.1 
//             // set control values in matrix B
//             // Our control values are Vector4 (Vector3 position/direction + unnused value)
//             // so we want our B to look like:
//             // B = [ cv1.x cv2.y cv3.z cv4.x]  // row 0
//             //     [ cv1.x cv2.y cv3.z cv4.y]  // row 1
//             //     [ cv1.x cv2.y cv3.z cv4.z]  // row 2
//             //     [   unnused    unnused   ]  // row 3
//             B = Matrix4x4.identity; // replace Matrix4x4.identity
//
//             // we work with the matrix shapes:
//             //  B     M     U    
//             // 4x4   4x4   4x1
//         }
//
//         /// <summary>
//         /// evaluate curve segment at u, for u in the normalized range [0,1]
//         /// </summary>
//         /// <param name="u"></param>
//         /// <returns></returns>
//         public Vector4 Evaluate(float u)
//         {
//             // TODO 2.1 exercise
//             // compute parameter matrix U and evaluate p at u
//             Vector4 U = Vector4.zero; // replace Vector4.zero
//             Vector4 p = Vector4.zero; // replace Vector4.zero
//             return p;
//         }
//
//         /// <summary>
//         /// evaluate tangent of curve segment at u, for u in the normalized range [0,1]
//         /// </summary>
//         /// <param name="u"></param>
//         /// <returns></returns>
//         public Vector4 EvaluateDv(float u)
//         {
//             // TODO 2.1 exercise 
//             // compute parameter matrix U and evaluate p at u
//             // you should compute the first derivative of U
//             Vector4 U = Vector4.zero; // replace Vector4.zero
//             Vector4 p = Vector4.zero; // replace Vector4.zero
//             return p;
//         }
//
//         /// <summary>
//         /// evaluate curvature of curve segment at u, for u in the normalized range [0,1]
//         /// </summary>
//         /// <param name="u"></param>
//         /// <returns></returns>
//         public Vector4 EvaluateDv2(float u)
//         {
//             // TODO 2.1 exercise 
//             // compute parameter matrix U and evaluate p at u
//             // you should compute the second derivative of U
//             Vector4 U = Vector4.zero; // replace Vector4.zero
//             Vector4 p = Vector4.zero; // replace Vector4.zero
//             return p;
//         }
//
//     }
// }