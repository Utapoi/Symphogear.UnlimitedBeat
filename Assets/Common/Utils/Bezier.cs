using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Symphogear.Common.Utils
{
    // Implementation from: https://faramira.com/implement-bezier-curve-using-csharp-in-unity/

    public class Bezier
    {
        private static readonly float[] Factorial = new float[]
        {
                1.0f,
                1.0f,
                2.0f,
                6.0f,
                24.0f,
                120.0f,
                720.0f,
                5040.0f,
                40320.0f,
                362880.0f,
                3628800.0f,
                39916800.0f,
                479001600.0f,
                6227020800.0f,
                87178291200.0f,
                1307674368000.0f,
                20922789888000.0f,
        };

        public static Vector3 Curve(float t, List<Vector3> controlPoints)
        {
            var n = controlPoints.Count - 1;

            if (n > 16)
            {
                Debug.Log("You have used more than 16 control points. The maximum control points allowed is 16.");
                controlPoints.RemoveRange(16, controlPoints.Count - 16);
            }

            if (t <= 0)
                return controlPoints[0];
            if (t >= 1)
                return controlPoints[^1];

            Vector3 p = new();

            for (var i = 0; i < controlPoints.Count; ++i)
            {
                var bn = Bernstein(n, i, t) * controlPoints[i];
                p += bn;
            }

            return p;
        }

        public static List<Vector3> Curve(List<Vector3> controlPoints, float interval = 0.01f)
        {
            var n = controlPoints.Count - 1;

            if (n > 16)
            {
                Debug.Log("You have used more than 16 control points. " +"The maximum control points allowed is 16.");
                controlPoints.RemoveRange(16, controlPoints.Count - 16);
            }

            List<Vector3> points = new();

            for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
            {
                Vector3 p = new();

                for (var i = 0; i < controlPoints.Count; ++i)
                {
                    var bn = Bernstein(n, i, t) * controlPoints[i];
                    p += bn;
                }

                points.Add(p);
            }

            return points;
        }

        private static float Binomial(int n, int i)
        {
            var a1 = Factorial[n];
            var a2 = Factorial[i];
            var a3 = Factorial[n - i];
         
            return  a1 / (a2 * a3);
        }

        private static float Bernstein(int n, int i, float t)
        {
            var t_i = Mathf.Pow(t, i);
            var t_n_minus_i = Mathf.Pow(1 - t, n - i);

            return Binomial(n, i) * t_i * t_n_minus_i;
        }
    }
}
