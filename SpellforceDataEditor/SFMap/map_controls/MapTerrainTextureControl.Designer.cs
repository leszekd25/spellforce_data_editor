namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapTerrainTextureControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonTextureImage = new System.Windows.Forms.Button();
            this.ButtonTextureID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonTextureImage
            // 
            this.ButtonTextureImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonTextureImage.Location = new System.Drawing.Point(3, 3);
            this.ButtonTextureImage.Name = "ButtonTextureImage";
            this.ButtonTextureImage.Size = new System.Drawing.Size(64, 64);
            this.ButtonTextureImage.TabIndex = 0;
            this.ButtonTextureImage.UseVisualStyleBackColor = true;
            this.ButtonTextureImage.Click += new System.EventHandler(this.ButtonTextureImage_Click);
            // 
            // ButtonTextureID
            // 
            this.ButtonTextureID.Location = new System.Drawing.Point(3, 70);
            this.ButtonTextureID.Name = "ButtonTextureID";
            this.ButtonTextureID.Size = new System.Drawing.Size(64, 16);
            this.ButtonTextureID.TabIndex = 1;
            this.ButtonTextureID.Text = "0";
            this.ButtonTextureID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MapTerrainTextureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonTextureID);
            this.Controls.Add(this.ButtonTextureImage);
            this.Name = "MapTerrainTextureControl";
            this.Size = new System.Drawing.Size(70, 89);
            this.Resize += new System.EventHandler(this.MapTerrainTextureControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonTextureImage;
        private System.Windows.Forms.Label ButtonTextureID;
    }
}
