using System;
using UnityEngine;

namespace Toolbox
{
    public static class RectExtensions
    {
        public static Rect AlignRight(this Rect rect, float newWidth)
        {
            rect.xMin = rect.xMax - newWidth;
            return rect;
        }

        public static Rect AlignLeft(this Rect rect, float newWidth)
        {
            rect.xMax = rect.xMin + newWidth;
            return rect;
        }

        public static Rect AlignBottom(this Rect rect, float newHeight)
        {
            rect.yMin = rect.yMax - newHeight;
            return rect;
        }

        public static Rect AlignTop(this Rect rect, float newHeight)
        {
            rect.yMax = rect.yMin + newHeight;
            return rect;
        }

        public static Rect AlignCenterX(this Rect rect, float newWidth)
        {
            var offset = (rect.width - newWidth) / 2;
            rect.xMin += offset;
            rect.xMax -= offset;
            return rect;
        }

        public static Rect AlignCenterY(this Rect rect, float newHeight)
        {
            var offset = (rect.height - newHeight) / 2;
            rect.yMin += offset;
            rect.yMax -= offset;
            return rect;
        }

        public static Rect MoveByX(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.x += offset + spacing;
            return rect;
        }

        public static Rect MoveByY(this Rect rect, float offset, float spacing = 0.0f)
        {
            rect.y += offset + spacing;
            return rect;
        }

        public static Rect AddMax(this Rect rect, Vector2 range)
        {
            rect.xMax += range.x;
            rect.yMin += range.y;
            return rect;
        }

        public static Rect AddMin(this Rect rect, Vector2 range)
        {
            rect.xMin += range.x;
            rect.yMin += range.y;
            return rect;
        }

        public static Rect SubMax(this Rect rect, Vector2 range)
        {
            rect.xMax -= range.x;
            rect.yMin -= range.y;
            return rect;
        }

        public static Rect SubMin(this Rect rect, Vector2 range)
        {
            rect.xMin -= range.x;
            rect.yMin -= range.y;
            return rect;
        }


        public static Rect WithXMin(this Rect rect, float xMin) => new(xMin, rect.yMin, rect.width, rect.height);
        public static Rect WithYMin(this Rect rect, float yMin) => new(rect.xMin, yMin, rect.width, rect.height);
        public static Rect WithWidth(this Rect rect, float width) => new(rect.xMin, rect.yMin, width, rect.height);
        public static Rect WithHeight(this Rect rect, float height) => new(rect.xMin, rect.yMin, rect.width, height);
        public static Rect WithXMax(this Rect rect, float xMax) => new(xMax - rect.width, rect.yMin, rect.width, rect.height);
        public static Rect WithYMax(this Rect rect, float yMax) => new(rect.xMin, yMax - rect.height, rect.width, rect.height);
        public static Vector2 TopLeft(this Rect rect) => new(rect.xMin, rect.yMax);
        public static Vector2 TopRight(this Rect rect) => new(rect.xMax, rect.yMax);
        public static Vector2 BottomLeft(this Rect rect) => new(rect.xMin, rect.yMin);
        public static Vector2 BottomRight(this Rect rect) => new(rect.xMax, rect.yMin);
        public static Vector2 Center(this Rect rect) => new(rect.x + (rect.width * 0.5f), rect.y + (rect.height * 0.5f));
        public static Rect Inflate(this Rect rect, float amount) => new(rect.x - amount, rect.y - amount, rect.width + (amount * 2), rect.height + (amount * 2));
        public static bool Contains(this Rect rect, Rect other) => rect.xMin <= other.xMin && rect.yMin <= other.yMin && rect.xMax >= other.xMax && rect.yMax >= other.yMax;
        public static bool Intersects(this Rect rect, Rect other) => rect.xMin < other.xMax && rect.xMax > other.xMin && rect.yMin < other.yMax && rect.yMax > other.yMin;
        public static Rect WithPosition(this Rect rect, Vector2 position) => new(position.x, position.y, rect.width, rect.height);
        public static Rect WithSize(this Rect rect, Vector2 size) => new(rect.x, rect.y, size.x, size.y);

