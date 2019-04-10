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
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderWindow = new OpenTK.GLControl();
            this.OpenMap = new System.Windows.Forms.OpenFileDialog();
            this.TimerAnimation = new System.Windows.Forms.Timer(this.components);
            this.SaveMap = new System.Windows.Forms.SaveFileDialog();
            this.PanelModes = new System.Windows.Forms.Panel();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.ButtonFlag = new System.Windows.Forms.Button();
            this.ButtonTerrainTexture = new System.Windows.Forms.Button();
            this.ButtonHeightmap = new System.Windows.Forms.Button();
            this.ToolbarStrip = new System.Windows.Forms.ToolStrip();
            this.StatusStrip = new System.Windows.Forms.ToolStrip();
            this.StatusText = new System.Windows.Forms.ToolStripLabel();
            this.InspectorPanel = new System.Windows.Forms.Panel();
            this.LabelMode = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.PanelModes.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1100, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveMapToolStripMenuItem});
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
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.saveMapToolStripMenuItem.Text = "Save map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // RenderWindow
            // 
            this.RenderWindow.BackColor = System.Drawing.Color.Black;
            this.RenderWindow.Location = new System.Drawing.Point(511, 52);
            this.RenderWindow.Name = "RenderWindow";
            this.RenderWindow.Size = new System.Drawing.Size(589, 589);
            this.RenderWindow.TabIndex = 2;
            this.RenderWindow.VSync = true;
            this.RenderWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderWindow_Paint);
            this.RenderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.RenderWindow.MouseEnter += new System.EventHandler(this.RenderWindow_MouseEnter);
            this.RenderWindow.MouseLeave += new System.EventHandler(this.RenderWindow_MouseLeave);
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
            // SaveMap
            // 
            this.SaveMap.DefaultExt = "map";
            this.SaveMap.Filter = "Map file | *.map";
            // 
            // PanelModes
            // 
            this.PanelModes.Controls.Add(this.button11);
            this.PanelModes.Controls.Add(this.button10);
            this.PanelModes.Controls.Add(this.button9);
            this.PanelModes.Controls.Add(this.button8);
            this.PanelModes.Controls.Add(this.button7);
            this.PanelModes.Controls.Add(this.button6);
            this.PanelModes.Controls.Add(this.button5);
            this.PanelModes.Controls.Add(this.button4);
            this.PanelModes.Controls.Add(this.ButtonFlag);
            this.PanelModes.Controls.Add(this.ButtonTerrainTexture);
            this.PanelModes.Controls.Add(this.ButtonHeightmap);
            this.PanelModes.Location = new System.Drawing.Point(0, 52);
            this.PanelModes.Name = "PanelModes";
            this.PanelModes.Size = new System.Drawing.Size(54, 589);
            this.PanelModes.TabIndex = 4;
            // 
            // button11
            // 
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button11.Image = global::SpellforceDataEditor.Properties.Resources.metadata_icon;
            this.button11.Location = new System.Drawing.Point(3, 541);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(48, 48);
            this.button11.TabIndex = 17;
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button10.Image = global::SpellforceDataEditor.Properties.Resources.enemycamp_icon;
            this.button10.Location = new System.Drawing.Point(3, 487);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(48, 48);
            this.button10.TabIndex = 16;
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button9.Image = global::SpellforceDataEditor.Properties.Resources.npc_icon;
            this.button9.Location = new System.Drawing.Point(3, 433);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(48, 48);
            this.button9.TabIndex = 15;
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button8.Image = global::SpellforceDataEditor.Properties.Resources.decoration_icon;
            this.button8.Location = new System.Drawing.Point(3, 381);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(48, 48);
            this.button8.TabIndex = 14;
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button7.Image = global::SpellforceDataEditor.Properties.Resources.object_icon;
            this.button7.Location = new System.Drawing.Point(3, 327);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(48, 48);
            this.button7.TabIndex = 13;
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Image = global::SpellforceDataEditor.Properties.Resources.building_icon;
            this.button6.Location = new System.Drawing.Point(3, 273);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 48);
            this.button6.TabIndex = 12;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button5.Image = global::SpellforceDataEditor.Properties.Resources.unit_icon;
            this.button5.Location = new System.Drawing.Point(3, 219);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(48, 48);
            this.button5.TabIndex = 11;
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Image = global::SpellforceDataEditor.Properties.Resources.lake_icon;
            this.button4.Location = new System.Drawing.Point(3, 165);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(48, 48);
            this.button4.TabIndex = 10;
            this.button4.UseVisualStyleBackColor = true;
            // 
            // ButtonFlag
            // 
            this.ButtonFlag.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonFlag.Image = global::SpellforceDataEditor.Properties.Resources.flag_icon;
            this.ButtonFlag.Location = new System.Drawing.Point(3, 111);
            this.ButtonFlag.Name = "ButtonFlag";
            this.ButtonFlag.Size = new System.Drawing.Size(48, 48);
            this.ButtonFlag.TabIndex = 9;
            this.ButtonFlag.UseVisualStyleBackColor = true;
            this.ButtonFlag.Click += new System.EventHandler(this.ButtonFlag_Click);
            // 
            // ButtonTerrainTexture
            // 
            this.ButtonTerrainTexture.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonTerrainTexture.Image = global::SpellforceDataEditor.Properties.Resources.texture_icon;
            this.ButtonTerrainTexture.Location = new System.Drawing.Point(3, 57);
            this.ButtonTerrainTexture.Name = "ButtonTerrainTexture";
            this.ButtonTerrainTexture.Size = new System.Drawing.Size(48, 48);
            this.ButtonTerrainTexture.TabIndex = 8;
            this.ButtonTerrainTexture.UseVisualStyleBackColor = true;
            this.ButtonTerrainTexture.Click += new System.EventHandler(this.ButtonTerrainTexture_Click);
            // 
            // ButtonHeightmap
            // 
            this.ButtonHeightmap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ButtonHeightmap.Image = global::SpellforceDataEditor.Properties.Resources.hmap_icon;
            this.ButtonHeightmap.Location = new System.Drawing.Point(3, 3);
            this.ButtonHeightmap.Name = "ButtonHeightmap";
            this.ButtonHeightmap.Size = new System.Drawing.Size(48, 48);
            this.ButtonHeightmap.TabIndex = 7;
            this.ButtonHeightmap.UseVisualStyleBackColor = true;
            this.ButtonHeightmap.Click += new System.EventHandler(this.ButtonHeightmap_Click);
            // 
            // ToolbarStrip
            // 
            this.ToolbarStrip.Location = new System.Drawing.Point(0, 24);
            this.ToolbarStrip.Name = "ToolbarStrip";
            this.ToolbarStrip.Size = new System.Drawing.Size(1100, 25);
            this.ToolbarStrip.TabIndex = 5;
            this.ToolbarStrip.Text = "toolStrip1";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
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
            // InspectorPanel
            // 
            this.InspectorPanel.Location = new System.Drawing.Point(60, 80);
            this.InspectorPanel.Name = "InspectorPanel";
            this.InspectorPanel.Size = new System.Drawing.Size(443, 561);
            this.InspectorPanel.TabIndex = 7;
            // 
            // LabelMode
            // 
            this.LabelMode.AutoSize = true;
            this.LabelMode.Location = new System.Drawing.Point(60, 55);
            this.LabelMode.Name = "LabelMode";
            this.LabelMode.Size = new System.Drawing.Size(0, 13);
            this.LabelMode.TabIndex = 0;
            // 
            // MapEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 667);
            this.Controls.Add(this.LabelMode);
            this.Controls.Add(this.InspectorPanel);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ToolbarStrip);
            this.Controls.Add(this.PanelModes);
            this.Controls.Add(this.RenderWindow);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1005, 706);
            this.Name = "MapEditorForm";
            this.Text = "MapEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapEditorForm_FormClosing);
            this.Shown += new System.EventHandler(this.MapEditorForm_Load);
            this.Resize += new System.EventHandler(this.MapEditorForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PanelModes.ResumeLayout(false);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private OpenTK.GLControl RenderWindow;
        private System.Windows.Forms.OpenFileDialog OpenMap;
        private System.Windows.Forms.Timer TimerAnimation;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog SaveMap;
        private System.Windows.Forms.Panel PanelModes;
        private System.Windows.Forms.ToolStrip ToolbarStrip;
        private System.Windows.Forms.ToolStrip StatusStrip;
        private System.Windows.Forms.ToolStripLabel StatusText;
        private System.Windows.Forms.Button ButtonHeightmap;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button ButtonFlag;
        private System.Windows.Forms.Button ButtonTerrainTexture;
        private System.Windows.Forms.Panel InspectorPanel;
        private System.Windows.Forms.Label LabelMode;
    }
}