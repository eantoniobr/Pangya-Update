namespace PangyaUpdate
{
    partial class FormOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdFull = new System.Windows.Forms.RadioButton();
            this.rdWindows = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbScreen = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ckShader = new System.Windows.Forms.CheckBox();
            this.ckTnl = new System.Windows.Forms.CheckBox();
            this.ckLod = new System.Windows.Forms.CheckBox();
            this.ckSound = new System.Windows.Forms.CheckBox();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdFull);
            this.groupBox1.Controls.Add(this.rdWindows);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(6, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 44);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Modo de Exibição";
            // 
            // rdFull
            // 
            this.rdFull.AutoSize = true;
            this.rdFull.Location = new System.Drawing.Point(142, 19);
            this.rdFull.Name = "rdFull";
            this.rdFull.Size = new System.Drawing.Size(76, 17);
            this.rdFull.TabIndex = 1;
            this.rdFull.TabStop = true;
            this.rdFull.Text = "Tela Cheia";
            this.rdFull.UseVisualStyleBackColor = true;
            // 
            // rdWindows
            // 
            this.rdWindows.AutoSize = true;
            this.rdWindows.Location = new System.Drawing.Point(6, 19);
            this.rdWindows.Name = "rdWindows";
            this.rdWindows.Size = new System.Drawing.Size(56, 17);
            this.rdWindows.TabIndex = 0;
            this.rdWindows.TabStop = true;
            this.rdWindows.Text = "Janela";
            this.rdWindows.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbScreen);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(7, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 56);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tamanho da Tela";
            // 
            // cmbScreen
            // 
            this.cmbScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbScreen.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbScreen.FormattingEnabled = true;
            this.cmbScreen.Items.AddRange(new object[] {
            "800 x 600",
            "1024 x 768",
            "1280 x 960"});
            this.cmbScreen.Location = new System.Drawing.Point(7, 23);
            this.cmbScreen.Name = "cmbScreen";
            this.cmbScreen.Size = new System.Drawing.Size(206, 23);
            this.cmbScreen.TabIndex = 0;
            this.cmbScreen.Text = "800 x 600";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ckShader);
            this.groupBox3.Controls.Add(this.ckTnl);
            this.groupBox3.Controls.Add(this.ckLod);
            this.groupBox3.Controls.Add(this.ckSound);
            this.groupBox3.Location = new System.Drawing.Point(7, 126);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(222, 110);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "H/W Opcoes de Problemas";
            // 
            // ckShader
            // 
            this.ckShader.AutoSize = true;
            this.ckShader.Location = new System.Drawing.Point(7, 87);
            this.ckShader.Name = "ckShader";
            this.ckShader.Size = new System.Drawing.Size(101, 17);
            this.ckShader.TabIndex = 3;
            this.ckShader.Text = "Shader Habilitar";
            this.ckShader.UseVisualStyleBackColor = true;
            // 
            // ckTnl
            // 
            this.ckTnl.AutoSize = true;
            this.ckTnl.Location = new System.Drawing.Point(7, 65);
            this.ckTnl.Name = "ckTnl";
            this.ckTnl.Size = new System.Drawing.Size(113, 17);
            this.ckTnl.TabIndex = 2;
            this.ckTnl.Text = "H/W TnL Habilitar";
            this.ckTnl.UseVisualStyleBackColor = true;
            // 
            // ckLod
            // 
            this.ckLod.AutoSize = true;
            this.ckLod.Location = new System.Drawing.Point(7, 42);
            this.ckLod.Name = "ckLod";
            this.ckLod.Size = new System.Drawing.Size(89, 17);
            this.ckLod.TabIndex = 1;
            this.ckLod.Text = "LOD Habilitar";
            this.ckLod.UseVisualStyleBackColor = true;
            // 
            // ckSound
            // 
            this.ckSound.AutoSize = true;
            this.ckSound.Location = new System.Drawing.Point(7, 19);
            this.ckSound.Name = "ckSound";
            this.ckSound.Size = new System.Drawing.Size(141, 17);
            this.ckSound.TabIndex = 0;
            this.ckSound.Text = "Usar Som do Dispositivo";
            this.ckSound.UseVisualStyleBackColor = true;
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(21, 242);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(88, 27);
            this.BtnOK.TabIndex = 3;
            this.BtnOK.Text = "&OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(132, 242);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(88, 27);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "&Fechar";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // FormOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 281);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Opcoes de Janela";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdFull;
        private System.Windows.Forms.RadioButton rdWindows;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbScreen;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox ckShader;
        private System.Windows.Forms.CheckBox ckTnl;
        private System.Windows.Forms.CheckBox ckLod;
        private System.Windows.Forms.CheckBox ckSound;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnClose;
    }
}