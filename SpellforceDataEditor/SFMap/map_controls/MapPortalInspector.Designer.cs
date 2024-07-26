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
            PanelPortalList = new System.Windows.Forms.Panel();
            ListPortals = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            ButtonResizeList = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            PosX = new System.Windows.Forms.TextBox();
            PosY = new System.Windows.Forms.TextBox();
            PortalID = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            PanelProperties = new System.Windows.Forms.Panel();
            AngleTrackbar = new System.Windows.Forms.TrackBar();
            Angle = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            PanelPortalList.SuspendLayout();
            PanelProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).BeginInit();
            SuspendLayout();
            // 
            // PanelPortalList
            // 
            PanelPortalList.Controls.Add(ListPortals);
            PanelPortalList.Controls.Add(label1);
            PanelPortalList.Controls.Add(ButtonResizeList);
            PanelPortalList.Location = new System.Drawing.Point(4, 233);
            PanelPortalList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelPortalList.Name = "PanelPortalList";
            PanelPortalList.Size = new System.Drawing.Size(338, 183);
            PanelPortalList.TabIndex = 20;
            // 
            // ListPortals
            // 
            ListPortals.FormattingEnabled = true;
            ListPortals.ItemHeight = 15;
            ListPortals.Location = new System.Drawing.Point(5, 37);
            ListPortals.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListPortals.Name = "ListPortals";
            ListPortals.Size = new System.Drawing.Size(330, 139);
            ListPortals.TabIndex = 20;
            ListPortals.SelectedIndexChanged += ListPortals_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(78, 15);
            label1.TabIndex = 19;
            label1.Text = "List of portals";
            // 
            // ButtonResizeList
            // 
            ButtonResizeList.Location = new System.Drawing.Point(309, 5);
            ButtonResizeList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonResizeList.Name = "ButtonResizeList";
            ButtonResizeList.Size = new System.Drawing.Size(26, 25);
            ButtonResizeList.TabIndex = 0;
            ButtonResizeList.Text = "-";
            ButtonResizeList.UseVisualStyleBackColor = true;
            ButtonResizeList.Click += ButtonResizeList_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 37);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 15);
            label4.TabIndex = 6;
            label4.Text = "Position";
            // 
            // PosX
            // 
            PosX.Enabled = false;
            PosX.Location = new System.Drawing.Point(114, 33);
            PosX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosX.Name = "PosX";
            PosX.Size = new System.Drawing.Size(53, 23);
            PosX.TabIndex = 5;
            // 
            // PosY
            // 
            PosY.Enabled = false;
            PosY.Location = new System.Drawing.Point(177, 33);
            PosY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosY.Name = "PosY";
            PosY.Size = new System.Drawing.Size(53, 23);
            PosY.TabIndex = 15;
            // 
            // PortalID
            // 
            PortalID.BackColor = System.Drawing.Color.DarkOrange;
            PortalID.Location = new System.Drawing.Point(114, 3);
            PortalID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PortalID.Name = "PortalID";
            PortalID.ShortcutsEnabled = false;
            PortalID.Size = new System.Drawing.Size(116, 23);
            PortalID.TabIndex = 1;
            PortalID.MouseDown += PortalID_MouseDown;
            PortalID.Validated += PortalID_Validated;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 7);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(52, 15);
            label2.TabIndex = 2;
            label2.Text = "Portal ID";
            // 
            // PanelProperties
            // 
            PanelProperties.Controls.Add(AngleTrackbar);
            PanelProperties.Controls.Add(Angle);
            PanelProperties.Controls.Add(label6);
            PanelProperties.Controls.Add(label2);
            PanelProperties.Controls.Add(PortalID);
            PanelProperties.Controls.Add(PosY);
            PanelProperties.Controls.Add(PosX);
            PanelProperties.Controls.Add(label4);
            PanelProperties.Enabled = false;
            PanelProperties.Location = new System.Drawing.Point(4, 0);
            PanelProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelProperties.Name = "PanelProperties";
            PanelProperties.Size = new System.Drawing.Size(338, 232);
            PanelProperties.TabIndex = 21;
            // 
            // AngleTrackbar
            // 
            AngleTrackbar.AutoSize = false;
            AngleTrackbar.Location = new System.Drawing.Point(114, 63);
            AngleTrackbar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AngleTrackbar.Maximum = 359;
            AngleTrackbar.Name = "AngleTrackbar";
            AngleTrackbar.Size = new System.Drawing.Size(117, 23);
            AngleTrackbar.TabIndex = 20;
            AngleTrackbar.TickFrequency = 45;
            AngleTrackbar.ValueChanged += AngleTrackbar_ValueChanged;
            AngleTrackbar.MouseDown += AngleTrackbar_MouseDown;
            AngleTrackbar.MouseUp += AngleTrackbar_MouseUp;
            // 
            // Angle
            // 
            Angle.Location = new System.Drawing.Point(239, 67);
            Angle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Angle.Name = "Angle";
            Angle.Size = new System.Drawing.Size(53, 23);
            Angle.TabIndex = 19;
            Angle.Validated += Angle_Validated;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 70);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(38, 15);
            label6.TabIndex = 18;
            label6.Text = "Angle";
            // 
            // MapPortalInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(PanelProperties);
            Controls.Add(PanelPortalList);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapPortalInspector";
            Size = new System.Drawing.Size(348, 421);
            Load += MapPortalInspector_Load;
            PanelPortalList.ResumeLayout(false);
            PanelPortalList.PerformLayout();
            PanelProperties.ResumeLayout(false);
            PanelProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).EndInit();
            ResumeLayout(false);
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
