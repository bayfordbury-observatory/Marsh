using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using ASCOM.Interface;
using ASCOM.Utilities;
using ASCOM.DriverAccess;
//using ASCOM.Helper;
using ASCOM.DeviceInterface;
using System.IO;
using System.Net;
using AstroLib;

namespace MarshControl {
    public partial class DomeWindow : Form {

        public Dome Maxdome;

        double lastDomeAz = 0;

        public DomeWindow() {



            Globals.Status = "Initialising dome window";
            Globals.LoadProgress = 40;

            InitializeComponent();

            Screen[] sc;
            sc = Screen.AllScreens;

            if(!Globals.testing && sc.Length > 2) {

                this.WindowState = FormWindowState.Normal;
                this.Left = sc[2].Bounds.Left;
                this.Top = sc[2].Bounds.Top;
                this.StartPosition = FormStartPosition.Manual;
                this.WindowState = FormWindowState.Maximized;
            }

            Globals.Status = "Downloading AllSky image";
            Globals.LoadProgress = 50;

            System.Net.WebClient client = new System.Net.WebClient();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            byte[] data = null;

            try {
                if(Globals.SunHor.alt < 0) {
                    data = client.DownloadData("http://star.herts.ac.uk/allsky/camera1/AllSkyCurrentImage.JPG");
                    
                } else {
                    data = client.DownloadData("http://star.herts.ac.uk/allsky/camera7/AllSkyCurrentImage.JPG");
                    
                }
                Globals.Status = "Writing stream";
                Globals.LoadProgress = 60;

                stream.Write(data, 0, data.Length);
                pictureBox1.Image = Image.FromStream(stream);
            } catch {
                pictureBox1.Image = null;
            }

            Globals.Status = "Connecting to dome";
            Globals.LoadProgress = 60;

            if(!Globals.testing) {
                connect();
            }


        }

