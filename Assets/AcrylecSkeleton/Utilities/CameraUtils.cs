using UnityEngine;

namespace AcrylecSkeleton.Utilities
{
    /// <summary>
    /// Camera Utilities,
    /// Should be used to change resolution, it takes responsive cameras into account.
    /// </summary>
    public static class CameraUtils
    {
        /// <summary>
        /// Changes resolution to desired resolution struct, fullscreen choice is optional.
        /// </summary>
        /// <param name="newRes">Desired resolution.</param>
        /// <param name="isFullscreen">Is it fullscreen?</param>
        public static void ChangeResolution(Resolution newRes, bool isFullscreen = false)
        {
            Screen.SetResolution(newRes.width, newRes.height, isFullscreen, newRes.refreshRate);

            foreach (var responsiveCamera in Object.FindObjectsOfType<ResponsiveCamera>())
                responsiveCamera.OnResolutionChanged();
        }

        /// <summary>
        /// Changes resolution to desired width, height, refresh, and choice to choose fullscreen.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshRate"></param>
        /// <param name="isFullscreen"></param>
        public static void ChangeResolution(int width, int height, int refreshRate = 120, bool isFullscreen = false)
        {
            Resolution newRes = new Resolution
            {
                width = width,
                height = height,
                refreshRate = refreshRate
            };
            ChangeResolution(newRes, isFullscreen);
        }

        /// <summary>
        /// Changes resolution checking all supported resoulutions and chosing by index.
        /// </summary>
        /// <param name="index">Index in Screen.resolution[]</param>
        public static void ChangeResolution(int index)
        {
            ChangeResolution(Screen.resolutions[index]);
        }
    }
}