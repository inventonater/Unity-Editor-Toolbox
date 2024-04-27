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
    }
}