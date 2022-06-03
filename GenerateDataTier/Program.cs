using System;
using System.Windows.Forms;

namespace ICEBG.GenerateDataTier
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new DataTierGeneratorForm());
        }
    }
}
