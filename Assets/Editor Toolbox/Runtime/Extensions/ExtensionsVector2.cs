using UnityEngine;

namespace Toolbox
{
    public static class ExtensionsVector2
    {
        /// <summary>
        /// Calculates the inverse of a vector.
        /// </summary>
        /// <param name="v">The vector to calculate the inverse of.</param>
        /// <returns>The inverse of the given vector.</returns>
        public static Vector2 Inverse(this Vector2 v) => new(1.0f / v.x, 1.0f / v.y);

        /// <summary>
        /// Divides a vector by another vector element-wise.
        /// </summary>
        /// <param name="numerator">The vector to be divided.</param>
        /// <param name="denominator">The vector to divide by.</param>
        /// <returns>The resulting vector after element-wise division.</returns>
        public static Vector2 Divide(this Vector2 numerator, Vector2 denominator) => new(numerator.x / denominator.x, numerator.y / denominator.y);

        /// <summary>
        /// Multiplies two Vector2 values element-wise and returns the result.
        /// </summary>
        /// <param name="a">The first Vector2 value to multiply.</param>
        /// <param name="b">The second Vector2 value to multiply.</param>
        /// <returns>A new Vector2 that is the result of multiplying each corresponding component of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static Vector2 Multiply(this Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);

        // Flipping
        /// <summary>
        /// Flips the given vector along the X-axis.
        /// </summary>
        /// <param name="v">The vector to flip.</param>
        /// <returns>The flipped vector.</returns>
        public static Vector2 FlipX(this Vector2 v) => new Vector2(-v.x, v.y);

        /// <summary>
        /// Flips the Y component of a vector.
        /// </summary>
        /// <param name="v">The vector to flip the Y component of.</param>
        /// <returns>The vector with the Y component flipped.</returns>
        public static Vector2 FlipY(this Vector2 v) => new Vector2(v.x, -v.y);

        /// <summary>
        /// Modifies the X component of a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to modify.</param>
        /// <param name="x">The new value for the X component.</param>
        /// <returns>A Vector2 with the X component modified.</returns>
        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

        /// <summary>
        /// Sets the Y component of a Vector2 to a given value.
        /// </summary>
        /// <param name="v">The original Vector2.</param>
        /// <param name="y">The value to set for the Y component.</param>
        /// <returns>A new Vector2 with the updated Y component.</returns>
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        // Swizzling
        /// <summary>
        /// Extracts the XY components of a Vector2 and returns them as a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to extract the XY components from.</param>
        /// <returns>A Vector2 containing the XY components of the given Vector2.</returns>
        public static Vector2 XY(this Vector2 v) => new Vector2(v.x, v.y);

        /// <summary>
        /// Converts a 2D vector to a 3D vector with Z component set to zero.
        /// </summary>
        /// <param name="v">The 2D vector to convert.</param>
        /// <returns>A new Vector3 object with X, Y, and Z components.</returns>
        public static Vector3 XYto3(this Vector2 v) => new Vector3(v.x, v.y, 0);

        /// <summary>
        /// Returns a vector that is composed of the minimum values of each component of the given vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector composed of the minimum values of each component of the given vectors.</returns>
        public static Vector2 Min(this Vector2 a, Vector2 b) => Vector2.Min(a, b);

        /// <summary>
        /// Returns the component-wise maximum of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The component-wise maximum of the two vectors.</returns>
        public static Vector2 Max(this Vector2 a, Vector2 b) => Vector2.Max(a, b);

        /// <summary>
        /// Returns the maximum component of the vector.
        /// </summary>
        /// <param name="v">The vector to find the maximum component of.</param>
        /// <returns>The maximum component of the vector.</returns>
        public static float MaxComponent(this Vector2 v) => Mathf.Max(v.x, v.y);

        /// <summary>
        /// Returns the minimum component value of a Vector2.
        /// </summary>
        /// <param name="v">The vector to retrieve the minimum component from.</param>
        /// <returns>The minimum component value of the given vector.</returns>
        public static float MinComponent(this Vector2 v) => Mathf.Min(v.x, v.y);

