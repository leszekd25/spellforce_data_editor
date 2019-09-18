namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapCoopCampInspector
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
            this.PanelCoopCampList = new System.Windows.Forms.Panel();
            this.ListCoopCamps = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonResizeList = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.Unknown1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PosX = new System.Windows.Forms.TextBox();
            this.PosY = new System.Windows.Forms.TextBox();
            this.CampID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PanelProperties = new System.Windows.Forms.Panel();
            this.PanelCoopCampList.SuspendLayout();
            this.PanelProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelCoopCampList
            // 
            this.PanelCoopCampList.Controls.Add(this.ListCoopCamps);
            this.PanelCoopCampList.Controls.Add(this.label1);
            this.PanelCoopCampList.Controls.Add(this.ButtonResizeList);
            this.PanelCoopCampList.Location = new System.Drawing.Point(3, 202);
            this.PanelCoopCampList.Name = "PanelCoopCampList";
            this.PanelCoopCampList.Size = new System.Drawing.Size(290, 159);
            this.PanelCoopCampList.TabIndex = 20;
            // 
            // ListCoopCamps
            // 
            this.ListCoopCamps.FormattingEnabled = true;
            this.ListCoopCamps.Location = new System.Drawing.Point(4, 32);
            this.ListCoopCamps.Name = "ListCoopCamps";
            this.ListCoopCamps.Size = new System.Drawing.Size(283, 121);
            this.ListCoopCamps.TabIndex = 20;
            this.ListCoopCamps.SelectedIndexChanged += new System.EventHandler(this.ListCoopCamps_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "List of coop camps";
            // 
            // ButtonResizeList
            // 
            this.ButtonResizeList.Location = new System.Drawing.Point(265, 4);
            this.ButtonResizeList.Name = "ButtonResizeList";
            this.ButtonResizeList.Size = new System.Drawing.Size(22, 22);
            this.ButtonResizeList.TabIndex = 0;
            this.ButtonResizeList.Text = "-";
            this.ButtonResizeList.UseVisualStyleBackColor = true;
            this.ButtonResizeList.Click += new System.EventHandler(this.ButtonResizeList_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Unknown";
            // 
            // Unknown1
            // 
            this.Unknown1.Location = new System.Drawing.Point(98, 55);
            this.Unknown1.Name = "Unknown1";
            this.Unknown1.Size = new System.Drawing.Size(100, 20);
            this.Unknown1.TabIndex = 9;
            this.Unknown1.Validated += new System.EventHandler(this.Unknown1_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Position";
            // 
            // PosX
            // 
            this.PosX.Enabled = false;
            this.PosX.Location = new System.Drawing.Point(98, 29);
            this.PosX.Name = "PosX";
            this.PosX.Size = new System.Drawing.Size(46, 20);
            this.PosX.TabIndex = 5;
            // 
            // PosY
            // 
            this.PosY.Enabled = false;
            this.PosY.Location = new System.Drawing.Point(152, 29);
            this.PosY.Name = "PosY";
            this.PosY.Size = new System.Drawing.Size(46, 20);
            this.PosY.TabIndex = 15;
            // 
            // CampID
            // 
            this.CampID.BackColor = System.Drawing.Color.DarkOrange;
            this.CampID.Location = new System.Drawing.Point(98, 3);
            this.CampID.Name = "CampID";
            this.CampID.Size = new System.Drawing.Size(100, 20);
            this.CampID.TabIndex = 1;
            this.CampID.Validated += new System.EventHandler(this.CampID_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Camp type";
            // 
            // PanelProperties
            // 
            this.PanelProperties.Controls.Add(this.label2);
            this.PanelProperties.Controls.Add(this.CampID);
            this.PanelProperties.Controls.Add(this.PosY);
            this.PanelProperties.Controls.Add(this.PosX);
            this.PanelProperties.Controls.Add(this.label4);
            this.PanelProperties.Controls.Add(this.Unknown1);
            this.PanelProperties.Controls.Add(this.label6);
            this.PanelProperties.Enabled = false;
            this.PanelProperties.Location = new System.Drawing.Point(3, 0);
            this.PanelProperties.Name = "PanelProperties";
            this.PanelProperties.Size = new System.Drawing.Size(290, 201);
            this.PanelProperties.TabIndex = 21;
            // 
            // MapCoopCampInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.PanelProperties);
            this.Controls.Add(this.PanelCoopCampList);
            this.Name = "MapCoopCampInspector";
            this.Size = new System.Drawing.Size(298, 365);
            this.Load += new System.EventHandler(this.MapCoopCampInspector_Load);
            this.PanelCoopCampList.ResumeLayout(false);
            this.PanelCoopCampList.PerformLayout();
            this.PanelProperties.ResumeLayout(false);
            this.PanelProperties.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PanelCoopCampList;
        private System.Windows.Forms.ListBox ListCoopCamps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Unknown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.TextBox CampID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PanelProperties;
    }
}
