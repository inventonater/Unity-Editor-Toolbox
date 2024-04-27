using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides extension methods for comparing and manipulating float and vector values.
    /// </summary>
    public static class FloatComparisonExtensions
    {
        /// <summary>
        /// The default threshold value used for float comparisons.
        /// </summary>
        public const float DefaultThreshold = 0.0001f;

        /// <summary>
        /// Forces each component of a Vector3 to be non-zero by setting components below the minimum size to the minimum size.
        /// </summary>
        /// <param name="val">The Vector3 to force non-zero components on.</param>
        /// <param name="minComponentSize">The minimum size for each component. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>A new Vector3 with components forced to be non-zero.</returns>
        public static Vector3 ForceNonZero(this Vector3 val, float minComponentSize = DefaultThreshold)
        {
            return Vector3.Max(val, Vector3.one * minComponentSize);
        }

        /// <summary>
        /// Sets a Vector3 to zero if its magnitude is near zero based on the specified threshold.
        /// </summary>
        /// <param name="val">The Vector3 to check and potentially set to zero.</param>
        /// <param name="threshold">The threshold for considering the magnitude as near zero. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>Vector3.zero if the magnitude is near zero, otherwise the original Vector3.</returns>
        public static Vector3 ZeroIfNearZero(this Vector3 val, float threshold = DefaultThreshold)
        {
            return val.IsNearZero(threshold) ? Vector3.zero : val;
        }

        /// <summary>
        /// Sets a float value to zero if it is near zero based on the specified threshold.
        /// </summary>
        /// <param name="val">The float value to check and potentially set to zero.</param>
        /// <param name="threshold">The threshold for considering the value as near zero. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>0 if the value is near zero, otherwise the original value.</returns>
        public static float ZeroIfNearZero(this float val, float threshold = DefaultThreshold)
        {
            return val.IsNearZero(threshold) ? 0 : val;
        }

        /// <summary>
        /// Determines if two float values have the same sign (positive, negative, or zero).
        /// </summary>
        /// <param name="val">The first float value.</param>
        /// <param name="other">The second float value to compare against.</param>
        /// <returns>True if both values have the same sign, false otherwise.</returns>
        public static bool IsSameSign(this float val, float other)
        {
            return Mathf.Sign(val) == Mathf.Sign(other);
        }

        /// <summary>
        /// Determines if a Vector3 is near zero based on the specified threshold.
        /// </summary>
        /// <param name="val">The Vector3 to check.</param>
        /// <param name="threshold">The threshold for considering the magnitude as near zero. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if the Vector3's magnitude is near zero, false otherwise.</returns>
        public static bool IsNearZero(this Vector3 val, float threshold = DefaultThreshold)
        {
            return val.magnitude.IsNearZero(threshold);
        }

        /// <summary>
        /// Determines if a float value is near zero based on the specified threshold.
        /// </summary>
        /// <param name="val">The float value to check.</param>
        /// <param name="threshold">The threshold for considering the value as near zero. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if the value is near zero, false otherwise.</returns>
        public static bool IsNearZero(this float val, float threshold = DefaultThreshold)
        {
            return val.IsApproximately(0, threshold);
        }

        /// <summary>
        /// Determines if two float values are approximately equal based on the specified threshold.
        /// </summary>
        /// <param name="val">The first float value.</param>
        /// <param name="other">The second float value to compare against.</param>
        /// <param name="threshold">The threshold for considering the values as approximately equal. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if the absolute difference between the values is less than the threshold, false otherwise.</returns>
        public static bool IsApproximately(this float val, float other, float threshold = DefaultThreshold)
        {
            return Mathf.Abs(val - other) < threshold;
        }

        /// <summary>
        /// Determines if two float values are not approximately equal based on the specified threshold.
        /// </summary>
        /// <param name="val">The first float value.</param>
        /// <param name="other">The second float value to compare against.</param>
        /// <param name="threshold">The threshold for considering the values as not approximately equal. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if the absolute difference between the values is greater than or equal to the threshold, false otherwise.</returns>
        public static bool IsNotApproximately(this float val, float other, float threshold = DefaultThreshold)
        {
            return !val.IsApproximately(other, threshold);
        }

        /// <summary>
        /// Determines if two Vector3 values are not approximately equal based on the specified threshold.
        /// </summary>
        /// <param name="val">The first Vector3 value.</param>
        /// <param name="other">The second Vector3 value to compare against.</param>
        /// <param name="threshold">The threshold for considering the values as not approximately equal. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if any component of the vectors is not approximately equal, false otherwise.</returns>
        public static bool IsNotApproximately(this Vector3 val, Vector3 other, float threshold = DefaultThreshold)
        {
            return !val.IsApproximately(other, threshold);
        }

        /// <summary>
        /// Determines if two Vector2 values are approximately equal based on the specified threshold.
        /// </summary>
        /// <param name="val">The first Vector2 value.</param>
        /// <param name="other">The second Vector2 value to compare against.</param>
        /// <param name="threshold">The threshold for considering the values as approximately equal. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if both components of the vectors are approximately equal, false otherwise.</returns>
        public static bool IsApproximately(this Vector2 val, Vector2 other, float threshold = DefaultThreshold)
        {
            return val.x.IsApproximately(other.x, threshold) && val.y.IsApproximately(other.y, threshold);
        }

        /// <summary>
        /// Determines if two Vector3 values are approximately equal based on the specified threshold.
        /// </summary>
        /// <param name="val">The first Vector3 value.</param>
        /// <param name="other">The second Vector3 value to compare against.</param>
        /// <param name="threshold">The threshold for considering the values as approximately equal. Defaults to <see cref="DefaultThreshold"/>.</param>
        /// <returns>True if all components of the vectors are approximately equal, false otherwise.</returns>
        public static bool IsApproximately(this Vector3 val, Vector3 other, float threshold = DefaultThreshold)
        {
            return val.x.IsApproximately(other.x, threshold) &&
                   val.y.IsApproximately(other.y, threshold) &&
                   val.z.IsApproximately(other.z, threshold);
        }

        /// <summary>
        /// Applies a deadzone to a float value based on the specified threshold.
        /// </summary>
        /// <param name="value">The float value to apply the deadzone to.</param>
        /// <param name="threshold">The threshold for the deadzone.</param>
        /// <returns>The value with the deadzone applied.</returns>
        public static float WithDeadzone(this float value, float threshold)
        {
            return value > 0 ? Mathf.Max(0, value - threshold) : Mathf.Min(0, value + threshold);
        }

        /// <summary>
        /// Applies a deadzone to a Vector3 value based on the specified threshold.
        /// </summary>
        /// <param name="value">The Vector3 value to apply the deadzone to.</param>
        /// <param name="threshold">The threshold for the deadzone.</param>
        /// <returns>The Vector3 with the deadzone applied.</returns>
        public static Vector3 WithDeadzone(this Vector3 value, float threshold)
        {
            float magnitude = value.magnitude;
            return magnitude > threshold ? (magnitude - threshold) * value.normalized : Vector3.zero;
        }

        /// <summary>
        /// Determines if a Vector3 value exceeds the specified deadzone threshold and returns the result.
        /// </summary>
        /// <param name="value">The Vector3 value to check.</param>
        /// <param name="threshold">The threshold for the deadzone.</param>
        /// <param name="result">The resulting Vector3 with the deadzone applied.</param>
        /// <returns>True if the magnitude of the Vector3 exceeds the threshold, false otherwise.</returns>
        public static bool IsDeadzoneExceeded(this Vector3 value, float threshold, out Vector3 result)
        {
            result = value.WithDeadzone(threshold);
            return result != Vector3.zero;
        }
    }
}