        private void SetupDome_Click(object sender, EventArgs e) {

            try {

                ASCOM.Utilities.Chooser Ch = default(ASCOM.Utilities.Chooser);
                string SelectedDevice = null;

                Ch = new ASCOM.Utilities.Chooser();
                Ch.DeviceType = "Dome";

                SelectedDevice = Ch.Choose();
                Ch.Dispose();
                Ch = null;

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            connect();
        }

        private void connect() {

            try {

                if(Maxdome != null) {
                    if(Maxdome.Connected) {
                        ConnectBtn.Text = "Connect";
                        Maxdome.Connected = false;

                        //pollDome.Enabled = false;
                        //pollTargetAz.Enabled = false;
                        //Maxdome.Dispose();
                        //Maxdome = null;
                        slaveChk.Checked = false;
                        CloseBtn.Enabled = false;
                        OpenBtn.Enabled = false;
                        AbortBtn.Enabled = false;
                        HomeBtn.Enabled = false;
                        GotoBtn.Enabled = false;

                    } else {
                        Maxdome = new Dome("MaxDome.Dome");
                        Maxdome.Connected = true;
                        if(Maxdome.Connected) {
                            pollDome.Enabled = true;
                            pollTargetAz.Enabled = true;
                            ConnectBtn.Text = "Disconnect";

                            CloseBtn.Enabled = true;
                            OpenBtn.Enabled = true;
                            AbortBtn.Enabled = true;
                            HomeBtn.Enabled = true;
                            GotoBtn.Enabled = true;


                        }
                    }
                } else {
                    Maxdome = new Dome("MaxDome.Dome");
                    Maxdome.Connected = true;
                    if(Maxdome.Connected) {
                        pollDome.Enabled = true;
                        pollTargetAz.Enabled = true;
                        ConnectBtn.Text = "Disconnect";
                        CloseBtn.Enabled = true;
                        OpenBtn.Enabled = true;
                        AbortBtn.Enabled = true;
                        HomeBtn.Enabled = true;
                        GotoBtn.Enabled = true;
                    }
                }

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }




        private void OpenBtn_Click(object sender, EventArgs e) {
            if(Maxdome.Connected) {
                Maxdome.OpenShutter();
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e) {
            slaveChk.Checked = false;
            if(Maxdome.Connected) {
                Maxdome.CloseShutter();
            }
        }

        private void AbortBtn_Click(object sender, EventArgs e) {
            slaveChk.Checked = false;
            if(Maxdome.Connected) {
                Maxdome.AbortSlew();
            }
        }

        private void GotoBtn_Click(object sender, EventArgs e) {
            //GotoTxt.Text
            slaveChk.Checked = false;
            double az = double.Parse(GotoTxt.Text);
            if(az >= 0 && az <= 360) {
                Maxdome.SlewToAzimuth(az);
            }
        }

        private void HomeBtn_Click(object sender, EventArgs e) {
            //Maxdome.CloseShutter();
            //Maxdome.FindHome();     
            slaveChk.Checked = false;
            Maxdome.SlewToAzimuth(225);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            //Refresh dome stats      5Hz

            ConnectLbl.Text = "Connected:" + Maxdome.Connected.ToString();
            if(Maxdome.Connected) {
                HomeLbl.Text = "At Home:" + Maxdome.AtHome.ToString();
                SlewingLbl.Text = "Slewing: " + Maxdome.Slewing.ToString();
                AzimuthLbl.Text = "Azimuth: " + Math.Round(Maxdome.Azimuth, 1).ToString();
                ShutterLbl.Text = "Shutter: " + Maxdome.ShutterStatus.ToString();
            }
        }


        private void timer2_Tick(object sender, EventArgs e) {
            //Update weather 5s

            try {

                WebClient client = new WebClient();
                string downloadString = client.DownloadString("http://observatory-weather.herts.ac.uk/live.php");

                string[] Weather = downloadString.Split('|');

                string[] Weather1 = Weather[0].Split(' ');
                string[] Weather2 = Weather[1].Split(' ');
                string[] Weather3 = Weather[2].Split(' ');


                double sqm = Convert.ToDouble(Weather3[7]);

                if(sqm == 0) {
                    txtSkyBrightness.Text = "Too bright";
                } else {
                    txtSkyBrightness.Text = Convert.ToString(Math.Round(sqm, 2)) + " mag/asec" + (char)178;
                }

                double skytemp = Convert.ToDouble(Weather1[1]);
                txtTemp.Text = Weather2[1] + (char)176 + "C";
                txtDewPt.Text = Weather2[7] + (char)176 + "C";


                txtPressure.Text = Weather2[5] + " hPa";
                if(skytemp > -10) {
                    txtSkyTemp.Font = new Font(txtSkyTemp.Font, FontStyle.Regular);
                    txtSkyTemp.ForeColor = Color.FromKnownColor(KnownColor.AppWorkspace);
                    txtSkyTemp.Text = Weather1[1] + (char)176 + "C (Thick cloud)";
                } else if(skytemp > -20) {
                    txtSkyTemp.Font = new Font(txtSkyTemp.Font, FontStyle.Regular);
                    txtSkyTemp.ForeColor = Color.FromKnownColor(KnownColor.AppWorkspace);
                    txtSkyTemp.Text = Weather1[1] + (char)176 + "C (Cloud)";
                } else if(skytemp > -100) {
                    txtSkyTemp.Font = new Font(txtSkyTemp.Font, FontStyle.Regular);
                    txtSkyTemp.ForeColor = Color.FromKnownColor(KnownColor.AppWorkspace);
                    txtSkyTemp.Text = Weather1[1] + (char)176 + "C (Clear)";
                } else {
                    txtSkyTemp.ForeColor = Color.Red;
                    txtSkyTemp.Font = new Font(txtSkyTemp.Font, FontStyle.Bold);
                    txtSkyTemp.Text = "Unreadable";
                }
                txtHumidity.Text = Weather2[2] + " " + (char)37;
                Int16 rain = Convert.ToInt16(Weather1[2]);
                if(rain == 0) {
                    txtRain.ForeColor = Color.FromKnownColor(KnownColor.AppWorkspace);
                    txtRain.Font = new Font(txtRain.Font, FontStyle.Regular);
                    txtRain.Text = "None";
                } else if(rain == 1) {
                    txtRain.ForeColor = Color.Red;
                    txtRain.Font = new Font(txtRain.Font, FontStyle.Regular);
                    txtRain.Text = "In last minute";
                } else if(rain == 2) {
                    txtRain.Text = "Currently raining";
                    txtRain.Font = new Font(txtRain.Font, FontStyle.Bold);
                    txtRain.ForeColor = Color.Red;
                }
                double wind = Convert.ToDouble(Weather2[3]);
                if(wind > 40) {
                    txtWindSpeed.Font = new Font(txtWindSpeed.Font, FontStyle.Bold);
                    txtWindSpeed.ForeColor = Color.Red;
                } else {
                    txtWindSpeed.Font = new Font(txtWindSpeed.Font, FontStyle.Regular);
                    txtWindSpeed.ForeColor = Color.FromKnownColor(KnownColor.AppWorkspace);

                }
                /*
           if (sqm2 > 19.5 & skytemp < -30) {
               txtSkyQual.Text = "Excellent";
           } else if (sqm2 > 18.5 & skytemp < -25) {
               txtSkyQual.Text = "Good";
           } else if (sqm2 > 0) {
               txtSkyQual.Text = "Fair";
           } else {
               txtSkyQual.Text = "Too bright";
           }
               */

                if(rain > 0) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (rain)";
                } else if(Convert.ToDouble(Weather1[4]) > 1 | skytemp > -10) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (cloud)";
                } else if(wind > 45) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (wind)";
                } else if(Convert.ToDouble(Weather2[1]) < -15) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (cold)";
                } else if(Convert.ToDouble(Weather2[1]) > 40) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (hot)";
                } else if(Convert.ToDouble(Weather2[2]) > 98) {
                    txtSafe.ForeColor = Color.Red;
                    txtSafe.Text = "UNSAFE (humid)";
                } else {
                    txtSafe.ForeColor = Color.GreenYellow;
                    txtSafe.Text = "SAFE";
                }

