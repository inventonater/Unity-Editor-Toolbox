using UnityEngine;

namespace Toolbox
{
    public static class FloatComparisonExtensions
    {
        public const float FloatCompareThreshold = 0.0001f;
        public static Vector3 ForceNonZero(this Vector3 val, float minComponentSize = FloatCompareThreshold) => Vector3.Max(val, Vector3.one * minComponentSize);
        public static Vector3 ZeroIfNearZero(this Vector3 val, float threshold = FloatCompareThreshold) => val.IsNearZero(threshold) ? Vector3.zero : val;
        public static float ZeroIfNearZero(this float val, float threshold = FloatCompareThreshold) => val.IsNearZero(threshold) ? 0 : val;
        public static bool IsSameSign(this float val, float other) => Mathf.Sign(val) == Mathf.Sign(other);
        public static bool IsNearZero(this Vector3 val, float threshold = FloatCompareThreshold) => val.magnitude.IsNearZero(threshold);
        public static bool IsNearZero(this float val, float threshold = FloatCompareThreshold) => val.IsApproximately(0, threshold);

        public static bool IsApproximately(this float val, float other, float threshold = FloatCompareThreshold) => Mathf.Abs(val - other) < threshold;
        public static bool IsNotApproximately(this float val, float other, float threshold = FloatCompareThreshold) => !val.IsApproximately(other, threshold);
        public static bool IsNotApproximately(this Vector3 val, Vector3 other, float threshold = FloatCompareThreshold) => !val.IsApproximately(other, threshold);

        public static bool IsApproximately(this Vector2 val, Vector2 other, float threshold = FloatCompareThreshold) => val.x.IsApproximately(other.x) && val.y.IsApproximately(other.y);
        public static bool IsApproximately(this Vector3 val, Vector3 other, float threshold = FloatCompareThreshold) => val.x.IsApproximately(other.x) && val.y.IsApproximately(other.y) && val.z.IsApproximately(other.z);

        public static float WithDeadzone(this float value, float threshold)
        {
            return value > 0 ? Mathf.Max(0, value - threshold) : Mathf.Min(0, value + threshold);
        }
        // public static Vector3 WithDeadzone(this Vector3 value, float threshold) => Mathf.Max(0, value.magnitude - threshold) * value.normalized;
        // public static bool IsDeadzoneExceeded(this Vector3 value, float threshold, out Vector3 result)
        // {
            // result = value.WithDeadzone(threshold);
            // return result != default;
        // }
    }
}