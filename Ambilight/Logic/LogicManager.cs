using System;
using System.Drawing;
using System.Threading.Tasks;
using Ambilight.DesktopDuplication;
using Ambilight.GUI;
using Colore;
using Colore.Data;
using NLog;
using Color = Colore.Data.Color;

namespace Ambilight.Logic
{
    /// <summary>
    /// This Class manages the Logic of the software. Handling the settings, Image Manipulation and logic functions
    /// </summary>
    class LogicManager
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly object _processLock = new object();

        private KeyboardLogic _keyboardLogic;
        private MousePadLogic _mousePadLogic;
        private MouseLogic _mouseLogic;
        private LinkLogic _linkLogic;
        private HeadsetLogic _headsetLogic;
        private KeypadLogic _keypadLogic;

        private readonly TraySettings settings;

        private LogicManager(TraySettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public static async Task<LogicManager> CreateAsync(TraySettings settings)
        {
            var manager = new LogicManager(settings);
            await manager.InitializeAsync();
            return manager;
        }

        private async Task InitializeAsync()
        {
            try
            {
                logger.Info("Initializing Chroma SDK...");
                //Initializing Chroma SDK
                IChroma chromaInstance = await ColoreProvider.CreateNativeAsync();
                AppInfo appInfo = new AppInfo(
                    "Ambilight for Razer devices",
                    "Shows an ambilight effect on your Razer Chroma devices",
                    "Nico Jeske",
                    "ambilight@nicojeske.de",
                    new[]
                    {
                        ApiDeviceType.Headset,
                        ApiDeviceType.Keyboard,
                        ApiDeviceType.Keypad,
                        ApiDeviceType.Mouse,
                        ApiDeviceType.Mousepad,
                        ApiDeviceType.ChromaLink
                    },
                    Category.Application);
                await chromaInstance.InitializeAsync(appInfo);

                logger.Info("Chroma SDK initialized successfully");

                _keyboardLogic = new KeyboardLogic(settings, chromaInstance);
                _mousePadLogic = new MousePadLogic(settings, chromaInstance);
                _mouseLogic = new MouseLogic(settings, chromaInstance);
                _linkLogic = new LinkLogic(settings, chromaInstance);
                _headsetLogic = new HeadsetLogic(settings, chromaInstance);
                _keypadLogic = new KeypadLogic(settings, chromaInstance);

                logger.Info("Device logic initialized");

                DesktopDuplicatorReader reader = new DesktopDuplicatorReader(this, settings);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to initialize LogicManager");
                throw;
            }
        }

        /// <summary>
        /// Processes a captured Screenshot and create an Ambilight effect for the selected devices
        /// Thread-safe method that can be called from desktop duplication reader thread
        /// </summary>
        /// <param name="img">Screenshot to process (will not be modified)</param>
        public void ProcessNewImage(Bitmap img)
        {
            // Lock to prevent race conditions on settings and device grids
            lock (_processLock)
            {
                // No need for defensive copy - all Process() methods only read the bitmap
                if (settings.KeyboardEnabled)
                    _keyboardLogic.Process(img);
                if (settings.PadEnabled)
                    _mousePadLogic.Process(img);
                if (settings.MouseEnabled)
                    _mouseLogic.Process(img);
                if (settings.LinkEnabled)
                    _linkLogic.Process(img);
                if (settings.HeadsetEnabled)
                    _headsetLogic.Process(img);
                if (settings.KeypadEnabled)
                    _keypadLogic.Process(img);
            }
        }
    }
}