        /// <summary>
        /// Calculates the distance between two points in the XY plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The distance between the two points.</returns>
        public static float DistanceXY(this Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Clamps the magnitude of a vector to a maximum length.
        /// </summary>
        /// <param name="v">The vector to clamp.</param>
        /// <param name="maxLength">The maximum length to clamp the vector to.</param>
        /// <returns>The clamped vector. If the magnitude of the vector is greater than maxLength, the vector is normalized and scaled to maxLength.</returns>
        public static Vector2 ClampMagnitude(this Vector2 v, float maxLength)
        {
            if (v.sqrMagnitude > maxLength * maxLength)
                return v.normalized * maxLength;
            return v;
        }

        /// <summary>
        /// Clamps the magnitude of a vector to a specified range.
        /// </summary>
        /// <param name="v">The vector to clamp.</param>
        /// <param name="maxLength">The maximum length the vector can have.</param>
        /// <returns>The clamped version of the vector.</returns>
        public static Vector2 ClampMagnitude(this Vector2 v, float min, float max)
        {
            float sqrMagnitude = v.sqrMagnitude;
            if (sqrMagnitude < min * min)
                return v.normalized * min;
            else if (sqrMagnitude > max * max)
                return v.normalized * max;
            return v;
        }

        /// <summary>
        /// Clamps each component of the given vector between the minimum and maximum values.
        /// </summary>
        /// <param name="v">The vector to be clamped.</param>
        /// <param name="min">The minimum values to clamp the components to.</param>
        /// <param name="max">The maximum values to clamp the components to.</param>
        /// <returns>The vector with each component clamped between the minimum and maximum values.</returns>
        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) => new(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));

        /// <summary>
        /// Converts the vector to a debug string representation.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>A string representation of the vector in the format "x: {value}, y: {value}"</returns>
        public static string ToDebugString(this Vector2 v) => $"x: {v.x:F3} y: {v.y:F3}";