                txtWindSpeed.Text = Convert.ToString(Math.Round(wind, 1)) + " kph ";
                double windir = Convert.ToDouble(Weather2[4]);
                txtWindDir.Text = Math.Round(windir).ToString() + (char)176;

            } catch {

            }
        }

        private void timer3_Tick(object sender, EventArgs e) {
            //Update allsky pic every 20s
            try {
                System.Net.WebClient client = new System.Net.WebClient();
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                byte[] data = null;

                if(Globals.SunHor.alt < 0) {
                    data = client.DownloadData("http://star.herts.ac.uk/allsky/camera1/AllSkyCurrentImage.JPG");
                    
                } else {
                    data = client.DownloadData("http://star.herts.ac.uk/allsky/camera7/AllSkyCurrentImage.JPG");
                    
                }
                client.Dispose();
                stream.Write(data, 0, data.Length);
                pictureBox1.Image = Image.FromStream(stream);
            } catch {
                pictureBox1.Image = null;
            }
        }

        private void timer4_Tick(object sender, EventArgs e) {
            //poll sun

            if(Globals.SunHor.alt > 0) {
                if(!Globals.SunUp) {
                    //Sun has just come up
                    slaveChk.Checked = false;

                    if(Maxdome.Azimuth < 200 || Maxdome.Azimuth > 250) {
                        Maxdome.SlewToAzimuth(225);
                    }

                    Maxdome.CloseShutter();
                }
                Globals.SunUp = true;
            } else {
                Globals.SunUp = false;
            }

        }

        double calcDomeAz() {


            double r = 0.65; //negative on west
            //Axis intersection offsets
            double X = 0;
            double Y = 0;
            double Z = -0.2;
            //Dome radius
            double R = 2.5;

            //Coords of optical axis and dec intersection
            double xa = X + r * Math.Cos(Globals.HA);
            double ya = Y - r * Math.Sin(Globals.loc.lat) * Math.Sin(Globals.HA);
            double za = Z + r * Math.Cos(Globals.loc.lat) * Math.Sin(Globals.HA);

            //point 1m towards end of telescope from xa,ya,za
            double xb = xa + Math.Sin(Globals.ScopeHor.az) * Math.Cos(Globals.ScopeHor.alt);
            double yb = ya + Math.Cos(Globals.ScopeHor.az) * Math.Cos(Globals.ScopeHor.alt);
            double zb = za + Math.Sin(Globals.ScopeHor.alt);

            double a = (xb - xa) * (xb - xa) + (yb - ya) * (yb - ya) + (zb - za) * (zb - za);
            double b = 2 * ((xb - xa) * (xa - X) + (yb - ya) * (ya - Y) + (zb - za) * (za - Z));
            double c = (xa - X) * (xa - X) + (ya - Y) * (ya - Y) + (za - Z) * (za - Z) - R * R;

            double delta = b * b - 4 * a * c;

            double d1 = (-b - Math.Sqrt(delta)) / (2 * a);
            double z1 = za + d1 * (zb - za);

            double domeAz;

            if(z1 > 0) {
                double x1 = xa + d1 * (xb - xa);
                double y1 = ya + d1 * (yb - ya);
                domeAz = Math.Atan2(x1, y1).ToDeg();
            } else {
                double d2 = (-b + Math.Sqrt(delta)) / (2 * a);

                double x1 = xa + d2 * (xb - xa);
                double y1 = ya + d2 * (yb - ya);
                domeAz = Math.Atan2(x1, y1).ToDeg();
            }

            domeAz = Utils.quad(domeAz);

            return domeAz;

        }

        private void timer5_Tick(object sender, EventArgs e) {
            //dome sync

            try {

                double domeAz = calcDomeAz();

                lastDomeAz = domeAz;
                double error = Math.Abs(domeAz - Maxdome.Azimuth);
                TargetLbl.Text = "Target az: " + domeAz.ToString("F2") + " (" + error.ToString("F3") + ")";

                if(slaveChk.Checked && error > 2 && !Maxdome.Slewing && Math.Abs(lastDomeAz - domeAz) < 10) {
                    Maxdome.SlewToAzimuth(domeAz);

                }

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
