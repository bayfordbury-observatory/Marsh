/*    ----------------------------------------------------
 * Marsh Telescope Control Software
*   University of Hertfordshire
*   David Campbell
* 
*   v1.0.0.0 ---------------------  7 March 2014
*   - Initial stable release    
*   
*   v1.0.1.0 ---------------------  7 April 2014
*   - Rewrite to use AstroLib.Net
*   - Autoconnect dome on start
*   - Show daytime allsky image during day
*   - Use new weather server format
*   - Autoclose dome and home at sunrise
*   v1.0.2.0 ---------------------  3 December 2014
*   - Telescope stops tracking at sunrise
*   - Dome won't move unless telescope has been fairly stationary
*  v1.0.3.0 ---------------------  21 March 2016
*   - Added worm and focuser position
*   - More error catching
*  v1.0.4.0 ---------------------  ?
*   - ?
*  v1.0.4.0 ---------------------  21 Jan 2019
*  - Log encoder values
* 
* 
* 
* 
* 
* 
* */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AstroLib;
using System.Text.RegularExpressions;
using System.Threading;


namespace MarshControl {

    public partial class MainForm: Form {

        #region variables
        private SerialPort mySerialPort = new SerialPort();

        int inbyte, inbyte2, timeout = 0;
        double T, t;
        public int count = 0, haenc, decenc;
        public bool dss = false;
        public double cd, ct, ci, cc, ce, cgamma, ctheta, cl, cr;
        public double moonl, moonb, bhr, lhr, posangle;
        public int wormindex = 0;
        double lst;

        int trackTimeout = 0;

        EclipticalCoords Earth;
        EclipticalCoords Sun;

        NutObl nutobl;

        EquatorialCoords SunEq;
        CoordsString SunEqS;

        public bool SerialInUse = false;

        int trackingin;
        double focuspos;

        string dateformat = "dd MMMM yyyy", hoursformat = "HH", minsformat = "mm", secsformat = "ss";

        #endregion

        public PictureBox MainPictureBox {
            get {
                return PictureBox1;
            }
        }

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
            if(!mySerialPort.IsOpen) {


                var _with1 = mySerialPort;
                _with1.PortName = "COM" + Convert.ToString(Properties.Settings.Default.com);
                _with1.BaudRate = 115200;
                _with1.DataBits = 8;
                _with1.Parity = Parity.None;
                _with1.StopBits = StopBits.One;
                _with1.Handshake = Handshake.None;
                _with1.WriteTimeout = 50;
                _with1.ReadTimeout = 50;

                try {
                    mySerialPort.Open();
                    Timer1.Enabled = true;
                    CommsLostLbl.Text = "";
                } catch(Exception ex) {
                    //Serial port doesn't exist or can't be opened
                    MessageBox.Show(ex.Message);
                    Timer1.Enabled = false;
                }
            }
        }

        public void LoadLoadingScreen() {
            Application.Run(new LoadScreen());
        }

