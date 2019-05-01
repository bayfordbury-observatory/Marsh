using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MarshControl {
    public partial class LoadScreen : Form {
        public LoadScreen() {
            InitializeComponent();
        }

        int time = 0;

        private void timer1_Tick(object sender, EventArgs e) {
            progressBar1.Value = Globals.LoadProgress;
            label1.Text = Globals.Status;

            time++;

            if(Globals.LoadProgress == 100 || time>300) {
                // this.Close();
                Thread.Sleep(500);
                Thread.CurrentThread.Abort();
            }
        }
    }
}
