namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapTerrainTextureInspector
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
            this.LabelTileType = new System.Windows.Forms.Label();
            this.PanelTiles = new System.Windows.Forms.Panel();
            this.PanelTileProperties = new System.Windows.Forms.Panel();
            this.TileBlocksVision = new System.Windows.Forms.CheckBox();
            this.TileBlocksMovement = new System.Windows.Forms.CheckBox();
            this.PanelTileMixer = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TexWeight3 = new System.Windows.Forms.TextBox();
            this.TexWeight2 = new System.Windows.Forms.TextBox();
            this.TexWeight1 = new System.Windows.Forms.TextBox();
            this.SelectedCustomTileMixImage3 = new SpellforceDataEditor.SFMap.map_controls.MapTerrainTextureControl();
            this.SelectedCustomTileMixImage2 = new SpellforceDataEditor.SFMap.map_controls.MapTerrainTextureControl();
            this.SelectedCustomTileMixImage1 = new SpellforceDataEditor.SFMap.map_controls.MapTerrainTextureControl();
            this.SelectedCustomTileTex = new SpellforceDataEditor.SFMap.map_controls.MapTerrainTextureControl();
            this.ButtonAddCustomTile = new System.Windows.Forms.Button();
            this.ButtonRemoveCustomTile = new System.Windows.Forms.Button();
            this.PanelButtons = new System.Windows.Forms.Panel();
            this.PanelTileProperties.SuspendLayout();
            this.PanelTileMixer.SuspendLayout();
            this.PanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelTileType
            // 
            this.LabelTileType.AutoSize = true;
            this.LabelTileType.Location = new System.Drawing.Point(0, 0);
            this.LabelTileType.Name = "LabelTileType";
            this.LabelTileType.Size = new System.Drawing.Size(79, 13);
            this.LabelTileType.TabIndex = 0;
            this.LabelTileType.Text = "Predefined tiles";
            // 
            // PanelTiles
            // 
            this.PanelTiles.AutoScroll = true;
            this.PanelTiles.Location = new System.Drawing.Point(3, 16);
            this.PanelTiles.Name = "PanelTiles";
            this.PanelTiles.Size = new System.Drawing.Size(280, 277);
            this.PanelTiles.TabIndex = 1;
            // 
            // PanelTileProperties
            // 
            this.PanelTileProperties.Controls.Add(this.TileBlocksVision);
            this.PanelTileProperties.Controls.Add(this.TileBlocksMovement);
            this.PanelTileProperties.Enabled = false;
            this.PanelTileProperties.Location = new System.Drawing.Point(3, 434);
            this.PanelTileProperties.Name = "PanelTileProperties";
            this.PanelTileProperties.Size = new System.Drawing.Size(280, 22);
            this.PanelTileProperties.TabIndex = 2;
            // 
            // TileBlocksVision
            // 
            this.TileBlocksVision.AutoCheck = false;
            this.TileBlocksVision.AutoSize = true;
            this.TileBlocksVision.Location = new System.Drawing.Point(127, 3);
            this.TileBlocksVision.Name = "TileBlocksVision";
            this.TileBlocksVision.Size = new System.Drawing.Size(88, 17);
            this.TileBlocksVision.TabIndex = 1;
            this.TileBlocksVision.Text = "Blocks vision";
            this.TileBlocksVision.UseVisualStyleBackColor = true;
            this.TileBlocksVision.Click += new System.EventHandler(this.TileBlocksVision_Click);
            // 
            // TileBlocksMovement
            // 
            this.TileBlocksMovement.AutoCheck = false;
            this.TileBlocksMovement.AutoSize = true;
            this.TileBlocksMovement.Location = new System.Drawing.Point(3, 3);
            this.TileBlocksMovement.Name = "TileBlocksMovement";
            this.TileBlocksMovement.Size = new System.Drawing.Size(110, 17);
            this.TileBlocksMovement.TabIndex = 0;
            this.TileBlocksMovement.Text = "Blocks movement";
            this.TileBlocksMovement.UseVisualStyleBackColor = true;
            this.TileBlocksMovement.Click += new System.EventHandler(this.TileBlocksMovement_Click);
            // 
            // PanelTileMixer
            // 
            this.PanelTileMixer.Controls.Add(this.label3);
            this.PanelTileMixer.Controls.Add(this.label2);
            this.PanelTileMixer.Controls.Add(this.TexWeight3);
            this.PanelTileMixer.Controls.Add(this.TexWeight2);
            this.PanelTileMixer.Controls.Add(this.TexWeight1);
            this.PanelTileMixer.Controls.Add(this.SelectedCustomTileMixImage3);
            this.PanelTileMixer.Controls.Add(this.SelectedCustomTileMixImage2);
            this.PanelTileMixer.Controls.Add(this.SelectedCustomTileMixImage1);
            this.PanelTileMixer.Controls.Add(this.SelectedCustomTileTex);
            this.PanelTileMixer.Location = new System.Drawing.Point(3, 328);
            this.PanelTileMixer.Name = "PanelTileMixer";
            this.PanelTileMixer.Size = new System.Drawing.Size(280, 100);
            this.PanelTileMixer.TabIndex = 3;
            this.PanelTileMixer.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Weights";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Mix with";
            // 
            // TexWeight3
            // 
            this.TexWeight3.Location = new System.Drawing.Point(231, 72);
            this.TexWeight3.Name = "TexWeight3";
            this.TexWeight3.Size = new System.Drawing.Size(46, 20);
            this.TexWeight3.TabIndex = 6;
            this.TexWeight3.Validated += new System.EventHandler(this.TexWeight3_Validated);
            // 
            // TexWeight2
            // 
            this.TexWeight2.Location = new System.Drawing.Point(179, 72);
            this.TexWeight2.Name = "TexWeight2";
            this.TexWeight2.Size = new System.Drawing.Size(46, 20);
            this.TexWeight2.TabIndex = 5;
            this.TexWeight2.Validated += new System.EventHandler(this.TexWeight2_Validated);
            // 
            // TexWeight1
            // 
            this.TexWeight1.Location = new System.Drawing.Point(127, 72);
            this.TexWeight1.Name = "TexWeight1";
            this.TexWeight1.Size = new System.Drawing.Size(46, 20);
            this.TexWeight1.TabIndex = 4;
            this.TexWeight1.Validated += new System.EventHandler(this.TexWeight1_Validated);
            // 
            // SelectedCustomTileMixImage3
            // 
            this.SelectedCustomTileMixImage3.ID = -1;
            this.SelectedCustomTileMixImage3.Location = new System.Drawing.Point(231, 3);
            this.SelectedCustomTileMixImage3.Name = "SelectedCustomTileMixImage3";
            this.SelectedCustomTileMixImage3.Size = new System.Drawing.Size(46, 70);
            this.SelectedCustomTileMixImage3.TabIndex = 3;
            // 
            // SelectedCustomTileMixImage2
            // 
            this.SelectedCustomTileMixImage2.ID = -1;
            this.SelectedCustomTileMixImage2.Location = new System.Drawing.Point(179, 3);
            this.SelectedCustomTileMixImage2.Name = "SelectedCustomTileMixImage2";
            this.SelectedCustomTileMixImage2.Size = new System.Drawing.Size(46, 70);
            this.SelectedCustomTileMixImage2.TabIndex = 2;
            // 
            // SelectedCustomTileMixImage1
            // 
            this.SelectedCustomTileMixImage1.ID = -1;
            this.SelectedCustomTileMixImage1.Location = new System.Drawing.Point(127, 3);
            this.SelectedCustomTileMixImage1.Name = "SelectedCustomTileMixImage1";
            this.SelectedCustomTileMixImage1.Size = new System.Drawing.Size(46, 70);
            this.SelectedCustomTileMixImage1.TabIndex = 1;
            // 
            // SelectedCustomTileTex
            // 
            this.SelectedCustomTileTex.ID = -1;
            this.SelectedCustomTileTex.Location = new System.Drawing.Point(3, 3);
            this.SelectedCustomTileTex.Name = "SelectedCustomTileTex";
            this.SelectedCustomTileTex.Size = new System.Drawing.Size(70, 89);
            this.SelectedCustomTileTex.TabIndex = 0;
            // 
            // ButtonAddCustomTile
            // 
            this.ButtonAddCustomTile.Location = new System.Drawing.Point(3, 3);
            this.ButtonAddCustomTile.Name = "ButtonAddCustomTile";
            this.ButtonAddCustomTile.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddCustomTile.TabIndex = 11;
            this.ButtonAddCustomTile.Text = "Add";
            this.ButtonAddCustomTile.UseVisualStyleBackColor = true;
            this.ButtonAddCustomTile.Click += new System.EventHandler(this.ButtonAddCustomTile_Click);
            // 
            // ButtonRemoveCustomTile
            // 
            this.ButtonRemoveCustomTile.Location = new System.Drawing.Point(202, 3);
            this.ButtonRemoveCustomTile.Name = "ButtonRemoveCustomTile";
            this.ButtonRemoveCustomTile.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemoveCustomTile.TabIndex = 12;
            this.ButtonRemoveCustomTile.Text = "Remove";
            this.ButtonRemoveCustomTile.UseVisualStyleBackColor = true;
            this.ButtonRemoveCustomTile.Click += new System.EventHandler(this.ButtonRemoveCustomTile_Click);
            // 
            // PanelButtons
            // 
            this.PanelButtons.Controls.Add(this.ButtonAddCustomTile);
            this.PanelButtons.Controls.Add(this.ButtonRemoveCustomTile);
            this.PanelButtons.Location = new System.Drawing.Point(3, 296);
            this.PanelButtons.Name = "PanelButtons";
            this.PanelButtons.Size = new System.Drawing.Size(280, 29);
            this.PanelButtons.TabIndex = 10;
            this.PanelButtons.Visible = false;
            // 
            // MapTerrainTextureInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelButtons);
            this.Controls.Add(this.PanelTileMixer);
            this.Controls.Add(this.PanelTileProperties);
            this.Controls.Add(this.PanelTiles);
            this.Controls.Add(this.LabelTileType);
            this.Name = "MapTerrainTextureInspector";
            this.Size = new System.Drawing.Size(286, 461);
            this.PanelTileProperties.ResumeLayout(false);
            this.PanelTileProperties.PerformLayout();
            this.PanelTileMixer.ResumeLayout(false);
            this.PanelTileMixer.PerformLayout();
            this.PanelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelTileType;
        private System.Windows.Forms.Panel PanelTiles;
        private System.Windows.Forms.Panel PanelTileProperties;
        private System.Windows.Forms.Panel PanelTileMixer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TexWeight3;
        private System.Windows.Forms.TextBox TexWeight2;
        private System.Windows.Forms.TextBox TexWeight1;
        private MapTerrainTextureControl SelectedCustomTileMixImage3;
        private MapTerrainTextureControl SelectedCustomTileMixImage2;
        private MapTerrainTextureControl SelectedCustomTileMixImage1;
        private MapTerrainTextureControl SelectedCustomTileTex;
        private System.Windows.Forms.CheckBox TileBlocksVision;
        private System.Windows.Forms.CheckBox TileBlocksMovement;
        private System.Windows.Forms.Button ButtonAddCustomTile;
        private System.Windows.Forms.Button ButtonRemoveCustomTile;
        private System.Windows.Forms.Panel PanelButtons;
    }
}
