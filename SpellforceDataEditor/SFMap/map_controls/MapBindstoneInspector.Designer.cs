namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapBindstoneInspector
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
            PanelBindstonesList = new System.Windows.Forms.Panel();
            ListBindstones = new System.Windows.Forms.ListBox();
            label1 = new System.Windows.Forms.Label();
            ButtonResizeList = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            PosX = new System.Windows.Forms.TextBox();
            PosY = new System.Windows.Forms.TextBox();
            PanelProperties = new System.Windows.Forms.Panel();
            Unknown = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            TextID = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            AngleTrackbar = new System.Windows.Forms.TrackBar();
            Angle = new System.Windows.Forms.TextBox();
            PanelBindstonesList.SuspendLayout();
            PanelProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).BeginInit();
            SuspendLayout();
            // 
            // PanelBindstonesList
            // 
            PanelBindstonesList.Controls.Add(ListBindstones);
            PanelBindstonesList.Controls.Add(label1);
            PanelBindstonesList.Controls.Add(ButtonResizeList);
            PanelBindstonesList.Location = new System.Drawing.Point(4, 233);
            PanelBindstonesList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelBindstonesList.Name = "PanelBindstonesList";
            PanelBindstonesList.Size = new System.Drawing.Size(338, 183);
            PanelBindstonesList.TabIndex = 20;
            // 
            // ListBindstones
            // 
            ListBindstones.FormattingEnabled = true;
            ListBindstones.ItemHeight = 15;
            ListBindstones.Location = new System.Drawing.Point(5, 37);
            ListBindstones.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListBindstones.Name = "ListBindstones";
            ListBindstones.Size = new System.Drawing.Size(330, 139);
            ListBindstones.TabIndex = 20;
            ListBindstones.SelectedIndexChanged += ListBindstones_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(100, 15);
            label1.TabIndex = 19;
            label1.Text = "List of bindstones";
            // 
            // ButtonResizeList
            // 
            ButtonResizeList.Location = new System.Drawing.Point(309, 5);
            ButtonResizeList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonResizeList.Name = "ButtonResizeList";
            ButtonResizeList.Size = new System.Drawing.Size(26, 25);
            ButtonResizeList.TabIndex = 0;
            ButtonResizeList.Text = "-";
            ButtonResizeList.UseVisualStyleBackColor = true;
            ButtonResizeList.Click += ButtonResizeList_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 70);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(38, 15);
            label6.TabIndex = 10;
            label6.Text = "Angle";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(4, 37);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 15);
            label4.TabIndex = 6;
            label4.Text = "Position";
            // 
            // PosX
            // 
            PosX.Enabled = false;
            PosX.Location = new System.Drawing.Point(114, 33);
            PosX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosX.Name = "PosX";
            PosX.Size = new System.Drawing.Size(53, 23);
            PosX.TabIndex = 5;
            // 
            // PosY
            // 
            PosY.Enabled = false;
            PosY.Location = new System.Drawing.Point(177, 33);
            PosY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PosY.Name = "PosY";
            PosY.Size = new System.Drawing.Size(53, 23);
            PosY.TabIndex = 15;
            // 
            // PanelProperties
            // 
            PanelProperties.Controls.Add(Unknown);
            PanelProperties.Controls.Add(label3);
            PanelProperties.Controls.Add(TextID);
            PanelProperties.Controls.Add(label2);
            PanelProperties.Controls.Add(AngleTrackbar);
            PanelProperties.Controls.Add(Angle);
            PanelProperties.Controls.Add(PosY);
            PanelProperties.Controls.Add(PosX);
            PanelProperties.Controls.Add(label4);
            PanelProperties.Controls.Add(label6);
            PanelProperties.Enabled = false;
            PanelProperties.Location = new System.Drawing.Point(4, 0);
            PanelProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PanelProperties.Name = "PanelProperties";
            PanelProperties.Size = new System.Drawing.Size(338, 232);
            PanelProperties.TabIndex = 21;
            // 
            // Unknown
            // 
            Unknown.Location = new System.Drawing.Point(114, 97);
            Unknown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Unknown.Name = "Unknown";
            Unknown.Size = new System.Drawing.Size(116, 23);
            Unknown.TabIndex = 20;
            Unknown.Validated += Unknown_Validated;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(4, 100);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(58, 15);
            label3.TabIndex = 21;
            label3.Text = "Unknown";
            // 
            // TextID
            // 
            TextID.BackColor = System.Drawing.Color.DarkOrange;
            TextID.Location = new System.Drawing.Point(114, 3);
            TextID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TextID.Name = "TextID";
            TextID.ShortcutsEnabled = false;
            TextID.Size = new System.Drawing.Size(116, 23);
            TextID.TabIndex = 18;
            TextID.MouseDown += TextID_MouseDown;
            TextID.Validated += TextID_Validated;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 7);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(42, 15);
            label2.TabIndex = 19;
            label2.Text = "Text ID";
            // 
            // AngleTrackbar
            // 
            AngleTrackbar.AutoSize = false;
            AngleTrackbar.Location = new System.Drawing.Point(114, 63);
            AngleTrackbar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AngleTrackbar.Maximum = 359;
            AngleTrackbar.Name = "AngleTrackbar";
            AngleTrackbar.Size = new System.Drawing.Size(117, 23);
            AngleTrackbar.TabIndex = 17;
            AngleTrackbar.TickFrequency = 45;
            AngleTrackbar.ValueChanged += AngleTrackbar_ValueChanged;
            AngleTrackbar.MouseDown += AngleTrackbar_MouseDown;
            AngleTrackbar.MouseUp += AngleTrackbar_MouseUp;
            // 
            // Angle
            // 
            Angle.Enabled = false;
            Angle.Location = new System.Drawing.Point(239, 67);
            Angle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Angle.Name = "Angle";
            Angle.Size = new System.Drawing.Size(53, 23);
            Angle.TabIndex = 16;
            Angle.Validated += Angle_Validated;
            // 
            // MapBindstoneInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(PanelProperties);
            Controls.Add(PanelBindstonesList);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapBindstoneInspector";
            Size = new System.Drawing.Size(348, 421);
            Load += MapBindstoneInspector_Load;
            PanelBindstonesList.ResumeLayout(false);
            PanelBindstonesList.PerformLayout();
            PanelProperties.ResumeLayout(false);
            PanelProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AngleTrackbar).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel PanelBindstonesList;
        private System.Windows.Forms.ListBox ListBindstones;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonResizeList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PosX;
        private System.Windows.Forms.TextBox PosY;
        private System.Windows.Forms.Panel PanelProperties;
        private System.Windows.Forms.TrackBar AngleTrackbar;
        private System.Windows.Forms.TextBox Angle;
        private System.Windows.Forms.TextBox Unknown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextID;
        private System.Windows.Forms.Label label2;
    }
}
