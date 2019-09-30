namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapManageTeamCompositions
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
            this.ListTeamComps = new System.Windows.Forms.ListBox();
            this.ListTeams = new System.Windows.Forms.ListBox();
            this.ListTeamMembers = new System.Windows.Forms.ListBox();
            this.ListAvailablePlayers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TeamCompAdd = new System.Windows.Forms.Button();
            this.TeamCompRemove = new System.Windows.Forms.Button();
            this.ButtonKickFromTeam = new System.Windows.Forms.Button();
            this.ButtonMoveToTeam = new System.Windows.Forms.Button();
            this.SelectedPlayerTextID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SelectedPlayerName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SelectedPlayerLevelRange = new System.Windows.Forms.TextBox();
            this.LabelSelectedPlayerText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ListTeamComps
            // 
            this.ListTeamComps.FormattingEnabled = true;
            this.ListTeamComps.Location = new System.Drawing.Point(12, 23);
            this.ListTeamComps.Name = "ListTeamComps";
            this.ListTeamComps.Size = new System.Drawing.Size(177, 95);
            this.ListTeamComps.TabIndex = 0;
            this.ListTeamComps.SelectedIndexChanged += new System.EventHandler(this.ListTeamComps_SelectedIndexChanged);
            // 
            // ListTeams
            // 
            this.ListTeams.FormattingEnabled = true;
            this.ListTeams.Location = new System.Drawing.Point(355, 23);
            this.ListTeams.Name = "ListTeams";
            this.ListTeams.Size = new System.Drawing.Size(177, 95);
            this.ListTeams.TabIndex = 1;
            this.ListTeams.SelectedIndexChanged += new System.EventHandler(this.ListTeams_SelectedIndexChanged);
            // 
            // ListTeamMembers
            // 
            this.ListTeamMembers.FormattingEnabled = true;
            this.ListTeamMembers.Location = new System.Drawing.Point(12, 166);
            this.ListTeamMembers.Name = "ListTeamMembers";
            this.ListTeamMembers.Size = new System.Drawing.Size(177, 147);
            this.ListTeamMembers.TabIndex = 2;
            this.ListTeamMembers.Click += new System.EventHandler(this.ListTeamMembers_SelectedIndexChanged);
            // 
            // ListAvailablePlayers
            // 
            this.ListAvailablePlayers.FormattingEnabled = true;
            this.ListAvailablePlayers.Location = new System.Drawing.Point(355, 166);
            this.ListAvailablePlayers.Name = "ListAvailablePlayers";
            this.ListAvailablePlayers.Size = new System.Drawing.Size(177, 147);
            this.ListAvailablePlayers.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Multiplayer team compositions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(352, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Teams";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Team members";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(352, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Available players";
            // 
            // TeamCompAdd
            // 
            this.TeamCompAdd.Location = new System.Drawing.Point(12, 124);
            this.TeamCompAdd.Name = "TeamCompAdd";
            this.TeamCompAdd.Size = new System.Drawing.Size(75, 23);
            this.TeamCompAdd.TabIndex = 8;
            this.TeamCompAdd.Text = "Add";
            this.TeamCompAdd.UseVisualStyleBackColor = true;
            this.TeamCompAdd.Click += new System.EventHandler(this.TeamCompAdd_Click);
            // 
            // TeamCompRemove
            // 
            this.TeamCompRemove.Location = new System.Drawing.Point(114, 124);
            this.TeamCompRemove.Name = "TeamCompRemove";
            this.TeamCompRemove.Size = new System.Drawing.Size(75, 23);
            this.TeamCompRemove.TabIndex = 9;
            this.TeamCompRemove.Text = "Remove";
            this.TeamCompRemove.UseVisualStyleBackColor = true;
            this.TeamCompRemove.Click += new System.EventHandler(this.TeamCompRemove_Click);
            // 
            // ButtonKickFromTeam
            // 
            this.ButtonKickFromTeam.Location = new System.Drawing.Point(195, 255);
            this.ButtonKickFromTeam.Name = "ButtonKickFromTeam";
            this.ButtonKickFromTeam.Size = new System.Drawing.Size(75, 58);
            this.ButtonKickFromTeam.TabIndex = 10;
            this.ButtonKickFromTeam.Text = "Kick from team";
            this.ButtonKickFromTeam.UseVisualStyleBackColor = true;
            this.ButtonKickFromTeam.Click += new System.EventHandler(this.TeamKickMember_Click);
            // 
            // ButtonMoveToTeam
            // 
            this.ButtonMoveToTeam.Location = new System.Drawing.Point(274, 255);
            this.ButtonMoveToTeam.Name = "ButtonMoveToTeam";
            this.ButtonMoveToTeam.Size = new System.Drawing.Size(75, 58);
            this.ButtonMoveToTeam.TabIndex = 11;
            this.ButtonMoveToTeam.Text = "Move to team";
            this.ButtonMoveToTeam.UseVisualStyleBackColor = true;
            this.ButtonMoveToTeam.Click += new System.EventHandler(this.TeamMovePlayerToTeam_Click);
            // 
            // SelectedPlayerTextID
            // 
            this.SelectedPlayerTextID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedPlayerTextID.Location = new System.Drawing.Point(274, 166);
            this.SelectedPlayerTextID.Name = "SelectedPlayerTextID";
            this.SelectedPlayerTextID.Size = new System.Drawing.Size(75, 20);
            this.SelectedPlayerTextID.TabIndex = 12;
            this.SelectedPlayerTextID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SelectedPlayerTextID_MouseDown);
            this.SelectedPlayerTextID.Validated += new System.EventHandler(this.SelectedPlayerTextID_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(195, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Text ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 322);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Selected player name";
            // 
            // SelectedPlayerName
            // 
            this.SelectedPlayerName.Location = new System.Drawing.Point(195, 319);
            this.SelectedPlayerName.Name = "SelectedPlayerName";
            this.SelectedPlayerName.Size = new System.Drawing.Size(337, 20);
            this.SelectedPlayerName.TabIndex = 14;
            this.SelectedPlayerName.Validated += new System.EventHandler(this.SelectedPlayerName_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 348);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Selected player level range";
            // 
            // SelectedPlayerLevelRange
            // 
            this.SelectedPlayerLevelRange.Location = new System.Drawing.Point(195, 345);
            this.SelectedPlayerLevelRange.Name = "SelectedPlayerLevelRange";
            this.SelectedPlayerLevelRange.Size = new System.Drawing.Size(337, 20);
            this.SelectedPlayerLevelRange.TabIndex = 16;
            this.SelectedPlayerLevelRange.Validated += new System.EventHandler(this.SelectedPlayerLevelRange_Validated);
            // 
            // LabelSelectedPlayerText
            // 
            this.LabelSelectedPlayerText.AutoSize = true;
            this.LabelSelectedPlayerText.Location = new System.Drawing.Point(195, 189);
            this.LabelSelectedPlayerText.Name = "LabelSelectedPlayerText";
            this.LabelSelectedPlayerText.Size = new System.Drawing.Size(0, 13);
            this.LabelSelectedPlayerText.TabIndex = 18;
            // 
            // MapManageTeamCompositions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 372);
            this.Controls.Add(this.LabelSelectedPlayerText);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SelectedPlayerLevelRange);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.SelectedPlayerName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SelectedPlayerTextID);
            this.Controls.Add(this.ButtonMoveToTeam);
            this.Controls.Add(this.ButtonKickFromTeam);
            this.Controls.Add(this.TeamCompRemove);
            this.Controls.Add(this.TeamCompAdd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ListAvailablePlayers);
            this.Controls.Add(this.ListTeamMembers);
            this.Controls.Add(this.ListTeams);
            this.Controls.Add(this.ListTeamComps);
            this.Name = "MapManageTeamCompositions";
            this.Text = "Manage team compositions";
            this.Load += new System.EventHandler(this.MapManageTeamCompositions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListTeamComps;
        private System.Windows.Forms.ListBox ListTeams;
        private System.Windows.Forms.ListBox ListTeamMembers;
        private System.Windows.Forms.ListBox ListAvailablePlayers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button TeamCompAdd;
        private System.Windows.Forms.Button TeamCompRemove;
        private System.Windows.Forms.Button ButtonKickFromTeam;
        private System.Windows.Forms.Button ButtonMoveToTeam;
        private System.Windows.Forms.TextBox SelectedPlayerTextID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox SelectedPlayerName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox SelectedPlayerLevelRange;
        private System.Windows.Forms.Label LabelSelectedPlayerText;
    }
}