using UnityEngine;

namespace Toolbox
{
    public static class ExtensionsVector3
    {
        /// <summary>
        /// Calculates the inverse of a vector.
        /// </summary>
        /// <param name="v">The vector to calculate the inverse of.</param>
        /// <returns>The inverse of the given vector.</returns>
        public static Vector3 Inverse(this Vector3 v) => new(1.0f / v.x, 1.0f / v.y, 1.0f / v.z);

        /// <summary>
        /// Divides a vector by another vector element-wise.
        /// </summary>
        /// <param name="numerator">The vector to be divided.</param>
        /// <param name="denominator">The vector to divide by.</param>
        /// <returns>The resulting vector after element-wise division.</returns>
        public static Vector3 Divide(this Vector3 numerator, Vector3 denominator) => new(numerator.x / denominator.x, numerator.y / denominator.y, numerator.z / denominator.z);

        /// <summary>
        /// Multiplies two Vector3 values element-wise and returns the result.
        /// </summary>
        /// <param name="a">The first Vector3 value to multiply.</param>
        /// <param name="b">The second Vector3 value to multiply.</param>
        /// <returns>A new Vector3 that is the result of multiplying each corresponding component of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static Vector3 Multiply(this Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

        /// <summary>
        /// Scales a Vector3 by another Vector3 component-wise.
        /// </summary>
        /// <param name="a">The Vector3 to scale.</param>
        /// <param name="b">The Vector3 used as the scaling factor.</param>
        /// <returns>A new Vector3 with the scaled values.</returns>
        public static Vector3 Scale(this Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

        // Flipping
        /// <summary>
        /// Flips the given vector along the X-axis.
        /// </summary>
        /// <param name="v">The vector to flip.</param>
        /// <returns>The flipped vector.</returns>
        public static Vector3 FlipX(this Vector3 v) => new Vector3(-v.x, v.y, v.z);

        /// <summary>
        /// Flips the Y component of a vector.
        /// </summary>
        /// <param name="v">The vector to flip the Y component of.</param>
        /// <returns>The vector with the Y component flipped.</returns>
        public static Vector3 FlipY(this Vector3 v) => new Vector3(v.x, -v.y, v.z);

        /// <summary>
        /// Flips the z-coordinate of a vector to its opposite value.
        /// </summary>
        /// <param name="v">The vector to flip the z-coordinate of.</param>
        /// <returns>The vector with the z-coordinate flipped.</returns>
        public static Vector3 FlipZ(this Vector3 v) => new Vector3(v.x, v.y, -v.z);

        /// <summary>
        /// Modifies the X component of a Vector3.
        /// </summary>
        /// <param name="v">The Vector3 to modify.</param>
        /// <param name="x">The new value for the X component.</param>
        /// <returns>A Vector3 with the X component modified.</returns>
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

        /// <summary>
        /// Sets the Y component of a Vector3 to a given value.
        /// </summary>
        /// <param name="v">The original Vector3.</param>
        /// <param name="y">The value to set for the Y component.</param>
        /// <returns>A new Vector3 with the updated Y component.</returns>
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        /// <summary>
        /// Sets the Z component of a 3D vector to a specified value.
        /// </summary>
        /// <param name="v">The original 3D vector.</param>
        /// <param name="z">The new value for the Z component.</param>
        /// <returns>A new Vector3 with the same X and Y components as the original vector, but with the Z component set to the specified value.</returns>
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        // Swizzling
        /// <summary>
        /// Extracts the XY components of a Vector3 and returns them as a Vector2.
        /// </summary>
        /// <param name="v">The Vector3 to extract the XY components from.</param>
        /// <returns>A Vector2 containing the XY components of the given Vector3.</returns>
        public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);

        /// <summary>
        /// Returns a new Vector2 with the 'y' and 'z' components of the given Vector3.
        /// </summary>
        /// <param name="v">The Vector3 to extract the 'y' and 'z' components from.</param>
        /// <returns>A new Vector2 with the 'y' and 'z' components of the given Vector3.</returns>
        public static Vector2 ZY(this Vector3 v) => new Vector2(v.z, v.y);

        /// <summary>
        /// Retrieves the X and Z components of a Vector3 as a Vector2.
        /// </summary>
        /// <param name="v">The Vector3 to retrieve the X and Z components from.</param>
        /// <returns>A Vector2 with the X and Z components of the original Vector3.</returns>
        public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);

