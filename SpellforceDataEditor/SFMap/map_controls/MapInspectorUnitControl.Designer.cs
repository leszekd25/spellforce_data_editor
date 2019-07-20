namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorUnitControl
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
            this.UnitToPlaceID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectedUnitPanel = new System.Windows.Forms.Panel();
            this.SelectedUnitNameAndLevel = new System.Windows.Forms.Label();
            this.SelectedUnitAngle = new System.Windows.Forms.TextBox();
            this.SelectedUnitAngleTrackBar = new System.Windows.Forms.TrackBar();
            this.SelectedUnitUnk2 = new System.Windows.Forms.TextBox();
            this.SelectedUnitGroup = new System.Windows.Forms.TextBox();
            this.SelectedUnitUnk1 = new System.Windows.Forms.TextBox();
            this.SelectedUnitY = new System.Windows.Forms.TextBox();
            this.SelectedUnitX = new System.Windows.Forms.TextBox();
            this.SelectedUnitNPCID = new System.Windows.Forms.TextBox();
            this.SelectedUnitID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ListUnits = new System.Windows.Forms.ListBox();
            this.UnitListFindNext = new System.Windows.Forms.Button();
            this.UnitListSearchPhrase = new System.Windows.Forms.TextBox();
            this.UnitListFindPrevious = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ComboRaces = new System.Windows.Forms.ComboBox();
            this.ComboUnit = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectedUnitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedUnitAngleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // UnitToPlaceID
            // 
            this.UnitToPlaceID.BackColor = System.Drawing.Color.DarkOrange;
            this.UnitToPlaceID.Location = new System.Drawing.Point(93, 13);
            this.UnitToPlaceID.Name = "UnitToPlaceID";
            this.UnitToPlaceID.Size = new System.Drawing.Size(100, 20);
            this.UnitToPlaceID.TabIndex = 1;
            this.UnitToPlaceID.Text = "0";
            this.UnitToPlaceID.TextChanged += new System.EventHandler(this.UnitToPlaceID_TextChanged);
            this.UnitToPlaceID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UnitToPlaceID_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Currently selected unit info:";
            // 
            // SelectedUnitPanel
            // 
            this.SelectedUnitPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitNameAndLevel);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitAngle);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitAngleTrackBar);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitUnk2);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitGroup);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitUnk1);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitY);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitX);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitNPCID);
            this.SelectedUnitPanel.Controls.Add(this.SelectedUnitID);
            this.SelectedUnitPanel.Controls.Add(this.label8);
            this.SelectedUnitPanel.Controls.Add(this.label7);
            this.SelectedUnitPanel.Controls.Add(this.label6);
            this.SelectedUnitPanel.Controls.Add(this.label5);
            this.SelectedUnitPanel.Controls.Add(this.label4);
            this.SelectedUnitPanel.Controls.Add(this.label3);
            this.SelectedUnitPanel.Controls.Add(this.labelX);
            this.SelectedUnitPanel.Location = new System.Drawing.Point(12, 106);
            this.SelectedUnitPanel.Name = "SelectedUnitPanel";
            this.SelectedUnitPanel.Size = new System.Drawing.Size(384, 191);
            this.SelectedUnitPanel.TabIndex = 4;
            // 
            // SelectedUnitNameAndLevel
            // 
            this.SelectedUnitNameAndLevel.AutoSize = true;
            this.SelectedUnitNameAndLevel.Location = new System.Drawing.Point(186, 10);
            this.SelectedUnitNameAndLevel.Name = "SelectedUnitNameAndLevel";
            this.SelectedUnitNameAndLevel.Size = new System.Drawing.Size(0, 13);
            this.SelectedUnitNameAndLevel.TabIndex = 10;
            // 
            // SelectedUnitAngle
            // 
            this.SelectedUnitAngle.Location = new System.Drawing.Point(189, 85);
            this.SelectedUnitAngle.Name = "SelectedUnitAngle";
            this.SelectedUnitAngle.Size = new System.Drawing.Size(35, 20);
            this.SelectedUnitAngle.TabIndex = 21;
            this.SelectedUnitAngle.Validated += new System.EventHandler(this.SelectedUnitAngle_Validated);
            // 
            // SelectedUnitAngleTrackBar
            // 
            this.SelectedUnitAngleTrackBar.AutoSize = false;
            this.SelectedUnitAngleTrackBar.LargeChange = 45;
            this.SelectedUnitAngleTrackBar.Location = new System.Drawing.Point(80, 85);
            this.SelectedUnitAngleTrackBar.Maximum = 359;
            this.SelectedUnitAngleTrackBar.Name = "SelectedUnitAngleTrackBar";
            this.SelectedUnitAngleTrackBar.Size = new System.Drawing.Size(100, 20);
            this.SelectedUnitAngleTrackBar.SmallChange = 5;
            this.SelectedUnitAngleTrackBar.TabIndex = 20;
            this.SelectedUnitAngleTrackBar.TickFrequency = 45;
            this.SelectedUnitAngleTrackBar.ValueChanged += new System.EventHandler(this.SelectedUnitAngleTrackBar_ValueChanged);
            // 
            // SelectedUnitUnk2
            // 
            this.SelectedUnitUnk2.Location = new System.Drawing.Point(80, 162);
            this.SelectedUnitUnk2.Name = "SelectedUnitUnk2";
            this.SelectedUnitUnk2.Size = new System.Drawing.Size(35, 20);
            this.SelectedUnitUnk2.TabIndex = 19;
            this.SelectedUnitUnk2.Validated += new System.EventHandler(this.SelectedUnitUnk2_Validated);
            // 
            // SelectedUnitGroup
            // 
            this.SelectedUnitGroup.Location = new System.Drawing.Point(80, 136);
            this.SelectedUnitGroup.Name = "SelectedUnitGroup";
            this.SelectedUnitGroup.Size = new System.Drawing.Size(35, 20);
            this.SelectedUnitGroup.TabIndex = 18;
            this.SelectedUnitGroup.Validated += new System.EventHandler(this.SelectedUnitGroup_Validated);
            // 
            // SelectedUnitUnk1
            // 
            this.SelectedUnitUnk1.Location = new System.Drawing.Point(80, 111);
            this.SelectedUnitUnk1.Name = "SelectedUnitUnk1";
            this.SelectedUnitUnk1.Size = new System.Drawing.Size(100, 20);
            this.SelectedUnitUnk1.TabIndex = 17;
            this.SelectedUnitUnk1.Validated += new System.EventHandler(this.SelectedUnitUnk1_Validated);
            // 
            // SelectedUnitY
            // 
            this.SelectedUnitY.Enabled = false;
            this.SelectedUnitY.Location = new System.Drawing.Point(144, 59);
            this.SelectedUnitY.Name = "SelectedUnitY";
            this.SelectedUnitY.Size = new System.Drawing.Size(36, 20);
            this.SelectedUnitY.TabIndex = 15;
            // 
            // SelectedUnitX
            // 
            this.SelectedUnitX.Enabled = false;
            this.SelectedUnitX.Location = new System.Drawing.Point(80, 59);
            this.SelectedUnitX.Name = "SelectedUnitX";
            this.SelectedUnitX.Size = new System.Drawing.Size(35, 20);
            this.SelectedUnitX.TabIndex = 14;
            // 
            // SelectedUnitNPCID
            // 
            this.SelectedUnitNPCID.BackColor = System.Drawing.Color.LightSteelBlue;
            this.SelectedUnitNPCID.Location = new System.Drawing.Point(80, 33);
            this.SelectedUnitNPCID.Name = "SelectedUnitNPCID";
            this.SelectedUnitNPCID.Size = new System.Drawing.Size(100, 20);
            this.SelectedUnitNPCID.TabIndex = 13;
            this.SelectedUnitNPCID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedUnitNPCID_MouseDown);
            // 
            // SelectedUnitID
            // 
            this.SelectedUnitID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedUnitID.Location = new System.Drawing.Point(80, 7);
            this.SelectedUnitID.Name = "SelectedUnitID";
            this.SelectedUnitID.Size = new System.Drawing.Size(100, 20);
            this.SelectedUnitID.TabIndex = 12;
            this.SelectedUnitID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedUnitID_MouseDown);
            this.SelectedUnitID.Validated += new System.EventHandler(this.SelectedUnitID_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Unknown 2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Group";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Unknown";
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
            this.labelX.Size = new System.Drawing.Size(40, 13);
            this.labelX.TabIndex = 5;
            this.labelX.Text = "Unit ID";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 309);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "List of units:";
            // 
            // ListUnits
            // 
            this.ListUnits.FormattingEnabled = true;
            this.ListUnits.Location = new System.Drawing.Point(20, 325);
            this.ListUnits.Name = "ListUnits";
            this.ListUnits.Size = new System.Drawing.Size(223, 225);
            this.ListUnits.TabIndex = 6;
            this.ListUnits.SelectedIndexChanged += new System.EventHandler(this.ListUnits_SelectedIndexChanged);
            // 
            // UnitListFindNext
            // 
            this.UnitListFindNext.Location = new System.Drawing.Point(249, 351);
            this.UnitListFindNext.Name = "UnitListFindNext";
            this.UnitListFindNext.Size = new System.Drawing.Size(147, 23);
            this.UnitListFindNext.TabIndex = 7;
            this.UnitListFindNext.Text = "Find next";
            this.UnitListFindNext.UseVisualStyleBackColor = true;
            this.UnitListFindNext.Click += new System.EventHandler(this.UnitListFindNext_Click);
            // 
            // UnitListSearchPhrase
            // 
            this.UnitListSearchPhrase.Location = new System.Drawing.Point(249, 325);
            this.UnitListSearchPhrase.Name = "UnitListSearchPhrase";
            this.UnitListSearchPhrase.Size = new System.Drawing.Size(147, 20);
            this.UnitListSearchPhrase.TabIndex = 8;
            // 
            // UnitListFindPrevious
            // 
            this.UnitListFindPrevious.Location = new System.Drawing.Point(249, 380);
            this.UnitListFindPrevious.Name = "UnitListFindPrevious";
            this.UnitListFindPrevious.Size = new System.Drawing.Size(147, 23);
            this.UnitListFindPrevious.TabIndex = 9;
            this.UnitListFindPrevious.Text = "Find previous";
            this.UnitListFindPrevious.UseVisualStyleBackColor = true;
            this.UnitListFindPrevious.Click += new System.EventHandler(this.UnitListFindPrevious_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Race";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(17, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Unit";
            // 
            // ComboRaces
            // 
            this.ComboRaces.FormattingEnabled = true;
            this.ComboRaces.Location = new System.Drawing.Point(93, 39);
            this.ComboRaces.Name = "ComboRaces";
            this.ComboRaces.Size = new System.Drawing.Size(195, 21);
            this.ComboRaces.TabIndex = 12;
            this.ComboRaces.SelectedIndexChanged += new System.EventHandler(this.ComboRaces_SelectedIndexChanged);
            // 
            // ComboUnit
            // 
            this.ComboUnit.FormattingEnabled = true;
            this.ComboUnit.Location = new System.Drawing.Point(93, 66);
            this.ComboUnit.Name = "ComboUnit";
            this.ComboUnit.Size = new System.Drawing.Size(303, 21);
            this.ComboUnit.TabIndex = 13;
            this.ComboUnit.SelectedIndexChanged += new System.EventHandler(this.ComboUnit_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unit to place:";
            // 
            // MapInspectorUnitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ComboUnit);
            this.Controls.Add(this.ComboRaces);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.UnitListFindPrevious);
            this.Controls.Add(this.UnitListSearchPhrase);
            this.Controls.Add(this.UnitListFindNext);
            this.Controls.Add(this.ListUnits);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.SelectedUnitPanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UnitToPlaceID);
            this.Controls.Add(this.label1);
            this.Name = "MapInspectorUnitControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorUnitControl_VisibleChanged);
            this.SelectedUnitPanel.ResumeLayout(false);
            this.SelectedUnitPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectedUnitAngleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox UnitToPlaceID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel SelectedUnitPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.TextBox SelectedUnitUnk2;
        private System.Windows.Forms.TextBox SelectedUnitGroup;
        private System.Windows.Forms.TextBox SelectedUnitUnk1;
        private System.Windows.Forms.TextBox SelectedUnitY;
        private System.Windows.Forms.TextBox SelectedUnitX;
        private System.Windows.Forms.TextBox SelectedUnitNPCID;
        private System.Windows.Forms.TextBox SelectedUnitID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SelectedUnitAngle;
        private System.Windows.Forms.TrackBar SelectedUnitAngleTrackBar;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox ListUnits;
        private System.Windows.Forms.Button UnitListFindNext;
        private System.Windows.Forms.TextBox UnitListSearchPhrase;
        private System.Windows.Forms.Button UnitListFindPrevious;
        private System.Windows.Forms.Label SelectedUnitNameAndLevel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox ComboRaces;
        private System.Windows.Forms.ComboBox ComboUnit;
        private System.Windows.Forms.Label label1;
    }
}
