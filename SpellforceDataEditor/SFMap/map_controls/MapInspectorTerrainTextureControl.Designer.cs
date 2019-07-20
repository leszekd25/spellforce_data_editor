namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorTerrainTextureControl
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
            this.BrushControl = new SpellforceDataEditor.SFMap.map_controls.MapBrushControl();
            this.PanelTexturePreview = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ListTiles = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MovementCheck = new System.Windows.Forms.CheckBox();
            this.VisionCheck = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TexIDTextBox = new System.Windows.Forms.TextBox();
            this.PanelTex1 = new System.Windows.Forms.Panel();
            this.Tex1Weight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Tex1Button = new System.Windows.Forms.Button();
            this.PanelTex2 = new System.Windows.Forms.Panel();
            this.Tex2Weight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Tex2Button = new System.Windows.Forms.Button();
            this.PanelTex3 = new System.Windows.Forms.Panel();
            this.Tex3Weight = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Tex3Button = new System.Windows.Forms.Button();
            this.TexPreview = new System.Windows.Forms.Button();
            this.TimerControl = new System.Windows.Forms.Timer(this.components);
            this.CheckEditSimilar = new System.Windows.Forms.CheckBox();
            this.PanelTex1.SuspendLayout();
            this.PanelTex2.SuspendLayout();
            this.PanelTex3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BrushControl
            // 
            this.BrushControl.Location = new System.Drawing.Point(0, 434);
            this.BrushControl.Name = "BrushControl";
            this.BrushControl.Size = new System.Drawing.Size(310, 124);
            this.BrushControl.TabIndex = 0;
            // 
            // PanelTexturePreview
            // 
            this.PanelTexturePreview.AutoScroll = true;
            this.PanelTexturePreview.Location = new System.Drawing.Point(3, 28);
            this.PanelTexturePreview.Name = "PanelTexturePreview";
            this.PanelTexturePreview.Size = new System.Drawing.Size(323, 161);
            this.PanelTexturePreview.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Base textures";
            // 
            // ListTiles
            // 
            this.ListTiles.FormattingEnabled = true;
            this.ListTiles.Location = new System.Drawing.Point(3, 219);
            this.ListTiles.Name = "ListTiles";
            this.ListTiles.Size = new System.Drawing.Size(120, 212);
            this.ListTiles.TabIndex = 4;
            this.ListTiles.SelectedIndexChanged += new System.EventHandler(this.ListTiles_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 203);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tile types";
            // 
            // MovementCheck
            // 
            this.MovementCheck.AutoSize = true;
            this.MovementCheck.Location = new System.Drawing.Point(274, 219);
            this.MovementCheck.Name = "MovementCheck";
            this.MovementCheck.Size = new System.Drawing.Size(110, 17);
            this.MovementCheck.TabIndex = 8;
            this.MovementCheck.Text = "Blocks movement";
            this.MovementCheck.UseVisualStyleBackColor = true;
            this.MovementCheck.CheckedChanged += new System.EventHandler(this.MovementCheck_CheckedChanged);
            // 
            // VisionCheck
            // 
            this.VisionCheck.AutoSize = true;
            this.VisionCheck.Location = new System.Drawing.Point(274, 240);
            this.VisionCheck.Name = "VisionCheck";
            this.VisionCheck.Size = new System.Drawing.Size(103, 17);
            this.VisionCheck.TabIndex = 9;
            this.VisionCheck.Text = "Blocks vision (?)";
            this.VisionCheck.UseVisualStyleBackColor = true;
            this.VisionCheck.CheckedChanged += new System.EventHandler(this.VisionCheck_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(327, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Texture ID";
            // 
            // TexIDTextBox
            // 
            this.TexIDTextBox.Location = new System.Drawing.Point(332, 44);
            this.TexIDTextBox.Name = "TexIDTextBox";
            this.TexIDTextBox.Size = new System.Drawing.Size(90, 20);
            this.TexIDTextBox.TabIndex = 11;
            this.TexIDTextBox.Validated += new System.EventHandler(this.TexIDTextBox_Validated);
            // 
            // PanelTex1
            // 
            this.PanelTex1.Controls.Add(this.Tex1Weight);
            this.PanelTex1.Controls.Add(this.label4);
            this.PanelTex1.Controls.Add(this.Tex1Button);
            this.PanelTex1.Location = new System.Drawing.Point(129, 219);
            this.PanelTex1.Name = "PanelTex1";
            this.PanelTex1.Size = new System.Drawing.Size(139, 67);
            this.PanelTex1.TabIndex = 6;
            // 
            // Tex1Weight
            // 
            this.Tex1Weight.Location = new System.Drawing.Point(70, 24);
            this.Tex1Weight.Name = "Tex1Weight";
            this.Tex1Weight.Size = new System.Drawing.Size(66, 20);
            this.Tex1Weight.TabIndex = 14;
            this.Tex1Weight.Validated += new System.EventHandler(this.Tex1Weight_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(80, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Weight";
            // 
            // Tex1Button
            // 
            this.Tex1Button.Location = new System.Drawing.Point(3, 3);
            this.Tex1Button.Name = "Tex1Button";
            this.Tex1Button.Size = new System.Drawing.Size(61, 61);
            this.Tex1Button.TabIndex = 12;
            this.Tex1Button.UseVisualStyleBackColor = true;
            // 
            // PanelTex2
            // 
            this.PanelTex2.Controls.Add(this.Tex2Weight);
            this.PanelTex2.Controls.Add(this.label5);
            this.PanelTex2.Controls.Add(this.Tex2Button);
            this.PanelTex2.Location = new System.Drawing.Point(129, 289);
            this.PanelTex2.Name = "PanelTex2";
            this.PanelTex2.Size = new System.Drawing.Size(139, 67);
            this.PanelTex2.TabIndex = 15;
            // 
            // Tex2Weight
            // 
            this.Tex2Weight.Location = new System.Drawing.Point(70, 24);
            this.Tex2Weight.Name = "Tex2Weight";
            this.Tex2Weight.Size = new System.Drawing.Size(66, 20);
            this.Tex2Weight.TabIndex = 14;
            this.Tex2Weight.Validated += new System.EventHandler(this.Tex2Weight_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(80, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Weight";
            // 
            // Tex2Button
            // 
            this.Tex2Button.Location = new System.Drawing.Point(3, 3);
            this.Tex2Button.Name = "Tex2Button";
            this.Tex2Button.Size = new System.Drawing.Size(61, 61);
            this.Tex2Button.TabIndex = 12;
            this.Tex2Button.UseVisualStyleBackColor = true;
            // 
            // PanelTex3
            // 
            this.PanelTex3.Controls.Add(this.Tex3Weight);
            this.PanelTex3.Controls.Add(this.label6);
            this.PanelTex3.Controls.Add(this.Tex3Button);
            this.PanelTex3.Location = new System.Drawing.Point(129, 359);
            this.PanelTex3.Name = "PanelTex3";
            this.PanelTex3.Size = new System.Drawing.Size(139, 67);
            this.PanelTex3.TabIndex = 15;
            // 
            // Tex3Weight
            // 
            this.Tex3Weight.Location = new System.Drawing.Point(70, 24);
            this.Tex3Weight.Name = "Tex3Weight";
            this.Tex3Weight.Size = new System.Drawing.Size(66, 20);
            this.Tex3Weight.TabIndex = 14;
            this.Tex3Weight.Validated += new System.EventHandler(this.Tex3Weight_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(80, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Weight";
            // 
            // Tex3Button
            // 
            this.Tex3Button.Location = new System.Drawing.Point(3, 3);
            this.Tex3Button.Name = "Tex3Button";
            this.Tex3Button.Size = new System.Drawing.Size(61, 61);
            this.Tex3Button.TabIndex = 12;
            this.Tex3Button.UseVisualStyleBackColor = true;
            // 
            // TexPreview
            // 
            this.TexPreview.Location = new System.Drawing.Point(274, 260);
            this.TexPreview.Name = "TexPreview";
            this.TexPreview.Size = new System.Drawing.Size(110, 110);
            this.TexPreview.TabIndex = 16;
            this.TexPreview.UseVisualStyleBackColor = true;
            // 
            // TimerControl
            // 
            this.TimerControl.Tick += new System.EventHandler(this.TimerControl_Tick);
            // 
            // CheckEditSimilar
            // 
            this.CheckEditSimilar.AutoSize = true;
            this.CheckEditSimilar.Location = new System.Drawing.Point(199, 532);
            this.CheckEditSimilar.Name = "CheckEditSimilar";
            this.CheckEditSimilar.Size = new System.Drawing.Size(133, 17);
            this.CheckEditSimilar.TabIndex = 17;
            this.CheckEditSimilar.Text = "Match movement flags";
            this.CheckEditSimilar.UseVisualStyleBackColor = true;
            // 
            // MapInspectorTerrainTextureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.CheckEditSimilar);
            this.Controls.Add(this.TexPreview);
            this.Controls.Add(this.PanelTex3);
            this.Controls.Add(this.PanelTex2);
            this.Controls.Add(this.TexIDTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.VisionCheck);
            this.Controls.Add(this.MovementCheck);
            this.Controls.Add(this.PanelTex1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ListTiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PanelTexturePreview);
            this.Controls.Add(this.BrushControl);
            this.Name = "MapInspectorTerrainTextureControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorTerrainTextureControl_VisibleChanged);
            this.PanelTex1.ResumeLayout(false);
            this.PanelTex1.PerformLayout();
            this.PanelTex2.ResumeLayout(false);
            this.PanelTex2.PerformLayout();
            this.PanelTex3.ResumeLayout(false);
            this.PanelTex3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapBrushControl BrushControl;
        private System.Windows.Forms.Panel PanelTexturePreview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox ListTiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox MovementCheck;
        private System.Windows.Forms.CheckBox VisionCheck;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TexIDTextBox;
        private System.Windows.Forms.Panel PanelTex1;
        private System.Windows.Forms.TextBox Tex1Weight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Tex1Button;
        private System.Windows.Forms.Panel PanelTex2;
        private System.Windows.Forms.TextBox Tex2Weight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Tex2Button;
        private System.Windows.Forms.Panel PanelTex3;
        private System.Windows.Forms.TextBox Tex3Weight;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Tex3Button;
        private System.Windows.Forms.Button TexPreview;
        private System.Windows.Forms.Timer TimerControl;
        private System.Windows.Forms.CheckBox CheckEditSimilar;
    }
}
