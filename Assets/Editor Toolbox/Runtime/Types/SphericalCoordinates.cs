﻿using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents a point in 3D space using spherical coordinates.
    /// Spherical coordinates define a point by its distance from the origin (radius),
    /// the azimuth angle in the x-z plane, and the elevation angle from the x-z plane.
    /// </summary>
    /// <remarks>
    /// Spherical coordinates are a way to describe a point in 3D space using three values:
    /// - Radius: The distance from the origin to the point.
    /// - Azimuth: The angle in the x-z plane, measured from the positive x-axis.
    /// - Elevation: The angle from the x-z plane to the point.
    ///
    /// Spherical coordinates are useful in various scenarios, such as:
    /// - Representing positions on a sphere or in a spherical space.
    /// - Describing the direction and distance of a point from the origin.
    /// - Working with 3D polar coordinates, where the azimuth and elevation angles are used.
    ///
    /// This struct provides methods to convert between spherical coordinates and other coordinate systems,
    /// such as Cartesian coordinates (Vector3) and polar coordinates (PolarCoordinates).
    ///
    /// For more information on spherical coordinates and their applications, you can refer to the following resources:
    /// https://en.wikipedia.org/wiki/Spherical_coordinate_system
    /// </remarks>
    public struct SphericalCoordinates
    {
        /// <summary>
        /// The radial distance from the origin to the point.
        /// </summary>
        public float Radius;

        /// <summary>
        /// The azimuth angle in radians, measured in the x-z plane from the positive x-axis.
        /// </summary>
        public float AzimuthRadians;
        public float AzimuthDegrees => AzimuthRadians * Mathf.Rad2Deg;

        /// <summary>
        /// The elevation angle in radians, measured from the x-z plane to the point.
        /// </summary>
        public float ElevationRadians;
        public float ElevationDegrees => ElevationRadians * Mathf.Rad2Deg;

        /// <summary>
        /// Converts the spherical coordinates to a Vector3 in Cartesian coordinates.
        /// </summary>
        /// <returns>A Vector3 representing the point in Cartesian coordinates.</returns>
        /// <remarks>
        /// This method converts the spherical coordinates to Cartesian coordinates using the following formulas:
        /// - x = radius * cos(elevation) * cos(azimuth)
        /// - y = radius * sin(elevation)
        /// - z = radius * cos(elevation) * sin(azimuth)
        ///
        /// The resulting Vector3 represents the point in the 3D Cartesian coordinate system.
        /// </remarks>
        public Vector3 ToVector3()
        {
            float x = Radius * Mathf.Cos(ElevationRadians) * Mathf.Cos(AzimuthRadians);
            float y = Radius * Mathf.Sin(ElevationRadians);
            float z = Radius * Mathf.Cos(ElevationRadians) * Mathf.Sin(AzimuthRadians);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Calculates the spherical coordinates from the given Vector3 in Cartesian coordinates.
        /// </summary>
        /// <param name="vector">The Vector3 in Cartesian coordinates.</param>
        /// <returns>A new SphericalCoordinates struct representing the spherical coordinates of the given Vector3.</returns>
        /// <remarks>
        /// This method calculates the spherical coordinates (radius, azimuth, and elevation) from the given Vector3 in Cartesian coordinates.
        ///
        /// The radius is calculated as the magnitude of the vector.
        /// The azimuth is calculated using the arctangent of the z and x components of the vector.
        /// The elevation is calculated using the arcsine of the y component divided by the radius.
        ///
        /// Note: This method assumes that the Vector3 represents a point in 3D space relative to the origin (0, 0, 0).
        /// </remarks>
        public static SphericalCoordinates FromVector3(Vector3 vector)
        {
            float radius = vector.magnitude;
            float azimuth = Mathf.Atan2(vector.z, vector.x);
            float elevation = (radius > 0) ? Mathf.Asin(vector.y / radius) : 0; // Prevent division by zero.
            return new SphericalCoordinates(radius, azimuth, elevation);
        }

        /// <summary>
        /// Converts the spherical coordinates to a Quaternion representing a rotation.
        /// </summary>
        /// <returns>A Quaternion representing the rotation in 3D space.</returns>
        /// <remarks>
        /// This method converts the spherical coordinates to a Quaternion using the following formulas:
        /// - pitch = elevation * Mathf.Rad2Deg
        /// - yaw = azimuth * Mathf.Rad2Deg
        /// - roll = 0
        /// The resulting Quaternion represents the rotation in the 3D space.
        /// </remarks>
        public Quaternion ToQuaternion() => Quaternion.Euler(ElevationRadians * Mathf.Rad2Deg, AzimuthRadians * Mathf.Rad2Deg, 0f);

        /// <summary>
        /// Creates a SphericalCoordinates object from a Quaternion.
        /// </summary>
        /// <param name="quaternion">The Quaternion to convert.</param>
        /// <returns>A new SphericalCoordinates object representing the Quaternion in spherical coordinates.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates object from a Quaternion, which represents the orientation of an object in 3D space.
        /// The SphericalCoordinates object contains the radius, azimuth, and elevation values in spherical coordinates.
        /// </remarks>
        public static SphericalCoordinates FromQuaternion(Quaternion quaternion) => new SphericalCoordinates(new Direction(quaternion));

        /// <summary>
        /// Initializes a new instance of the <see cref="SphericalCoordinates"/> struct with the specified radius, azimuth, and elevation.
        /// </summary>
        /// <param name="radius">The radial distance from the origin.</param>
        /// <param name="azimuthRadians">The azimuth angle in radians, measured in the x-z plane from the positive x-axis.</param>
        /// <param name="elevationRadians">The elevation angle in radians, measured from the x-z plane.</param>
        public SphericalCoordinates(float radius, float azimuthRadians, float elevationRadians)
        {
            Radius = Mathf.Max(0, radius); // Ensure that radius is never negative.
            AzimuthRadians = azimuthRadians;
            ElevationRadians = Mathf.Clamp(elevationRadians, -Mathf.PI / 2, Mathf.PI / 2); // Clamp the elevation to valid range.
        }

        public SphericalCoordinates(SphericalCoordinates sphericalCoordinates)
        {
            Radius = sphericalCoordinates.Radius;
            AzimuthRadians = sphericalCoordinates.AzimuthRadians;
            ElevationRadians = sphericalCoordinates.ElevationRadians;
        }
        public SphericalCoordinates(Vector3 vector) : this(FromVector3(vector)) { }
        public SphericalCoordinates(Direction direction) : this(FromDirection(direction)) { }
        public SphericalCoordinates(Quaternion quaternion) : this(new Direction(quaternion)) { }

        /// <summary>
        /// Rotates the current spherical coordinates by the specified rotation spherical coordinates.
        /// </summary>
        /// <param name="other">The rotation spherical coordinates.</param>
        /// <returns>The rotated spherical coordinates.</returns>
        /// <remarks>
        /// This method applies the rotation represented by the `rotation` spherical coordinates to the current spherical coordinates.
        /// The rotation is performed by converting both the current and rotation spherical coordinates to direction vectors,
        /// applying the rotation using Quaternion multiplication, and then converting the result back to spherical coordinates.
        /// </remarks>
        public SphericalCoordinates Rotate(SphericalCoordinates other)
        {
            return FromVector3(other.ToQuaternion() * ToQuaternion() * Vector3.forward * Radius);
        }

        /// <summary>
        /// Inverse rotates the current spherical coordinates by the specified rotation spherical coordinates.
        /// </summary>
        /// <param name="other">The rotation spherical coordinates.</param>
        /// <returns>The inverse rotated spherical coordinates.</returns>
        /// <remarks>
        /// This method applies the inverse rotation represented by the `rotation` spherical coordinates to the current spherical coordinates.
        /// The inverse rotation is performed by converting both the current and rotation spherical coordinates to direction vectors,
        /// applying the inverse rotation using Quaternion multiplication, and then converting the result back to spherical coordinates.
        /// </remarks>
        public SphericalCoordinates InverseRotate(SphericalCoordinates other)
        {
            return FromVector3(Quaternion.Inverse(other.ToQuaternion()) * ToQuaternion() * Vector3.forward * Radius);
        }

        /// <summary>
        /// Converts the spherical coordinates to a Direction.
        /// </summary>
        /// <returns>A Direction representing the point in Cartesian coordinates.</returns>
        public Direction ToDirection() => new(ToVector3());

        /// <summary>
        /// Calculates the spherical coordinates from the given Direction.
        /// </summary>
        /// <param name="direction">The Direction.</param>
        /// <returns>A new SphericalCoordinates struct representing the spherical coordinates of the given Direction.</returns>
        public static SphericalCoordinates FromDirection(Direction direction) => FromVector3(direction.Vector3);

        /// <summary>
        /// Converts the spherical coordinates to polar coordinates in the x-z plane.
        /// </summary>
        /// <returns>The polar coordinates in the x-z plane.</returns>
        /// <remarks>
        /// This method converts the spherical coordinates to polar coordinates by projecting the point onto the x-z plane.
        /// The resulting PolarCoordinates struct represents the point in the 2D polar coordinate system,
        /// where the radius is the distance from the origin to the projected point, and the angle is the azimuth.
        /// </remarks>
        public PolarCoordinates ToPolarCoordinates()
        {
            float projectedRadius = Radius * Mathf.Cos(ElevationRadians);
            return new PolarCoordinates(projectedRadius, AzimuthRadians);
        }

        /// <summary>
        /// Returns a new SphericalCoordinates struct with the radius clamped between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value for the radius.</param>
        /// <param name="max">The maximum value for the radius.</param>
        /// <returns>A new SphericalCoordinates struct with the clamped radius.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates struct with the same azimuth and elevation as the original,
        /// but with the radius clamped between the specified minimum and maximum values.
        /// If the original radius is within the range, it remains unchanged.
        /// </remarks>
        public SphericalCoordinates ClampRadius(float min, float max)
        {
            float clampedRadius = Mathf.Clamp(Radius, min, max);
            return new SphericalCoordinates(clampedRadius, AzimuthRadians, ElevationRadians);
        }

        /// <summary>
        /// Returns a new SphericalCoordinates struct with the azimuth angle wrapped to the range [0, 2π].
        /// </summary>
        /// <returns>A new SphericalCoordinates struct with the wrapped azimuth angle.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates struct with the same radius and elevation as the original,
        /// but with the azimuth angle wrapped to the range [0, 2π]. If the original azimuth is already within the range,
        /// it remains unchanged.
        ///
        /// Wrapping the azimuth ensures that it stays within a complete rotation (360 degrees or 2π radians) in the x-z plane.
        /// </remarks>
        public SphericalCoordinates WrapAzimuth()
        {
            float wrappedAzimuth = Mathf.Repeat(AzimuthRadians, Mathf.PI * 2f);
            return new SphericalCoordinates(Radius, wrappedAzimuth, ElevationRadians);
        }

        /// <summary>
        /// Returns a new SphericalCoordinates struct with the elevation angle clamped to the range [-π/2, π/2].
        /// </summary>
        /// <returns>A new SphericalCoordinates struct with the clamped elevation angle.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates struct with the same radius and azimuth as the original,
        /// but with the elevation angle clamped to the range [-π/2, π/2]. If the original elevation is already within the range,
        /// it remains unchanged.
        ///
        /// Clamping the elevation ensures that it stays within the valid range, where -π/2 represents the negative y-axis,
        /// 0 represents the x-z plane, and π/2 represents the positive y-axis.
        /// </remarks>
        public SphericalCoordinates ClampElevation()
        {
            float clampedElevation = Mathf.Clamp(ElevationRadians, -Mathf.PI / 2f, Mathf.PI / 2f);
            return new SphericalCoordinates(Radius, AzimuthRadians, clampedElevation);
        }

        /// <summary>
        /// Returns a new SphericalCoordinates struct with the radius multiplied by the specified scale factor.
        /// </summary>
        /// <param name="scaleFactor">The scale factor to multiply the radius by.</param>
        /// <returns>A new SphericalCoordinates struct with the scaled radius.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates struct with the same azimuth and elevation as the original,
        /// but with the radius multiplied by the specified scale factor. This operation effectively scales the distance
        /// from the origin while preserving the direction.
        /// </remarks>
        public SphericalCoordinates ScaleRadius(float scaleFactor)
        {
            float scaledRadius = Radius * scaleFactor;
            return new SphericalCoordinates(scaledRadius, AzimuthRadians, ElevationRadians);
        }

        /// <summary>
        /// Returns a new SphericalCoordinates struct with the azimuth angle offset by the specified value in radians.
        /// </summary>
        /// <param name="offset">The azimuth angle offset in radians.</param>
        /// <returns>A new SphericalCoordinates struct with the offset azimuth angle.</returns>
        /// <remarks>
        /// This method creates a new SphericalCoordinates struct with the same radius and elevation as the original,
        /// but with the azimuth angle offset by the specified value in radians. Positive offset values rotate the azimuth
        /// counterclockwise in the x-z plane, while negative values rotate it clockwise.
        ///
        /// The resulting azimuth angle is automatically wrapped to the range [0, 2π] to ensure it represents a valid angle
        /// within a complete rotation in the x-z plane.
        /// </remarks>
        public SphericalCoordinates OffsetAzimuth(float offset)
        {
            float offsetAzimuth = AzimuthRadians + offset;
            return new SphericalCoordinates(Radius, offsetAzimuth, ElevationRadians).WrapAzimuth();
        }

        /// <summary>
        /// Adjusts the elevation angle with respect to the surface curvature of the sphere.
        /// </summary>
        /// <param name="angleChange">The change in elevation angle in radians.</param>
        /// <returns>A new SphericalCoordinates with the adjusted elevation.</returns>
        /// <remarks>
        /// This method adjusts the elevation in a way that more naturally follows the curvature of the sphere.
        /// It first converts to Cartesian coordinates, applies the elevation change, and then converts back.
        /// This prevents invalid elevation values and ensures smooth transitions in 3D space.
        /// </remarks>
        public SphericalCoordinates OffsetElevation(float angleChange)
        {
            // Calculate new elevation in Cartesian space and apply
            float newElevation = ElevationRadians + angleChange;
            float radiusAtNewElevation = Mathf.Cos(newElevation) * Radius;
            Vector3 adjustedVector = new Vector3(radiusAtNewElevation * Mathf.Cos(AzimuthRadians), Radius * Mathf.Sin(newElevation), radiusAtNewElevation * Mathf.Sin(AzimuthRadians));
            // Convert back to spherical coordinates
            return FromVector3(adjustedVector);
        }

        /// <summary>
        /// Returns a string representation of the spherical coordinates.
        /// </summary>
        /// <returns>A string representation of the spherical coordinates.</returns>
        /// <remarks>
        /// This method returns a string that describes the spherical coordinates in a human-readable format.
        /// The string includes the radius, azimuth, and elevation values, with the angles converted from radians to degrees
        /// for better readability.
        ///
        /// The string format is: "Radius: {radius}, Azimuth: {azimuth}°, Elevation: {elevation}°"
        /// </remarks>
        public override string ToString()
        {
            return $"Radius: {Radius}, Azimuth: {AzimuthRadians * Mathf.Rad2Deg}°, Elevation: {ElevationRadians * Mathf.Rad2Deg}°";
        }

        /// <summary>
        /// Performs a spherical linear interpolation between two SphericalCoordinates.
        /// </summary>
        /// <param name="target">The target SphericalCoordinates to interpolate towards.</param>
        /// <param name="t">The interpolation parameter between the two SphericalCoordinates. This value should be between 0 and 1.</param>
        /// <returns>The interpolated SphericalCoordinates.</returns>
        /// <remarks>
        /// This method performs a spherical linear interpolation (Slerp) between two SphericalCoordinates.
        /// Slerp is a type of interpolation that occurs on a unit sphere, and creates a smooth interpolation between two points.
        /// The resulting SphericalCoordinates represents the intermediate point between the two SphericalCoordinates, based on the interpolation parameter 't'.
        /// The Slerp algorithm uses quaternion interpolation internally to calculate the interpolated SphericalCoordinates.
        /// Quaternion.Slerp is used to perform the actual interpolation, with 't' specifying the interpolation factor.
        /// Note that 't' should typically be a value between 0 and 1 to produce valid results. Values outside this range might result in unexpected behavior.
        /// </remarks>
        public SphericalCoordinates Slerp (SphericalCoordinates target, float t) => new(Quaternion.Slerp(ToQuaternion(), target.ToQuaternion(), t));

        /// <summary>
        /// Determines whether the specified object is equal to the current SphericalCoordinates.
        /// </summary>
        /// <param name="obj">The object to compare with the current SphericalCoordinates.</param>
        /// <returns>true if the specified object is equal to the current SphericalCoordinates; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is SphericalCoordinates other)
                return Equals(other);
            return false;
        }

        /// <summary>
        /// Determines whether the specified SphericalCoordinates is equal to the current SphericalCoordinates.
        /// </summary>
        /// <param name="other">The SphericalCoordinates to compare with the current SphericalCoordinates.</param>
        /// <returns>true if the specified SphericalCoordinates is equal to the current SphericalCoordinates; otherwise, false.</returns>
        public bool Equals(SphericalCoordinates other)
        {
            return Radius == other.Radius &&
                   AzimuthRadians == other.AzimuthRadians &&
                   ElevationRadians == other.ElevationRadians;
        }

        /// <summary>
        /// Determines whether the current SphericalCoordinates is approximately equal to another SphericalCoordinates within a specified tolerance.
        /// </summary>
        /// <param name="other">The SphericalCoordinates to compare with the current SphericalCoordinates.</param>
        /// <param name="tolerance">The maximum allowed difference between the coordinates.</param>
        /// <returns>true if the current SphericalCoordinates is approximately equal to the other SphericalCoordinates; otherwise, false.</returns>
        public bool ApproximatelyEquals(SphericalCoordinates other, float tolerance = 1e-6f)
        {
            return Mathf.Abs(Radius - other.Radius) < tolerance &&
                   Mathf.Abs(Mathf.DeltaAngle(AzimuthRadians, other.AzimuthRadians)) < tolerance &&
                   Mathf.Abs(Mathf.DeltaAngle(ElevationRadians, other.ElevationRadians)) < tolerance;
        }

        /// <summary>
        /// Returns the hash code for this SphericalCoordinates.
        /// </summary>
        /// <returns>A hash code for the current SphericalCoordinates.</returns>
        public override int GetHashCode()
        {
            return Radius.GetHashCode() ^ AzimuthRadians.GetHashCode() ^ ElevationRadians.GetHashCode();
        }

        /// <summary>
        /// Determines whether two SphericalCoordinates instances are equal.
        /// </summary>
        /// <param name="a">The first SphericalCoordinates to compare.</param>
        /// <param name="b">The second SphericalCoordinates to compare.</param>
        /// <returns>true if the specified SphericalCoordinates are equal; otherwise, false.</returns>
        public static bool operator ==(SphericalCoordinates a, SphericalCoordinates b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two SphericalCoordinates instances are not equal.
        /// </summary>
        /// <param name="a">The first SphericalCoordinates to compare.</param>
        /// <param name="b">The second SphericalCoordinates to compare.</param>
        /// <returns>true if the specified SphericalCoordinates are not equal; otherwise, false.</returns>
        public static bool operator !=(SphericalCoordinates a, SphericalCoordinates b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Returns a new instance of the SphericalCoordinates struct with the negated radius.
        /// </summary>
        /// <returns>A new SphericalCoordinates struct with the negated radius.</returns>
        public SphericalCoordinates Negate()
        {
            return new SphericalCoordinates(-Radius, AzimuthRadians, ElevationRadians);
        }

        /// <summary>
        /// Returns a new instance of the SphericalCoordinates struct with the absolute value of the radius.
        /// </summary>
        /// <returns>A new SphericalCoordinates struct with the absolute value of the radius.</returns>
        public SphericalCoordinates Abs()
        {
            return new SphericalCoordinates(Mathf.Abs(Radius), AzimuthRadians, ElevationRadians);
        }

        /// <summary>
        /// Calculates the distance between two SphericalCoordinates.
        /// </summary>
        /// <param name="other">The other SphericalCoordinates to calculate the distance to.</param>
        /// <returns>The distance between the current and other SphericalCoordinates.</returns>
        public float Distance(SphericalCoordinates other)
        {
            return (ToVector3() - other.ToVector3()).magnitude;
        }

        /// <summary>
        /// Calculates the angle between two SphericalCoordinates in radians.
        /// </summary>
        /// <param name="other">The other SphericalCoordinates to calculate the angle to.</param>
        /// <returns>The angle between the current and other SphericalCoordinates in radians.</returns>
        public float AngleTo(SphericalCoordinates other)
        {
            Vector3 vec1 = ToVector3().normalized;
            Vector3 vec2 = other.ToVector3().normalized;
            return Mathf.Acos(Vector3.Dot(vec1, vec2));
        }
    }
}