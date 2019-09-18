namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapModifyTextureSet
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
            this.PanelTextureSet = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PanelAllTextures = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PanelTextureSet
            // 
            this.PanelTextureSet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelTextureSet.Location = new System.Drawing.Point(12, 42);
            this.PanelTextureSet.Name = "PanelTextureSet";
            this.PanelTextureSet.Size = new System.Drawing.Size(378, 350);
            this.PanelTextureSet.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(378, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current texture set";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(378, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Select a texture from this set to modify it";
            // 
            // PanelAllTextures
            // 
            this.PanelAllTextures.AutoScroll = true;
            this.PanelAllTextures.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelAllTextures.Location = new System.Drawing.Point(431, 42);
            this.PanelAllTextures.Name = "PanelAllTextures";
            this.PanelAllTextures.Size = new System.Drawing.Size(406, 350);
            this.PanelAllTextures.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(428, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(378, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Select a texture here to replace currently selected texture from texture set";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(428, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(378, 14);
            this.label4.TabIndex = 4;
            this.label4.Text = "All available textures";
            // 
            // MapModifyTextureSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 432);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PanelAllTextures);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PanelTextureSet);
            this.Name = "MapModifyTextureSet";
            this.Text = "Modify texture set";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelTextureSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel PanelAllTextures;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}