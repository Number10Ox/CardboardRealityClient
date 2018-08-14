using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace CardboardRealityClient {

	static class Program {

		/// <summary>
		/// Gets the main form in the application.
		/// </summary>
		internal static MainForm MainForm { get; private set; }

        /// <summary>
        /// Gets network client 
        /// </summary>
        internal static CardboardRealityNetClient NetClient { get; private set; }

        /// <summary>
        /// Gets network client 
        /// </summary>
        static System.Windows.Forms.Timer NetClientUpdateTimer = new System.Windows.Forms.Timer();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args) {
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			Uri uri = null;
			if (args.Length > 0) {
				// a URI was passed and needs to be handled
				try {
					uri = new Uri(args[0].Trim());
				}
				catch (UriFormatException) {
					Console.WriteLine("Invalid URI.");
				}
			}

			IUriHandler handler = UriHandler.GetHandler();
			if (handler != null) {
				// the singular instance of the application is already running
				if (uri != null) handler.HandleUri(uri);

				// the process will now exit without displaying the main form
				//...
			}
			else {
				// this must become the singular instance of the application
				UriHandler.Register();

				MainForm = new MainForm();
                NetClient = new CardboardRealityNetClient();
                NetClient.Initialize();

				if (uri != null) {
					// a URI was passed, handle it locally
					MainForm.Shown += (o, e) => new UriHandler().HandleUri(uri);
				}

                NetClientUpdateTimer.Tick += new EventHandler(TimerEventProcessor);

                // Sets the timer interval to 5 seconds.
                NetClientUpdateTimer.Interval = 50;
                NetClientUpdateTimer.Start();

                // load and display the main form
                System.Windows.Forms.Application.Run(MainForm);
			}
		}

        private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            NetClient.Update();
        }
    }
}