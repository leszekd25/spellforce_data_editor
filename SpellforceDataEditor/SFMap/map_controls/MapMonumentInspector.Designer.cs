namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapMonumentInspector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PanelMonumentList = new System.Windows.Forms.Panel();
            this.ListMonuments = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.PanelProperties = new System.Windows.Forms.Panel();
            this.AngleTrackbar = new System.Windows.Forms.TrackBar();
            this.Angle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PanelMonumentList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelMonumentList
            // 
            this.PanelMonumentList.Controls.Add(this.ListMonuments);
            this.PanelMonumentList.Controls.Add(this.label1);
            this.PanelMonumentList.Controls.Add(this.ButtonResizeList);
            this.PanelMonumentList.Location = new System.Drawing.Point(3, 202);
            this.PanelMonumentList.Name = "PanelMonumentList";
            this.PanelMonumentList.Size = new System.Drawing.Size(290, 159);
            this.PanelMonumentList.TabIndex = 20;
            // 
            // ListMonuments
            // 
            this.ListMonuments.FormattingEnabled = true;
            this.ListMonuments.Location = new System.Drawing.Point(4, 32);
            this.ListMonuments.Name = "ListMonuments";
            this.ListMonuments.Size = new System.Drawing.Size(283, 121);
            this.ListMonuments.TabIndex = 20;
            this.ListMonuments.SelectedIndexChanged += new System.EventHandler(this.ListMonuments_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of monuments";
            // 
            // ButtonResizeList
            // 
            this.ButtonResizeList.Location = new System.Drawing.Point(265, 4);
            this.ButtonResizeList.Name = "ButtonResizeList";
            this.ButtonResizeList.Size = new System.Drawing.Size(22, 22);
            this.ButtonResizeList.TabIndex = 0;
            this.ButtonResizeList.Text = "-";
            this.ButtonResizeList.UseVisualStyleBackColor = true;
            this.ButtonResizeList.Click += new System.EventHandler(this.ButtonResizeList_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Position";
            // 
            // PosX
            // 
            this.PosX.Enabled = false;
            this.PosX.Location = new System.Drawing.Point(98, 3);
            this.PosX.Name = "PosX";
            this.PosX.Size = new System.Drawing.Size(46, 20);
            this.PosX.TabIndex = 5;
            // 
            // PosY
            // 
            this.PosY.Enabled = false;
            this.PosY.Location = new System.Drawing.Point(152, 3);
            this.PosY.Name = "PosY";
            this.PosY.Size = new System.Drawing.Size(46, 20);
            this.PosY.TabIndex = 15;
            // 
            // PanelProperties
            // 
            this.PanelProperties.Controls.Add(this.AngleTrackbar);
            this.PanelProperties.Controls.Add(this.Angle);
            this.PanelProperties.Controls.Add(this.label6);
            this.PanelProperties.Controls.Add(this.PosY);
            this.PanelProperties.Controls.Add(this.PosX);
            this.PanelProperties.Controls.Add(this.label4);
            this.PanelProperties.Enabled = false;
            this.PanelProperties.Location = new System.Drawing.Point(3, 0);
            this.PanelProperties.Name = "PanelProperties";
            this.PanelProperties.Size = new System.Drawing.Size(290, 201);
            this.PanelProperties.TabIndex = 21;
            // 
            // AngleTrackbar
            // 
            this.AngleTrackbar.AutoSize = false;
            this.AngleTrackbar.Location = new System.Drawing.Point(98, 29);
            this.AngleTrackbar.Maximum = 359;
            this.AngleTrackbar.Name = "AngleTrackbar";
            this.AngleTrackbar.Size = new System.Drawing.Size(100, 20);
            this.AngleTrackbar.TabIndex = 20;
            this.AngleTrackbar.TickFrequency = 45;
            this.AngleTrackbar.ValueChanged += new System.EventHandler(this.AngleTrackbar_ValueChanged);
            this.AngleTrackbar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AngleTrackbar_MouseDown);
            this.AngleTrackbar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AngleTrackbar_MouseUp);
            // 
            // Angle
            // 
            this.Angle.Enabled = false;
            this.Angle.Location = new System.Drawing.Point(205, 32);
            this.Angle.Name = "Angle";
            this.Angle.Size = new System.Drawing.Size(46, 20);
            this.Angle.TabIndex = 19;
            this.Angle.Validated += new System.EventHandler(this.Angle_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Angle";
            // 
            // MapMonumentInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelMonumentList);
            this.Name = "MapMonumentInspector";
            this.Size = new System.Drawing.Size(298, 365);
            this.Load += new System.EventHandler(this.MapMonumentInspector_Load);
            this.PanelMonumentList.ResumeLayout(false);
            this.PanelMonumentList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelMonumentList;
        private System.Windows.Forms.ListBox ListMonuments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.Label label6;
    }
}
