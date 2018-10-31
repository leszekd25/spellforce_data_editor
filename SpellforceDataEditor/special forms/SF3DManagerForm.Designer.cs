namespace SpellforceDataEditor.special_forms
{
    partial class SF3DManagerForm
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
            this.glControl1 = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 24, 0, 8));
            this.ListEntries = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.GameDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.ComboBrowseMode = new System.Windows.Forms.ComboBox();
            this.ListAnimations = new System.Windows.Forms.ListBox();
            this.TimerAnimation = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.ListEntries.Size = new System.Drawing.Size(353, 199);
            this.ListEntries.TabIndex = 1;
            this.ListEntries.SelectedIndexChanged += new System.EventHandler(this.ListEntries_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(771, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem1});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.testToolStripMenuItem.Text = "Manager";
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
            this.testToolStripMenuItem1.Text = "Specify game directory...";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);
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
            "Synchronize with editor"});
            this.ComboBrowseMode.Location = new System.Drawing.Point(12, 27);
            this.ComboBrowseMode.Name = "ComboBrowseMode";
            this.ComboBrowseMode.Size = new System.Drawing.Size(174, 21);
            this.ComboBrowseMode.TabIndex = 4;
            this.ComboBrowseMode.SelectedIndexChanged += new System.EventHandler(this.ComboBrowseMode_SelectedIndexChanged);
            // 
            // ListAnimations
            // 
            this.ListAnimations.FormattingEnabled = true;
            this.ListAnimations.Location = new System.Drawing.Point(12, 259);
            this.ListAnimations.Name = "ListAnimations";
            this.ListAnimations.Size = new System.Drawing.Size(353, 160);
            this.ListAnimations.TabIndex = 5;
            this.ListAnimations.SelectedIndexChanged += new System.EventHandler(this.ListAnimations_SelectedIndexChanged);
            // 
            // TimerAnimation
            // 
            this.TimerAnimation.Tick += new System.EventHandler(this.TimerAnimation_Tick);
            // 
            // SF3DManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 450);
            this.Controls.Add(this.ListAnimations);
            this.Controls.Add(this.ComboBrowseMode);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ListEntries);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SF3DManagerForm";
            this.Text = "SF3DManagerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SF3DManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.SF3DManagerForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.ListBox ListEntries;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.FolderBrowserDialog GameDirDialog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ComboBox ComboBrowseMode;
        private System.Windows.Forms.ListBox ListAnimations;
        private System.Windows.Forms.Timer TimerAnimation;
    }
}