using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents a point in 2D space using polar coordinates.
    /// Polar coordinates define a point by its distance from the origin (radius) and the angle from the positive x-axis (angle).
    /// </summary>
    /// <remarks>
    /// Polar coordinates are useful when working with circular or radial patterns in 2D space.
    /// They provide a more intuitive way to represent and manipulate points based on their distance and angle from the origin.
    ///
    /// In polar coordinates, the origin is considered to be the point (0, 0) in Cartesian coordinates.
    /// The radius is the distance from the origin to the point, and the angle is measured counterclockwise from the positive x-axis.
    ///
    /// The angle is typically expressed in radians, where:
    /// - 0 radians represents the positive x-axis.
    /// - π/2 radians (90 degrees) represents the positive y-axis.
    /// - π radians (180 degrees) represents the negative x-axis.
    /// - 3π/2 radians (270 degrees) represents the negative y-axis.
    ///
    /// This struct provides methods to convert between polar coordinates and other coordinate systems,
    /// such as Cartesian coordinates (Vector2 or Vector3) and spherical coordinates (SphericalCoordinates).
    /// </remarks>
    public struct PolarCoordinates
    {
        /// <summary>
        /// The radial distance from the origin to the point.
        /// </summary>
        public float Radius;

        /// <summary>
        /// The angle in radians, measured counterclockwise from the positive x-axis.
        /// </summary>
        public float AngleRadians;

        /// <summary>
        /// The angle in degrees, calculated from the angle in radians for easier readability and usage in some contexts.
        /// </summary>
        public float AngleDegrees => AngleRadians * Mathf.Rad2Deg;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarCoordinates"/> struct with the specified radius and angle.
        /// </summary>
        /// <param name="radius">The radial distance from the origin.</param>
        /// <param name="angleRadians">The angle in radians, measured counterclockwise from the positive x-axis.</param>
        public PolarCoordinates(float radius, float angleRadians)
        {
            Radius = radius;
            AngleRadians = angleRadians;
        }

        /// <summary>
        /// Converts the polar coordinates to a Vector2 in Cartesian coordinates.
        /// </summary>
        /// <returns>A Vector2 representing the point in Cartesian coordinates.</returns>
        /// <remarks>
        /// This method converts the polar coordinates to Cartesian coordinates using the following formulas:
        /// - x = radius * cos(angle)
        /// - y = radius * sin(angle)
        ///
        /// The resulting Vector2 represents the point in the 2D Cartesian coordinate system.
        /// </remarks>
        public Vector2 ToVector2()
        {
            float x = Radius * Mathf.Cos(AngleRadians);
            float y = Radius * Mathf.Sin(AngleRadians);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts the polar coordinates to a Vector3 in the x-z plane (y = 0).
        /// </summary>
        /// <returns>A Vector3 representing the point in the x-z plane.</returns>
        /// <remarks>
        /// This method converts the polar coordinates to a Vector3 in the x-z plane by setting the y-component to zero.
        /// The resulting Vector3 represents the point in the 3D Cartesian coordinate system, where the polar coordinates
        /// are mapped to the x-z plane.
        /// </remarks>
        public Vector3 ToVector3()
        {
            return new Vector3(ToVector2().x, 0f, ToVector2().y);
        }

        /// <summary>
        /// Converts the polar coordinates to spherical coordinates with zero elevation.
        /// </summary>
        /// <returns>The spherical coordinates with zero elevation.</returns>
        /// <remarks>
        /// This method converts the polar coordinates to spherical coordinates by setting the elevation angle to zero.
        /// The resulting SphericalCoordinates struct represents the point in the 3D spherical coordinate system,
        /// where the polar coordinates are mapped to the equatorial plane (elevation = 0).
        /// </remarks>
        public SphericalCoordinates ToSphericalCoordinates()
        {
            return new SphericalCoordinates(Radius, AngleRadians, 0f);
        }

        /// <summary>
        /// Returns a new PolarCoordinates struct with the radius clamped between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value for the radius.</param>
        /// <param name="max">The maximum value for the radius.</param>
        /// <returns>A new PolarCoordinates struct with the clamped radius.</returns>
        /// <remarks>
        /// This method creates a new PolarCoordinates struct with the same angle as the original, but with the radius clamped
        /// between the specified minimum and maximum values. If the original radius is within the range, it remains unchanged.
        /// </remarks>
        public PolarCoordinates ClampRadius(float min, float max)
        {
            float clampedRadius = Mathf.Clamp(Radius, min, max);
            return new PolarCoordinates(clampedRadius, AngleRadians);
        }

        /// <summary>
        /// Returns a new PolarCoordinates struct with the angle wrapped to the range [0, 2π].
        /// </summary>
        /// <returns>A new PolarCoordinates struct with the wrapped angle.</returns>
        /// <remarks>
        /// This method creates a new PolarCoordinates struct with the same radius as the original, but with the angle wrapped
        /// to the range [0, 2π]. If the original angle is already within the range, it remains unchanged.
        ///
        /// Wrapping the angle ensures that it stays within a complete rotation (360 degrees or 2π radians).
        /// </remarks>
        public PolarCoordinates WrapAngle()
        {
            float wrappedAngle = Mathf.Repeat(AngleRadians, Mathf.PI * 2f);
            return new PolarCoordinates(Radius, wrappedAngle);
        }

        /// <summary>
        /// Scales the radius and angle of the <see cref="PolarCoordinates"/> struct by the specified factor.
        /// </summary>
        /// <param name="scaleFactor">The factor by which to scale the radius and angle.</param>
        /// <returns>A new instance of the <see cref="PolarCoordinates"/> struct with the scaled radius and angle.</returns>
        public PolarCoordinates Scale(float scaleFactor)
        {
            float newRadius = Radius * scaleFactor;
            float newAngle = AngleRadians * scaleFactor;
            return new PolarCoordinates(newRadius, newAngle).WrapAngle();
        }

        /// <summary>
        /// Returns a new PolarCoordinates struct with the angle offset by the specified value in radians.
        /// </summary>
        /// <param name="offset">The angle offset in radians.</param>
        /// <returns>A new PolarCoordinates struct with the offset angle.</returns>
        /// <remarks>
        /// This method creates a new PolarCoordinates struct with the same radius as the original, but with the angle offset
        /// by the specified value in radians. Positive offset values rotate the angle counterclockwise, while negative values
        /// rotate it clockwise.
        ///
        /// The resulting angle is automatically wrapped to the range [0, 2π] to ensure it represents a valid angle within a
        /// complete rotation.
        /// </remarks>
        public PolarCoordinates OffsetAngle(float offset)
        {
            float offsetAngle = AngleRadians + offset;
            return new PolarCoordinates(Radius, offsetAngle).WrapAngle();
        }

        /// <summary>
        /// Returns a string representation of the polar coordinates.
        /// </summary>
        /// <returns>A string representation of the polar coordinates.</returns>
        /// <remarks>
        /// This method returns a string that describes the polar coordinates in a human-readable format.
        /// The string includes the radius and angle values, with the angle converted from radians to degrees for better readability.
        ///
        /// The string format is: "Radius: {radius}, Angle: {angle}°"
        /// </remarks>
        public override string ToString()
        {
            return $"Radius: {Radius}, Angle: {AngleRadians * Mathf.Rad2Deg}°";
        }

        /// <summary>
        /// Adds two PolarCoordinates together and returns the result as a new PolarCoordinates instance.
        /// </summary>
        /// <param name="other">The PolarCoordinates instance to add to this instance.</param>
        /// <returns>A new PolarCoordinates instance that represents the sum of this instance and the other instance.</returns>
        public PolarCoordinates Add(PolarCoordinates other)
        {
            return FromVector2(ToVector2() + other.ToVector2());
        }

        /// <summary>
        /// Subtracts the specified PolarCoordinates from another PolarCoordinates.
        /// </summary>
        /// <param name="other">The PolarCoordinates to subtract.</param>
        /// <returns>A new PolarCoordinates structure that represents the result of the subtraction operation.</returns>
        public PolarCoordinates Subtract(PolarCoordinates other)
        {
            return FromVector2(ToVector2() - other.ToVector2());
        }

        /// <summary>
        /// Rotates the angle of the polar coordinates by the specified number of radians.
        /// </summary>
        /// <param name="angleRadians">The angle in radians to rotate by.</param>
        /// <returns>The rotated polar coordinates.</returns>
        public PolarCoordinates Rotate(float angleRadians)
        {
            return new PolarCoordinates(Radius, AngleRadians + angleRadians).WrapAngle();
        }

        /// <summary>
        /// Linearly interpolates between two polar coordinates.
        /// </summary>
        /// <param name="target">The target polar coordinates to interpolate towards.</param>
        /// <param name="t">The interpolation parameter value between 0 and 1.</param>
        /// <returns>The interpolated polar coordinates between the current and target coordinates.</returns>
        public PolarCoordinates Lerp(PolarCoordinates target, float t)
        {
            float lerpedRadius = Mathf.Lerp(Radius, target.Radius, t);
            float deltaAngle = Mathf.DeltaAngle(AngleRadians, target.AngleRadians);
            float lerpedAngle = AngleRadians + deltaAngle * t;
            return new PolarCoordinates(lerpedRadius, lerpedAngle).WrapAngle();
        }

        /// <summary>
        /// Calculates the polar coordinates from the given Vector2 in Cartesian coordinates.
        /// </summary>
        /// <param name="vector">The Vector2 in Cartesian coordinates.</param>
        /// <returns>A new PolarCoordinates struct representing the polar coordinates of the given Vector2.</returns>
        /// <remarks>
        /// This method calculates the polar coordinates (radius and angle) from the given Vector2 in Cartesian coordinates.
        ///
        /// The radius is calculated as the magnitude of the vector.
        /// The angle is calculated using the arctangent of the y and x components of the vector.
        ///
        /// Note: This method assumes that the Vector2 represents a point in 2D space relative to the origin (0, 0).
        /// </remarks>
        public static PolarCoordinates FromVector2(Vector2 vector)
        {
            float radius = vector.magnitude;
            float angle = Mathf.Atan2(vector.y, vector.x);
            return new PolarCoordinates(radius, angle);
        }


        /// <summary>
        /// Determines whether the specified object is equal to the current PolarCoordinates.
        /// </summary>
        /// <param name="obj">The object to compare with the current PolarCoordinates.</param>
        /// <returns>true if the specified object is equal to the current PolarCoordinates; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is PolarCoordinates other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Determines whether the specified PolarCoordinates is equal to the current PolarCoordinates.
        /// </summary>
        /// <param name="other">The PolarCoordinates to compare with the current PolarCoordinates.</param>
        /// <returns>true if the specified PolarCoordinates is equal to the current PolarCoordinates; otherwise, false.</returns>
        public bool Equals(PolarCoordinates other)
        {
            return Radius == other.Radius && AngleRadians == other.AngleRadians;
        }

        /// <summary>
        /// Determines whether the current PolarCoordinates is approximately equal to another PolarCoordinates within a specified tolerance.
        /// </summary>
        /// <param name="other">The PolarCoordinates to compare with the current PolarCoordinates.</param>
        /// <param name="tolerance">The maximum allowed difference between the coordinates.</param>
        /// <returns>true if the current PolarCoordinates is approximately equal to the other PolarCoordinates; otherwise, false.</returns>
        public bool ApproximatelyEquals(PolarCoordinates other, float tolerance = 1e-6f)
        {
            return Mathf.Abs(Radius - other.Radius) < tolerance &&
                   Mathf.Abs(Mathf.DeltaAngle(AngleRadians, other.AngleRadians)) < tolerance;
        }

        /// <summary>
        /// Returns the hash code for this PolarCoordinates.
        /// </summary>
        /// <returns>A hash code for the current PolarCoordinates.</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode() ^ AngleRadians.GetHashCode();
        }

        /// <summary>
        /// Determines whether two PolarCoordinates instances are equal.
        /// </summary>
        /// <param name="a">The first PolarCoordinates to compare.</param>
        /// <param name="b">The second PolarCoordinates to compare.</param>
        /// <returns>true if the specified PolarCoordinates are equal; otherwise, false.</returns>
        public static bool operator ==(PolarCoordinates a, PolarCoordinates b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two PolarCoordinates instances are not equal.
        /// </summary>
        /// <param name="a">The first PolarCoordinates to compare.</param>
        /// <param name="b">The second PolarCoordinates to compare.</param>
        /// <returns>true if the specified PolarCoordinates are not equal; otherwise, false.</returns>
        public static bool operator !=(PolarCoordinates a, PolarCoordinates b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Returns a new instance of the PolarCoordinates struct with the negated radius.
        /// </summary>
        /// <returns>A new PolarCoordinates struct with the negated radius.</returns>
        public PolarCoordinates Negate()
        {
            return new PolarCoordinates(-Radius, AngleRadians);
        }

        /// <summary>
        /// Returns a new instance of the PolarCoordinates struct with the absolute value of the radius.
        /// </summary>
        /// <returns>A new PolarCoordinates struct with the absolute value of the radius.</returns>
        public PolarCoordinates Abs()
        {
            return new PolarCoordinates(Mathf.Abs(Radius), AngleRadians);
        }

        /// <summary>
        /// Converts the polar coordinates to a Vector3 with the specified height (y-coordinate).
        /// </summary>
        /// <param name="height">The height (y-coordinate) of the resulting Vector3.</param>
        /// <returns>A Vector3 representing the polar coordinates with the specified height.</returns>
        public Vector3 ToVector3(float height)
        {
            Vector2 vector2 = ToVector2();
            return new Vector3(vector2.x, height, vector2.y);
        }

        /// <summary>
        /// Calculates the distance between two polar coordinates.
        /// </summary>
        /// <param name="other">The other polar coordinates to calculate the distance to.</param>
        /// <returns>The distance between the current and other polar coordinates.</returns>
        public float Distance(PolarCoordinates other)
        {
            return (ToVector2() - other.ToVector2()).magnitude;
        }

        /// <summary>
        /// Calculates the angle between two polar coordinates in radians.
        /// </summary>
        /// <param name="other">The other polar coordinates to calculate the angle to.</param>
        /// <returns>The angle between the current and other polar coordinates in radians.</returns>
        public float AngleTo(PolarCoordinates other)
        {
            return Mathf.DeltaAngle(AngleRadians, other.AngleRadians);
        }
    }
}