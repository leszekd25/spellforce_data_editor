namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapBuildingInspector
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
            this.LabelBuildingName = new System.Windows.Forms.Label();
            this.BuildingID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NPCID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Level = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.RaceID = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.Angle = new System.Windows.Forms.TextBox();
            this.AngleTrackbar = new System.Windows.Forms.TrackBar();
            this.PanelBuildingList = new System.Windows.Forms.Panel();
            this.SearchBuildingPrevious = new System.Windows.Forms.Button();
            this.SearchBuildingNext = new System.Windows.Forms.Button();
            this.SearchBuildingText = new System.Windows.Forms.TextBox();
            this.ListBuildings = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.PanelProperties = new System.Windows.Forms.Panel();
            this.NPCScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).BeginInit();
            this.PanelBuildingList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelBuildingName
            // 
            this.LabelBuildingName.AutoSize = true;
            this.LabelBuildingName.Location = new System.Drawing.Point(95, 4);
            this.LabelBuildingName.Name = "LabelBuildingName";
            this.LabelBuildingName.Size = new System.Drawing.Size(0, 13);
            this.LabelBuildingName.TabIndex = 0;
            // 
            // BuildingID
            // 
            this.BuildingID.BackColor = System.Drawing.Color.DarkOrange;
            this.BuildingID.Location = new System.Drawing.Point(98, 20);
            this.BuildingID.Name = "BuildingID";
            this.BuildingID.Size = new System.Drawing.Size(100, 20);
            this.BuildingID.TabIndex = 1;
            this.BuildingID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BuildingID_MouseDown);
            this.BuildingID.Validated += new System.EventHandler(this.BuildingID_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Building ID";
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
            this.NPCID.Location = new System.Drawing.Point(98, 46);
            this.NPCID.Name = "NPCID";
            this.NPCID.Size = new System.Drawing.Size(100, 20);
            this.NPCID.TabIndex = 3;
            this.NPCID.Validated += new System.EventHandler(this.NPCID_Validated);
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
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Level";
            // 
            // Level
            // 
            this.Level.Location = new System.Drawing.Point(98, 124);
            this.Level.Name = "Level";
            this.Level.Size = new System.Drawing.Size(100, 20);
            this.Level.TabIndex = 9;
            this.Level.Validated += new System.EventHandler(this.Level_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Race ID";
            // 
            // RaceID
            // 
            this.RaceID.Location = new System.Drawing.Point(98, 150);
            this.RaceID.Name = "RaceID";
            this.RaceID.Size = new System.Drawing.Size(100, 20);
            this.RaceID.TabIndex = 11;
            this.RaceID.Validated += new System.EventHandler(this.RaceID_Validated);
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
            // PanelBuildingList
            // 
            this.PanelBuildingList.Controls.Add(this.SearchBuildingPrevious);
            this.PanelBuildingList.Controls.Add(this.SearchBuildingNext);
            this.PanelBuildingList.Controls.Add(this.SearchBuildingText);
            this.PanelBuildingList.Controls.Add(this.ListBuildings);
            this.PanelBuildingList.Controls.Add(this.label1);
            this.PanelBuildingList.Controls.Add(this.ButtonResizeList);
            this.PanelBuildingList.Location = new System.Drawing.Point(3, 203);
            this.PanelBuildingList.Name = "PanelBuildingList";
            this.PanelBuildingList.Size = new System.Drawing.Size(290, 214);
            this.PanelBuildingList.TabIndex = 18;
            // 
            // SearchBuildingPrevious
            // 
            this.SearchBuildingPrevious.Location = new System.Drawing.Point(153, 187);
            this.SearchBuildingPrevious.Name = "SearchBuildingPrevious";
            this.SearchBuildingPrevious.Size = new System.Drawing.Size(134, 23);
            this.SearchBuildingPrevious.TabIndex = 23;
            this.SearchBuildingPrevious.Text = "Find previous";
            this.SearchBuildingPrevious.UseVisualStyleBackColor = true;
            this.SearchBuildingPrevious.Click += new System.EventHandler(this.SearchBuildingPrevious_Click);
            // 
            // SearchBuildingNext
            // 
            this.SearchBuildingNext.Location = new System.Drawing.Point(4, 187);
            this.SearchBuildingNext.Name = "SearchBuildingNext";
            this.SearchBuildingNext.Size = new System.Drawing.Size(141, 23);
            this.SearchBuildingNext.TabIndex = 22;
            this.SearchBuildingNext.Text = "Find next";
            this.SearchBuildingNext.UseVisualStyleBackColor = true;
            this.SearchBuildingNext.Click += new System.EventHandler(this.SearchBuildingNext_Click);
            // 
            // SearchBuildingText
            // 
            this.SearchBuildingText.Location = new System.Drawing.Point(4, 161);
            this.SearchBuildingText.Name = "SearchBuildingText";
            this.SearchBuildingText.Size = new System.Drawing.Size(283, 20);
            this.SearchBuildingText.TabIndex = 21;
            // 
            // ListBuildings
            // 
            this.ListBuildings.FormattingEnabled = true;
            this.ListBuildings.Location = new System.Drawing.Point(4, 32);
            this.ListBuildings.Name = "ListBuildings";
            this.ListBuildings.Size = new System.Drawing.Size(283, 121);
            this.ListBuildings.TabIndex = 20;
            this.ListBuildings.SelectedIndexChanged += new System.EventHandler(this.ListBuildings_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of buildings";
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
            this.PanelProperties.Controls.Add(this.NPCScript);
            this.PanelProperties.Controls.Add(this.label2);
            this.PanelProperties.Controls.Add(this.LabelBuildingName);
            this.PanelProperties.Controls.Add(this.AngleTrackbar);
            this.PanelProperties.Controls.Add(this.BuildingID);
            this.PanelProperties.Controls.Add(this.Angle);
            this.PanelProperties.Controls.Add(this.NPCID);
            this.PanelProperties.Controls.Add(this.PosY);
            this.PanelProperties.Controls.Add(this.label3);
            this.PanelProperties.Controls.Add(this.PosX);
            this.PanelProperties.Controls.Add(this.label4);
            this.PanelProperties.Controls.Add(this.label7);
            this.PanelProperties.Controls.Add(this.label5);
            this.PanelProperties.Controls.Add(this.RaceID);
            this.PanelProperties.Controls.Add(this.Level);
            this.PanelProperties.Controls.Add(this.label6);
            this.PanelProperties.Enabled = false;
            this.PanelProperties.Location = new System.Drawing.Point(3, 0);
            this.PanelProperties.Name = "PanelProperties";
            this.PanelProperties.Size = new System.Drawing.Size(290, 201);
            this.PanelProperties.TabIndex = 19;
            // 
            // NPCScript
            // 
            this.NPCScript.Location = new System.Drawing.Point(204, 44);
            this.NPCScript.Name = "NPCScript";
            this.NPCScript.Size = new System.Drawing.Size(83, 23);
            this.NPCScript.TabIndex = 19;
            this.NPCScript.Text = "Open script";
            this.NPCScript.UseVisualStyleBackColor = true;
            this.NPCScript.Click += new System.EventHandler(this.NPCScript_Click);
            // 
            // MapBuildingInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelBuildingList);
            this.Name = "MapBuildingInspector";
            this.Size = new System.Drawing.Size(296, 420);
            this.Load += new System.EventHandler(this.MapBuildingInspector_Load);
            this.Resize += new System.EventHandler(this.MapBuildingInspector_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).EndInit();
            this.PanelBuildingList.ResumeLayout(false);
            this.PanelBuildingList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelBuildingName;
        private System.Windows.Forms.TextBox BuildingID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NPCID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Level;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox RaceID;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.Panel PanelBuildingList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Button SearchBuildingPrevious;
        private System.Windows.Forms.Button SearchBuildingNext;
        private System.Windows.Forms.TextBox SearchBuildingText;
        private System.Windows.Forms.ListBox ListBuildings;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.Button NPCScript;
    }
}
