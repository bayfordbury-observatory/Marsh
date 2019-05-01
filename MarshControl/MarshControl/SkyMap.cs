using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShadowEngine;
using ShadowEngine.OpenGL;
using Tao.OpenGl;
using System.Runtime.InteropServices;
using System.Threading;

namespace MarshControl {
    public partial class SkyMap : Form {
        //System.Threading.Timer Timer;

        uint hdc;
        double thetaC = Math.PI, phiC = 0;
        double[,] s1, s3, s6, clines;
        int s1l, s3l, s6l, clinesl;

        public List<Position> stars = new List<Position>();

        public void Draw() {

            double theta, phi;
            double col, red, green, blue;

            Gl.glEnable(Gl.GL_MULTISAMPLE);
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glColor3d(0.2, 0.2, 0.2);

            for (int i = 0; i < 361; i = i + 15) {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (int j = -80; j < 81; j++) {
                    theta = i * Math.PI / 180;
                    phi = j * Math.PI / 180;
                    Gl.glVertex3d(
                        1.01 * Math.Cos(phi) * Math.Cos(theta),
                        1.01 * Math.Cos(phi) * Math.Sin(theta),
                        1.01 * Math.Sin(phi));
                }
                Gl.glEnd();
            }

            Gl.glColor3d(0.2, 0.2, 0.2);

            for (int j = -80; j < 81; j = j + 10) {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (int i = 0; i < 361; i++) {
                    theta = i * Math.PI / 180;
                    phi = j * Math.PI / 180;
                    Gl.glVertex3d(
                        1.01 * Math.Cos(phi) * Math.Cos(theta),
                        1.01 * Math.Cos(phi) * Math.Sin(theta),
                        1.01 * Math.Sin(phi));
                }
                Gl.glEnd();
            }

            Gl.glColor3d(1, 0, 0);

            for (int i = 0; i < clinesl; i++) {

                Gl.glBegin(Gl.GL_LINE_STRIP);

                theta = clines[i, 0];
                phi = clines[i, 1];
                Gl.glVertex3d(
                    1.005 * Math.Cos(phi) * Math.Cos(theta),
                    1.005 * Math.Cos(phi) * Math.Sin(theta),
                    1.005 * Math.Sin(phi));

                theta = clines[i, 2];
                phi = clines[i, 3];
                Gl.glVertex3d(
                    1.005 * Math.Cos(phi) * Math.Cos(theta),
                    1.005 * Math.Cos(phi) * Math.Sin(theta),
                    1.005 * Math.Sin(phi));

                Gl.glEnd();
            }

            Gl.glColor3d(0, 1, 0);

            //richTextBox2.Text = Globals.targetRA + Environment.NewLine + Globals.targetDec;
            theta = (Globals.targetRAnow) * (Math.PI / 180);
            phi = Globals.targetDecnow * (Math.PI / 180);
            //richTextBox2.Text = Globals.targetRA + Environment.NewLine + Globals.targetDec + Environment.NewLine + String.Format("{0:0.00}", theta) + " " + String.Format("{0:0.00}", phi + Environment.NewLine + clinesl.ToString());
            double xhairLength;
            if (fovBar.Value == 0) {
                xhairLength = 0.05;
            } else {
                xhairLength = fovBar.Value / 1000.0;
            }

            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(
              0.99 * Math.Cos(phi + xhairLength) * Math.Cos(theta),
              0.99 * Math.Cos(phi + xhairLength) * Math.Sin(theta),
              0.99 * Math.Sin(phi + xhairLength));

            Gl.glVertex3d(
              0.99 * Math.Cos(phi - xhairLength) * Math.Cos(theta),
              0.99 * Math.Cos(phi - xhairLength) * Math.Sin(theta),
              0.99 * Math.Sin(phi - xhairLength));
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(
              0.99 * Math.Cos(phi) * Math.Cos(theta + xhairLength),
              0.99 * Math.Cos(phi) * Math.Sin(theta + xhairLength),
              0.99 * Math.Sin(phi));

            Gl.glVertex3d(
              0.99 * Math.Cos(phi) * Math.Cos(theta - xhairLength),
              0.99 * Math.Cos(phi) * Math.Sin(theta - xhairLength),
              0.99 * Math.Sin(phi));
            Gl.glEnd();

            Gl.glPointSize(9);
            Gl.glBegin(Gl.GL_POINTS);

            for (int i = 0; i < s1l; i++) {
                theta = s1[i, 0];
                phi = s1[i, 1];
                col = s1[i, 2];
                red = 255;
                if (col >= 0.5) {
                    red = 255 - (col * 5);
                }
                if (col < 0) {
                    red = 255 + (475 * col);
                }
                ///Blue
                if (col < 0.08) {
                    blue = 255;
                } else if (col > 0.4) {
                    blue = 120 - (20 * (col - 1.6));
                } else {
                    blue = 255 - (175 * col);
                }
                ///Green
                green = 255;
                if (col <= 0) {
                    green = 255 - (-170 * col);
                }
                if (col >= 0.4) {
                    green = 255 - (60 * (col - 0.4));
                }
                if (col >= 2) {
                    green = 225 - (35 * (col - 0.5));
                }

                Gl.glColor3d(red / 255, green / 255, blue / 255);

                Gl.glVertex3d
                    (Math.Cos(phi) * Math.Cos(theta), Math.Cos(phi) * Math.Sin(theta), Math.Sin(phi));
            }
            Gl.glEnd();


            Gl.glPointSize(5);
            Gl.glBegin(Gl.GL_POINTS);



            for (int i = 0; i < s3l; i++) {
                theta = s3[i, 0];
                phi = s3[i, 1];
                col = s3[i, 2];
                red = 255;
                if (col >= 0.5) {
                    red = 255 - (col * 5);
                }
                if (col < 0) {
                    red = 255 + (475 * col);
                }
                ///Blue
                if (col < 0.08) {
                    blue = 255;
                } else if (col > 0.4) {
                    blue = 120 - (20 * (col - 1.6));
                } else {
                    blue = 255 - (175 * col);
                }
                ///Green
                green = 255;
                if (col <= 0) {
                    green = 255 - (-170 * col);
                }
                if (col >= 0.4) {
                    green = 255 - (60 * (col - 0.4));
                }
                if (col >= 2) {
                    green = 225 - (35 * (col - 0.5));
                }

                Gl.glColor3d(red / 255, green / 255, blue / 255);
                Gl.glVertex3d(Math.Cos(phi) * Math.Cos(theta), Math.Cos(phi) * Math.Sin(theta), Math.Sin(phi));
            }
            Gl.glEnd();


            Gl.glPointSize(0.25F);
            Gl.glBegin(Gl.GL_POINTS);

            for (int i = 0; i < s6l; i++) {
                theta = s6[i, 0];
                phi = s6[i, 1];
                col = s6[i, 2];
                red = 255;
                if (col >= 0.5) {
                    red = 255 - (col * 5);
                }
                if (col < 0) {
                    red = 255 + (475 * col);
                }
                ///Blue
                if (col < 0.08) {
                    blue = 255;
                } else if (col > 0.4) {
                    blue = 120 - (20 * (col - 1.6));
                } else {
                    blue = 255 - (175 * col);
                }
                ///Green
                green = 255;
                if (col <= 0) {
                    green = 255 - (-170 * col);
                }
                if (col >= 0.4) {
                    green = 255 - (60 * (col - 0.4));
                }
                if (col >= 2) {
                    green = 225 - (35 * (col - 0.5));
                }

                Gl.glColor3d(red / 255, green / 255, blue / 255);
                Gl.glVertex3d(Math.Cos(phi) * Math.Cos(theta), Math.Cos(phi) * Math.Sin(theta), Math.Sin(phi));
            }
            Gl.glEnd();

        }