        public static Rect Encapsulate(this Rect rect, Vector2 point)
        {
            float xMin = Mathf.Min(rect.xMin, point.x);
            float yMin = Mathf.Min(rect.yMin, point.y);
            float xMax = Mathf.Max(rect.xMax, point.x);
            float yMax = Mathf.Max(rect.yMax, point.y);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static Rect Encapsulate(this Rect rect, Rect other)
        {
            float xMin = Mathf.Min(rect.xMin, other.xMin);
            float yMin = Mathf.Min(rect.yMin, other.yMin);
            float xMax = Mathf.Max(rect.xMax, other.xMax);
            float yMax = Mathf.Max(rect.yMax, other.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static Rect Intersection(this Rect rect, Rect other)
        {
            float xMin = Mathf.Max(rect.xMin, other.xMin);
            float yMin = Mathf.Max(rect.yMin, other.yMin);
            float xMax = Mathf.Min(rect.xMax, other.xMax);
            float yMax = Mathf.Min(rect.yMax, other.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static Vector2 TopCenter(this Rect rect) => new(rect.x + (rect.width * 0.5f), rect.yMax);
        public static Vector2 BottomCenter(this Rect rect) => new(rect.x + (rect.width * 0.5f), rect.yMin);
        public static Vector2 LeftCenter(this Rect rect) => new(rect.xMin, rect.y + (rect.height * 0.5f));
        public static Vector2 RightCenter(this Rect rect) => new(rect.xMax, rect.y + (rect.height * 0.5f));
        public static Rect WithX(this Rect rect, float x) => new(x, rect.y, rect.width, rect.height);
        public static Rect WithY(this Rect rect, float y) => new(rect.x, y, rect.width, rect.height);
        public static Rect Translate(this Rect rect, Vector2 translation) => new(rect.x + translation.x, rect.y + translation.y, rect.width, rect.height);
        public static Rect Scale(this Rect rect, Vector2 scale) => new(rect.x, rect.y, rect.width * scale.x, rect.height * scale.y);

        public static Rect ScaleFromCenter(this Rect rect, Vector2 scale)
        {
            float deltaWidth = rect.width * (scale.x - 1);
            float deltaHeight = rect.height * (scale.y - 1);
            return new Rect(rect.x - (deltaWidth * 0.5f), rect.y - (deltaHeight * 0.5f), rect.width * scale.x, rect.height * scale.y);
        }

        public static void GetWorldCorners(this Rect rect, Vector3[] corners)
        {
            if (corners.Length < 4)
                throw new ArgumentException("Array is not long enough to hold all corners of the Rect.");
            corners[0] = new Vector3(rect.xMin, rect.yMin);
            corners[1] = new Vector3(rect.xMax, rect.yMin);
            corners[2] = new Vector3(rect.xMax, rect.yMax);
            corners[3] = new Vector3(rect.xMin, rect.yMax);
        }

        public static Rect Rotate(this Rect rect, float angle)
        {
            Vector2 center = rect.center;
            Matrix4x4 m = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++) corners[i] = m.MultiplyPoint3x4(corners[i]);
            return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }

        public static Rect FlipHorizontal(this Rect rect) => new(rect.xMax, rect.yMin, -rect.width, rect.height);
        public static Rect FlipVertical(this Rect rect) => new(rect.xMin, rect.yMax, rect.width, -rect.height);
        public static Rect Offset(this Rect rect, Vector2 offset) => new(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);
        public static Rect Shrink(this Rect rect, float amount) => new(rect.x + amount, rect.y + amount, rect.width - 2 * amount, rect.height - 2 * amount);
        public static bool Contains(this Rect rect, Vector2 point) => point.x >= rect.xMin && point.x <= rect.xMax && point.y >= rect.yMin && point.y <= rect.yMax;

        public static Rect Union(this Rect rect, Rect other)
        {
            float xMin = Mathf.Min(rect.xMin, other.xMin);
            float yMin = Mathf.Min(rect.yMin, other.yMin);
            float xMax = Mathf.Max(rect.xMax, other.xMax);
            float yMax = Mathf.Max(rect.yMax, other.yMax);
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
