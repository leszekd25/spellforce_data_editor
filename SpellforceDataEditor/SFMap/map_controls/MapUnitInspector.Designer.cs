namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapUnitInspector
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
            this.LabelUnitName = new System.Windows.Forms.Label();
            this.UnitID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NPCID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Unknown1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Group = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Unknown2 = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.Angle = new System.Windows.Forms.TextBox();
            this.AngleTrackbar = new System.Windows.Forms.TrackBar();
            this.PanelUnitList = new System.Windows.Forms.Panel();
            this.SearchUnitPrevious = new System.Windows.Forms.Button();
            this.SearchUnitNext = new System.Windows.Forms.Button();
            this.SearchUnitText = new System.Windows.Forms.TextBox();
            this.ListUnits = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.PanelProperties = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).BeginInit();
            this.PanelUnitList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelUnitName
            // 
            this.LabelUnitName.AutoSize = true;
            this.LabelUnitName.Location = new System.Drawing.Point(95, 4);
            this.LabelUnitName.Name = "LabelUnitName";
            this.LabelUnitName.Size = new System.Drawing.Size(0, 13);
            this.LabelUnitName.TabIndex = 0;
            // 
            // UnitID
            // 
            this.UnitID.BackColor = System.Drawing.Color.DarkOrange;
            this.UnitID.Location = new System.Drawing.Point(98, 20);
            this.UnitID.Name = "UnitID";
            this.UnitID.Size = new System.Drawing.Size(100, 20);
            this.UnitID.TabIndex = 1;
            this.UnitID.Validated += new System.EventHandler(this.UnitID_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Unit ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "NPC ID";
            // 
            // NPCID
            // 
            this.NPCID.Enabled = false;
            this.NPCID.Location = new System.Drawing.Point(98, 46);
            this.NPCID.Name = "NPCID";
            this.NPCID.Size = new System.Drawing.Size(100, 20);
            this.NPCID.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Position";
            // 
            // PosX
            // 
            this.PosX.Enabled = false;
            this.PosX.Location = new System.Drawing.Point(98, 72);
            this.PosX.Name = "PosX";
            this.PosX.Size = new System.Drawing.Size(46, 20);
            this.PosX.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Angle";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Unknown";
            // 
            // Unknown1
            // 
            this.Unknown1.Location = new System.Drawing.Point(98, 124);
            this.Unknown1.Name = "Unknown1";
            this.Unknown1.Size = new System.Drawing.Size(100, 20);
            this.Unknown1.TabIndex = 9;
            this.Unknown1.Validated += new System.EventHandler(this.Unknown1_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Group";
            // 
            // Group
            // 
            this.Group.Location = new System.Drawing.Point(98, 150);
            this.Group.Name = "Group";
            this.Group.Size = new System.Drawing.Size(100, 20);
            this.Group.TabIndex = 11;
            this.Group.Validated += new System.EventHandler(this.Group_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Unknown 2";
            // 
            // Unknown2
            // 
            this.Unknown2.Location = new System.Drawing.Point(98, 176);
            this.Unknown2.Name = "Unknown2";
            this.Unknown2.Size = new System.Drawing.Size(100, 20);
            this.Unknown2.TabIndex = 13;
            this.Unknown2.Validated += new System.EventHandler(this.Unknown2_Validated);
            // 
            // PosY
            // 
            this.PosY.Enabled = false;
            this.PosY.Location = new System.Drawing.Point(152, 72);
            this.PosY.Name = "PosY";
            this.PosY.Size = new System.Drawing.Size(46, 20);
            this.PosY.TabIndex = 15;
            // 
            // Angle
            // 
            this.Angle.Location = new System.Drawing.Point(207, 98);
            this.Angle.Name = "Angle";
            this.Angle.Size = new System.Drawing.Size(46, 20);
            this.Angle.TabIndex = 16;
            this.Angle.Validated += new System.EventHandler(this.Angle_Validated);
            // 
            // AngleTrackbar
            // 
            this.AngleTrackbar.AutoSize = false;
            this.AngleTrackbar.Location = new System.Drawing.Point(98, 98);
            this.AngleTrackbar.Maximum = 359;
            this.AngleTrackbar.Name = "AngleTrackbar";
            this.AngleTrackbar.Size = new System.Drawing.Size(100, 20);
            this.AngleTrackbar.TabIndex = 17;
            this.AngleTrackbar.TickFrequency = 30;
            this.AngleTrackbar.ValueChanged += new System.EventHandler(this.AngleTrackbar_ValueChanged);
            // 
            // PanelUnitList
            // 
            this.PanelUnitList.Controls.Add(this.SearchUnitPrevious);
            this.PanelUnitList.Controls.Add(this.SearchUnitNext);
            this.PanelUnitList.Controls.Add(this.SearchUnitText);
            this.PanelUnitList.Controls.Add(this.ListUnits);
            this.PanelUnitList.Controls.Add(this.label1);
            this.PanelUnitList.Controls.Add(this.ButtonResizeList);
            this.PanelUnitList.Location = new System.Drawing.Point(3, 203);
            this.PanelUnitList.Name = "PanelUnitList";
            this.PanelUnitList.Size = new System.Drawing.Size(290, 214);
            this.PanelUnitList.TabIndex = 18;
            // 
            // SearchUnitPrevious
            // 
            this.SearchUnitPrevious.Location = new System.Drawing.Point(153, 187);
            this.SearchUnitPrevious.Name = "SearchUnitPrevious";
            this.SearchUnitPrevious.Size = new System.Drawing.Size(134, 23);
            this.SearchUnitPrevious.TabIndex = 23;
            this.SearchUnitPrevious.Text = "Find previous";
            this.SearchUnitPrevious.UseVisualStyleBackColor = true;
            this.SearchUnitPrevious.Click += new System.EventHandler(this.SearchUnitPrevious_Click);
            // 
            // SearchUnitNext
            // 
            this.SearchUnitNext.Location = new System.Drawing.Point(4, 187);
            this.SearchUnitNext.Name = "SearchUnitNext";
            this.SearchUnitNext.Size = new System.Drawing.Size(141, 23);
            this.SearchUnitNext.TabIndex = 22;
            this.SearchUnitNext.Text = "Find next";
            this.SearchUnitNext.UseVisualStyleBackColor = true;
            this.SearchUnitNext.Click += new System.EventHandler(this.SearchUnitNext_Click);
            // 
            // SearchUnitText
            // 
            this.SearchUnitText.Location = new System.Drawing.Point(4, 161);
            this.SearchUnitText.Name = "SearchUnitText";
            this.SearchUnitText.Size = new System.Drawing.Size(283, 20);
            this.SearchUnitText.TabIndex = 21;
            // 
            // ListUnits
            // 
            this.ListUnits.FormattingEnabled = true;
            this.ListUnits.Location = new System.Drawing.Point(4, 32);
            this.ListUnits.Name = "ListUnits";
            this.ListUnits.Size = new System.Drawing.Size(283, 121);
            this.ListUnits.TabIndex = 20;
            this.ListUnits.SelectedIndexChanged += new System.EventHandler(this.ListUnits_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of units";
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
            // PanelProperties
            // 
            this.PanelProperties.Controls.Add(this.label2);
            this.PanelProperties.Controls.Add(this.LabelUnitName);
            this.PanelProperties.Controls.Add(this.AngleTrackbar);
            this.PanelProperties.Controls.Add(this.UnitID);
            this.PanelProperties.Controls.Add(this.Angle);
            this.PanelProperties.Controls.Add(this.NPCID);
            this.PanelProperties.Controls.Add(this.PosY);
            this.PanelProperties.Controls.Add(this.label3);
            this.PanelProperties.Controls.Add(this.label8);
            this.PanelProperties.Controls.Add(this.PosX);
            this.PanelProperties.Controls.Add(this.Unknown2);
            this.PanelProperties.Controls.Add(this.label4);
            this.PanelProperties.Controls.Add(this.label7);
            this.PanelProperties.Controls.Add(this.label5);
            this.PanelProperties.Controls.Add(this.Group);
            this.PanelProperties.Controls.Add(this.Unknown1);
            this.PanelProperties.Controls.Add(this.label6);
            this.PanelProperties.Enabled = false;
            this.PanelProperties.Location = new System.Drawing.Point(3, 0);
            this.PanelProperties.Name = "PanelProperties";
            this.PanelProperties.Size = new System.Drawing.Size(290, 201);
            this.PanelProperties.TabIndex = 19;
            // 
            // MapUnitInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelUnitList);
            this.Name = "MapUnitInspector";
            this.Size = new System.Drawing.Size(296, 420);
            this.Load += new System.EventHandler(this.MapUnitInspector_Load);
            this.Resize += new System.EventHandler(this.MapUnitInspector_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).EndInit();
            this.PanelUnitList.ResumeLayout(false);
            this.PanelUnitList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelUnitName;
        private System.Windows.Forms.TextBox UnitID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NPCID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Unknown1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Group;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Unknown2;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.Panel PanelUnitList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Button SearchUnitPrevious;
        private System.Windows.Forms.Button SearchUnitNext;
        private System.Windows.Forms.TextBox SearchUnitText;
        private System.Windows.Forms.ListBox ListUnits;
        private System.Windows.Forms.Panel PanelProperties;
    }
}
