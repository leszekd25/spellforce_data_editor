namespace SpellforceDataEditor.special_forms
{
    partial class EffectEditorForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newEffectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asNewEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asSubeffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportEffectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TreeObject = new System.Windows.Forms.TreeView();
            this.ButtonRunEffect = new System.Windows.Forms.Button();
            this.PropertyInspector = new System.Windows.Forms.Panel();
            this.MovieModifiers = new System.Windows.Forms.ListBox();
            this.AvailableEffects = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelType = new System.Windows.Forms.Label();
            this.PanelEffects = new System.Windows.Forms.Panel();
            this.ButtonAddEffect = new System.Windows.Forms.Button();
            this.ButtonRemoveEffect = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.PanelEffects.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newEffectToolStripMenuItem,
            this.movieToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1239, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newEffectToolStripMenuItem
            // 
            this.newEffectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newEffectToolStripMenuItem1,
            this.loadEffectToolStripMenuItem,
            this.saveEffectToolStripMenuItem,
            this.exportEffectToolStripMenuItem});
            this.newEffectToolStripMenuItem.Name = "newEffectToolStripMenuItem";
            this.newEffectToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.newEffectToolStripMenuItem.Text = "Effect";
            // 
            // newEffectToolStripMenuItem1
            // 
            this.newEffectToolStripMenuItem1.Name = "newEffectToolStripMenuItem1";
            this.newEffectToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
            this.newEffectToolStripMenuItem1.Text = "New effect";
            this.newEffectToolStripMenuItem1.Click += new System.EventHandler(this.newEffectToolStripMenuItem1_Click);
            // 
            // loadEffectToolStripMenuItem
            // 
            this.loadEffectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asNewEffectToolStripMenuItem,
            this.asSubeffectToolStripMenuItem});
            this.loadEffectToolStripMenuItem.Name = "loadEffectToolStripMenuItem";
            this.loadEffectToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.loadEffectToolStripMenuItem.Text = "Load effect";
            // 
            // asNewEffectToolStripMenuItem
            // 
            this.asNewEffectToolStripMenuItem.Name = "asNewEffectToolStripMenuItem";
            this.asNewEffectToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.asNewEffectToolStripMenuItem.Text = "As new effect";
            this.asNewEffectToolStripMenuItem.Click += new System.EventHandler(this.asNewEffectToolStripMenuItem_Click);
            // 
            // asSubeffectToolStripMenuItem
            // 
            this.asSubeffectToolStripMenuItem.Name = "asSubeffectToolStripMenuItem";
            this.asSubeffectToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.asSubeffectToolStripMenuItem.Text = "As subeffect";
            this.asSubeffectToolStripMenuItem.Click += new System.EventHandler(this.asSubeffectToolStripMenuItem_Click);
            // 
            // saveEffectToolStripMenuItem
            // 
            this.saveEffectToolStripMenuItem.Name = "saveEffectToolStripMenuItem";
            this.saveEffectToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveEffectToolStripMenuItem.Text = "Save effect";
            this.saveEffectToolStripMenuItem.Click += new System.EventHandler(this.saveEffectToolStripMenuItem_Click);
            // 
            // exportEffectToolStripMenuItem
            // 
            this.exportEffectToolStripMenuItem.Name = "exportEffectToolStripMenuItem";
            this.exportEffectToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.exportEffectToolStripMenuItem.Text = "Export effect";
            this.exportEffectToolStripMenuItem.Click += new System.EventHandler(this.exportEffectToolStripMenuItem_Click);
            // 
            // movieToolStripMenuItem
            // 
            this.movieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMovieToolStripMenuItem,
            this.saveMovieToolStripMenuItem});
            this.movieToolStripMenuItem.Name = "movieToolStripMenuItem";
            this.movieToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.movieToolStripMenuItem.Text = "Movie";
            // 
            // loadMovieToolStripMenuItem
            // 
            this.loadMovieToolStripMenuItem.Name = "loadMovieToolStripMenuItem";
            this.loadMovieToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.loadMovieToolStripMenuItem.Text = "Load movie";
            this.loadMovieToolStripMenuItem.Click += new System.EventHandler(this.loadMovieToolStripMenuItem_Click);
            // 
            // saveMovieToolStripMenuItem
            // 
            this.saveMovieToolStripMenuItem.Name = "saveMovieToolStripMenuItem";
            this.saveMovieToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.saveMovieToolStripMenuItem.Text = "Save movie";
            this.saveMovieToolStripMenuItem.Click += new System.EventHandler(this.saveMovieToolStripMenuItem_Click);
            // 
            // TreeObject
            // 
            this.TreeObject.Location = new System.Drawing.Point(17, 70);
            this.TreeObject.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TreeObject.Name = "TreeObject";
            this.TreeObject.Size = new System.Drawing.Size(352, 427);
            this.TreeObject.TabIndex = 1;
            this.TreeObject.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeObject_NodeMouseClick);
            // 
            // ButtonRunEffect
            // 
            this.ButtonRunEffect.Location = new System.Drawing.Point(948, 503);
            this.ButtonRunEffect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ButtonRunEffect.Name = "ButtonRunEffect";
            this.ButtonRunEffect.Size = new System.Drawing.Size(275, 28);
            this.ButtonRunEffect.TabIndex = 3;
            this.ButtonRunEffect.Text = "Run effect";
            this.ButtonRunEffect.UseVisualStyleBackColor = true;
            this.ButtonRunEffect.Click += new System.EventHandler(this.ButtonRunEffect_Click);
            // 
            // PropertyInspector
            // 
            this.PropertyInspector.AutoScroll = true;
            this.PropertyInspector.Location = new System.Drawing.Point(379, 70);
            this.PropertyInspector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PropertyInspector.Name = "PropertyInspector";
            this.PropertyInspector.Size = new System.Drawing.Size(561, 427);
            this.PropertyInspector.TabIndex = 4;
            // 
            // MovieModifiers
            // 
            this.MovieModifiers.FormattingEnabled = true;
            this.MovieModifiers.ItemHeight = 16;
            this.MovieModifiers.Items.AddRange(new object[] {
            "Animation",
            "Choose",
            "Color",
            "GlobalTrail",
            "Numbers",
            "Rotation",
            "Scale",
            "Sound",
            "TargetColor",
            "Trail",
            "Translation"});
            this.MovieModifiers.Location = new System.Drawing.Point(948, 283);
            this.MovieModifiers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MovieModifiers.Name = "MovieModifiers";
            this.MovieModifiers.Size = new System.Drawing.Size(273, 212);
            this.MovieModifiers.TabIndex = 5;
            this.MovieModifiers.SelectedIndexChanged += new System.EventHandler(this.MovieModifiers_SelectedIndexChanged);
            // 
            // AvailableEffects
            // 
            this.AvailableEffects.FormattingEnabled = true;
            this.AvailableEffects.ItemHeight = 16;
            this.AvailableEffects.Location = new System.Drawing.Point(948, 70);
            this.AvailableEffects.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AvailableEffects.Name = "AvailableEffects";
            this.AvailableEffects.Size = new System.Drawing.Size(273, 180);
            this.AvailableEffects.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(944, 263);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Available movie modifiers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(944, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Available effects";
            // 
            // LabelType
            // 
            this.LabelType.AutoSize = true;
            this.LabelType.Location = new System.Drawing.Point(379, 50);
            this.LabelType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelType.Name = "LabelType";
            this.LabelType.Size = new System.Drawing.Size(0, 16);
            this.LabelType.TabIndex = 9;
            // 
            // PanelEffects
            // 
            this.PanelEffects.Controls.Add(this.ButtonRemoveEffect);
            this.PanelEffects.Controls.Add(this.ButtonAddEffect);
            this.PanelEffects.Location = new System.Drawing.Point(17, 27);
            this.PanelEffects.Name = "PanelEffects";
            this.PanelEffects.Size = new System.Drawing.Size(352, 36);
            this.PanelEffects.TabIndex = 10;
            // 
            // ButtonAddEffect
            // 
            this.ButtonAddEffect.Location = new System.Drawing.Point(3, 3);
            this.ButtonAddEffect.Name = "ButtonAddEffect";
            this.ButtonAddEffect.Size = new System.Drawing.Size(151, 30);
            this.ButtonAddEffect.TabIndex = 11;
            this.ButtonAddEffect.Text = "Add subeffect";
            this.ButtonAddEffect.UseVisualStyleBackColor = true;
            this.ButtonAddEffect.Click += new System.EventHandler(this.ButtonAddEffect_Click);
            // 
            // ButtonRemoveEffect
            // 
            this.ButtonRemoveEffect.Location = new System.Drawing.Point(197, 3);
            this.ButtonRemoveEffect.Name = "ButtonRemoveEffect";
            this.ButtonRemoveEffect.Size = new System.Drawing.Size(152, 30);
            this.ButtonRemoveEffect.TabIndex = 12;
            this.ButtonRemoveEffect.Text = "Remove subeffect";
            this.ButtonRemoveEffect.UseVisualStyleBackColor = true;
            this.ButtonRemoveEffect.Click += new System.EventHandler(this.ButtonRemoveEffect_Click);
            // 
            // EffectEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 544);
            this.Controls.Add(this.PanelEffects);
            this.Controls.Add(this.LabelType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AvailableEffects);
            this.Controls.Add(this.MovieModifiers);
            this.Controls.Add(this.PropertyInspector);
            this.Controls.Add(this.ButtonRunEffect);
            this.Controls.Add(this.TreeObject);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "EffectEditorForm";
            this.Text = "Effect Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PanelEffects.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TreeView TreeObject;
        private System.Windows.Forms.ToolStripMenuItem newEffectToolStripMenuItem;
        private System.Windows.Forms.Button ButtonRunEffect;
        private System.Windows.Forms.ToolStripMenuItem newEffectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem movieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMovieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMovieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asNewEffectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asSubeffectToolStripMenuItem;
        private System.Windows.Forms.Panel PropertyInspector;
        private System.Windows.Forms.ListBox MovieModifiers;
        private System.Windows.Forms.ListBox AvailableEffects;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelType;
        private System.Windows.Forms.Panel PanelEffects;
        private System.Windows.Forms.Button ButtonRemoveEffect;
        private System.Windows.Forms.Button ButtonAddEffect;
    }
}