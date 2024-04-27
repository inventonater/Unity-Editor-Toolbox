using UnityEngine;

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
        public float Azimuth;

        /// <summary>
        /// The elevation angle in radians, measured from the x-z plane to the point.
        /// </summary>
        public float Elevation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SphericalCoordinates"/> struct with the specified radius, azimuth, and elevation.
        /// </summary>
        /// <param name="radius">The radial distance from the origin.</param>
        /// <param name="azimuth">The azimuth angle in radians, measured in the x-z plane from the positive x-axis.</param>
        /// <param name="elevation">The elevation angle in radians, measured from the x-z plane.</param>
        public SphericalCoordinates(float radius, float azimuth, float elevation)
        {
            Radius = Mathf.Max(0, radius); // Ensure that radius is never negative.
            Azimuth = azimuth;
            Elevation = Mathf.Clamp(elevation, -Mathf.PI / 2, Mathf.PI / 2); // Clamp the elevation to valid range.
        }

        public SphericalCoordinates(SphericalCoordinates sphericalCoordinates)
        {
            Radius = sphericalCoordinates.Radius;
            Azimuth = sphericalCoordinates.Azimuth;
            Elevation = sphericalCoordinates.Elevation;
        }
        public SphericalCoordinates(Vector3 vector) : this(FromVector3(vector)) { }
        public SphericalCoordinates(Direction direction) : this(FromDirection(direction)) { }
        public SphericalCoordinates(Quaternion quaternion) : this(new Direction(quaternion)) { }

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
            float x = Radius * Mathf.Cos(Elevation) * Mathf.Cos(Azimuth);
            float y = Radius * Mathf.Sin(Elevation);
            float z = Radius * Mathf.Cos(Elevation) * Mathf.Sin(Azimuth);

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
            float projectedRadius = Radius * Mathf.Cos(Elevation);
            return new PolarCoordinates(projectedRadius, Azimuth);
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
            return new SphericalCoordinates(clampedRadius, Azimuth, Elevation);
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
            float wrappedAzimuth = Mathf.Repeat(Azimuth, Mathf.PI * 2f);
            return new SphericalCoordinates(Radius, wrappedAzimuth, Elevation);
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
            float clampedElevation = Mathf.Clamp(Elevation, -Mathf.PI / 2f, Mathf.PI / 2f);
            return new SphericalCoordinates(Radius, Azimuth, clampedElevation);
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
            return new SphericalCoordinates(scaledRadius, Azimuth, Elevation);
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
            float offsetAzimuth = Azimuth + offset;
            return new SphericalCoordinates(Radius, offsetAzimuth, Elevation).WrapAzimuth();
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
            float newElevation = Elevation + angleChange;
            float radiusAtNewElevation = Mathf.Cos(newElevation) * Radius;
            Vector3 adjustedVector = new Vector3(radiusAtNewElevation * Mathf.Cos(Azimuth), Radius * Mathf.Sin(newElevation), radiusAtNewElevation * Mathf.Sin(Azimuth));
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
            return $"Radius: {Radius}, Azimuth: {Azimuth * Mathf.Rad2Deg}°, Elevation: {Elevation * Mathf.Rad2Deg}°";
        }
    }
}