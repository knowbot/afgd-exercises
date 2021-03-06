// Implementer bu Julia Beryl based on the GDC talk: 
// Math for Game Programmers: Fast and Funky 1D Nonlinear Transformations
// https://youtu.be/mr5xkf6zSzk
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD
{
    public static class EasingFunctions
    {

        //--------------------
        // starting functions
        //--------------------
        public static float Linear(float t)
        {
            return t;
        }

        public static float SmoothStart2(float t)
        {
            return t * t;
        }

        public static float SmoothStart3(float t)
        {
            return t * t * t;
        }

        public static float SmoothStart4(float t)
        {
            return t * t * t * t;
        }

        public static float SmoothStart5(float t)
        {
            return t * t * t * t * t;
        }

        //--------------------
        // stopping functions
        //--------------------
        public static float SmoothStop2(float t)
        {
            return 1 - (1 - t) * (1 - t);
        }

        public static float SmoothStop3(float t)
        {
            return 1 - (1 - t) * (1 - t) * (1 - t);
        }

        public static float SmoothStop4(float t)
        {
            return 1 - (1 - t) * (1 - t) * (1 - t) * (1 - t);
        }

        public static float SmoothStop5(float t)
        {
            return 1 - (1 - t) * (1 - t) * (1 - t) * (1 - t) * (1 - t);
        }

        //--------------------
        // mixing functions
        //--------------------
        public static float Mix(System.Func<float, float> a, System.Func<float, float> b, float weightB, float t)
        {
            return a(t) + weightB * (b(t) - a(t));
        }

        // can be used to achieve smoothstep (combine smoothstart and smooth stop)
        public static float Crossfade(System.Func<float, float> a, System.Func<float, float> b, float weightB, float t)
        {
            return a(t) + t * (b(t) - a(t));
        }

        public static float Scale(System.Func<float, float> f, float t)
        {
            return t * f(t);
        }

        public static float ReverseScale(System.Func<float, float> f, float t)
        {
            return (1 - t) * f(t);
        }


        //--------------------
        // arching functions
        //--------------------
        public static float Arch2(float t)
        {
            return t * (1 - t);
        }

        public static float SmoothStartArch3(float t)
        {
            return t * t * (1 - t);
        }

        public static float SmoothStoptArch3(float t)
        {
            return t * (1 - t) * (1 - t);
        }

        public static float SmoothStepArch4(float t)
        {
            return (1 - t) * t * t * (1 - t);
        }

        public static float BellCurve6(float t)
        {
            return SmoothStop3(t) * SmoothStart3(t);
        }

        // cubic Bezier through a, b, c, d where a and d are assumed to be 0 and 1 respectively
        public static float NormalizedBezier3(float b, float c, float t)
        {
            float a = 0.0f;
            float d = 1.0f;
            Vector4 B = new Vector4(a, b, c, d);

            Matrix4x4 M = new Matrix4x4(
                new Vector4(-1, 3, -3, 1),
                new Vector4(3, -6, 3, 0),
                new Vector4(-3, 3, 0, 0),
                new Vector4(1, 0, 0, 0));

            Vector4 T = new Vector4(t * t * t, t * t, t, 1);
            return Vector4.Dot(T, M * B);
        }

        public static float BounceClampBottom(float t) {
            return Mathf.Abs(t);
        }
        public static float BounceClampTop(float t)
        {
            return 1.0f - Mathf.Abs(1.0f - t);
        }
        public static float BounceClampBottomTop(float t)
        {
            return BounceClampTop(BounceClampBottom(t));
        }
    }

}