namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorDecorationControl
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
            this.DecorationBrush = new SpellforceDataEditor.SFMap.map_controls.MapBrushControl();
            this.ListGroups = new System.Windows.Forms.ListBox();
            this.ListGroupElements = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DecorationWeight = new System.Windows.Forms.TextBox();
            this.DecorationID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LabelSelectedGroup = new System.Windows.Forms.Label();
            this.ButtonAddElement = new System.Windows.Forms.Button();
            this.ButtonRemoveElement = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DecorationBrush
            // 
            this.DecorationBrush.Location = new System.Drawing.Point(3, 3);
            this.DecorationBrush.Name = "DecorationBrush";
            this.DecorationBrush.Size = new System.Drawing.Size(310, 124);
            this.DecorationBrush.TabIndex = 0;
            // 
            // ListGroups
            // 
            this.ListGroups.FormattingEnabled = true;
            this.ListGroups.Location = new System.Drawing.Point(19, 164);
            this.ListGroups.Name = "ListGroups";
            this.ListGroups.Size = new System.Drawing.Size(111, 199);
            this.ListGroups.TabIndex = 1;
            this.ListGroups.SelectedIndexChanged += new System.EventHandler(this.ListGroups_SelectedIndexChanged);
            // 
            // ListGroupElements
            // 
            this.ListGroupElements.FormattingEnabled = true;
            this.ListGroupElements.Location = new System.Drawing.Point(136, 164);
            this.ListGroupElements.Name = "ListGroupElements";
            this.ListGroupElements.Size = new System.Drawing.Size(115, 199);
            this.ListGroupElements.TabIndex = 2;
            this.ListGroupElements.SelectedIndexChanged += new System.EventHandler(this.ListGroupElements_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(261, 320);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Decoration ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(261, 346);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Weight (?)";
            // 
            // DecorationWeight
            // 
            this.DecorationWeight.Location = new System.Drawing.Point(340, 343);
            this.DecorationWeight.Name = "DecorationWeight";
            this.DecorationWeight.Size = new System.Drawing.Size(41, 20);
            this.DecorationWeight.TabIndex = 5;
            this.DecorationWeight.Validated += new System.EventHandler(this.DecorationWeight_Validated);
            // 
            // DecorationID
            // 
            this.DecorationID.Location = new System.Drawing.Point(340, 317);
            this.DecorationID.Name = "DecorationID";
            this.DecorationID.Size = new System.Drawing.Size(100, 20);
            this.DecorationID.TabIndex = 6;
            this.DecorationID.Validated += new System.EventHandler(this.DecorationID_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Current group:";
            // 
            // LabelSelectedGroup
            // 
            this.LabelSelectedGroup.AutoSize = true;
            this.LabelSelectedGroup.Location = new System.Drawing.Point(95, 148);
            this.LabelSelectedGroup.Name = "LabelSelectedGroup";
            this.LabelSelectedGroup.Size = new System.Drawing.Size(0, 13);
            this.LabelSelectedGroup.TabIndex = 8;
            // 
            // ButtonAddElement
            // 
            this.ButtonAddElement.Location = new System.Drawing.Point(262, 164);
            this.ButtonAddElement.Name = "ButtonAddElement";
            this.ButtonAddElement.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddElement.TabIndex = 9;
            this.ButtonAddElement.Text = "Add";
            this.ButtonAddElement.UseVisualStyleBackColor = true;
            this.ButtonAddElement.Click += new System.EventHandler(this.ButtonAddElement_Click);
            // 
            // ButtonRemoveElement
            // 
            this.ButtonRemoveElement.Location = new System.Drawing.Point(262, 193);
            this.ButtonRemoveElement.Name = "ButtonRemoveElement";
            this.ButtonRemoveElement.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemoveElement.TabIndex = 10;
            this.ButtonRemoveElement.Text = "Remove";
            this.ButtonRemoveElement.UseVisualStyleBackColor = true;
            this.ButtonRemoveElement.Click += new System.EventHandler(this.ButtonRemoveElement_Click);
            // 
            // MapInspectorDecorationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ButtonRemoveElement);
            this.Controls.Add(this.ButtonAddElement);
            this.Controls.Add(this.LabelSelectedGroup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DecorationID);
            this.Controls.Add(this.DecorationWeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ListGroupElements);
            this.Controls.Add(this.ListGroups);
            this.Controls.Add(this.DecorationBrush);
            this.Name = "MapInspectorDecorationControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorDecorationControl_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapBrushControl DecorationBrush;
        private System.Windows.Forms.ListBox ListGroups;
        private System.Windows.Forms.ListBox ListGroupElements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DecorationWeight;
        private System.Windows.Forms.TextBox DecorationID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label LabelSelectedGroup;
        private System.Windows.Forms.Button ButtonAddElement;
        private System.Windows.Forms.Button ButtonRemoveElement;
    }
}
