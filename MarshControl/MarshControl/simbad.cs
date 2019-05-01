using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;



namespace MarshControl {
    public partial class simbad : Form {
        public simbad() {
            InitializeComponent();
        }


        double ra;
        double dec;
        private string hh(double deg) {
            string degs = null;
            degs = Math.Floor(deg).ToString("00");
            return degs;
        }

        private string dd(double deg) {
            string degs = null;
            if (deg > 0) {
                degs = Math.Floor(deg).ToString("+00");
            } else if (deg > -1) {
                degs = Math.Ceiling(deg).ToString("-00");
            } else {
                degs = Math.Ceiling(deg).ToString("00");
            }
            return degs;
        }

        private string dm(double deg) {
            deg = Math.Abs(deg);
            return Math.Floor((deg - Math.Floor(deg)) * 60).ToString("00");

        }

        private string ds(double deg) {
            deg = Math.Abs(deg);
            double mini = ((deg - Math.Floor(deg)) * 60);
            return Math.Round((mini - Math.Floor(mini)) * 60).ToString("00.0");
        }

        public double deg2rad(double deg) {
            return (deg * Math.PI / 180.0);
        }

        public double rad2deg(double deg) {
            return (deg / Math.PI * 180.0);
        }


        public void updater() {
            ProgressBar1.Visible = true;
            Label13.Visible = true;

            ProgressBar1.Maximum = 7;
            ProgressBar1.Step = 1;
            ProgressBar1.Value = 0;

            //Button1.Text = "Searching"
            string input = TargetNameBox.Text;
            Label13.Text = "Connecting to simbad.u-strasbg.fr";
            Label13.Refresh();
            WebClient client = new WebClient();
            string url = "http://simbad.u-strasbg.fr/simbad/sim-script?script=output console=off script=off%0Aformat%20object%20%22%25FLUXLIST%28R,V,B;%20N=F,%29%20|%20%25OTYPE%28V%29%20|%20%25COO%28d;%20A%20D;FK5;J2000;%29%20|%20%25DIM%28X%20Y%29%20|%20%25IDLIST[%25*,]%20%22%0A" + HttpUtility.UrlEncode(input);
            ProgressBar1.Value = 1;

            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
            client.Headers.Add("Accept", "*/*");
            client.Headers.Add("Accept-Language", "en-gb,en;q=0.5");
            client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            ProgressBar1.Value = 2;
            //Try
            string reply = client.DownloadString(url);

            //'RichTextBox1.Text = reply
            ProgressBar1.Value = 3;
            Label13.Text = "";
            Label13.Refresh();

            if (reply.Substring(0, 7) == "::error" | reply.Substring(0, 7) == "!! A pr" | string.IsNullOrEmpty(input)) {
                FluxLbl.Text = "";
                TypeLbl.Text = "";
                RALbl.Text = "";
                SizeLbl.Text = "";
                DecLbl.Text = "";
                AltLbl.Text = "";
                AzLbl.Text = "";
                InfoBox.Text = "";
                Label13.Text = "Object not found, or other error";
                ProgressBar1.Value = 7;
                ProgressBar1.Visible = false;
            } else {
                string[] parts = reply.Split('|');

                TargetsetBtn.Enabled = true;
                FluxLbl.Text = parts[0].Trim();
                //flux
                FluxLbl.Refresh();
                TypeLbl.Text = parts[1].Trim();
                //type
                TypeLbl.Refresh();
                //'Label11.Text = parts(2) 'coords
                string[] coords = parts[2].Split(' ');
                double rightasc = Convert.ToDouble(coords[1]);
                double declin = Convert.ToDouble(coords[2]);
                ra = Convert.ToDouble(rightasc);
                dec = Convert.ToDouble(declin);


                RALbl.Text = hh(rightasc / 15) + "h " + dm(rightasc / 15) + "m " + ds(rightasc / 15) + "s";
                DecLbl.Text = dd(declin) + (Char)176 + " " + dm(declin) + (Char)39 + " " + ds(declin) + (Char)34;

                TimeSpan _TimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                double juliandec = ((_TimeSpan.TotalSeconds) / 86400) + 2440587.5;
                double T = (juliandec - 2451545.0) / 36525;
                double theta = 280.46061837 + (360.98564736629 * (juliandec - 2451545)) + (0.000387933 * T * T) - ((T * T * T) / 38710000);
                double latrad = deg2rad(51.7763);

                declin = deg2rad(declin);
                double H = deg2rad(theta - 0.0963 - rightasc);
                double alt = Math.Round(rad2deg(Math.Asin(((Math.Sin(latrad)) * (Math.Sin(declin))) + ((Math.Cos(latrad)) * (Math.Cos(declin)) * (Math.Cos(H))))), 2);
                double az = Math.Round(rad2deg(Math.Atan2(Math.Sin(H), (Math.Cos(H) * Math.Sin(latrad)) - (Math.Tan(declin) * Math.Cos(latrad)))) + 180, 2);
                declin = rad2deg(declin);
                AltLbl.Text = Convert.ToString(alt);
                AltLbl.Refresh();
                AzLbl.Text = Convert.ToString(az);
                AzLbl.Refresh();

                RALbl.Refresh();
                if (parts[3] == "     ~     ~ ") {
                    SizeLbl.Text = "star";
                    //size
                } else {
                    SizeLbl.Text = parts[3];
                    //size
                }
                //TextBox2.Text = parts(3) 'size()
                SizeLbl.Refresh();
                //RichTextBox3.Text = parts(4) 'names

                string[] names = parts[4].TrimStart().Split(',');
                int namenum = names.Count();
                string namestring = "";
                //RichTextBox3.Text = "test" + System.Environment.NewLine + CStr(namenum)
                ProgressBar1.Value = 4;
                for (int i = 0; i < namenum; i++) {
                    if (i == namenum) {
                        namestring = namestring + names[i];
                    } else {
                        namestring = namestring + names[i] + System.Environment.NewLine;
                    }
                }
                InfoBox.Text = namestring.TrimEnd();
                //Label6.Text = CStr(rightasc) + " " + CStr(declin)

                if (ShowImageChk.Checked) {
                    PictureBox1.Visible = true;
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    byte[] data = null;
                    ProgressBar1.Value = 5;
                    Label13.Text = "Downloading image                            ";
                    Label13.Refresh();
                    string url2 = "http://147.197.130.115/dss.php?w=300&ra=" + Convert.ToString(rightasc) + "&dec=" + Convert.ToString(declin) + "&x=10&y=10";
                    data = client.DownloadData(url2);

                    //RichTextBox2.Text = url2
                    client.Dispose();
                    stream.Write(data, 0, data.Length);
                    ProgressBar1.Value = 6;
                    PictureBox1.Image = Image.FromStream(stream);
                    PictureBox1.Visible = true;
                    CreditLbl.Visible = false;


                } else {
                    ProgressBar1.Value = 6;
                    PictureBox1.Image = null;
                    PictureBox1.Visible = false;
                    CreditLbl.Visible = false;
                }

                Label13.Text = "Done";
                //Button1.Text = "Submit"
                ProgressBar1.Value = 7;
                ProgressBar1.Visible = false;
                //Label13.Visible = False
            }

            //Catch ex As Exception
            //    Label1.Text = ""
            //    Label2.Text = ""
            //    Label3.Text = ""
            //    Label4.Text = ""
            // Label11.Text = ""
            // Label16.Text = ""
            // Label14.Text = ""
            //  RichTextBox3.Text = ""
            //  ProgressBar1.Value = 7
            //   ProgressBar1.Visible = False
            //   Label13.Text = "Failed to connect to Simbad or other error"
            //End Try


        }

        private void Targetset_Click(object sender, EventArgs e) {
            //MainForm MainFormInst = new MainForm(false);

            Globals.targetname = TargetNameBox.Text;

            Globals.Target.ra = deg2rad(ra);
            Globals.Target.dec = deg2rad(dec);

           // MainFormInst.dss = true;
            if (PictureBox1.Visible) {
                Globals.targettype = "DSS_" + TypeLbl.Text;
                //MainFormInst.PictureBox1.Image = PictureBox1.Image;

               // PictureBox MainPic = new MainForm.MainPictureBox;

               // simbad simbad = new simbad(MainForm.MainPictureBox);

               // MainForm.MainPictureBox.Image = PictureBox1.Image;
               // MainFormInst.PictureBox1.Visible = true;
            } else {
                Globals.targettype = TypeLbl.Text;
            }
            this.Hide();
        }

        private void Search_Click(object sender, EventArgs e) {
            updater();
        }



        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                updater();
            }
        }

    }
}
