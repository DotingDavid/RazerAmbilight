using System;
using AutoUpdaterDotNET;
using NLog;

namespace Ambilight
{

    /// <summary>
    /// Entry point
    /// </summary>
    internal class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Entry point. Checks for updates and initializes the software
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            logger.Info("\n\n\n --- Razer Ambilight Version 3.0.0 ----");
            AutoUpdater.Start("https://nicojeske.de/ambi/ambi.xml");

            GUI.TraySettings tray = new GUI.TraySettings();
            logger.Info("Tray Created");

            // Initialize LogicManager asynchronously with proper error handling
            try
            {
                var logicManager = Logic.LogicManager.CreateAsync(tray).GetAwaiter().GetResult();
                logger.Info("LogicManager initialized successfully");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Failed to initialize LogicManager. Application will exit.");
                System.Windows.Forms.MessageBox.Show(
                    "Failed to initialize Razer Chroma SDK. Please ensure Razer Synapse is running.\n\n" +
                    "Error: " + ex.Message,
                    "Initialization Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

    }
}