using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD
{

    /// <summary>
    /// List the curve types
    /// enum is explicit for readability
    /// </summary>
    public enum CurveType { HERMITE = 0, CATMULLROM = 1, BEZIER = 2, BSPLINE = 3 };

    public static class CurveCoefficients
    {
        /// <summary>
        /// coefficient matrices
        /// </summary>
        private static Matrix4x4[] matrices = {
            // TODO 2.1 create the coefficient matrices used to evaluate points in the curve
            // replace the matrices below with their respective 
            //Matrix4x4.identity, // Hermite
            //Matrix4x4.identity, // CatmullRom
            //Matrix4x4.identity, // Bezier
            //Matrix4x4.identity, // B-spline
                new Matrix4x4(// Hermite
                    new Vector4( 2, 1, 1,-2),  // column 0
                    new Vector4(-3,-2,-1, 3),  // column 1
                    new Vector4( 0, 1, 0, 0),  // column 2
                    new Vector4( 1, 0, 0, 0)), // column 3
                new Matrix4x4(// CatmullRom
                    new Vector4(-1, 3,-3, 1)/2.0f,  // column 0
                    new Vector4( 2,-5, 4,-1)/2.0f,  // column 1
                    new Vector4(-1, 0, 1, 0)/2.0f,  // column 2
                    new Vector4( 0, 2, 0, 0)/2.0f), // column 3
                new Matrix4x4(// Bezier
                    new Vector4(-1, 3,-3, 1),  // column 0
                    new Vector4( 3,-6, 3, 0),  // column 1
                    new Vector4(-3, 3, 0, 0),  // column 2
                    new Vector4( 1, 0, 0, 0)), // column 3
                new Matrix4x4(// B-spline
                    new Vector4(-1, 3,-3, 1)/6.0f,  // column 0
                    new Vector4( 3,-6, 3, 0)/6.0f,  // column 1
                    new Vector4(-3, 0, 3, 0)/6.0f,  // column 2
                    new Vector4( 1, 4, 1, 0)/6.0f), // column 3
                };

        /// <summary>
        /// Return 4x4 coefficient matrix of type CurveType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Matrix4x4 GetCoefficients(CurveType type)
        {
            return matrices[(int)type];
        }
    }



    public class CurveSegment
    {
        /// <summary>
        /// B contains the control parameters (points/vectors) of the curve
        /// </summary>
        public Matrix4x4 B;

        /// <summary>
        /// the M matrix contains the coefficients of the cubic polynomials used in the curve formulation
        /// </summary>
        private Matrix4x4 M;

        private CurveType _type;
        /// <summary>
        /// Type is used to easily update the curve coefficients
        /// </summary>
        public CurveType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                M = CurveCoefficients.GetCoefficients(_type);
            }
        }



        /// <summary>
        /// Curve segment contructor 
        /// </summary>
        /// <param name="cv1">control value 1</param>
        /// <param name="cv2">control value 2</param>
        /// <param name="cv3">control value 3</param>
        /// <param name="cv4">control value 4</param>
        /// <param name="type">curve type</param>
        public CurveSegment(Vector4 cv1, Vector4 cv2, Vector4 cv3, Vector4 cv4, CurveType type = CurveType.HERMITE)
        {
            // curve type, it also set the matrix of coefficients
            this.Type = type;
            B = new Matrix4x4(cv1, cv2, cv3, cv4);
            
            // we work with the matrix shapes:
            //  B     M     U    
            // 4x4   4x4   4x1
        }

        /// <summary>
        /// evaluate curve segment at u, for u in the normalized range [0,1]
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Vector4 Evaluate(float u)
        {
            Vector4 U = new Vector4(u * u * u, u * u, u, 1.0f);
            Vector4 p = B * M * U;
            return p;
        }

        /// <summary>
        /// evaluate tangent of curve segment at u, for u in the normalized range [0,1]
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Vector4 EvaluateDv(float u)
        {
            Vector4 U = new Vector4(u * u * 3, u * 2, 1, 0);
            Vector4 p = B * M * U;
            return p;
        }

        /// <summary>
        /// evaluate curvature of curve segment at u, for u in the normalized range [0,1]
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Vector4 EvaluateDv2(float u)
        {
            Vector4 U = new Vector4(u * 6, 2, 0, 0);
            Vector4 p = B * M * U;
            return p;
        }

    }
}