        Camara camara = new Camara();
        public Camara Camara {
            get { return camara; }
        }

        public void LoadLoadingScreen() {
            Application.Run(new LoadScreen());
        }

        public SkyMap() {

            Thread ThreadT = new Thread(new ThreadStart(LoadLoadingScreen));
            Globals.Status = "Loading, please wait...";
            ThreadT.Start();


            InitializeComponent();

            Screen[] sc;
            sc = Screen.AllScreens;

            if(!Globals.testing && sc.Length > 2) {

                this.WindowState = FormWindowState.Normal;

                this.Left = sc[3].Bounds.Left;
                this.Top = sc[3].Bounds.Top;
                this.StartPosition = FormStartPosition.Manual;
                this.WindowState = FormWindowState.Maximized;
            } 

           // richTextBox3.Text = this.Location.X.ToString() + " " + this.Location.Y.ToString();

            //richTextBox1.Text = "initialised";

            Globals.Status = "Initialising sky map";
            Globals.LoadProgress = 10;

            if(!Globals.testing) {

                hdc = (uint)pnlViewPort.Handle;
                string error = "";
                OpenGLControl.OpenGLInit(ref hdc, pnlViewPort.Width, pnlViewPort.Height, ref error);

                if(error != "") {
                    MessageBox.Show(error);
                }

                Camara.InitCamara();
                string[] split;
                int n = 0;
                clinesl = File.ReadAllLines(@"catalogs\constlines.dat").Length;
                StreamReader reader = File.OpenText(@"catalogs\constlines.dat");

                clines = new double[clinesl, 4];
                while(!reader.EndOfStream) {
                    split = reader.ReadLine().Split(',');
                    clines[n, 0] = Convert.ToDouble(split[0]) / 57.2957795;
                    clines[n, 1] = Convert.ToDouble(split[1]) / 57.2957795;
                    clines[n, 2] = Convert.ToDouble(split[2]) / 57.2957795;
                    clines[n, 3] = Convert.ToDouble(split[3]) / 57.2957795;
                    n++;
                }
                reader.Dispose();
                n = 0;
                s1l = File.ReadAllLines(@"catalogs\s1.dat").Length;
                reader = File.OpenText(@"catalogs\s1.dat");

                s1 = new double[s1l, 3];
                while(!reader.EndOfStream) {
                    split = reader.ReadLine().Split(',');
                    s1[n, 0] = Convert.ToDouble(split[1]) / 57.2957795;
                    s1[n, 1] = Convert.ToDouble(split[2]) / 57.2957795;
                    s1[n, 2] = Convert.ToDouble(split[6]);
                    n++;
                }
                reader.Dispose();
                n = 0;
                s3l = File.ReadAllLines(@"catalogs\s3.dat").Length;
                reader = File.OpenText(@"catalogs\s3.dat");

                s3 = new double[s3l, 3];
                while(!reader.EndOfStream) {
                    split = reader.ReadLine().Split(',');
                    s3[n, 0] = Convert.ToDouble(split[1]) / 57.2957795;
                    s3[n, 1] = Convert.ToDouble(split[2]) / 57.2957795;
                    s3[n, 2] = Convert.ToDouble(split[6]);
                    n++;
                }
                reader.Dispose();
                n = 0;
                s6l = File.ReadAllLines(@"catalogs\s6.dat").Length;
                reader = File.OpenText(@"catalogs\s6.dat");

                s6 = new double[s6l, 3];
                while(!reader.EndOfStream) {
                    split = reader.ReadLine().Split(',');
                    s6[n, 0] = Convert.ToDouble(split[1]) / 57.2957795;
                    s6[n, 1] = Convert.ToDouble(split[2]) / 57.2957795;
                    s6[n, 2] = Convert.ToDouble(split[6]);
                    n++;
                }
                reader.Dispose();

                //bg color
                Gl.glClearColor(0, 0, 0, 1);

            }
            Globals.Status = "Loading dome window";
            Globals.LoadProgress = 30;

            //   Timer = new System.Threading.Timer(TimerCallback, null, 0, 1000);
        }

