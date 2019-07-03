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
            this.components = new System.ComponentModel.Container();
            this.glControl1 = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 8, 4));
            this.ListEntries = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.GameDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.ComboBrowseMode = new System.Windows.Forms.ComboBox();
            this.ListAnimations = new System.Windows.Forms.ListBox();
            this.TimerAnimation = new System.Windows.Forms.Timer(this.components);
            this.PanelSound = new System.Windows.Forms.Panel();
            this.labelSoundDuration = new System.Windows.Forms.Label();
            this.trackSoundDuration = new System.Windows.Forms.TrackBar();
            this.buttonSoundStop = new System.Windows.Forms.Button();
            this.buttonSoundPlay = new System.Windows.Forms.Button();
            this.TimerSoundDuration = new System.Windows.Forms.Timer(this.components);
            this.button1Extract = new System.Windows.Forms.Button();
            this.button2Extract = new System.Windows.Forms.Button();
            this.comboMessages = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            this.PanelSound.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackSoundDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(371, 27);
            this.glControl1.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(400, 400);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.glControl1_KeyPress);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            // 
            // ListEntries
            // 
            this.ListEntries.FormattingEnabled = true;
            this.ListEntries.Location = new System.Drawing.Point(12, 54);
            this.ListEntries.Name = "ListEntries";
            this.ListEntries.Size = new System.Drawing.Size(266, 147);
            this.ListEntries.TabIndex = 1;
            this.ListEntries.Visible = false;
            this.ListEntries.SelectedIndexChanged += new System.EventHandler(this.ListEntries_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(771, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(771, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(137, 17);
            this.StatusText.Text = "Specify game directory...";
            // 
            // ComboBrowseMode
            // 
            this.ComboBrowseMode.FormattingEnabled = true;
            this.ComboBrowseMode.Items.AddRange(new object[] {
            "Meshes",
            "Animations",
            "Synchronize with editor",
            "Music",
            "Sounds",
            "Messages"});
            this.ComboBrowseMode.Location = new System.Drawing.Point(12, 27);
            this.ComboBrowseMode.Name = "ComboBrowseMode";
            this.ComboBrowseMode.Size = new System.Drawing.Size(174, 21);
            this.ComboBrowseMode.TabIndex = 4;
            this.ComboBrowseMode.SelectedIndexChanged += new System.EventHandler(this.ComboBrowseMode_SelectedIndexChanged);
            // 
            // ListAnimations
            // 
            this.ListAnimations.FormattingEnabled = true;
            this.ListAnimations.Location = new System.Drawing.Point(12, 271);
            this.ListAnimations.Name = "ListAnimations";
            this.ListAnimations.Size = new System.Drawing.Size(266, 147);
            this.ListAnimations.TabIndex = 5;
            this.ListAnimations.Visible = false;
            this.ListAnimations.SelectedIndexChanged += new System.EventHandler(this.ListAnimations_SelectedIndexChanged);
            // 
            // TimerAnimation
            // 
            this.TimerAnimation.Tick += new System.EventHandler(this.TimerAnimation_Tick);
            // 
            // PanelSound
            // 
            this.PanelSound.Controls.Add(this.labelSoundDuration);
            this.PanelSound.Controls.Add(this.trackSoundDuration);
            this.PanelSound.Controls.Add(this.buttonSoundStop);
            this.PanelSound.Controls.Add(this.buttonSoundPlay);
            this.PanelSound.Location = new System.Drawing.Point(12, 207);
            this.PanelSound.Name = "PanelSound";
            this.PanelSound.Size = new System.Drawing.Size(266, 58);
            this.PanelSound.TabIndex = 6;
            this.PanelSound.Visible = false;
            // 
            // labelSoundDuration
            // 
            this.labelSoundDuration.AutoSize = true;
            this.labelSoundDuration.Location = new System.Drawing.Point(165, 35);
            this.labelSoundDuration.Name = "labelSoundDuration";
            this.labelSoundDuration.Size = new System.Drawing.Size(0, 13);
            this.labelSoundDuration.TabIndex = 3;
            // 
            // trackSoundDuration
            // 
            this.trackSoundDuration.AutoSize = false;
            this.trackSoundDuration.LargeChange = 0;
            this.trackSoundDuration.Location = new System.Drawing.Point(3, 32);
            this.trackSoundDuration.Maximum = 1000000;
            this.trackSoundDuration.Name = "trackSoundDuration";
            this.trackSoundDuration.Size = new System.Drawing.Size(156, 23);
            this.trackSoundDuration.SmallChange = 50000;
            this.trackSoundDuration.TabIndex = 2;
            this.trackSoundDuration.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackSoundDuration.Scroll += new System.EventHandler(this.trackSoundDuration_Scroll);
            // 
            // buttonSoundStop
            // 
            this.buttonSoundStop.Location = new System.Drawing.Point(84, 3);
            this.buttonSoundStop.Name = "buttonSoundStop";
            this.buttonSoundStop.Size = new System.Drawing.Size(75, 23);
            this.buttonSoundStop.TabIndex = 1;
            this.buttonSoundStop.Text = "Stop";
            this.buttonSoundStop.UseVisualStyleBackColor = true;
            this.buttonSoundStop.Click += new System.EventHandler(this.buttonSoundStop_Click);
            // 
            // buttonSoundPlay
            // 
            this.buttonSoundPlay.Location = new System.Drawing.Point(3, 3);
            this.buttonSoundPlay.Name = "buttonSoundPlay";
            this.buttonSoundPlay.Size = new System.Drawing.Size(75, 23);
            this.buttonSoundPlay.TabIndex = 0;
            this.buttonSoundPlay.Text = "Play";
            this.buttonSoundPlay.UseVisualStyleBackColor = true;
            this.buttonSoundPlay.Click += new System.EventHandler(this.buttonSoundPlay_Click);
            // 
            // TimerSoundDuration
            // 
            this.TimerSoundDuration.Tick += new System.EventHandler(this.TimerSoundDuration_Tick);
            // 
            // button1Extract
            // 
            this.button1Extract.Location = new System.Drawing.Point(284, 54);
            this.button1Extract.Name = "button1Extract";
            this.button1Extract.Size = new System.Drawing.Size(81, 23);
            this.button1Extract.TabIndex = 7;
            this.button1Extract.Text = "Extract";
            this.button1Extract.UseVisualStyleBackColor = true;
            this.button1Extract.Visible = false;
            this.button1Extract.Click += new System.EventHandler(this.button1Extract_Click);
            // 
            // button2Extract
            // 
            this.button2Extract.Location = new System.Drawing.Point(284, 271);
            this.button2Extract.Name = "button2Extract";
            this.button2Extract.Size = new System.Drawing.Size(81, 23);
            this.button2Extract.TabIndex = 8;
            this.button2Extract.Text = "Extract";
            this.button2Extract.UseVisualStyleBackColor = true;
            this.button2Extract.Visible = false;
            this.button2Extract.Click += new System.EventHandler(this.button2Extract_Click);
            // 
            // comboMessages
            // 
            this.comboMessages.FormattingEnabled = true;
            this.comboMessages.Items.AddRange(new object[] {
            "Male",
            "Female",
            "RTS Workers",
            "RTS Battle",
            "NPC"});
            this.comboMessages.Location = new System.Drawing.Point(192, 27);
            this.comboMessages.Name = "comboMessages";
            this.comboMessages.Size = new System.Drawing.Size(86, 21);
            this.comboMessages.TabIndex = 9;
            this.comboMessages.Visible = false;
            this.comboMessages.SelectedIndexChanged += new System.EventHandler(this.comboMessages_SelectedIndexChanged);
            // 
            // SFAssetManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 450);
            this.Controls.Add(this.comboMessages);
            this.Controls.Add(this.button2Extract);
            this.Controls.Add(this.button1Extract);
            this.Controls.Add(this.PanelSound);
            this.Controls.Add(this.ListAnimations);
            this.Controls.Add(this.ComboBrowseMode);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ListEntries);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SFAssetManagerForm";
            this.Text = "SF3DManagerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SF3DManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.SF3DManagerForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.PanelSound.ResumeLayout(false);
            this.PanelSound.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackSoundDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
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
    }
}