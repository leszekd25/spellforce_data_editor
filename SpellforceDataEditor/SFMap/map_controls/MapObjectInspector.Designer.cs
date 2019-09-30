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
            this.LabelObjectName = new System.Windows.Forms.Label();
            this.ObjectID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NPCID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Unknown1 = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.Angle = new System.Windows.Forms.TextBox();
            this.AngleTrackbar = new System.Windows.Forms.TrackBar();
            this.PanelObjectList = new System.Windows.Forms.Panel();
            this.SearchObjectPrevious = new System.Windows.Forms.Button();
            this.SearchObjectNext = new System.Windows.Forms.Button();
            this.SearchObjectText = new System.Windows.Forms.TextBox();
            this.ListObjects = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.PanelProperties = new System.Windows.Forms.Panel();
            this.NPCScript = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).BeginInit();
            this.PanelObjectList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelObjectName
            // 
            this.LabelObjectName.AutoSize = true;
            this.LabelObjectName.Location = new System.Drawing.Point(95, 4);
            this.LabelObjectName.Name = "LabelObjectName";
            this.LabelObjectName.Size = new System.Drawing.Size(0, 13);
            this.LabelObjectName.TabIndex = 0;
            // 
            // ObjectID
            // 
            this.ObjectID.BackColor = System.Drawing.Color.DarkOrange;
            this.ObjectID.Location = new System.Drawing.Point(98, 20);
            this.ObjectID.Name = "ObjectID";
            this.ObjectID.Size = new System.Drawing.Size(100, 20);
            this.ObjectID.TabIndex = 1;
            this.ObjectID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ObjectID_MouseDown);
            this.ObjectID.Validated += new System.EventHandler(this.ObjectID_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Object ID";
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
            // PanelObjectList
            // 
            this.PanelObjectList.Controls.Add(this.SearchObjectPrevious);
            this.PanelObjectList.Controls.Add(this.SearchObjectNext);
            this.PanelObjectList.Controls.Add(this.SearchObjectText);
            this.PanelObjectList.Controls.Add(this.ListObjects);
            this.PanelObjectList.Controls.Add(this.label1);
            this.PanelObjectList.Controls.Add(this.ButtonResizeList);
            this.PanelObjectList.Location = new System.Drawing.Point(3, 203);
            this.PanelObjectList.Name = "PanelObjectList";
            this.PanelObjectList.Size = new System.Drawing.Size(290, 214);
            this.PanelObjectList.TabIndex = 18;
            // 
            // SearchObjectPrevious
            // 
            this.SearchObjectPrevious.Location = new System.Drawing.Point(153, 187);
            this.SearchObjectPrevious.Name = "SearchObjectPrevious";
            this.SearchObjectPrevious.Size = new System.Drawing.Size(134, 23);
            this.SearchObjectPrevious.TabIndex = 23;
            this.SearchObjectPrevious.Text = "Find previous";
            this.SearchObjectPrevious.UseVisualStyleBackColor = true;
            this.SearchObjectPrevious.Click += new System.EventHandler(this.SearchObjectPrevious_Click);
            // 
            // SearchObjectNext
            // 
            this.SearchObjectNext.Location = new System.Drawing.Point(4, 187);
            this.SearchObjectNext.Name = "SearchObjectNext";
            this.SearchObjectNext.Size = new System.Drawing.Size(141, 23);
            this.SearchObjectNext.TabIndex = 22;
            this.SearchObjectNext.Text = "Find next";
            this.SearchObjectNext.UseVisualStyleBackColor = true;
            this.SearchObjectNext.Click += new System.EventHandler(this.SearchObjectNext_Click);
            // 
            // SearchObjectText
            // 
            this.SearchObjectText.Location = new System.Drawing.Point(4, 161);
            this.SearchObjectText.Name = "SearchObjectText";
            this.SearchObjectText.Size = new System.Drawing.Size(283, 20);
            this.SearchObjectText.TabIndex = 21;
            // 
            // ListObjects
            // 
            this.ListObjects.FormattingEnabled = true;
            this.ListObjects.Location = new System.Drawing.Point(4, 32);
            this.ListObjects.Name = "ListObjects";
            this.ListObjects.Size = new System.Drawing.Size(283, 121);
            this.ListObjects.TabIndex = 20;
            this.ListObjects.SelectedIndexChanged += new System.EventHandler(this.ListObjects_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of objects";
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
            this.PanelProperties.Controls.Add(this.LabelObjectName);
            this.PanelProperties.Controls.Add(this.AngleTrackbar);
            this.PanelProperties.Controls.Add(this.ObjectID);
            this.PanelProperties.Controls.Add(this.Angle);
            this.PanelProperties.Controls.Add(this.NPCID);
            this.PanelProperties.Controls.Add(this.PosY);
            this.PanelProperties.Controls.Add(this.label3);
            this.PanelProperties.Controls.Add(this.PosX);
            this.PanelProperties.Controls.Add(this.label4);
            this.PanelProperties.Controls.Add(this.label5);
            this.PanelProperties.Controls.Add(this.Unknown1);
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
            this.NPCScript.TabIndex = 20;
            this.NPCScript.Text = "Open script";
            this.NPCScript.UseVisualStyleBackColor = true;
            this.NPCScript.Click += new System.EventHandler(this.NPCScript_Click);
            // 
            // MapObjectInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelObjectList);
            this.Name = "MapObjectInspector";
            this.Size = new System.Drawing.Size(296, 420);
            this.Load += new System.EventHandler(this.MapObjectInspector_Load);
            this.Resize += new System.EventHandler(this.MapObjectInspector_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.AngleTrackbar)).EndInit();
            this.PanelObjectList.ResumeLayout(false);
            this.PanelObjectList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            this.ResumeLayout(false);

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
