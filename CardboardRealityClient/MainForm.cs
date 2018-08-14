using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardboardRealityClient {

	public partial class MainForm : Form {

		public MainForm() {
			InitializeComponent();
		}

		/// <summary>
		/// Adds the specified URI to the text area on the form.
		/// </summary>
		/// <param name="uri"></param>
		public void AddUri(Uri uri) {
			if (InvokeRequired) {
				BeginInvoke(new Action<Uri>(AddUri), uri);
			}
			else {
				txtOutput.Text += uri.ToString() + Environment.NewLine;
			}
		}
	}
}
