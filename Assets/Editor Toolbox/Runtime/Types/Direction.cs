using System;
using UnityEngine;

namespace Toolbox
{
    [Serializable]
    public struct Direction
    {
        private readonly Vector3 _direction;
        private readonly Quaternion _rotation;

        public Vector3 Vector3 => _direction;
        public Vector3 EulerAngles => _rotation.eulerAngles;
        public Quaternion Quaternion => _rotation;

        /// <summary>
        /// Initializes a new instance of the Direction struct with a specified direction vector.
        /// </summary>
        /// <param name="direction">The vector indicating the direction. It must not be the zero vector.</param>
        public Direction(Vector3 direction)
        {
            if (direction == Vector3.zero)
                throw new ArgumentException("Cannot create a Direction from a zero vector.");
            _direction = direction.normalized;
            _rotation = Quaternion.LookRotation(_direction);
        }

        /// <summary>
        /// Initializes a new instance of the Direction struct with a specified quaternion.
        /// </summary>
        /// <param name="rotation">The quaternion representing the rotation.</param>
        public Direction(Quaternion rotation) : this(rotation * Vector3.forward) { }

        /// <summary>
        /// Rotates the direction by another direction.
        /// </summary>
        /// <param name="other">The direction to rotate by.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction Rotate(Direction other) => new(_rotation * other.Quaternion);

        /// <summary>
        /// Rotates the direction by an angle around an axis.
        /// </summary>
        /// <param name="angle">The angle in degrees to rotate.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction AxisAngle(float angle, Vector3 axis) => new(Quaternion.AngleAxis(angle, axis) * _rotation);

        /// <summary>
        /// Performs spherical linear interpolation between two directions.
        /// </summary>
        /// <param name="from">The start direction.</param>
        /// <param name="to">The end direction.</param>
        /// <param name="t">The interpolation factor between 0 and 1.</param>
        /// <returns>A new Direction interpolated between the two inputs.</returns>
        public static Direction SLerp(Direction from, Direction to, float t) => new(Quaternion.Slerp(from.Quaternion, to.Quaternion, t));

        /// <summary>
        /// Adds two Direction instances, resulting in a combined rotation.
        /// </summary>
        /// <param name="a">The first Direction operand.</param>
        /// <param name="b">The second Direction operand.</param>
        /// <returns>A new Direction that combines the rotations of both operands.</returns>
        public static Direction operator +(Direction a, Direction b) => a.Rotate(b);

        /// <summary>
        /// Subtracts one Direction from another, effectively applying the inverse rotation of the second to the first.
        /// </summary>
        /// <param name="a">The original Direction.</param>
        /// <param name="b">The Direction to subtract (reverse).</param>
        /// <returns>A new Direction that is the result of subtracting the second direction from the first.</returns>
        public static Direction operator -(Direction a, Direction b) => a.Rotate(new Direction(Quaternion.Inverse(b.Quaternion)));

        /// <summary>
        /// Multiplies a Direction by a Quaternion, resulting in a new Direction that combines the rotation of both.
        /// </summary>
        /// <param name="a">The Direction to be rotated.</param>
        /// <param name="b">The Quaternion to rotate by.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public static Direction operator *(Direction a, Quaternion b) => new Direction(b * a.Quaternion);

        /// <summary>
        /// Multiplies a Quaternion by a Direction, resulting in a new Direction that combines the rotation of both.
        /// </summary>
        /// <param name="a">The Quaternion to rotate by.</param>
        /// <param name="b">The Direction to be rotated.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public static Direction operator *(Quaternion a, Direction b) => new Direction(a * b.Quaternion);

        /// <summary>
        /// Implicitly converts a Direction to a Vector3 representing its direction vector.
        /// </summary>
        /// <param name="d">The Direction to convert.</param>
        public static implicit operator Vector3(Direction d) => d._direction;

        /// <summary>
        /// Explicitly converts a Vector3 to a Direction assuming the vector represents a direction in 3D space.
        /// </summary>
        /// <param name="v">The Vector3 to convert.</param>
        public static explicit operator Direction(Vector3 v) => new(v);