        //void Main(string[] args) {
        //################################################################# Main
        public MainForm(bool start) {

            Globals.Status = "Initialising main window";
            Globals.LoadProgress = 70;

            InitializeComponent();

            try {

                if(!Globals.testing) {

                    Screen[] sc;
                    sc = Screen.AllScreens;
                    this.WindowState = FormWindowState.Normal;
                    this.Left = sc[0].Bounds.Left;
                    this.Top = sc[0].Bounds.Top;
                    this.StartPosition = FormStartPosition.Manual;
                    this.WindowState = FormWindowState.Maximized;

                }

                Globals.Status = "Connecting to telescope";
                Globals.LoadProgress = 80;

                if(start && !Globals.testing) {
                    CommPortSetup();
                    Globals.Status = "Loading settings";
                    Globals.LoadProgress = 90;
                    load_settings();
                }

                Globals.LoadProgress = 100;
                Globals.Status = "Done!";

                if(Globals.testing) {
                    Timer1.Enabled = true;
                }

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
            // ThreadT.Abort();
        }

        //#################################################################

        public int[,] maparray = new int[721, 361];

        public void loadmap() {

            try {
                //Load bitmap for the moon map
                Bitmap map = new Bitmap("images/moon.bmp");

                for(int i = 0; i <= 719; i++) {
                    for(int j = 0; j <= 359; j++) {
                        maparray[i, j] = map.GetPixel(i, j).R;
                    }
                }
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void moonmap() {

            //Draw the moon map

            int count = 0;
            //Label32.Text = CStr(lhr) + " " + CStr(bhr) + " " + CStr(L) + " " + CStr(b) + " " + CStr(posangle)

            int moonwidth = 300;
            Bitmap image = new Bitmap(moonwidth, moonwidth);

            double liblatd = moonb;
            double liblat = -liblatd.ToRad();
            double liblongd = moonl;
            double liblong = liblongd.ToRad();
            double sinx = Math.Sin(posangle.ToRad());
            double cosx = Math.Cos(posangle.ToRad());
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

            lhr = lhr.ToRad();
            bhr = bhr.ToRad();

            // Create a Bitmap object from a file. 
            if(count == 0) {
                loadmap();
            }

            for(int i = 0; i < moonwidth; i++) {

                for(int j = 0; j < moonwidth; j++) {

                    x2 = ((double)i + 1 - ((double)moonwidth / 2)) / ((double)moonwidth / 2);
                    y2 = ((double)j + 1 - ((double)moonwidth / 2)) / ((double)moonwidth / 2);

                    x = (x2 * cosx) - (y2 * sinx);
                    y = (x2 * sinx) + (y2 * cosx);

                    rho = Math.Sqrt((x * x) + (y * y));
                    //debug.AppendText(i.ToString() + " " + j.ToString() + " "+x2.ToString() + " " + y2.ToString() + " " + x.ToString() + " " + y.ToString() + " " + rho.ToString() + Environment.NewLine);

                    if(rho < 1) {
                        if(rho == 0) {
                            theta = liblong.ToDeg();
                            phi = liblat.ToDeg();

                        } else {
                            c = Math.Asin(rho);
                            theta = (liblong + Math.Atan2(x * rho, (rho * Math.Cos(liblat) * Math.Cos(c)) - (y * Math.Sin(liblat) * rho))).ToDeg();
                            phi = (Math.Asin(Math.Cos(c) * Math.Sin(liblat) + ((y * rho * Math.Cos(liblat)) / rho))).ToDeg();
                        }


                        if((theta >= 180)) {
                            theta = theta - 360;
                        } else if((theta < -180)) {
                            theta = theta + 360;
                        }

                        theta3 = Math.Round(2 * theta) / 2;

                        if((phi < -90)) {
                            phi = phi + 360;
                        } else if((phi > 90)) {
                            phi = phi - 360;
                        }

                        delta = (Math.Acos(Math.Sin(phi.ToRad()) * Math.Sin(bhr) + Math.Cos(phi.ToRad()) * Math.Cos(bhr) * Math.Cos(lhr - theta.ToRad()))).ToDeg();

                        if(double.IsNaN(c) | double.IsInfinity(c)) {

                        } else {

                            tone = Convert.ToDouble(maparray[Convert.ToInt32(Math.Floor((theta + 180) * 2)), Convert.ToInt32(Math.Floor((phi + 90) * 2))]);
                            //debug.AppendText(theta.ToString() + " " + phi.ToString() + Environment.NewLine);
                            if((delta > 93)) {
                                tone = 0;
                            } else if((delta > 81)) {
                                tone = Math.Round(tone * (1 - ((delta - 81) / 12)));
                            }
                            image.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(tone), Convert.ToInt32(tone), Convert.ToInt32(tone)));
                        }
                    } else {
                        image.SetPixel(i, j, Color.Black);
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

            double inbyted, inbyted2;

            double JD = Utils.jd();

            try {
                if(Globals.reloadSettings) {
                    load_settings();
                    Globals.reloadSettings = false;
                }

                //First thing, Work out the time
                DateTime time = DateTime.UtcNow;

                timelbl.Text = "UTC: " + time.ToString(hoursformat) + "h " + time.ToString(minsformat) + "m " + time.ToString(secsformat) + "s";
                datelbl.Text = "Date: " + time.ToString(dateformat);


                T = Utils.JDtoT(JD);
                t = Utils.JDtot(JD);

                //Local sidereal time
                lst = Utils.lst(JD, T, Globals.loc);

                //Update times on display

                jdelbl.Text = "Julian date: " + JD.ToString("F5");

                double lstdeg = Utils.quad(lst.ToDeg());

                lstlbl.Text = "LST: " + Utils.hh(lstdeg / 15) + "h " + Utils.dm(lstdeg / 15) + "m " + Utils.ds(lstdeg / 15) + "s";

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
            //Check serial port exists   
            if(mySerialPort.IsOpen || Globals.testing) {

                //Read the encoder positions and convert to HA/DEC
                if(timeout < (5000 / Timer1.Interval)) {
                    try {

                        if(!Globals.testing) {

                            //Request HA encoder values

                            while(SerialInUse) { }


                            mySerialPort.DiscardInBuffer();
                            SerialInUse = true;
                            mySerialPort.Write("#H");
                            SerialInUse = false;

                            inbyte = mySerialPort.ReadByte();
                            inbyte2 = mySerialPort.ReadByte();

                            //check for correct return characters
                            if(((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "H") {

                                inbyte = mySerialPort.ReadByte();  //MSB   
                                inbyte2 = mySerialPort.ReadByte(); //LSB

                                inbyted = Convert.ToDouble(inbyte);
                                inbyted2 = Convert.ToDouble(inbyte2);
                                haenc = (((inbyte) * 256) + (inbyte2));

                                Globals.HAdeg = 360 - (((inbyted * 256) + inbyted2) * (360.0 / 16384.0));
                            }

                            //Request DEC encoder values
                            mySerialPort.DiscardInBuffer();
                            SerialInUse = true;
                            mySerialPort.Write("#D");
                            SerialInUse = false;

                            inbyte = mySerialPort.ReadByte();
                            inbyte2 = mySerialPort.ReadByte();

                            //check for correct return characters
                            if(((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "D") {

                                inbyte = mySerialPort.ReadByte();
                                //MSB
                                inbyte2 = mySerialPort.ReadByte();
                                //LSB
                                inbyted = Convert.ToDouble(inbyte);
                                inbyted2 = Convert.ToDouble(inbyte2);
                                decenc = (((inbyte) * 256) + (inbyte2));

                                Globals.DECdeg = ((inbyted * 256) + inbyted2) * (360.0 / 16384.0) + cd;

                                if(Globals.DECdeg > 180) {
                                    Globals.DECdeg = Globals.DECdeg - 360;
                                }


                                Globals.HAdeg = Utils.quad(Globals.HAdeg + ct);

                                Globals.Scope.dec = Globals.DECdeg.ToRad();

                                Globals.HA = Globals.HAdeg.ToRad();

                                //Analytical pointing model corrections
                                Globals.HAdeg = Globals.HAdeg - cgamma * Math.Sin(Globals.HA - (ctheta).ToRad()) * Math.Tan(Globals.Scope.dec) + cc * Math2.Sec(Globals.Scope.dec)
                                    - ci * Math.Tan(Globals.Scope.dec) + ce * Math.Cos(Globals.loc.lat) * Math2.Sec(Globals.Scope.dec) * Math.Sin(Globals.HA)
                                    + cl * (Math.Sin(Globals.loc.lat) * Math.Tan(Globals.Scope.dec) + Math.Cos(Globals.Scope.dec) * Math.Cos(Globals.HA))
                                    + cr * Globals.HAdeg;

                                Globals.DECdeg = Globals.DECdeg - cgamma * Math.Cos(Globals.HA - (ctheta).ToRad()) - ce * (Math.Sin(Globals.loc.lat) * Math.Cos(Globals.Scope.dec)
                                    - Math.Cos(Globals.loc.lat) * Math.Sin(Globals.Scope.dec) * Math.Cos(Globals.HA));

                                Globals.HA = Globals.HAdeg.ToRad();
                                Globals.Scope.dec = Globals.DECdeg.ToRad();


                                enclbl.Text = "Encoders: " + haenc.ToString("00000.##") + " " + decenc.ToString("00000.##");
                                encodersBin.Text = Convert.ToString(haenc, 2) + " " + Convert.ToString(decenc, 2);

                                Globals.Scope.ra = lst - Globals.HA;

                                Globals.ScopeHor = Globals.Scope.ToHorizontal(Globals.loc, T);

                                if(RetryBtn.Visible) { RetryBtn.Visible = false; CommsLostLbl.Visible = false; }

                            }

                            //track rate

                            SerialInUse = true;
                            mySerialPort.DiscardInBuffer();
                            mySerialPort.Write("#S");
                            SerialInUse = false;

                            inbyte = mySerialPort.ReadByte();
                            inbyte2 = mySerialPort.ReadByte();

                            //check for correct return characters
                            if(((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "S") {

                                trackingin = mySerialPort.ReadByte();

                                if(trackingin == 1) {

                                    trackingLbl.Text = "Tracking: Sidereal";
                                    TrackingChk.Checked = true;
                                } else {
                                    trackingLbl.Text = "Tracking: Off";
                                    TrackingChk.Checked = false;
                                }

                            }




                            //focus pos
                            SerialInUse = true;
                            mySerialPort.DiscardInBuffer();
                            mySerialPort.Write("#F");
                            SerialInUse = false;

                            inbyte = mySerialPort.ReadByte();
                            inbyte2 = mySerialPort.ReadByte();

                            //check for correct return characters
                            if(((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "F") {

                                inbyte = mySerialPort.ReadByte();  //MSB   
                                inbyte2 = mySerialPort.ReadByte(); //LSB

                                inbyted = Convert.ToDouble(inbyte);
                                inbyted2 = Convert.ToDouble(inbyte2);

                                focuspos = (inbyted * 256) + inbyted2;
                            }

                            //worm pos
                            mySerialPort.DiscardInBuffer();
                            SerialInUse = true;
                            mySerialPort.Write("#W");
                            SerialInUse = false;

                            inbyte = mySerialPort.ReadByte();
                            inbyte2 = mySerialPort.ReadByte();

                            //check for correct return characters
                            if(((Char)inbyte).ToString() == "$" & ((Char)inbyte2).ToString() == "W") {

                                wormindex = mySerialPort.ReadByte();

                            }
                            SerialInUse = false;

                            focusLbl.Text = "Focus position: " + focuspos.ToString();
                            wormLbl.Text = "Worm index: " + wormindex.ToString();
                            // debugBox.Text = wormindex.ToString() + " " + trackingin.ToString() + " " + focuspos.ToString();

                        }

                    } catch(Exception ex1) {
                        timeout = timeout + 1;
                        Timer1.Interval = 200;

                        CommsLostLbl.Visible = true;
                        RetryBtn.Visible = false;
                    }
                } else {
                    if(timeout < 10000) {
                        //uh oh
                        timeout = 60001;
                        MessageBox.Show("No response from telescope for 5 seconds");
                        CommsLostLbl.Text = "Communication Lost";
                        CommsLostLbl.Visible = true;
                        RetryBtn.Visible = true;
                    }
                }

                try {

                    //rounded to nearest 79"

                    EquatorialCoords rounded = new EquatorialCoords((Math.Round(Globals.Scope.ra.ToDeg() * 45.51111) / 45.51111).ToRad(), (Math.Round(Globals.Scope.dec.ToDeg() * 45.51111) / 45.51111).ToRad());
                    CoordsString roundedS = rounded.ToDMS();

                    if(timeout < 60000) {
                        RAlbl.Text = " RA:  " + roundedS.ra;
                        DEClbl.Text = "DEC: " + roundedS.dec;

                        halbl.Text = "Hour Angle: " + Utils.hh(Globals.HAdeg / 15) + "h " + Utils.dm(Globals.HAdeg / 15) + "m " + Utils.ds(Globals.HAdeg / 15) + "s";

                        azlbl.Text = "Azimuth: " + String.Format("{0:0.00}", Globals.ScopeHor.az.ToDeg()) + (Char)176;
                        altlbl.Text = "Altitude: " + String.Format("{0:0.00}", Globals.ScopeHor.alt.ToDeg()) + (Char)176;

                        if(Globals.ScopeHor.alt < 0 && TrackingChk.Checked) {
                            // 
                            
                            if(trackTimeout > 60) {
                                turnTrackingOff();
                            } else {
                                trackTimeout++;
                            }
                        } else {
                            trackTimeout = 0;
                        }


                    } else {
                        RAlbl.Text = " RA:  --h --m --s";

                        DEClbl.Text = "DEC: ---" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;

                        halbl.Text = "Hour Angle: --h --m --s";
                        enclbl.Text = "Encoders: ----- -----";

                        azlbl.Text = "Azimuth: ---" + (Char)176;
                        altlbl.Text = "Altitude: --" + (Char)176;
                    }


                    TargetNameLbl.Text = "Name: " + Globals.targetname;
                    if(Globals.targettype.Length > 4) {
                        if(Globals.targettype.Substring(0, 4) == "DSS_") {
                            TargetTypeLbl.Text = "Type: " + Globals.targettype.Substring(4);
                        } else {
                            TargetTypeLbl.Text = "Type: " + Globals.targettype;
                        }
                    } else {
                        TargetTypeLbl.Text = "Type: " + Globals.targettype;
                    }

                    //Calculate various solar system parameters related to the sun, ecliptic..etc

                    Earth = SolarSystem.Planet("Earth", t);
                    Sun = SolarSystem.Sun(Earth);

                    nutobl = new NutObl(T);

                    SunEq = Sun.EclipticalToEquatorial(nutobl);
                    SunEqS = SunEq.ToDMS();
                    Globals.SunHor = SunEq.ToHorizontal(Globals.loc, T);


                    /*
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
                 
                      */

                    // debug.Text = Globals.targettype + " " + Globals.targetname;

                    if(Globals.targettype == "Planet" | Globals.targettype == "Moon" | Globals.targettype == "Sun") {
                        //>Solar system

                        PictureBox1.Visible = true;
                        TargetJ2000RALbl.Text = " RA J2000:  --h --m --s";
                        TargetJ2000DecLbl.Text = "DEC J2000: ---" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;

                        if(Globals.targetname == "Sun") {
                            PictureBox1.Image = Image.FromFile("images/sun.jpg");
                            Label54.Text = "";
                            Label45.Text = "";
                            Label46.Text = "";
                            Label47.Text = "";

                            Globals.TargetNow = SunEq;


                        } else if(Globals.targetname == "Moon") {
                            //Condensed ELP2000 moon theory coordinates

                            EclipticalCoords Moon = SolarSystem.MoonLow(T);
                            Moon.l += nutobl.deltapsi;

                            EquatorialCoords MoonEq = Moon.EclipticalToEquatorial(nutobl);







                            //Parallax correction

                            double deltaau = Moon.r / 149597870.691;

                            double H = 70;

                            double sinpi = (Math.Sin((8.794 / 3600).ToRad())) / deltaau;

                            double ruserlat = Globals.loc.lat;
                            //double ruserlong = -location.lon;


                            double C = 1 - (1 / 298.257);
                            double u = Math.Atan(C * Math.Tan(ruserlat));
                            double psin = C * Math.Sin(u) + (H / 6378140) * Math.Sin(ruserlat);
                            double pcos = Math.Cos(u) + (H / 6378140) * Math.Cos(ruserlat);

                            H = lst - MoonEq.ra;
                            //hour angle

                            //double moonH = H;
                            double mA = (Math.Cos(MoonEq.dec)) * (Math.Sin(H));
                            double mB = (Math.Cos(MoonEq.dec) * Math.Cos(H)) - (pcos * sinpi);
                            double mC = (Math.Sin(MoonEq.dec)) - (psin * sinpi);
                            double Q = Math.Sqrt((mA * mA) + (mB * mB) + (mC * mC));


                            MoonEq.ra = lst - Math.Atan2(mA, mB);

                            MoonEq.dec = Math.Asin(mC / Q);

                            HorizontalCoords MoonHor = MoonEq.ToHorizontal(Globals.loc, T);

                            Globals.TargetNow = MoonEq;


                            /*
                            //----------------------------
                            double lambdaH = Slon + 180 + (deltaau * 57.296 * ((Math.Cos(Moon.b)) * (Math.Cos(deg2rad(Slon) - Moon.l))));
                            double betaH = deg2rad(rad2deg(Moon.b) * deltaau);
                            //----------------------------
                            double I = deg2rad(1.5424166666);
                            double WH = deg2rad(quad(lambdaH - deltapsi - omega));
                            double AH = quad(rad2deg(Math.Atan2((Math.Sin(WH) * Math.Cos(betaH) * Math.Cos(I)) - (Math.Sin(betaH) * Math.Sin(I)), Math.Cos(WH) * Math.Cos(betaH))));
                            double lH = quad(AH - F);

                            if (lH > 90) {
                                lH = lH - 360;
                            }
                            double bH = Math.Asin(((-Math.Sin(WH)) * Math.Cos(betaH) * Math.Sin(I)) - (Math.Sin(betaH) * Math.Cos(I)));

                            //double psii = Math.Acos((Math.Cos(beta)) * (Math.Cos(lambda - deg2rad(Slon))));
                            //double ii = rad2deg(Math.Atan2(sR * Math.Sin(psii), deltaau - (sR * Math.Cos(psii))));
                            //double k = (1 + (Math.Cos(deg2rad((ii))))) / 0.02;

                             * 
                             * /
                             * 
                             */


                            double cosPhi = Math.Sin(SunEq.dec) * Math.Sin(MoonEq.dec) + Math.Cos(SunEq.dec) * Math.Cos(MoonEq.dec) * Math.Cos(SunEq.ra - MoonEq.ra);

                            double ii = Math.Acos(-cosPhi).ToDeg();

                            double k = (1 - cosPhi) / 0.02;



                            /*
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
                            double alpha = deg2rad(quad(Globals.targetRAnow));

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
                         
                          
                          */


                            if(count == 0) {
                                moonmap();
                            }
                            count = count + 1;
                            if(count > 5) {
                                count = 0;
                            }
                            //RichTextBox1.Text = "ldd: " + CStr(Ldd) + Environment.NewLine + "bdd: " + CStr(bdd) + Environment.NewLine + "ld: " + CStr(Ld) + Environment.NewLine + "bd: " + CStr(bd) + Environment.NewLine + "deltal: " + CStr(deltal) + Environment.NewLine + "deltab: " + CStr(deltab)

                            // double k = 1;

                            Label54.Text = "Moon phase: " + String.Format("{0:0.0}", k) + (Char)37;
                            Label45.Text = "Lib. Longitude: " + String.Format("{0:0.000}", moonl) + (Char)176;
                            Label46.Text = "Lib. Latitude: " + String.Format("{0:0.000}", moonb) + (Char)176;
                            Label47.Text = "Position angle: " + String.Format("{0:0.00}", posangle) + (Char)176;

                        } else if(Globals.targettype == "Planet") {
                            Label54.Text = "";
                            Label45.Text = "";
                            Label46.Text = "";
                            Label47.Text = "";

                            //Planet and Earth terms for orbital location

                            //pt pterms = new pt();

                            EclipticalCoords planetEcl = new EclipticalCoords();

                            if(Globals.targetname == "Mercury") {

                                PictureBox1.Image = null;
                                planetEcl = SolarSystem.Planet("Mercury", t);

                            } else if(Globals.targetname == "Venus") {

                                PictureBox1.Image = Image.FromFile("images/venus.jpg");
                                planetEcl = SolarSystem.Planet("Venus", t);

                            } else if(Globals.targetname == "Mars") {

                                PictureBox1.Image = Image.FromFile("images/mars.jpg");
                                planetEcl = SolarSystem.Planet("Mars", t);

                            } else if(Globals.targetname == "Jupiter") {

                                PictureBox1.Image = Image.FromFile("images/jupiter.jpg");
                                planetEcl = SolarSystem.Planet("Jupiter", t);

                            } else if(Globals.targetname == "Saturn") {

                                PictureBox1.Image = Image.FromFile("images/saturn.jpg");
                                planetEcl = SolarSystem.Planet("Saturn", t);

                            } else if(Globals.targetname == "Uranus") {

                                PictureBox1.Image = null;
                                planetEcl = SolarSystem.Planet("Uranus", t);

                            } else if(Globals.targetname == "Neptune") {

                                PictureBox1.Image = null;
                                planetEcl = SolarSystem.Planet("Neptune", t);

                            } else {
                                PictureBox1.Image = null;
                            }

                            Globals.TargetNow = planetEcl.HelToEquatorial(Earth, nutobl);

                            debugBox.Text = "lbr " + (Utils.rad2deg(planetEcl.l)).ToString() + " " + (Utils.rad2deg(planetEcl.b)).ToString() + " " + (Utils.rad2deg(planetEcl.r)).ToString();

                        }
                    } else {
                        if(Globals.targettype.Length > 4) {
                            if(!(Globals.targettype.Substring(0, 4) == "DSS_")) {
                                //PictureBox1.Image = null;
                                PictureBox1.Visible = true;
                            } else {
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

                        CoordsString TargetJ2000S = Globals.Target.ToDMS();

                        TargetJ2000RALbl.Text = " RA J2000:  " + TargetJ2000S.ra;
                        TargetJ2000DecLbl.Text = "DEC J2000: " + TargetJ2000S.dec;

                        //Correct target coords for precession, nutation, aberration----------------------


                        //precession

                        Globals.TargetNow = Utils.J2000ToNow(Globals.Target, JD);

                        //double targetRAprecessed = precessra(Globals.targetRA, Globals.targetDec, T);
                        //double targetDecprecessed = precessdec(Globals.targetRA, Globals.targetDec, T);

                        //nutation
                        Globals.TargetNow.Nutation(nutobl);

                        // double nutationra = deltapsi * (Math.Cos(epsilonr) + Math.Sin(epsilonr) * Math.Sin(deg2rad(targetRAprecessed)) * Math.Tan(deg2rad(targetDecprecessed))) - deltaepsilon * Math.Cos(deg2rad(targetRAprecessed)) * Math.Tan(deg2rad(targetDecprecessed));
                        //double nutationdec = deltapsi * Math.Sin(epsilonr) * Math.Cos(deg2rad(targetRAprecessed)) + deltaepsilon * Math.Sin(deg2rad(targetRAprecessed));

                        //aberration
                        Globals.TargetNow.Aberation(Sun, nutobl, T);
                        /*
                    double k = 20.49552 / 3600;
                    double ee = 0.016708634 - 4.2037E-05 * T - 1.267E-07 * T * T;
                    double pir = deg2rad(102.93735 + 1.71946 * T + 0.00046 * T * T);

                    double aberrationra = ((-k / Math.Cos(deg2rad(targetDecprecessed))) * (Math.Cos(deg2rad(targetRAprecessed)) * Math.Cos(Slonr) * Math.Cos(epsilonr) + Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(Slonr))) + (((ee * k) / Math.Cos(deg2rad(targetDecprecessed))) * (Math.Cos(deg2rad(targetRAprecessed)) * Math.Cos(pir) * Math.Cos(epsilonr) + Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(pir)));
                    double aberrationdec = (ee * k * (Math.Cos(pir) * Math.Cos(epsilonr) * (Math.Tan(epsilonr) * Math.Cos(deg2rad(targetDecprecessed)) - Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed))) + Math.Cos(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed)) * Math.Sin(pir))) - (k * (Math.Cos(Slonr) * Math.Cos(epsilonr) * (Math.Tan(epsilonr) * Math.Cos(deg2rad(targetDecprecessed)) - Math.Sin(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed))) + Math.Cos(deg2rad(targetRAprecessed)) * Math.Sin(deg2rad(targetDecprecessed)) * Math.Sin(Slonr)));
                       */
                        //Globals.targetRAnow = targetRAprecessed + nutationra + aberrationra;
                        //Globals.targetDecnow = targetDecprecessed + nutationdec + aberrationdec;



                    }

                    HorizontalCoords TargetHor = Globals.TargetNow.ToHorizontal(Globals.loc, T);



                    if(TargetHor.alt.ToDeg() > -0.5) {
                        //atmospheric refraction
                        double Rh = ((1.02 / Math.Tan((TargetHor.alt.ToDeg() + (10.3 / (TargetHor.alt.ToDeg() + 5.11))).ToRad())) + 0.0019279) / 60;

                        TargetHor.alt += Rh.ToRad();

                        //double targetaltr = deg2rad(targetalt);
                        //double targetazr = deg2rad(targetaz - 180);

                        // targetHAr = Math.Atan2(Math.Sin(targetazr), (Math.Cos(targetazr) * Math.Sin(latrad) + Math.Tan(targetaltr) * Math.Cos(latrad)));
                        //targetDecr = Math.Asin(Math.Sin(latrad) * Math.Sin(targetaltr) - Math.Cos(latrad) * Math.Cos(targetaltr) * Math.Cos(targetazr));
                        //targetRAr = deg2rad(lst) - targetHAr;

                        Globals.TargetNow = TargetHor.ToEquatorial(Globals.loc, T);

                        //Globals.targetRAnow = quad(rad2deg(targetRAr))
                        //Globals.targetDecnow = rad2deg(targetDecr)
                    }


                    // double targetRArnow = deg2rad(Globals.targetRAnow);
                    //double targetDecrnow = deg2rad(Globals.targetDecnow);

                    CoordsString TargetNowS = Globals.TargetNow.ToDMS();

                    //Show JNow coords
                    TargetRALbl.Text = " RA:  " + TargetNowS.ra;
                    TargetDecLbl.Text = "DEC: " + TargetNowS.dec;
                    TargetAltLbl.Text = "Altitude: " + String.Format("{0:0.00}", TargetHor.alt.ToDeg()) + (Char)176;
                    TargetAzLbl.Text = "Azimuth: " + String.Format("{0:0.00}", TargetHor.az.ToDeg()) + (Char)176;

                    //Calculate angular separation between target coords and current telescope position
                    double sep = (Math.Acos(Math.Sin(Globals.Scope.dec) * Math.Sin(Globals.TargetNow.dec) + Math.Cos(Globals.Scope.dec) * Math.Cos(Globals.TargetNow.dec) * Math.Cos(Globals.Scope.ra - Globals.TargetNow.ra))).ToDeg();

                    if(sep < 0.5) {
                        distancelbl.ForeColor = Color.LimeGreen;
                    } else {
                        distancelbl.ForeColor = Color.Red;
                    }

                    //Position angle
                    double positionangle = Utils.quad((Math.Atan2(Math.Sin(Globals.TargetNow.ra - Globals.Scope.ra), Math.Cos(Globals.Scope.dec) * Math.Tan(Globals.TargetNow.dec) - Math.Sin(Globals.Scope.dec) * Math.Cos(Globals.TargetNow.ra - Globals.Scope.ra))).ToDeg());

                    //Distances in RA and DEC
                    double radist = (Math.Acos((Math.Cos(Globals.Scope.ra - Globals.TargetNow.ra)))).ToDeg();
                    double decdist = (Globals.TargetNow.dec - Globals.Scope.dec).ToDeg();

                    //RichTextBox1.Text = CStr(Globals.targetDec) + " " + CStr(Globals.DEC) + " " + CStr(decdist) + Environment.NewLine

                    if(radist < 0.1) {
                        radistlbl.ForeColor = Color.LimeGreen;
                        TargetRALbl.ForeColor = Color.LimeGreen;
                    } else {
                        TargetRALbl.ForeColor = Color.Red;
                        radistlbl.ForeColor = Color.Red;
                    }

                    if(Math.Abs(decdist) < 0.1) {
                        decdistlbl.ForeColor = Color.LimeGreen;
                        TargetDecLbl.ForeColor = Color.LimeGreen;
                    } else {
                        TargetDecLbl.ForeColor = Color.Red;
                        decdistlbl.ForeColor = Color.Red;
                    }
                    if(timeout < 60000) {
                        distancelbl.Text = "Distance to target: " + String.Format("{0:0.00}", sep) + (Char)176;
                        posanglelbl.Text = "Position angle: " + String.Format("{0:0.0}", positionangle) + (Char)176;
                        radistlbl.Text = "RA difference: " + Utils.dd1(radist) + (Char)176 + " " + Utils.dm(radist) + (Char)39 + " " + Utils.ds(radist) + (Char)34;
                        decdistlbl.Text = "DEC difference: " + Utils.dd1(decdist) + (Char)176 + " " + Utils.dm(decdist) + (Char)39 + " " + Utils.ds(decdist) + (Char)34;
                    } else {
                        distancelbl.Text = "Distance to target: --.--" + (Char)176;
                        posanglelbl.Text = "Position angle: --" + (Char)176;
                        radistlbl.Text = "RA difference: --" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;
                        decdistlbl.Text = "DEC difference: --" + (Char)176 + " --" + (Char)39 + " --" + (Char)34;
                        distancelbl.ForeColor = Color.Red;
                        radistlbl.ForeColor = Color.Red;
                        decdistlbl.ForeColor = Color.Red;
                        TargetJ2000DecLbl.ForeColor = Color.Red;
                        TargetDecLbl.ForeColor = Color.Red;
                    }


                } catch(Exception ex) {
                    MessageBox.Show(ex.Message);
                }

            } else {
                //no serial port, abandon ship
                CommsLostLbl.Text = "Port COM" + Properties.Settings.Default.com + " not found";
                CommsLostLbl.Visible = true;
                Timer1.Enabled = false;
            }



        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e) {

            try {

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

            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void Retry_Click(object sender, EventArgs e) {
            timeout = 0;
        }

        private void starClick(object sender, EventArgs e) {


            Globals.targetname = sender.ToString();
            Globals.targettype = "Star";

            ToolStripMenuItem mnu = (ToolStripMenuItem)sender;

            Regex rgx = new Regex("[^0-9]");
            int item = Convert.ToInt16(rgx.Replace(mnu.Name, ""));

            debugBox.Text = item.ToString();

            Globals.targetRA = starsRA[item].ToRad();
            Globals.targetDec = starsDec[item].ToRad();



        }



        #region Stars


        public double[] starsRA = new double[58] { 104.656452, 68.980161, 319.644881, 154.993144, 47.042215, 99.427921, 193.507289, 30.974804, 84.053389, 85.189696, 141.896847, 233.67195, 2.096911, 297.695829, 111.023761, 247.35192, 213.9153, 81.282763, 206.885157, 88.792939, 79.172329, 2.294521, 113.649428, 310.357978, 10.897379, 177.264907, 240.083359, 165.931953, 81.572972, 269.151541, 326.046492, 344.412694, 311.552845, 31.793363, 222.67636, 346.190224, 165.46032, 17.433015, 51.08071, 95.674939, 200.981429, 283.816357, 178.457698, 37.954515, 116.32896, 114.825493, 263.733627, 152.092961, 78.634468, 305.557091, 86.93912, 345.943573, 10.126835, 101.287155, 201.298247, 14.177216, 279.234735, 107.097851 };
        public double[] starsDec = new double[58] { -28.972084, 16.509301, 62.585573, 19.841489, 40.955648, 16.399252, 55.959821, 42.329725, -1.20192, -1.942572, -8.658603, 26.714693, 29.090432, 8.868322, -29.303104, -26.432002, 19.18241, 6.349702, 49.313265, 7.407063, 45.997991, 59.14978, 31.888276, 45.280338, -17.986605, 14.57206, -22.62171, 61.751033, 28.60745, 51.488895, 9.875011, -29.622236, 33.970256, 23.462423, 74.155505, 15.205264, 56.382427, 35.620558, 49.86118, -17.955918, 54.925362, -26.296722, 53.69476, 89.26411, 28.026199, 5.224993, 12.560035, 11.967207, -8.201641, 40.256679, -9.669605, 28.082789, 56.537331, -16.716116, -11.161322, 60.71674, 38.783692, -26.3932 };


        #endregion

        #region messier

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

        private void timer2_Tick(object sender, EventArgs e) {

            DateTime now = DateTime.UtcNow;
            string lineout = now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + " " + Convert.ToString(haenc, 2) + " " + Convert.ToString(decenc, 2) + " " + haenc.ToString("00000.##") + " " + decenc.ToString("00000.##") + " " +
            Globals.HAdeg.ToString("F5") + " " +
                Globals.DECdeg.ToString("F5") + " " +
                String.Format("{0:0.00}", Globals.ScopeHor.az.ToDeg()) + " " +
                String.Format("{0:0.00}", Globals.ScopeHor.alt.ToDeg())+" "+ trackingin.ToString();



            string date = now.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);


            File.AppendAllText(@"C:\Users\Astro\Documents\logs\encoder_" + date + ".txt", lineout + Environment.NewLine);
        }

        private void azlbl_Click(object sender, EventArgs e) {

        }

        private void M110ToolStripMenuItem_Click(object sender, EventArgs e) {
            Globals.targettype = "Dwarf Elliptical Galaxy";
            Globals.targetname = sender.ToString();
            Globals.targetRA = 10.092084;
            Globals.targetDec = 41.6852778;
        }
        #endregion

        #region Solar system

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

        #endregion

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

        private void debugbutton_Click(object sender, EventArgs e) {

        }

        private void trackingChk_CheckedChanged(object sender, EventArgs e) {

            if(TrackingChk.Checked) {
                //turn on tracking                
                turnTrackingOn();
            } else {
                //turn off tracking
                turnTrackingOff();
            }
        }

        private void LightBox_SelectedIndexChanged(object sender, EventArgs e) {

            while(SerialInUse) { }
            SerialInUse = true;
            byte[] outbyte = new byte[1];
            if(LightBox.SelectedIndex == 8) {
                outbyte[0] = 255;
            } else {
                outbyte[0] = (byte)(LightBox.SelectedIndex * 32);
            }

            mySerialPort.Write("#l");
            mySerialPort.Write(outbyte, 0, 1);
            debugBox.Text = outbyte[0].ToString();
            SerialInUse = false;
        }

        private void turnTrackingOff() {
            //turn telescope off tracking

            while(SerialInUse) { }
            SerialInUse = true;
            mySerialPort.Write("#s@");
            SerialInUse = false;

        }

        private void turnTrackingOn() {
            //turn telescope off tracking

            while(SerialInUse) { }
            SerialInUse = true;
            mySerialPort.Write("#sA");
            SerialInUse = false;

        }

        private void pollSunScope_Tick(object sender, EventArgs e) {

            if(Globals.SunHor.alt > 0) {
                if(!Globals.SunUp) {
                    //Sun has just come up
                    turnTrackingOff();
                }
                Globals.SunUp = true;
            } else {
                Globals.SunUp = false;
            }
        }


    }
}