        /// <summary>
        /// Calculates the absolute value of each component of a vector.
        /// </summary>
        /// <param name="v">The vector to calculate the absolute value for.</param>
        /// <returns>The vector with each component replaced by its absolute value.</returns>
        public static Vector2 Abs(this Vector2 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y));

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="a">The starting vector.</param>
        /// <param name="b">The ending vector.</param>
        /// <param name="t">The interpolation factor. Value between 0 and 1.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);

        /// <summary>
        /// Performs a linear interpolation between two vectors without restricting the t value to the range [0, 1].
        /// </summary>
        /// <param name="a">The start vector.</param>
        /// <param name="b">The end vector.</param>
        /// <param name="t">The interpolation value.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector2 LerpUnclamped(this Vector2 a, Vector2 b, float t) => new(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t));

        /// <summary>
        /// Moves a vector towards a target vector by a specified maximum distance.
        /// </summary>
        /// <param name="current">The current vector to move.</param>
        /// <param name="target">The target vector to move towards.</param>
        /// <param name="maxDistanceDelta">The maximum distance the current vector can move towards the target vector.</param>
        /// <returns>A new vector that is moved towards the target vector by the specified distance.</returns>
        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta) => Vector2.MoveTowards(current, target, maxDistanceDelta);

        /// <summary>
        /// Checks if a vector is normalized.
        /// </summary>
        /// <param name="v">The vector to check.</param>
        /// <returns>True if the vector is normalized; otherwise, false.</returns>
        public static bool IsNormalized(this Vector2 v) => Mathf.Approximately(v.magnitude, 1f);

        /// <summary>
        /// Calculates the angle, in degrees, between two vectors.
        /// </summary>
        /// <param name="from">The first vector.</param>
        /// <param name="to">The second vector.</param>
        /// <returns>The angle, in degrees, between the two vectors.</returns>
        public static float Angle(this Vector2 from, Vector2 to) => Vector2.Angle(from, to);

        /// <summary>
        /// Calculates the signed angle between two vectors in degrees.
        /// </summary>
        /// <param name="from">The vector from which the angle is measured.</param>
        /// <param name="to">The vector to which the angle is measured.</param>
        /// <returns>The signed angle in degrees between the two vectors.</returns>
        public static float AngleSigned(this Vector2 from, Vector2 to) => Vector2.SignedAngle(from, to);

        /// <summary>
        /// Calculates the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float Dot(this Vector2 a, Vector2 b) => Vector2.Dot(a, b);

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The position we are trying to reach.</param>
        /// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <returns>A vector moving towards the target.</returns>
        public static Vector2 SmoothDamp(this Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime) => Vector2.SmoothDamp(current, target, ref currentVelocity, smoothTime);

        /// <summary>
        /// Calculates a smooth damp of the current vector towards a target vector.
        /// </summary>
        /// <param name="current">The current vector.</param>
        /// <param name="target">The target vector to move towards.</param>
        /// <param name="currentVelocity">The current velocity, modified by the function.</param>
        /// <param name="smoothTime">The smooth time representing the time it takes to reach the target.</param>
        /// <returns>The resulting vector after applying the smooth damp.</returns>
        public static Vector2 SmoothDamp(this Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed) => Vector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);

        /// <summary>
        /// Smoothly interpolates between two vectors using a spring-damper system.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The target position.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target.</param>
        /// <returns>The smoothly interpolated vector.</returns>
        public static Vector2 SmoothDamp(this Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) => Vector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);

        /// <summary>
        /// Negates a given Vector2.
        /// </summary>
        /// <param name="v">The vector to negate.</param>
        /// <returns>The negated vector.</returns>
        public static Vector2 Negate(this Vector2 v) => -v;

        /// <summary>
        /// Offsets a given Vector2 with specified x and y values.
        /// </summary>
        /// <param name="v">The vector to offset.</param>
        /// <param name="x">Offset on X axis.</param>
        /// <param name="y">Offset on Y axis.</param>
        /// <returns>A vector offsetted by specified values on X and Y.</returns>
        public static Vector2 Offset(this Vector2 v, float x, float y) => v + new Vector2(x, y);

        /// <summary>
        /// Rotates a given Vector2 by a specified angle in degrees.
        /// </summary>
        /// <param name="v">The vector to rotate.</param>
        /// <param name="angle">Angle in degrees.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        /// <summary>
        /// Calculates the perpendicular vector in clockwise direction.
        /// </summary>
        /// <param name="v">The vector to calculate the perpendicular of.</param>
        /// <returns>A vector that is perpendicular to the input vector, rotated in clockwise direction.</returns>
        public static Vector2 PerpendicularClockwise(this Vector2 v) => new Vector2(v.y, -v.x);

        /// <summary>
        /// Calculates the perpendicular vector in counter-clockwise direction.
        /// </summary>
        /// <param name="v">The vector to calculate the perpendicular of.</param>
        /// <returns>A vector that is perpendicular to the input vector, rotated in counter-clockwise direction.</returns>
        public static Vector2 PerpendicularCounterClockwise(this Vector2 v) => new Vector2(-v.y, v.x);


        /// <summary>
        /// Projects a vector onto a given normal.
        /// </summary>
        /// <param name="v">The vector to project.</param>
        /// <param name="onNormal">The normal vector onto which to project.</param>
        /// <returns>The projection of the vector onto the given normal.</returns>
        public static Vector2 Project(this Vector2 v, Vector2 onNormal)
        {
            float sqrMagnitude = onNormal.sqrMagnitude;
            if (sqrMagnitude < Mathf.Epsilon)
                return Vector2.zero;
            float dot = Vector2.Dot(v, onNormal);
            return onNormal * (dot / sqrMagnitude);
        }

        /// <summary>
        /// Projects a vector onto a line defined by two points.
        /// </summary>
        /// <param name="v">The vector to project.</param>
        /// <param name="lineStart">The starting point of the line.</param>
        /// <param name="lineEnd">The ending point of the line.</param>
        /// <returns>The projected vector.</returns>
        public static Vector2 ProjectOnLine(this Vector2 v, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 lineDirection = lineEnd - lineStart;
            float sqrMagnitude = lineDirection.sqrMagnitude;
            if (sqrMagnitude < Mathf.Epsilon)
                return lineStart;
            float dot = Vector2.Dot(v - lineStart, lineDirection);
            return lineStart + lineDirection * (dot / sqrMagnitude);
        }

        /// <summary>
        /// Rejects the given vector on a specified normal vector.
        /// </summary>
        /// <param name="v">The vector to be rejected.</param>
        /// <param name="onNormal">The normal vector to reject the vector on.</param>
        /// <returns>The rejected vector.</returns>
        public static Vector2 Reject(this Vector2 v, Vector2 onNormal) => v - v.Project(onNormal);

        /// <summary>
        /// Reflects a given direction vector off a surface defined by a normal vector.
        /// </summary>
        /// <param name="inDirection">The incident direction vector.</param>
        /// <param name="inNormal">The normal vector of the surface.</param>
        /// <returns>The reflected direction vector.</returns>
        public static Vector2 Reflect(this Vector2 inDirection, Vector2 inNormal) => inDirection - 2f * Vector2.Dot(inDirection, inNormal) * inNormal;

        /// <summary>
        /// Rounds the components of a Vector2 to the nearest whole number.
        /// </summary>
        /// <param name="v">The Vector2 to be rounded.</param>
        /// <returns>A new Vector2 with rounded components.</returns>
        public static Vector2 Round(this Vector2 v) => new(Mathf.Round(v.x), Mathf.Round(v.y));

        /// <summary>
        /// Returns the largest integer less than or equal to the specified vector components.
        /// </summary>
        /// <param name="v">The vector to floor.</param>
        /// <returns>The result of flooring the vector components.</returns>
        public static Vector2 Floor(this Vector2 v) => new(Mathf.Floor(v.x), Mathf.Floor(v.y));

        /// <summary>
        /// Calculates the smallest integer greater than or equal to each component of a vector.
        /// </summary>
        /// <param name="v">The vector to calculate the ceiling value of.</param>
        /// <returns>A new vector with each component rounded up to the nearest integer.</returns>
        public static Vector2 Ceil(this Vector2 v) => new(Mathf.Ceil(v.x), Mathf.Ceil(v.y));

        /// <summary>
        /// Calculates the 2D cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The 2D cross product of the two vectors.</returns>
        public static float Cross(this Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        /// <summary>
        /// Scales a vector by another vector component-wise.
        /// </summary>
        /// <param name="v">The vector to scale.</param>
        /// <param name="scale">The vector to scale by.</param>
        /// <returns>The component-wise scaled vector.</returns>
        public static Vector2 Scale(this Vector2 v, Vector2 scale) => Vector2.Scale(v, scale);

        /// <summary>
        /// Calculates the direction vector from one point to another.
        /// </summary>
        /// <param name="from">The starting point of the direction.</param>
        /// <param name="to">The end point of the direction.</param>
        /// <returns>The direction vector from the starting point to the end point.</returns>
        public static Vector2 Direction(this Vector2 from, Vector2 to) => (to - from).normalized;

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="v">The first vector.</param>
        /// <param name="other">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector2 v, Vector2 other) => (v - other).sqrMagnitude;

        /// <summary>
        /// Performs a smooth interpolation between two vectors.
        /// </summary>
        /// <param name="a">The starting vector.</param>
        /// <param name="b">The ending vector.</param>
        /// <param name="t">The interpolation factor (between 0 and 1).</param>
        /// <returns>The interpolated vector between the starting and ending vectors.</returns>
        public static Vector2 SmoothStep(this Vector2 a, Vector2 b, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t * (3f - 2f * t);
            return Vector2.Lerp(a, b, t);
        }

        /// <summary>
        /// Wraps the given vector within the specified range.
        /// </summary>
        /// <param name="v">The vector to wrap.</param>
        /// <param name="min">The minimum values to wrap around.</param>
        /// <param name="max">The maximum values to wrap around.</param>
        /// <returns>The wrapped vector within the specified range.</returns>
        public static Vector2 Wrap(this Vector2 v, Vector2 min, Vector2 max) => new(Mathf.Repeat(v.x, max.x - min.x) + min.x, Mathf.Repeat(v.y, max.y - min.y) + min.y);

        /// <summary>
        /// Checks if two Vector2 instances are almost equal within a given tolerance.
        /// </summary>
        /// <param name="v">The first Vector2 instance.</param>
        /// <param name="other">The second Vector2 instance.</param>
        /// <param name="tolerance">The tolerance value used to determine if the vectors are almost equal. Defaults to 0.00001f.</param>
        /// <returns>True if the vectors are almost equal within the specified tolerance, false otherwise.</returns>
        public static bool AlmostEquals(this Vector2 v, Vector2 other, float tolerance = 0.00001f) => (v - other).sqrMagnitude < tolerance * tolerance;
    }
}