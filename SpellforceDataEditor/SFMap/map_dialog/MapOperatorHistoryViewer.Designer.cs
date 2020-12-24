namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapOperatorHistoryViewer
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
            this.TreeOperators = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // TreeOperators
            // 
            this.TreeOperators.Location = new System.Drawing.Point(12, 12);
            this.TreeOperators.Name = "TreeOperators";
            this.TreeOperators.Size = new System.Drawing.Size(428, 387);
            this.TreeOperators.TabIndex = 0;
            // 
            // MapOperatorHistoryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 409);
            this.Controls.Add(this.TreeOperators);
            this.Name = "MapOperatorHistoryViewer";
            this.Text = "Operation history";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TreeOperators;
    }
}