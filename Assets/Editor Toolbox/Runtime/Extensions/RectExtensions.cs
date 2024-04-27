using System;
using UnityEngine;

namespace Toolbox
{
    public static class RectExtensions
    {
        #region Alignment

        /// <summary>
        /// Aligns the right side of the Rect to a specified width.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newWidth">The new width of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignRight(this Rect rect, float newWidth)
        {
            rect.xMin = rect.xMax - newWidth;
            return rect;
        }

        /// <summary>
        /// Aligns the left side of the Rect to a specified width.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newWidth">The new width of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignLeft(this Rect rect, float newWidth)
        {
            rect.xMax = rect.xMin + newWidth;
            return rect;
        }

        /// <summary>
        /// Aligns the bottom side of the Rect to a specified height.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newHeight">The new height of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignBottom(this Rect rect, float newHeight)
        {
            rect.yMin = rect.yMax - newHeight;
            return rect;
        }

        /// <summary>
        /// Aligns the top side of the Rect to a specified height.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newHeight">The new height of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignTop(this Rect rect, float newHeight)
        {
            rect.yMax = rect.yMin + newHeight;
            return rect;
        }

        /// <summary>
        /// Aligns the Rect horizontally to a specified width, centering it.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newWidth">The new width of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignCenterX(this Rect rect, float newWidth)
        {
            float offset = (rect.width - newWidth) * 0.5f;
            rect.xMin += offset;
            rect.xMax -= offset;
            return rect;
        }

        /// <summary>
        /// Aligns the Rect vertically to a specified height, centering it.
        /// </summary>
        /// <param name="rect">The Rect to align.</param>
        /// <param name="newHeight">The new height of the Rect.</param>
        /// <returns>The aligned Rect.</returns>
        public static Rect AlignCenterY(this Rect rect, float newHeight)
        {
            float offset = (rect.height - newHeight) * 0.5f;
            rect.yMin += offset;
            rect.yMax -= offset;
            return rect;
        }

        #endregion

        #region Movement

        /// <summary>
        /// Moves the Rect horizontally by a specified offset and optional spacing.
        /// </summary>
        /// <param name="rect">The Rect to move.</param>
        /// <param name="offset">The horizontal offset to move the Rect by.</param>
        /// <param name="spacing">Optional spacing to add to the offset.</param>
        /// <returns>The moved Rect.</returns>
        public static Rect MoveByX(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.x += offset + spacing;
            return rect;
        }

        /// <summary>
        /// Moves the Rect vertically by a specified offset and optional spacing.
        /// </summary>
        /// <param name="rect">The Rect to move.</param>
        /// <param name="offset">The vertical offset to move the Rect by.</param>
        /// <param name="spacing">Optional spacing to add to the offset.</param>
        /// <returns>The moved Rect.</returns>
        public static Rect MoveByY(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.y += offset + spacing;
            return rect;
        }

        /// <summary>
        /// Moves the Rect by a specified translation vector.
        /// </summary>
        /// <param name="rect">The Rect to move.</param>
        /// <param name="translation">The translation vector to move the Rect by.</param>
        /// <returns>The moved Rect.</returns>
        public static Rect Translate(this Rect rect, Vector2 translation)
        {
            rect.x += translation.x;
            rect.y += translation.y;
            return rect;
        }

        #endregion

        #region Scaling

        /// <summary>
        /// Scales the Rect by a specified scale factor.
        /// </summary>
        /// <param name="rect">The Rect to scale.</param>
        /// <param name="scale">The scale factor to scale the Rect by.</param>
        /// <returns>The scaled Rect.</returns>
        public static Rect Scale(this Rect rect, Vector2 scale)
        {
            rect.width *= scale.x;
            rect.height *= scale.y;
            return rect;
        }

