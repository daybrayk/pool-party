using UnityEngine;

namespace Daybrayk
{
    public static class TransformExtensions
    {
        public static Vector3 DirectionTo(this Transform source, Transform destination)
        {
            return (destination.position - source.position).normalized;
        }

        public static float DistanceFrom(this Transform source, Transform destination)
        {
            return Vector3.Distance(destination.position, source.position);
        }

        public static float DistanceFrom(this Transform source, Vector3 destination)
        {
            return Vector3.Distance(destination, source.position);
        }
    }
}
