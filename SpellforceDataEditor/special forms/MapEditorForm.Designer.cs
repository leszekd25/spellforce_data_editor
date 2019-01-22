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
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.RenderWindow = new OpenTK.GLControl();
            this.OpenMap = new System.Windows.Forms.OpenFileDialog();
            this.TimerAnimation = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.loadToolStripMenuItem.Text = "Load map";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // RenderWindow
            // 
            this.RenderWindow.BackColor = System.Drawing.Color.Black;
            this.RenderWindow.Location = new System.Drawing.Point(262, 27);
            this.RenderWindow.Name = "RenderWindow";
            this.RenderWindow.Size = new System.Drawing.Size(510, 510);
            this.RenderWindow.TabIndex = 2;
            this.RenderWindow.VSync = true;
            this.RenderWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderWindow_Paint);
            this.RenderWindow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RenderWindow_KeyPress);
            this.RenderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.RenderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseMove);
            this.RenderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
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
            // MapEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.RenderWindow);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MapEditorForm";
            this.Text = "MapEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapEditorForm_FormClosing);
            this.Shown += new System.EventHandler(this.MapEditorForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private OpenTK.GLControl RenderWindow;
        private System.Windows.Forms.OpenFileDialog OpenMap;
        private System.Windows.Forms.Timer TimerAnimation;
    }
}