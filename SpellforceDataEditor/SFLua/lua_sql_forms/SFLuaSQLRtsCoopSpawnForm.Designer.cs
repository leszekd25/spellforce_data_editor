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
            this.label11 = new System.Windows.Forms.Label();
            this.SpawnDataUnitRemove = new System.Windows.Forms.Button();
            this.SpawnDataUnitAdd = new System.Windows.Forms.Button();
            this.SelectedSpawnDataUnitID = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SpawnDataUnits = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SpawnDataSeconds = new System.Windows.Forms.TextBox();
            this.SpawnDataMinutes = new System.Windows.Forms.TextBox();
            this.SpawnDataHours = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SpawnDataRemove = new System.Windows.Forms.Button();
            this.SpawnDataAdd = new System.Windows.Forms.Button();
            this.GroupSpawnData = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.GroupUnitRemove = new System.Windows.Forms.Button();
            this.GroupUnitAdd = new System.Windows.Forms.Button();
            this.SelectedUnitID = new System.Windows.Forms.TextBox();
            this.GroupGoal = new System.Windows.Forms.ComboBox();
            this.GroupClanSize = new System.Windows.Forms.TextBox();
            this.GroupLevelRange = new System.Windows.Forms.TextBox();
            this.GroupName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.GroupStartUnits = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSaveChanges = new System.Windows.Forms.Button();
            this.ButtonCancelChanges = new System.Windows.Forms.Button();
            this.ButtonAddCoopSpawn = new System.Windows.Forms.Button();
            this.ListSpawnTypes = new System.Windows.Forms.ListBox();
            this.SpawnDataActivation = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 13);
            this.label11.TabIndex = 68;
            this.label11.Text = "Spawn types";
            // 
            // SpawnDataUnitRemove
            // 
            this.SpawnDataUnitRemove.Location = new System.Drawing.Point(914, 276);
            this.SpawnDataUnitRemove.Name = "SpawnDataUnitRemove";
            this.SpawnDataUnitRemove.Size = new System.Drawing.Size(75, 23);
            this.SpawnDataUnitRemove.TabIndex = 67;
            this.SpawnDataUnitRemove.Text = "Remove";
            this.SpawnDataUnitRemove.UseVisualStyleBackColor = true;
            this.SpawnDataUnitRemove.Click += new System.EventHandler(this.SpawnDataUnitRemove_Click);
            // 
            // SpawnDataUnitAdd
            // 
            this.SpawnDataUnitAdd.Location = new System.Drawing.Point(807, 276);
            this.SpawnDataUnitAdd.Name = "SpawnDataUnitAdd";
            this.SpawnDataUnitAdd.Size = new System.Drawing.Size(75, 23);
            this.SpawnDataUnitAdd.TabIndex = 66;
            this.SpawnDataUnitAdd.Text = "Add";
            this.SpawnDataUnitAdd.UseVisualStyleBackColor = true;
            this.SpawnDataUnitAdd.Click += new System.EventHandler(this.SpawnDataUnitAdd_Click);
            // 
            // SelectedSpawnDataUnitID
            // 
            this.SelectedSpawnDataUnitID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedSpawnDataUnitID.Location = new System.Drawing.Point(807, 250);
            this.SelectedSpawnDataUnitID.Name = "SelectedSpawnDataUnitID";
            this.SelectedSpawnDataUnitID.Size = new System.Drawing.Size(100, 20);
            this.SelectedSpawnDataUnitID.TabIndex = 65;
            this.SelectedSpawnDataUnitID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedSpawnDataUnitID_MouseDown);
            this.SelectedSpawnDataUnitID.Validated += new System.EventHandler(this.SelectedSpawnDataUnitID_Validated);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(770, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 64;
            this.label10.Text = "Units";
            // 
            // SpawnDataUnits
            // 
            this.SpawnDataUnits.FormattingEnabled = true;
            this.SpawnDataUnits.Location = new System.Drawing.Point(807, 56);
            this.SpawnDataUnits.Name = "SpawnDataUnits";
            this.SpawnDataUnits.Size = new System.Drawing.Size(182, 186);
            this.SpawnDataUnits.TabIndex = 63;
            this.SpawnDataUnits.SelectedIndexChanged += new System.EventHandler(this.SpawnDataUnits_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(904, 34);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(15, 13);
            this.label9.TabIndex = 62;
            this.label9.Text = "m";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(849, 34);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "h";
            // 
            // SpawnDataSeconds
            // 
            this.SpawnDataSeconds.Location = new System.Drawing.Point(922, 31);
            this.SpawnDataSeconds.Name = "SpawnDataSeconds";
            this.SpawnDataSeconds.Size = new System.Drawing.Size(39, 20);
            this.SpawnDataSeconds.TabIndex = 60;
            this.SpawnDataSeconds.Validated += new System.EventHandler(this.SpawnDataSeconds_Validated);
            // 
            // SpawnDataMinutes
            // 
            this.SpawnDataMinutes.Location = new System.Drawing.Point(862, 31);
            this.SpawnDataMinutes.Name = "SpawnDataMinutes";
            this.SpawnDataMinutes.Size = new System.Drawing.Size(39, 20);
            this.SpawnDataMinutes.TabIndex = 59;
            this.SpawnDataMinutes.Validated += new System.EventHandler(this.SpawnDataMinutes_Validated);
            // 
            // SpawnDataHours
            // 
            this.SpawnDataHours.Location = new System.Drawing.Point(807, 31);
            this.SpawnDataHours.Name = "SpawnDataHours";
            this.SpawnDataHours.Size = new System.Drawing.Size(39, 20);
            this.SpawnDataHours.TabIndex = 58;
            this.SpawnDataHours.Validated += new System.EventHandler(this.SpawnDataHours_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(733, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 57;
            this.label7.Text = "Wave period";
            // 
            // SpawnDataRemove
            // 
            this.SpawnDataRemove.Location = new System.Drawing.Point(509, 276);
            this.SpawnDataRemove.Name = "SpawnDataRemove";
            this.SpawnDataRemove.Size = new System.Drawing.Size(203, 23);
            this.SpawnDataRemove.TabIndex = 56;
            this.SpawnDataRemove.Text = "Remove";
            this.SpawnDataRemove.UseVisualStyleBackColor = true;
            this.SpawnDataRemove.Click += new System.EventHandler(this.SpawnDataRemove_Click);
            // 
            // SpawnDataAdd
            // 
            this.SpawnDataAdd.Location = new System.Drawing.Point(509, 247);
            this.SpawnDataAdd.Name = "SpawnDataAdd";
            this.SpawnDataAdd.Size = new System.Drawing.Size(203, 23);
            this.SpawnDataAdd.TabIndex = 55;
            this.SpawnDataAdd.Text = "Add";
            this.SpawnDataAdd.UseVisualStyleBackColor = true;
            this.SpawnDataAdd.Click += new System.EventHandler(this.SpawnDataAdd_Click);
            // 
            // GroupSpawnData
            // 
            this.GroupSpawnData.FormattingEnabled = true;
            this.GroupSpawnData.Location = new System.Drawing.Point(509, 31);
            this.GroupSpawnData.Name = "GroupSpawnData";
            this.GroupSpawnData.Size = new System.Drawing.Size(203, 186);
            this.GroupSpawnData.TabIndex = 54;
            this.GroupSpawnData.SelectedIndexChanged += new System.EventHandler(this.GroupSpawnData_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(506, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 53;
            this.label6.Text = "Group spawn waves";
            // 
            // GroupUnitRemove
            // 
            this.GroupUnitRemove.Location = new System.Drawing.Point(409, 276);
            this.GroupUnitRemove.Name = "GroupUnitRemove";
            this.GroupUnitRemove.Size = new System.Drawing.Size(75, 23);
            this.GroupUnitRemove.TabIndex = 52;
            this.GroupUnitRemove.Text = "Remove";
            this.GroupUnitRemove.UseVisualStyleBackColor = true;
            this.GroupUnitRemove.Click += new System.EventHandler(this.GroupUnitRemove_Click);
            // 
            // GroupUnitAdd
            // 
            this.GroupUnitAdd.Location = new System.Drawing.Point(302, 276);
            this.GroupUnitAdd.Name = "GroupUnitAdd";
            this.GroupUnitAdd.Size = new System.Drawing.Size(75, 23);
            this.GroupUnitAdd.TabIndex = 51;
            this.GroupUnitAdd.Text = "Add";
            this.GroupUnitAdd.UseVisualStyleBackColor = true;
            this.GroupUnitAdd.Click += new System.EventHandler(this.GroupUnitAdd_Click);
            // 
            // SelectedUnitID
            // 
            this.SelectedUnitID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedUnitID.Location = new System.Drawing.Point(302, 250);
            this.SelectedUnitID.Name = "SelectedUnitID";
            this.SelectedUnitID.Size = new System.Drawing.Size(100, 20);
            this.SelectedUnitID.TabIndex = 50;
            this.SelectedUnitID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedUnitID_MouseDown);
            this.SelectedUnitID.Validated += new System.EventHandler(this.SelectedUnitID_Validated);
            // 
            // GroupGoal
            // 
            this.GroupGoal.FormattingEnabled = true;
            this.GroupGoal.Location = new System.Drawing.Point(302, 81);
            this.GroupGoal.Name = "GroupGoal";
            this.GroupGoal.Size = new System.Drawing.Size(121, 21);
            this.GroupGoal.TabIndex = 49;
            this.GroupGoal.SelectedIndexChanged += new System.EventHandler(this.GroupGoal_SelectedIndexChanged);
            // 
            // GroupClanSize
            // 
            this.GroupClanSize.Location = new System.Drawing.Point(302, 108);
            this.GroupClanSize.Name = "GroupClanSize";
            this.GroupClanSize.Size = new System.Drawing.Size(100, 20);
            this.GroupClanSize.TabIndex = 48;
            this.GroupClanSize.Validated += new System.EventHandler(this.GroupClanSize_Validated);
            // 
            // GroupLevelRange
            // 
            this.GroupLevelRange.Location = new System.Drawing.Point(302, 56);
            this.GroupLevelRange.Name = "GroupLevelRange";
            this.GroupLevelRange.Size = new System.Drawing.Size(182, 20);
            this.GroupLevelRange.TabIndex = 47;
            this.GroupLevelRange.Validated += new System.EventHandler(this.GroupLevelRange_Validated);
            // 
            // GroupName
            // 
            this.GroupName.Location = new System.Drawing.Point(302, 31);
            this.GroupName.Name = "GroupName";
            this.GroupName.Size = new System.Drawing.Size(182, 20);
            this.GroupName.TabIndex = 46;
            this.GroupName.Validated += new System.EventHandler(this.GroupName_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(218, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 45;
            this.label5.Text = "Starting units";
            // 
            // GroupStartUnits
            // 
            this.GroupStartUnits.FormattingEnabled = true;
            this.GroupStartUnits.Location = new System.Drawing.Point(302, 135);
            this.GroupStartUnits.Name = "GroupStartUnits";
            this.GroupStartUnits.Size = new System.Drawing.Size(182, 108);
            this.GroupStartUnits.TabIndex = 44;
            this.GroupStartUnits.SelectedIndexChanged += new System.EventHandler(this.GroupStartUnits_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(218, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "Max clan size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Goal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "Level range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Name";
            // 
            // ButtonSaveChanges
            // 
            this.ButtonSaveChanges.Location = new System.Drawing.Point(915, 341);
            this.ButtonSaveChanges.Name = "ButtonSaveChanges";
            this.ButtonSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveChanges.TabIndex = 39;
            this.ButtonSaveChanges.Text = "Apply";
            this.ButtonSaveChanges.UseVisualStyleBackColor = true;
            this.ButtonSaveChanges.Click += new System.EventHandler(this.ButtonSaveChanges_Click);
            // 
            // ButtonCancelChanges
            // 
            this.ButtonCancelChanges.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancelChanges.Location = new System.Drawing.Point(12, 341);
            this.ButtonCancelChanges.Name = "ButtonCancelChanges";
            this.ButtonCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancelChanges.TabIndex = 38;
            this.ButtonCancelChanges.Text = "Cancel";
            this.ButtonCancelChanges.UseVisualStyleBackColor = true;
            this.ButtonCancelChanges.Click += new System.EventHandler(this.ButtonCancelChanges_Click);
            // 
            // ButtonAddCoopSpawn
            // 
            this.ButtonAddCoopSpawn.Location = new System.Drawing.Point(12, 276);
            this.ButtonAddCoopSpawn.Name = "ButtonAddCoopSpawn";
            this.ButtonAddCoopSpawn.Size = new System.Drawing.Size(182, 23);
            this.ButtonAddCoopSpawn.TabIndex = 36;
            this.ButtonAddCoopSpawn.Text = "Add new spawn type";
            this.ButtonAddCoopSpawn.UseVisualStyleBackColor = true;
            this.ButtonAddCoopSpawn.Click += new System.EventHandler(this.ButtonAddCoopSpawn_Click);
            // 
            // ListSpawnTypes
            // 
            this.ListSpawnTypes.FormattingEnabled = true;
            this.ListSpawnTypes.Location = new System.Drawing.Point(12, 32);
            this.ListSpawnTypes.Name = "ListSpawnTypes";
            this.ListSpawnTypes.Size = new System.Drawing.Size(182, 238);
            this.ListSpawnTypes.TabIndex = 35;
            this.ListSpawnTypes.SelectedIndexChanged += new System.EventHandler(this.ListSpawnTypes_SelectedIndexChanged);
            // 
            // SpawnDataActivation
            // 
            this.SpawnDataActivation.Location = new System.Drawing.Point(509, 221);
            this.SpawnDataActivation.Name = "SpawnDataActivation";
            this.SpawnDataActivation.Size = new System.Drawing.Size(203, 20);
            this.SpawnDataActivation.TabIndex = 69;
            this.SpawnDataActivation.Validated += new System.EventHandler(this.SpawnDataActivation_Validated);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(964, 34);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(12, 13);
            this.label12.TabIndex = 70;
            this.label12.Text = "s";
            // 
            // SFLuaSQLRtsCoopSpawnForm
            // 
            this.AcceptButton = this.ButtonSaveChanges;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancelChanges;
            this.ClientSize = new System.Drawing.Size(1000, 373);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.SpawnDataActivation);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.SpawnDataUnitRemove);
            this.Controls.Add(this.SpawnDataUnitAdd);
            this.Controls.Add(this.SelectedSpawnDataUnitID);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.SpawnDataUnits);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.SpawnDataSeconds);
            this.Controls.Add(this.SpawnDataMinutes);
            this.Controls.Add(this.SpawnDataHours);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SpawnDataRemove);
            this.Controls.Add(this.SpawnDataAdd);
            this.Controls.Add(this.GroupSpawnData);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.GroupUnitRemove);
            this.Controls.Add(this.GroupUnitAdd);
            this.Controls.Add(this.SelectedUnitID);
            this.Controls.Add(this.GroupGoal);
            this.Controls.Add(this.GroupClanSize);
            this.Controls.Add(this.GroupLevelRange);
            this.Controls.Add(this.GroupName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.GroupStartUnits);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSaveChanges);
            this.Controls.Add(this.ButtonCancelChanges);
            this.Controls.Add(this.ButtonAddCoopSpawn);
            this.Controls.Add(this.ListSpawnTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SFLuaSQLRtsCoopSpawnForm";
            this.Text = "GdsRtsCoopSpawnGroups";
            this.Load += new System.EventHandler(this.SFLuaSQLRtsCoopSpawnForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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