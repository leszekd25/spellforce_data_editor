namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapObjectInspector
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
            LabelObjectName = new System.Windows.Forms.Label();
            ObjectID = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            NPCID = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            PosX = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            Unknown1 = new System.Windows.Forms.TextBox();
            PosY = new System.Windows.Forms.TextBox();
            Angle = new System.Windows.Forms.TextBox();
            AngleTrackbar = new System.Windows.Forms.TrackBar();
            PanelObjectList = new System.Windows.Forms.Panel();
            SearchObjectPrevious = new System.Windows.Forms.Button();
            SearchObjectNext = new System.Windows.Forms.Button();
            SearchObjectText = new System.Windows.Forms.TextBox();
            ListObjects = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            ButtonResizeList = new System.Windows.Forms.Button();
            PanelProperties = new System.Windows.Forms.Panel();
            NPCScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).BeginInit();
            PanelObjectList.SuspendLayout();
            PanelProperties.SuspendLayout();
            SuspendLayout();
            // 
            // LabelObjectName
            // 
            LabelObjectName.AutoSize = true;
            LabelObjectName.Location = new System.Drawing.Point(111, 5);
            LabelObjectName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelObjectName.Name = "LabelObjectName";
            LabelObjectName.Size = new System.Drawing.Size(0, 15);
            LabelObjectName.TabIndex = 0;
            // 
            // ObjectID
            // 
            ObjectID.BackColor = System.Drawing.Color.DarkOrange;
            ObjectID.Location = new System.Drawing.Point(114, 23);
            ObjectID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ObjectID.Name = "ObjectID";
            ObjectID.ShortcutsEnabled = false;
            ObjectID.Size = new System.Drawing.Size(116, 23);
            ObjectID.TabIndex = 1;
            ObjectID.MouseDown += ObjectID_MouseDown;
            ObjectID.Validated += ObjectID_Validated;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 27);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(56, 15);
            label2.TabIndex = 2;
            label2.Text = "Object ID";
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
            label6.Size = new System.Drawing.Size(58, 15);
            label6.TabIndex = 10;
            label6.Text = "Unknown";
            // 
            // Unknown1
            // 
            Unknown1.Location = new System.Drawing.Point(114, 143);
            Unknown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Unknown1.Name = "Unknown1";
            Unknown1.Size = new System.Drawing.Size(116, 23);
            Unknown1.TabIndex = 9;
            Unknown1.Validated += Unknown1_Validated;
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
            // PanelObjectList
            // 
            PanelObjectList.Controls.Add(SearchObjectPrevious);
            PanelObjectList.Controls.Add(SearchObjectNext);
            PanelObjectList.Controls.Add(SearchObjectText);
            PanelObjectList.Controls.Add(ListObjects);
            PanelObjectList.Controls.Add(label1);
            PanelObjectList.Controls.Add(ButtonResizeList);
            PanelObjectList.Location = new System.Drawing.Point(4, 234);
            PanelObjectList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelObjectList.Name = "PanelObjectList";
            PanelObjectList.Size = new System.Drawing.Size(338, 247);
            PanelObjectList.TabIndex = 18;
            // 
            // SearchObjectPrevious
            // 
            SearchObjectPrevious.Location = new System.Drawing.Point(178, 216);
            SearchObjectPrevious.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchObjectPrevious.Name = "SearchObjectPrevious";
            SearchObjectPrevious.Size = new System.Drawing.Size(156, 27);
            SearchObjectPrevious.TabIndex = 23;
            SearchObjectPrevious.Text = "Find previous";
            SearchObjectPrevious.UseVisualStyleBackColor = true;
            SearchObjectPrevious.Click += SearchObjectPrevious_Click;
            // 
            // SearchObjectNext
            // 
            SearchObjectNext.Location = new System.Drawing.Point(5, 216);
            SearchObjectNext.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchObjectNext.Name = "SearchObjectNext";
            SearchObjectNext.Size = new System.Drawing.Size(164, 27);
            SearchObjectNext.TabIndex = 22;
            SearchObjectNext.Text = "Find next";
            SearchObjectNext.UseVisualStyleBackColor = true;
            SearchObjectNext.Click += SearchObjectNext_Click;
            // 
            // SearchObjectText
            // 
            SearchObjectText.Location = new System.Drawing.Point(5, 186);
            SearchObjectText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchObjectText.Name = "SearchObjectText";
            SearchObjectText.Size = new System.Drawing.Size(330, 23);
            SearchObjectText.TabIndex = 21;
            // 
            // ListObjects
            // 
            ListObjects.FormattingEnabled = true;
            ListObjects.ItemHeight = 15;
            ListObjects.Location = new System.Drawing.Point(5, 37);
            ListObjects.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListObjects.Name = "ListObjects";
            ListObjects.Size = new System.Drawing.Size(330, 139);
            ListObjects.TabIndex = 20;
            ListObjects.SelectedIndexChanged += ListObjects_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(80, 15);
            label1.TabIndex = 19;
            label1.Text = "List of objects";
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
            PanelProperties.Controls.Add(LabelObjectName);
            PanelProperties.Controls.Add(AngleTrackbar);
            PanelProperties.Controls.Add(ObjectID);
            PanelProperties.Controls.Add(Angle);
            PanelProperties.Controls.Add(NPCID);
            PanelProperties.Controls.Add(PosY);
            PanelProperties.Controls.Add(label3);
            PanelProperties.Controls.Add(PosX);
            PanelProperties.Controls.Add(label4);
            PanelProperties.Controls.Add(label5);
            PanelProperties.Controls.Add(Unknown1);
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
            NPCScript.TabIndex = 20;
            NPCScript.Text = "Open script";
            NPCScript.UseVisualStyleBackColor = true;
            NPCScript.Click += NPCScript_Click;
            // 
            // MapObjectInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(PanelProperties);
            Controls.Add(PanelObjectList);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapObjectInspector";
            Size = new System.Drawing.Size(345, 485);
            Load += MapObjectInspector_Load;
            Resize += MapObjectInspector_Resize;
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).EndInit();
            PanelObjectList.ResumeLayout(false);
            PanelObjectList.PerformLayout();
            PanelProperties.ResumeLayout(false);
            PanelProperties.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label LabelObjectName;
        private System.Windows.Forms.TextBox ObjectID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox NPCID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Unknown1;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.Panel PanelObjectList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Button SearchObjectPrevious;
        private System.Windows.Forms.Button SearchObjectNext;
        private System.Windows.Forms.TextBox SearchObjectText;
        private System.Windows.Forms.ListBox ListObjects;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.Button NPCScript;
    }
}