        /// <summary>
        /// Scales the Rect from its center by a specified scale factor.
        /// </summary>
        /// <param name="rect">The Rect to scale.</param>
        /// <param name="scale">The scale factor to scale the Rect by.</param>
        /// <returns>The scaled Rect.</returns>
        public static Rect ScaleFromCenter(this Rect rect, Vector2 scale)
        {
            float deltaWidth = rect.width * (scale.x - 1);
            float deltaHeight = rect.height * (scale.y - 1);
            rect.x -= deltaWidth * 0.5f;
            rect.y -= deltaHeight * 0.5f;
            rect.width *= scale.x;
            rect.height *= scale.y;
            return rect;
        }

        #endregion

        #region Modification

        /// <summary>
        /// Creates a new Rect with a modified minimum X value.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="xMin">The new minimum X value.</param>
        /// <returns>A new Rect with the modified minimum X value.</returns>
        public static Rect WithXMin(this Rect rect, float xMin)
        {
            return new Rect(xMin, rect.yMin, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified minimum Y value.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="yMin">The new minimum Y value.</param>
        /// <returns>A new Rect with the modified minimum Y value.</returns>
        public static Rect WithYMin(this Rect rect, float yMin)
        {
            return new Rect(rect.xMin, yMin, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified width.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="width">The new width.</param>
        /// <returns>A new Rect with the modified width.</returns>
        public static Rect WithWidth(this Rect rect, float width)
        {
            return new Rect(rect.xMin, rect.yMin, width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified height.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="height">The new height.</param>
        /// <returns>A new Rect with the modified height.</returns>
        public static Rect WithHeight(this Rect rect, float height)
        {
            return new Rect(rect.xMin, rect.yMin, rect.width, height);
        }

        /// <summary>
        /// Creates a new Rect with a modified maximum X value.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="xMax">The new maximum X value.</param>
        /// <returns>A new Rect with the modified maximum X value.</returns>
        public static Rect WithXMax(this Rect rect, float xMax)
        {
            return new Rect(xMax - rect.width, rect.yMin, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified maximum Y value.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="yMax">The new maximum Y value.</param>
        /// <returns>A new Rect with the modified maximum Y value.</returns>
        public static Rect WithYMax(this Rect rect, float yMax)
        {
            return new Rect(rect.xMin, yMax - rect.height, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified X position.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="x">The new X position.</param>
        /// <returns>A new Rect with the modified X position.</returns>
        public static Rect WithX(this Rect rect, float x)
        {
            return new Rect(x, rect.y, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified Y position.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="y">The new Y position.</param>
        /// <returns>A new Rect with the modified Y position.</returns>
        public static Rect WithY(this Rect rect, float y)
        {
            return new Rect(rect.x, y, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified position.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="position">The new position.</param>
        /// <returns>A new Rect with the modified position.</returns>
        public static Rect WithPosition(this Rect rect, Vector2 position)
        {
            return new Rect(position.x, position.y, rect.width, rect.height);
        }

        /// <summary>
        /// Creates a new Rect with a modified size.
        /// </summary>
        /// <param name="rect">The original Rect.</param>
        /// <param name="size">The new size.</param>
        /// <returns>A new Rect with the modified size.</returns>
        public static Rect WithSize(this Rect rect, Vector2 size)
        {
            return new Rect(rect.x, rect.y, size.x, size.y);
        }

        #endregion

        #region Corners

        /// <summary>
        /// Gets the top-left corner of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The top-left corner of the Rect.</returns>
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMax);
        }

        /// <summary>
        /// Gets the top-right corner of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The top-right corner of the Rect.</returns>
        public static Vector2 TopRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }

        /// <summary>
        /// Gets the bottom-left corner of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The bottom-left corner of the Rect.</returns>
        public static Vector2 BottomLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        /// <summary>
        /// Gets the bottom-right corner of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The bottom-right corner of the Rect.</returns>
        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMin);
        }

        /// <summary>
        /// Gets the center of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The center of the Rect.</returns>
        public static Vector2 Center(this Rect rect)
        {
            return new Vector2(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f);
        }

        /// <summary>
        /// Gets the top-center point of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The top-center point of the Rect.</returns>
        public static Vector2 TopCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width * 0.5f, rect.yMax);
        }

        /// <summary>
        /// Gets the bottom-center point of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The bottom-center point of the Rect.</returns>
        public static Vector2 BottomCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width * 0.5f, rect.yMin);
        }

        /// <summary>
        /// Gets the left-center point of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The left-center point of the Rect.</returns>
        public static Vector2 LeftCenter(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.y + rect.height * 0.5f);
        }

        /// <summary>
        /// Gets the right-center point of the Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>The right-center point of the Rect.</returns>
        public static Vector2 RightCenter(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.y + rect.height * 0.5f);
        }

        #endregion

        /// <summary>
        /// Encapsulates a point within the Rect, expanding the Rect if necessary.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="point">The point to encapsulate.</param>
        /// <returns>A new Rect that encapsulates the point.</returns>
        public static Rect Encapsulate(this Rect rect, Vector2 point)
        {
            float xMin = Mathf.Min(rect.xMin, point.x);
            float yMin = Mathf.Min(rect.yMin, point.y);
            float xMax = Mathf.Max(rect.xMax, point.x);
            float yMax = Mathf.Max(rect.yMax, point.y);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// Encapsulates the current Rect to include the specified point.
        /// </summary>
        /// <param name="rect">The Rect to encapsulate.</param>
        /// <param name="point">The point to include in the encapsulated Rect.</param>
        /// <returns>The new encapsulated Rect.</returns>
        public static Rect Encapsulate(this Rect rect, Rect other)
        {
            float xMin = Mathf.Min(rect.xMin, other.xMin);
            float yMin = Mathf.Min(rect.yMin, other.yMin);
            float xMax = Mathf.Max(rect.xMax, other.xMax);
            float yMax = Mathf.Max(rect.yMax, other.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// Returns the intersection of two Rectangles.
        /// </summary>
        /// <param name="rect">The first Rectangle.</param>
        /// <param name="other">The second Rectangle.</param>
        /// <returns>The intersection of the two Rectangles.</returns>
        public static Rect Intersection(this Rect rect, Rect other)
        {
            float xMin = Mathf.Max(rect.xMin, other.xMin);
            float yMin = Mathf.Max(rect.yMin, other.yMin);
            float xMax = Mathf.Min(rect.xMax, other.xMax);
            float yMax = Mathf.Min(rect.yMax, other.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// Retrieves the world corner positions of a Rect in counter-clockwise order starting from the bottom-left corner.
        /// </summary>
        /// <param name="rect">The Rect whose world corner positions are to be retrieved.</param>
        /// <param name="corners">An array of Vector3 to store the world corner positions.</param>
        public static void GetWorldCorners(this Rect rect, Vector3[] corners)
        {
            if (corners.Length < 4)
                throw new ArgumentException("Array is not long enough to hold all corners of the Rect.");
            corners[0] = new Vector3(rect.xMin, rect.yMin);
            corners[1] = new Vector3(rect.xMax, rect.yMin);
            corners[2] = new Vector3(rect.xMax, rect.yMax);
            corners[3] = new Vector3(rect.xMin, rect.yMax);
        }

        /// <summary>
        /// Retrieves the world corner positions of a Rect in counter-clockwise order starting from the bottom-left corner.
        /// </summary>
        /// <param name="rect">The Rect whose world corner positions are to be retrieved.</param>
        /// <returns>An array of Vector3 representing the world corner positions.</returns>
        public static Vector3[] GetWorldCorners(this Rect rect)
        {
            Vector3[] corners = new Vector3[4];
            GetWorldCorners(rect, corners);
            return corners;
        }

        /// <summary>
        /// Rotates the given Rect by a specified angle.
        /// </summary>
        /// <param name="rect">The Rect to rotate.</param>
        /// <param name="angle">The angle in degrees to rotate the Rect.</param>
        /// <returns>The rotated Rect.</returns>
        public static Rect Rotate(this Rect rect, float angle)
        {
            Vector2 center = rect.center;
            Matrix4x4 m = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++) corners[i] = m.MultiplyPoint3x4(corners[i]);
            return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }

        /// <summary>
        /// Flips the Rect horizontally, changing its x-position and negating its width.
        /// </summary>
        /// <param name="rect">The Rect to flip.</param>
        /// <returns>The flipped Rect.</returns>
        public static Rect FlipHorizontal(this Rect rect) => new(rect.xMax, rect.yMin, -rect.width, rect.height);

        /// <summary>
        /// Flips the Rect vertically by changing the y-coordinate and height values.
        /// </summary>
        /// <param name="rect">The Rect to flip.</param>
        /// <returns>The flipped Rect.</returns>
        public static Rect FlipVertical(this Rect rect) => new(rect.xMin, rect.yMax, rect.width, -rect.height);

        /// <summary>
        /// Offsets the position of a Rect by a specified vector.
        /// </summary>
        /// <param name="rect">The Rect to offset.</param>
        /// <param name="offset">The vector that specifies the amount of offset in each direction.</param>
        /// <returns>The Rect with the offset applied.</returns>
        public static Rect Offset(this Rect rect, Vector2 offset) => new(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);

        /// <summary>
        /// Shrinks the Rect by reducing its width and height while maintaining its position.
        /// </summary>
        /// <param name="rect">The Rect to shrink.</param>
        /// <param name="amount">The amount to shrink the Rect by.</param>
        /// <returns>The shrunk Rect.</returns>
        public static Rect Shrink(this Rect rect, float amount) => new(rect.x + amount, rect.y + amount, rect.width - 2 * amount, rect.height - 2 * amount);

        /// <summary>
        /// Returns a value indicating whether the specified point is contained within the Rect.
        /// </summary>
        /// <param name="rect">The Rect in which to check for containment.</param>
        /// <param name="point">The point to check for containment.</param>
        /// <returns>
        /// <c>true</c> if the point is contained within the Rect; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this Rect rect, Vector2 point) => point.x >= rect.xMin && point.x <= rect.xMax && point.y >= rect.yMin && point.y <= rect.yMax;

        /// <summary>
        /// Calculates the union of two rectangles.
        /// </summary>
        /// <param name="rect">The first rectangle.</param>
        /// <param name="other">The second rectangle.</param>
        /// <returns>The union of the two rectangles.</returns>
        public static Rect Union(this Rect rect, Rect other)
        {
            float xMin = Mathf.Min(rect.xMin, other.xMin);
            float yMin = Mathf.Min(rect.yMin, other.yMin);
            float xMax = Mathf.Max(rect.xMax, other.xMax);
            float yMax = Mathf.Max(rect.yMax, other.yMax);
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// Expands or contracts the Rect by the specified amounts on each side.
        /// </summary>
        /// <param name="rect">The Rect to inflate.</param>
        /// <param name="horizontalAmount">The amount to expand or contract the Rect horizontally.</param>
        /// <param name="verticalAmount">The amount to expand or contract the Rect vertically.</param>
        /// <returns>The inflated Rect.</returns>
        public static Rect Inflate(this Rect rect, float horizontalAmount, float verticalAmount)
        {
            return new Rect(rect.x - horizontalAmount, rect.y - verticalAmount, rect.width + horizontalAmount * 2, rect.height + verticalAmount * 2);
        }

        /// <summary>
        /// Expands or contracts the Rect by the specified amount on each side.
        /// </summary>
        /// <param name="rect">The Rect to inflate.</param>
        /// <param name="amount">The amount to expand or contract the Rect.</param>
        /// <returns>The inflated Rect.</returns>
        public static Rect Inflate(this Rect rect, Vector2 amount)
        {
            return Inflate(rect, amount.x, amount.y);
        }

        /// <summary>
        /// Contracts the Rect by the specified amount on each side.
        /// </summary>
        /// <param name="rect">The Rect to deflate.</param>
        /// <param name="amount">The amount to contract the Rect.</param>
        /// <returns>The deflated Rect.</returns>
        public static Rect Deflate(this Rect rect, float amount)
        {
            return Inflate(rect, -amount, -amount);
        }

        /// <summary>
        /// Contracts the Rect by the specified amounts on each side.
        /// </summary>
        /// <param name="rect">The Rect to deflate.</param>
        /// <param name="horizontalAmount">The amount to contract the Rect horizontally.</param>
        /// <param name="verticalAmount">The amount to contract the Rect vertically.</param>
        /// <returns>The deflated Rect.</returns>
        public static Rect Deflate(this Rect rect, float horizontalAmount, float verticalAmount)
        {
            return Inflate(rect, -horizontalAmount, -verticalAmount);
        }

        /// <summary>
        /// Linearly interpolates between two Rects by the specified interpolation factor.
        /// </summary>
        /// <param name="from">The starting Rect.</param>
        /// <param name="to">The ending Rect.</param>
        /// <param name="t">The interpolation factor between 0 and 1.</param>
        /// <returns>The interpolated Rect.</returns>
        public static Rect Lerp(Rect from, Rect to, float t)
        {
            t = Mathf.Clamp01(t);
            return new Rect(
                Mathf.Lerp(from.x, to.x, t),
                Mathf.Lerp(from.y, to.y, t),
                Mathf.Lerp(from.width, to.width, t),
                Mathf.Lerp(from.height, to.height, t)
            );
        }

        /// <summary>
        /// Determines whether the Rect intersects with another Rect.
        /// </summary>
        /// <param name="rect">The first Rect.</param>
        /// <param name="other">The second Rect.</param>
        /// <returns>True if the Rects intersect; otherwise, false.</returns>
        public static bool IntersectsWith(this Rect rect, Rect other)
        {
            return rect.Overlaps(other);
        }

        /// <summary>
        /// Determines whether the Rect completely contains another Rect.
        /// </summary>
        /// <param name="rect">The containing Rect.</param>
        /// <param name="other">The contained Rect.</param>
        /// <returns>True if the Rect completely contains the other Rect; otherwise, false.</returns>
        public static bool ContainsRect(this Rect rect, Rect other)
        {
            return rect.Contains(other.min) && rect.Contains(other.max);
        }

        /// <summary>
        /// Centers the Rect within another Rect.
        /// </summary>
        /// <param name="rect">The Rect to center.</param>
        /// <param name="other">The Rect to center within.</param>
        /// <returns>The centered Rect.</returns>
        public static Rect CenterIn(this Rect rect, Rect other)
        {
            float x = other.center.x - rect.width * 0.5f;
            float y = other.center.y - rect.height * 0.5f;
            return new Rect(x, y, rect.width, rect.height);
        }

        /// <summary>
        /// Resizes the Rect to fit inside another Rect while maintaining its aspect ratio.
        /// </summary>
        /// <param name="rect">The Rect to resize.</param>
        /// <param name="other">The Rect to fit inside.</param>
        /// <returns>The resized Rect.</returns>
        public static Rect FitInside(this Rect rect, Rect other)
        {
            float aspectRatio = rect.width / rect.height;
            float targetAspectRatio = other.width / other.height;

            if (aspectRatio > targetAspectRatio)
            {
                float width = other.width;
                float height = width / aspectRatio;
                float y = other.y + (other.height - height) * 0.5f;
                return new Rect(other.x, y, width, height);
            }
            else
            {
                float height = other.height;
                float width = height * aspectRatio;
                float x = other.x + (other.width - width) * 0.5f;
                return new Rect(x, other.y, width, height);
            }
        }

        /// <summary>
        /// Resizes the Rect to fit outside another Rect while maintaining its aspect ratio.
        /// </summary>
        /// <param name="rect">The Rect to resize.</param>
        /// <param name="other">The Rect to fit outside.</param>
        /// <returns>The resized Rect.</returns>
        public static Rect FitOutside(this Rect rect, Rect other)
        {
            float aspectRatio = rect.width / rect.height;
            float targetAspectRatio = other.width / other.height;

            if (aspectRatio < targetAspectRatio)
            {
                float width = other.width;
                float height = width / aspectRatio;
                float y = other.y + (other.height - height) * 0.5f;
                return new Rect(other.x, y, width, height);
            }
            else
            {
                float height = other.height;
                float width = height * aspectRatio;
                float x = other.x + (other.width - width) * 0.5f;
                return new Rect(x, other.y, width, height);
            }
        }
    }
}