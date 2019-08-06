namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorNPCControl
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
            this.ListNPCs = new System.Windows.Forms.ListBox();
            this.ButtonRemoveNPC = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectedNPCID = new System.Windows.Forms.TextBox();
            this.LabelNPCName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ButtonNPCMapProperties = new System.Windows.Forms.Button();
            this.ButtonCreateNPC = new System.Windows.Forms.Button();
            this.ButtonOpenScript = new System.Windows.Forms.Button();
            this.ButtonPlatformScripts = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListNPCs
            // 
            this.ListNPCs.FormattingEnabled = true;
            this.ListNPCs.Location = new System.Drawing.Point(21, 41);
            this.ListNPCs.Name = "ListNPCs";
            this.ListNPCs.Size = new System.Drawing.Size(211, 212);
            this.ListNPCs.TabIndex = 0;
            this.ListNPCs.SelectedIndexChanged += new System.EventHandler(this.ListNPCs_SelectedIndexChanged);
            // 
            // ButtonRemoveNPC
            // 
            this.ButtonRemoveNPC.Location = new System.Drawing.Point(353, 69);
            this.ButtonRemoveNPC.Name = "ButtonRemoveNPC";
            this.ButtonRemoveNPC.Size = new System.Drawing.Size(87, 23);
            this.ButtonRemoveNPC.TabIndex = 1;
            this.ButtonRemoveNPC.Text = "Remove";
            this.ButtonRemoveNPC.UseVisualStyleBackColor = true;
            this.ButtonRemoveNPC.Click += new System.EventHandler(this.ButtonRemoveNPC_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(238, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "NPC ID";
            // 
            // SelectedNPCID
            // 
            this.SelectedNPCID.BackColor = System.Drawing.Color.DarkOrange;
            this.SelectedNPCID.Location = new System.Drawing.Point(287, 43);
            this.SelectedNPCID.Name = "SelectedNPCID";
            this.SelectedNPCID.Size = new System.Drawing.Size(60, 20);
            this.SelectedNPCID.TabIndex = 3;
            this.SelectedNPCID.Validated += new System.EventHandler(this.SelectedNPCID_Validated);
            // 
            // LabelNPCName
            // 
            this.LabelNPCName.AutoSize = true;
            this.LabelNPCName.Location = new System.Drawing.Point(238, 16);
            this.LabelNPCName.Name = "LabelNPCName";
            this.LabelNPCName.Size = new System.Drawing.Size(0, 13);
            this.LabelNPCName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "List of NPCs:";
            // 
            // ButtonNPCMapProperties
            // 
            this.ButtonNPCMapProperties.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ButtonNPCMapProperties.Location = new System.Drawing.Point(238, 69);
            this.ButtonNPCMapProperties.Name = "ButtonNPCMapProperties";
            this.ButtonNPCMapProperties.Size = new System.Drawing.Size(109, 23);
            this.ButtonNPCMapProperties.TabIndex = 12;
            this.ButtonNPCMapProperties.Text = "Go to properties";
            this.ButtonNPCMapProperties.UseVisualStyleBackColor = false;
            this.ButtonNPCMapProperties.Click += new System.EventHandler(this.ButtonNPCMapProperties_Click);
            // 
            // ButtonCreateNPC
            // 
            this.ButtonCreateNPC.BackColor = System.Drawing.Color.DarkOrange;
            this.ButtonCreateNPC.Location = new System.Drawing.Point(353, 41);
            this.ButtonCreateNPC.Name = "ButtonCreateNPC";
            this.ButtonCreateNPC.Size = new System.Drawing.Size(87, 23);
            this.ButtonCreateNPC.TabIndex = 13;
            this.ButtonCreateNPC.Text = "Create new ID";
            this.ButtonCreateNPC.UseVisualStyleBackColor = false;
            this.ButtonCreateNPC.Click += new System.EventHandler(this.ButtonCreateNPC_Click);
            // 
            // ButtonOpenScript
            // 
            this.ButtonOpenScript.Location = new System.Drawing.Point(238, 98);
            this.ButtonOpenScript.Name = "ButtonOpenScript";
            this.ButtonOpenScript.Size = new System.Drawing.Size(109, 23);
            this.ButtonOpenScript.TabIndex = 14;
            this.ButtonOpenScript.Text = "Open NPC script";
            this.ButtonOpenScript.UseVisualStyleBackColor = true;
            this.ButtonOpenScript.Click += new System.EventHandler(this.ButtonOpenScript_Click);
            // 
            // ButtonPlatformScripts
            // 
            this.ButtonPlatformScripts.Location = new System.Drawing.Point(238, 11);
            this.ButtonPlatformScripts.Name = "ButtonPlatformScripts";
            this.ButtonPlatformScripts.Size = new System.Drawing.Size(109, 23);
            this.ButtonPlatformScripts.TabIndex = 15;
            this.ButtonPlatformScripts.Text = "Open platform script";
            this.ButtonPlatformScripts.UseVisualStyleBackColor = true;
            this.ButtonPlatformScripts.Click += new System.EventHandler(this.ButtonPlatformScripts_Click);
            // 
            // MapInspectorNPCControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.ButtonPlatformScripts);
            this.Controls.Add(this.ButtonOpenScript);
            this.Controls.Add(this.ButtonCreateNPC);
            this.Controls.Add(this.ButtonNPCMapProperties);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LabelNPCName);
            this.Controls.Add(this.SelectedNPCID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonRemoveNPC);
            this.Controls.Add(this.ListNPCs);
            this.Name = "MapInspectorNPCControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorNPCControl_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListNPCs;
        private System.Windows.Forms.Button ButtonRemoveNPC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SelectedNPCID;
        private System.Windows.Forms.Label LabelNPCName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButtonNPCMapProperties;
        private System.Windows.Forms.Button ButtonCreateNPC;
        private System.Windows.Forms.Button ButtonOpenScript;
        private System.Windows.Forms.Button ButtonPlatformScripts;
    }
}
