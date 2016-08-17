using HarmonyHub;
using System;
using System.Windows.Forms;

namespace HarmonyDemo
{
    static class Program
    {
        public static HarmonyClient Client { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