        /// <summary>
        /// Converts a 2D vector to a 3D vector with Z component set to zero.
        /// </summary>
        /// <param name="v">The 2D vector to convert.</param>
        /// <returns>A new Vector3 object with X, Y, and Z components.</returns>
        public static Vector3 XYto3(this Vector2 v) => new Vector3(v.x, v.y, 0);

        /// <summary>
        /// Converts a 2D vector (Z and Y values) to a 3D vector (X, Y, and Z values).
        /// </summary>
        /// <param name="v">The 2D vector to convert.</param>
        /// <returns>The converted 3D vector.</returns>
        public static Vector3 ZYto3(this Vector2 v) => new Vector3(0, v.y, v.x);

        /// <summary>
        /// Converts a 2D vector in the XZ plane to a 3D vector by adding a 0 value for the Y​ coordinate.
        /// </summary>
        /// <param name="v">The 2D vector to convert to a 3D vector.</param>
        /// <returns>A 3D vector with the X and Z values from the given 2D vector and a 0 value for the Y coordinate.</returns>
        public static Vector3 XZto3(this Vector2 v) => new Vector3(v.x, 0, v.y);

        /// <summary>
        /// Returns a vector that is composed of the minimum values of each component of the given vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector composed of the minimum values of each component of the given vectors.</returns>
        public static Vector3 Min(this Vector3 a, Vector3 b) => Vector3.Min(a, b);

        /// <summary>
        /// Returns the component-wise maximum of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The component-wise maximum of the two vectors.</returns>
        /// <example>
        /// Vector3 a = new Vector3(1, 2, 3);
        /// Vector3 b = new Vector3(4, 5, 6);
        /// Vector3 result = Max(a, b); // (4,5,6)
        /// </example>
        public static Vector3 Max(this Vector3 a, Vector3 b) => Vector3.Max(a, b);

        /// <summary>
        /// Returns the maximum component of the vector.
        /// </summary>
        /// <param name="v">The vector to find the maximum component of.</param>
        /// <returns>The maximum component of the vector.</returns>
        public static float MaxComponent(this Vector3 v) => Mathf.Max(v.x, v.y, v.z);

        /// <summary>
        /// Returns the minimum component value of a Vector3.
        /// </summary>
        /// <param name="v">The vector to retrieve the minimum component from.</param>
        /// <returns>The minimum component value of the given vector.</returns>
        public static float MinComponent(this Vector3 v) => Mathf.Min(v.x, v.y, v.z);

        /// <summary>
        /// Calculates the distance between two points in the XY plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The distance between the two points.</returns>
        public static float DistanceXY(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculates the distance between two points in the YZ plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The distance between the two points in the YZ plane.</returns>
        public static float DistanceYZ(this Vector3 a, Vector3 b)
        {
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return Mathf.Sqrt(dy * dy + dz * dz);
        }

        /// <summary>
        /// Calculates the Euclidean distance between two points in the XZ plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>The Euclidean distance between the two points.</returns>
        public static float DistanceXZ(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>
        /// Clamps the magnitude of a vector to a maximum length.
        /// </summary>
        /// <param name="v">The vector to clamp.</param>
        /// <param name="maxLength">The maximum length to clamp the vector to.</param>
        /// <returns>The clamped vector. If the magnitude of the vector is greater than maxLength, the vector is normalized and scaled to maxLength.</returns>
        public static Vector3 ClampMagnitude(this Vector3 v, float maxLength)
        {
            if (v.sqrMagnitude > maxLength * maxLength)
                return v.normalized * maxLength;
            return v;
        }

        /// <summary>
        /// Clamps each component of the given vector between the minimum and maximum values.
        /// </summary>
        /// <param name="v">The vector to be clamped.</param>
        /// <param name="min">The minimum values to clamp the components to.</param>
        /// <param name="max">The maximum values to clamp the components to.</param>
        /// <returns>The vector with each component clamped between the minimum and maximum values.</returns>
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) =>
            new(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));

