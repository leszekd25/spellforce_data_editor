namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorMetadataControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboMapType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.CoopMaxClanSize1 = new System.Windows.Forms.TextBox();
            this.CoopMaxClanSize2 = new System.Windows.Forms.TextBox();
            this.CoopMaxClanSize3 = new System.Windows.Forms.TextBox();
            this.CoopInitSpawn3 = new System.Windows.Forms.TextBox();
            this.CoopInitSpawn2 = new System.Windows.Forms.TextBox();
            this.CoopInitSpawn1 = new System.Windows.Forms.TextBox();
            this.CoopBeginWave3 = new System.Windows.Forms.TextBox();
            this.CoopBeginWave2 = new System.Windows.Forms.TextBox();
            this.CoopBeginWave1 = new System.Windows.Forms.TextBox();
            this.CoopSpawnDelay3 = new System.Windows.Forms.TextBox();
            this.CoopSpawnDelay2 = new System.Windows.Forms.TextBox();
            this.CoopSpawnDelay1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ListPlayerSpawns = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.SelectedSpawnTextID = new System.Windows.Forms.TextBox();
            this.SelectedSpawnUnknown = new System.Windows.Forms.TextBox();
            this.ListTeamComps = new System.Windows.Forms.ListBox();
            this.label15 = new System.Windows.Forms.Label();
            this.ListTeams = new System.Windows.Forms.ListBox();
            this.ListTeamMembers = new System.Windows.Forms.ListBox();
            this.label16 = new System.Windows.Forms.Label();
            this.TeamCompAdd = new System.Windows.Forms.Button();
            this.TeamCompRemove = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.ListAvailablePlayers = new System.Windows.Forms.ListBox();
            this.TeamMovePlayerToTeam = new System.Windows.Forms.Button();
            this.TeamKickMember = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.SelectedPlayerName = new System.Windows.Forms.TextBox();
            this.SelectedPlayerLevelRange = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.PanelCoopParameters = new System.Windows.Forms.Panel();
            this.PanelSpawnPoints = new System.Windows.Forms.Panel();
            this.PanelMultiplayerCompositions = new System.Windows.Forms.Panel();
            this.ComboBindstoneList = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.SelectedPlayerTextID = new System.Windows.Forms.TextBox();
            this.LabelSelectedPlayerText = new System.Windows.Forms.Label();
            this.PanelCoopParameters.SuspendLayout();
            this.PanelSpawnPoints.SuspendLayout();
            this.PanelMultiplayerCompositions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Coop spawn parameters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Map type";
            // 
            // ComboMapType
            // 
            this.ComboMapType.FormattingEnabled = true;
            this.ComboMapType.Items.AddRange(new object[] {
            "Campaign",
            "Coop",
            "Multiplayer"});
            this.ComboMapType.Location = new System.Drawing.Point(156, 3);
            this.ComboMapType.Name = "ComboMapType";
            this.ComboMapType.Size = new System.Drawing.Size(121, 21);
            this.ComboMapType.TabIndex = 2;
            this.ComboMapType.SelectedIndexChanged += new System.EventHandler(this.ComboMapType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Max clan size factor";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Init spawn factor";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Begin wave factor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Spawn delay factor";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(150, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "1 player";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "2 players";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(292, 7);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(49, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "3 players";
            // 
            // CoopMaxClanSize1
            // 
            this.CoopMaxClanSize1.Location = new System.Drawing.Point(153, 23);
            this.CoopMaxClanSize1.Name = "CoopMaxClanSize1";
            this.CoopMaxClanSize1.Size = new System.Drawing.Size(65, 20);
            this.CoopMaxClanSize1.TabIndex = 10;
            this.CoopMaxClanSize1.Validated += new System.EventHandler(this.CoopMaxClanSize1_Validated);
            // 
            // CoopMaxClanSize2
            // 
            this.CoopMaxClanSize2.Location = new System.Drawing.Point(224, 23);
            this.CoopMaxClanSize2.Name = "CoopMaxClanSize2";
            this.CoopMaxClanSize2.Size = new System.Drawing.Size(65, 20);
            this.CoopMaxClanSize2.TabIndex = 11;
            this.CoopMaxClanSize2.Validated += new System.EventHandler(this.CoopMaxClanSize2_Validated);
            // 
            // CoopMaxClanSize3
            // 
            this.CoopMaxClanSize3.Location = new System.Drawing.Point(295, 23);
            this.CoopMaxClanSize3.Name = "CoopMaxClanSize3";
            this.CoopMaxClanSize3.Size = new System.Drawing.Size(65, 20);
            this.CoopMaxClanSize3.TabIndex = 12;
            this.CoopMaxClanSize3.Validated += new System.EventHandler(this.CoopMaxClanSize3_Validated);
            // 
            // CoopInitSpawn3
            // 
            this.CoopInitSpawn3.Location = new System.Drawing.Point(295, 49);
            this.CoopInitSpawn3.Name = "CoopInitSpawn3";
            this.CoopInitSpawn3.Size = new System.Drawing.Size(65, 20);
            this.CoopInitSpawn3.TabIndex = 15;
            this.CoopInitSpawn3.Validated += new System.EventHandler(this.CoopInitSpawn3_Validated);
            // 
            // CoopInitSpawn2
            // 
            this.CoopInitSpawn2.Location = new System.Drawing.Point(224, 49);
            this.CoopInitSpawn2.Name = "CoopInitSpawn2";
            this.CoopInitSpawn2.Size = new System.Drawing.Size(65, 20);
            this.CoopInitSpawn2.TabIndex = 14;
            this.CoopInitSpawn2.Validated += new System.EventHandler(this.CoopInitSpawn2_Validated);
            // 
            // CoopInitSpawn1
            // 
            this.CoopInitSpawn1.Location = new System.Drawing.Point(153, 49);
            this.CoopInitSpawn1.Name = "CoopInitSpawn1";
            this.CoopInitSpawn1.Size = new System.Drawing.Size(65, 20);
            this.CoopInitSpawn1.TabIndex = 13;
            this.CoopInitSpawn1.Validated += new System.EventHandler(this.CoopInitSpawn1_Validated);
            // 
            // CoopBeginWave3
            // 
            this.CoopBeginWave3.Location = new System.Drawing.Point(295, 75);
            this.CoopBeginWave3.Name = "CoopBeginWave3";
            this.CoopBeginWave3.Size = new System.Drawing.Size(65, 20);
            this.CoopBeginWave3.TabIndex = 18;
            this.CoopBeginWave3.Validated += new System.EventHandler(this.CoopBeginWave3_Validated);
            // 
            // CoopBeginWave2
            // 
            this.CoopBeginWave2.Location = new System.Drawing.Point(224, 75);
            this.CoopBeginWave2.Name = "CoopBeginWave2";
            this.CoopBeginWave2.Size = new System.Drawing.Size(65, 20);
            this.CoopBeginWave2.TabIndex = 17;
            this.CoopBeginWave2.Validated += new System.EventHandler(this.CoopBeginWave2_Validated);
            // 
            // CoopBeginWave1
            // 
            this.CoopBeginWave1.Location = new System.Drawing.Point(153, 75);
            this.CoopBeginWave1.Name = "CoopBeginWave1";
            this.CoopBeginWave1.Size = new System.Drawing.Size(65, 20);
            this.CoopBeginWave1.TabIndex = 16;
            this.CoopBeginWave1.Validated += new System.EventHandler(this.CoopBeginWave1_Validated);
            // 
            // CoopSpawnDelay3
            // 
            this.CoopSpawnDelay3.Location = new System.Drawing.Point(295, 101);
            this.CoopSpawnDelay3.Name = "CoopSpawnDelay3";
            this.CoopSpawnDelay3.Size = new System.Drawing.Size(65, 20);
            this.CoopSpawnDelay3.TabIndex = 21;
            this.CoopSpawnDelay3.Validated += new System.EventHandler(this.CoopSpawnDelay3_Validated);
            // 
            // CoopSpawnDelay2
            // 
            this.CoopSpawnDelay2.Location = new System.Drawing.Point(224, 101);
            this.CoopSpawnDelay2.Name = "CoopSpawnDelay2";
            this.CoopSpawnDelay2.Size = new System.Drawing.Size(65, 20);
            this.CoopSpawnDelay2.TabIndex = 20;
            this.CoopSpawnDelay2.Validated += new System.EventHandler(this.CoopSpawnDelay2_Validated);
            // 
            // CoopSpawnDelay1
            // 
            this.CoopSpawnDelay1.Location = new System.Drawing.Point(153, 101);
            this.CoopSpawnDelay1.Name = "CoopSpawnDelay1";
            this.CoopSpawnDelay1.Size = new System.Drawing.Size(65, 20);
            this.CoopSpawnDelay1.TabIndex = 19;
            this.CoopSpawnDelay1.Validated += new System.EventHandler(this.CoopSpawnDelay1_Validated);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 5);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(147, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Multiplayer team compositions";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 2);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(101, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Player spawn points";
            // 
            // ListPlayerSpawns
            // 
            this.ListPlayerSpawns.FormattingEnabled = true;
            this.ListPlayerSpawns.HorizontalScrollbar = true;
            this.ListPlayerSpawns.Location = new System.Drawing.Point(17, 18);
            this.ListPlayerSpawns.Name = "ListPlayerSpawns";
            this.ListPlayerSpawns.Size = new System.Drawing.Size(201, 95);
            this.ListPlayerSpawns.TabIndex = 24;
            this.ListPlayerSpawns.SelectedIndexChanged += new System.EventHandler(this.ListPlayerSpawns_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(225, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Bindstone";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(225, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(42, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Text ID";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(225, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 13);
            this.label14.TabIndex = 27;
            this.label14.Text = "Unknown";
            // 
            // SelectedSpawnTextID
            // 
            this.SelectedSpawnTextID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedSpawnTextID.Location = new System.Drawing.Point(296, 45);
            this.SelectedSpawnTextID.Name = "SelectedSpawnTextID";
            this.SelectedSpawnTextID.Size = new System.Drawing.Size(65, 20);
            this.SelectedSpawnTextID.TabIndex = 29;
            this.SelectedSpawnTextID.Validated += new System.EventHandler(this.SelectedSpawnTextID_Validated);
            // 
            // SelectedSpawnUnknown
            // 
            this.SelectedSpawnUnknown.Location = new System.Drawing.Point(296, 71);
            this.SelectedSpawnUnknown.Name = "SelectedSpawnUnknown";
            this.SelectedSpawnUnknown.Size = new System.Drawing.Size(65, 20);
            this.SelectedSpawnUnknown.TabIndex = 30;
            this.SelectedSpawnUnknown.Validated += new System.EventHandler(this.SelectedSpawnUnknown_Validated);
            // 
            // ListTeamComps
            // 
            this.ListTeamComps.FormattingEnabled = true;
            this.ListTeamComps.Location = new System.Drawing.Point(17, 21);
            this.ListTeamComps.Name = "ListTeamComps";
            this.ListTeamComps.Size = new System.Drawing.Size(130, 82);
            this.ListTeamComps.TabIndex = 32;
            this.ListTeamComps.SelectedIndexChanged += new System.EventHandler(this.ListTeamComps_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(292, 5);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 13);
            this.label15.TabIndex = 33;
            this.label15.Text = "Teams";
            // 
            // ListTeams
            // 
            this.ListTeams.FormattingEnabled = true;
            this.ListTeams.Location = new System.Drawing.Point(295, 21);
            this.ListTeams.Name = "ListTeams";
            this.ListTeams.Size = new System.Drawing.Size(133, 82);
            this.ListTeams.TabIndex = 34;
            this.ListTeams.SelectedIndexChanged += new System.EventHandler(this.ListTeams_SelectedIndexChanged);
            // 
            // ListTeamMembers
            // 
            this.ListTeamMembers.FormattingEnabled = true;
            this.ListTeamMembers.Location = new System.Drawing.Point(17, 122);
            this.ListTeamMembers.Name = "ListTeamMembers";
            this.ListTeamMembers.Size = new System.Drawing.Size(130, 82);
            this.ListTeamMembers.TabIndex = 35;
            this.ListTeamMembers.SelectedIndexChanged += new System.EventHandler(this.ListTeamMembers_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 106);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(79, 13);
            this.label16.TabIndex = 36;
            this.label16.Text = "Team members";
            // 
            // TeamCompAdd
            // 
            this.TeamCompAdd.Location = new System.Drawing.Point(153, 21);
            this.TeamCompAdd.Name = "TeamCompAdd";
            this.TeamCompAdd.Size = new System.Drawing.Size(55, 23);
            this.TeamCompAdd.TabIndex = 37;
            this.TeamCompAdd.Text = "Add";
            this.TeamCompAdd.UseVisualStyleBackColor = true;
            // 
            // TeamCompRemove
            // 
            this.TeamCompRemove.Location = new System.Drawing.Point(153, 50);
            this.TeamCompRemove.Name = "TeamCompRemove";
            this.TeamCompRemove.Size = new System.Drawing.Size(55, 23);
            this.TeamCompRemove.TabIndex = 38;
            this.TeamCompRemove.Text = "Remove";
            this.TeamCompRemove.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(292, 106);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(86, 13);
            this.label17.TabIndex = 40;
            this.label17.Text = "Available players";
            // 
            // ListAvailablePlayers
            // 
            this.ListAvailablePlayers.FormattingEnabled = true;
            this.ListAvailablePlayers.Location = new System.Drawing.Point(295, 122);
            this.ListAvailablePlayers.Name = "ListAvailablePlayers";
            this.ListAvailablePlayers.Size = new System.Drawing.Size(133, 82);
            this.ListAvailablePlayers.TabIndex = 39;
            // 
            // TeamMovePlayerToTeam
            // 
            this.TeamMovePlayerToTeam.Location = new System.Drawing.Point(224, 165);
            this.TeamMovePlayerToTeam.Name = "TeamMovePlayerToTeam";
            this.TeamMovePlayerToTeam.Size = new System.Drawing.Size(65, 39);
            this.TeamMovePlayerToTeam.TabIndex = 41;
            this.TeamMovePlayerToTeam.Text = "Move to team";
            this.TeamMovePlayerToTeam.UseVisualStyleBackColor = true;
            this.TeamMovePlayerToTeam.Click += new System.EventHandler(this.TeamMovePlayerToTeam_Click);
            // 
            // TeamKickMember
            // 
            this.TeamKickMember.Location = new System.Drawing.Point(153, 165);
            this.TeamKickMember.Name = "TeamKickMember";
            this.TeamKickMember.Size = new System.Drawing.Size(65, 39);
            this.TeamKickMember.TabIndex = 42;
            this.TeamKickMember.Text = "Kick from team";
            this.TeamKickMember.UseVisualStyleBackColor = true;
            this.TeamKickMember.Click += new System.EventHandler(this.TeamKickMember_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(14, 213);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(109, 13);
            this.label18.TabIndex = 43;
            this.label18.Text = "Selected player name";
            // 
            // SelectedPlayerName
            // 
            this.SelectedPlayerName.Location = new System.Drawing.Point(153, 210);
            this.SelectedPlayerName.Name = "SelectedPlayerName";
            this.SelectedPlayerName.Size = new System.Drawing.Size(204, 20);
            this.SelectedPlayerName.TabIndex = 44;
            // 
            // SelectedPlayerLevelRange
            // 
            this.SelectedPlayerLevelRange.Location = new System.Drawing.Point(153, 236);
            this.SelectedPlayerLevelRange.Name = "SelectedPlayerLevelRange";
            this.SelectedPlayerLevelRange.Size = new System.Drawing.Size(204, 20);
            this.SelectedPlayerLevelRange.TabIndex = 46;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(14, 239);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(135, 13);
            this.label19.TabIndex = 45;
            this.label19.Text = "Selected player level range";
            // 
            // PanelCoopParameters
            // 
            this.PanelCoopParameters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelCoopParameters.Controls.Add(this.CoopMaxClanSize1);
            this.PanelCoopParameters.Controls.Add(this.label1);
            this.PanelCoopParameters.Controls.Add(this.label3);
            this.PanelCoopParameters.Controls.Add(this.label4);
            this.PanelCoopParameters.Controls.Add(this.label5);
            this.PanelCoopParameters.Controls.Add(this.label6);
            this.PanelCoopParameters.Controls.Add(this.label7);
            this.PanelCoopParameters.Controls.Add(this.label8);
            this.PanelCoopParameters.Controls.Add(this.label9);
            this.PanelCoopParameters.Controls.Add(this.CoopMaxClanSize2);
            this.PanelCoopParameters.Controls.Add(this.CoopMaxClanSize3);
            this.PanelCoopParameters.Controls.Add(this.CoopInitSpawn1);
            this.PanelCoopParameters.Controls.Add(this.CoopInitSpawn2);
            this.PanelCoopParameters.Controls.Add(this.CoopInitSpawn3);
            this.PanelCoopParameters.Controls.Add(this.CoopBeginWave1);
            this.PanelCoopParameters.Controls.Add(this.CoopBeginWave2);
            this.PanelCoopParameters.Controls.Add(this.CoopBeginWave3);
            this.PanelCoopParameters.Controls.Add(this.CoopSpawnDelay1);
            this.PanelCoopParameters.Controls.Add(this.CoopSpawnDelay2);
            this.PanelCoopParameters.Controls.Add(this.CoopSpawnDelay3);
            this.PanelCoopParameters.Enabled = false;
            this.PanelCoopParameters.Location = new System.Drawing.Point(3, 30);
            this.PanelCoopParameters.Name = "PanelCoopParameters";
            this.PanelCoopParameters.Size = new System.Drawing.Size(437, 132);
            this.PanelCoopParameters.TabIndex = 47;
            // 
            // PanelSpawnPoints
            // 
            this.PanelSpawnPoints.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelSpawnPoints.Controls.Add(this.ComboBindstoneList);
            this.PanelSpawnPoints.Controls.Add(this.label11);
            this.PanelSpawnPoints.Controls.Add(this.ListPlayerSpawns);
            this.PanelSpawnPoints.Controls.Add(this.label12);
            this.PanelSpawnPoints.Controls.Add(this.label13);
            this.PanelSpawnPoints.Controls.Add(this.label14);
            this.PanelSpawnPoints.Controls.Add(this.SelectedSpawnTextID);
            this.PanelSpawnPoints.Controls.Add(this.SelectedSpawnUnknown);
            this.PanelSpawnPoints.Location = new System.Drawing.Point(3, 168);
            this.PanelSpawnPoints.Name = "PanelSpawnPoints";
            this.PanelSpawnPoints.Size = new System.Drawing.Size(437, 120);
            this.PanelSpawnPoints.TabIndex = 48;
            // 
            // PanelMultiplayerCompositions
            // 
            this.PanelMultiplayerCompositions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelMultiplayerCompositions.Controls.Add(this.LabelSelectedPlayerText);
            this.PanelMultiplayerCompositions.Controls.Add(this.label20);
            this.PanelMultiplayerCompositions.Controls.Add(this.ListTeamComps);
            this.PanelMultiplayerCompositions.Controls.Add(this.SelectedPlayerTextID);
            this.PanelMultiplayerCompositions.Controls.Add(this.label10);
            this.PanelMultiplayerCompositions.Controls.Add(this.label15);
            this.PanelMultiplayerCompositions.Controls.Add(this.SelectedPlayerLevelRange);
            this.PanelMultiplayerCompositions.Controls.Add(this.ListTeams);
            this.PanelMultiplayerCompositions.Controls.Add(this.label19);
            this.PanelMultiplayerCompositions.Controls.Add(this.ListTeamMembers);
            this.PanelMultiplayerCompositions.Controls.Add(this.SelectedPlayerName);
            this.PanelMultiplayerCompositions.Controls.Add(this.label16);
            this.PanelMultiplayerCompositions.Controls.Add(this.label18);
            this.PanelMultiplayerCompositions.Controls.Add(this.TeamCompAdd);
            this.PanelMultiplayerCompositions.Controls.Add(this.TeamKickMember);
            this.PanelMultiplayerCompositions.Controls.Add(this.TeamCompRemove);
            this.PanelMultiplayerCompositions.Controls.Add(this.TeamMovePlayerToTeam);
            this.PanelMultiplayerCompositions.Controls.Add(this.ListAvailablePlayers);
            this.PanelMultiplayerCompositions.Controls.Add(this.label17);
            this.PanelMultiplayerCompositions.Enabled = false;
            this.PanelMultiplayerCompositions.Location = new System.Drawing.Point(3, 294);
            this.PanelMultiplayerCompositions.Name = "PanelMultiplayerCompositions";
            this.PanelMultiplayerCompositions.Size = new System.Drawing.Size(437, 262);
            this.PanelMultiplayerCompositions.TabIndex = 49;
            // 
            // ComboBindstoneList
            // 
            this.ComboBindstoneList.FormattingEnabled = true;
            this.ComboBindstoneList.Location = new System.Drawing.Point(296, 18);
            this.ComboBindstoneList.Name = "ComboBindstoneList";
            this.ComboBindstoneList.Size = new System.Drawing.Size(132, 21);
            this.ComboBindstoneList.TabIndex = 31;
            this.ComboBindstoneList.SelectedIndexChanged += new System.EventHandler(this.ComboBindstoneList_SelectedIndexChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(153, 125);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(42, 13);
            this.label20.TabIndex = 32;
            this.label20.Text = "Text ID";
            // 
            // SelectedPlayerTextID
            // 
            this.SelectedPlayerTextID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedPlayerTextID.Location = new System.Drawing.Point(224, 122);
            this.SelectedPlayerTextID.Name = "SelectedPlayerTextID";
            this.SelectedPlayerTextID.Size = new System.Drawing.Size(65, 20);
            this.SelectedPlayerTextID.TabIndex = 33;
            this.SelectedPlayerTextID.Validated += new System.EventHandler(this.SelectedPlayerTextID_Validated);
            // 
            // LabelSelectedPlayerText
            // 
            this.LabelSelectedPlayerText.AutoSize = true;
            this.LabelSelectedPlayerText.Location = new System.Drawing.Point(221, 145);
            this.LabelSelectedPlayerText.Name = "LabelSelectedPlayerText";
            this.LabelSelectedPlayerText.Size = new System.Drawing.Size(0, 13);
            this.LabelSelectedPlayerText.TabIndex = 47;
            // 
            // MapInspectorMetadataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelMultiplayerCompositions);
            this.Controls.Add(this.PanelSpawnPoints);
            this.Controls.Add(this.PanelCoopParameters);
            this.Controls.Add(this.ComboMapType);
            this.Controls.Add(this.label2);
            this.Name = "MapInspectorMetadataControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorMetadataControl_VisibleChanged);
            this.PanelCoopParameters.ResumeLayout(false);
            this.PanelCoopParameters.PerformLayout();
            this.PanelSpawnPoints.ResumeLayout(false);
            this.PanelSpawnPoints.PerformLayout();
            this.PanelMultiplayerCompositions.ResumeLayout(false);
            this.PanelMultiplayerCompositions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboMapType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox CoopMaxClanSize1;
        private System.Windows.Forms.TextBox CoopMaxClanSize2;
        private System.Windows.Forms.TextBox CoopMaxClanSize3;
        private System.Windows.Forms.TextBox CoopInitSpawn3;
        private System.Windows.Forms.TextBox CoopInitSpawn2;
        private System.Windows.Forms.TextBox CoopInitSpawn1;
        private System.Windows.Forms.TextBox CoopBeginWave3;
        private System.Windows.Forms.TextBox CoopBeginWave2;
        private System.Windows.Forms.TextBox CoopBeginWave1;
        private System.Windows.Forms.TextBox CoopSpawnDelay3;
        private System.Windows.Forms.TextBox CoopSpawnDelay2;
        private System.Windows.Forms.TextBox CoopSpawnDelay1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox ListPlayerSpawns;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox SelectedSpawnTextID;
        private System.Windows.Forms.TextBox SelectedSpawnUnknown;
        private System.Windows.Forms.ListBox ListTeamComps;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ListBox ListTeams;
        private System.Windows.Forms.ListBox ListTeamMembers;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button TeamCompAdd;
        private System.Windows.Forms.Button TeamCompRemove;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ListBox ListAvailablePlayers;
        private System.Windows.Forms.Button TeamMovePlayerToTeam;
        private System.Windows.Forms.Button TeamKickMember;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox SelectedPlayerName;
        private System.Windows.Forms.TextBox SelectedPlayerLevelRange;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Panel PanelCoopParameters;
        private System.Windows.Forms.Panel PanelSpawnPoints;
        private System.Windows.Forms.Panel PanelMultiplayerCompositions;
        private System.Windows.Forms.ComboBox ComboBindstoneList;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox SelectedPlayerTextID;
        private System.Windows.Forms.Label LabelSelectedPlayerText;
    }
}
