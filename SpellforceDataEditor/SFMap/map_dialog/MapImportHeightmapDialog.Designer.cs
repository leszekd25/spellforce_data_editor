namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapImportHeightmapDialog
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
            this.button1 = new System.Windows.Forms.Button();
            this.PreviewPic = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.HMapScale = new System.Windows.Forms.TrackBar();
            this.OpenHMapBitMap = new System.Windows.Forms.OpenFileDialog();
            this.HMapScaleText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.HMapOffset = new System.Windows.Forms.TrackBar();
            this.HMapOffsetText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HMapScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HMapOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(199, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select heightmap file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PreviewPic
            // 
            this.PreviewPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PreviewPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewPic.Location = new System.Drawing.Point(347, 12);
            this.PreviewPic.Name = "PreviewPic";
            this.PreviewPic.Size = new System.Drawing.Size(128, 128);
            this.PreviewPic.TabIndex = 1;
            this.PreviewPic.TabStop = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(217, 116);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Accept";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Heightmap scale";
            // 
            // HMapScale
            // 
            this.HMapScale.AutoSize = false;
            this.HMapScale.Location = new System.Drawing.Point(217, 41);
            this.HMapScale.Maximum = 50;
            this.HMapScale.Minimum = 1;
            this.HMapScale.Name = "HMapScale";
            this.HMapScale.Size = new System.Drawing.Size(104, 22);
            this.HMapScale.TabIndex = 5;
            this.HMapScale.TickFrequency = 5;
            this.HMapScale.Value = 50;
            this.HMapScale.ValueChanged += new System.EventHandler(this.HMapScale_ValueChanged);
            // 
            // OpenHMapBitMap
            // 
            this.OpenHMapBitMap.Filter = "Bitmap files|*.bmp|PNG files|*.png";
            // 
            // HMapScaleText
            // 
            this.HMapScaleText.Location = new System.Drawing.Point(111, 41);
            this.HMapScaleText.Name = "HMapScaleText";
            this.HMapScaleText.Size = new System.Drawing.Size(100, 20);
            this.HMapScaleText.TabIndex = 6;
            this.HMapScaleText.Text = "50";
            this.HMapScaleText.Validated += new System.EventHandler(this.HMapScaleText_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Height offset";
            // 
            // HMapOffset
            // 
            this.HMapOffset.AutoSize = false;
            this.HMapOffset.Location = new System.Drawing.Point(217, 67);
            this.HMapOffset.Maximum = 255;
            this.HMapOffset.Minimum = 1;
            this.HMapOffset.Name = "HMapOffset";
            this.HMapOffset.Size = new System.Drawing.Size(104, 22);
            this.HMapOffset.TabIndex = 8;
            this.HMapOffset.TickFrequency = 25;
            this.HMapOffset.Value = 55;
            this.HMapOffset.ValueChanged += new System.EventHandler(this.HMapOffset_ValueChanged);
            // 
            // HMapOffsetText
            // 
            this.HMapOffsetText.Location = new System.Drawing.Point(111, 69);
            this.HMapOffsetText.Name = "HMapOffsetText";
            this.HMapOffsetText.Size = new System.Drawing.Size(100, 20);
            this.HMapOffsetText.TabIndex = 9;
            this.HMapOffsetText.Text = "55";
            this.HMapOffsetText.Validated += new System.EventHandler(this.HMapOffsetText_Validated);
            // 
            // MapImportHeightmapDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 151);
            this.Controls.Add(this.HMapOffsetText);
            this.Controls.Add(this.HMapOffset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HMapScaleText);
            this.Controls.Add(this.HMapScale);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.PreviewPic);
            this.Controls.Add(this.button1);
            this.Name = "MapImportHeightmapDialog";
            this.Text = "Import heightmap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapImportHeightmapDialog_FormClosing);
            this.Load += new System.EventHandler(this.MapImportHeightmapDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HMapScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HMapOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox PreviewPic;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar HMapScale;
        private System.Windows.Forms.OpenFileDialog OpenHMapBitMap;
        private System.Windows.Forms.TextBox HMapScaleText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar HMapOffset;
        private System.Windows.Forms.TextBox HMapOffsetText;
    }
}