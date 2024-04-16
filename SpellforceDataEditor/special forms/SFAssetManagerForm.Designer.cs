namespace SpellforceDataEditor.special_forms
{
    partial class SFAssetManagerForm
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
            components = new System.ComponentModel.Container();
            glControl1 = new OpenTK.WinForms.GLControl();
            ListEntries = new System.Windows.Forms.ListBox();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            resetCameraPosiitonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            reloadCurrentSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exportSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            textureRepairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            GameDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            ComboBrowseMode = new System.Windows.Forms.ComboBox();
            ListAnimations = new System.Windows.Forms.ListBox();
            TimerAnimation = new System.Windows.Forms.Timer(components);
            PanelSound = new System.Windows.Forms.Panel();
            labelSoundDuration = new System.Windows.Forms.Label();
            trackSoundDuration = new System.Windows.Forms.TrackBar();
            buttonSoundStop = new System.Windows.Forms.Button();
            buttonSoundPlay = new System.Windows.Forms.Button();
            TimerSoundDuration = new System.Windows.Forms.Timer(components);
            button1Extract = new System.Windows.Forms.Button();
            button2Extract = new System.Windows.Forms.Button();
            comboMessages = new System.Windows.Forms.ComboBox();
            ButtonToggleFloor = new System.Windows.Forms.Button();
            ButtonToggleBoneDisplay = new System.Windows.Forms.Button();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            PanelSound.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackSoundDuration).BeginInit();
            SuspendLayout();
            // 
            // glControl1
            // 
            glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl1.APIVersion = new System.Version(3, 3, 0, 0);
            glControl1.BackColor = System.Drawing.Color.Black;
            glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Location = new System.Drawing.Point(433, 31);
            glControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            glControl1.MaximumSize = new System.Drawing.Size(1195, 1182);
            glControl1.Name = "glControl1";
            glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl1.Size = new System.Drawing.Size(467, 462);
            glControl1.TabIndex = 0;
            glControl1.Paint += glControl1_Paint;
            glControl1.MouseDown += glControl1_MouseDown;
            glControl1.MouseMove += glControl1_MouseMove;
            glControl1.MouseUp += glControl1_MouseUp;
            // 
            // ListEntries
            // 
            ListEntries.FormattingEnabled = true;
            ListEntries.ItemHeight = 15;
            ListEntries.Location = new System.Drawing.Point(14, 62);
            ListEntries.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListEntries.Name = "ListEntries";
            ListEntries.Size = new System.Drawing.Size(310, 169);
            ListEntries.TabIndex = 1;
            ListEntries.Visible = false;
            ListEntries.SelectedIndexChanged += ListEntries_SelectedIndexChanged;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { optionsToolStripMenuItem, toolsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(899, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { resetCameraPosiitonToolStripMenuItem, reloadCurrentSceneToolStripMenuItem, exportSettingsToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // resetCameraPosiitonToolStripMenuItem
            // 
            resetCameraPosiitonToolStripMenuItem.Name = "resetCameraPosiitonToolStripMenuItem";
            resetCameraPosiitonToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            resetCameraPosiitonToolStripMenuItem.Text = "Reset camera posiiton (Space)";
            resetCameraPosiitonToolStripMenuItem.Click += resetCameraPosiitonToolStripMenuItem_Click;
            // 
            // reloadCurrentSceneToolStripMenuItem
            // 
            reloadCurrentSceneToolStripMenuItem.Name = "reloadCurrentSceneToolStripMenuItem";
            reloadCurrentSceneToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            reloadCurrentSceneToolStripMenuItem.Text = "Reload current scene (Ctrl+R)";
            reloadCurrentSceneToolStripMenuItem.Click += reloadCurrentSceneToolStripMenuItem_Click;
            // 
            // exportSettingsToolStripMenuItem
            // 
            exportSettingsToolStripMenuItem.Name = "exportSettingsToolStripMenuItem";
            exportSettingsToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            exportSettingsToolStripMenuItem.Text = "Extraction settings...";
            exportSettingsToolStripMenuItem.Click += exportSettingsToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { textureRepairToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // textureRepairToolStripMenuItem
            // 
            textureRepairToolStripMenuItem.Name = "textureRepairToolStripMenuItem";
            textureRepairToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            textureRepairToolStripMenuItem.Text = "Texture repair...";
            textureRepairToolStripMenuItem.Click += textureRepairToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { StatusText });
            statusStrip1.Location = new System.Drawing.Point(0, 503);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(899, 25);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // StatusText
            // 
            StatusText.Name = "StatusText";
            StatusText.Size = new System.Drawing.Size(0, 20);
            // 
            // ComboBrowseMode
            // 
            ComboBrowseMode.FormattingEnabled = true;
            ComboBrowseMode.Items.AddRange(new object[] { "Meshes", "Animations", "Synchronize with GameData Editor", "Music", "Sounds", "Messages" });
            ComboBrowseMode.Location = new System.Drawing.Point(14, 31);
            ComboBrowseMode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ComboBrowseMode.Name = "ComboBrowseMode";
            ComboBrowseMode.Size = new System.Drawing.Size(202, 23);
            ComboBrowseMode.TabIndex = 4;
            ComboBrowseMode.SelectedIndexChanged += ComboBrowseMode_SelectedIndexChanged;
            // 
            // ListAnimations
            // 
            ListAnimations.FormattingEnabled = true;
            ListAnimations.ItemHeight = 15;
            ListAnimations.Location = new System.Drawing.Point(14, 313);
            ListAnimations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListAnimations.Name = "ListAnimations";
            ListAnimations.Size = new System.Drawing.Size(310, 169);
            ListAnimations.TabIndex = 5;
            ListAnimations.Visible = false;
            ListAnimations.SelectedIndexChanged += ListAnimations_SelectedIndexChanged;
            // 
            // TimerAnimation
            // 
            TimerAnimation.Tick += TimerAnimation_Tick;
            // 
            // PanelSound
            // 
            PanelSound.Controls.Add(labelSoundDuration);
            PanelSound.Controls.Add(trackSoundDuration);
            PanelSound.Controls.Add(buttonSoundStop);
            PanelSound.Controls.Add(buttonSoundPlay);
            PanelSound.Location = new System.Drawing.Point(14, 239);
            PanelSound.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelSound.Name = "PanelSound";
            PanelSound.Size = new System.Drawing.Size(310, 67);
            PanelSound.TabIndex = 6;
            PanelSound.Visible = false;
            // 
            // labelSoundDuration
            // 
            labelSoundDuration.AutoSize = true;
            labelSoundDuration.Location = new System.Drawing.Point(192, 40);
            labelSoundDuration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelSoundDuration.Name = "labelSoundDuration";
            labelSoundDuration.Size = new System.Drawing.Size(0, 15);
            labelSoundDuration.TabIndex = 3;
            // 
            // trackSoundDuration
            // 
            trackSoundDuration.AutoSize = false;
            trackSoundDuration.LargeChange = 0;
            trackSoundDuration.Location = new System.Drawing.Point(4, 37);
            trackSoundDuration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackSoundDuration.Maximum = 1000000;
            trackSoundDuration.Name = "trackSoundDuration";
            trackSoundDuration.Size = new System.Drawing.Size(182, 27);
            trackSoundDuration.SmallChange = 50000;
            trackSoundDuration.TabIndex = 2;
            trackSoundDuration.TickStyle = System.Windows.Forms.TickStyle.None;
            trackSoundDuration.Scroll += trackSoundDuration_Scroll;
            // 
            // buttonSoundStop
            // 
            buttonSoundStop.Location = new System.Drawing.Point(98, 3);
            buttonSoundStop.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSoundStop.Name = "buttonSoundStop";
            buttonSoundStop.Size = new System.Drawing.Size(88, 27);
            buttonSoundStop.TabIndex = 1;
            buttonSoundStop.Text = "Stop";
            buttonSoundStop.UseVisualStyleBackColor = true;
            buttonSoundStop.Click += buttonSoundStop_Click;
            // 
            // buttonSoundPlay
            // 
            buttonSoundPlay.Location = new System.Drawing.Point(4, 3);
            buttonSoundPlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonSoundPlay.Name = "buttonSoundPlay";
            buttonSoundPlay.Size = new System.Drawing.Size(88, 27);
            buttonSoundPlay.TabIndex = 0;
            buttonSoundPlay.Text = "Play";
            buttonSoundPlay.UseVisualStyleBackColor = true;
            buttonSoundPlay.Click += buttonSoundPlay_Click;
            // 
            // TimerSoundDuration
            // 
            TimerSoundDuration.Tick += TimerSoundDuration_Tick;
            // 
            // button1Extract
            // 
            button1Extract.Location = new System.Drawing.Point(331, 62);
            button1Extract.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1Extract.Name = "button1Extract";
            button1Extract.Size = new System.Drawing.Size(94, 27);
            button1Extract.TabIndex = 7;
            button1Extract.Text = "Extract";
            button1Extract.UseVisualStyleBackColor = true;
            button1Extract.Visible = false;
            button1Extract.Click += button1Extract_Click;
            // 
            // button2Extract
            // 
            button2Extract.Location = new System.Drawing.Point(331, 313);
            button2Extract.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2Extract.Name = "button2Extract";
            button2Extract.Size = new System.Drawing.Size(94, 27);
            button2Extract.TabIndex = 8;
            button2Extract.Text = "Extract";
            button2Extract.UseVisualStyleBackColor = true;
            button2Extract.Visible = false;
            button2Extract.Click += button2Extract_Click;
            // 
            // comboMessages
            // 
            comboMessages.FormattingEnabled = true;
            comboMessages.Items.AddRange(new object[] { "Male", "Female", "RTS Workers", "RTS Battle", "NPC" });
            comboMessages.Location = new System.Drawing.Point(224, 31);
            comboMessages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboMessages.Name = "comboMessages";
            comboMessages.Size = new System.Drawing.Size(100, 23);
            comboMessages.TabIndex = 9;
            comboMessages.Visible = false;
            comboMessages.SelectedIndexChanged += comboMessages_SelectedIndexChanged;
            // 
            // ButtonToggleFloor
            // 
            ButtonToggleFloor.Location = new System.Drawing.Point(433, 499);
            ButtonToggleFloor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonToggleFloor.Name = "ButtonToggleFloor";
            ButtonToggleFloor.Size = new System.Drawing.Size(100, 27);
            ButtonToggleFloor.TabIndex = 10;
            ButtonToggleFloor.Text = "Toggle floor";
            ButtonToggleFloor.UseVisualStyleBackColor = true;
            ButtonToggleFloor.Click += ButtonToggleFloor_Click;
            // 
            // ButtonToggleBoneDisplay
            // 
            ButtonToggleBoneDisplay.Location = new System.Drawing.Point(540, 493);
            ButtonToggleBoneDisplay.Name = "ButtonToggleBoneDisplay";
            ButtonToggleBoneDisplay.Size = new System.Drawing.Size(91, 23);
            ButtonToggleBoneDisplay.TabIndex = 11;
            ButtonToggleBoneDisplay.Text = "Toggle bones";
            ButtonToggleBoneDisplay.UseVisualStyleBackColor = true;
            ButtonToggleBoneDisplay.Visible = false;
            ButtonToggleBoneDisplay.Click += ButtonToggleBoneDisplay_Click;
            // 
            // SFAssetManagerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(899, 528);
            Controls.Add(ButtonToggleBoneDisplay);
            Controls.Add(ButtonToggleFloor);
            Controls.Add(comboMessages);
            Controls.Add(button2Extract);
            Controls.Add(button1Extract);
            Controls.Add(PanelSound);
            Controls.Add(ListAnimations);
            Controls.Add(ComboBrowseMode);
            Controls.Add(statusStrip1);
            Controls.Add(ListEntries);
            Controls.Add(glControl1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(915, 558);
            Name = "SFAssetManagerForm";
            Text = "Asset Viewer";
            FormClosing += SF3DManagerForm_FormClosing;
            Load += SF3DManagerForm_Load;
            Resize += SFAssetManagerForm_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            PanelSound.ResumeLayout(false);
            PanelSound.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackSoundDuration).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenTK.WinForms.GLControl glControl1;
        private System.Windows.Forms.ListBox ListEntries;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.FolderBrowserDialog GameDirDialog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ComboBox ComboBrowseMode;
        private System.Windows.Forms.ListBox ListAnimations;
        private System.Windows.Forms.Timer TimerAnimation;
        private System.Windows.Forms.Panel PanelSound;
        private System.Windows.Forms.Label labelSoundDuration;
        private System.Windows.Forms.TrackBar trackSoundDuration;
        private System.Windows.Forms.Button buttonSoundStop;
        private System.Windows.Forms.Button buttonSoundPlay;
        private System.Windows.Forms.Timer TimerSoundDuration;
        private System.Windows.Forms.Button button1Extract;
        private System.Windows.Forms.Button button2Extract;
        private System.Windows.Forms.ComboBox comboMessages;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textureRepairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCameraPosiitonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadCurrentSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSettingsToolStripMenuItem;
        private System.Windows.Forms.Button ButtonToggleFloor;
        private System.Windows.Forms.Button ButtonToggleBoneDisplay;
    }
}