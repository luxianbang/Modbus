
using System;
using System.Windows.Forms;
using EasyModbus;

namespace EasyModbusServerSimulator
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	static class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
	}
}
