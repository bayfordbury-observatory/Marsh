using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarshControl {
    public partial class Settings : Form {
        public Settings() {
            InitializeComponent();
        }


        private void Cancel_Click(object sender, EventArgs e) {
            this.Close();
        }
     

       private void Defaults_Click(object sender, EventArgs e) {

           Properties.Settings.Default.Reload();

           TextBoxcom.Text = Properties.Settings.Default.com.ToString();
           TextBoxdd.Text = Properties.Settings.Default.dd.ToString();
           TextBoxdt.Text = Properties.Settings.Default.dt.ToString();
           TextBoxi.Text = Properties.Settings.Default.i.ToString();
           TextBoxc.Text = Properties.Settings.Default.c.ToString();
           TextBoxe.Text = Properties.Settings.Default.e.ToString();
           TextBoxgamma.Text = Properties.Settings.Default.gamma.ToString();
           TextBoxtheta.Text = Properties.Settings.Default.theta.ToString();
           TextBoxl.Text = Properties.Settings.Default.l.ToString();
           TextBoxr.Text = Properties.Settings.Default.r.ToString();
        }

       private void Save_Click(object sender, EventArgs e){
            Properties.Settings.Default.com = Convert.ToInt32(TextBoxcom.Text);
            Properties.Settings.Default.dd = Convert.ToDouble(TextBoxdd.Text);
            Properties.Settings.Default.dt = Convert.ToDouble(TextBoxdt.Text);
            Properties.Settings.Default.i = Convert.ToDouble(TextBoxi.Text);
            Properties.Settings.Default.c = Convert.ToDouble(TextBoxc.Text);
            Properties.Settings.Default.e = Convert.ToDouble(TextBoxe.Text);
            Properties.Settings.Default.gamma = Convert.ToDouble(TextBoxgamma.Text);
            Properties.Settings.Default.theta = Convert.ToDouble(TextBoxtheta.Text);
            Properties.Settings.Default.l = Convert.ToDouble(TextBoxl.Text);
            Properties.Settings.Default.r = Convert.ToDouble(TextBoxr.Text);
            Properties.Settings.Default.Save();
            Globals.reloadSettings = true;
            this.Close();            
        }

      
    }
}
