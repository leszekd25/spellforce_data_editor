namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    partial class SFLuaSQLRtsCoopSpawnForm
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
            label11 = new System.Windows.Forms.Label();
            SpawnDataUnitRemove = new System.Windows.Forms.Button();
            SpawnDataUnitAdd = new System.Windows.Forms.Button();
            SelectedSpawnDataUnitID = new System.Windows.Forms.TextBox();
            label10 = new System.Windows.Forms.Label();
            SpawnDataUnits = new System.Windows.Forms.ListBox();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            SpawnDataSeconds = new System.Windows.Forms.TextBox();
            SpawnDataMinutes = new System.Windows.Forms.TextBox();
            SpawnDataHours = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            SpawnDataRemove = new System.Windows.Forms.Button();
            SpawnDataAdd = new System.Windows.Forms.Button();
            GroupSpawnData = new System.Windows.Forms.ListBox();
            label6 = new System.Windows.Forms.Label();
            GroupUnitRemove = new System.Windows.Forms.Button();
            GroupUnitAdd = new System.Windows.Forms.Button();
            SelectedUnitID = new System.Windows.Forms.TextBox();
            GroupGoal = new System.Windows.Forms.ComboBox();
            GroupClanSize = new System.Windows.Forms.TextBox();
            GroupLevelRange = new System.Windows.Forms.TextBox();
            GroupName = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            GroupStartUnits = new System.Windows.Forms.ListBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            ButtonSaveChanges = new System.Windows.Forms.Button();
            ButtonCancelChanges = new System.Windows.Forms.Button();
            ButtonAddCoopSpawn = new System.Windows.Forms.Button();
            ListSpawnTypes = new System.Windows.Forms.ListBox();
            SpawnDataActivation = new System.Windows.Forms.TextBox();
            label12 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(14, 10);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(73, 15);
            label11.TabIndex = 68;
            label11.Text = "Spawn types";
            // 
            // SpawnDataUnitRemove
            // 
            SpawnDataUnitRemove.Location = new System.Drawing.Point(1066, 318);
            SpawnDataUnitRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataUnitRemove.Name = "SpawnDataUnitRemove";
            SpawnDataUnitRemove.Size = new System.Drawing.Size(88, 27);
            SpawnDataUnitRemove.TabIndex = 67;
            SpawnDataUnitRemove.Text = "Remove";
            SpawnDataUnitRemove.UseVisualStyleBackColor = true;
            SpawnDataUnitRemove.Click += SpawnDataUnitRemove_Click;
            // 
            // SpawnDataUnitAdd
            // 
            SpawnDataUnitAdd.Location = new System.Drawing.Point(941, 318);
            SpawnDataUnitAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataUnitAdd.Name = "SpawnDataUnitAdd";
            SpawnDataUnitAdd.Size = new System.Drawing.Size(88, 27);
            SpawnDataUnitAdd.TabIndex = 66;
            SpawnDataUnitAdd.Text = "Add";
            SpawnDataUnitAdd.UseVisualStyleBackColor = true;
            SpawnDataUnitAdd.Click += SpawnDataUnitAdd_Click;
            // 
            // SelectedSpawnDataUnitID
            // 
            SelectedSpawnDataUnitID.BackColor = System.Drawing.SystemColors.ControlLight;
            SelectedSpawnDataUnitID.Location = new System.Drawing.Point(941, 288);
            SelectedSpawnDataUnitID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SelectedSpawnDataUnitID.Name = "SelectedSpawnDataUnitID";
            SelectedSpawnDataUnitID.Size = new System.Drawing.Size(116, 23);
            SelectedSpawnDataUnitID.TabIndex = 65;
            SelectedSpawnDataUnitID.Validated += SelectedSpawnDataUnitID_Validated;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(898, 65);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(34, 15);
            label10.TabIndex = 64;
            label10.Text = "Units";
            // 
            // SpawnDataUnits
            // 
            SpawnDataUnits.FormattingEnabled = true;
            SpawnDataUnits.ItemHeight = 15;
            SpawnDataUnits.Location = new System.Drawing.Point(941, 65);
            SpawnDataUnits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataUnits.Name = "SpawnDataUnits";
            SpawnDataUnits.Size = new System.Drawing.Size(212, 214);
            SpawnDataUnits.TabIndex = 63;
            SpawnDataUnits.SelectedIndexChanged += SpawnDataUnits_SelectedIndexChanged;
            // 
            // label9
            // 
            label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(1055, 39);
            label9.Margin = new System.Windows.Forms.Padding(0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(18, 15);
            label9.TabIndex = 62;
            label9.Text = "m";
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(990, 39);
            label8.Margin = new System.Windows.Forms.Padding(0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(14, 15);
            label8.TabIndex = 61;
            label8.Text = "h";
            // 
            // SpawnDataSeconds
            // 
            SpawnDataSeconds.Location = new System.Drawing.Point(1076, 36);
            SpawnDataSeconds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataSeconds.Name = "SpawnDataSeconds";
            SpawnDataSeconds.Size = new System.Drawing.Size(45, 23);
            SpawnDataSeconds.TabIndex = 60;
            SpawnDataSeconds.Validated += SpawnDataSeconds_Validated;
            // 
            // SpawnDataMinutes
            // 
            SpawnDataMinutes.Location = new System.Drawing.Point(1006, 36);
            SpawnDataMinutes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataMinutes.Name = "SpawnDataMinutes";
            SpawnDataMinutes.Size = new System.Drawing.Size(45, 23);
            SpawnDataMinutes.TabIndex = 59;
            SpawnDataMinutes.Validated += SpawnDataMinutes_Validated;
            // 
            // SpawnDataHours
            // 
            SpawnDataHours.Location = new System.Drawing.Point(941, 36);
            SpawnDataHours.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataHours.Name = "SpawnDataHours";
            SpawnDataHours.Size = new System.Drawing.Size(45, 23);
            SpawnDataHours.TabIndex = 58;
            SpawnDataHours.Validated += SpawnDataHours_Validated;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(855, 39);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(73, 15);
            label7.TabIndex = 57;
            label7.Text = "Wave period";
            // 
            // SpawnDataRemove
            // 
            SpawnDataRemove.Location = new System.Drawing.Point(594, 318);
            SpawnDataRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataRemove.Name = "SpawnDataRemove";
            SpawnDataRemove.Size = new System.Drawing.Size(237, 27);
            SpawnDataRemove.TabIndex = 56;
            SpawnDataRemove.Text = "Remove";
            SpawnDataRemove.UseVisualStyleBackColor = true;
            SpawnDataRemove.Click += SpawnDataRemove_Click;
            // 
            // SpawnDataAdd
            // 
            SpawnDataAdd.Location = new System.Drawing.Point(594, 285);
            SpawnDataAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataAdd.Name = "SpawnDataAdd";
            SpawnDataAdd.Size = new System.Drawing.Size(237, 27);
            SpawnDataAdd.TabIndex = 55;
            SpawnDataAdd.Text = "Add";
            SpawnDataAdd.UseVisualStyleBackColor = true;
            SpawnDataAdd.Click += SpawnDataAdd_Click;
            // 
            // GroupSpawnData
            // 
            GroupSpawnData.FormattingEnabled = true;
            GroupSpawnData.ItemHeight = 15;
            GroupSpawnData.Location = new System.Drawing.Point(594, 36);
            GroupSpawnData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupSpawnData.Name = "GroupSpawnData";
            GroupSpawnData.Size = new System.Drawing.Size(236, 214);
            GroupSpawnData.TabIndex = 54;
            GroupSpawnData.SelectedIndexChanged += GroupSpawnData_SelectedIndexChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(590, 10);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(112, 15);
            label6.TabIndex = 53;
            label6.Text = "Group spawn waves";
            // 
            // GroupUnitRemove
            // 
            GroupUnitRemove.Location = new System.Drawing.Point(477, 318);
            GroupUnitRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupUnitRemove.Name = "GroupUnitRemove";
            GroupUnitRemove.Size = new System.Drawing.Size(88, 27);
            GroupUnitRemove.TabIndex = 52;
            GroupUnitRemove.Text = "Remove";
            GroupUnitRemove.UseVisualStyleBackColor = true;
            GroupUnitRemove.Click += GroupUnitRemove_Click;
            // 
            // GroupUnitAdd
            // 
            GroupUnitAdd.Location = new System.Drawing.Point(352, 318);
            GroupUnitAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupUnitAdd.Name = "GroupUnitAdd";
            GroupUnitAdd.Size = new System.Drawing.Size(88, 27);
            GroupUnitAdd.TabIndex = 51;
            GroupUnitAdd.Text = "Add";
            GroupUnitAdd.UseVisualStyleBackColor = true;
            GroupUnitAdd.Click += GroupUnitAdd_Click;
            // 
            // SelectedUnitID
            // 
            SelectedUnitID.BackColor = System.Drawing.SystemColors.ControlLight;
            SelectedUnitID.Location = new System.Drawing.Point(352, 288);
            SelectedUnitID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SelectedUnitID.Name = "SelectedUnitID";
            SelectedUnitID.Size = new System.Drawing.Size(116, 23);
            SelectedUnitID.TabIndex = 50;
            SelectedUnitID.Validated += SelectedUnitID_Validated;
            // 
            // GroupGoal
            // 
            GroupGoal.FormattingEnabled = true;
            GroupGoal.Location = new System.Drawing.Point(352, 93);
            GroupGoal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupGoal.Name = "GroupGoal";
            GroupGoal.Size = new System.Drawing.Size(140, 23);
            GroupGoal.TabIndex = 49;
            GroupGoal.SelectedIndexChanged += GroupGoal_SelectedIndexChanged;
            // 
            // GroupClanSize
            // 
            GroupClanSize.Location = new System.Drawing.Point(352, 125);
            GroupClanSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupClanSize.Name = "GroupClanSize";
            GroupClanSize.Size = new System.Drawing.Size(116, 23);
            GroupClanSize.TabIndex = 48;
            GroupClanSize.Validated += GroupClanSize_Validated;
            // 
            // GroupLevelRange
            // 
            GroupLevelRange.Location = new System.Drawing.Point(352, 65);
            GroupLevelRange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupLevelRange.Name = "GroupLevelRange";
            GroupLevelRange.Size = new System.Drawing.Size(212, 23);
            GroupLevelRange.TabIndex = 47;
            GroupLevelRange.Validated += GroupLevelRange_Validated;
            // 
            // GroupName
            // 
            GroupName.Location = new System.Drawing.Point(352, 36);
            GroupName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupName.Name = "GroupName";
            GroupName.Size = new System.Drawing.Size(212, 23);
            GroupName.TabIndex = 46;
            GroupName.Validated += GroupName_Validated;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(254, 155);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(77, 15);
            label5.TabIndex = 45;
            label5.Text = "Starting units";
            // 
            // GroupStartUnits
            // 
            GroupStartUnits.FormattingEnabled = true;
            GroupStartUnits.ItemHeight = 15;
            GroupStartUnits.Location = new System.Drawing.Point(352, 156);
            GroupStartUnits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GroupStartUnits.Name = "GroupStartUnits";
            GroupStartUnits.Size = new System.Drawing.Size(212, 124);
            GroupStartUnits.TabIndex = 44;
            GroupStartUnits.SelectedIndexChanged += GroupStartUnits_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(254, 128);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(77, 15);
            label4.TabIndex = 43;
            label4.Text = "Max clan size";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(254, 97);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(31, 15);
            label3.TabIndex = 42;
            label3.Text = "Goal";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(254, 68);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(67, 15);
            label2.TabIndex = 41;
            label2.Text = "Level range";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(254, 39);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 40;
            label1.Text = "Name";
            // 
            // ButtonSaveChanges
            // 
            ButtonSaveChanges.Location = new System.Drawing.Point(1068, 393);
            ButtonSaveChanges.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonSaveChanges.Name = "ButtonSaveChanges";
            ButtonSaveChanges.Size = new System.Drawing.Size(88, 27);
            ButtonSaveChanges.TabIndex = 39;
            ButtonSaveChanges.Text = "Apply";
            ButtonSaveChanges.UseVisualStyleBackColor = true;
            ButtonSaveChanges.Click += ButtonSaveChanges_Click;
            // 
            // ButtonCancelChanges
            // 
            ButtonCancelChanges.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonCancelChanges.Location = new System.Drawing.Point(14, 393);
            ButtonCancelChanges.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonCancelChanges.Name = "ButtonCancelChanges";
            ButtonCancelChanges.Size = new System.Drawing.Size(88, 27);
            ButtonCancelChanges.TabIndex = 38;
            ButtonCancelChanges.Text = "Cancel";
            ButtonCancelChanges.UseVisualStyleBackColor = true;
            ButtonCancelChanges.Click += ButtonCancelChanges_Click;
            // 
            // ButtonAddCoopSpawn
            // 
            ButtonAddCoopSpawn.Location = new System.Drawing.Point(14, 318);
            ButtonAddCoopSpawn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonAddCoopSpawn.Name = "ButtonAddCoopSpawn";
            ButtonAddCoopSpawn.Size = new System.Drawing.Size(212, 27);
            ButtonAddCoopSpawn.TabIndex = 36;
            ButtonAddCoopSpawn.Text = "Add new spawn type";
            ButtonAddCoopSpawn.UseVisualStyleBackColor = true;
            ButtonAddCoopSpawn.Click += ButtonAddCoopSpawn_Click;
            // 
            // ListSpawnTypes
            // 
            ListSpawnTypes.FormattingEnabled = true;
            ListSpawnTypes.ItemHeight = 15;
            ListSpawnTypes.Location = new System.Drawing.Point(14, 37);
            ListSpawnTypes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListSpawnTypes.Name = "ListSpawnTypes";
            ListSpawnTypes.Size = new System.Drawing.Size(212, 274);
            ListSpawnTypes.TabIndex = 35;
            ListSpawnTypes.SelectedIndexChanged += ListSpawnTypes_SelectedIndexChanged;
            // 
            // SpawnDataActivation
            // 
            SpawnDataActivation.Location = new System.Drawing.Point(594, 255);
            SpawnDataActivation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SpawnDataActivation.Name = "SpawnDataActivation";
            SpawnDataActivation.Size = new System.Drawing.Size(236, 23);
            SpawnDataActivation.TabIndex = 69;
            SpawnDataActivation.Validated += SpawnDataActivation_Validated;
            // 
            // label12
            // 
            label12.Anchor = System.Windows.Forms.AnchorStyles.None;
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(1125, 39);
            label12.Margin = new System.Windows.Forms.Padding(0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(12, 15);
            label12.TabIndex = 70;
            label12.Text = "s";
            // 
            // SFLuaSQLRtsCoopSpawnForm
            // 
            AcceptButton = ButtonSaveChanges;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = ButtonCancelChanges;
            ClientSize = new System.Drawing.Size(1167, 430);
            Controls.Add(label12);
            Controls.Add(SpawnDataActivation);
            Controls.Add(label11);
            Controls.Add(SpawnDataUnitRemove);
            Controls.Add(SpawnDataUnitAdd);
            Controls.Add(SelectedSpawnDataUnitID);
            Controls.Add(label10);
            Controls.Add(SpawnDataUnits);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(SpawnDataSeconds);
            Controls.Add(SpawnDataMinutes);
            Controls.Add(SpawnDataHours);
            Controls.Add(label7);
            Controls.Add(SpawnDataRemove);
            Controls.Add(SpawnDataAdd);
            Controls.Add(GroupSpawnData);
            Controls.Add(label6);
            Controls.Add(GroupUnitRemove);
            Controls.Add(GroupUnitAdd);
            Controls.Add(SelectedUnitID);
            Controls.Add(GroupGoal);
            Controls.Add(GroupClanSize);
            Controls.Add(GroupLevelRange);
            Controls.Add(GroupName);
            Controls.Add(label5);
            Controls.Add(GroupStartUnits);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ButtonSaveChanges);
            Controls.Add(ButtonCancelChanges);
            Controls.Add(ButtonAddCoopSpawn);
            Controls.Add(ListSpawnTypes);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SFLuaSQLRtsCoopSpawnForm";
            Text = "GdsRtsCoopSpawnGroups";
            Load += SFLuaSQLRtsCoopSpawnForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button SpawnDataUnitRemove;
        private System.Windows.Forms.Button SpawnDataUnitAdd;
        private System.Windows.Forms.TextBox SelectedSpawnDataUnitID;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox SpawnDataUnits;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox SpawnDataSeconds;
        private System.Windows.Forms.TextBox SpawnDataMinutes;
        private System.Windows.Forms.TextBox SpawnDataHours;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button SpawnDataRemove;
        private System.Windows.Forms.Button SpawnDataAdd;
        private System.Windows.Forms.ListBox GroupSpawnData;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button GroupUnitRemove;
        private System.Windows.Forms.Button GroupUnitAdd;
        private System.Windows.Forms.TextBox SelectedUnitID;
        private System.Windows.Forms.ComboBox GroupGoal;
        private System.Windows.Forms.TextBox GroupClanSize;
        private System.Windows.Forms.TextBox GroupLevelRange;
        private System.Windows.Forms.TextBox GroupName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox GroupStartUnits;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSaveChanges;
        private System.Windows.Forms.Button ButtonCancelChanges;
        private System.Windows.Forms.Button ButtonAddCoopSpawn;
        private System.Windows.Forms.ListBox ListSpawnTypes;
        private System.Windows.Forms.TextBox SpawnDataActivation;
        private System.Windows.Forms.Label label12;
    }
}