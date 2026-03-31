using UnityEngine;

namespace Framework.Mathematics
{
    public static class Mathematics
    {
        /// <summary>
        /// Maps a value given from `a` space to `b`.
        /// </summary>
        public static float MapValue(this float x, float a0, float a1, float b0, float b1)
        {
            var divisor = a1 - a0;
            
            if (Mathf.Approximately(divisor, 0f))
            {
                // Division by 0
                Debug.LogError("Division by 0. Returning fallback.");
                return GetFallback();
            }
        
            var value = b0 + (b1 - b0) * ((x - a0) / divisor);
            var clampedValue = Mathf.Clamp(value, Mathf.Min(b0, b1), Mathf.Max(b0, b1));

            if (float.IsNaN(clampedValue) || float.IsInfinity(clampedValue))
            {
                // Math failed
                Debug.LogError("Math failed, returning fallback.");
                return GetFallback();
            }
            else
            {
                return clampedValue;
            }

            float GetFallback()
            {
                var fallback = Mathf.Min(b0, b1);

                if (float.IsNaN(fallback) || float.IsInfinity(fallback))
                {
                    Debug.LogError("Fallback is not valid, returning original value.");
                    fallback = x;
                }

                return fallback;
            }
        }
        
        public static float Snap(this float value, float multiple, SnapType snapType = SnapType.Floor)
        {
            if (multiple == 0)
                return value;

            if (snapType == SnapType.Floor)
            {
                return Mathf.Floor(value / multiple) * multiple;
            }
            else
            {
                return Mathf.Round(value / multiple) * multiple;
            }
        }
        
        public enum SnapType
        {
            Floor,
            Round
        }
    }
}