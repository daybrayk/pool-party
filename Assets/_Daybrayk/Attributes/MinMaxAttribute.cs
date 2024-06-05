using UnityEngine;

namespace Daybrayk

{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public MinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public MinMaxAttribute() { min = 0; max = 1; }
    }
}
