namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapAutoTextureDialog
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
            this.SlopeValueTrackbar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelBelowThreshold = new System.Windows.Forms.Panel();
            this.ButtonAddBelow = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SlopeValue = new System.Windows.Forms.TextBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonApply = new System.Windows.Forms.Button();
            this.PanelAboveThreshold = new System.Windows.Forms.Panel();
            this.ButtonAddAbove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SlopeValueTrackbar)).BeginInit();
            this.PanelBelowThreshold.SuspendLayout();
            this.PanelAboveThreshold.SuspendLayout();
            this.SuspendLayout();
            // 
            // SlopeValueTrackbar
            // 
            this.SlopeValueTrackbar.Location = new System.Drawing.Point(101, 12);
            this.SlopeValueTrackbar.Maximum = 90;
            this.SlopeValueTrackbar.Name = "SlopeValueTrackbar";
            this.SlopeValueTrackbar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.SlopeValueTrackbar.Size = new System.Drawing.Size(45, 334);
            this.SlopeValueTrackbar.SmallChange = 5;
            this.SlopeValueTrackbar.TabIndex = 0;
            this.SlopeValueTrackbar.TickFrequency = 10;
            this.SlopeValueTrackbar.Value = 30;
            this.SlopeValueTrackbar.ValueChanged += new System.EventHandler(this.SlopeValueTrackbar_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Slope  threshold";
            // 
            // PanelBelowThreshold
            // 
            this.PanelBelowThreshold.Controls.Add(this.ButtonAddBelow);
            this.PanelBelowThreshold.Controls.Add(this.label5);
            this.PanelBelowThreshold.Controls.Add(this.label2);
            this.PanelBelowThreshold.Controls.Add(this.label4);
            this.PanelBelowThreshold.Location = new System.Drawing.Point(152, 182);
            this.PanelBelowThreshold.Name = "PanelBelowThreshold";
            this.PanelBelowThreshold.Size = new System.Drawing.Size(421, 164);
            this.PanelBelowThreshold.TabIndex = 0;
            // 
            // ButtonAddBelow
            // 
            this.ButtonAddBelow.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonAddBelow.Location = new System.Drawing.Point(58, 58);
            this.ButtonAddBelow.Name = "ButtonAddBelow";
            this.ButtonAddBelow.Size = new System.Drawing.Size(70, 70);
            this.ButtonAddBelow.TabIndex = 6;
            this.ButtonAddBelow.Text = "+";
            this.ButtonAddBelow.UseVisualStyleBackColor = true;
            this.ButtonAddBelow.Click += new System.EventHandler(this.ButtonAddBelow_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Weight";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tiles below threshold";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Tile ID";
            // 
            // SlopeValue
            // 
            this.SlopeValue.Location = new System.Drawing.Point(12, 182);
            this.SlopeValue.Name = "SlopeValue";
            this.SlopeValue.Size = new System.Drawing.Size(83, 20);
            this.SlopeValue.TabIndex = 3;
            this.SlopeValue.Text = "30";
            this.SlopeValue.Validated += new System.EventHandler(this.SlopeValue_Validated);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(12, 352);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Close";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonApply
            // 
            this.ButtonApply.Location = new System.Drawing.Point(498, 352);
            this.ButtonApply.Name = "ButtonApply";
            this.ButtonApply.Size = new System.Drawing.Size(75, 23);
            this.ButtonApply.TabIndex = 5;
            this.ButtonApply.Text = "Apply";
            this.ButtonApply.UseVisualStyleBackColor = true;
            this.ButtonApply.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // PanelAboveThreshold
            // 
            this.PanelAboveThreshold.Controls.Add(this.ButtonAddAbove);
            this.PanelAboveThreshold.Controls.Add(this.label3);
            this.PanelAboveThreshold.Controls.Add(this.label6);
            this.PanelAboveThreshold.Controls.Add(this.label7);
            this.PanelAboveThreshold.Location = new System.Drawing.Point(152, 12);
            this.PanelAboveThreshold.Name = "PanelAboveThreshold";
            this.PanelAboveThreshold.Size = new System.Drawing.Size(421, 164);
            this.PanelAboveThreshold.TabIndex = 7;
            // 
            // ButtonAddAbove
            // 
            this.ButtonAddAbove.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonAddAbove.Location = new System.Drawing.Point(58, 58);
            this.ButtonAddAbove.Name = "ButtonAddAbove";
            this.ButtonAddAbove.Size = new System.Drawing.Size(70, 70);
            this.ButtonAddAbove.TabIndex = 6;
            this.ButtonAddAbove.Text = "+";
            this.ButtonAddAbove.UseVisualStyleBackColor = true;
            this.ButtonAddAbove.Click += new System.EventHandler(this.ButtonAddAbove_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Weight";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Tiles above threshold";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Tile ID";
            // 
            // MapAutoTextureDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 388);
            this.Controls.Add(this.PanelAboveThreshold);
            this.Controls.Add(this.ButtonApply);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.SlopeValue);
            this.Controls.Add(this.PanelBelowThreshold);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SlopeValueTrackbar);
            this.Name = "MapAutoTextureDialog";
            this.Text = "MapAutoTextureDialog";
            ((System.ComponentModel.ISupportInitialize)(this.SlopeValueTrackbar)).EndInit();
            this.PanelBelowThreshold.ResumeLayout(false);
            this.PanelBelowThreshold.PerformLayout();
            this.PanelAboveThreshold.ResumeLayout(false);
            this.PanelAboveThreshold.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar SlopeValueTrackbar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel PanelBelowThreshold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox SlopeValue;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonApply;
        private System.Windows.Forms.Button ButtonAddBelow;
        private System.Windows.Forms.Panel PanelAboveThreshold;
        private System.Windows.Forms.Button ButtonAddAbove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}