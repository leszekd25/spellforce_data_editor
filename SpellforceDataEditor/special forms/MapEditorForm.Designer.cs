namespace SpellforceDataEditor.special_forms
{
    partial class MapEditorForm
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportHeightmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slopebasedPaintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visibilitySettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMap = new System.Windows.Forms.OpenFileDialog();
            this.TimerAnimation = new System.Windows.Forms.Timer(this.components);
            this.DialogSaveMap = new System.Windows.Forms.SaveFileDialog();
            this.StatusStrip = new System.Windows.Forms.ToolStrip();
            this.StatusText = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SpecificText = new System.Windows.Forms.ToolStripLabel();
            this.PanelInspector = new System.Windows.Forms.Panel();
            this.TabPageDecorations = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.PanelDecalGroups = new System.Windows.Forms.Panel();
            this.TabPageEntities = new System.Windows.Forms.TabPage();
            this.PanelMonumentType = new System.Windows.Forms.Panel();
            this.MonumentHero = new System.Windows.Forms.RadioButton();
            this.label15 = new System.Windows.Forms.Label();
            this.MonumentHuman = new System.Windows.Forms.RadioButton();
            this.MonumentElf = new System.Windows.Forms.RadioButton();
            this.MonumentOrc = new System.Windows.Forms.RadioButton();
            this.MonumentTroll = new System.Windows.Forms.RadioButton();
            this.MonumentDwarf = new System.Windows.Forms.RadioButton();
            this.MonumentDarkElf = new System.Windows.Forms.RadioButton();
            this.EditCoopCampTypes = new System.Windows.Forms.Button();
            this.PanelEntityPlacementSelect = new System.Windows.Forms.Panel();
            this.PanelUnitPlacementSelect = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.UnitName = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.UnitRace = new System.Windows.Forms.ComboBox();
            this.EntityID = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.RadioModeMonuments = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.RadioEntityModeUnit = new System.Windows.Forms.RadioButton();
            this.RadioEntityModeBuilding = new System.Windows.Forms.RadioButton();
            this.RadioModeCoopCamps = new System.Windows.Forms.RadioButton();
            this.RadioModeBindstones = new System.Windows.Forms.RadioButton();
            this.RadioEntityModeObject = new System.Windows.Forms.RadioButton();
            this.RadioModePortals = new System.Windows.Forms.RadioButton();
            this.TabPageTextures = new System.Windows.Forms.TabPage();
            this.ButtonSlopePaint = new System.Windows.Forms.Button();
            this.TTexMatchMovementFlags = new System.Windows.Forms.CheckBox();
            this.ButtonModifyTextureSet = new System.Windows.Forms.Button();
            this.PanelTileType = new System.Windows.Forms.Panel();
            this.RadioTileTypeCustom = new System.Windows.Forms.RadioButton();
            this.RadioTileTypeBase = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.TabPageTerrain = new System.Windows.Forms.TabPage();
            this.PanelFlags = new System.Windows.Forms.Panel();
            this.CheckDisplayFlags = new System.Windows.Forms.CheckBox();
            this.RadioFlagVision = new System.Windows.Forms.RadioButton();
            this.RadioFlagMovement = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.PanelBrushShape = new System.Windows.Forms.Panel();
            this.BrushSizeVal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.RadioDiamond = new System.Windows.Forms.RadioButton();
            this.RadioCircle = new System.Windows.Forms.RadioButton();
            this.RadioSquare = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BrushSizeTrackbar = new System.Windows.Forms.TrackBar();
            this.PanelTerrainSettings = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.RadioIntSinusoidal = new System.Windows.Forms.RadioButton();
            this.RadioIntSquare = new System.Windows.Forms.RadioButton();
            this.RadioIntLinear = new System.Windows.Forms.RadioButton();
            this.RadioIntConstant = new System.Windows.Forms.RadioButton();
            this.panel4 = new System.Windows.Forms.Panel();
            this.RadioModeSmooth = new System.Windows.Forms.RadioButton();
            this.RadioModeSet = new System.Windows.Forms.RadioButton();
            this.RadioModeRaise = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.PanelStrength = new System.Windows.Forms.Panel();
            this.TerrainValueLabel = new System.Windows.Forms.Label();
            this.TerrainValue = new System.Windows.Forms.TextBox();
            this.TerrainTrackbar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.RadioHMap = new System.Windows.Forms.RadioButton();
            this.RadioFlags = new System.Windows.Forms.RadioButton();
            this.RadioLakes = new System.Windows.Forms.RadioButton();
            this.TabEditorModes = new System.Windows.Forms.TabControl();
            this.TabPageMetadata = new System.Windows.Forms.TabPage();
            this.ButtonTeams = new System.Windows.Forms.Button();
            this.PanelCoopParams = new System.Windows.Forms.Panel();
            this.label24 = new System.Windows.Forms.Label();
            this.CoopSpawnParam34 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.CoopSpawnParam33 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.CoopSpawnParam32 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.CoopSpawnParam24 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.CoopSpawnParam23 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.CoopSpawnParam22 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.CoopSpawnParam14 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.CoopSpawnParam13 = new System.Windows.Forms.TextBox();
            this.CoopSpawnParam12 = new System.Windows.Forms.TextBox();
            this.CoopSpawnParam31 = new System.Windows.Forms.TextBox();
            this.CoopSpawnParam21 = new System.Windows.Forms.TextBox();
            this.CoopSpawnParam11 = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.MapTypeCampaign = new System.Windows.Forms.RadioButton();
            this.MapTypeCoop = new System.Windows.Forms.RadioButton();
            this.MapTypeMultiplayer = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.TabPageDecorations.SuspendLayout();
            this.TabPageEntities.SuspendLayout();
            this.PanelMonumentType.SuspendLayout();
            this.PanelEntityPlacementSelect.SuspendLayout();
            this.PanelUnitPlacementSelect.SuspendLayout();
            this.panel5.SuspendLayout();
            this.TabPageTextures.SuspendLayout();
            this.PanelTileType.SuspendLayout();
            this.TabPageTerrain.SuspendLayout();
            this.PanelFlags.SuspendLayout();
            this.PanelBrushShape.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushSizeTrackbar)).BeginInit();
            this.PanelTerrainSettings.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.PanelStrength.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TerrainTrackbar)).BeginInit();
            this.panel1.SuspendLayout();
            this.TabEditorModes.SuspendLayout();
            this.TabPageMetadata.SuspendLayout();
            this.PanelCoopParams.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1100, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewMapToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveMapToolStripMenuItem,
            this.closeMapToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createNewMapToolStripMenuItem
            // 
            this.createNewMapToolStripMenuItem.Name = "createNewMapToolStripMenuItem";
            this.createNewMapToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.createNewMapToolStripMenuItem.Text = "Create new map";
            this.createNewMapToolStripMenuItem.Click += new System.EventHandler(this.createNewMapToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadToolStripMenuItem.Text = "Load map";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.saveMapToolStripMenuItem.Text = "Save map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // closeMapToolStripMenuItem
            // 
            this.closeMapToolStripMenuItem.Name = "closeMapToolStripMenuItem";
            this.closeMapToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeMapToolStripMenuItem.Text = "Close map";
            this.closeMapToolStripMenuItem.Click += new System.EventHandler(this.closeMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importHeightmapToolStripMenuItem,
            this.exportHeightmapToolStripMenuItem,
            this.slopebasedPaintToolStripMenuItem,
            this.visibilitySettingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // importHeightmapToolStripMenuItem
            // 
            this.importHeightmapToolStripMenuItem.Name = "importHeightmapToolStripMenuItem";
            this.importHeightmapToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.importHeightmapToolStripMenuItem.Text = "Import heightmap";
            this.importHeightmapToolStripMenuItem.Click += new System.EventHandler(this.importHeightmapToolStripMenuItem_Click);
            // 
            // exportHeightmapToolStripMenuItem
            // 
            this.exportHeightmapToolStripMenuItem.Name = "exportHeightmapToolStripMenuItem";
            this.exportHeightmapToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.exportHeightmapToolStripMenuItem.Text = "Export heightmap";
            this.exportHeightmapToolStripMenuItem.Click += new System.EventHandler(this.exportHeightmapToolStripMenuItem_Click);
            // 
            // slopebasedPaintToolStripMenuItem
            // 
            this.slopebasedPaintToolStripMenuItem.Name = "slopebasedPaintToolStripMenuItem";
            this.slopebasedPaintToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.slopebasedPaintToolStripMenuItem.Text = "Slope-based paint";
            this.slopebasedPaintToolStripMenuItem.Click += new System.EventHandler(this.slopebasedPaintToolStripMenuItem_Click);
            // 
            // visibilitySettingsToolStripMenuItem
            // 
            this.visibilitySettingsToolStripMenuItem.Name = "visibilitySettingsToolStripMenuItem";
            this.visibilitySettingsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.visibilitySettingsToolStripMenuItem.Text = "Visibility settings";
            this.visibilitySettingsToolStripMenuItem.Click += new System.EventHandler(this.visibilitySettingsToolStripMenuItem_Click);
            // 
            // OpenMap
            // 
            this.OpenMap.DefaultExt = "map";
            this.OpenMap.Filter = "Map file|*.map";
            // 
            // TimerAnimation
            // 
            this.TimerAnimation.Tick += new System.EventHandler(this.TimerAnimation_Tick);
            // 
            // DialogSaveMap
            // 
            this.DialogSaveMap.DefaultExt = "map";
            this.DialogSaveMap.Filter = "Map file | *.map";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText,
            this.toolStripSeparator1,
            this.SpecificText});
            this.StatusStrip.Location = new System.Drawing.Point(0, 642);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(1100, 25);
            this.StatusStrip.TabIndex = 6;
            this.StatusStrip.Text = "toolStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // SpecificText
            // 
            this.SpecificText.Name = "SpecificText";
            this.SpecificText.Size = new System.Drawing.Size(0, 22);
            // 
            // PanelInspector
            // 
            this.PanelInspector.Location = new System.Drawing.Point(822, 172);
            this.PanelInspector.Name = "PanelInspector";
            this.PanelInspector.Size = new System.Drawing.Size(274, 467);
            this.PanelInspector.TabIndex = 8;
            // 
            // TabPageDecorations
            // 
            this.TabPageDecorations.Controls.Add(this.label14);
            this.TabPageDecorations.Controls.Add(this.PanelDecalGroups);
            this.TabPageDecorations.Location = new System.Drawing.Point(4, 25);
            this.TabPageDecorations.Name = "TabPageDecorations";
            this.TabPageDecorations.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageDecorations.Size = new System.Drawing.Size(1092, 114);
            this.TabPageDecorations.TabIndex = 4;
            this.TabPageDecorations.Text = "Decorations";
            this.TabPageDecorations.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(582, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Decoration groups";
            // 
            // PanelDecalGroups
            // 
            this.PanelDecalGroups.AutoScroll = true;
            this.PanelDecalGroups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelDecalGroups.Location = new System.Drawing.Point(366, 15);
            this.PanelDecalGroups.Name = "PanelDecalGroups";
            this.PanelDecalGroups.Size = new System.Drawing.Size(512, 99);
            this.PanelDecalGroups.TabIndex = 0;
            // 
            // TabPageEntities
            // 
            this.TabPageEntities.Controls.Add(this.PanelMonumentType);
            this.TabPageEntities.Controls.Add(this.EditCoopCampTypes);
            this.TabPageEntities.Controls.Add(this.PanelEntityPlacementSelect);
            this.TabPageEntities.Controls.Add(this.panel5);
            this.TabPageEntities.Location = new System.Drawing.Point(4, 25);
            this.TabPageEntities.Name = "TabPageEntities";
            this.TabPageEntities.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageEntities.Size = new System.Drawing.Size(1092, 114);
            this.TabPageEntities.TabIndex = 2;
            this.TabPageEntities.Text = "Entities";
            this.TabPageEntities.UseVisualStyleBackColor = true;
            // 
            // PanelMonumentType
            // 
            this.PanelMonumentType.Controls.Add(this.MonumentHero);
            this.PanelMonumentType.Controls.Add(this.label15);
            this.PanelMonumentType.Controls.Add(this.MonumentHuman);
            this.PanelMonumentType.Controls.Add(this.MonumentElf);
            this.PanelMonumentType.Controls.Add(this.MonumentOrc);
            this.PanelMonumentType.Controls.Add(this.MonumentTroll);
            this.PanelMonumentType.Controls.Add(this.MonumentDwarf);
            this.PanelMonumentType.Controls.Add(this.MonumentDarkElf);
            this.PanelMonumentType.Location = new System.Drawing.Point(580, 3);
            this.PanelMonumentType.Name = "PanelMonumentType";
            this.PanelMonumentType.Size = new System.Drawing.Size(144, 108);
            this.PanelMonumentType.TabIndex = 13;
            this.PanelMonumentType.Visible = false;
            // 
            // MonumentHero
            // 
            this.MonumentHero.AutoSize = true;
            this.MonumentHero.Checked = true;
            this.MonumentHero.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MonumentHero.Location = new System.Drawing.Point(79, 85);
            this.MonumentHero.Name = "MonumentHero";
            this.MonumentHero.Size = new System.Drawing.Size(48, 17);
            this.MonumentHero.TabIndex = 12;
            this.MonumentHero.TabStop = true;
            this.MonumentHero.Text = "Hero";
            this.MonumentHero.UseVisualStyleBackColor = true;
            this.MonumentHero.CheckedChanged += new System.EventHandler(this.MonumentHero_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(28, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Monument type";
            // 
            // MonumentHuman
            // 
            this.MonumentHuman.AutoSize = true;
            this.MonumentHuman.Location = new System.Drawing.Point(3, 16);
            this.MonumentHuman.Name = "MonumentHuman";
            this.MonumentHuman.Size = new System.Drawing.Size(59, 17);
            this.MonumentHuman.TabIndex = 8;
            this.MonumentHuman.Text = "Human";
            this.MonumentHuman.UseVisualStyleBackColor = true;
            this.MonumentHuman.CheckedChanged += new System.EventHandler(this.MonumentHuman_CheckedChanged);
            // 
            // MonumentElf
            // 
            this.MonumentElf.AutoSize = true;
            this.MonumentElf.Location = new System.Drawing.Point(3, 39);
            this.MonumentElf.Name = "MonumentElf";
            this.MonumentElf.Size = new System.Drawing.Size(37, 17);
            this.MonumentElf.TabIndex = 9;
            this.MonumentElf.Text = "Elf";
            this.MonumentElf.UseVisualStyleBackColor = true;
            this.MonumentElf.CheckedChanged += new System.EventHandler(this.MonumentElf_CheckedChanged);
            // 
            // MonumentOrc
            // 
            this.MonumentOrc.AutoSize = true;
            this.MonumentOrc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MonumentOrc.Location = new System.Drawing.Point(79, 16);
            this.MonumentOrc.Name = "MonumentOrc";
            this.MonumentOrc.Size = new System.Drawing.Size(42, 17);
            this.MonumentOrc.TabIndex = 8;
            this.MonumentOrc.Text = "Orc";
            this.MonumentOrc.UseVisualStyleBackColor = true;
            this.MonumentOrc.CheckedChanged += new System.EventHandler(this.MonumentOrc_CheckedChanged);
            // 
            // MonumentTroll
            // 
            this.MonumentTroll.AutoSize = true;
            this.MonumentTroll.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MonumentTroll.Location = new System.Drawing.Point(79, 39);
            this.MonumentTroll.Name = "MonumentTroll";
            this.MonumentTroll.Size = new System.Drawing.Size(45, 17);
            this.MonumentTroll.TabIndex = 9;
            this.MonumentTroll.Text = "Troll";
            this.MonumentTroll.UseVisualStyleBackColor = true;
            this.MonumentTroll.CheckedChanged += new System.EventHandler(this.MonumentTroll_CheckedChanged);
            // 
            // MonumentDwarf
            // 
            this.MonumentDwarf.AutoSize = true;
            this.MonumentDwarf.Location = new System.Drawing.Point(3, 62);
            this.MonumentDwarf.Name = "MonumentDwarf";
            this.MonumentDwarf.Size = new System.Drawing.Size(53, 17);
            this.MonumentDwarf.TabIndex = 10;
            this.MonumentDwarf.Text = "Dwarf";
            this.MonumentDwarf.UseVisualStyleBackColor = true;
            this.MonumentDwarf.CheckedChanged += new System.EventHandler(this.MonumentDwarf_CheckedChanged);
            // 
            // MonumentDarkElf
            // 
            this.MonumentDarkElf.AutoSize = true;
            this.MonumentDarkElf.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MonumentDarkElf.Location = new System.Drawing.Point(79, 62);
            this.MonumentDarkElf.Name = "MonumentDarkElf";
            this.MonumentDarkElf.Size = new System.Drawing.Size(62, 17);
            this.MonumentDarkElf.TabIndex = 10;
            this.MonumentDarkElf.Text = "Dark elf";
            this.MonumentDarkElf.UseVisualStyleBackColor = true;
            this.MonumentDarkElf.CheckedChanged += new System.EventHandler(this.MonumentDarkElf_CheckedChanged);
            // 
            // EditCoopCampTypes
            // 
            this.EditCoopCampTypes.Location = new System.Drawing.Point(486, 3);
            this.EditCoopCampTypes.Name = "EditCoopCampTypes";
            this.EditCoopCampTypes.Size = new System.Drawing.Size(88, 108);
            this.EditCoopCampTypes.TabIndex = 11;
            this.EditCoopCampTypes.Text = "Edit coop camp types...";
            this.EditCoopCampTypes.UseVisualStyleBackColor = true;
            this.EditCoopCampTypes.Click += new System.EventHandler(this.EditCoopCampTypes_Click);
            // 
            // PanelEntityPlacementSelect
            // 
            this.PanelEntityPlacementSelect.Controls.Add(this.PanelUnitPlacementSelect);
            this.PanelEntityPlacementSelect.Controls.Add(this.EntityID);
            this.PanelEntityPlacementSelect.Controls.Add(this.label11);
            this.PanelEntityPlacementSelect.Controls.Add(this.label10);
            this.PanelEntityPlacementSelect.Location = new System.Drawing.Point(187, 3);
            this.PanelEntityPlacementSelect.Name = "PanelEntityPlacementSelect";
            this.PanelEntityPlacementSelect.Size = new System.Drawing.Size(293, 108);
            this.PanelEntityPlacementSelect.TabIndex = 10;
            // 
            // PanelUnitPlacementSelect
            // 
            this.PanelUnitPlacementSelect.Controls.Add(this.label12);
            this.PanelUnitPlacementSelect.Controls.Add(this.UnitName);
            this.PanelUnitPlacementSelect.Controls.Add(this.label13);
            this.PanelUnitPlacementSelect.Controls.Add(this.UnitRace);
            this.PanelUnitPlacementSelect.Location = new System.Drawing.Point(5, 41);
            this.PanelUnitPlacementSelect.Name = "PanelUnitPlacementSelect";
            this.PanelUnitPlacementSelect.Size = new System.Drawing.Size(288, 55);
            this.PanelUnitPlacementSelect.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Race";
            // 
            // UnitName
            // 
            this.UnitName.FormattingEnabled = true;
            this.UnitName.Location = new System.Drawing.Point(50, 30);
            this.UnitName.Name = "UnitName";
            this.UnitName.Size = new System.Drawing.Size(235, 21);
            this.UnitName.TabIndex = 18;
            this.UnitName.SelectedIndexChanged += new System.EventHandler(this.UnitName_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 33);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(26, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Unit";
            // 
            // UnitRace
            // 
            this.UnitRace.FormattingEnabled = true;
            this.UnitRace.Location = new System.Drawing.Point(50, 4);
            this.UnitRace.Name = "UnitRace";
            this.UnitRace.Size = new System.Drawing.Size(235, 21);
            this.UnitRace.TabIndex = 17;
            this.UnitRace.SelectedIndexChanged += new System.EventHandler(this.UnitRace_SelectedIndexChanged);
            // 
            // EntityID
            // 
            this.EntityID.BackColor = System.Drawing.Color.DarkOrange;
            this.EntityID.Location = new System.Drawing.Point(55, 20);
            this.EntityID.Name = "EntityID";
            this.EntityID.Size = new System.Drawing.Size(71, 20);
            this.EntityID.TabIndex = 16;
            this.EntityID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EntityID_MouseDown);
            this.EntityID.Validated += new System.EventHandler(this.EntityID_Validated);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "ID";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(100, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Entity to place";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.RadioModeMonuments);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.RadioEntityModeUnit);
            this.panel5.Controls.Add(this.RadioEntityModeBuilding);
            this.panel5.Controls.Add(this.RadioModeCoopCamps);
            this.panel5.Controls.Add(this.RadioModeBindstones);
            this.panel5.Controls.Add(this.RadioEntityModeObject);
            this.panel5.Controls.Add(this.RadioModePortals);
            this.panel5.Location = new System.Drawing.Point(8, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(173, 108);
            this.panel5.TabIndex = 9;
            // 
            // RadioModeMonuments
            // 
            this.RadioModeMonuments.AutoSize = true;
            this.RadioModeMonuments.ForeColor = System.Drawing.Color.Green;
            this.RadioModeMonuments.Location = new System.Drawing.Point(79, 85);
            this.RadioModeMonuments.Name = "RadioModeMonuments";
            this.RadioModeMonuments.Size = new System.Drawing.Size(80, 17);
            this.RadioModeMonuments.TabIndex = 12;
            this.RadioModeMonuments.Text = "Monuments";
            this.RadioModeMonuments.UseVisualStyleBackColor = true;
            this.RadioModeMonuments.CheckedChanged += new System.EventHandler(this.RadioModeMonuments_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Editor mode";
            // 
            // RadioEntityModeUnit
            // 
            this.RadioEntityModeUnit.AutoSize = true;
            this.RadioEntityModeUnit.Checked = true;
            this.RadioEntityModeUnit.Location = new System.Drawing.Point(3, 16);
            this.RadioEntityModeUnit.Name = "RadioEntityModeUnit";
            this.RadioEntityModeUnit.Size = new System.Drawing.Size(49, 17);
            this.RadioEntityModeUnit.TabIndex = 8;
            this.RadioEntityModeUnit.TabStop = true;
            this.RadioEntityModeUnit.Text = "Units";
            this.RadioEntityModeUnit.UseVisualStyleBackColor = true;
            this.RadioEntityModeUnit.CheckedChanged += new System.EventHandler(this.RadioEntityModeUnit_CheckedChanged);
            // 
            // RadioEntityModeBuilding
            // 
            this.RadioEntityModeBuilding.AutoSize = true;
            this.RadioEntityModeBuilding.Location = new System.Drawing.Point(3, 39);
            this.RadioEntityModeBuilding.Name = "RadioEntityModeBuilding";
            this.RadioEntityModeBuilding.Size = new System.Drawing.Size(67, 17);
            this.RadioEntityModeBuilding.TabIndex = 9;
            this.RadioEntityModeBuilding.Text = "Buildings";
            this.RadioEntityModeBuilding.UseVisualStyleBackColor = true;
            this.RadioEntityModeBuilding.CheckedChanged += new System.EventHandler(this.RadioEntityModeBuilding_CheckedChanged);
            // 
            // RadioModeCoopCamps
            // 
            this.RadioModeCoopCamps.AutoSize = true;
            this.RadioModeCoopCamps.ForeColor = System.Drawing.Color.Green;
            this.RadioModeCoopCamps.Location = new System.Drawing.Point(79, 16);
            this.RadioModeCoopCamps.Name = "RadioModeCoopCamps";
            this.RadioModeCoopCamps.Size = new System.Drawing.Size(84, 17);
            this.RadioModeCoopCamps.TabIndex = 8;
            this.RadioModeCoopCamps.Text = "Coop camps";
            this.RadioModeCoopCamps.UseVisualStyleBackColor = true;
            this.RadioModeCoopCamps.CheckedChanged += new System.EventHandler(this.RadioModeCoopCamps_CheckedChanged);
            // 
            // RadioModeBindstones
            // 
            this.RadioModeBindstones.AutoSize = true;
            this.RadioModeBindstones.ForeColor = System.Drawing.Color.Green;
            this.RadioModeBindstones.Location = new System.Drawing.Point(79, 39);
            this.RadioModeBindstones.Name = "RadioModeBindstones";
            this.RadioModeBindstones.Size = new System.Drawing.Size(77, 17);
            this.RadioModeBindstones.TabIndex = 9;
            this.RadioModeBindstones.Text = "Bindstones";
            this.RadioModeBindstones.UseVisualStyleBackColor = true;
            this.RadioModeBindstones.CheckedChanged += new System.EventHandler(this.RadioModeBindstones_CheckedChanged);
            // 
            // RadioEntityModeObject
            // 
            this.RadioEntityModeObject.AutoSize = true;
            this.RadioEntityModeObject.Location = new System.Drawing.Point(3, 62);
            this.RadioEntityModeObject.Name = "RadioEntityModeObject";
            this.RadioEntityModeObject.Size = new System.Drawing.Size(61, 17);
            this.RadioEntityModeObject.TabIndex = 10;
            this.RadioEntityModeObject.Text = "Objects";
            this.RadioEntityModeObject.UseVisualStyleBackColor = true;
            this.RadioEntityModeObject.CheckedChanged += new System.EventHandler(this.RadioEntityModeObject_CheckedChanged);
            // 
            // RadioModePortals
            // 
            this.RadioModePortals.AutoSize = true;
            this.RadioModePortals.ForeColor = System.Drawing.Color.Green;
            this.RadioModePortals.Location = new System.Drawing.Point(79, 62);
            this.RadioModePortals.Name = "RadioModePortals";
            this.RadioModePortals.Size = new System.Drawing.Size(57, 17);
            this.RadioModePortals.TabIndex = 10;
            this.RadioModePortals.Text = "Portals";
            this.RadioModePortals.UseVisualStyleBackColor = true;
            this.RadioModePortals.CheckedChanged += new System.EventHandler(this.RadioModePortals_CheckedChanged);
            // 
            // TabPageTextures
            // 
            this.TabPageTextures.Controls.Add(this.ButtonSlopePaint);
            this.TabPageTextures.Controls.Add(this.TTexMatchMovementFlags);
            this.TabPageTextures.Controls.Add(this.ButtonModifyTextureSet);
            this.TabPageTextures.Controls.Add(this.PanelTileType);
            this.TabPageTextures.Location = new System.Drawing.Point(4, 25);
            this.TabPageTextures.Name = "TabPageTextures";
            this.TabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTextures.Size = new System.Drawing.Size(1092, 114);
            this.TabPageTextures.TabIndex = 1;
            this.TabPageTextures.Text = "Textures";
            this.TabPageTextures.UseVisualStyleBackColor = true;
            // 
            // ButtonSlopePaint
            // 
            this.ButtonSlopePaint.Location = new System.Drawing.Point(444, 6);
            this.ButtonSlopePaint.Name = "ButtonSlopePaint";
            this.ButtonSlopePaint.Size = new System.Drawing.Size(90, 102);
            this.ButtonSlopePaint.TabIndex = 20;
            this.ButtonSlopePaint.Text = "Slope-based paint...";
            this.ButtonSlopePaint.UseVisualStyleBackColor = true;
            this.ButtonSlopePaint.Click += new System.EventHandler(this.ButtonSlopePaint_Click);
            // 
            // TTexMatchMovementFlags
            // 
            this.TTexMatchMovementFlags.AutoSize = true;
            this.TTexMatchMovementFlags.Location = new System.Drawing.Point(102, 91);
            this.TTexMatchMovementFlags.Name = "TTexMatchMovementFlags";
            this.TTexMatchMovementFlags.Size = new System.Drawing.Size(133, 17);
            this.TTexMatchMovementFlags.TabIndex = 18;
            this.TTexMatchMovementFlags.Text = "Match movement flags";
            this.TTexMatchMovementFlags.UseVisualStyleBackColor = true;
            this.TTexMatchMovementFlags.CheckedChanged += new System.EventHandler(this.TTexMatchMovementFlags_CheckedChanged);
            // 
            // ButtonModifyTextureSet
            // 
            this.ButtonModifyTextureSet.Location = new System.Drawing.Point(6, 6);
            this.ButtonModifyTextureSet.Name = "ButtonModifyTextureSet";
            this.ButtonModifyTextureSet.Size = new System.Drawing.Size(90, 102);
            this.ButtonModifyTextureSet.TabIndex = 19;
            this.ButtonModifyTextureSet.Text = "Modify texture set...";
            this.ButtonModifyTextureSet.UseVisualStyleBackColor = true;
            this.ButtonModifyTextureSet.Click += new System.EventHandler(this.ButtonModifyTextureSet_Click);
            // 
            // PanelTileType
            // 
            this.PanelTileType.Controls.Add(this.RadioTileTypeCustom);
            this.PanelTileType.Controls.Add(this.RadioTileTypeBase);
            this.PanelTileType.Controls.Add(this.label9);
            this.PanelTileType.Location = new System.Drawing.Point(366, 6);
            this.PanelTileType.Name = "PanelTileType";
            this.PanelTileType.Size = new System.Drawing.Size(72, 102);
            this.PanelTileType.TabIndex = 18;
            // 
            // RadioTileTypeCustom
            // 
            this.RadioTileTypeCustom.AutoSize = true;
            this.RadioTileTypeCustom.Location = new System.Drawing.Point(6, 45);
            this.RadioTileTypeCustom.Name = "RadioTileTypeCustom";
            this.RadioTileTypeCustom.Size = new System.Drawing.Size(60, 17);
            this.RadioTileTypeCustom.TabIndex = 16;
            this.RadioTileTypeCustom.Text = "Custom";
            this.RadioTileTypeCustom.UseVisualStyleBackColor = true;
            this.RadioTileTypeCustom.CheckedChanged += new System.EventHandler(this.RadioTileTypeCustom_CheckedChanged);
            // 
            // RadioTileTypeBase
            // 
            this.RadioTileTypeBase.AutoSize = true;
            this.RadioTileTypeBase.Checked = true;
            this.RadioTileTypeBase.Location = new System.Drawing.Point(6, 22);
            this.RadioTileTypeBase.Name = "RadioTileTypeBase";
            this.RadioTileTypeBase.Size = new System.Drawing.Size(49, 17);
            this.RadioTileTypeBase.TabIndex = 15;
            this.RadioTileTypeBase.TabStop = true;
            this.RadioTileTypeBase.Text = "Base";
            this.RadioTileTypeBase.UseVisualStyleBackColor = true;
            this.RadioTileTypeBase.CheckedChanged += new System.EventHandler(this.RadioTileTypeBase_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Tile type";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TabPageTerrain
            // 
            this.TabPageTerrain.Controls.Add(this.PanelFlags);
            this.TabPageTerrain.Controls.Add(this.PanelBrushShape);
            this.TabPageTerrain.Controls.Add(this.PanelTerrainSettings);
            this.TabPageTerrain.Controls.Add(this.PanelStrength);
            this.TabPageTerrain.Controls.Add(this.panel1);
            this.TabPageTerrain.Location = new System.Drawing.Point(4, 25);
            this.TabPageTerrain.Name = "TabPageTerrain";
            this.TabPageTerrain.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTerrain.Size = new System.Drawing.Size(1092, 114);
            this.TabPageTerrain.TabIndex = 0;
            this.TabPageTerrain.Text = "Terrain";
            this.TabPageTerrain.UseVisualStyleBackColor = true;
            // 
            // PanelFlags
            // 
            this.PanelFlags.Controls.Add(this.CheckDisplayFlags);
            this.PanelFlags.Controls.Add(this.RadioFlagVision);
            this.PanelFlags.Controls.Add(this.RadioFlagMovement);
            this.PanelFlags.Controls.Add(this.label8);
            this.PanelFlags.Location = new System.Drawing.Point(675, 3);
            this.PanelFlags.Name = "PanelFlags";
            this.PanelFlags.Size = new System.Drawing.Size(207, 73);
            this.PanelFlags.TabIndex = 8;
            this.PanelFlags.Visible = false;
            // 
            // CheckDisplayFlags
            // 
            this.CheckDisplayFlags.AutoSize = true;
            this.CheckDisplayFlags.Location = new System.Drawing.Point(59, 41);
            this.CheckDisplayFlags.Name = "CheckDisplayFlags";
            this.CheckDisplayFlags.Size = new System.Drawing.Size(85, 17);
            this.CheckDisplayFlags.TabIndex = 4;
            this.CheckDisplayFlags.Text = "Display flags";
            this.CheckDisplayFlags.UseVisualStyleBackColor = true;
            this.CheckDisplayFlags.CheckedChanged += new System.EventHandler(this.CheckDisplayFlags_CheckedChanged);
            // 
            // RadioFlagVision
            // 
            this.RadioFlagVision.AutoSize = true;
            this.RadioFlagVision.Location = new System.Drawing.Point(140, 16);
            this.RadioFlagVision.Name = "RadioFlagVision";
            this.RadioFlagVision.Size = new System.Drawing.Size(53, 17);
            this.RadioFlagVision.TabIndex = 2;
            this.RadioFlagVision.Text = "Vision";
            this.RadioFlagVision.UseVisualStyleBackColor = true;
            this.RadioFlagVision.CheckedChanged += new System.EventHandler(this.RadioFlagVision_CheckedChanged);
            // 
            // RadioFlagMovement
            // 
            this.RadioFlagMovement.AutoSize = true;
            this.RadioFlagMovement.Checked = true;
            this.RadioFlagMovement.Location = new System.Drawing.Point(59, 16);
            this.RadioFlagMovement.Name = "RadioFlagMovement";
            this.RadioFlagMovement.Size = new System.Drawing.Size(75, 17);
            this.RadioFlagMovement.TabIndex = 1;
            this.RadioFlagMovement.TabStop = true;
            this.RadioFlagMovement.Text = "Movement";
            this.RadioFlagMovement.UseVisualStyleBackColor = true;
            this.RadioFlagMovement.CheckedChanged += new System.EventHandler(this.RadioFlagMovement_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Flag type";
            // 
            // PanelBrushShape
            // 
            this.PanelBrushShape.Controls.Add(this.BrushSizeVal);
            this.PanelBrushShape.Controls.Add(this.label3);
            this.PanelBrushShape.Controls.Add(this.panel2);
            this.PanelBrushShape.Controls.Add(this.label4);
            this.PanelBrushShape.Controls.Add(this.label2);
            this.PanelBrushShape.Controls.Add(this.BrushSizeTrackbar);
            this.PanelBrushShape.Location = new System.Drawing.Point(102, 3);
            this.PanelBrushShape.Name = "PanelBrushShape";
            this.PanelBrushShape.Size = new System.Drawing.Size(261, 73);
            this.PanelBrushShape.TabIndex = 8;
            // 
            // BrushSizeVal
            // 
            this.BrushSizeVal.Location = new System.Drawing.Point(62, 18);
            this.BrushSizeVal.Name = "BrushSizeVal";
            this.BrushSizeVal.Size = new System.Drawing.Size(42, 20);
            this.BrushSizeVal.TabIndex = 12;
            this.BrushSizeVal.Text = "3";
            this.BrushSizeVal.Validated += new System.EventHandler(this.BrushSizeVal_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Size";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RadioDiamond);
            this.panel2.Controls.Add(this.RadioCircle);
            this.panel2.Controls.Add(this.RadioSquare);
            this.panel2.Location = new System.Drawing.Point(62, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(196, 28);
            this.panel2.TabIndex = 8;
            // 
            // RadioDiamond
            // 
            this.RadioDiamond.AutoSize = true;
            this.RadioDiamond.Location = new System.Drawing.Point(125, 5);
            this.RadioDiamond.Name = "RadioDiamond";
            this.RadioDiamond.Size = new System.Drawing.Size(67, 17);
            this.RadioDiamond.TabIndex = 2;
            this.RadioDiamond.Text = "Diamond";
            this.RadioDiamond.UseVisualStyleBackColor = true;
            this.RadioDiamond.CheckedChanged += new System.EventHandler(this.RadioDiamond_CheckedChanged);
            // 
            // RadioCircle
            // 
            this.RadioCircle.AutoSize = true;
            this.RadioCircle.Checked = true;
            this.RadioCircle.Location = new System.Drawing.Point(68, 5);
            this.RadioCircle.Name = "RadioCircle";
            this.RadioCircle.Size = new System.Drawing.Size(51, 17);
            this.RadioCircle.TabIndex = 1;
            this.RadioCircle.TabStop = true;
            this.RadioCircle.Text = "Circle";
            this.RadioCircle.UseVisualStyleBackColor = true;
            this.RadioCircle.CheckedChanged += new System.EventHandler(this.RadioCircle_CheckedChanged);
            // 
            // RadioSquare
            // 
            this.RadioSquare.AutoSize = true;
            this.RadioSquare.Location = new System.Drawing.Point(3, 5);
            this.RadioSquare.Name = "RadioSquare";
            this.RadioSquare.Size = new System.Drawing.Size(59, 17);
            this.RadioSquare.TabIndex = 0;
            this.RadioSquare.Text = "Square";
            this.RadioSquare.UseVisualStyleBackColor = true;
            this.RadioSquare.CheckedChanged += new System.EventHandler(this.RadioSquare_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Shape";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Brush settings";
            // 
            // BrushSizeTrackbar
            // 
            this.BrushSizeTrackbar.AutoSize = false;
            this.BrushSizeTrackbar.Location = new System.Drawing.Point(110, 18);
            this.BrushSizeTrackbar.Maximum = 15;
            this.BrushSizeTrackbar.Minimum = 1;
            this.BrushSizeTrackbar.Name = "BrushSizeTrackbar";
            this.BrushSizeTrackbar.Size = new System.Drawing.Size(104, 20);
            this.BrushSizeTrackbar.TabIndex = 8;
            this.BrushSizeTrackbar.TickFrequency = 2;
            this.BrushSizeTrackbar.Value = 3;
            this.BrushSizeTrackbar.ValueChanged += new System.EventHandler(this.BrushSizeTrackbar_ValueChanged);
            // 
            // PanelTerrainSettings
            // 
            this.PanelTerrainSettings.Controls.Add(this.label5);
            this.PanelTerrainSettings.Controls.Add(this.panel3);
            this.PanelTerrainSettings.Controls.Add(this.panel4);
            this.PanelTerrainSettings.Controls.Add(this.label7);
            this.PanelTerrainSettings.Location = new System.Drawing.Point(366, 3);
            this.PanelTerrainSettings.Name = "PanelTerrainSettings";
            this.PanelTerrainSettings.Size = new System.Drawing.Size(303, 108);
            this.PanelTerrainSettings.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Interpolation";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.RadioIntSinusoidal);
            this.panel3.Controls.Add(this.RadioIntSquare);
            this.panel3.Controls.Add(this.RadioIntLinear);
            this.panel3.Controls.Add(this.RadioIntConstant);
            this.panel3.Location = new System.Drawing.Point(84, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(177, 54);
            this.panel3.TabIndex = 9;
            // 
            // RadioIntSinusoidal
            // 
            this.RadioIntSinusoidal.AutoSize = true;
            this.RadioIntSinusoidal.Checked = true;
            this.RadioIntSinusoidal.Location = new System.Drawing.Point(76, 28);
            this.RadioIntSinusoidal.Name = "RadioIntSinusoidal";
            this.RadioIntSinusoidal.Size = new System.Drawing.Size(73, 17);
            this.RadioIntSinusoidal.TabIndex = 3;
            this.RadioIntSinusoidal.TabStop = true;
            this.RadioIntSinusoidal.Text = "Sinusoidal";
            this.RadioIntSinusoidal.UseVisualStyleBackColor = true;
            this.RadioIntSinusoidal.CheckedChanged += new System.EventHandler(this.RadioIntSinusoidal_CheckedChanged);
            // 
            // RadioIntSquare
            // 
            this.RadioIntSquare.AutoSize = true;
            this.RadioIntSquare.Location = new System.Drawing.Point(3, 28);
            this.RadioIntSquare.Name = "RadioIntSquare";
            this.RadioIntSquare.Size = new System.Drawing.Size(59, 17);
            this.RadioIntSquare.TabIndex = 2;
            this.RadioIntSquare.Text = "Square";
            this.RadioIntSquare.UseVisualStyleBackColor = true;
            this.RadioIntSquare.CheckedChanged += new System.EventHandler(this.RadioIntSquare_CheckedChanged);
            // 
            // RadioIntLinear
            // 
            this.RadioIntLinear.AutoSize = true;
            this.RadioIntLinear.Location = new System.Drawing.Point(76, 5);
            this.RadioIntLinear.Name = "RadioIntLinear";
            this.RadioIntLinear.Size = new System.Drawing.Size(54, 17);
            this.RadioIntLinear.TabIndex = 1;
            this.RadioIntLinear.Text = "Linear";
            this.RadioIntLinear.UseVisualStyleBackColor = true;
            this.RadioIntLinear.CheckedChanged += new System.EventHandler(this.RadioIntLinear_CheckedChanged);
            // 
            // RadioIntConstant
            // 
            this.RadioIntConstant.AutoSize = true;
            this.RadioIntConstant.Location = new System.Drawing.Point(3, 5);
            this.RadioIntConstant.Name = "RadioIntConstant";
            this.RadioIntConstant.Size = new System.Drawing.Size(67, 17);
            this.RadioIntConstant.TabIndex = 0;
            this.RadioIntConstant.Text = "Constant";
            this.RadioIntConstant.UseVisualStyleBackColor = true;
            this.RadioIntConstant.CheckedChanged += new System.EventHandler(this.RadioIntConstant_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.RadioModeSmooth);
            this.panel4.Controls.Add(this.RadioModeSet);
            this.panel4.Controls.Add(this.RadioModeRaise);
            this.panel4.Location = new System.Drawing.Point(84, 75);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(177, 28);
            this.panel4.TabIndex = 9;
            // 
            // RadioModeSmooth
            // 
            this.RadioModeSmooth.AutoSize = true;
            this.RadioModeSmooth.Location = new System.Drawing.Point(108, 5);
            this.RadioModeSmooth.Name = "RadioModeSmooth";
            this.RadioModeSmooth.Size = new System.Drawing.Size(61, 17);
            this.RadioModeSmooth.TabIndex = 2;
            this.RadioModeSmooth.Text = "Smooth";
            this.RadioModeSmooth.UseVisualStyleBackColor = true;
            this.RadioModeSmooth.CheckedChanged += new System.EventHandler(this.RadioModeSmooth_CheckedChanged);
            // 
            // RadioModeSet
            // 
            this.RadioModeSet.AutoSize = true;
            this.RadioModeSet.Location = new System.Drawing.Point(61, 5);
            this.RadioModeSet.Name = "RadioModeSet";
            this.RadioModeSet.Size = new System.Drawing.Size(41, 17);
            this.RadioModeSet.TabIndex = 1;
            this.RadioModeSet.Text = "Set";
            this.RadioModeSet.UseVisualStyleBackColor = true;
            this.RadioModeSet.CheckedChanged += new System.EventHandler(this.RadioModeSet_CheckedChanged);
            // 
            // RadioModeRaise
            // 
            this.RadioModeRaise.AutoSize = true;
            this.RadioModeRaise.Checked = true;
            this.RadioModeRaise.Location = new System.Drawing.Point(3, 5);
            this.RadioModeRaise.Name = "RadioModeRaise";
            this.RadioModeRaise.Size = new System.Drawing.Size(52, 17);
            this.RadioModeRaise.TabIndex = 0;
            this.RadioModeRaise.TabStop = true;
            this.RadioModeRaise.Text = "Raise";
            this.RadioModeRaise.UseVisualStyleBackColor = true;
            this.RadioModeRaise.CheckedChanged += new System.EventHandler(this.RadioModeRaise_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(44, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Mode";
            // 
            // PanelStrength
            // 
            this.PanelStrength.Controls.Add(this.TerrainValueLabel);
            this.PanelStrength.Controls.Add(this.TerrainValue);
            this.PanelStrength.Controls.Add(this.TerrainTrackbar);
            this.PanelStrength.Location = new System.Drawing.Point(102, 79);
            this.PanelStrength.Name = "PanelStrength";
            this.PanelStrength.Size = new System.Drawing.Size(261, 29);
            this.PanelStrength.TabIndex = 17;
            // 
            // TerrainValueLabel
            // 
            this.TerrainValueLabel.AutoSize = true;
            this.TerrainValueLabel.Location = new System.Drawing.Point(9, 6);
            this.TerrainValueLabel.Name = "TerrainValueLabel";
            this.TerrainValueLabel.Size = new System.Drawing.Size(47, 13);
            this.TerrainValueLabel.TabIndex = 14;
            this.TerrainValueLabel.Text = "Strength";
            this.TerrainValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TerrainValue
            // 
            this.TerrainValue.Location = new System.Drawing.Point(62, 3);
            this.TerrainValue.Name = "TerrainValue";
            this.TerrainValue.Size = new System.Drawing.Size(42, 20);
            this.TerrainValue.TabIndex = 15;
            this.TerrainValue.Text = "20";
            this.TerrainValue.Validated += new System.EventHandler(this.TerrainValue_Validated);
            // 
            // TerrainTrackbar
            // 
            this.TerrainTrackbar.AutoSize = false;
            this.TerrainTrackbar.Location = new System.Drawing.Point(110, 3);
            this.TerrainTrackbar.Maximum = 100;
            this.TerrainTrackbar.Name = "TerrainTrackbar";
            this.TerrainTrackbar.Size = new System.Drawing.Size(104, 20);
            this.TerrainTrackbar.TabIndex = 13;
            this.TerrainTrackbar.TickFrequency = 10;
            this.TerrainTrackbar.Value = 20;
            this.TerrainTrackbar.ValueChanged += new System.EventHandler(this.TerrainTrackbar_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.RadioHMap);
            this.panel1.Controls.Add(this.RadioFlags);
            this.panel1.Controls.Add(this.RadioLakes);
            this.panel1.Location = new System.Drawing.Point(6, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(93, 108);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Editor mode";
            // 
            // RadioHMap
            // 
            this.RadioHMap.AutoSize = true;
            this.RadioHMap.Checked = true;
            this.RadioHMap.Location = new System.Drawing.Point(3, 16);
            this.RadioHMap.Name = "RadioHMap";
            this.RadioHMap.Size = new System.Drawing.Size(76, 17);
            this.RadioHMap.TabIndex = 8;
            this.RadioHMap.TabStop = true;
            this.RadioHMap.Text = "Heightmap";
            this.RadioHMap.UseVisualStyleBackColor = true;
            this.RadioHMap.CheckedChanged += new System.EventHandler(this.RadioHMap_CheckedChanged);
            // 
            // RadioFlags
            // 
            this.RadioFlags.AutoSize = true;
            this.RadioFlags.Location = new System.Drawing.Point(3, 39);
            this.RadioFlags.Name = "RadioFlags";
            this.RadioFlags.Size = new System.Drawing.Size(83, 17);
            this.RadioFlags.TabIndex = 9;
            this.RadioFlags.Text = "Terrain flags";
            this.RadioFlags.UseVisualStyleBackColor = true;
            this.RadioFlags.CheckedChanged += new System.EventHandler(this.RadioFlags_CheckedChanged);
            // 
            // RadioLakes
            // 
            this.RadioLakes.AutoSize = true;
            this.RadioLakes.Location = new System.Drawing.Point(3, 62);
            this.RadioLakes.Name = "RadioLakes";
            this.RadioLakes.Size = new System.Drawing.Size(49, 17);
            this.RadioLakes.TabIndex = 10;
            this.RadioLakes.Text = "Lake";
            this.RadioLakes.UseVisualStyleBackColor = true;
            this.RadioLakes.CheckedChanged += new System.EventHandler(this.RadioLakes_CheckedChanged);
            // 
            // TabEditorModes
            // 
            this.TabEditorModes.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TabEditorModes.Controls.Add(this.TabPageTerrain);
            this.TabEditorModes.Controls.Add(this.TabPageTextures);
            this.TabEditorModes.Controls.Add(this.TabPageEntities);
            this.TabEditorModes.Controls.Add(this.TabPageDecorations);
            this.TabEditorModes.Controls.Add(this.TabPageMetadata);
            this.TabEditorModes.Enabled = false;
            this.TabEditorModes.Location = new System.Drawing.Point(0, 27);
            this.TabEditorModes.Name = "TabEditorModes";
            this.TabEditorModes.Padding = new System.Drawing.Point(72, 3);
            this.TabEditorModes.SelectedIndex = 0;
            this.TabEditorModes.Size = new System.Drawing.Size(1100, 143);
            this.TabEditorModes.TabIndex = 7;
            this.TabEditorModes.SelectedIndexChanged += new System.EventHandler(this.TabEditorModes_SelectedIndexChanged);
            // 
            // TabPageMetadata
            // 
            this.TabPageMetadata.Controls.Add(this.ButtonTeams);
            this.TabPageMetadata.Controls.Add(this.PanelCoopParams);
            this.TabPageMetadata.Controls.Add(this.panel6);
            this.TabPageMetadata.Location = new System.Drawing.Point(4, 25);
            this.TabPageMetadata.Name = "TabPageMetadata";
            this.TabPageMetadata.Size = new System.Drawing.Size(1092, 114);
            this.TabPageMetadata.TabIndex = 6;
            this.TabPageMetadata.Text = "Metadata";
            this.TabPageMetadata.UseVisualStyleBackColor = true;
            // 
            // ButtonTeams
            // 
            this.ButtonTeams.Location = new System.Drawing.Point(102, 3);
            this.ButtonTeams.Name = "ButtonTeams";
            this.ButtonTeams.Size = new System.Drawing.Size(95, 108);
            this.ButtonTeams.TabIndex = 11;
            this.ButtonTeams.Text = "Manage team compositions...";
            this.ButtonTeams.UseVisualStyleBackColor = true;
            this.ButtonTeams.Click += new System.EventHandler(this.ButtonTeams_Click);
            // 
            // PanelCoopParams
            // 
            this.PanelCoopParams.Controls.Add(this.label24);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam34);
            this.PanelCoopParams.Controls.Add(this.label23);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam33);
            this.PanelCoopParams.Controls.Add(this.label22);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam32);
            this.PanelCoopParams.Controls.Add(this.label21);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam24);
            this.PanelCoopParams.Controls.Add(this.label20);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam23);
            this.PanelCoopParams.Controls.Add(this.label19);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam22);
            this.PanelCoopParams.Controls.Add(this.label18);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam14);
            this.PanelCoopParams.Controls.Add(this.label17);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam13);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam12);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam31);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam21);
            this.PanelCoopParams.Controls.Add(this.CoopSpawnParam11);
            this.PanelCoopParams.Location = new System.Drawing.Point(203, 3);
            this.PanelCoopParams.Name = "PanelCoopParams";
            this.PanelCoopParams.Size = new System.Drawing.Size(482, 108);
            this.PanelCoopParams.TabIndex = 10;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 88);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(49, 13);
            this.label24.TabIndex = 16;
            this.label24.Text = "3 players";
            // 
            // CoopSpawnParam34
            // 
            this.CoopSpawnParam34.Location = new System.Drawing.Point(376, 85);
            this.CoopSpawnParam34.Name = "CoopSpawnParam34";
            this.CoopSpawnParam34.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam34.TabIndex = 11;
            this.CoopSpawnParam34.Validated += new System.EventHandler(this.CoopSpawnParam34_Validated);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 62);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(49, 13);
            this.label23.TabIndex = 15;
            this.label23.Text = "2 players";
            // 
            // CoopSpawnParam33
            // 
            this.CoopSpawnParam33.Location = new System.Drawing.Point(270, 85);
            this.CoopSpawnParam33.Name = "CoopSpawnParam33";
            this.CoopSpawnParam33.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam33.TabIndex = 10;
            this.CoopSpawnParam33.Validated += new System.EventHandler(this.CoopSpawnParam33_Validated);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 36);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(44, 13);
            this.label22.TabIndex = 14;
            this.label22.Text = "1 player";
            // 
            // CoopSpawnParam32
            // 
            this.CoopSpawnParam32.Location = new System.Drawing.Point(164, 85);
            this.CoopSpawnParam32.Name = "CoopSpawnParam32";
            this.CoopSpawnParam32.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam32.TabIndex = 9;
            this.CoopSpawnParam32.Validated += new System.EventHandler(this.CoopSpawnParam32_Validated);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(390, 16);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(68, 13);
            this.label21.TabIndex = 13;
            this.label21.Text = "Spawn delay";
            // 
            // CoopSpawnParam24
            // 
            this.CoopSpawnParam24.Location = new System.Drawing.Point(376, 59);
            this.CoopSpawnParam24.Name = "CoopSpawnParam24";
            this.CoopSpawnParam24.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam24.TabIndex = 8;
            this.CoopSpawnParam24.Validated += new System.EventHandler(this.CoopSpawnParam24_Validated);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(289, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(63, 13);
            this.label20.TabIndex = 12;
            this.label20.Text = "Begin wave";
            // 
            // CoopSpawnParam23
            // 
            this.CoopSpawnParam23.Location = new System.Drawing.Point(270, 59);
            this.CoopSpawnParam23.Name = "CoopSpawnParam23";
            this.CoopSpawnParam23.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam23.TabIndex = 7;
            this.CoopSpawnParam23.Validated += new System.EventHandler(this.CoopSpawnParam23_Validated);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(187, 17);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 13);
            this.label19.TabIndex = 11;
            this.label19.Text = "Init spawn";
            // 
            // CoopSpawnParam22
            // 
            this.CoopSpawnParam22.Location = new System.Drawing.Point(164, 59);
            this.CoopSpawnParam22.Name = "CoopSpawnParam22";
            this.CoopSpawnParam22.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam22.TabIndex = 6;
            this.CoopSpawnParam22.Validated += new System.EventHandler(this.CoopSpawnParam22_Validated);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(74, 18);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 13);
            this.label18.TabIndex = 10;
            this.label18.Text = "Max clan size";
            // 
            // CoopSpawnParam14
            // 
            this.CoopSpawnParam14.Location = new System.Drawing.Point(376, 33);
            this.CoopSpawnParam14.Name = "CoopSpawnParam14";
            this.CoopSpawnParam14.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam14.TabIndex = 5;
            this.CoopSpawnParam14.Validated += new System.EventHandler(this.CoopSpawnParam14_Validated);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(161, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(121, 13);
            this.label17.TabIndex = 9;
            this.label17.Text = "Coop spawn parameters";
            // 
            // CoopSpawnParam13
            // 
            this.CoopSpawnParam13.Location = new System.Drawing.Point(270, 33);
            this.CoopSpawnParam13.Name = "CoopSpawnParam13";
            this.CoopSpawnParam13.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam13.TabIndex = 4;
            this.CoopSpawnParam13.Validated += new System.EventHandler(this.CoopSpawnParam13_Validated);
            // 
            // CoopSpawnParam12
            // 
            this.CoopSpawnParam12.Location = new System.Drawing.Point(164, 33);
            this.CoopSpawnParam12.Name = "CoopSpawnParam12";
            this.CoopSpawnParam12.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam12.TabIndex = 3;
            this.CoopSpawnParam12.Validated += new System.EventHandler(this.CoopSpawnParam12_Validated);
            // 
            // CoopSpawnParam31
            // 
            this.CoopSpawnParam31.Location = new System.Drawing.Point(58, 85);
            this.CoopSpawnParam31.Name = "CoopSpawnParam31";
            this.CoopSpawnParam31.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam31.TabIndex = 2;
            this.CoopSpawnParam31.Validated += new System.EventHandler(this.CoopSpawnParam31_Validated);
            // 
            // CoopSpawnParam21
            // 
            this.CoopSpawnParam21.Location = new System.Drawing.Point(58, 59);
            this.CoopSpawnParam21.Name = "CoopSpawnParam21";
            this.CoopSpawnParam21.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam21.TabIndex = 1;
            this.CoopSpawnParam21.Validated += new System.EventHandler(this.CoopSpawnParam21_Validated);
            // 
            // CoopSpawnParam11
            // 
            this.CoopSpawnParam11.Location = new System.Drawing.Point(58, 33);
            this.CoopSpawnParam11.Name = "CoopSpawnParam11";
            this.CoopSpawnParam11.Size = new System.Drawing.Size(100, 20);
            this.CoopSpawnParam11.TabIndex = 0;
            this.CoopSpawnParam11.Validated += new System.EventHandler(this.CoopSpawnParam11_Validated);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label16);
            this.panel6.Controls.Add(this.MapTypeCampaign);
            this.panel6.Controls.Add(this.MapTypeCoop);
            this.panel6.Controls.Add(this.MapTypeMultiplayer);
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(93, 108);
            this.panel6.TabIndex = 9;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(51, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "Map type";
            // 
            // MapTypeCampaign
            // 
            this.MapTypeCampaign.AutoSize = true;
            this.MapTypeCampaign.Location = new System.Drawing.Point(3, 16);
            this.MapTypeCampaign.Name = "MapTypeCampaign";
            this.MapTypeCampaign.Size = new System.Drawing.Size(72, 17);
            this.MapTypeCampaign.TabIndex = 8;
            this.MapTypeCampaign.Text = "Campaign";
            this.MapTypeCampaign.UseVisualStyleBackColor = true;
            this.MapTypeCampaign.CheckedChanged += new System.EventHandler(this.MapTypeCampaign_CheckedChanged);
            // 
            // MapTypeCoop
            // 
            this.MapTypeCoop.AutoSize = true;
            this.MapTypeCoop.Location = new System.Drawing.Point(3, 39);
            this.MapTypeCoop.Name = "MapTypeCoop";
            this.MapTypeCoop.Size = new System.Drawing.Size(50, 17);
            this.MapTypeCoop.TabIndex = 9;
            this.MapTypeCoop.Text = "Coop";
            this.MapTypeCoop.UseVisualStyleBackColor = true;
            this.MapTypeCoop.CheckedChanged += new System.EventHandler(this.MapTypeCoop_CheckedChanged);
            // 
            // MapTypeMultiplayer
            // 
            this.MapTypeMultiplayer.AutoSize = true;
            this.MapTypeMultiplayer.Location = new System.Drawing.Point(3, 62);
            this.MapTypeMultiplayer.Name = "MapTypeMultiplayer";
            this.MapTypeMultiplayer.Size = new System.Drawing.Size(75, 17);
            this.MapTypeMultiplayer.TabIndex = 10;
            this.MapTypeMultiplayer.Text = "Multiplayer";
            this.MapTypeMultiplayer.UseVisualStyleBackColor = true;
            this.MapTypeMultiplayer.CheckedChanged += new System.EventHandler(this.MapTypeMultiplayer_CheckedChanged);
            // 
            // MapEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 667);
            this.Controls.Add(this.PanelInspector);
            this.Controls.Add(this.TabEditorModes);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1005, 706);
            this.Name = "MapEditorForm";
            this.Text = "Map Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapEditorForm_FormClosing);
            this.Shown += new System.EventHandler(this.MapEditorForm_Load);
            this.Resize += new System.EventHandler(this.MapEditorForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.TabPageDecorations.ResumeLayout(false);
            this.TabPageDecorations.PerformLayout();
            this.TabPageEntities.ResumeLayout(false);
            this.PanelMonumentType.ResumeLayout(false);
            this.PanelMonumentType.PerformLayout();
            this.PanelEntityPlacementSelect.ResumeLayout(false);
            this.PanelEntityPlacementSelect.PerformLayout();
            this.PanelUnitPlacementSelect.ResumeLayout(false);
            this.PanelUnitPlacementSelect.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.TabPageTextures.ResumeLayout(false);
            this.TabPageTextures.PerformLayout();
            this.PanelTileType.ResumeLayout(false);
            this.PanelTileType.PerformLayout();
            this.TabPageTerrain.ResumeLayout(false);
            this.PanelFlags.ResumeLayout(false);
            this.PanelFlags.PerformLayout();
            this.PanelBrushShape.ResumeLayout(false);
            this.PanelBrushShape.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushSizeTrackbar)).EndInit();
            this.PanelTerrainSettings.ResumeLayout(false);
            this.PanelTerrainSettings.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.PanelStrength.ResumeLayout(false);
            this.PanelStrength.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TerrainTrackbar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.TabEditorModes.ResumeLayout(false);
            this.TabPageMetadata.ResumeLayout(false);
            this.PanelCoopParams.ResumeLayout(false);
            this.PanelCoopParams.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenMap;
        private System.Windows.Forms.Timer TimerAnimation;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog DialogSaveMap;
        private System.Windows.Forms.ToolStrip StatusStrip;
        private System.Windows.Forms.ToolStripLabel StatusText;
        private System.Windows.Forms.ToolStripMenuItem closeMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel SpecificText;
        private System.Windows.Forms.ToolStripMenuItem createNewMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slopebasedPaintToolStripMenuItem;
        private System.Windows.Forms.Panel PanelInspector;
        private System.Windows.Forms.TabPage TabPageDecorations;
        private System.Windows.Forms.TabPage TabPageEntities;
        private System.Windows.Forms.Panel PanelEntityPlacementSelect;
        private System.Windows.Forms.Panel PanelUnitPlacementSelect;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox UnitName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox UnitRace;
        private System.Windows.Forms.TextBox EntityID;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton RadioEntityModeUnit;
        private System.Windows.Forms.RadioButton RadioEntityModeBuilding;
        private System.Windows.Forms.RadioButton RadioEntityModeObject;
        private System.Windows.Forms.TabPage TabPageTextures;
        private System.Windows.Forms.Button ButtonSlopePaint;
        private System.Windows.Forms.CheckBox TTexMatchMovementFlags;
        private System.Windows.Forms.Button ButtonModifyTextureSet;
        private System.Windows.Forms.Panel PanelTileType;
        private System.Windows.Forms.RadioButton RadioTileTypeCustom;
        private System.Windows.Forms.RadioButton RadioTileTypeBase;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage TabPageTerrain;
        private System.Windows.Forms.Panel PanelFlags;
        private System.Windows.Forms.CheckBox CheckDisplayFlags;
        private System.Windows.Forms.RadioButton RadioFlagVision;
        private System.Windows.Forms.RadioButton RadioFlagMovement;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel PanelBrushShape;
        private System.Windows.Forms.TextBox BrushSizeVal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton RadioDiamond;
        private System.Windows.Forms.RadioButton RadioCircle;
        private System.Windows.Forms.RadioButton RadioSquare;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar BrushSizeTrackbar;
        private System.Windows.Forms.Panel PanelTerrainSettings;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton RadioIntSinusoidal;
        private System.Windows.Forms.RadioButton RadioIntSquare;
        private System.Windows.Forms.RadioButton RadioIntLinear;
        private System.Windows.Forms.RadioButton RadioIntConstant;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton RadioModeSmooth;
        private System.Windows.Forms.RadioButton RadioModeSet;
        private System.Windows.Forms.RadioButton RadioModeRaise;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel PanelStrength;
        private System.Windows.Forms.Label TerrainValueLabel;
        private System.Windows.Forms.TextBox TerrainValue;
        private System.Windows.Forms.TrackBar TerrainTrackbar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton RadioHMap;
        private System.Windows.Forms.RadioButton RadioFlags;
        private System.Windows.Forms.RadioButton RadioLakes;
        private System.Windows.Forms.TabControl TabEditorModes;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Panel PanelDecalGroups;
        private System.Windows.Forms.TabPage TabPageMetadata;
        private System.Windows.Forms.RadioButton RadioModeMonuments;
        private System.Windows.Forms.RadioButton RadioModeCoopCamps;
        private System.Windows.Forms.RadioButton RadioModeBindstones;
        private System.Windows.Forms.RadioButton RadioModePortals;
        private System.Windows.Forms.Button EditCoopCampTypes;
        private System.Windows.Forms.Panel PanelMonumentType;
        private System.Windows.Forms.RadioButton MonumentHero;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RadioButton MonumentHuman;
        private System.Windows.Forms.RadioButton MonumentElf;
        private System.Windows.Forms.RadioButton MonumentOrc;
        private System.Windows.Forms.RadioButton MonumentTroll;
        private System.Windows.Forms.RadioButton MonumentDwarf;
        private System.Windows.Forms.RadioButton MonumentDarkElf;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton MapTypeCampaign;
        private System.Windows.Forms.RadioButton MapTypeCoop;
        private System.Windows.Forms.RadioButton MapTypeMultiplayer;
        private System.Windows.Forms.Panel PanelCoopParams;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox CoopSpawnParam34;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox CoopSpawnParam33;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox CoopSpawnParam32;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox CoopSpawnParam24;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox CoopSpawnParam23;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox CoopSpawnParam22;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox CoopSpawnParam14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox CoopSpawnParam13;
        private System.Windows.Forms.TextBox CoopSpawnParam12;
        private System.Windows.Forms.TextBox CoopSpawnParam31;
        private System.Windows.Forms.TextBox CoopSpawnParam21;
        private System.Windows.Forms.TextBox CoopSpawnParam11;
        private System.Windows.Forms.Button ButtonTeams;
        private System.Windows.Forms.ToolStripMenuItem visibilitySettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importHeightmapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportHeightmapToolStripMenuItem;
    }
}