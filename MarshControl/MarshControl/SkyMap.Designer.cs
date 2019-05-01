namespace MarshControl {
    partial class SkyMap {
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
            this.pnlViewPort = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.fovBar = new System.Windows.Forms.TrackBar();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.fovBar)).BeginInit();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlViewPort
            // 
            this.pnlViewPort.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlViewPort.Location = new System.Drawing.Point(6, 25);
            this.pnlViewPort.Name = "pnlViewPort";
            this.pnlViewPort.Size = new System.Drawing.Size(800, 800);
            this.pnlViewPort.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // fovBar
            // 
            this.fovBar.LargeChange = 1;
            this.fovBar.Location = new System.Drawing.Point(6, 831);
            this.fovBar.Maximum = 70;
            this.fovBar.Name = "fovBar";
            this.fovBar.Size = new System.Drawing.Size(800, 45);
            this.fovBar.TabIndex = 2;
            this.fovBar.Value = 70;
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.fovBar);
            this.GroupBox2.Controls.Add(this.pnlViewPort);
            this.GroupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox2.ForeColor = System.Drawing.Color.DarkGray;
            this.GroupBox2.Location = new System.Drawing.Point(12, 12);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(812, 880);
            this.GroupBox2.TabIndex = 31;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Tag = "";
            this.GroupBox2.Text = "Sky Map";
            // 
            // SkyMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(944, 962);
            this.Controls.Add(this.GroupBox2);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SkyMap";
            this.Text = "SkyMap";
            ((System.ComponentModel.ISupportInitialize)(this.fovBar)).EndInit();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlViewPort;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar fovBar;
        internal System.Windows.Forms.GroupBox GroupBox2;
    }
}