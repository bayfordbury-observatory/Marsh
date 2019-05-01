namespace MarshControl {
    partial class DomeWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.SetupBtn = new System.Windows.Forms.Button();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.GotoTxt = new System.Windows.Forms.TextBox();
            this.pollDome = new System.Windows.Forms.Timer(this.components);
            this.HomeLbl = new System.Windows.Forms.Label();
            this.ShutterLbl = new System.Windows.Forms.Label();
            this.SlewingLbl = new System.Windows.Forms.Label();
            this.AzimuthLbl = new System.Windows.Forms.Label();
            this.ConnectLbl = new System.Windows.Forms.Label();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.AbortBtn = new System.Windows.Forms.Button();
            this.GotoBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.HomeBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.slaveChk = new System.Windows.Forms.CheckBox();
            this.TargetLbl = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtSafe = new System.Windows.Forms.Label();
            this.txtSkyBrightness = new System.Windows.Forms.Label();
            this.txtRain = new System.Windows.Forms.Label();
            this.txtPressure = new System.Windows.Forms.Label();
            this.txtHumidity = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtDewPt = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.txtWindDir = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtWindSpeed = new System.Windows.Forms.Label();
            this.txtSkyTemp = new System.Windows.Forms.Label();
            this.txtTemp = new System.Windows.Forms.Label();
            this.Label41 = new System.Windows.Forms.Label();
            this.Label37 = new System.Windows.Forms.Label();
            this.Label38 = new System.Windows.Forms.Label();
            this.Label40 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.pollWeather = new System.Windows.Forms.Timer(this.components);
            this.pollAllSky = new System.Windows.Forms.Timer(this.components);
            this.pollSun = new System.Windows.Forms.Timer(this.components);
            this.pollTargetAz = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SetupBtn
            // 
            this.SetupBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SetupBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetupBtn.ForeColor = System.Drawing.Color.Black;
            this.SetupBtn.Location = new System.Drawing.Point(35, 88);
            this.SetupBtn.Name = "SetupBtn";
            this.SetupBtn.Size = new System.Drawing.Size(90, 38);
            this.SetupBtn.TabIndex = 0;
            this.SetupBtn.Text = "Setup";
            this.SetupBtn.UseVisualStyleBackColor = false;
            this.SetupBtn.Click += new System.EventHandler(this.SetupDome_Click);
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ConnectBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectBtn.ForeColor = System.Drawing.Color.Black;
            this.ConnectBtn.Location = new System.Drawing.Point(131, 88);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(123, 38);
            this.ConnectBtn.TabIndex = 1;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = false;
            this.ConnectBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // GotoTxt
            // 
            this.GotoTxt.BackColor = System.Drawing.Color.White;
            this.GotoTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GotoTxt.ForeColor = System.Drawing.Color.Black;
            this.GotoTxt.Location = new System.Drawing.Point(127, 159);
            this.GotoTxt.Name = "GotoTxt";
            this.GotoTxt.Size = new System.Drawing.Size(65, 29);
            this.GotoTxt.TabIndex = 2;
            // 
            // pollDome
            // 
            this.pollDome.Interval = 200;
            this.pollDome.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // HomeLbl
            // 
            this.HomeLbl.AutoSize = true;
            this.HomeLbl.BackColor = System.Drawing.Color.Black;
            this.HomeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomeLbl.ForeColor = System.Drawing.Color.Silver;
            this.HomeLbl.Location = new System.Drawing.Point(37, 275);
            this.HomeLbl.Name = "HomeLbl";
            this.HomeLbl.Size = new System.Drawing.Size(80, 20);
            this.HomeLbl.TabIndex = 3;
            this.HomeLbl.Text = "At Home: ";
            // 
            // ShutterLbl
            // 
            this.ShutterLbl.AutoSize = true;
            this.ShutterLbl.BackColor = System.Drawing.Color.Black;
            this.ShutterLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShutterLbl.ForeColor = System.Drawing.Color.Silver;
            this.ShutterLbl.Location = new System.Drawing.Point(37, 305);
            this.ShutterLbl.Name = "ShutterLbl";
            this.ShutterLbl.Size = new System.Drawing.Size(66, 20);
            this.ShutterLbl.TabIndex = 4;
            this.ShutterLbl.Text = "Shutter:";
            // 
            // SlewingLbl
            // 
            this.SlewingLbl.AutoSize = true;
            this.SlewingLbl.BackColor = System.Drawing.Color.Black;
            this.SlewingLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlewingLbl.ForeColor = System.Drawing.Color.Silver;
            this.SlewingLbl.Location = new System.Drawing.Point(37, 338);
            this.SlewingLbl.Name = "SlewingLbl";
            this.SlewingLbl.Size = new System.Drawing.Size(68, 20);
            this.SlewingLbl.TabIndex = 5;
            this.SlewingLbl.Text = "Slewing:";
            // 
            // AzimuthLbl
            // 
            this.AzimuthLbl.AutoSize = true;
            this.AzimuthLbl.BackColor = System.Drawing.Color.Black;
            this.AzimuthLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AzimuthLbl.ForeColor = System.Drawing.Color.Silver;
            this.AzimuthLbl.Location = new System.Drawing.Point(37, 369);
            this.AzimuthLbl.Name = "AzimuthLbl";
            this.AzimuthLbl.Size = new System.Drawing.Size(75, 20);
            this.AzimuthLbl.TabIndex = 6;
            this.AzimuthLbl.Text = "Azimuth: ";
            // 
            // ConnectLbl
            // 
            this.ConnectLbl.AutoSize = true;
            this.ConnectLbl.BackColor = System.Drawing.Color.Black;
            this.ConnectLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectLbl.ForeColor = System.Drawing.Color.Silver;
            this.ConnectLbl.Location = new System.Drawing.Point(37, 246);
            this.ConnectLbl.Name = "ConnectLbl";
            this.ConnectLbl.Size = new System.Drawing.Size(134, 20);
            this.ConnectLbl.TabIndex = 7;
            this.ConnectLbl.Text = "Connected: False";
            // 
            // OpenBtn
            // 
            this.OpenBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.OpenBtn.Enabled = false;
            this.OpenBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenBtn.ForeColor = System.Drawing.Color.Black;
            this.OpenBtn.Location = new System.Drawing.Point(291, 88);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(146, 61);
            this.OpenBtn.TabIndex = 8;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = false;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // CloseBtn
            // 
            this.CloseBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CloseBtn.Enabled = false;
            this.CloseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseBtn.ForeColor = System.Drawing.Color.Black;
            this.CloseBtn.Location = new System.Drawing.Point(291, 169);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(146, 61);
            this.CloseBtn.TabIndex = 9;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = false;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // AbortBtn
            // 
            this.AbortBtn.BackColor = System.Drawing.Color.Red;
            this.AbortBtn.Enabled = false;
            this.AbortBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AbortBtn.ForeColor = System.Drawing.Color.Black;
            this.AbortBtn.Location = new System.Drawing.Point(319, 386);
            this.AbortBtn.Name = "AbortBtn";
            this.AbortBtn.Size = new System.Drawing.Size(146, 61);
            this.AbortBtn.TabIndex = 10;
            this.AbortBtn.Text = "STOP";
            this.AbortBtn.UseVisualStyleBackColor = false;
            this.AbortBtn.Click += new System.EventHandler(this.AbortBtn_Click);
            // 
            // GotoBtn
            // 
            this.GotoBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.GotoBtn.Enabled = false;
            this.GotoBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GotoBtn.ForeColor = System.Drawing.Color.Black;
            this.GotoBtn.Location = new System.Drawing.Point(198, 156);
            this.GotoBtn.Name = "GotoBtn";
            this.GotoBtn.Size = new System.Drawing.Size(66, 36);
            this.GotoBtn.TabIndex = 11;
            this.GotoBtn.Text = "Goto";
            this.GotoBtn.UseVisualStyleBackColor = false;
            this.GotoBtn.Click += new System.EventHandler(this.GotoBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(37, 162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 24);
            this.label1.TabIndex = 12;
            this.label1.Text = "Azimuth: ";
            // 
            // HomeBtn
            // 
            this.HomeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HomeBtn.Enabled = false;
            this.HomeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomeBtn.ForeColor = System.Drawing.Color.Black;
            this.HomeBtn.Location = new System.Drawing.Point(291, 250);
            this.HomeBtn.Name = "HomeBtn";
            this.HomeBtn.Size = new System.Drawing.Size(146, 61);
            this.HomeBtn.TabIndex = 13;
            this.HomeBtn.Text = "Home";
            this.HomeBtn.UseVisualStyleBackColor = false;
            this.HomeBtn.Click += new System.EventHandler(this.HomeBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.slaveChk);
            this.groupBox1.Controls.Add(this.TargetLbl);
            this.groupBox1.Controls.Add(this.SetupBtn);
            this.groupBox1.Controls.Add(this.HomeBtn);
            this.groupBox1.Controls.Add(this.AzimuthLbl);
            this.groupBox1.Controls.Add(this.ConnectLbl);
            this.groupBox1.Controls.Add(this.SlewingLbl);
            this.groupBox1.Controls.Add(this.ConnectBtn);
            this.groupBox1.Controls.Add(this.ShutterLbl);
            this.groupBox1.Controls.Add(this.GotoBtn);
            this.groupBox1.Controls.Add(this.HomeLbl);
            this.groupBox1.Controls.Add(this.CloseBtn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.OpenBtn);
            this.groupBox1.Controls.Add(this.GotoTxt);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Silver;
            this.groupBox1.Location = new System.Drawing.Point(28, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 486);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dome Control";
            // 
            // slaveChk
            // 
            this.slaveChk.AutoSize = true;
            this.slaveChk.Enabled = false;
            this.slaveChk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slaveChk.Location = new System.Drawing.Point(41, 410);
            this.slaveChk.Name = "slaveChk";
            this.slaveChk.Size = new System.Drawing.Size(202, 24);
            this.slaveChk.TabIndex = 15;
            this.slaveChk.Text = "Slave dome to telescope";
            this.slaveChk.UseVisualStyleBackColor = true;
            // 
            // TargetLbl
            // 
            this.TargetLbl.AutoSize = true;
            this.TargetLbl.BackColor = System.Drawing.Color.Black;
            this.TargetLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TargetLbl.ForeColor = System.Drawing.Color.Silver;
            this.TargetLbl.Location = new System.Drawing.Point(37, 437);
            this.TargetLbl.Name = "TargetLbl";
            this.TargetLbl.Size = new System.Drawing.Size(84, 20);
            this.TargetLbl.TabIndex = 14;
            this.TargetLbl.Text = "Target az: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSafe);
            this.groupBox2.Controls.Add(this.txtSkyBrightness);
            this.groupBox2.Controls.Add(this.txtRain);
            this.groupBox2.Controls.Add(this.txtPressure);
            this.groupBox2.Controls.Add(this.txtHumidity);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.txtDewPt);
            this.groupBox2.Controls.Add(this.Label8);
            this.groupBox2.Controls.Add(this.txtWindDir);
            this.groupBox2.Controls.Add(this.Label7);
            this.groupBox2.Controls.Add(this.Label4);
            this.groupBox2.Controls.Add(this.Label5);
            this.groupBox2.Controls.Add(this.txtWindSpeed);
            this.groupBox2.Controls.Add(this.txtSkyTemp);
            this.groupBox2.Controls.Add(this.txtTemp);
            this.groupBox2.Controls.Add(this.Label41);
            this.groupBox2.Controls.Add(this.Label37);
            this.groupBox2.Controls.Add(this.Label38);
            this.groupBox2.Controls.Add(this.Label40);
            this.groupBox2.Controls.Add(this.Label30);
            this.groupBox2.Controls.Add(this.Label34);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Silver;
            this.groupBox2.Location = new System.Drawing.Point(559, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(673, 828);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weather";
            // 
            // txtSafe
            // 
            this.txtSafe.AutoSize = true;
            this.txtSafe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSafe.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.txtSafe.Location = new System.Drawing.Point(152, 66);
            this.txtSafe.Name = "txtSafe";
            this.txtSafe.Size = new System.Drawing.Size(12, 16);
            this.txtSafe.TabIndex = 85;
            this.txtSafe.Text = "-";
            // 
            // txtSkyBrightness
            // 
            this.txtSkyBrightness.AutoSize = true;
            this.txtSkyBrightness.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSkyBrightness.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtSkyBrightness.Location = new System.Drawing.Point(152, 250);
            this.txtSkyBrightness.Name = "txtSkyBrightness";
            this.txtSkyBrightness.Size = new System.Drawing.Size(62, 16);
            this.txtSkyBrightness.TabIndex = 72;
            this.txtSkyBrightness.Text = "loading...";
            // 
            // txtRain
            // 
            this.txtRain.AutoSize = true;
            this.txtRain.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRain.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtRain.Location = new System.Drawing.Point(152, 230);
            this.txtRain.Name = "txtRain";
            this.txtRain.Size = new System.Drawing.Size(62, 16);
            this.txtRain.TabIndex = 75;
            this.txtRain.Text = "loading...";
            // 
            // txtPressure
            // 
            this.txtPressure.AutoSize = true;
            this.txtPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPressure.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtPressure.Location = new System.Drawing.Point(152, 191);
            this.txtPressure.Name = "txtPressure";
            this.txtPressure.Size = new System.Drawing.Size(62, 16);
            this.txtPressure.TabIndex = 82;
            this.txtPressure.Text = "loading...";
            // 
            // txtHumidity
            // 
            this.txtHumidity.AutoSize = true;
            this.txtHumidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHumidity.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtHumidity.Location = new System.Drawing.Point(152, 210);
            this.txtHumidity.Name = "txtHumidity";
            this.txtHumidity.Size = new System.Drawing.Size(62, 16);
            this.txtHumidity.TabIndex = 74;
            this.txtHumidity.Text = "loading...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(16, 305);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // txtDewPt
            // 
            this.txtDewPt.AutoSize = true;
            this.txtDewPt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDewPt.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtDewPt.Location = new System.Drawing.Point(152, 170);
            this.txtDewPt.Name = "txtDewPt";
            this.txtDewPt.Size = new System.Drawing.Size(62, 16);
            this.txtDewPt.TabIndex = 81;
            this.txtDewPt.Text = "loading...";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label8.Location = new System.Drawing.Point(27, 66);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(62, 16);
            this.Label8.TabIndex = 69;
            this.Label8.Text = "Weather";
            // 
            // txtWindDir
            // 
            this.txtWindDir.AutoSize = true;
            this.txtWindDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWindDir.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtWindDir.Location = new System.Drawing.Point(152, 150);
            this.txtWindDir.Name = "txtWindDir";
            this.txtWindDir.Size = new System.Drawing.Size(62, 16);
            this.txtWindDir.TabIndex = 78;
            this.txtWindDir.Text = "loading...";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label7.Location = new System.Drawing.Point(27, 90);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(89, 16);
            this.Label7.TabIndex = 68;
            this.Label7.Text = "Temperature:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label4.Location = new System.Drawing.Point(27, 110);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(115, 16);
            this.Label4.TabIndex = 65;
            this.Label4.Text = "Sky Temperature:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label5.Location = new System.Drawing.Point(27, 130);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(86, 16);
            this.Label5.TabIndex = 66;
            this.Label5.Text = "Wind Speed:";
            // 
            // txtWindSpeed
            // 
            this.txtWindSpeed.AutoSize = true;
            this.txtWindSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWindSpeed.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtWindSpeed.Location = new System.Drawing.Point(152, 130);
            this.txtWindSpeed.Name = "txtWindSpeed";
            this.txtWindSpeed.Size = new System.Drawing.Size(62, 16);
            this.txtWindSpeed.TabIndex = 64;
            this.txtWindSpeed.Text = "loading...";
            // 
            // txtSkyTemp
            // 
            this.txtSkyTemp.AutoSize = true;
            this.txtSkyTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSkyTemp.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtSkyTemp.Location = new System.Drawing.Point(152, 110);
            this.txtSkyTemp.Name = "txtSkyTemp";
            this.txtSkyTemp.Size = new System.Drawing.Size(62, 16);
            this.txtSkyTemp.TabIndex = 70;
            this.txtSkyTemp.Text = "loading...";
            // 
            // txtTemp
            // 
            this.txtTemp.AutoSize = true;
            this.txtTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTemp.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtTemp.Location = new System.Drawing.Point(152, 90);
            this.txtTemp.Name = "txtTemp";
            this.txtTemp.Size = new System.Drawing.Size(62, 16);
            this.txtTemp.TabIndex = 67;
            this.txtTemp.Text = "loading...";
            // 
            // Label41
            // 
            this.Label41.AutoSize = true;
            this.Label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label41.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label41.Location = new System.Drawing.Point(27, 190);
            this.Label41.Name = "Label41";
            this.Label41.Size = new System.Drawing.Size(87, 16);
            this.Label41.TabIndex = 80;
            this.Label41.Text = "Air  Pressure:";
            // 
            // Label37
            // 
            this.Label37.AutoSize = true;
            this.Label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label37.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label37.Location = new System.Drawing.Point(27, 230);
            this.Label37.Name = "Label37";
            this.Label37.Size = new System.Drawing.Size(93, 16);
            this.Label37.TabIndex = 76;
            this.Label37.Text = "Rain/Wetness";
            // 
            // Label38
            // 
            this.Label38.AutoSize = true;
            this.Label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label38.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label38.Location = new System.Drawing.Point(27, 150);
            this.Label38.Name = "Label38";
            this.Label38.Size = new System.Drawing.Size(98, 16);
            this.Label38.TabIndex = 77;
            this.Label38.Text = "Wind Direction:";
            // 
            // Label40
            // 
            this.Label40.AutoSize = true;
            this.Label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label40.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label40.Location = new System.Drawing.Point(27, 170);
            this.Label40.Name = "Label40";
            this.Label40.Size = new System.Drawing.Size(67, 16);
            this.Label40.TabIndex = 79;
            this.Label40.Text = "Dew point";
            // 
            // Label30
            // 
            this.Label30.AutoSize = true;
            this.Label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label30.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label30.Location = new System.Drawing.Point(27, 250);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(100, 16);
            this.Label30.TabIndex = 71;
            this.Label30.Text = "Sky Brightness:";
            // 
            // Label34
            // 
            this.Label34.AutoSize = true;
            this.Label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label34.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.Label34.Location = new System.Drawing.Point(27, 210);
            this.Label34.Name = "Label34";
            this.Label34.Size = new System.Drawing.Size(60, 16);
            this.Label34.TabIndex = 73;
            this.Label34.Text = "Humidity";
            // 
            // pollWeather
            // 
            this.pollWeather.Enabled = true;
            this.pollWeather.Interval = 5000;
            this.pollWeather.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // pollAllSky
            // 
            this.pollAllSky.Enabled = true;
            this.pollAllSky.Interval = 20000;
            this.pollAllSky.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // pollSun
            // 
            this.pollSun.Enabled = true;
            this.pollSun.Interval = 60000;
            this.pollSun.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // pollTargetAz
            // 
            this.pollTargetAz.Interval = 5000;
            this.pollTargetAz.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // DomeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1244, 962);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.AbortBtn);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "DomeWindow";
            this.Text = "Dome";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SetupBtn;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.TextBox GotoTxt;
        private System.Windows.Forms.Timer pollDome;
        private System.Windows.Forms.Label HomeLbl;
        private System.Windows.Forms.Label ShutterLbl;
        private System.Windows.Forms.Label SlewingLbl;
        private System.Windows.Forms.Label AzimuthLbl;
        private System.Windows.Forms.Label ConnectLbl;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Button AbortBtn;
        private System.Windows.Forms.Button GotoBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button HomeBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Timer pollWeather;
        internal System.Windows.Forms.Label txtSafe;
        internal System.Windows.Forms.Label txtPressure;
        internal System.Windows.Forms.Label txtDewPt;
        internal System.Windows.Forms.Label Label41;
        internal System.Windows.Forms.Label Label40;
        internal System.Windows.Forms.Label txtWindDir;
        internal System.Windows.Forms.Label Label38;
        internal System.Windows.Forms.Label Label37;
        internal System.Windows.Forms.Label txtRain;
        internal System.Windows.Forms.Label txtHumidity;
        internal System.Windows.Forms.Label Label34;
        internal System.Windows.Forms.Label txtSkyBrightness;
        internal System.Windows.Forms.Label Label30;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label txtTemp;
        internal System.Windows.Forms.Label txtSkyTemp;
        internal System.Windows.Forms.Label txtWindSpeed;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label Label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer pollAllSky;
        private System.Windows.Forms.Timer pollSun;
        private System.Windows.Forms.Label TargetLbl;
        private System.Windows.Forms.Timer pollTargetAz;
        private System.Windows.Forms.CheckBox slaveChk;
    }
}