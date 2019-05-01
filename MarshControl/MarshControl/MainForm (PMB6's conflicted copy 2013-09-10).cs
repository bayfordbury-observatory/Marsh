//using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Configuration;

namespace MarshControl {

    public static class Globals {
        public static string targetname = "None selected";
        public static string targettype = "-";
        public static double targetRA = 0;
        public static double targetDec = 0;


    }

    public partial class MainForm : Form {
        //David Campbell - 2013
        //University of Hertfordshire


        //Useful functions below
        private string dd(double deg) {
            string degs = null;
            if (deg >= 0) {
                degs = Math.Floor(deg).ToString("+00");
            } else if (deg > -1) {
                degs = Math.Ceiling(deg).ToString("-00");
            } else {
                degs = Math.Ceiling(deg).ToString("00");
            }
            return degs;


        }

        private string dd1(double deg) {
            string degs = null;
            if (deg >= 0) {
                degs = Math.Floor(deg).ToString("00");
            } else if (deg > -1) {
                degs = Math.Ceiling(deg).ToString("-00");
            } else {
                degs = Math.Ceiling(deg).ToString("00");
            }
            return degs;
        }

        private string hh(double deg) {
            string degs = null;
            degs = Math.Floor(deg).ToString("00");
            return degs;
        }

        private string dm(double deg) {
            deg = Math.Abs(deg);
            return Math.Floor((deg - Math.Floor(deg)) * 60).ToString("00");
        }

        private string ds(double deg) {
            deg = Math.Abs(deg);
            double mini = ((deg - Math.Floor(deg)) * 60);
            return Math.Round((mini - Math.Floor(mini)) * 60).ToString("00");
        }

        public double quad(double inn) {
            //return a value in the range 0-360
            if (inn < 0) {
                return inn + ((0 - (Math.Floor(inn / 360))) * 360);
            } else {
                if (inn > 360) {
                    return inn - ((Math.Floor(inn / 360)) * 360);
                } else {
                    return inn;
                }
            }
        }

        public double deg2rad(double deg) {
            return (deg * Math.PI / 180.0);
        }

        public double rad2deg(double deg) {
            return (deg / Math.PI * 180.0);
        }

        public double Sec(double angle) {
            // Calculate the secant of angle, in radians.
            return 1.0 / Math.Cos(angle);
        }

        public double precessdec(double ra, double dec, double t) {

            double ra0 = deg2rad(ra);
            double dec0 = deg2rad(dec);

            double e = deg2rad((2306.2181 * t + 0.30188 * t * t + 0.017998 * t * t * t) / 3600);
            double z = deg2rad((2306.2181 * t + 1.09468 * t * t + 0.018203 * t * t * t) / 3600);
            double th = deg2rad((2004.3109 * t - 0.42665 * t * t - 0.041833 * t * t * t) / 3600);

            double A = Math.Cos(dec0) * Math.Sin(ra0 + e);
            double B = Math.Cos(th) * Math.Cos(dec0) * Math.Cos(ra0 + e) - Math.Sin(th) * Math.Sin(dec0);
            double C = Math.Sin(th) * Math.Cos(dec0) * Math.Cos(ra0 + e) + Math.Cos(th) * Math.Sin(dec0);

            if (dec > 85) {
                return rad2deg(Math.Acos(Math.Sqrt(A * A + B * B)));
            } else {
                return rad2deg(Math.Asin(C));
            }

        }

        public double precessra(double ra, double dec, double t) {

            double ra0 = deg2rad(ra);
            double dec0 = deg2rad(dec);

            double e = deg2rad((2306.2181 * t + 0.30188 * t * t + 0.017998 * t * t * t) / 3600);
            double z = deg2rad((2306.2181 * t + 1.09468 * t * t + 0.018203 * t * t * t) / 3600);
            double th = deg2rad((2004.3109 * t - 0.42665 * t * t - 0.041833 * t * t * t) / 3600);

            double A = Math.Cos(dec0) * Math.Sin(ra0 + e);
            double B = Math.Cos(th) * Math.Cos(dec0) * Math.Cos(ra0 + e) - Math.Sin(th) * Math.Sin(dec0);
            double C = Math.Sin(th) * Math.Cos(dec0) * Math.Cos(ra0 + e) + Math.Cos(th) * Math.Sin(dec0);

            return quad(rad2deg(Math.Atan2(A, B) + z));

        }

        private SerialPort mySerialPort = new SerialPort();
        double HA = 0;

        double RA = 0;
        double DEC = 0;
        double domeazimuth = 0;
        int inbyte;
        int inbyte2;
        double latrad = 0.90366691;
        int timeout = 0;
        //public string Globals.targetname = "None selected";
        //public string Globals.targettype = "-";
        //public double Globals.targetRA;
        // public double Globals.targetDec;
        double obslat = 51.7763;
        //double targetaz;
        //double targetalt;
        double T;
        double tt;
        public double le;
        public double re;
        public double be;
        public double lp;
        public double rp;
        public double bp;
        public int count = 0;
        public int haenc;
        public int decenc;

        public bool dss = false;
        double cd;
        double ct;
        double ci;
        double cc;
        double ce;
        double cgamma;
        double ctheta;
        double cl;

        double cr;

        public void load_settings() {
            cd = Properties.Settings.Default.dd;
            ct = Properties.Settings.Default.dt;
            ci = Properties.Settings.Default.i;
            cc = Properties.Settings.Default.c;
            ce = Properties.Settings.Default.e;
            cgamma = Properties.Settings.Default.gamma;
            ctheta = Properties.Settings.Default.theta;
            cl = Properties.Settings.Default.l;
            cr = Properties.Settings.Default.r;
        }

        public void CommPortSetup() {
            if (!mySerialPort.IsOpen) {


                var _with1 = mySerialPort;
                _with1.PortName = "COM" + Convert.ToString(Properties.Settings.Default.com);
                _with1.BaudRate = 19200;
                _with1.DataBits = 8;
                _with1.Parity = Parity.None;
                _with1.StopBits = StopBits.One;
                _with1.Handshake = Handshake.None;
                _with1.WriteTimeout = 100;
                _with1.ReadTimeout = 100;

                try {
                    mySerialPort.Open();
                    Timer1.Enabled = true;
                    Label8.Text = "";
                } catch (Exception ex) {
                    //Serial port doesn't exist or can't be opened
                    MessageBox.Show(ex.Message);
                    Timer1.Enabled = false;
                }
            }
        }

        //void Main(string[] args) {
        //################################################################## Main
        public MainForm(bool start) {
            InitializeComponent();

            if (start) {
                CommPortSetup();
                load_settings();
            }
        }

        //#################################################################

        public int[,] maparray = new int[721, 361];

        public void loadmap() {
            Bitmap map = new Bitmap("images/moon.bmp");

            for (int i = 0; i <= 719; i++) {
                for (int j = 0; j <= 359; j++) {
                    maparray[i, j] = map.GetPixel(i, j).R;


                }
            }
        }

        double salt;
        double saz;
        double malt;
        double maz;
        public double moonl;
        public double moonb;
        public double bhr;
        public double lhr;

        public double posangle;


        private void moonmap() {
            int count = 0;
            //Label32.Text = CStr(lhr) + " " + CStr(bhr) + " " + CStr(L) + " " + CStr(b) + " " + CStr(posangle)

            int moonwidth = 300;
            Bitmap image = new Bitmap(moonwidth, moonwidth);


            //Dim colour As Color
            double liblatd = moonb;
            double liblat = -deg2rad(liblatd);
            double liblongd = moonl;
            double liblong = deg2rad(liblongd);
            double sinx = Math.Sin(deg2rad(posangle));
            double cosx = Math.Cos(deg2rad(posangle));

            double rho = 0;
            double x2 = 0;
            double y2 = 0;
            double x = 0;
            double y = 0;
            double theta = 0;
            double phi = 0;
            double theta3 = 0;
            double delta = 0;
            double tone = 0;
            double c = 0;
            lhr = deg2rad(lhr);
            bhr = deg2rad(bhr);

            // Create a Bitmap object from a file. 
            if (count == 0) {
                loadmap();


            }







            for (int i = 0; i < moonwidth; i++) {

                for (int j = 0; j < moonwidth; j++) {

                    x2 = ((double)i + 1 - ((double)moonwidth / 2)) / ((double)moonwidth / 2);
                    y2 = ((double)j + 1 - ((double)moonwidth / 2)) / ((double)moonwidth / 2);

                    x = (x2 * cosx) - (y2 * sinx);
                    y = (x2 * sinx) + (y2 * cosx);

                    rho = Math.Sqrt((x * x) + (y * y));
                    //debug.AppendText(i.ToString() + " " + j.ToString() + " "+x2.ToString() + " " + y2.ToString() + " " + x.ToString() + " " + y.ToString() + " " + rho.ToString() + Environment.NewLine);

                    if (rho < 1) {
                        if (rho == 0) {
                            theta = rad2deg(liblong);
                            phi = rad2deg(liblat);

                        } else {
                            c = Math.Asin(rho);
                            theta = rad2deg(liblong + Math.Atan2(x * rho, (rho * Math.Cos(liblat) * Math.Cos(c)) - (y * Math.Sin(liblat) * rho)));
                            phi = rad2deg(Math.Asin(Math.Cos(c) * Math.Sin(liblat) + ((y * rho * Math.Cos(liblat)) / rho)));
                        }


                        if ((theta >= 180)) {
                            theta = theta - 360;
                        } else if ((theta < -180)) {
                            theta = theta + 360;
                        }


                        theta3 = Math.Round(2 * theta) / 2;

                        if ((phi < -90)) {
                            phi = phi + 360;
                        } else if ((phi > 90)) {
                            phi = phi - 360;
                        }


                        delta = rad2deg(Math.Acos(Math.Sin(deg2rad(phi)) * Math.Sin(bhr) + Math.Cos(deg2rad(phi)) * Math.Cos(bhr) * Math.Cos(lhr - deg2rad(theta))));


                        if (double.IsNaN(c) | double.IsInfinity(c)) {

                        } else {

                            tone = Convert.ToDouble(maparray[Convert.ToInt32(Math.Floor((theta + 180) * 2)), Convert.ToInt32(Math.Floor((phi + 90) * 2))]);
                            //debug.AppendText(theta.ToString() + " " + phi.ToString() + Environment.NewLine);
                            if ((delta > 93)) {
                                tone = 0;
                            } else if ((delta > 81)) {
                                tone = Math.Round(tone * (1 - ((delta - 81) / 12)));
                            }

                            image.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(tone), Convert.ToInt32(tone), Convert.ToInt32(tone)));


                        }
                    }
                }
            }
            PictureBox1.Enabled = true;
            PictureBox1.Image = image;

            //Label1.Text = "Started"

        }

        // End of functions, main repeating sub

        void Timer1_Tick_1(object sender, EventArgs e) {
            //Timer subroutine, 5-10 Hz



            //First thing, Work out the time
            DateTime time = DateTime.Now;
            string dateformat = "dd MMMM yyyy";
            string hoursformat = "HH";
            string minsformat = "mm";
            string secsformat = "ss";
            timelbl.Text = "UTC: " + time.ToString(hoursformat) + "h " + time.ToString(minsformat) + "m " + time.ToString(secsformat) + "s";
            datelbl.Text = "Date: " + time.ToString(dateformat);
            TimeSpan _TimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            string julian = (((_TimeSpan.TotalSeconds) / 86400) + 2440587.5).ToString("F4");
            double juliandec = ((_TimeSpan.TotalSeconds) / 86400) + 2440587.5;

            // juliandec = 2448724.5

            T = (juliandec - 2451545.0) / 36525;

            //Sidereal time at Greenwich
            double theta = 280.46061837 + (360.98564736629 * (juliandec - 2451545)) + (0.000387933 * T * T) - ((T * T * T) / 38710000);
            //Local sidereal time
            dynamic lst = quad(theta - 0.0963);

            //Check serial port exists

            if (mySerialPort.IsOpen) {
                //returnValue = "";

                //Read the encoder positions and convert to HA/DEC
                if (timeout < (5000 / Timer1.Interval)) {
                    try {
                        mySerialPort.Write("#H");
                        //Request HA encoder values
                        //mySerialPort.Write("H") 'Request HA encoder values
                        inbyte = mySerialPort.ReadByte();
                        inbyte2 = mySerialPort.ReadByte();
                        //RichTextBox1.AppendText("#H: " + CStr(Chr(inbyte)) + CStr(Chr(inbyte2)) + " ")
                        //check for correct return characters
                        if (((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "H") {
                            inbyte = mySerialPort.ReadByte();
                            //MSB
                            inbyte2 = mySerialPort.ReadByte();
                            //LSB
                            haenc = (((inbyte) * 256) + (inbyte2));
                            //haenclbl.Text = "HA encoder: " + 
                            //decenclbl.Text = CStr(inbyte) + " " + CStr(inbyte2)
                            HA = ((Convert.ToDouble(inbyte) * 256) + Convert.ToDouble(inbyte2)) * (360 / 16384);
                            //RichTextBox1.AppendText("-" + CStr(inbyte) + " " + CStr(inbyte2) + " " + CStr(HA) + Environment.NewLine)
                        }
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.Write("#D");
                        //Request DEC encoder values
                        inbyte = mySerialPort.ReadByte();
                        inbyte2 = mySerialPort.ReadByte();
                        // RichTextBox1.AppendText("#D: " + CStr(Chr(inbyte)) + CStr(Chr(inbyte2)) + " ")
                        //check for correct return characters
                        if (((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "D") {

                            inbyte = mySerialPort.ReadByte();
                            //MSB
                            inbyte2 = mySerialPort.ReadByte();
                            //LSB

                            decenc = (((inbyte) * 256) + (inbyte2));
                            DEC = ((Convert.ToDouble(inbyte) * 256) + Convert.ToDouble(inbyte2)) * (360 / 16384);
                            if (DEC > 180) {
                                DEC = DEC - 360;
                            }
                            HA = quad(HA);
                            // RichTextBox1.AppendText("-" + CStr(inbyte) + " " + CStr(inbyte2) + " " + CStr(DEC) + Environment.NewLine)
                            //RichTextBox1.AppendText(CStr(DEC) + Environment.NewLine)
                            //RichTextBox1.AppendText(Convert.ToString(inbyte, 2) + Environment.NewLine)

                            //Analytical pointing model corrections
                            HA = ct - cgamma * Math.Sin(deg2rad(HA - ctheta)) * Math.Tan(deg2rad(DEC)) + cc * Sec(deg2rad(DEC)) - ci * Math.Tan(deg2rad(DEC)) + ce * Math.Cos(deg2rad(obslat)) * Sec(deg2rad(DEC)) * Math.Sin(deg2rad(HA)) + cl * (Math.Sin(deg2rad(obslat)) * Math.Tan(deg2rad(DEC)) + Math.Cos(deg2rad(DEC)) * Math.Cos(deg2rad(HA))) + cr * HA + HA;
                            DEC = cd - cgamma * Math.Cos(deg2rad(HA - ctheta)) - ce * (Math.Sin(deg2rad(obslat)) * Math.Cos(deg2rad(DEC)) - Math.Cos(deg2rad(obslat)) * Math.Sin(deg2rad(DEC)) * Math.Cos(deg2rad(HA))) + DEC;
                            enclbl.Text = "Encoders: " + haenc.ToString("00000.##") + " " + decenc.ToString("00000.##");
                        }

                        mySerialPort.DiscardInBuffer();
                        mySerialPort.Write("#A");
                        //Request dome azimuth values
                        inbyte = mySerialPort.ReadByte();
                        inbyte2 = mySerialPort.ReadByte();
                        //RichTextBox1.AppendText("#D: " + CStr(Chr(inbyte)) + CStr(Chr(inbyte2)) + " ")
                        //check for correct return characters
                        if (((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "A") {

                            inbyte = mySerialPort.ReadByte();
                            //MSB

                            inbyte2 = mySerialPort.ReadByte();
                            //LSB

                            domeazimuth = (((Convert.ToDouble(inbyte) * 256) + Convert.ToDouble(inbyte2)) - 44) * 1.139241;
                            if (domeazimuth >= 360) {
                                domeazlbl.Text = "Dome azimuth: Uncalibrated (>360)";
                            } else if (domeazimuth < 0) {
                                domeazlbl.Text = "Dome azimuth: Uncalibrated (<0)";
                            } else {
                                domeazlbl.Text = "Dome azimuth: " + domeazimuth.ToString() + (Char)176;
                            }

                            //RichTextBox1.AppendText(CStr(inbyte) + " " + CStr(inbyte2) + " " + CStr(domeazimuth) + Environment.NewLine)
                            //RichTextBox1.AppendText(CStr(DEC) + Environment.NewLine)
                            //RichTextBox1.AppendText(Convert.ToString(inbyte, 2) + Environment.NewLine)


                        }

                    } catch (Exception ex1) {
                        timeout = timeout + 1;
                        Timer1.Interval = 200;
                        //RichTextBox1.AppendText(ex1.ToString)
                        //RichTextBox1.AppendText(CStr(timeout) + " ")
                        Label8.Visible = true;
                        Retry.Visible = false;
                    }
                } else {
                    if (timeout < 10000) {
                        //uh oh
                        timeout = 60001;
                        MessageBox.Show("No response from telescope for 5 seconds");
                        Label8.Text = "Communication Lost";
                        Label8.Visible = true;
                        Retry.Visible = true;
                    }
                }

                RA = quad(lst - HA);
                double targetRAnow = 0;
                double targetDecnow = 0;

                double RAr = deg2rad(RA);
                double DECr = deg2rad(DEC);
                double HAr = deg2rad(HA);
                double alt = rad2deg(Math.Asin(((Math.Sin(latrad)) * (Math.Sin(DECr))) + ((Math.Cos(latrad)) * (Math.Cos(DECr)) * (Math.Cos(HAr)))));
                double az = rad2deg(Math.Atan2(Math.Sin(HAr), (Math.Cos(HAr) * Math.Sin(latrad)) - (Math.Tan(DECr) * Math.Cos(latrad)))) + 180;

                //Update RA and DEC...etc displays

                jdelbl.Text = "Julian date: " + julian;
                lstlbl.Text = "LST: " + hh(lst / 15) + "h " + dm(lst / 15) + "m " + ds(lst / 15) + "s";

                //rounded to nearest 79"
                RA = Math.Round(RA * 45.51111) / 45.51111;
                DEC = Math.Round(DEC * 45.51111) / 45.51111;

                RAr = deg2rad(RA);
                DECr = deg2rad(DEC);

                if (timeout < 60000) {
                    RAlbl.Text = " RA:  " + hh(RA / 15) + "h " + dm(RA / 15) + "m " + ds(RA / 15) + "s";
                    DEClbl.Text = "DEC: " + dd(DEC) + (Char)176 + " " + dm(DEC) + (Char)39 + " " + ds(DEC) + (Char)34;
                    halbl.Text = "Hour Angle: " + hh(HA / 15) + "h " + dm(HA / 15) + "m " + ds(HA / 15) + "s";
                    azlbl.Text = "Azimuth: " + String.Format("{0:0.00}", az) + (Char)176;
                    altlbl.Text = "Altitude: " + String.Format("{0:0.00}", alt) + (Char)176;
                } else {
                    RAlbl.Text = " RA:  --h --m --s";
                    DEClbl.Text = "DEC: ---" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;
                    halbl.Text = "Hour Angle: --h --m --s";
                    domeazlbl.Text = "Dome azimuth: ---" + (Char)176;
                    enclbl.Text = "Encoders: ----- -----";
                    azlbl.Text = "Azimuth: ---" + (Char)176;
                    altlbl.Text = "Altitude: --" + (Char)176;
                }

                //sample target coords go here
                //Globals.targetRA = (19 + (26 / 60) + (31 / 3600)) * 15
                //Globals.targetDec = 0 + (21 / 60)

                Label11.Text = "Name: " + Globals.targetname;
                if (Globals.targettype.Length > 4) {
                    if (Globals.targettype.Substring(0, 4) == "DSS_") {
                        Label12.Text = "Type: " + Globals.targettype.Substring(4);
                    } else {
                        Label12.Text = "Type: " + Globals.targettype;
                    }
                } else {
                    Label12.Text = "Type: " + Globals.targettype;
                }

                //Calculate various solar system parameters related to the sun, ecliptic..etc

                double sM = 357.52911 + (T * (35999.05029 + (0.0001537 * T)));
                double L = 280.4665 + (36000.7698 * T);
                double Ldash = 218.3165 + (481267.8813 * T);
                double sL0 = 280.46646 + (T * (36000.76983 + (0.0003032 * T)));
                double sC = ((1.914602 - (T * (0.004817 + (1.4E-05 * T)))) * (Math.Sin(deg2rad(sM)))) + ((0.019993 - (T * 0.000101)) * (Math.Sin(deg2rad(2 * sM)))) + 0.000289 * (Math.Sin(deg2rad(3 * sM)));
                double see = 0.016708634 - (T * (4.2037E-05 + (1.267E-07 * T)));
                double sv = sM + sC;
                double sR = (1.000001018 * (1 - (see * see))) / (1 + (see * (Math.Cos(deg2rad(sv)))));
                double omega = 125.0445479 - (1934.1362891 * T) + (0.0020754 * T * T) + ((T * T * T) / 467441) - ((T * T * T * T) / 60616000);
                double deltaepsilon = (((9.2 / 3600) * (Math.Cos(deg2rad(omega)))) + ((0.57 / 3600) * (Math.Cos(deg2rad(2 * L)))) + ((0.1 / 3600) * (Math.Cos(deg2rad(2 * Ldash)))) - ((0.09 / 3600) * (Math.Cos(deg2rad(2 * omega)))));
                double deltapsi = (((-17.2 / 3600) * (Math.Sin(deg2rad(omega)))) - ((1.32 / 3600) * (Math.Sin(deg2rad(2 * L)))) - ((2.23 / 3600) * (Math.Sin(deg2rad(2 * Ldash)))) + ((0.21 / 3600) * (Math.Sin(deg2rad(2 * omega)))));
                double epsilon0 = 23.4392911111 - (0.013004166667 * T) - ((1.63889 / 10000000) * T * T) + (T * T * T * (5.03611 / 10000000));
                double epsilon = epsilon0 + deltaepsilon;
                double epsilonr = deg2rad(epsilon);
                double Slon = sL0 + sC;
                double Slonr = deg2rad(Slon);

                // debug.Text = Globals.targettype + " " + Globals.targetname;


                if (Globals.targettype == "Planet" | Globals.targettype == "Moon" | Globals.targettype == "Sun") {
                    //>Solar system
                    PictureBox1.Visible = true;
                    Label3.Text = " RA J2000:  --h --m --s";
                    Label4.Text = "DEC J2000: ---" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;

                    if (Globals.targetname == "Sun") {
                        PictureBox1.Image = Image.FromFile("images/sun.jpg");
                        Label54.Text = "";
                        Label45.Text = "";
                        Label46.Text = "";
                        Label47.Text = "";

                        targetRAnow = quad(rad2deg(Math.Atan2(((Math.Cos(deg2rad(epsilon))) * (Math.Sin(deg2rad(Slon)))), (Math.Cos(deg2rad(Slon))))));
                        targetDecnow = rad2deg(Math.Asin((Math.Sin(deg2rad(epsilon))) * (Math.Sin(deg2rad(Slon)))));


                    } else if (Globals.targetname == "Moon") {
                        //Condensed ELP2000 moon theory coordinates

                        double Ld = quad(218.3164477 + (481267.88123421 * T) - (0.0015786 * T * T) + ((T * T * T) / 538841) - ((T * T * T * T) / 65194000));
                        double D = quad(297.8501921 + (445267.1114034 * T) - (0.0018819 * T * T) + ((T * T * T) / 545868) - ((T * T * T * T) / 113065000));
                        double M = quad(357.5291092 + (35999.0502909 * T) - (0.0001536 * T * T) + ((T * T * T) / 24490000));
                        double Md = quad(134.9633964 + (477198.8675055 * T) + (0.0087414 * T * T) + ((T * T * T) / 69699) - ((T * T * T * T) / 14712000));
                        double F = quad(93.272095 + (483202.0175233 * T) - (0.0036539 * T * T) + ((T * T * T) / 3526000) - ((T * T * T * T) / 863310000));

                        double A1 = 119.75 + (131.849 * T);
                        double A2 = 53.09 + (479264.29 * T);
                        double A3 = 313.45 + (481266.484 * T);
                        double ee = 1 - (0.002516 * T) - (7.4E-06 * T * T);



                        double longi = 6288774 * Math.Sin(deg2rad(Md)) + 1274027 * Math.Sin(deg2rad((2 * D) - Md)) + 658314 * Math.Sin(deg2rad((2 * D))) + 213618 * Math.Sin(deg2rad((2 * Md))) - 185116 * ee * Math.Sin(deg2rad(M)) - 114332 * Math.Sin(deg2rad((2 * F))) + 58793 * Math.Sin(deg2rad((2 * D) - (2 * Md))) + 57066 * ee * Math.Sin(deg2rad((2 * D) - M - Md)) + 53322 * Math.Sin(deg2rad((2 * D) + Md)) + 45758 * ee * Math.Sin(deg2rad((2 * D) - M)) - 40923 * ee * Math.Sin(deg2rad(M - Md)) - 34720 * Math.Sin(deg2rad(D)) - 30383 * ee * Math.Sin(deg2rad(M + Md)) + 15327 * Math.Sin(deg2rad((2 * D) - (2 * F))) - 12528 * Math.Sin(deg2rad(Md + (2 * F))) + 10980 * Math.Sin(deg2rad(Md - (2 * F))) + 10675 * Math.Sin(deg2rad((4 * D) - Md)) + 10034 * Math.Sin(deg2rad((3 * Md))) + 8548 * Math.Sin(deg2rad((4 * D) - (2 * Md))) - 7888 * ee * Math.Sin(deg2rad((2 * D) + M - Md)) - 6766 * ee * Math.Sin(deg2rad((2 * D) + M)) - 5163 * Math.Sin(deg2rad(D - Md)) + 4987 * ee * Math.Sin(deg2rad(D + M)) + 4036 * ee * Math.Sin(deg2rad((2 * D) - M + Md)) + 3994 * Math.Sin(deg2rad((2 * D) + (2 * Md))) + 3861 * Math.Sin(deg2rad((4 * D))) + 3665 * Math.Sin(deg2rad((2 * D) - (3 * Md))) - 2689 * ee * Math.Sin(deg2rad(M - (2 * Md))) - 2602 * Math.Sin(deg2rad((2 * D) - Md + (2 * F))) + 2390 * ee * Math.Sin(deg2rad((2 * D) - M - (2 * Md))) - 2348 * Math.Sin(deg2rad(D + Md)) + 2236 * ee * ee * Math.Sin(deg2rad((2 * D) - (2 * M))) - 2120 * ee * Math.Sin(deg2rad(M + (2 * Md))) - 2069 * ee * ee * Math.Sin(deg2rad((2 * M))) + 2048 * ee * ee * Math.Sin(deg2rad((2 * D) - (2 * M) - Md)) - 1773 * Math.Sin(deg2rad((2 * D) + Md - (2 * F))) - 1595 * Math.Sin(deg2rad((2 * D) + (2 * F))) + 1215 * ee * Math.Sin(deg2rad((4 * D) - M - Md)) - 1110 * Math.Sin(deg2rad((2 * Md) + (2 * F))) - 892 * Math.Sin(deg2rad((3 * D) - Md)) - 810 * ee * Math.Sin(deg2rad((2 * D) + M + Md)) + 759 * ee * Math.Sin(deg2rad((4 * D) - M - (2 * Md))) - 713 * ee * ee * Math.Sin(deg2rad((2 * M) - Md)) - 700 * ee * ee * Math.Sin(deg2rad((2 * D) + (2 * M) - Md)) + 691 * ee * Math.Sin(deg2rad((2 * D) + M - (2 * Md))) + 596 * ee * Math.Sin(deg2rad((2 * D) - M - (2 * F))) + 549 * Math.Sin(deg2rad((4 * D) + Md)) + 537 * Math.Sin(deg2rad((4 * Md))) + 520 * ee * Math.Sin(deg2rad((4 * D) - M)) - 487 * Math.Sin(deg2rad(D - (2 * Md))) - 399 * ee * Math.Sin(deg2rad((2 * D) + M - (2 * F))) - 381 * Math.Sin(deg2rad((2 * Md) - (2 * F))) + 351 * ee * Math.Sin(deg2rad(D + M + Md)) - 340 * Math.Sin(deg2rad((3 * D) - (2 * Md))) + 330 * Math.Sin(deg2rad((4 * D) - (3 * Md))) + 327 * ee * Math.Sin(deg2rad((2 * D) - M + (2 * Md))) - 323 * ee * ee * Math.Sin(deg2rad((2 * M) + Md)) + 299 * ee * Math.Sin(deg2rad(D + M - Md)) + 294 * Math.Sin(deg2rad((2 * D) + (3 * Md))) + 3958 * Math.Sin(deg2rad(A1)) + 1962 * Math.Sin(deg2rad(Ld - F)) + 318 * Math.Sin(deg2rad(A2));
                        double lat = 5128122 * Math.Sin(deg2rad(F)) + 280602 * Math.Sin(deg2rad(Md + F)) + 277693 * Math.Sin(deg2rad(Md - F)) + 173237 * Math.Sin(deg2rad((2 * D) - F)) + 55413 * Math.Sin(deg2rad((2 * D) - Md + F)) + 46271 * Math.Sin(deg2rad((2 * D) - Md - F)) + 32573 * Math.Sin(deg2rad((2 * D) + F)) + 17198 * Math.Sin(deg2rad((2 * Md) + F)) + 9266 * Math.Sin(deg2rad((2 * D) + Md - F)) + 8822 * Math.Sin(deg2rad((2 * Md) - F)) + 8216 * ee * Math.Sin(deg2rad((2 * D) - M - F)) + 4324 * Math.Sin(deg2rad((2 * D) - (2 * Md) - F)) + 4200 * Math.Sin(deg2rad((2 * D) + Md + F)) - 3359 * ee * Math.Sin(deg2rad((2 * D) + M - F)) + 2463 * ee * Math.Sin(deg2rad((2 * D) - M - Md + F)) + 2211 * ee * Math.Sin(deg2rad((2 * D) - M + F)) + 2065 * ee * Math.Sin(deg2rad((2 * D) - M - Md - F)) - 1870 * ee * Math.Sin(deg2rad(M - Md - F)) + 1828 * Math.Sin(deg2rad((4 * D) - Md - F)) - 1794 * ee * Math.Sin(deg2rad(M + F)) - 1749 * Math.Sin(deg2rad((3 * F))) - 1565 * ee * Math.Sin(deg2rad(M - Md + F)) - 1491 * Math.Sin(deg2rad(D + F)) - 1475 * ee * Math.Sin(deg2rad(M + Md + F)) - 1410 * ee * Math.Sin(deg2rad(M + Md - F)) - 1344 * ee * Math.Sin(deg2rad(M - F)) - 1335 * Math.Sin(deg2rad(D - F)) + 1107 * Math.Sin(deg2rad((3 * Md) + F)) + 1021 * Math.Sin(deg2rad((4 * D) - F)) + 833 * Math.Sin(deg2rad((4 * D) - Md + F)) + 777 * Math.Sin(deg2rad(Md - (3 * F))) + 671 * Math.Sin(deg2rad((4 * D) - (2 * Md) + F)) + 607 * Math.Sin(deg2rad((2 * D) - (3 * F))) + 596 * Math.Sin(deg2rad((2 * D) + (2 * Md) - F)) + 491 * Math.Sin(deg2rad((2 * D) - M + Md - F)) - 451 * Math.Sin(deg2rad((2 * D) - (2 * Md) + F)) + 439 * Math.Sin(deg2rad((3 * Md) - F)) + 422 * Math.Sin(deg2rad((2 * D) + (2 * Md) + F)) + 421 * Math.Sin(deg2rad((2 * D) - (3 * Md) - F)) - 366 * ee * Math.Sin(deg2rad((2 * D) + M - Md + F)) - 351 * ee * Math.Sin(deg2rad((2 * D) + M + F)) + 331 * Math.Sin(deg2rad((4 * D) + F)) + 315 * ee * Math.Sin(deg2rad((2 * D) - M + Md + F)) + 302 * ee * ee * Math.Sin(deg2rad((2 * D) - (2 * M) - F)) - 283 * Math.Sin(deg2rad(Md + (3 * F))) - 229 * ee * Math.Sin(deg2rad((2 * D) + M + Md - F)) + 223 * ee * Math.Sin(deg2rad(D + M - F)) + 223 * ee * Math.Sin(deg2rad(D + M + F)) - 220 * ee * Math.Sin(deg2rad(M - (2 * Md) - F)) - 220 * ee * Math.Sin(deg2rad((2 * D) + M - Md - F)) - 185 * Math.Sin(deg2rad(D + Md + F)) + 181 * ee * Math.Sin(deg2rad((2 * D) - M - (2 * Md) - F)) - 177 * ee * Math.Sin(deg2rad(M + (2 * Md) + F)) + 176 * Math.Sin(deg2rad((4 * D) - (2 * Md) - F)) + 166 * ee * Math.Sin(deg2rad((4 * D) - M - Md - F)) - 164 * Math.Sin(deg2rad(D + Md - F)) + 132 * Math.Sin(deg2rad((4 * D) + Md - F)) - 119 * Math.Sin(deg2rad(D - Md - F)) + 115 * ee * Math.Sin(deg2rad((4 * D) - M - F)) + 107 * ee * Math.Sin(deg2rad((2 * D) - (2 * M) + F)) - 2235 * Math.Sin(deg2rad(Ld)) + 382 * Math.Sin(deg2rad(A3)) + 175 * Math.Sin(deg2rad(A1 - F)) + 175 * Math.Sin(deg2rad(A1 + F)) + 127 * Math.Sin(deg2rad(Ld - Md)) - 115 * Math.Sin(deg2rad(Ld + Md));
                        double dist = (-20905355 * Math.Cos(deg2rad(Md))) - 3699111 * Math.Cos(deg2rad((2 * D) - Md)) - 2955968 * Math.Cos(deg2rad((2 * D))) - 569925 * Math.Cos(deg2rad((2 * Md))) + 48888 * ee * Math.Cos(deg2rad(M)) - 3149 * Math.Cos(deg2rad((2 * F))) + 246158 * Math.Cos(deg2rad((2 * D) - (2 * Md))) - 152138 * ee * Math.Cos(deg2rad((2 * D) - M - Md)) - 170733 * Math.Cos(deg2rad((2 * D) + Md)) - 204586 * ee * Math.Cos(deg2rad((2 * D) - M)) - 129620 * ee * Math.Cos(deg2rad(M - Md)) + 108743 * Math.Cos(deg2rad(D)) + 104755 * ee * Math.Cos(deg2rad(M + Md)) + 10321 * Math.Cos(deg2rad((2 * D) - (2 * F))) + 79661 * Math.Cos(deg2rad(Md - (2 * F))) - 34782 * Math.Cos(deg2rad((4 * D) - Md)) - 23210 * Math.Cos(deg2rad((3 * Md))) - 21636 * Math.Cos(deg2rad((4 * D) - (2 * Md))) + 24208 * ee * Math.Cos(deg2rad((2 * D) + M - Md)) + 30827 * ee * Math.Cos(deg2rad((2 * D) + M)) - 8379 * Math.Cos(deg2rad(D - Md)) - 16675 * ee * Math.Cos(deg2rad(D + M)) - 12831 * ee * Math.Cos(deg2rad((2 * D) - M + Md)) - 10445 * Math.Cos(deg2rad((2 * D) + (2 * Md))) - 11650 * Math.Cos(deg2rad((4 * D))) + 14403 * Math.Cos(deg2rad((2 * D) - (3 * Md))) - 7003 * ee * Math.Cos(deg2rad(M - (2 * Md))) + 10056 * ee * Math.Cos(deg2rad((2 * D) - M - (2 * Md))) + 6322 * Math.Cos(deg2rad(D + Md)) - 9884 * ee * ee * Math.Cos(deg2rad((2 * D) - (2 * M))) + 5751 * ee * Math.Cos(deg2rad(M + (2 * Md))) - 4950 * ee * ee * Math.Cos(deg2rad((2 * D) - (2 * M) - Md)) + 4130 * Math.Cos(deg2rad((2 * D) + Md - (2 * F))) - 3958 * ee * Math.Cos(deg2rad((4 * D) - M - Md)) + 3258 * Math.Cos(deg2rad((3 * D) - Md)) + 2616 * ee * Math.Cos(deg2rad((2 * D) + M + Md)) - 1897 * ee * Math.Cos(deg2rad((4 * D) - M - (2 * Md))) - 2117 * ee * ee * Math.Cos(deg2rad((2 * M) - Md)) + 2354 * ee * ee * Math.Cos(deg2rad((2 * D) + (2 * M) - Md)) - 1423 * Math.Cos(deg2rad((4 * D) + Md)) - 1117 * Math.Cos(deg2rad((4 * Md))) - 1571 * ee * Math.Cos(deg2rad((4 * D) - M)) - 1739 * Math.Cos(deg2rad(D - (2 * Md))) - 4421 * Math.Cos(deg2rad((2 * Md) - (2 * F))) - 1165 * ee * ee * Math.Cos(deg2rad((2 * M) + Md)) + 8752 * Math.Cos(deg2rad(2 * D - Md - 2 * F));

                        double lambda = quad(Ld + (longi / 1000000));
                        double beta = deg2rad(lat / 1000000);
                        double delta = 385000.56 + (dist / 1000);

                        lambda = deg2rad(lambda + deltapsi);
                        // + psi

                        targetRAnow = rad2deg(Math.Atan2((((Math.Cos(deg2rad(epsilon))) * (Math.Sin(lambda))) - ((Math.Tan(beta)) * (Math.Sin(deg2rad(epsilon))))), (Math.Cos(lambda))));
                        targetDecnow = Math.Asin(((Math.Sin(beta)) * (Math.Cos(deg2rad(epsilon)))) + ((Math.Cos(beta)) * (Math.Sin(deg2rad(epsilon))) * (Math.Sin(lambda))));

                        //RichTextBox1.Text = "lambda: " + CStr(rad2deg(lambda)) + Environment.NewLine + "beta: " + CStr(rad2deg(beta)) + Environment.NewLine + "psi: " + CStr(deltapsi) + Environment.NewLine + "epsilon: " + CStr(epsilon) + Environment.NewLine + "Md: " + CStr(Md) + Environment.NewLine + "Ld: " + CStr(Ld) + Environment.NewLine + "El: " + CStr(longi) + Environment.NewLine + "Eb: " + CStr(lat) + Environment.NewLine + "delta: " + CStr(delta) + Environment.NewLine
                        //Parallax correction

                        double deltaau = delta / 149597870.691;

                        double H = 70;

                        double sinpi = (Math.Sin(deg2rad(8.794 / 3600))) / deltaau;
                        //##### au
                        double ruserlat = latrad;
                        double ruserlong = deg2rad(-0.0963);
                        double rra = deg2rad(targetRAnow);
                        double rdec = targetDecnow;

                        double C = 1 - (1 / 298.257);
                        double u = Math.Atan(C * Math.Tan(ruserlat));
                        double psin = C * Math.Sin(u) + (H / 6378140) * Math.Sin(ruserlat);
                        double pcos = Math.Cos(u) + (H / 6378140) * Math.Cos(ruserlat);

                        H = deg2rad(lst - targetRAnow);
                        //hour angle

                        double moonH = H;
                        double mA = (Math.Cos(rdec)) * (Math.Sin(H));
                        double mB = (Math.Cos(rdec) * Math.Cos(H)) - (pcos * sinpi);
                        double mC = (Math.Sin(rdec)) - (psin * sinpi);
                        double Q = Math.Sqrt((mA * mA) + (mB * mB) + (mC * mC));
                        H = rad2deg(Math.Atan2(mA, mB));
                        //corrected HA in degs
                        targetDecnow = rad2deg(Math.Asin(mC / Q));
                        double targetDecnowr = Math.Asin(mC / Q);
                        //Dim targetDecnowr As Double = targetDecnow
                        //targetDecnow = rad2deg(targetDecnowr)
                        targetRAnow = quad(lst - H);


                        malt = rad2deg(Math.Asin(((Math.Sin(latrad)) * (Math.Sin(targetDecnowr))) + ((Math.Cos(latrad)) * (Math.Cos(targetDecnowr)) * (Math.Cos(H)))));
                        maz = rad2deg(Math.Atan2(Math.Sin(H), (Math.Cos(H) * Math.Sin(latrad)) - (Math.Tan(targetDecnowr) * Math.Cos(latrad)))) + 180;


                        //----------------------------
                        double lambdaH = Slon + 180 + (deltaau * 57.296 * ((Math.Cos(beta)) * (Math.Cos(deg2rad(Slon) - lambda))));
                        double betaH = deg2rad(rad2deg(beta) * deltaau);
                        //----------------------------
                        double I = deg2rad(1.5424166666);
                        double WH = deg2rad(quad(lambdaH - deltapsi - omega));
                        double AH = quad(rad2deg(Math.Atan2((Math.Sin(WH) * Math.Cos(betaH) * Math.Cos(I)) - (Math.Sin(betaH) * Math.Sin(I)), Math.Cos(WH) * Math.Cos(betaH))));
                        double lH = quad(AH - F);

                        if (lH > 90) {
                            lH = lH - 360;
                        }
                        double bH = Math.Asin(((-Math.Sin(WH)) * Math.Cos(betaH) * Math.Sin(I)) - (Math.Sin(betaH) * Math.Cos(I)));


                        double psii = Math.Acos((Math.Cos(beta)) * (Math.Cos(lambda - deg2rad(Slon))));
                        double ii = rad2deg(Math.Atan2(sR * Math.Sin(psii), deltaau - (sR * Math.Cos(psii))));
                        double k = (1 + (Math.Cos(deg2rad((ii))))) / 0.02;


                        double W = lambda - deg2rad(deltapsi + omega);
                        double Am = quad(rad2deg(Math.Atan2((Math.Sin(W) * Math.Cos(beta) * Math.Cos(I)) - (Math.Sin(beta) * Math.Sin(I)), Math.Cos(W) * Math.Cos(beta))));
                        Ld = (Am - F);


                        if (Ld < -90) {
                            Ld = Ld - (((Math.Round(Ld / 360, 0))) * 360);
                        } else {
                            if (Ld > 90) {
                                Ld = Ld - ((Math.Floor(Ld / 360)) * 360);

                            } else {
                                //Ld = Ld;
                            }
                        }


                        double bd = rad2deg(Math.Asin(((-Math.Sin(W)) * Math.Cos(beta) * Math.Sin(I)) - (Math.Sin(beta) * Math.Cos(I))));
                        //physical libration
                        double K1 = 119.75 + (131.849 * T);
                        double K2 = 72.56 + (20.186 * T);

                        double rho = -0.02752 * (Math.Cos(deg2rad(Md))) - 0.02245 * (Math.Sin(deg2rad(F))) + 0.00684 * (Math.Cos(deg2rad(Md - 2 * F))) - 0.00293 * (Math.Cos(deg2rad(2 * F))) - 0.00085 * (Math.Cos(deg2rad(2 * F - 2 * D))) - 0.00054 * (Math.Cos(deg2rad(Md - 2 * D))) - 0.0002 * (Math.Sin(deg2rad(Md + F))) - 0.0002 * (Math.Cos(deg2rad(Md + 2 * F))) - 0.0002 * (Math.Cos(deg2rad(Md - F))) + 0.00014 * (Math.Cos(deg2rad(Md + 2 * F - 2 * D)));
                        double sigma = -0.02816 * (Math.Sin(deg2rad(Md))) + 0.02244 * (Math.Cos(deg2rad(F))) - 0.00682 * (Math.Sin(deg2rad(Md - 2 * F))) - 0.00279 * (Math.Sin(deg2rad(2 * F))) - 0.00083 * (Math.Sin(deg2rad(2 * F - 2 * D))) + 0.00069 * (Math.Sin(deg2rad(Md - 2 * D))) + 0.0004 * (Math.Cos(deg2rad(Md + F))) - 0.00025 * (Math.Sin(deg2rad(2 * Md))) - 0.00023 * (Math.Sin(deg2rad(Md + 2 * F))) + 0.0002 * (Math.Cos(deg2rad(Md - F))) + 0.00019 * (Math.Sin(deg2rad(Md - F))) + 0.00013 * (Math.Sin(deg2rad(Md + 2 * F - 2 * D))) - 0.0001 * (Math.Cos(deg2rad(Md - 3 * F)));
                        double tau = 0.0252 * ee * (Math.Sin(deg2rad(M))) + 0.00473 * (Math.Sin(deg2rad(2 * Md - 2 * F))) - 0.00467 * (Math.Sin(deg2rad(Md))) + 0.00396 * (Math.Sin(deg2rad(K1))) + 0.00276 * (Math.Sin(deg2rad(2 * Md - 2 * D))) + 0.00196 * (Math.Sin(deg2rad(omega))) - 0.00183 * (Math.Cos(deg2rad(Md - F))) + 0.00115 * (Math.Sin(deg2rad(Md - 2 * D))) - 0.00096 * (Math.Sin(deg2rad(Md - D))) + 0.00046 * (Math.Sin(deg2rad(2 * F - 2 * D))) - 0.00039 * (Math.Sin(deg2rad(Md - F))) - 0.00032 * (Math.Sin(deg2rad(Md - M - D))) + 0.00027 * (Math.Sin(deg2rad(2 * Md - M - 2 * D))) + 0.00023 * (Math.Sin(deg2rad(K2))) - 0.00014 * (Math.Sin(deg2rad(2 * D))) + 0.00014 * (Math.Cos(deg2rad(2 * Md - 2 * F))) - 0.00012 * (Math.Sin(deg2rad(Md - 2 * F))) - 0.00012 * (Math.Sin(deg2rad(2 * Md))) + 0.00011 * (Math.Sin(deg2rad(2 * Md - 2 * M - 2 * D)));

                        double Ldd = -tau + (rho * Math.Cos(deg2rad(Am)) + sigma * Math.Sin(deg2rad(Am))) * Math.Tan(moonb);
                        double bdd = sigma * Math.Cos(deg2rad(Am)) - rho * Math.Sin(deg2rad(Am));

                        double ldH = -tau + (rho * Math.Cos(deg2rad(AH)) + sigma * Math.Sin(deg2rad(AH))) * Math.Tan(bH);
                        double bdH = sigma * Math.Cos(deg2rad(AH)) - rho * Math.Sin(deg2rad(AH));



                        lhr = lH + ldH;
                        bhr = rad2deg(bH) + bdH;

                        double b1 = deg2rad(bhr);


                        //position angle
                        double v = omega + deltapsi + (rho / (Math.Sin(I)));
                        double X = Math.Sin(I + deg2rad(rho)) * Math.Sin(deg2rad(v));
                        double Y = (Math.Sin(I + deg2rad(rho)) * Math.Cos(deg2rad(v)) * Math.Cos(deg2rad(epsilon))) - (Math.Cos(I + deg2rad(rho)) * Math.Sin(deg2rad(epsilon)));
                        double W2 = Math.Atan2(X, Y);
                        double alpha = deg2rad(quad(targetRAnow));

                        double P1 = Math.Asin(((Math.Sqrt((X * X) + (Y * Y))) * Math.Cos(alpha - W2)) / (Math.Cos(b1)));


                        //topocentric lib

                        //Dim phi As Double = latrad
                        //rra = deg2rad(rageo)
                        //rdec = deg2rad(decgeo)
                        //Dim ast As Double = 280.46061837 + (360.98564736629 * (juliandec - 2451545)) + (0.000387922 * T * T) - ((T * T * T) / 38710000)


                        moonb = deg2rad(bd + bdd);




                        Q = Math.Atan2((Math.Cos(latrad) * Math.Sin(moonH)), (Math.Cos(rdec) * Math.Sin(latrad) - Math.Sin(rdec) * Math.Cos(latrad) * Math.Cos(moonH)));
                        // had wrong sign and Sine on denom
                        double z = Math.Acos(Math.Sin(rdec) * Math.Sin(latrad) + Math.Cos(rdec) * Math.Cos(latrad) * Math.Cos(moonH));
                        //had wrong asin

                        double pi = (8.794 / 3600) / deltaau;
                        double pid = pi * (Math.Sin(z) + 0.0084 * Math.Sin(2 * z));


                        double deltal = -(pid * Math.Sin(Q - P1)) / Math.Cos(moonb);
                        //p1 was in degs
                        double deltab = pid * Math.Cos(Q - P1);
                        //was latrad instead of pid
                        double deltaP = deltal * Math.Sin(moonb + (deg2rad(deltab))) - pid * Math.Sin(Q) * Math.Tan(rdec);


                        posangle = rad2deg(P1) + deltaP;
                        double Pround = posangle;

                        moonl = Ld + Ldd + deltal;
                        moonb = bd + bdd + deltab;


                        if (count == 0) {
                            moonmap();
                        }
                        count = count + 1;
                        if (count > 9000) {
                            count = 0;
                        }

                        if (moonl > 180) { moonl = moonl - 360; }
                        if (moonb > 180) { moonb = moonb - 360; }
                        //RichTextBox1.Text = "ldd: " + CStr(Ldd) + Environment.NewLine + "bdd: " + CStr(bdd) + Environment.NewLine + "ld: " + CStr(Ld) + Environment.NewLine + "bd: " + CStr(bd) + Environment.NewLine + "deltal: " + CStr(deltal) + Environment.NewLine + "deltab: " + CStr(deltab)

                        Label54.Text = "Moon phase: " + String.Format("{0:0.0}", k) + (Char)37;
                        Label45.Text = "Lib. Longitude: " + String.Format("{0:0.000}", moonl) + (Char)176;
                        Label46.Text = "Lib. Latitude: " + String.Format("{0:0.000}", moonb) + (Char)176;
                        Label47.Text = "Position angle: " + String.Format("{0:0.00}", posangle) + (Char)176;



                    } else if (Globals.targettype == "Planet") {

                        Label54.Text = "";
                        Label45.Text = "";
                        Label46.Text = "";
                        Label47.Text = "";

                        //Planet and Earth terms for orbital location

                        pt pterms = new pt();


                        if (Globals.targetname == "Mercury") {
                            PictureBox1.Image = null;
                            tt = (T / 10) - ((0.0057755183 * 1) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptmercury(tt, 1);
                            bp = pterms.ptmercury(tt, 2);
                            rp = pterms.ptmercury(tt, 3);

                        } else if (Globals.targetname == "Venus") {
                            PictureBox1.Image = Image.FromFile("images/venus.jpg");
                            tt = (T / 10) - ((0.0057755183 * 1) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptvenus(tt, 1);
                            bp = pterms.ptmercury(tt, 2);
                            rp = pterms.ptmercury(tt, 3);
                        } else if (Globals.targetname == "Mars") {
                            PictureBox1.Image = Image.FromFile("images/mars.jpg");
                            tt = (T / 10) - ((0.0057755183 * 1) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptmars(tt, 1);
                            bp = pterms.ptmars(tt, 2);
                            rp = pterms.ptmars(tt, 3);
                        } else if (Globals.targetname == "Jupiter") {
                            PictureBox1.Image = Image.FromFile("images/jupiter.jpg");
                            tt = (T / 10) - ((0.0057755183 * 5) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptjupiter(tt, 1);
                            bp = pterms.ptjupiter(tt, 2);
                            rp = pterms.ptjupiter(tt, 3);
                        } else if (Globals.targetname == "Saturn") {
                            PictureBox1.Image = Image.FromFile("images/saturn.jpg");
                            tt = (T / 10) - ((0.0057755183 * 9) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptsaturn(tt, 1);
                            bp = pterms.ptsaturn(tt, 2);
                            rp = pterms.ptsaturn(tt, 3);
                        } else if (Globals.targetname == "Uranus") {
                            PictureBox1.Image = null;
                            tt = (T / 10) - ((0.0057755183 * 20) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.pturanus(tt, 1);
                            bp = pterms.pturanus(tt, 2);
                            rp = pterms.pturanus(tt, 3);
                        } else if (Globals.targetname == "Neptune") {
                            PictureBox1.Image = null;
                            tt = (T / 10) - ((0.0057755183 * 30) / 365250);
                            le = pterms.ptearth(tt, 1);
                            be = pterms.ptearth(tt, 2);
                            re = pterms.ptearth(tt, 3);

                            lp = pterms.ptneptune(tt, 1);
                            bp = pterms.ptneptune(tt, 2);
                            rp = pterms.ptneptune(tt, 3);
                        } else {

                            PictureBox1.Image = null;
                        }

                        double x = rp * Math.Cos(bp) * Math.Cos(lp) - re * Math.Cos(be) * Math.Cos(le);
                        double y = rp * Math.Cos(bp) * Math.Sin(lp) - re * Math.Cos(be) * Math.Sin(le);
                        double z = rp * Math.Sin(bp) - re * Math.Sin(be);

                        double lambda = Math.Atan2(y, x);
                        double beta = Math.Atan2(z, Math.Sqrt(x * x + y * y));

                        targetRAnow = quad(rad2deg(Math.Atan2((((Math.Cos(deg2rad(epsilon))) * (Math.Sin(lambda))) - ((Math.Tan(beta)) * (Math.Sin(deg2rad(epsilon))))), (Math.Cos(lambda)))));
                        targetDecnow = rad2deg(Math.Asin(((Math.Sin(beta)) * (Math.Cos(deg2rad(epsilon)))) + ((Math.Cos(beta)) * (Math.Sin(deg2rad(epsilon))) * (Math.Sin(lambda)))));

                    }

                } else {


                    if (Globals.targettype.Length > 4) {
                        if (!(Globals.targettype.Substring(0, 4) == "DSS_")) {
                            PictureBox1.Image = null;
                            PictureBox1.Visible = false;
                        }
                    } else {
                        PictureBox1.Image = null;
                        PictureBox1.Visible = false;
                    }
                    Label54.Text = "";
                    Label45.Text = "";
                    Label46.Text = "";
                    Label47.Text = "";

                    //If the target is not in the solar system we need to correct the coordinates for a few things

                    //print J2000 coords of target 
                    Label3.Text = " RA J2000:  " + hh(Globals.targetRA / 15) + "h " + dm(Globals.targetRA / 15) + "m " + ds(Globals.targetRA / 15) + "s";
                    Label4.Text = "DEC J2000: " + dd(Globals.targetDec) + (Char)176 + " " + dm(Globals.targetDec) + (Char)39 + " " + ds(Globals.targetDec) + (Char)34;


                    //Correct target coords for precession, nutation, aberration----------------------

                    //precession
                    dynamic targetRAprecessed = precessra(Globals.targetRA, Globals.targetDec, T);
                    dynamic targetDecprecessed = precessdec(Globals.targetRA, Globals.targetDec, T);

                    //nutation
                    double nutationra = deltapsi * (Math.Cos(epsilonr) + Math.Sin(epsilonr) * Math.Sin(deg2rad(targetRAprecessed)) * Math.Tan(deg2rad(targetDecprecessed))) - deltaepsilon * Math.Cos(deg2rad(targetRAprecessed)) * Math.Tan(deg2rad(targetDecprecessed));
                    double nutationdec = deltapsi * Math.Sin(epsilonr) * Math.Cos(deg2rad(targetRAprecessed)) + deltaepsilon * Math.Sin(deg2rad(targetRAprecessed));

                    //aberration

                    double k = 20.49552 / 3600;
                    double ee = 0.016708634 - 4.2037E-05 * T - 1.267E-07 * T * T;
                    double pir = deg2rad(102.93735 + 1.71946 * T + 0.00046 * T * T);

                    dynamic aberrationra = ((-k / Math.Cos(deg2rad(targetDecprecessed))) * (Math.Cos(deg2rad(targetRAprecessed)) * Math.Cos(Slonr) * Math.Cos(epsilonr) + Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(Slonr))) + (((ee * k) / Math.Cos(deg2rad(targetDecprecessed))) * (Math.Cos(deg2rad(targetRAprecessed)) * Math.Cos(pir) * Math.Cos(epsilonr) + Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(pir)));
                    dynamic aberrationdec = (ee * k * (Math.Cos(pir) * Math.Cos(epsilonr) * (Math.Tan(epsilonr) * Math.Cos(deg2rad(targetDecprecessed)) - Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed))) + Math.Cos(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed)) * Math.Sin(pir))) - (k * (Math.Cos(Slonr) * Math.Cos(epsilonr) * (Math.Tan(epsilonr) * Math.Cos(deg2rad(targetDecprecessed)) - Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed))) + Math.Cos(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed)) * Math.Sin(Slonr)));

                    targetRAnow = targetRAprecessed + nutationra + aberrationra;
                    targetDecnow = targetDecprecessed + nutationdec + aberrationdec;

                }

                double targetRAr = deg2rad(targetRAnow);
                double targetDecr = deg2rad(targetDecnow);
                double targetHAr = deg2rad(lst - targetRAnow);

                //RichTextBox1.Text = CStr(nutationra) + Environment.NewLine + CStr(nutationdec) + Environment.NewLine + CStr(aberrationra) + Environment.NewLine + CStr(aberrationdec)

                double targetalt = rad2deg(Math.Asin(((Math.Sin(latrad)) * (Math.Sin(targetDecr))) + ((Math.Cos(latrad)) * (Math.Cos(targetDecr)) * (Math.Cos(targetHAr)))));
                double targetaz = rad2deg(Math.Atan2(Math.Sin(targetHAr), (Math.Cos(targetHAr) * Math.Sin(latrad)) - (Math.Tan(targetDecr) * Math.Cos(latrad)))) + 180;

                if (targetalt > -0.5) {
                    //atmospheric refraction
                    double Rh = ((1.02 / Math.Tan(deg2rad(targetalt + (10.3 / (targetalt + 5.11))))) + 0.0019279) / 60;
                    targetalt = targetalt + Rh;

                    double targetaltr = deg2rad(targetalt);
                    double targetazr = deg2rad(targetaz - 180);

                    targetHAr = Math.Atan2(Math.Sin(targetazr), (Math.Cos(targetazr) * Math.Sin(latrad) + Math.Tan(targetaltr) * Math.Cos(latrad)));
                    targetDecr = Math.Asin(Math.Sin(latrad) * Math.Sin(targetaltr) - Math.Cos(latrad) * Math.Cos(targetaltr) * Math.Cos(targetazr));
                    targetRAr = deg2rad(lst) - targetHAr;

                    //targetRAnow = quad(rad2deg(targetRAr))
                    //targetDecnow = rad2deg(targetDecr)
                }

                dynamic targetRArnow = deg2rad(targetRAnow);
                dynamic targetDecrnow = deg2rad(targetDecnow);
                //Show JNow coords
                Label5.Text = " RA:  " + hh(targetRAnow / 15) + "h " + dm(targetRAnow / 15) + "m " + ds(targetRAnow / 15) + "s";
                Label6.Text = "DEC: " + dd(targetDecnow) + (Char)176 + " " + dm(targetDecnow) + (Char)39 + " " + ds(targetDecnow) + (Char)34;
                Label9.Text = "Altitude: " + String.Format("{0:0.00}", targetalt) + (Char)176;
                Label10.Text = "Azimuth: " + String.Format("{0:0.00}", targetaz) + (Char)176;


                //Calculate angular separation between target coords and current telescope position
                double sep = rad2deg(Math.Acos(Math.Sin(DECr) * Math.Sin(targetDecrnow) + Math.Cos(DECr) * Math.Cos(targetDecrnow) * Math.Cos(RAr - targetRArnow)));

                if (sep < 0.5) {
                    distance.ForeColor = Color.LimeGreen;
                } else {
                    distance.ForeColor = Color.Red;
                }

                //Position angle
                double positionangle = quad(rad2deg(Math.Atan2(Math.Sin(targetRArnow - RAr), Math.Cos(DECr) * Math.Tan(targetDecrnow) - Math.Sin(DECr) * Math.Cos(targetRArnow - RAr))));

                //Distances in RA and DEC


                double radist = rad2deg(Math.Acos((Math.Cos(RAr - targetRArnow))));
                double decdist = targetDecnow - DEC;
                //RichTextBox1.Text = CStr(Globals.targetDec) + " " + CStr(DEC) + " " + CStr(decdist) + Environment.NewLine

                if (radist < 0.1) {
                    radistlbl.ForeColor = Color.LimeGreen;
                    Label5.ForeColor = Color.LimeGreen;
                } else {
                    Label5.ForeColor = Color.Red;
                    radistlbl.ForeColor = Color.Red;
                }

                if (Math.Abs(decdist) < 0.1) {
                    decdistlbl.ForeColor = Color.LimeGreen;
                    Label6.ForeColor = Color.LimeGreen;
                } else {
                    Label6.ForeColor = Color.Red;
                    decdistlbl.ForeColor = Color.Red;
                }
                if (timeout < 60000) {
                    distance.Text = "Distance to target: " + String.Format("{0:0.00}", sep) + (Char)176;
                    posanglelbl.Text = "Position angle: " + String.Format("{0:0.0}", positionangle) + (Char)176;
                    radistlbl.Text = "RA difference: " + dd1(radist) + (Char)176 + " " + dm(radist) + (Char)39 + " " + ds(radist) + (Char)34;
                    decdistlbl.Text = "DEC difference: " + dd1(decdist) + (Char)176 + " " + dm(decdist) + (Char)39 + " " + ds(decdist) + (Char)34;
                } else {
                    distance.Text = "Distance to target: --.--" + (Char)176;
                    posanglelbl.Text = "Position angle: --" + (Char)176;
                    radistlbl.Text = "RA difference: --" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;
                    decdistlbl.Text = "DEC difference: --" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;
                    distance.ForeColor = Color.Red;
                    radistlbl.ForeColor = Color.Red;
                    decdistlbl.ForeColor = Color.Red;
                    Label4.ForeColor = Color.Red;
                    Label6.ForeColor = Color.Red;
                }

            } else {
                //no serial port, abandon ship
                Label8.Text = "Port COM" + Properties.Settings.Default.com + " not found";
                Label8.Visible = true;
                Timer1.Enabled = false;
            }

        }



        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e) {
            Settings SettingsInst = new Settings();
            SettingsInst.Show();
            SettingsInst.TextBoxcom.Text = Properties.Settings.Default.com.ToString();
            SettingsInst.TextBoxdd.Text = Properties.Settings.Default.dd.ToString();
            SettingsInst.TextBoxdt.Text = Properties.Settings.Default.dt.ToString();
            SettingsInst.TextBoxi.Text = Properties.Settings.Default.i.ToString();
            SettingsInst.TextBoxc.Text = Properties.Settings.Default.c.ToString();
            SettingsInst.TextBoxe.Text = Properties.Settings.Default.e.ToString();
            SettingsInst.TextBoxgamma.Text = Properties.Settings.Default.gamma.ToString();
            SettingsInst.TextBoxtheta.Text = Properties.Settings.Default.theta.ToString();
            SettingsInst.TextBoxl.Text = Properties.Settings.Default.l.ToString();
            SettingsInst.TextBoxr.Text = Properties.Settings.Default.r.ToString();


        }

        private void Retry_Click(object sender, EventArgs e) {
            timeout = 0;
        }


        /*
        //Stars
        private void Star1ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 104.656452;
            Globals.targetDec = -28.972084;
        }
        private void Star2ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 68.980161;
            Globals.targetDec = 16.509301;
        }
        private void Star3ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 319.644881;
            Globals.targetDec = 62.585573;
        }
        private void Star4ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 154.993144;
            Globals.targetDec = 19.841489;
        }
        private void Star5ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 47.042215;
            Globals.targetDec = 40.955648;
        }
        private void Star6ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 99.427921;
            Globals.targetDec = 16.399252;
        }
        private void Star7ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 193.507289;
            Globals.targetDec = 55.959821;
        }
        private void Star8ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 30.974804;
            Globals.targetDec = 42.329725;
        }
        private void Star9ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 84.053389;
            Globals.targetDec = -1.20192;
        }
        private void Star10ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 85.189696;
            Globals.targetDec = -1.942572;
        }
        private void Star11ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 141.896847;
            Globals.targetDec = -8.658603;
        }
        private void Star12ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 233.67195;
            Globals.targetDec = 26.714693;
        }
        private void Star13ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 2.096911;
            Globals.targetDec = 29.090432;
        }
        private void Star14ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 297.695829;
            Globals.targetDec = 8.868322;
        }
        private void Star15ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 111.023761;
            Globals.targetDec = -29.303104;
        }
        private void Star16ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 247.35192;
            Globals.targetDec = -26.432002;
        }
        private void Star17ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 213.9153;
            Globals.targetDec = 19.18241;
        }
        private void Star18ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 81.282763;
            Globals.targetDec = 6.349702;
        }
        private void Star19ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 206.885157;
            Globals.targetDec = 49.313265;
        }
        private void Star20ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 88.792939;
            Globals.targetDec = 7.407063;
        }
        private void Star21ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 79.172329;
            Globals.targetDec = 45.997991;
        }
        private void Star22ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 2.294521;
            Globals.targetDec = 59.14978;
        }
        private void Star23ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 113.649428;
            Globals.targetDec = 31.888276;
        }
        private void Star24ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 310.357978;
            Globals.targetDec = 45.280338;
        }
        private void Star25ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.897379;
            Globals.targetDec = -17.986605;
        }
        private void Star26ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 177.264907;
            Globals.targetDec = 14.57206;
        }
        private void Star27ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 240.083359;
            Globals.targetDec = -22.62171;
        }
        private void Star28ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 165.931953;
            Globals.targetDec = 61.751033;
        }
        private void Star29ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 81.572972;
            Globals.targetDec = 28.60745;
        }
        private void Star30ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 269.151541;
            Globals.targetDec = 51.488895;
        }
        private void Star31ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 326.046492;
            Globals.targetDec = 9.875011;
        }
        private void Star32ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 344.412694;
            Globals.targetDec = -29.622236;
        }
        private void Star33ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 311.552845;
            Globals.targetDec = 33.970256;
        }
        private void Star34ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 31.793363;
            Globals.targetDec = 23.462423;
        }
        private void Star35ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 222.67636;
            Globals.targetDec = 74.155505;
        }
        private void Star36ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 346.190224;
            Globals.targetDec = 15.205264;
        }
        private void Star37ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 165.46032;
            Globals.targetDec = 56.382427;
        }
        private void Star38ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 17.433015;
            Globals.targetDec = 35.620558;
        }
        private void Star39ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 51.08071;
            Globals.targetDec = 49.86118;
        }
        private void Star40ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 95.674939;
            Globals.targetDec = -17.955918;
        }
        private void Star41ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 200.981429;
            Globals.targetDec = 54.925362;
        }
        private void Star42ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 283.816357;
            Globals.targetDec = -26.296722;
        }
        private void Star43ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 178.457698;
            Globals.targetDec = 53.69476;
        }
        private void Star44ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 37.954515;
            Globals.targetDec = 89.26411;
        }
        private void Star45ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 116.32896;
            Globals.targetDec = 28.026199;
        }
        private void Star46ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 114.825493;
            Globals.targetDec = 5.224993;
        }
        private void Star47ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 263.733627;
            Globals.targetDec = 12.560035;
        }
        private void Star48ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 152.092961;
            Globals.targetDec = 11.967207;
        }
        private void Star49ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 78.634468;
            Globals.targetDec = -8.201641;
        }
        private void Star50ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 305.557091;
            Globals.targetDec = 40.256679;
        }
        private void Star51ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 86.93912;
            Globals.targetDec = -9.669605;
        }
        private void Star52ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 345.943573;
            Globals.targetDec = 28.082789;
        }
        private void Star53ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.126835;
            Globals.targetDec = 56.537331;
        }
        private void Star54ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 101.287155;
            Globals.targetDec = -16.716116;
        }
        private void Star55ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 201.298247;
            Globals.targetDec = -11.161322;
        }
        private void Star56ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 14.177216;
            Globals.targetDec = 60.71674;
        }
        private void Star57ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 279.234735;
            Globals.targetDec = 38.783692;
        }
        private void Star58ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 107.097851;
            Globals.targetDec = -26.3932;
        }

       


        //messier

        private void M1ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Supernova Remnant";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 83.632458;
            Globals.targetDec = 22.0166666;
        }
        private void M2ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 323.3625;
            Globals.targetDec = -0.8233333;
        }
        private void M3ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 205.546791;
            Globals.targetDec = 28.3783333;
        }
        private void M4ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 245.897541;
            Globals.targetDec = -26.525;
        }
        private void M5ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 229.640625;
            Globals.targetDec = 2.0833333;
        }
        private void M6ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 265.0249995;
            Globals.targetDec = -32.1927778;
        }
        private void M7ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 268.4749995;
            Globals.targetDec = -34.7927778;
        }
        private void M8ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Emission Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 270.904167;
            Globals.targetDec = -24.3833333;
        }
        private void M9ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 259.7990835;
            Globals.targetDec = -18.5166667;
        }
        private void M10ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 254.287458;
            Globals.targetDec = -4.1;
        }
        private void M11ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 282.7750005;
            Globals.targetDec = -6.2566667;
        }
        private void M12ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = -29.2000005;
            Globals.targetDec = 1214.4;
        }
        private void M13ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 250.422666;
            Globals.targetDec = 36.46;
        }
        private void M14ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 264.4006245;
            Globals.targetDec = -3.2466667;
        }
        private void M15ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 322.49325;
            Globals.targetDec = 12.1666667;
        }
        private void M16ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster with Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 274.6999995;
            Globals.targetDec = -13.7622222;
        }
        private void M17ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Emission Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 275.108334;
            Globals.targetDec = -16.16;
        }
        private void M18ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 274.9750005;
            Globals.targetDec = -17.16;
        }
        private void M19ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 255.657042;
            Globals.targetDec = -26.2683333;
        }
        private void M20ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Emission/ Reflection Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 270.595833;
            Globals.targetDec = -23.0155556;
        }
        private void M21ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 63.75;
            Globals.targetDec = 18.0766667;
        }
        private void M22ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 279.100875;
            Globals.targetDec = -23.9033889;
        }
        private void M23ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 32.25;
            Globals.targetDec = 17.9466667;
        }
        private void M24ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Milky Way Star Cloud";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 150;
            Globals.targetDec = 18.2816667;
        }
        private void M25ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 30;
            Globals.targetDec = 18.5266667;
        }
        private void M26ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 75;
            Globals.targetDec = 18.7533333;
        }
        private void M27ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planetary Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 299.901417;
            Globals.targetDec = 22.7211361;
        }
        private void M28ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 276.1370415;
            Globals.targetDec = -24.8698333;
        }
        private void M29ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 60;
            Globals.targetDec = 20.3988889;
        }
        private void M30ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 325.0917915;
            Globals.targetDec = -23.1790556;
        }
        private void M31ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.684584;
            Globals.targetDec = 41.2691667;
        }
        private void M32ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Dwarf Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.6741665;
            Globals.targetDec = 40.8658333;
        }
        private void M33ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 23.4620835;
            Globals.targetDec = 30.66;
        }
        private void M34ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 21;
            Globals.targetDec = 2.7016667;
        }
        private void M35ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 42;
            Globals.targetDec = 6.1516667;
        }
        private void M36ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 61.5;
            Globals.targetDec = 5.6016667;
        }
        private void M37ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 66;
            Globals.targetDec = 5.8733333;
        }
        private void M38ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 82.1749995;
            Globals.targetDec = 35.855;
        }
        private void M39ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 12.375;
            Globals.targetDec = 21.5366667;
        }
        private void M40ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Double Star";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 185.5520835;
            Globals.targetDec = 58.0830556;
        }
        private void M41ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 34.5;
            Globals.targetDec = 6.7666667;
        }
        private void M42ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Emission/ Reflection Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 83.8220835;
            Globals.targetDec = -5.3911111;
        }
        private void M43ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Emission/ Reflection Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 24;
            Globals.targetDec = 5.5933333;
        }
        private void M44ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 8.655;
            Globals.targetDec = 8.6683333;
        }
        private void M45ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 5.7;
            Globals.targetDec = 3.79;
        }
        private void M46ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 81;
            Globals.targetDec = 7.6966667;
        }
        private void M47ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 24;
            Globals.targetDec = 7.61;
        }
        private void M48ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 22.5;
            Globals.targetDec = 8.2283333;
        }
        private void M49ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 187.444584;
            Globals.targetDec = 8.0005556;
        }
        private void M50ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 45;
            Globals.targetDec = 7.0533333;
        }
        private void M51ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 202.469583;
            Globals.targetDec = 47.1952778;
        }
        private void M52ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 75;
            Globals.targetDec = 23.4033333;
        }
        private void M53ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 198.2304165;
            Globals.targetDec = 18.1691667;
        }
        private void M54ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 283.763667;
            Globals.targetDec = -30.4785;
        }
        private void M55ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 294.9975;
            Globals.targetDec = -30.9620833;
        }
        private void M56ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 289.147917;
            Globals.targetDec = 30.1845;
        }
        private void M57ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planetary Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 283.3961625;
            Globals.targetDec = 33.029175;
        }
        private void M58ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 189.43125;
            Globals.targetDec = 11.8180556;
        }
        private void M59ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 190.5095835;
            Globals.targetDec = 11.6469444;
        }
        private void M60ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 190.9150005;
            Globals.targetDec = 11.5525;
        }
        private void M61ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 185.47875;
            Globals.targetDec = 4.4736111;
        }
        private void M62ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 255.3025005;
            Globals.targetDec = -20.1123611;
        }
        private void M63ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 198.955416;
            Globals.targetDec = 42.0291667;
        }
        private void M64ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 194.182083;
            Globals.targetDec = 21.6827778;
        }
        private void M65ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 169.732917;
            Globals.targetDec = 13.0922222;
        }
        private void M66ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 170.0625;
            Globals.targetDec = 12.9916667;
        }
        private void M67ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 132.8500005;
            Globals.targetDec = 11.8119444;
        }
        private void M68ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 189.866709;
            Globals.targetDec = -26.7430278;
        }
        private void M69ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 277.846791;
            Globals.targetDec = -32.3479722;
        }
        private void M70ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 280.802667;
            Globals.targetDec = -32.2918889;
        }
        private void M71ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 298.4421255;
            Globals.targetDec = 18.7784167;
        }
        private void M72ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 313.3662915;
            Globals.targetDec = 12.5370556;
        }
        private void M73ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Asterism";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 30;
            Globals.targetDec = 20.9816667;
        }
        private void M74ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 24.1741665;
            Globals.targetDec = 15.7836111;
        }
        private void M75ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 301.519791;
            Globals.targetDec = -21.9211667;
        }
        private void M76ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planetary Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 51;
            Globals.targetDec = 1.7066667;
        }
        private void M77ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 40.669584;
            Globals.targetDec = -0.0133333;
        }
        private void M78ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Reflection Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 24;
            Globals.targetDec = 5.7783333;
        }
        private void M79ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 81.0441255;
            Globals.targetDec = -24.52425;
        }
        private void M80ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 244.260459;
            Globals.targetDec = -22.9751111;
        }
        private void M81ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 148.8883335;
            Globals.targetDec = 69.0652778;
        }
        private void M82ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Starburst Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 148.9675005;
            Globals.targetDec = 69.6797222;
        }
        private void M83ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 204.2537505;
            Globals.targetDec = -29.8658333;
        }
        private void M84ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Lenticular Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 186.265416;
            Globals.targetDec = 12.8702778;
        }
        private void M85ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Lenticular Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 186.3499995;
            Globals.targetDec = 18.1911111;
        }
        private void M86ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Lenticular Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 186.5487495;
            Globals.targetDec = 12.9461111;
        }
        private void M87ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 187.705833;
            Globals.targetDec = 12.3911111;
        }
        private void M88ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 187.9966665;
            Globals.targetDec = 14.4205556;
        }
        private void M89ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 188.9158335;
            Globals.targetDec = 12.5563889;
        }
        private void M90ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 189.2074995;
            Globals.targetDec = 13.1627778;
        }
        private void M91ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 188.8600005;
            Globals.targetDec = 14.4963889;
        }
        private void M92ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 259.280292;
            Globals.targetDec = 43.1365278;
        }
        private void M93ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 54;
            Globals.targetDec = 7.7433333;
        }
        private void M94ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 192.7212495;
            Globals.targetDec = 41.1205556;
        }
        private void M95ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 160.990416;
            Globals.targetDec = 11.7038889;
        }
        private void M96ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 161.6904165;
            Globals.targetDec = 11.82;
        }
        private void M97ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planetary Nebula";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 39;
            Globals.targetDec = 11.2466667;
        }
        private void M98ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 183.4512495;
            Globals.targetDec = 14.9002778;
        }
        private void M99ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 184.706667;
            Globals.targetDec = 14.4163889;
        }
        private void M100ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 185.7287505;
            Globals.targetDec = 15.8225;
        }
        private void M101ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 210.8025;
            Globals.targetDec = 54.3491667;
        }
        private void M102ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Lenticular Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 226.622916;
            Globals.targetDec = 55.7633333;
        }
        private void M103ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Open Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 127.5;
            Globals.targetDec = 1.5533333;
        }
        private void M104ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 189.9975;
            Globals.targetDec = -11.6230556;
        }
        private void M105ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 161.9566665;
            Globals.targetDec = 12.5816667;
        }
        private void M106ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 184.739583;
            Globals.targetDec = 47.3038889;
        }
        private void M107ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Globular Cluster";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 248.132958;
            Globals.targetDec = -13.0536389;
        }
        private void M108ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 167.879166;
            Globals.targetDec = 53.6741667;
        }
        private void M109ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Barred Spiral Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 179.4;
            Globals.targetDec = 53.3744444;
        }
        private void M110ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Dwarf Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.092084;
            Globals.targetDec = 41.6852778;
        }
            */
        /*private void CustomToolStripMenuItem_Click(object sender, EventArgs e) {
            // simbad.Show();
        }   */

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutBox1 AboutBox1Inst = new AboutBox1();
            AboutBox1Inst.Show();
        }

        private void CustomToolStripMenuItem_Click(object sender, EventArgs e) {
            simbad simbad = new simbad();
            simbad.Show();
        }
        //Solar system

        private void MoonToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Moon";
            Globals.targetname = "Moon";
        }

        private void SunToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Sun";
            Globals.targetname = "Sun";
        }

        private void MercuryToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Mercury";
        }

        private void VenusToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Venus";
        }

        private void MarsToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Mars";
        }

        private void JupiterToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Jupiter";
        }

        private void SaturnToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Saturn";
        }

        private void UranusToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Uranus";
        }

        private void NeptuneToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Planet";
            Globals.targetname = "Neptune";
        }

        private void debugbutton_Click(object sender, EventArgs e) {
            loadmap();


        }




    }
}