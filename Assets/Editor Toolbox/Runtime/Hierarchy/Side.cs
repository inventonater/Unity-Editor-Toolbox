using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents the side or orientation of an object or component.
    /// </summary>
    [System.Flags]
    public enum Side
    {
        /// <summary>
        /// Indicates no specific side or orientation.
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents the left side or orientation.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Represents the right side or orientation.
        /// </summary>
        Right = 2,
        /// <summary>
        /// Represents both the left and right sides or orientations.
        /// </summary>
        Both = Left | Right
    }

    /// <summary>
    /// Provides extension methods for the Side enum.
    /// </summary>
    public static class ExtensionsSide
    {
        /// <summary>
        /// Determines the Side of a component based on its name.
        /// </summary>
        /// <param name="component">The component to determine the Side for.</param>
        /// <returns>The Side of the component based on its name, or Side.None if the component is null or the Side cannot be determined.</returns>
        public static Side SideFromName(this Component component)
        {
            if (component == null)
            {
                Debug.LogWarning("Component is null");
                return Side.None;
            }

            string name = component.name.ToLower();
            if (name.Contains("left")) return Side.Left;
            if (name.Contains("right")) return Side.Right;
            if (name.Contains("both")) return Side.Both;

            var parent = component.transform.parent;
            return parent != null ? parent.SideFromName() : Side.None;
        }

        /// <summary>
        /// Checks if the Side is Side.None.
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side is Side.None, false otherwise.</returns>
        public static bool IsNone(this Side side) => side == Side.None;

        /// <summary>
        /// Checks if the Side has a valid value (not Side.None).
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side has a valid value, false if it is Side.None.</returns>
        public static bool HasValue(this Side side) => side != Side.None;

        /// <summary>
        /// Checks if the Side includes the left side or orientation.
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side includes the left side or orientation, false otherwise.</returns>
        public static bool IsLeft(this Side side) => (side & Side.Left) == Side.Left;

        /// <summary>
        /// Checks if the Side includes the right side or orientation.
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side includes the right side or orientation, false otherwise.</returns>
        public static bool IsRight(this Side side) => (side & Side.Right) == Side.Right;

        /// <summary>
        /// Checks if the Side is either the left or right side or orientation.
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side is either the left or right side or orientation, false otherwise.</returns>
        public static bool IsLeftOrRight(this Side side) => side.IsLeft() || side.IsRight();

        /// <summary>
        /// Checks if the Side includes both the left and right sides or orientations.
        /// </summary>
        /// <param name="side">The Side to check.</param>
        /// <returns>True if the Side includes both the left and right sides or orientations, false otherwise.</returns>
        public static bool IsBoth(this Side side) => side == Side.Both;

        /// <summary>
        /// Returns the opposite Side of the specified Side.
        /// </summary>
        /// <param name="side">The Side to get the opposite of.</param>
        /// <returns>The opposite Side of the specified Side.</returns>
        public static Side Opposite(this Side side)
        {
            switch (side)
            {
                case Side.Left: return Side.Right;
                case Side.Right: return Side.Left;
                case Side.Both: return Side.None;
                default: return Side.None;
            }
        }

        /// <summary>
        /// Adds the specified Side to the current Side.
        /// </summary>
        /// <param name="side">The current Side.</param>
        /// <param name="other">The Side to add.</param>
        /// <returns>The resulting Side after adding the specified Side.</returns>
        public static Side Add(this Side side, Side other)
        {
            if (side == Side.None) return other;
            if (other == Side.None) return side;
            return Side.Both;
        }

        /// <summary>
        /// Removes the specified Side from the current Side.
        /// </summary>
        /// <param name="side">The current Side.</param>
        /// <param name="other">The Side to remove.</param>
        /// <returns>The resulting Side after removing the specified Side.</returns>
        public static Side Remove(this Side side, Side other)
        {
            if (side == Side.None || other == Side.None) return side;
            if (side == other) return Side.None;
            return side;
        }

        /// <summary>
        /// Returns a string representation of the Side.
        /// </summary>
        /// <param name="side">The Side to get the string representation of.</param>
        /// <returns>A string representation of the Side.</returns>
        public static string ToString(this Side side)
        {
            switch (side)
            {
                case Side.Left: return "Left";
                case Side.Right: return "Right";
                case Side.Both: return "Both";
                default: return "None";
            }
        }
    }
}