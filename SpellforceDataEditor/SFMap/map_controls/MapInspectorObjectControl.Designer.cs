namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorObjectControl
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
            this.ObjectListReset = new System.Windows.Forms.Button();
            this.ObjectListSearchPhrase = new System.Windows.Forms.TextBox();
            this.ObjectListSearch = new System.Windows.Forms.Button();
            this.ListObjects = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SelectedObjectPanel = new System.Windows.Forms.Panel();
            this.SelectedObjectName = new System.Windows.Forms.Label();
            this.SelectedObjectAngle = new System.Windows.Forms.TextBox();
            this.SelectedObjectAngleTrackBar = new System.Windows.Forms.TrackBar();
            this.SelectedObjectUnk1 = new System.Windows.Forms.TextBox();
            this.SelectedObjectY = new System.Windows.Forms.TextBox();
            this.SelectedObjectX = new System.Windows.Forms.TextBox();
            this.SelectedObjectNPCID = new System.Windows.Forms.TextBox();
            this.SelectedObjectID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ObjectToPlaceName = new System.Windows.Forms.Label();
            this.ObjectToPlaceID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectedObjectPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedObjectAngleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ObjectListReset
            // 
            this.ObjectListReset.Location = new System.Drawing.Point(251, 488);
            this.ObjectListReset.Name = "ObjectListReset";
            this.ObjectListReset.Size = new System.Drawing.Size(147, 23);
            this.ObjectListReset.TabIndex = 29;
            this.ObjectListReset.Text = "Reset";
            this.ObjectListReset.UseVisualStyleBackColor = true;
            // 
            // ObjectListSearchPhrase
            // 
            this.ObjectListSearchPhrase.Location = new System.Drawing.Point(251, 286);
            this.ObjectListSearchPhrase.Name = "ObjectListSearchPhrase";
            this.ObjectListSearchPhrase.Size = new System.Drawing.Size(147, 20);
            this.ObjectListSearchPhrase.TabIndex = 28;
            // 
            // ObjectListSearch
            // 
            this.ObjectListSearch.Location = new System.Drawing.Point(251, 312);
            this.ObjectListSearch.Name = "ObjectListSearch";
            this.ObjectListSearch.Size = new System.Drawing.Size(147, 23);
            this.ObjectListSearch.TabIndex = 27;
            this.ObjectListSearch.Text = "Search";
            this.ObjectListSearch.UseVisualStyleBackColor = true;
            // 
            // ListObjects
            // 
            this.ListObjects.FormattingEnabled = true;
            this.ListObjects.Location = new System.Drawing.Point(22, 286);
            this.ListObjects.Name = "ListObjects";
            this.ListObjects.Size = new System.Drawing.Size(223, 225);
            this.ListObjects.TabIndex = 26;
            this.ListObjects.SelectedIndexChanged += new System.EventHandler(this.ListObjects_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 270);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "List of objects:";
            // 
            // SelectedObjectPanel
            // 
            this.SelectedObjectPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectName);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectAngle);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectAngleTrackBar);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectUnk1);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectY);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectX);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectNPCID);
            this.SelectedObjectPanel.Controls.Add(this.SelectedObjectID);
            this.SelectedObjectPanel.Controls.Add(this.label7);
            this.SelectedObjectPanel.Controls.Add(this.label5);
            this.SelectedObjectPanel.Controls.Add(this.label4);
            this.SelectedObjectPanel.Controls.Add(this.label3);
            this.SelectedObjectPanel.Controls.Add(this.labelX);
            this.SelectedObjectPanel.Location = new System.Drawing.Point(14, 67);
            this.SelectedObjectPanel.Name = "SelectedObjectPanel";
            this.SelectedObjectPanel.Size = new System.Drawing.Size(384, 142);
            this.SelectedObjectPanel.TabIndex = 24;
            // 
            // SelectedObjectName
            // 
            this.SelectedObjectName.AutoSize = true;
            this.SelectedObjectName.Location = new System.Drawing.Point(203, 10);
            this.SelectedObjectName.Name = "SelectedObjectName";
            this.SelectedObjectName.Size = new System.Drawing.Size(0, 13);
            this.SelectedObjectName.TabIndex = 10;
            // 
            // SelectedObjectAngle
            // 
            this.SelectedObjectAngle.Location = new System.Drawing.Point(206, 85);
            this.SelectedObjectAngle.Name = "SelectedObjectAngle";
            this.SelectedObjectAngle.Size = new System.Drawing.Size(35, 20);
            this.SelectedObjectAngle.TabIndex = 21;
            this.SelectedObjectAngle.Validated += new System.EventHandler(this.SelectedObjectAngle_Validated);
            // 
            // SelectedObjectAngleTrackBar
            // 
            this.SelectedObjectAngleTrackBar.AutoSize = false;
            this.SelectedObjectAngleTrackBar.LargeChange = 45;
            this.SelectedObjectAngleTrackBar.Location = new System.Drawing.Point(97, 85);
            this.SelectedObjectAngleTrackBar.Maximum = 359;
            this.SelectedObjectAngleTrackBar.Name = "SelectedObjectAngleTrackBar";
            this.SelectedObjectAngleTrackBar.Size = new System.Drawing.Size(100, 20);
            this.SelectedObjectAngleTrackBar.SmallChange = 5;
            this.SelectedObjectAngleTrackBar.TabIndex = 20;
            this.SelectedObjectAngleTrackBar.TickFrequency = 45;
            this.SelectedObjectAngleTrackBar.ValueChanged += new System.EventHandler(this.SelectedObjectAngleTrackBar_ValueChanged);
            // 
            // SelectedObjectUnk1
            // 
            this.SelectedObjectUnk1.Location = new System.Drawing.Point(97, 111);
            this.SelectedObjectUnk1.Name = "SelectedObjectUnk1";
            this.SelectedObjectUnk1.Size = new System.Drawing.Size(100, 20);
            this.SelectedObjectUnk1.TabIndex = 18;
            this.SelectedObjectUnk1.Validated += new System.EventHandler(this.SelectedObjectUnk1_Validated);
            // 
            // SelectedObjectY
            // 
            this.SelectedObjectY.Enabled = false;
            this.SelectedObjectY.Location = new System.Drawing.Point(161, 59);
            this.SelectedObjectY.Name = "SelectedObjectY";
            this.SelectedObjectY.Size = new System.Drawing.Size(36, 20);
            this.SelectedObjectY.TabIndex = 15;
            // 
            // SelectedObjectX
            // 
            this.SelectedObjectX.Enabled = false;
            this.SelectedObjectX.Location = new System.Drawing.Point(97, 59);
            this.SelectedObjectX.Name = "SelectedObjectX";
            this.SelectedObjectX.Size = new System.Drawing.Size(35, 20);
            this.SelectedObjectX.TabIndex = 14;
            // 
            // SelectedObjectNPCID
            // 
            this.SelectedObjectNPCID.BackColor = System.Drawing.Color.LightSteelBlue;
            this.SelectedObjectNPCID.Location = new System.Drawing.Point(97, 33);
            this.SelectedObjectNPCID.Name = "SelectedObjectNPCID";
            this.SelectedObjectNPCID.Size = new System.Drawing.Size(100, 20);
            this.SelectedObjectNPCID.TabIndex = 13;
            this.SelectedObjectNPCID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedObjectNPCID_MouseDown);
            // 
            // SelectedObjectID
            // 
            this.SelectedObjectID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedObjectID.Location = new System.Drawing.Point(97, 7);
            this.SelectedObjectID.Name = "SelectedObjectID";
            this.SelectedObjectID.Size = new System.Drawing.Size(100, 20);
            this.SelectedObjectID.TabIndex = 12;
            this.SelectedObjectID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedObjectID_MouseDown);
            this.SelectedObjectID.Validated += new System.EventHandler(this.SelectedObjectID_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Unknown";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Angle";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "NPC ID";
            // 
            // labelX
            // 
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(4, 10);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(52, 13);
            this.labelX.TabIndex = 5;
            this.labelX.Text = "Object ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Currently selected object info:";
            // 
            // ObjectToPlaceName
            // 
            this.ObjectToPlaceName.AutoSize = true;
            this.ObjectToPlaceName.Location = new System.Drawing.Point(218, 16);
            this.ObjectToPlaceName.Name = "ObjectToPlaceName";
            this.ObjectToPlaceName.Size = new System.Drawing.Size(0, 13);
            this.ObjectToPlaceName.TabIndex = 22;
            // 
            // ObjectToPlaceID
            // 
            this.ObjectToPlaceID.BackColor = System.Drawing.Color.DarkOrange;
            this.ObjectToPlaceID.Location = new System.Drawing.Point(112, 13);
            this.ObjectToPlaceID.Name = "ObjectToPlaceID";
            this.ObjectToPlaceID.Size = new System.Drawing.Size(100, 20);
            this.ObjectToPlaceID.TabIndex = 21;
            this.ObjectToPlaceID.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Object to place:";
            // 
            // MapInspectorObjectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ObjectListReset);
            this.Controls.Add(this.ObjectListSearchPhrase);
            this.Controls.Add(this.ObjectListSearch);
            this.Controls.Add(this.ListObjects);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.SelectedObjectPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ObjectToPlaceName);
            this.Controls.Add(this.ObjectToPlaceID);
            this.Controls.Add(this.label1);
            this.Name = "MapInspectorObjectControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorObjectControl_VisibleChanged);
            this.SelectedObjectPanel.ResumeLayout(false);
            this.SelectedObjectPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedObjectAngleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ObjectListReset;
        private System.Windows.Forms.TextBox ObjectListSearchPhrase;
        private System.Windows.Forms.Button ObjectListSearch;
        private System.Windows.Forms.ListBox ListObjects;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel SelectedObjectPanel;
        private System.Windows.Forms.Label SelectedObjectName;
        private System.Windows.Forms.TextBox SelectedObjectAngle;
        private System.Windows.Forms.TrackBar SelectedObjectAngleTrackBar;
        private System.Windows.Forms.TextBox SelectedObjectUnk1;
        private System.Windows.Forms.TextBox SelectedObjectY;
        private System.Windows.Forms.TextBox SelectedObjectX;
        private System.Windows.Forms.TextBox SelectedObjectNPCID;
        private System.Windows.Forms.TextBox SelectedObjectID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label ObjectToPlaceName;
        private System.Windows.Forms.TextBox ObjectToPlaceID;
        private System.Windows.Forms.Label label1;
    }
}
