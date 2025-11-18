namespace Ambilight.Logic
{
    /// <summary>
    /// Constants for Razer device LED configurations
    /// </summary>
    public static class DeviceConstants
    {
        /// <summary>
        /// Mousepad LED configuration
        /// The mousepad has 15 LEDs arranged around the edges
        /// </summary>
        public static class Mousepad
        {
            public const int GridWidth = 7;
            public const int GridHeight = 6;
            public const int TotalLeds = 15;

            // LED position mapping (for non-ambi mode)
            public const int RightEdgeMaxIndex = 3;      // LEDs 0-3 (right side)
            public const int BottomRightCornerIndex = 4; // LED 4 (corner)
            public const int BottomEdgeStartX = 5;       // Bottom edge starts at x=5
            public const int BottomEdgeEndX = 0;         // Bottom edge ends at x=0
            public const int LeftEdgeMaxIndex = 3;       // Left edge 4 LEDs

            // LED position for ambi mode (single color zones)
            public const int AmbiRightX = 6;
            public const int AmbiRightY = 5;
            public const int AmbiBottomX = 5;
            public const int AmbiBottomY = 5;
            public const int AmbiLeftX = 4;
            public const int AmbiLeftY = 5;
        }

        /// <summary>
        /// Mouse LED configuration
        /// </summary>
        public static class Mouse
        {
            // Ambi mode: sample from center-bottom of grid
            public const int AmbiModeSampleX = 6;
            public const int AmbiModeSampleY = 8;
        }

        /// <summary>
        /// Headset LED configuration
        /// </summary>
        public static class Headset
        {
            public const int GridWidth = 2;
            public const int GridHeight = 1;
            public const int LeftEarIndex = 0;
            public const int RightEarIndex = 1;
        }

        /// <summary>
        /// Chroma Link LED configuration
        /// </summary>
        public static class ChromaLink
        {
            public const int GridWidth = 4;
            public const int GridHeight = 1;
            public const int FirstLedIndex = 0;  // Special handling for LED 0
            public const int StandardLedsStartIndex = 1;
        }
    }
}
