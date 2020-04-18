namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleMovieEffectControl
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
            this.ButtonMaximize = new System.Windows.Forms.Button();
            this.ButtonMoveUp = new System.Windows.Forms.Button();
            this.ButtonMoveDown = new System.Windows.Forms.Button();
            this.LabelName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TextBoxRange = new System.Windows.Forms.TextBox();
            this.TextBoxStart = new System.Windows.Forms.TextBox();
            this.ComboPlay = new System.Windows.Forms.ComboBox();
            this.ComboPath = new System.Windows.Forms.ComboBox();
            this.ComboDim = new System.Windows.Forms.ComboBox();
            this.ComboTrail = new System.Windows.Forms.ComboBox();
            this.TextBoxTrailConstant = new System.Windows.Forms.TextBox();
            this.ButtonCustomPath = new System.Windows.Forms.Button();
            this.ButtonCustomTrail = new System.Windows.Forms.Button();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonMaximize
            // 
            this.ButtonMaximize.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonMaximize.Location = new System.Drawing.Point(323, 3);
            this.ButtonMaximize.Name = "ButtonMaximize";
            this.ButtonMaximize.Size = new System.Drawing.Size(34, 34);
            this.ButtonMaximize.TabIndex = 0;
            this.ButtonMaximize.Text = "-";
            this.ButtonMaximize.UseVisualStyleBackColor = true;
            this.ButtonMaximize.Click += new System.EventHandler(this.ButtonMaximize_Click);
            // 
            // ButtonMoveUp
            // 
            this.ButtonMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonMoveUp.Location = new System.Drawing.Point(243, 3);
            this.ButtonMoveUp.Name = "ButtonMoveUp";
            this.ButtonMoveUp.Size = new System.Drawing.Size(34, 34);
            this.ButtonMoveUp.TabIndex = 1;
            this.ButtonMoveUp.Text = "^";
            this.ButtonMoveUp.UseVisualStyleBackColor = true;
            this.ButtonMoveUp.Click += new System.EventHandler(this.ButtonMoveUp_Click);
            // 
            // ButtonMoveDown
            // 
            this.ButtonMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonMoveDown.Location = new System.Drawing.Point(283, 3);
            this.ButtonMoveDown.Name = "ButtonMoveDown";
            this.ButtonMoveDown.Size = new System.Drawing.Size(34, 34);
            this.ButtonMoveDown.TabIndex = 2;
            this.ButtonMoveDown.Text = "v";
            this.ButtonMoveDown.UseVisualStyleBackColor = true;
            this.ButtonMoveDown.Click += new System.EventHandler(this.ButtonMoveDown_Click);
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LabelName.Location = new System.Drawing.Point(3, 13);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(52, 13);
            this.LabelName.TabIndex = 3;
            this.LabelName.Text = "Modifier";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Start";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Range";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Play mode";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Path type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Dim mode";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Trail";
            // 
            // TextBoxRange
            // 
            this.TextBoxRange.Location = new System.Drawing.Point(283, 43);
            this.TextBoxRange.Name = "TextBoxRange";
            this.TextBoxRange.Size = new System.Drawing.Size(114, 20);
            this.TextBoxRange.TabIndex = 10;
            this.TextBoxRange.Validated += new System.EventHandler(this.TextBoxRange_Validated);
            // 
            // TextBoxStart
            // 
            this.TextBoxStart.Location = new System.Drawing.Point(70, 43);
            this.TextBoxStart.Name = "TextBoxStart";
            this.TextBoxStart.Size = new System.Drawing.Size(121, 20);
            this.TextBoxStart.TabIndex = 11;
            this.TextBoxStart.Validated += new System.EventHandler(this.TextBoxStart_Validated);
            // 
            // ComboPlay
            // 
            this.ComboPlay.FormattingEnabled = true;
            this.ComboPlay.Items.AddRange(new object[] {
            "Once",
            "Looped",
            "Bounce",
            "Clamped",
            "Stretched",
            "Continuous"});
            this.ComboPlay.Location = new System.Drawing.Point(70, 69);
            this.ComboPlay.Name = "ComboPlay";
            this.ComboPlay.Size = new System.Drawing.Size(121, 21);
            this.ComboPlay.TabIndex = 12;
            this.ComboPlay.SelectedIndexChanged += new System.EventHandler(this.ComboPlay_SelectedIndexChanged);
            // 
            // ComboPath
            // 
            this.ComboPath.FormattingEnabled = true;
            this.ComboPath.Items.AddRange(new object[] {
            "Linear",
            "Cosine",
            "Sine",
            "Parabola",
            "Inverse parabola",
            "Random",
            "Custom"});
            this.ComboPath.Location = new System.Drawing.Point(70, 123);
            this.ComboPath.Name = "ComboPath";
            this.ComboPath.Size = new System.Drawing.Size(121, 21);
            this.ComboPath.TabIndex = 13;
            this.ComboPath.SelectedIndexChanged += new System.EventHandler(this.ComboPath_SelectedIndexChanged);
            // 
            // ComboDim
            // 
            this.ComboDim.FormattingEnabled = true;
            this.ComboDim.Items.AddRange(new object[] {
            "Time",
            "Particle",
            "Power",
            "Time to end",
            "Time scaled",
            "Time absolute",
            "Random",
            "Target size",
            "Player"});
            this.ComboDim.Location = new System.Drawing.Point(70, 96);
            this.ComboDim.Name = "ComboDim";
            this.ComboDim.Size = new System.Drawing.Size(121, 21);
            this.ComboDim.TabIndex = 14;
            this.ComboDim.SelectedIndexChanged += new System.EventHandler(this.ComboDim_SelectedIndexChanged);
            // 
            // ComboTrail
            // 
            this.ComboTrail.FormattingEnabled = true;
            this.ComboTrail.Items.AddRange(new object[] {
            "Constant",
            "Variable"});
            this.ComboTrail.Location = new System.Drawing.Point(70, 150);
            this.ComboTrail.Name = "ComboTrail";
            this.ComboTrail.Size = new System.Drawing.Size(121, 21);
            this.ComboTrail.TabIndex = 15;
            this.ComboTrail.SelectedIndexChanged += new System.EventHandler(this.ComboTrail_SelectedIndexChanged);
            // 
            // TextBoxTrailConstant
            // 
            this.TextBoxTrailConstant.Location = new System.Drawing.Point(283, 150);
            this.TextBoxTrailConstant.Name = "TextBoxTrailConstant";
            this.TextBoxTrailConstant.Size = new System.Drawing.Size(114, 20);
            this.TextBoxTrailConstant.TabIndex = 16;
            this.TextBoxTrailConstant.Validated += new System.EventHandler(this.TextBoxTrailConstant_Validated);
            // 
            // ButtonCustomPath
            // 
            this.ButtonCustomPath.Location = new System.Drawing.Point(283, 121);
            this.ButtonCustomPath.Name = "ButtonCustomPath";
            this.ButtonCustomPath.Size = new System.Drawing.Size(114, 23);
            this.ButtonCustomPath.TabIndex = 17;
            this.ButtonCustomPath.Text = "Custom path...";
            this.ButtonCustomPath.UseVisualStyleBackColor = true;
            this.ButtonCustomPath.Visible = false;
            this.ButtonCustomPath.Click += new System.EventHandler(this.ButtonCustomPath_Click);
            // 
            // ButtonCustomTrail
            // 
            this.ButtonCustomTrail.Location = new System.Drawing.Point(283, 176);
            this.ButtonCustomTrail.Name = "ButtonCustomTrail";
            this.ButtonCustomTrail.Size = new System.Drawing.Size(114, 23);
            this.ButtonCustomTrail.TabIndex = 18;
            this.ButtonCustomTrail.Text = "Custom trail...";
            this.ButtonCustomTrail.UseVisualStyleBackColor = true;
            this.ButtonCustomTrail.Visible = false;
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ButtonRemove.Location = new System.Drawing.Point(363, 3);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(34, 34);
            this.ButtonRemove.TabIndex = 19;
            this.ButtonRemove.Text = "x";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // ParticleMovieEffectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonCustomTrail);
            this.Controls.Add(this.ButtonCustomPath);
            this.Controls.Add(this.TextBoxTrailConstant);
            this.Controls.Add(this.ComboTrail);
            this.Controls.Add(this.ComboDim);
            this.Controls.Add(this.ComboPath);
            this.Controls.Add(this.ComboPlay);
            this.Controls.Add(this.TextBoxStart);
            this.Controls.Add(this.TextBoxRange);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelName);
            this.Controls.Add(this.ButtonMoveDown);
            this.Controls.Add(this.ButtonMoveUp);
            this.Controls.Add(this.ButtonMaximize);
            this.Name = "ParticleMovieEffectControl";
            this.Size = new System.Drawing.Size(400, 202);
            this.Load += new System.EventHandler(this.ParticleMovieEffectControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonMaximize;
        private System.Windows.Forms.Button ButtonMoveUp;
        private System.Windows.Forms.Button ButtonMoveDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TextBoxRange;
        private System.Windows.Forms.TextBox TextBoxStart;
        private System.Windows.Forms.ComboBox ComboPlay;
        private System.Windows.Forms.ComboBox ComboPath;
        private System.Windows.Forms.ComboBox ComboDim;
        private System.Windows.Forms.ComboBox ComboTrail;
        private System.Windows.Forms.TextBox TextBoxTrailConstant;
        private System.Windows.Forms.Button ButtonCustomPath;
        private System.Windows.Forms.Button ButtonCustomTrail;
        protected System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.Button ButtonRemove;
    }
}
