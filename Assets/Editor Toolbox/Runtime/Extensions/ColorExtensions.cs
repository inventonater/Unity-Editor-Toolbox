using UnityEngine;

namespace Toolbox
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float a) => new(color.r, color.g, color.b, a);
    }
}
