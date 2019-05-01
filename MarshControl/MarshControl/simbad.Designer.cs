namespace MarshControl {
     partial class simbad {
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
            this.TargetsetBtn = new System.Windows.Forms.Button();
            this.Label13 = new System.Windows.Forms.Label();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.IdentsLbl = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.AzLbl = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.AltLbl = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.DecLbl = new System.Windows.Forms.Label();
            this.InfoBox = new System.Windows.Forms.RichTextBox();
            this.CreditLbl = new System.Windows.Forms.Label();
            this.ShowImageChk = new System.Windows.Forms.CheckBox();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.SizeLbl = new System.Windows.Forms.Label();
            this.RALbl = new System.Windows.Forms.Label();
            this.TypeLbl = new System.Windows.Forms.Label();
            this.FluxLbl = new System.Windows.Forms.Label();
            this.TargetNameBox = new System.Windows.Forms.TextBox();
            this.SearchBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // TargetsetBtn
            // 
            this.TargetsetBtn.Enabled = false;
            this.TargetsetBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TargetsetBtn.Location = new System.Drawing.Point(127, 318);
            this.TargetsetBtn.Name = "TargetsetBtn";
            this.TargetsetBtn.Size = new System.Drawing.Size(92, 23);
            this.TargetsetBtn.TabIndex = 82;
            this.TargetsetBtn.Text = "Set as target?";
            this.TargetsetBtn.UseVisualStyleBackColor = true;
            this.TargetsetBtn.Click += new System.EventHandler(this.Targetset_Click);
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.BackColor = System.Drawing.Color.Gray;
            this.Label13.Location = new System.Drawing.Point(6, 359);
            this.Label13.Name = "Label13";
            this.Label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label13.Size = new System.Drawing.Size(10, 13);
            this.Label13.TabIndex = 71;
            this.Label13.Text = "-";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label13.Visible = false;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(195, 356);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(435, 20);
            this.ProgressBar1.TabIndex = 70;
            this.ProgressBar1.Visible = false;
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.BackColor = System.Drawing.Color.Gray;
            this.StatusStrip1.Location = new System.Drawing.Point(0, 355);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(631, 22);
            this.StatusStrip1.TabIndex = 81;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // IdentsLbl
            // 
            this.IdentsLbl.AutoSize = true;
            this.IdentsLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.IdentsLbl.Location = new System.Drawing.Point(14, 192);
            this.IdentsLbl.Name = "IdentsLbl";
            this.IdentsLbl.Size = new System.Drawing.Size(82, 13);
            this.IdentsLbl.TabIndex = 80;
            this.IdentsLbl.Text = "List of identifiers";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label6.Location = new System.Drawing.Point(14, 143);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(19, 13);
            this.Label6.TabIndex = 79;
            this.Label6.Text = "Az";
            // 
            // AzLbl
            // 
            this.AzLbl.AutoSize = true;
            this.AzLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.AzLbl.Location = new System.Drawing.Point(55, 143);
            this.AzLbl.Name = "AzLbl";
            this.AzLbl.Size = new System.Drawing.Size(10, 13);
            this.AzLbl.TabIndex = 78;
            this.AzLbl.Text = "-";
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label15.Location = new System.Drawing.Point(14, 127);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(19, 13);
            this.Label15.TabIndex = 77;
            this.Label15.Text = "Alt";
            // 
            // AltLbl
            // 
            this.AltLbl.AutoSize = true;
            this.AltLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.AltLbl.Location = new System.Drawing.Point(55, 127);
            this.AltLbl.Name = "AltLbl";
            this.AltLbl.Size = new System.Drawing.Size(10, 13);
            this.AltLbl.TabIndex = 76;
            this.AltLbl.Text = "-";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label5.Location = new System.Drawing.Point(14, 72);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(27, 13);
            this.Label5.TabIndex = 75;
            this.Label5.Text = "Dec";
            // 
            // DecLbl
            // 
            this.DecLbl.AutoSize = true;
            this.DecLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.DecLbl.Location = new System.Drawing.Point(55, 69);
            this.DecLbl.Name = "DecLbl";
            this.DecLbl.Size = new System.Drawing.Size(10, 13);
            this.DecLbl.TabIndex = 74;
            this.DecLbl.Text = "-";
            // 
            // InfoBox
            // 
            this.InfoBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.InfoBox.Location = new System.Drawing.Point(14, 208);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.InfoBox.Size = new System.Drawing.Size(205, 104);
            this.InfoBox.TabIndex = 73;
            this.InfoBox.Text = "";
            // 
            // CreditLbl
            // 
            this.CreditLbl.AutoSize = true;
            this.CreditLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.CreditLbl.Location = new System.Drawing.Point(352, 315);
            this.CreditLbl.Name = "CreditLbl";
            this.CreditLbl.Size = new System.Drawing.Size(242, 13);
            this.CreditLbl.TabIndex = 72;
            this.CreditLbl.Text = "Digitized Sky Survey image. Field of view 20\' x 20\'";
            this.CreditLbl.Visible = false;
            // 
            // ShowImageChk
            // 
            this.ShowImageChk.AutoSize = true;
            this.ShowImageChk.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.ShowImageChk.Location = new System.Drawing.Point(220, 41);
            this.ShowImageChk.Name = "ShowImageChk";
            this.ShowImageChk.Size = new System.Drawing.Size(84, 17);
            this.ShowImageChk.TabIndex = 69;
            this.ShowImageChk.Text = "Show image";
            this.ShowImageChk.UseVisualStyleBackColor = true;
            // 
            // PictureBox1
            // 
            this.PictureBox1.BackColor = System.Drawing.SystemColors.ControlText;
            this.PictureBox1.Location = new System.Drawing.Point(321, 12);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(300, 300);
            this.PictureBox1.TabIndex = 68;
            this.PictureBox1.TabStop = false;
            this.PictureBox1.Visible = false;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label10.Location = new System.Drawing.Point(14, 163);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(27, 13);
            this.Label10.TabIndex = 67;
            this.Label10.Text = "Size";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label9.Location = new System.Drawing.Point(14, 52);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(22, 13);
            this.Label9.TabIndex = 66;
            this.Label9.Text = "RA";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label8.Location = new System.Drawing.Point(14, 87);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(31, 13);
            this.Label8.TabIndex = 65;
            this.Label8.Text = "Type";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Label7.Location = new System.Drawing.Point(14, 107);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(26, 13);
            this.Label7.TabIndex = 64;
            this.Label7.Text = "Flux";
            // 
            // SizeLbl
            // 
            this.SizeLbl.AutoSize = true;
            this.SizeLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.SizeLbl.Location = new System.Drawing.Point(54, 163);
            this.SizeLbl.Name = "SizeLbl";
            this.SizeLbl.Size = new System.Drawing.Size(10, 13);
            this.SizeLbl.TabIndex = 63;
            this.SizeLbl.Text = "-";
            // 
            // RALbl
            // 
            this.RALbl.AutoSize = true;
            this.RALbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.RALbl.Location = new System.Drawing.Point(55, 51);
            this.RALbl.Name = "RALbl";
            this.RALbl.Size = new System.Drawing.Size(10, 13);
            this.RALbl.TabIndex = 62;
            this.RALbl.Text = "-";
            // 
            // TypeLbl
            // 
            this.TypeLbl.AutoSize = true;
            this.TypeLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.TypeLbl.Location = new System.Drawing.Point(55, 87);
            this.TypeLbl.Name = "TypeLbl";
            this.TypeLbl.Size = new System.Drawing.Size(10, 13);
            this.TypeLbl.TabIndex = 61;
            this.TypeLbl.Text = "-";
            // 
            // FluxLbl
            // 
            this.FluxLbl.AutoSize = true;
            this.FluxLbl.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FluxLbl.Location = new System.Drawing.Point(55, 107);
            this.FluxLbl.Name = "FluxLbl";
            this.FluxLbl.Size = new System.Drawing.Size(10, 13);
            this.FluxLbl.TabIndex = 60;
            this.FluxLbl.Text = "-";
            // 
            // TargetNameBox
            // 
            this.TargetNameBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.TargetNameBox.Location = new System.Drawing.Point(14, 12);
            this.TargetNameBox.Name = "TargetNameBox";
            this.TargetNameBox.Size = new System.Drawing.Size(205, 20);
            this.TargetNameBox.TabIndex = 59;
            this.TargetNameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            // 
            // SearchBtn
            // 
            this.SearchBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.SearchBtn.Location = new System.Drawing.Point(229, 12);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(75, 23);
            this.SearchBtn.TabIndex = 58;
            this.SearchBtn.Text = "Search";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.Search_Click);
            // 
            // simbad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(631, 377);
            this.Controls.Add(this.TargetsetBtn);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.ProgressBar1);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.IdentsLbl);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.AzLbl);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.AltLbl);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.DecLbl);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.CreditLbl);
            this.Controls.Add(this.ShowImageChk);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.SizeLbl);
            this.Controls.Add(this.RALbl);
            this.Controls.Add(this.TypeLbl);
            this.Controls.Add(this.FluxLbl);
            this.Controls.Add(this.TargetNameBox);
            this.Controls.Add(this.SearchBtn);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Name = "simbad";
            this.Text = "simbad";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button TargetsetBtn;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.ProgressBar ProgressBar1;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.Label IdentsLbl;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label AzLbl;
        internal System.Windows.Forms.Label Label15;
        internal System.Windows.Forms.Label AltLbl;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label DecLbl;
        internal System.Windows.Forms.RichTextBox InfoBox;
        internal System.Windows.Forms.Label CreditLbl;
        internal System.Windows.Forms.CheckBox ShowImageChk;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label SizeLbl;
        internal System.Windows.Forms.Label RALbl;
        internal System.Windows.Forms.Label TypeLbl;
        internal System.Windows.Forms.Label FluxLbl;
        internal System.Windows.Forms.TextBox TargetNameBox;
        internal System.Windows.Forms.Button SearchBtn;
    }
}