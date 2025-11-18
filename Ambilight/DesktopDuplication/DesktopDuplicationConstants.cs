namespace Ambilight.DesktopDuplication
{
    /// <summary>
    /// Constants for desktop duplication and screen capture
    /// </summary>
    public static class DesktopDuplicationConstants
    {
        /// <summary>
        /// Mipmap level for GPU downscaling (2 = 4x reduction, 3 = 8x reduction)
        /// Higher values = more downscaling = better performance but lower quality
        /// </summary>
        public const int MipMapLevel = 2;

        /// <summary>
        /// Scaling factor calculated from mipmap level (1 << MipMapLevel)
        /// For MipMapLevel=2, this is 4 (divides resolution by 4)
        /// </summary>
        public const int ScalingFactor = 1 << MipMapLevel;

        /// <summary>
        /// Timeout in milliseconds for waiting for next frame
        /// </summary>
        public const int FrameAcquireTimeoutMs = 500;

        /// <summary>
        /// Ultrawide monitor aspect ratio constants
        /// </summary>
        public static class UltrawideConversion
        {
            /// <summary>
            /// Source aspect ratio divisor (21:9 ultrawide)
            /// </summary>
            public const double SourceAspectDivisor = 21.0;

            /// <summary>
            /// Horizontal offset multiplier (crops 2.5/21 from each side)
            /// </summary>
            public const double HorizontalOffsetMultiplier = 2.5;

            /// <summary>
            /// Target width multiplier (16:9 standard)
            /// </summary>
            public const double TargetWidthMultiplier = 16.0;

            /// <summary>
            /// Minimum image width required for ultrawide cropping
            /// </summary>
            public const int MinimumWidth = 21;
        }
    }
}
