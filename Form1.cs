using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace betterThanYou {
	public partial class Form1 : Form {
		Dictionary<int, Process> processDict = new Dictionary<int, Process>();
		List<(int, byte, string, DateTime, bool)> log = new List<(int, byte, string, DateTime, bool)>();

		public void UpdateProcessList(Object myObject, EventArgs myEventArgs) {
			Process[] processArray = Process.GetProcesses();
			Dictionary<int, Process> newProcessDict = new Dictionary<int, Process>();

			for (int i = 0; i < processArray.Length; i++)
				newProcessDict[processArray[i].Id] = processArray[i];

			foreach (KeyValuePair<int, Process> entry in newProcessDict) {
				if (!processDict.ContainsKey(entry.Key)) {
					processDict[entry.Key] = entry.Value;
					log.Add((entry.Key, 1, entry.Value.ProcessName, DateTime.Now, true));
				}
			}

			List<int> toBeRemoved = new List<int>();
			foreach (KeyValuePair<int, Process> entry in processDict) {
				if (!newProcessDict.ContainsKey(entry.Key)) {
					toBeRemoved.Add(entry.Key);
					log.Add((entry.Value.Id, 0, entry.Value.ProcessName, DateTime.Now, true));
				}
			}

			foreach (int i in toBeRemoved)
				processDict.Remove(i);

			newProcessDict = null;
			toBeRemoved = null;
			this.UpdateTextBox();
		}

		public void AppendText(RichTextBox box, string text, Color color) {
			box.SelectionStart = box.TextLength;
			box.SelectionLength = 0;

			box.SelectionColor = color;
			box.AppendText(text);
			box.SelectionColor = box.ForeColor;
		}

		public string getPaddedString((int, byte, string, DateTime, bool) item) {
			StringBuilder builder = new StringBuilder();
			builder.Append(item.Item1);
			while (builder.Length < 12)
				builder.Append(" ");

			builder.Append(item.Item3);
			while (builder.Length < 12 + 64)
				builder.Append(" ");

			builder.Append(item.Item4.ToString());
			while (builder.Length < 12 + 64 + 20)
				builder.Append(" ");

			builder.Append("\n");
			return builder.ToString();
		}

		public void UpdateTextBox() {
			//for (int i = log.Count - 1; i >= 0; i --) {
			for (int i = 0; i < log.Count; i++) {
				if (log[i].Item5) {
					log[i] = (log[i].Item1, log[i].Item2, log[i].Item3, log[i].Item4, false);
					if (log[i].Item2 == 1) {
						//this.richTextBox1.AppendText(getPaddedString(log[i]));
						AppendText(this.richTextBox1, getPaddedString(log[i]), Color.LightBlue);
					} else {
						AppendText(this.richTextBox1, getPaddedString(log[i]), Color.IndianRed);
					}
				}
			}
		}

		public Form1() {
			InitializeComponent();
			Timer timer = new Timer();
			timer.Interval = 2000;
			timer.Tick += new EventHandler(UpdateProcessList);
			timer.Enabled = true;
		}
	}
}
