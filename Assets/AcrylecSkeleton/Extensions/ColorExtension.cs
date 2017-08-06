using UnityEngine;

namespace AcrylecSkeleton.Extensions
{
    public static class ColorExtension
    {
        private static Color ChangeAlpha(Color color, float alpha)
        {
            Color temp = color;
            temp.a = alpha;
            return temp;
        }

        public static Color ChangeAlpha(this SpriteRenderer renderer, float alpha)
        {
            if (renderer == null)
                return new Color(1, 1, 1, alpha);

            Color temp = ChangeAlpha(renderer.color, alpha);
            renderer.color = temp;
            return temp;
        }

        public static Color ChangeAlpha(this TrailRenderer renderer, float alpha)
        {
            if (renderer == null)
                return new Color(1, 1, 1, alpha);

            Color temp = ChangeAlpha(renderer.startColor, alpha);
            renderer.startColor = temp;

            temp = ChangeAlpha(renderer.endColor, alpha);
            renderer.endColor = temp;
            return temp;
        }

        public static Color ChangeAlpha(this LineRenderer renderer, float alpha)
        {
            if (renderer == null)
                return new Color(1, 1, 1, alpha);

            Color temp = ChangeAlpha(renderer.startColor, alpha);
            renderer.startColor = temp;

            temp = ChangeAlpha(renderer.endColor, alpha);
            renderer.endColor = temp;
            return temp;
        }

    }
}