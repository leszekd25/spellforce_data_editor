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
            LabelBuildingName = new System.Windows.Forms.Label();
            BuildingID = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            NPCID = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            PosX = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            Level = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            RaceID = new System.Windows.Forms.TextBox();
            PosY = new System.Windows.Forms.TextBox();
            Angle = new System.Windows.Forms.TextBox();
            AngleTrackbar = new System.Windows.Forms.TrackBar();
            PanelBuildingList = new System.Windows.Forms.Panel();
            SearchBuildingPrevious = new System.Windows.Forms.Button();
            SearchBuildingNext = new System.Windows.Forms.Button();
            SearchBuildingText = new System.Windows.Forms.TextBox();
            ListBuildings = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            ButtonResizeList = new System.Windows.Forms.Button();
            PanelProperties = new System.Windows.Forms.Panel();
            NPCScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).BeginInit();
            PanelBuildingList.SuspendLayout();
            PanelProperties.SuspendLayout();
            SuspendLayout();
            // 
            // LabelBuildingName
            // 
            LabelBuildingName.AutoSize = true;
            LabelBuildingName.Location = new System.Drawing.Point(111, 5);
            LabelBuildingName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelBuildingName.Name = "LabelBuildingName";
            LabelBuildingName.Size = new System.Drawing.Size(0, 15);
            LabelBuildingName.TabIndex = 0;
            // 
            // BuildingID
            // 
            BuildingID.BackColor = System.Drawing.Color.DarkOrange;
            BuildingID.Location = new System.Drawing.Point(114, 23);
            BuildingID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BuildingID.Name = "BuildingID";
            BuildingID.ShortcutsEnabled = false;
            BuildingID.Size = new System.Drawing.Size(116, 23);
            BuildingID.TabIndex = 1;
            BuildingID.MouseDown += BuildingID_MouseDown;
            BuildingID.Validated += BuildingID_Validated;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 27);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 15);
            label2.TabIndex = 2;
            label2.Text = "Building ID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(4, 57);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(45, 15);
            label3.TabIndex = 4;
            label3.Text = "NPC ID";
            // 
            // NPCID
            // 
            NPCID.Location = new System.Drawing.Point(114, 53);
            NPCID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NPCID.Name = "NPCID";
            NPCID.Size = new System.Drawing.Size(116, 23);
            NPCID.TabIndex = 3;
            NPCID.Validated += NPCID_Validated;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 87);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 15);
            label4.TabIndex = 6;
            label4.Text = "Position";
            // 
            // PosX
            // 
            PosX.Enabled = false;
            PosX.Location = new System.Drawing.Point(114, 83);
            PosX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosX.Name = "PosX";
            PosX.Size = new System.Drawing.Size(53, 23);
            PosX.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(4, 117);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(38, 15);
            label5.TabIndex = 8;
            label5.Text = "Angle";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 147);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(34, 15);
            label6.TabIndex = 10;
            label6.Text = "Level";
            // 
            // Level
            // 
            Level.Location = new System.Drawing.Point(114, 143);
            Level.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Level.Name = "Level";
            Level.Size = new System.Drawing.Size(116, 23);
            Level.TabIndex = 9;
            Level.Validated += Level_Validated;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(4, 177);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(46, 15);
            label7.TabIndex = 12;
            label7.Text = "Race ID";
            // 
            // RaceID
            // 
            RaceID.Location = new System.Drawing.Point(114, 173);
            RaceID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RaceID.Name = "RaceID";
            RaceID.Size = new System.Drawing.Size(116, 23);
            RaceID.TabIndex = 11;
            RaceID.Validated += RaceID_Validated;
            // 
            // PosY
            // 
            PosY.Enabled = false;
            PosY.Location = new System.Drawing.Point(177, 83);
            PosY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosY.Name = "PosY";
            PosY.Size = new System.Drawing.Size(53, 23);
            PosY.TabIndex = 15;
            // 
            // Angle
            // 
            Angle.Location = new System.Drawing.Point(241, 113);
            Angle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Angle.Name = "Angle";
            Angle.Size = new System.Drawing.Size(53, 23);
            Angle.TabIndex = 16;
            Angle.Validated += Angle_Validated;
            // 
            // AngleTrackbar
            // 
            AngleTrackbar.AutoSize = false;
            AngleTrackbar.Location = new System.Drawing.Point(114, 113);
            AngleTrackbar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AngleTrackbar.Maximum = 359;
            AngleTrackbar.Name = "AngleTrackbar";
            AngleTrackbar.Size = new System.Drawing.Size(117, 23);
            AngleTrackbar.TabIndex = 17;
            AngleTrackbar.TickFrequency = 30;
            AngleTrackbar.ValueChanged += AngleTrackbar_ValueChanged;
            AngleTrackbar.MouseDown += AngleTrackbar_MouseDown;
            AngleTrackbar.MouseUp += AngleTrackbar_MouseUp;
            // 
            // PanelBuildingList
            // 
            PanelBuildingList.Controls.Add(SearchBuildingPrevious);
            PanelBuildingList.Controls.Add(SearchBuildingNext);
            PanelBuildingList.Controls.Add(SearchBuildingText);
            PanelBuildingList.Controls.Add(ListBuildings);
            PanelBuildingList.Controls.Add(label1);
            PanelBuildingList.Controls.Add(ButtonResizeList);
            PanelBuildingList.Location = new System.Drawing.Point(4, 234);
            PanelBuildingList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelBuildingList.Name = "PanelBuildingList";
            PanelBuildingList.Size = new System.Drawing.Size(338, 247);
            PanelBuildingList.TabIndex = 18;
            // 
            // SearchBuildingPrevious
            // 
            SearchBuildingPrevious.Location = new System.Drawing.Point(178, 216);
            SearchBuildingPrevious.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchBuildingPrevious.Name = "SearchBuildingPrevious";
            SearchBuildingPrevious.Size = new System.Drawing.Size(156, 27);
            SearchBuildingPrevious.TabIndex = 23;
            SearchBuildingPrevious.Text = "Find previous";
            SearchBuildingPrevious.UseVisualStyleBackColor = true;
            SearchBuildingPrevious.Click += SearchBuildingPrevious_Click;
            // 
            // SearchBuildingNext
            // 
            SearchBuildingNext.Location = new System.Drawing.Point(5, 216);
            SearchBuildingNext.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchBuildingNext.Name = "SearchBuildingNext";
            SearchBuildingNext.Size = new System.Drawing.Size(164, 27);
            SearchBuildingNext.TabIndex = 22;
            SearchBuildingNext.Text = "Find next";
            SearchBuildingNext.UseVisualStyleBackColor = true;
            SearchBuildingNext.Click += SearchBuildingNext_Click;
            // 
            // SearchBuildingText
            // 
            SearchBuildingText.Location = new System.Drawing.Point(5, 186);
            SearchBuildingText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchBuildingText.Name = "SearchBuildingText";
            SearchBuildingText.Size = new System.Drawing.Size(330, 23);
            SearchBuildingText.TabIndex = 21;
            // 
            // ListBuildings
            // 
            ListBuildings.FormattingEnabled = true;
            ListBuildings.ItemHeight = 15;
            ListBuildings.Location = new System.Drawing.Point(5, 37);
            ListBuildings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListBuildings.Name = "ListBuildings";
            ListBuildings.Size = new System.Drawing.Size(330, 139);
            ListBuildings.TabIndex = 20;
            ListBuildings.SelectedIndexChanged += ListBuildings_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(91, 15);
            label1.TabIndex = 19;
            label1.Text = "List of buildings";
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
            // PanelProperties
            // 
            PanelProperties.Controls.Add(NPCScript);
            PanelProperties.Controls.Add(label2);
            PanelProperties.Controls.Add(LabelBuildingName);
            PanelProperties.Controls.Add(AngleTrackbar);
            PanelProperties.Controls.Add(BuildingID);
            PanelProperties.Controls.Add(Angle);
            PanelProperties.Controls.Add(NPCID);
            PanelProperties.Controls.Add(PosY);
            PanelProperties.Controls.Add(label3);
            PanelProperties.Controls.Add(PosX);
            PanelProperties.Controls.Add(label4);
            PanelProperties.Controls.Add(label7);
            PanelProperties.Controls.Add(label5);
            PanelProperties.Controls.Add(RaceID);
            PanelProperties.Controls.Add(Level);
            PanelProperties.Controls.Add(label6);
            PanelProperties.Enabled = false;
            PanelProperties.Location = new System.Drawing.Point(4, 0);
            PanelProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelProperties.Name = "PanelProperties";
            PanelProperties.Size = new System.Drawing.Size(338, 232);
            PanelProperties.TabIndex = 19;
            // 
            // NPCScript
            // 
            NPCScript.Location = new System.Drawing.Point(238, 51);
            NPCScript.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NPCScript.Name = "NPCScript";
            NPCScript.Size = new System.Drawing.Size(97, 27);
            NPCScript.TabIndex = 19;
            NPCScript.Text = "Open script";
            NPCScript.UseVisualStyleBackColor = true;
            NPCScript.Click += NPCScript_Click;
            // 
            // MapBuildingInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(PanelProperties);
            Controls.Add(PanelBuildingList);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapBuildingInspector";
            Size = new System.Drawing.Size(345, 485);
            Load += MapBuildingInspector_Load;
            Resize += MapBuildingInspector_Resize;
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).EndInit();
            PanelBuildingList.ResumeLayout(false);
            PanelBuildingList.PerformLayout();
            PanelProperties.ResumeLayout(false);
            PanelProperties.PerformLayout();
            ResumeLayout(false);
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
