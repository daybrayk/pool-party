using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    public static class MathUtil
    {
        public static Vector2 QuadraticEquation(float a, float b, float c)
        {
            float x1;
            float x2;
            float radicand = Mathf.Pow(b, 2) - (4 * a * c);

            if (radicand < 0) return Vector2.negativeInfinity;

            float rs = Mathf.Sqrt(radicand);

            x1 = ((-b) + rs) / 2 * a;
            x2 = ((-b) - rs) / 2 * a;

            return new Vector2(x1, x2);
        }

        #region Trigonometry
        public static float SinLaw(float a, float b, float B)
        {
            return Mathf.Asin(a * Mathf.Sin(B) / b);
        }

        public static float CosinLaw(float a, float b, float theta)
        {
            return Mathf.Pow(a, 2) * Mathf.Pow(b, 2) - 2 * a * b * Mathf.Cos(theta);
        }
        #endregion

        #region Bezier
        public static Vector3 LinearBezierPoint(float t, Vector3 p0, Vector3 p1)
        {
            t = Mathf.Clamp(t, 0, 1);

            float u = 1 - t;

            return u * p0 + t * p1;
        }

        public static Vector2 QuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            t = Mathf.Clamp(t, 0, 1);

            float u = 1 - t;
            float tt = t * t;
            float u2 = u * u;

            Vector3 p = u2 * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        public static Vector3 CubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            t = Mathf.Clamp(t, 0, 1);

            float u = 1 - t;
            float u2 = u * u;
            float u3 = u * u * u;
            float t2 = t * t;
            float t3 = t * t * t;

            Vector3 p = u3 * p0;
            p += 3 * u2 * t * p1;
            p += 3 * u * t2 * p2;
            p += t3 * p3;

            return p;
        }
        #endregion
    }
}
