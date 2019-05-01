using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarshControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var skymap = new SkyMap();
            skymap.Show();

            var dome = new DomeWindow();
            dome.Show();

            Application.Run(new MainForm(true));

     
           // Application.Run(new SkyMap());
        }
    }
}
