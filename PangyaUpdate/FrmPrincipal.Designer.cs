using PangyaUpdate.Tools;
using System.Net.Sockets;
using System.Numerics;
using System.Windows.Forms;
using static PangyaUpdate.Patcher;

namespace PangyaUpdate
{
    partial class FrmMain
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.BarUpdate = new System.Windows.Forms.ProgressBar();
            this.BarProcess = new System.Windows.Forms.ProgressBar();
            this.BtnReport = new System.Windows.Forms.Button();
            this.Banner = new System.Windows.Forms.WebBrowser();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.BtnLogo = new System.Windows.Forms.Button();
            this.BtnResetPatch = new System.Windows.Forms.Button();
            this.BtnOptions = new System.Windows.Forms.Button();
            this.BtnSafeMode = new System.Windows.Forms.Button();
            this.BtnAbrirProjectG = new System.Windows.Forms.Button();
            this.BtnSair = new System.Windows.Forms.Button();
            this.lblPatchVer = new System.Windows.Forms.Label();
            this.lbProcessDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BarUpdate
            // 
            this.BarUpdate.BackColor = System.Drawing.Color.DarkOrange;
            this.BarUpdate.ForeColor = System.Drawing.Color.DarkOrange;
            this.BarUpdate.Location = new System.Drawing.Point(251, 436);
            this.BarUpdate.Maximum = 1000;
            this.BarUpdate.Name = "BarUpdate";
            this.BarUpdate.Size = new System.Drawing.Size(233, 15);
            this.BarUpdate.TabIndex = 0;
            this.BarUpdate.Paint += new System.Windows.Forms.PaintEventHandler(this.BarOnPaint);
            // 
            // BarProcess
            // 
            this.BarProcess.BackColor = System.Drawing.Color.Crimson;
            this.BarProcess.ForeColor = System.Drawing.Color.Crimson;
            this.BarProcess.Location = new System.Drawing.Point(251, 405);
            this.BarProcess.Maximum = 1000;
            this.BarProcess.Name = "BarProcess";
            this.BarProcess.Size = new System.Drawing.Size(233, 16);
            this.BarProcess.TabIndex = 1;
            this.BarProcess.Paint += new System.Windows.Forms.PaintEventHandler(this.BarOnPaint);
            // 
            // BtnReport
            // 
            this.BtnReport.BackColor = System.Drawing.Color.Transparent;
            this.BtnReport.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnReportar_OK;
            this.BtnReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnReport.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnReport.FlatAppearance.BorderSize = 0;
            this.BtnReport.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnReport.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnReport.Location = new System.Drawing.Point(121, 427);
            this.BtnReport.Name = "BtnReport";
            this.BtnReport.Size = new System.Drawing.Size(103, 28);
            this.BtnReport.TabIndex = 5;
            this.BtnReport.UseVisualStyleBackColor = false;
            this.BtnReport.Click += new System.EventHandler(this.BtnReport_Click);
            this.BtnReport.Paint += new System.Windows.Forms.PaintEventHandler(this.BtnPaint);
            this.BtnReport.MouseLeave += new System.EventHandler(this._MouseLeave);
            // 
            // Banner
            // 
            this.Banner.Location = new System.Drawing.Point(10, 35);
            this.Banner.MinimumSize = new System.Drawing.Size(20, 20);
            this.Banner.Name = "Banner";
            this.Banner.ScriptErrorsSuppressed = true;
            this.Banner.ScrollBarsEnabled = false;
            this.Banner.Size = new System.Drawing.Size(694, 338);
            this.Banner.TabIndex = 12;
            this.Banner.Url = new System.Uri("http://144.217.169.230/game", System.UriKind.Absolute);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(75, 223);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(0, 13);
            this.linkLabel1.TabIndex = 18;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 300;
            this.timer2.Tick += new System.EventHandler(this.TimeUpdate_Tick);
            // 
            // BtnLogo
            // 
            this.BtnLogo.BackColor = System.Drawing.Color.Transparent;
            this.BtnLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnLogo.BackgroundImage")));
            this.BtnLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnLogo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnLogo.FlatAppearance.BorderSize = 0;
            this.BtnLogo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnLogo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnLogo.Location = new System.Drawing.Point(61, 470);
            this.BtnLogo.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.BtnLogo.Name = "BtnLogo";
            this.BtnLogo.Size = new System.Drawing.Size(90, 43);
            this.BtnLogo.TabIndex = 20;
            this.BtnLogo.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.BtnLogo.UseVisualStyleBackColor = false;
            // 
            // BtnResetPatch
            // 
            this.BtnResetPatch.BackColor = System.Drawing.Color.Transparent;
            this.BtnResetPatch.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnReset_OK;
            this.BtnResetPatch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnResetPatch.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnResetPatch.FlatAppearance.BorderSize = 0;
            this.BtnResetPatch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnResetPatch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnResetPatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnResetPatch.Location = new System.Drawing.Point(121, 393);
            this.BtnResetPatch.Name = "BtnResetPatch";
            this.BtnResetPatch.Size = new System.Drawing.Size(103, 28);
            this.BtnResetPatch.TabIndex = 21;
            this.BtnResetPatch.UseVisualStyleBackColor = false;
            this.BtnResetPatch.Click += new System.EventHandler(this.BtnResetPatch_Click);
            // 
            // BtnOptions
            // 
            this.BtnOptions.BackColor = System.Drawing.Color.Transparent;
            this.BtnOptions.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnOpt_OK;
            this.BtnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnOptions.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnOptions.FlatAppearance.BorderSize = 0;
            this.BtnOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnOptions.Location = new System.Drawing.Point(12, 393);
            this.BtnOptions.Name = "BtnOptions";
            this.BtnOptions.Size = new System.Drawing.Size(103, 28);
            this.BtnOptions.TabIndex = 23;
            this.BtnOptions.UseVisualStyleBackColor = false;
            this.BtnOptions.Click += new System.EventHandler(this.BtnOptions_Click);
            // 
            // BtnSafeMode
            // 
            this.BtnSafeMode.BackColor = System.Drawing.Color.Transparent;
            this.BtnSafeMode.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnSeguro_OK;
            this.BtnSafeMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnSafeMode.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnSafeMode.FlatAppearance.BorderSize = 0;
            this.BtnSafeMode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnSafeMode.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnSafeMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSafeMode.Location = new System.Drawing.Point(12, 427);
            this.BtnSafeMode.Name = "BtnSafeMode";
            this.BtnSafeMode.Size = new System.Drawing.Size(103, 28);
            this.BtnSafeMode.TabIndex = 22;
            this.BtnSafeMode.UseVisualStyleBackColor = false;
            this.BtnSafeMode.Click += new System.EventHandler(this.BtnSafeMode_Click);
            // 
            // BtnAbrirProjectG
            // 
            this.BtnAbrirProjectG.BackColor = System.Drawing.Color.Transparent;
            this.BtnAbrirProjectG.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnJogar_OK;
            this.BtnAbrirProjectG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnAbrirProjectG.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnAbrirProjectG.FlatAppearance.BorderSize = 0;
            this.BtnAbrirProjectG.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnAbrirProjectG.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnAbrirProjectG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAbrirProjectG.Location = new System.Drawing.Point(501, 393);
            this.BtnAbrirProjectG.Name = "BtnAbrirProjectG";
            this.BtnAbrirProjectG.Size = new System.Drawing.Size(98, 62);
            this.BtnAbrirProjectG.TabIndex = 24;
            this.BtnAbrirProjectG.UseVisualStyleBackColor = false;
            this.BtnAbrirProjectG.VisibleChanged += new System.EventHandler(this.BtnAbrirProjectG_VisibleChanged);
            this.BtnAbrirProjectG.Click += new System.EventHandler(this.BtnAbrirProjectG_Click);
            // 
            // BtnSair
            // 
            this.BtnSair.BackColor = System.Drawing.Color.Transparent;
            this.BtnSair.BackgroundImage = global::PangyaUpdate.Properties.Resources.BtnSair_OK;
            this.BtnSair.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnSair.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnSair.FlatAppearance.BorderSize = 0;
            this.BtnSair.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnSair.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSair.Location = new System.Drawing.Point(605, 393);
            this.BtnSair.Name = "BtnSair";
            this.BtnSair.Size = new System.Drawing.Size(98, 62);
            this.BtnSair.TabIndex = 25;
            this.BtnSair.UseVisualStyleBackColor = false;
            this.BtnSair.Click += new System.EventHandler(this.BtnSair_Click);
            // 
            // lblPatchVer
            // 
            this.lblPatchVer.AutoSize = true;
            this.lblPatchVer.BackColor = System.Drawing.Color.Transparent;
            this.lblPatchVer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPatchVer.Font = new System.Drawing.Font("Arial Black", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatchVer.ForeColor = System.Drawing.Color.Black;
            this.lblPatchVer.Location = new System.Drawing.Point(169, 13);
            this.lblPatchVer.Name = "lblPatchVer";
            this.lblPatchVer.Size = new System.Drawing.Size(51, 11);
            this.lblPatchVer.TabIndex = 26;
            this.lblPatchVer.Text = "Ver (None)";
            this.lblPatchVer.Visible = false;
            // 
            // lbProcessDesc
            // 
            this.lbProcessDesc.AutoSize = true;
            this.lbProcessDesc.BackColor = System.Drawing.Color.Transparent;
            this.lbProcessDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcessDesc.ForeColor = System.Drawing.Color.Blue;
            this.lbProcessDesc.Location = new System.Drawing.Point(336, 389);
            this.lbProcessDesc.Name = "lbProcessDesc";
            this.lbProcessDesc.Size = new System.Drawing.Size(20, 13);
            this.lbProcessDesc.TabIndex = 19;
            this.lbProcessDesc.Text = "file";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PangyaUpdate.Properties.Resources.Bitmap140;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CancelButton = this.BtnSair;
            this.ClientSize = new System.Drawing.Size(715, 522);
            this.Controls.Add(this.lblPatchVer);
            this.Controls.Add(this.BtnSair);
            this.Controls.Add(this.BtnAbrirProjectG);
            this.Controls.Add(this.BtnOptions);
            this.Controls.Add(this.BtnSafeMode);
            this.Controls.Add(this.BtnResetPatch);
            this.Controls.Add(this.BtnLogo);
            this.Controls.Add(this.lbProcessDesc);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.Banner);
            this.Controls.Add(this.BtnReport);
            this.Controls.Add(this.BarProcess);
            this.Controls.Add(this.BarUpdate);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Pangya Launcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmMain_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmMain_MouseDown);
            this.MouseHover += new System.EventHandler(this.FrmMain_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmMain_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ProgressBar BarUpdate;
        public ProgressBar BarProcess;
        public Button BtnReport;
        public WebBrowser Banner;
        public LinkLabel linkLabel1;
        public Timer timer1;
        public ToolTip toolTip1;
        public Timer timer2;
        public Button BtnLogo;
        public Button BtnResetPatch;
        public Button BtnOptions;
        public Button BtnSafeMode;
        public Button BtnAbrirProjectG;
        public Button BtnSair;
        public Label lblPatchVer;
        private UpdateUnit UpdateUnit = new UpdateUnit();
        public Label lbProcessDesc;
    }
}

