namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapSelectTile
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
            this.PanelTiles = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // PanelTiles
            // 
            this.PanelTiles.AutoScroll = true;
            this.PanelTiles.Location = new System.Drawing.Point(12, 12);
            this.PanelTiles.Name = "PanelTiles";
            this.PanelTiles.Size = new System.Drawing.Size(700, 344);
            this.PanelTiles.TabIndex = 2;
            // 
            // MapSelectTile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 367);
            this.Controls.Add(this.PanelTiles);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapSelectTile";
            this.Text = "Select tile";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelTiles;
    }
}