        /// <summary>
        /// Calculates the direction vector that is most aligned with the up direction (Vector3.up) based on a given normal vector.
        /// </summary>
        /// <param name="n">The normal vector to calculate the direction from.</param>
        /// <returns>The direction vector that is most aligned with the up direction.</returns>
        public static Vector3 NormalToUp(this Vector3 n)
        {
            var upDot = Mathf.Abs(Vector3.Dot(n, Vector3.up));
            var rightDot = Mathf.Abs(Vector3.Dot(n, Vector3.right));
            var fwdDot = Mathf.Abs(Vector3.Dot(n, Vector3.forward));

            return upDot < rightDot ? (fwdDot < upDot ? Vector3.forward : Vector3.up) : (fwdDot < rightDot ? Vector3.forward : Vector3.right);
        }

        /// <summary>
        /// Converts the vector to a debug string representation.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>A string representation of the vector in the format "x: {value}, y: {value}, z: {value}"</returns>
        public static string ToDebugString(this Vector3 v) => $"x: {v.x:F3} y: {v.y:F3}, z: {v.z:F3}";

        /// <summary>
        /// Calculates the absolute value of each component of a vector.
        /// </summary>
        /// <param name="v">The vector to calculate the absolute value for.</param>
        /// <returns>The vector with each component replaced by its absolute value.</returns>
        public static Vector3 Abs(this Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="a">The starting vector.</param>
        /// <param name="b">The ending vector.</param>
        /// <param name="t">The interpolation factor. Value between 0 and 1.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector3 Lerp(this Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);

        /// <summary>
        /// Performs a linear interpolation between two vectors without restricting the t value to the range [0, 1].
        /// </summary>
        /// <param name="a">The start vector.</param>
        /// <param name="b">The end vector.</param>
        /// <param name="t">The interpolation value.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector3 LerpUnclamped(this Vector3 a, Vector3 b, float t) =>
            new(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t), Mathf.LerpUnclamped(a.z, b.z, t));