        public struct Position {
            public double x;
            public double y;
            public double z;

            public Position(int x, int y, int z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }


        private void timer1_Tick(object sender, EventArgs e) {
            // }
            //     private void TimerCallback(object state){
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Camara.fov = fovBar.Value;

            thetaC = Globals.Scope.ra;
            phiC = Globals.Scope.dec;

            if (thetaC < 0) { thetaC += (Math.PI * 2); }
            if (thetaC > (2 * Math.PI)) { thetaC -= (Math.PI * 2); }
            Camara.centerx = Math.Cos(phiC) * Math.Cos(thetaC);
            Camara.centery = Math.Cos(phiC) * Math.Sin(thetaC);
            Camara.centerz = Math.Sin(phiC);
            /*richTextBox1.Text = String.Format(" theta {0:0.00}", thetaC)
                + Environment.NewLine + String.Format("phi {0:0.00}", phiC) + Environment.NewLine
                + Environment.NewLine + String.Format("sin theta {0:0.00}", Math.Sin(thetaC))
                + Environment.NewLine + String.Format("cos theta {0:0.00}", Math.Cos(thetaC))
                + Environment.NewLine + String.Format("sin phi {0:0.00}", Math.Sin(phiC))
                + Environment.NewLine + String.Format("cos phi{0:0.00}", Math.Cos(phiC))
                + Environment.NewLine
                + Environment.NewLine + String.Format("x {0:0.00}", Camara.centerx)
                + Environment.NewLine + String.Format("y {0:0.00}", Camara.centery)
                + Environment.NewLine + String.Format("z phi{0:0.00}", Camara.centerz)
                + Environment.NewLine
                 + Environment.NewLine + String.Format("RA {0:0.00}", (180 * thetaC) / (Math.PI * 15))
                + Environment.NewLine + String.Format("Dec {0:0.00}", (180 * phiC) / (Math.PI))
                + Environment.NewLine
                + Environment.NewLine + String.Format("fov {0:0.00}", Camara.fov);
              */
            Camara.Update();
            Draw();
            Gl.glFlush();
            Gl.glFinish();
            SwapBuffers(hdc);

        }


        [DllImport("GDI32.dll")]
        public static extern void SwapBuffers(uint hdc);
    }

    public class Camara {

        #region Private attributes

        public static double centerx, centery, centerz, fov, tanfov;
        #endregion


        public void InitCamara() {
            centerx = 0;
            centery = 0;
            centerz = -1;
            fov = 0;
            Update();
        }

        public void Update() {
            Gl.glViewport(0, 0, 800, 800);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            tanfov = Math.Tan(fov * Math.PI / 360);
            Gl.glOrtho(-tanfov, tanfov, -tanfov, tanfov, -0.05, 5);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Glu.gluLookAt(0, 0, 0, centerx, centery, centerz, 0, 0, 1);

        }
    }
}