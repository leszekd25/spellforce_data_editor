namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapPortalInspector
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
            this.PanelPortalList = new System.Windows.Forms.Panel();
            this.ListPortals = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.PortalID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PanelProperties = new System.Windows.Forms.Panel();
            this.AngleTrackbar = new System.Windows.Forms.TrackBar();
            this.Angle = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PanelPortalList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelPortalList
            // 
            this.PanelPortalList.Controls.Add(this.ListPortals);
            this.PanelPortalList.Controls.Add(this.label1);
            this.PanelPortalList.Controls.Add(this.ButtonResizeList);
            this.PanelPortalList.Location = new System.Drawing.Point(3, 202);
            this.PanelPortalList.Name = "PanelPortalList";
            this.PanelPortalList.Size = new System.Drawing.Size(290, 159);
            this.PanelPortalList.TabIndex = 20;
            // 
            // ListPortals
            // 
            this.ListPortals.FormattingEnabled = true;
            this.ListPortals.Location = new System.Drawing.Point(4, 32);
            this.ListPortals.Name = "ListPortals";
            this.ListPortals.Size = new System.Drawing.Size(283, 121);
            this.ListPortals.TabIndex = 20;
            this.ListPortals.SelectedIndexChanged += new System.EventHandler(this.ListPortals_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of portals";
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
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Position";
            // 
            // PosX
            // 
            this.PosX.Enabled = false;
            this.PosX.Location = new System.Drawing.Point(98, 29);
            this.PosX.Name = "PosX";
            this.PosX.Size = new System.Drawing.Size(46, 20);
            this.PosX.TabIndex = 5;
            // 
            // PosY
            // 
            this.PosY.Enabled = false;
            this.PosY.Location = new System.Drawing.Point(152, 29);
            this.PosY.Name = "PosY";
            this.PosY.Size = new System.Drawing.Size(46, 20);
            this.PosY.TabIndex = 15;
            // 
            // PortalID
            // 
            this.PortalID.BackColor = System.Drawing.Color.DarkOrange;
            this.PortalID.Location = new System.Drawing.Point(98, 3);
            this.PortalID.Name = "PortalID";
            this.PortalID.Size = new System.Drawing.Size(100, 20);
            this.PortalID.TabIndex = 1;
            this.PortalID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PortalID_MouseDown);
            this.PortalID.Validated += new System.EventHandler(this.PortalID_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Portal ID";
            // 
            // PanelProperties
            // 
            this.PanelProperties.Controls.Add(this.AngleTrackbar);
            this.PanelProperties.Controls.Add(this.Angle);
            this.PanelProperties.Controls.Add(this.label6);
            this.PanelProperties.Controls.Add(this.label2);
            this.PanelProperties.Controls.Add(this.PortalID);
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
            this.AngleTrackbar.Location = new System.Drawing.Point(98, 55);
            this.AngleTrackbar.Maximum = 359;
            this.AngleTrackbar.Name = "AngleTrackbar";
            this.AngleTrackbar.Size = new System.Drawing.Size(100, 20);
            this.AngleTrackbar.TabIndex = 20;
            this.AngleTrackbar.TickFrequency = 45;
            // 
            // Angle
            // 
            this.Angle.Enabled = false;
            this.Angle.Location = new System.Drawing.Point(205, 58);
            this.Angle.Name = "Angle";
            this.Angle.Size = new System.Drawing.Size(46, 20);
            this.Angle.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Angle";
            // 
            // MapPortalInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelPortalList);
            this.Name = "MapPortalInspector";
            this.Size = new System.Drawing.Size(298, 365);
            this.Load += new System.EventHandler(this.MapPortalInspector_Load);
            this.PanelPortalList.ResumeLayout(false);
            this.PanelPortalList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelPortalList;
        private System.Windows.Forms.ListBox ListPortals;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.TextBox PortalID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.Label label6;
    }
}