        /// <summary>
        /// Provides a string representation of the Direction for debugging purposes.
        /// </summary>
        /// <returns>A string representing the internal state of the Direction.</returns>
        public override string ToString() => $"Direction: {_direction}, Rotation: {_rotation.eulerAngles}";


        /// <summary>
        /// Rotates the direction around the X-axis by the specified angle.
        /// </summary>
        /// <param name="angle">The angle in degrees to rotate around the X-axis.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction RotateX(float angle) => AxisAngle(angle, Vector3.right);

        /// <summary>
        /// Rotates the direction around the Y-axis by the specified angle.
        /// </summary>
        /// <param name="angle">The angle in degrees to rotate around the Y-axis.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction RotateY(float angle) => AxisAngle(angle, Vector3.up);

        /// <summary>
        /// Rotates the direction around the Z-axis by the specified angle.
        /// </summary>
        /// <param name="angle">The angle in degrees to rotate around the Z-axis.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction RotateZ(float angle) => AxisAngle(angle, Vector3.forward);

        /// <summary>
        /// Rotates the direction by the specified Euler angles.
        /// </summary>
        /// <param name="euler">The Euler angles to rotate by.</param>
        /// <returns>A new Direction that is the result of the rotation.</returns>
        public Direction RotateEuler(Vector3 euler) => new Direction(Quaternion.Euler(euler) * _rotation);

        // Equality comparison

        /// <summary>
        /// Determines whether the specified object is equal to the current Direction.
        /// </summary>
        /// <param name="obj">The object to compare with the current Direction.</param>
        /// <returns>true if the specified object is equal to the current Direction; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Direction other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Determines whether the specified Direction is equal to the current Direction.
        /// </summary>
        /// <param name="other">The Direction to compare with the current Direction.</param>
        /// <returns>true if the specified Direction is equal to the current Direction; otherwise, false.</returns>
        public bool Equals(Direction other)
        {
            return _direction == other._direction && _rotation == other._rotation;
        }

        /// <summary>
        /// Returns the hash code for this Direction.
        /// </summary>
        /// <returns>A hash code for the current Direction.</returns>
        public override int GetHashCode()
        {
            return _direction.GetHashCode() ^ _rotation.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Direction instances are equal.
        /// </summary>
        /// <param name="a">The first Direction to compare.</param>
        /// <param name="b">The second Direction to compare.</param>
        /// <returns>true if the specified Directions are equal; otherwise, false.</returns>
        public static bool operator ==(Direction a, Direction b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two Direction instances are not equal.
        /// </summary>
        /// <param name="a">The first Direction to compare.</param>
        /// <param name="b">The second Direction to compare.</param>
        /// <returns>true if the specified Directions are not equal; otherwise, false.</returns>
        public static bool operator !=(Direction a, Direction b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Determines whether the current Direction is approximately equal to another Direction within a specified tolerance.
        /// </summary>
        /// <param name="other">The Direction to compare with the current Direction.</param>
        /// <param name="tolerance">The maximum allowed difference between the directions.</param>
        /// <returns>true if the current Direction is approximately equal to the other Direction; otherwise, false.</returns>
        public bool ApproximatelyEquals(Direction other, float tolerance = 1e-6f)
        {
            return Vector3.Dot(_direction, other._direction) > 1f - tolerance &&
                   Quaternion.Angle(_rotation, other._rotation) < tolerance * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Determines whether two Direction instances are approximately equal within a specified tolerance.
        /// </summary>
        /// <param name="a">The first Direction to compare.</param>
        /// <param name="b">The second Direction to compare.</param>
        /// <param name="tolerance">The maximum allowed difference between the directions.</param>
        /// <returns>true if the specified Directions are approximately equal; otherwise, false.</returns>
        public static bool ApproximatelyEqual(Direction a, Direction b, float tolerance = 1e-6f)
        {
            return a.ApproximatelyEquals(b, tolerance);
        }
    }
}