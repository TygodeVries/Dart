using Runtime.Graphics;

namespace Project.Editor
{
    internal class DefaultsTextures
    {
        private static ImageTexture? fallbackTexture;
        /// <summary>
        /// A fallback texture that will be used in case an error has occured.
        /// </summary>
        /// <returns></returns>
        public static ImageTexture GetFallbackTexture()
        {
            if (fallbackTexture == null)
            {
                fallbackTexture = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/Unknown.png"));
            }

            return fallbackTexture;
        }

        private static ImageTexture? loadingTexture;

        /// <summary>
        /// A generic texture used for loading
        /// </summary>
        /// <returns></returns>
        public static ImageTexture GetLoadingTexture()
        {
            if (loadingTexture == null)
            {
                loadingTexture = ImageTexture.LoadFromPng(EditorUtils.GetAssetDatabase().GetAsset("assets/textures/icons/loading.png"));
            }

            return loadingTexture;
        }
    }
}
