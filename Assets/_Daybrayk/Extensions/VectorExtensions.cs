using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
    public static class VectorExtensions
    {
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            vector.x = x ?? vector.x;
            vector.y = y ?? vector.y;
            vector.z = z ?? vector.z;

            return vector;
        }
    }
}
