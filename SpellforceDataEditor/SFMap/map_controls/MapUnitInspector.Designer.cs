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
            LabelUnitName = new System.Windows.Forms.Label();
            UnitID = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            NPCID = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            PosX = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            Unknown1 = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            Group = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            Unknown2 = new System.Windows.Forms.TextBox();
            PosY = new System.Windows.Forms.TextBox();
            Flags = new System.Windows.Forms.TextBox();
            PanelUnitList = new System.Windows.Forms.Panel();
            SearchUnitPrevious = new System.Windows.Forms.Button();
            SearchUnitNext = new System.Windows.Forms.Button();
            SearchUnitText = new System.Windows.Forms.TextBox();
            ListUnits = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            ButtonResizeList = new System.Windows.Forms.Button();
            PanelProperties = new System.Windows.Forms.Panel();
            NPCScript = new System.Windows.Forms.Button();
            PanelUnitList.SuspendLayout();
            PanelProperties.SuspendLayout();
            SuspendLayout();
            // 
            // LabelUnitName
            // 
            LabelUnitName.AutoSize = true;
            LabelUnitName.Location = new System.Drawing.Point(111, 5);
            LabelUnitName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelUnitName.Name = "LabelUnitName";
            LabelUnitName.Size = new System.Drawing.Size(0, 15);
            LabelUnitName.TabIndex = 0;
            // 
            // UnitID
            // 
            UnitID.BackColor = System.Drawing.Color.DarkOrange;
            UnitID.Location = new System.Drawing.Point(115, 23);
            UnitID.Margin = new System.Windows.Forms.Padding(4);
            UnitID.Name = "UnitID";
            UnitID.ShortcutsEnabled = false;
            UnitID.Size = new System.Drawing.Size(116, 23);
            UnitID.TabIndex = 1;
            UnitID.MouseDown += UnitID_MouseDown;
            UnitID.Validated += UnitID_Validated;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 26);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 15);
            label2.TabIndex = 2;
            label2.Text = "Unit ID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(4, 56);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(45, 15);
            label3.TabIndex = 4;
            label3.Text = "NPC ID";
            // 
            // NPCID
            // 
            NPCID.Location = new System.Drawing.Point(115, 53);
            NPCID.Margin = new System.Windows.Forms.Padding(4);
            NPCID.Name = "NPCID";
            NPCID.Size = new System.Drawing.Size(116, 23);
            NPCID.TabIndex = 3;
            NPCID.Validated += NPCID_Validated;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 86);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 15);
            label4.TabIndex = 6;
            label4.Text = "Position";
            // 
            // PosX
            // 
            PosX.Enabled = false;
            PosX.Location = new System.Drawing.Point(115, 83);
            PosX.Margin = new System.Windows.Forms.Padding(4);
            PosX.Name = "PosX";
            PosX.Size = new System.Drawing.Size(53, 23);
            PosX.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(4, 116);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(34, 15);
            label5.TabIndex = 8;
            label5.Text = "Flags";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 146);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(58, 15);
            label6.TabIndex = 10;
            label6.Text = "Unknown";
            // 
            // Unknown1
            // 
            Unknown1.Location = new System.Drawing.Point(115, 143);
            Unknown1.Margin = new System.Windows.Forms.Padding(4);
            Unknown1.Name = "Unknown1";
            Unknown1.Size = new System.Drawing.Size(116, 23);
            Unknown1.TabIndex = 9;
            Unknown1.Validated += Unknown1_Validated;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(4, 176);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(40, 15);
            label7.TabIndex = 12;
            label7.Text = "Group";
            // 
            // Group
            // 
            Group.Location = new System.Drawing.Point(115, 173);
            Group.Margin = new System.Windows.Forms.Padding(4);
            Group.Name = "Group";
            Group.Size = new System.Drawing.Size(116, 23);
            Group.TabIndex = 11;
            Group.Validated += Group_Validated;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(4, 206);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(67, 15);
            label8.TabIndex = 14;
            label8.Text = "Unknown 2";
            // 
            // Unknown2
            // 
            Unknown2.Location = new System.Drawing.Point(115, 203);
            Unknown2.Margin = new System.Windows.Forms.Padding(4);
            Unknown2.Name = "Unknown2";
            Unknown2.Size = new System.Drawing.Size(116, 23);
            Unknown2.TabIndex = 13;
            Unknown2.Validated += Unknown2_Validated;
            // 
            // PosY
            // 
            PosY.Enabled = false;
            PosY.Location = new System.Drawing.Point(178, 83);
            PosY.Margin = new System.Windows.Forms.Padding(4);
            PosY.Name = "PosY";
            PosY.Size = new System.Drawing.Size(53, 23);
            PosY.TabIndex = 15;
            // 
            // Flags
            // 
            Flags.Location = new System.Drawing.Point(114, 113);
            Flags.Margin = new System.Windows.Forms.Padding(4);
            Flags.Name = "Flags";
            Flags.Size = new System.Drawing.Size(117, 23);
            Flags.TabIndex = 16;
            Flags.Validated += Flags_Validated;
            // 
            // PanelUnitList
            // 
            PanelUnitList.Controls.Add(SearchUnitPrevious);
            PanelUnitList.Controls.Add(SearchUnitNext);
            PanelUnitList.Controls.Add(SearchUnitText);
            PanelUnitList.Controls.Add(ListUnits);
            PanelUnitList.Controls.Add(label1);
            PanelUnitList.Controls.Add(ButtonResizeList);
            PanelUnitList.Location = new System.Drawing.Point(4, 234);
            PanelUnitList.Margin = new System.Windows.Forms.Padding(4);
            PanelUnitList.Name = "PanelUnitList";
            PanelUnitList.Size = new System.Drawing.Size(339, 247);
            PanelUnitList.TabIndex = 18;
            // 
            // SearchUnitPrevious
            // 
            SearchUnitPrevious.Location = new System.Drawing.Point(178, 216);
            SearchUnitPrevious.Margin = new System.Windows.Forms.Padding(4);
            SearchUnitPrevious.Name = "SearchUnitPrevious";
            SearchUnitPrevious.Size = new System.Drawing.Size(157, 26);
            SearchUnitPrevious.TabIndex = 23;
            SearchUnitPrevious.Text = "Find previous";
            SearchUnitPrevious.UseVisualStyleBackColor = true;
            SearchUnitPrevious.Click += SearchUnitPrevious_Click;
            // 
            // SearchUnitNext
            // 
            SearchUnitNext.Location = new System.Drawing.Point(4, 216);
            SearchUnitNext.Margin = new System.Windows.Forms.Padding(4);
            SearchUnitNext.Name = "SearchUnitNext";
            SearchUnitNext.Size = new System.Drawing.Size(164, 26);
            SearchUnitNext.TabIndex = 22;
            SearchUnitNext.Text = "Find next";
            SearchUnitNext.UseVisualStyleBackColor = true;
            SearchUnitNext.Click += SearchUnitNext_Click;
            // 
            // SearchUnitText
            // 
            SearchUnitText.Location = new System.Drawing.Point(4, 186);
            SearchUnitText.Margin = new System.Windows.Forms.Padding(4);
            SearchUnitText.Name = "SearchUnitText";
            SearchUnitText.Size = new System.Drawing.Size(330, 23);
            SearchUnitText.TabIndex = 21;
            // 
            // ListUnits
            // 
            ListUnits.FormattingEnabled = true;
            ListUnits.ItemHeight = 15;
            ListUnits.Location = new System.Drawing.Point(4, 37);
            ListUnits.Margin = new System.Windows.Forms.Padding(4);
            ListUnits.Name = "ListUnits";
            ListUnits.Size = new System.Drawing.Size(330, 139);
            ListUnits.TabIndex = 20;
            ListUnits.SelectedIndexChanged += ListUnits_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(68, 15);
            label1.TabIndex = 19;
            label1.Text = "List of units";
            // 
            // ButtonResizeList
            // 
            ButtonResizeList.Location = new System.Drawing.Point(309, 5);
            ButtonResizeList.Margin = new System.Windows.Forms.Padding(4);
            ButtonResizeList.Name = "ButtonResizeList";
            ButtonResizeList.Size = new System.Drawing.Size(25, 25);
            ButtonResizeList.TabIndex = 0;
            ButtonResizeList.Text = "-";
            ButtonResizeList.UseVisualStyleBackColor = true;
            ButtonResizeList.Click += ButtonResizeList_Click;
            // 
            // PanelProperties
            // 
            PanelProperties.Controls.Add(NPCScript);
            PanelProperties.Controls.Add(label2);
            PanelProperties.Controls.Add(LabelUnitName);
            PanelProperties.Controls.Add(UnitID);
            PanelProperties.Controls.Add(Flags);
            PanelProperties.Controls.Add(NPCID);
            PanelProperties.Controls.Add(PosY);
            PanelProperties.Controls.Add(label3);
            PanelProperties.Controls.Add(label8);
            PanelProperties.Controls.Add(PosX);
            PanelProperties.Controls.Add(Unknown2);
            PanelProperties.Controls.Add(label4);
            PanelProperties.Controls.Add(label7);
            PanelProperties.Controls.Add(label5);
            PanelProperties.Controls.Add(Group);
            PanelProperties.Controls.Add(Unknown1);
            PanelProperties.Controls.Add(label6);
            PanelProperties.Enabled = false;
            PanelProperties.Location = new System.Drawing.Point(4, 0);
            PanelProperties.Margin = new System.Windows.Forms.Padding(4);
            PanelProperties.Name = "PanelProperties";
            PanelProperties.Size = new System.Drawing.Size(339, 232);
            PanelProperties.TabIndex = 19;
            // 
            // NPCScript
            // 
            NPCScript.Location = new System.Drawing.Point(238, 51);
            NPCScript.Margin = new System.Windows.Forms.Padding(4);
            NPCScript.Name = "NPCScript";
            NPCScript.Size = new System.Drawing.Size(97, 26);
            NPCScript.TabIndex = 18;
            NPCScript.Text = "Open script";
            NPCScript.UseVisualStyleBackColor = true;
            NPCScript.Click += NPCScript_Click;
            // 
            // MapUnitInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(PanelProperties);
            Controls.Add(PanelUnitList);
            Margin = new System.Windows.Forms.Padding(4);
            Name = "MapUnitInspector";
            Size = new System.Drawing.Size(346, 485);
            Load += MapUnitInspector_Load;
            Resize += MapUnitInspector_Resize;
            PanelUnitList.ResumeLayout(false);
            PanelUnitList.PerformLayout();
            PanelProperties.ResumeLayout(false);
            PanelProperties.PerformLayout();
            ResumeLayout(false);
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
        private System.Windows.Forms.TextBox Flags;
        private System.Windows.Forms.Panel PanelUnitList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Button SearchUnitPrevious;
        private System.Windows.Forms.Button SearchUnitNext;
        private System.Windows.Forms.TextBox SearchUnitText;
        private System.Windows.Forms.ListBox ListUnits;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.Button NPCScript;
    }
}
