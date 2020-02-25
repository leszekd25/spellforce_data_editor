using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapMinimap
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapMinimap));
            this.pictureBoxMapTexture = new System.Windows.Forms.PictureBox();
            this.pictureBoxCameraPosition = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMapTexture)).BeginInit();
            this.pictureBoxMapTexture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCameraPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxMapTexture
            // 
            this.pictureBoxMapTexture.BackColor = System.Drawing.Color.White;
            this.pictureBoxMapTexture.Controls.Add(this.pictureBoxCameraPosition);
            resources.ApplyResources(this.pictureBoxMapTexture, "pictureBoxMapTexture");
            this.pictureBoxMapTexture.Name = "pictureBoxMapTexture";
            this.pictureBoxMapTexture.TabStop = false;
            // 
            // pictureBoxCameraPosition
            // 
            this.pictureBoxCameraPosition.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBoxCameraPosition, "pictureBoxCameraPosition");
            this.pictureBoxCameraPosition.Name = "pictureBoxCameraPosition";
            this.pictureBoxCameraPosition.TabStop = false;
            this.pictureBoxCameraPosition.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxCameraPosition_MouseClick);
            // 
            // MapMinimap
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.pictureBoxMapTexture);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "MapMinimap";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMapTexture)).EndInit();
            this.pictureBoxMapTexture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCameraPosition)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxMapTexture;
        private System.Windows.Forms.PictureBox pictureBoxCameraPosition;
    }
}