        /// <summary>
        /// Moves a vector towards a target vector by a specified maximum distance.
        /// </summary>
        /// <param name="current">The current vector to move.</param>
        /// <param name="target">The target vector to move towards.</param>
        /// <param name="maxDistanceDelta">The maximum distance the current vector can move towards the target vector.</param>
        /// <returns>A new vector that is moved towards the target vector by the specified distance.</returns>
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta) => Vector3.MoveTowards(current, target, maxDistanceDelta);

        /// <summary>
        /// Returns the reflection of a vector off a surface using the specified normal.
        /// </summary>
        /// <param name="inDirection">The incoming vector to be reflected.</param>
        /// <param name="inNormal">The normal of the surface.</param>
        /// <returns>The reflected vector.</returns>
        public static Vector3 Reflect(this Vector3 inDirection, Vector3 inNormal) => Vector3.Reflect(inDirection, inNormal);

        /// <summary>
        /// Checks if a vector is normalized.
        /// </summary>
        /// <param name="v">The vector to check.</param>
        /// <returns>True if the vector is normalized; otherwise, false.</returns>
        public static bool IsNormalized(this Vector3 v) => Mathf.Approximately(v.magnitude, 1f);

        /// ////<summary>
        /// Calculates the angle, in degrees, between two vectors.
        /// </summary>
        /// <param name="from">The first vector.</param>
        /// <param name="to">The second vector.</param>
        /// <returns>The angle, in degrees, between the two vectors.</returns>
        public static float Angle(this Vector3 from, Vector3 to) => Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f)) * Mathf.Rad2Deg;

        /// <summary>
        /// Calculates the signed angle between two vectors in degrees around a specified axis.
        /// </summary>
        /// <param name="from">The vector from which the angle is measured.</param>
        /// <param name="to">The vector to which the angle is measured.</param>
        /// <param name="axis">The axis around which the angle is measured.</param>
        /// <returns>The signed angle in degrees between the two vectors around the specified axis.</returns>
        public static float SignedAngle(this Vector3 from, Vector3 to, Vector3 axis) => Vector3.SignedAngle(from, to, axis);

        /// <summary>
        /// Calculates the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The cross product of the given vectors.</returns>
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);

        /// Converts a Vector3 into a Quaternion representing a rotation.
        /// The resulting Quaternion will have its forward direction aligned with the input Vector3.
        /// <param name="v">The Vector3 to convert into a rotation.</param>
        /// <returns>The Quaternion representing the rotation.</returns>
        public static Quaternion ToRotation(this Vector3 v) => Quaternion.LookRotation(v);

        /// <summary>
        /// Rotates a vector towards a target vector by a given angle and magnitude.
        /// </summary>
        /// <param name="current">The current vector to rotate.</param>
        /// <param name="target">The target vector to rotate towards.</param>
        /// <param name="maxRadiansDelta">The maximum angle in radians that the vector can rotate.</param>
        /// <param name="maxMagnitudeDelta">The maximum magnitude change that the vector can undergo.</param>
        /// <returns>The rotated vector towards the target vector.</returns>
        public static Vector3 RotateTowards(this Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta) =>
            Vector3.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta);

        /// <summary>
        /// Rotates a point around a given pivot by a specified rotation.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="pivot">The pivot point to rotate around.</param>
        /// <param name="rotation">The rotation to apply.</param>
        /// <returns>The rotated point.</returns>
        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation) => pivot + rotation * (point - pivot);

        /// <summary>
        /// Projects a vector onto a plane defined by its normal.
        /// </summary>
        /// <param name="v">The vector to project.</param>
        /// <param name="onNormal">The normal vector defining the plane.</param>
        /// <returns>The projected vector onto the plane.</returns>
        public static Vector3 Project(this Vector3 v, Vector3 onNormal) => Vector3.Project(v, onNormal);

        /// <summary>
        /// Projects a vector onto a plane defined by its normal.
        /// </summary>
        /// <param name="v">The vector to project onto the plane.</param>
        /// <param name="planeNormal">The normal of the plane.</param>
        /// <returns>The projection of the vector onto the plane.</returns>
        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal) => Vector3.ProjectOnPlane(v, planeNormal);

        /// <summary>
        /// Makes the vectors orthonormal to each other.
        /// </summary>
        /// <param name="normal">The normal vector.</param>
        /// <param name="tangent">The tangent vector.</param>
        /// <remarks>
        /// This method orthonormalizes the input vectors `normal` and `tangent`, ensuring that they are orthogonal
        /// (perpendicular) to each other and of unit length. The `normal` vector is assumed to be already normalized.
        /// The `tangent` vector is modified to be orthogonal to the `normal` vector while maintaining its direction
        /// as closely as possible.
        ///
        /// After calling this method, the `normal` and `tangent` vectors will be orthonormal, meaning they are
        /// orthogonal and of unit length.
        ///
        /// This is useful when you have a normal vector and a tangent vector that you want to ensure are perpendicular
        /// and normalized.
        /// </remarks>
        public static void OrthoNormalize(this Vector3 normal, ref Vector3 tangent) => Vector3.OrthoNormalize(ref normal, ref tangent);

        /// <summary>
        /// Makes the vectors orthonormal to each other.
        /// </summary>
        /// <param name="normal">The normal vector.</param>
        /// <param name="tangent">The tangent vector.</param>
        /// <param name="binormal">The binormal vector.</param>
        /// <remarks>
        /// This method orthonormalizes the input vectors `normal`, `tangent`, and `binormal`, ensuring that they are
        /// orthogonal (perpendicular) to each other and of unit length. The `normal` vector is assumed to be already
        /// normalized. The `tangent` and `binormal` vectors are modified to be orthogonal to the `normal` vector and
        /// to each other while maintaining their directions as closely as possible.
        ///
        /// After calling this method, the `normal`, `tangent`, and `binormal` vectors will be orthonormal, meaning
        /// they are orthogonal and of unit length.
        ///
        /// This is useful when you have a normal vector, a tangent vector, and a binormal vector that you want to
        /// ensure are perpendicular and normalized, forming an orthonormal basis.
        ///
        /// Note: The `binormal` vector is also known as the bitangent vector.
        /// </remarks>
        public static void OrthoNormalize(this Vector3 normal, ref Vector3 tangent, ref Vector3 binormal) => Vector3.OrthoNormalize(ref normal, ref tangent, ref binormal);

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The position we are trying to reach.</param>
        /// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <returns>A vector moving towards the target.</returns>
        /// <remarks>
        /// This method smoothly interpolates a vector towards a desired goal over time. It is commonly used for smoothing
        /// movement or gradually changing a value towards a target.
        ///
        /// The `current` parameter represents the current position or value. The `target` parameter is the desired position
        /// or value we want to reach. The `currentVelocity` parameter is a reference to the current velocity, which is
        /// modified by the function every time it is called. The `smoothTime` parameter determines the approximate time
        /// it will take to reach the target, with smaller values resulting in faster convergence.
        ///
        /// The returned vector moves towards the target position or value smoothly over time, taking into account the
        /// current position and velocity.
        /// </remarks>
        public static Vector3 SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) =>
            Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The position we are trying to reach.</param>
        /// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed">Optionally allows you to clamp the maximum speed.</param>
        /// <returns>A vector moving towards the target.</returns>
        /// <remarks>
        /// This method is similar to the SmoothDamp method above, but with an additional `maxSpeed` parameter that allows
        /// you to clamp the maximum speed of the interpolation. If the interpolation speed exceeds `maxSpeed`, it will be
        /// clamped to that value.
        /// </remarks>
        public static Vector3 SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed) =>
            Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);

        /// <summary>
        /// Gradually changes a vector towards a desired goal over time.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="target">The position we are trying to reach.</param>
        /// <param name="currentVelocity">The current velocity, this value is modified by the function every time you call it.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed">Optionally allows you to clamp the maximum speed.</param>
        /// <param name="deltaTime">The time since the last call to this function. By default Time.deltaTime.</param>
        /// <returns>A vector moving towards the target.</returns>
        /// <remarks>
        /// This method is similar to the SmoothDamp method above, but with an additional `deltaTime` parameter that allows
        /// you to specify the time elapsed since the last call to this function. This is useful when you want to control
        /// the interpolation speed independently of the frame rate.
        /// </remarks>
        public static Vector3 SmoothDamp(this Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) =>
            Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);

        /// <summary>
        /// Generates a random Vector3 within given range.
        /// </summary>
        /// <param name="min">The minimum value for each component of the random Vector3.</param>
        /// <param name="max">The maximum value for each component of the random Vector3.</param>
        /// <returns>A randomly generated Vector3 within the specified range.</returns>
        public static Vector3 Randomize(float min, float max) => new(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));

        /// <summary>
        /// Generates a random Vector3 within the given range.
        /// </summary>
        /// <param name="min">The minimum range values for each component of the Vector3.</param>
        /// <param name="max">The maximum range values for each component of the Vector3.</param>
        /// <returns>A random Vector3 within the specified range.</returns>
        public static Vector3 Randomize(Vector3 min, Vector3 max) =>
            new(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));

        /// <summary>
        /// Generates a random vector within a specified range and magnitude.
        /// </summary>
        /// <param name="min">The minimum values for each component of the vector.</param>
        /// <param name="max">The maximum values for each component of the vector.</param>
        /// <param name="minMagnitude">The minimum magnitude of the generated vector.</param>
        /// <param name="maxMagnitude">The maximum magnitude of the generated vector.</param>
        /// <returns>A randomly generated vector within the specified range and magnitude.</returns>
        public static Vector3 Randomize(Vector3 min, Vector3 max, float minMagnitude, float maxMagnitude)
        {
            var v = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
            v = v.normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude);
            return v;
        }

        /// <summary>
        /// Generates a random Vector3 based on specified ranges for each component.
        /// </summary>
        /// <param name="min">The minimum values for each component of the random Vector3.</param>
        /// <param name="max">The maximum values for each component of the random Vector3.</param>
        /// <param name="minMagnitude">The minimum magnitude for the random Vector3.</param>
        /// <param name="maxMagnitude">The maximum magnitude for the random Vector3.</param>
        /// <returns>A randomly generated Vector3.</returns>
        public static Vector3 Randomize(Vector3 min, Vector3 max, Vector3 minMagnitude, Vector3 maxMagnitude)
        {
            var v = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
            v = new Vector3(UnityEngine.Random.Range(minMagnitude.x, maxMagnitude.x), UnityEngine.Random.Range(minMagnitude.y, maxMagnitude.y),
                UnityEngine.Random.Range(minMagnitude.z, maxMagnitude.z));
            return v;
        }

        public static Vector3 Normalize(this Vector3 v) => v.normalized;

        public static float Distance(this Vector3 a, Vector3 b) => Vector3.Distance(a, b);
        public static float DistanceSquared(this Vector3 a, Vector3 b) => Vector3.SqrMagnitude(a - b);
        public static Vector3 Direction(this Vector3 from, Vector3 to) => (to - from).normalized;

        public static float AngleBetween(this Vector3 from, Vector3 to) => Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f)) * Mathf.Rad2Deg;
        public static Vector3 RotateAround(this Vector3 point, Vector3 center, Vector3 axis, float angle) => Quaternion.AngleAxis(angle, axis) * (point - center) + center;

        public static Vector3 RotateTowardsDirection(this Vector3 current, Vector3 target, float maxRadiansDelta) =>
            Vector3.RotateTowards(current, target, maxRadiansDelta, float.PositiveInfinity).normalized;

        public static Vector3 ProjectOnVector(this Vector3 v, Vector3 onVector) => Vector3.Project(v, onVector);
        public static Vector3 RejectFromVector(this Vector3 v, Vector3 fromVector) => v - v.ProjectOnVector(fromVector);

        public static Vector3 ScaleToLength(this Vector3 v, float length) => v.normalized * length;
        public static Vector3 Resize(this Vector3 v, float newLength) => v.normalized * newLength;

        public static Vector3 SmoothStep(this Vector3 from, Vector3 to, float t) =>
            new Vector3(Mathf.SmoothStep(from.x, to.x, t), Mathf.SmoothStep(from.y, to.y, t), Mathf.SmoothStep(from.z, to.z, t));

        public static Vector3 Ease(this Vector3 from, Vector3 to, float t, EaseType easeTypeType) => new Vector3(Mathf.Lerp(from.x, to.x, EaseFunction(easeTypeType, t)),
            Mathf.Lerp(from.y, to.y, EaseFunction(easeTypeType, t)), Mathf.Lerp(from.z, to.z, EaseFunction(easeTypeType, t)));

        public static Vector3 Cross(this Vector3 a, Vector3 b, Vector3 c) => Vector3.Cross(Vector3.Cross(a, b), c);
        public static Vector3 Perpendicular(this Vector3 v) => v.x != 0f || v.z != 0f ? new Vector3(-v.z, 0f, v.x) : new Vector3(0f, -v.z, v.y);

        // Randomization
        public static Vector3 RandomInsideUnitSphere() => UnityEngine.Random.insideUnitSphere;
        public static Vector3 RandomOnUnitSphere() => UnityEngine.Random.onUnitSphere;
        public static Vector2 RandomInsideUnitCircle() => UnityEngine.Random.insideUnitCircle;
        public static Vector2 RandomOnUnitCircle() => UnityEngine.Random.insideUnitCircle.normalized;

        // Swizzling and Component Manipulation
        public static Vector3 Swizzle(this Vector3 v, int x, int y, int z) => new Vector3(v[x], v[y], v[z]);

        public static Vector3 WithComponent(this Vector3 v, int index, float value)
        {
            v[index] = value;
            return v;
        }

        public enum EaseType
        {
            Linear,
            QuadraticIn,
            QuadraticOut,
            QuadraticInOut,
            CubicIn,
            CubicOut,
            CubicInOut
        }

        // Ease Function
        private static float EaseFunction(EaseType easeTypeType, float t)
        {
            switch (easeTypeType)
            {
                case EaseType.Linear:
                    return t;
                case EaseType.QuadraticIn:
                    return t * t;
                case EaseType.QuadraticOut:
                    return t * (2f - t);
                case EaseType.QuadraticInOut:
                    return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
                case EaseType.CubicIn:
                    return t * t * t;
                case EaseType.CubicOut:
                    return (--t) * t * t + 1f;
                case EaseType.CubicInOut:
                    return t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Converts a vector from Cartesian coordinates to spherical coordinates.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>The spherical coordinates of the vector.</returns>
        public static SphericalCoordinates ToSpherical(this Vector3 v)
        {
            float radius = v.magnitude;
            float azimuth = Mathf.Atan2(v.z, v.x);
            float elevation = Mathf.Asin(v.y / radius);

            return new SphericalCoordinates(radius, azimuth, elevation);
        }

        /// <summary>
        /// Converts a vector from Cartesian coordinates to polar coordinates in the x-z plane.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>The polar coordinates of the vector in the x-z plane.</returns>
        public static PolarCoordinates ToPolar(this Vector3 v)
        {
            float radius = Mathf.Sqrt(v.x * v.x + v.z * v.z);
            float angle = Mathf.Atan2(v.z, v.x);

            return new PolarCoordinates(radius, angle);
        }
    }
}

namespace Toolbox
{
}