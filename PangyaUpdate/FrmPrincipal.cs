﻿using PangyaUpdate.Tools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PangyaUpdate.Patcher;
namespace PangyaUpdate
{
    /// <summary>
    /// criado por LuisMK :D
    /// update 09 04 2023
    /// codigo foi original nao existe :D
    /// </summary>
    public partial class FrmMain : Form
    {
        public PangyaReg reg;

        public FrmMain()
        {
            InitializeComponent();
            BarUpdate.SetState(1);
            BarProcess.SetState(2);
            // Inicialize a instância de RecursiveFileProcessor
            UpdateUnit = new UpdateUnit();
            // Adicione o manipulador de eventos OnPatchProgress
            UpdateUnit. OnProgressEvent += UpdateUnit.OnPatchProgress;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //carregar as configurações aqui:
            this.BarUpdate.Value = 0;
            this.BarProcess.Value = 0;
            CheckForIllegalCrossThreadCalls = false;
            this.lbProcessDesc.Text = "";
            SetToolTips();
            reg = new PangyaReg();
            if (reg.ExistRegMain()) 
                lblPatchVer.Text = $"Ver ({reg.getMainValue("Ver")})";

            this.Banner.Url = new System.Uri(UpdateUnit.Notice, System.UriKind.Absolute);
            BtnAbrirProjectG.Enabled = false;
            BtnResetPatch.Enabled = false;
            timer2.Start();
        }

        private void BtnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();//sai somente
        }

        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var X = this.Left - MousePosition.X;
            var Y = this.Top - MousePosition.Y;
        }
        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle clickableArea = new Rectangle(100, 100, 200, 200); // Define as coordenadas e dimensões da área clicável

            if (clickableArea.Contains(e.Location))
            {
                Cursor = Cursors.Hand; // Altera o cursor para a mão (indicando que a área é clicável)
            }
            else
            {
                Cursor = Cursors.Default; // Restaura o cursor padrão
            }
        }


        private void BtnResetPatch_Click(object sender, EventArgs e)
        {
            // Restart and run as admin
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas",
                Arguments = "restart"
            };
            Process.Start(startInfo);
            Application.Exit();
        }
        //aqqui eu abro o app
        private void BtnAbrirProjectG_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("ProjectG.exe").Length == 0)
            {
                Process process;
                process = new Process();
                process.StartInfo.FileName = ("ProjectG.exe");
                Environment.SetEnvironmentVariable("PANGYA_ARG", "{E69B65A2-7A7E-4977-85E5-B19516D885CB}");
                process.Start();
                timer1.Start();
            }
            else
            {
                MessageBox.Show("O programa já está em execução!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
        //aqui e options(basei no US)
        private void BtnOptions_Click(object sender, EventArgs e)
        {
            new FormOptions().ShowDialog();
        }

        private void BtnSafeMode_Click(object sender, EventArgs e)
        {
            //aqui abrir em modo safe(como eu faço isso?)
        }
        //usando umas referencias 
        private void timer1_Tick(object sender, EventArgs e)
        {
            var IsRun = Process.GetProcessesByName("ProjectG").Length > 0;
            if (IsRun)
            {
                //for ProjectG JP is 983.00 :D
                //edit memory fast ;)
                var mem = new PangyaAPI.Memory.MemoryS8();
                while (IsRun)
                {
                    var ver = mem.ReadString("ProjectG.exe+8DD13C", "");//version 
                    var packet_ver_key = mem.ReadString("ProjectG.exe+8DD105", "");//packet version
                    var GuildEmblem = mem.ReadString("ProjectG.exe+911B80", "", 59);//guild emblem link
                    var SelfDesign = mem.ReadString("ProjectG.exe+911B10", "", 58);//self design link
                }
            }
        }
        //chama o bug report :D
        private void BtnReport_Click(object sender, EventArgs e)
        {
            //enviar o dump log pro site
            //reportar bug ou erro aqui
            if (MessageBox.Show("No seguinte caso: " +
                "\n\n* Não é possível jogar " +
                "\n* Ocorre um erro no dispositivo " +
                "\n* Não mostrado na tela " +
                "\n* Congelou ou reiniciou após o carregamento do jogo" +
                "\n\nisso é para você quando as informações do seu sistema são transferidas para nossa equipe de desenvolvimento para analisar os problemas e resolvê-los. Suas informações pessoais que excluem esta análise nunca seriam transferidas para o servidor nem para qualquer uso de marketing." +
                "\n\nNo entanto, você deve colocar seu ID e e-mail que o verifica. " +
                "\nSe você não concorda com o acima, escolha [NÃO]." +
                "\n\nVocê gravaria as informações atuais do sistema(gráfico, som, CPU) para o servidor?", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.None) == DialogResult.Yes)
            {
                new FrmReport().Show();
            }
        }
        private void FrmMain_Shown(object sender, EventArgs e)
        {

            if (File.Exists("exception.log") && (File.Exists("stack.log")))
            {
                //enviar pro site aqui
                if (MessageBox.Show("Um log foi criado quando o cliente foi encerrado de forma incomum. " +
                    "\nGostaria de enviar o log para a equipe de desenvolvimento? " +
                    "\nLogs serão usados ​​para ajudar a tornar Pangya um jogo melhor. " +
                    "\n(Observe que apenas informações sobre erros ocorridos e informações básicas do sistema serão enviadas com log. " +
                    "\nEle não contém nenhuma informação pessoal. Ele será usado apenas para corrigir bugs e nada mais.)", "PangyaUpdate", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    //aqui e pra enviar o log ne(quando confirma), quando cancela ele exclui(eu acho)
                }
            }
            //Download();//aqui inicia o dl(versao nova)
        }

        private void _MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Active = false;
        }
        private void SetToolTips()
        {
            ToolTip tooltip = new ToolTip();

            tooltip.SetToolTip(BtnReport, "Clique aqui para reportar um bug");
            tooltip.SetToolTip(BtnSafeMode, "Clique aqui para iniciar em modo de segurança");
            tooltip.SetToolTip(BtnAbrirProjectG, "Clique aqui para abrir o ProjectG");
            tooltip.SetToolTip(BtnSair, "Clique aqui para sair do aplicativo");
            tooltip.SetToolTip(BtnResetPatch, "Clique aqui para resetar o patch");

            // Personalize o comportamento e a aparência do tooltip, se necessário

            // Exemplo de personalização:
            tooltip.AutoPopDelay = 5000; // Tempo em milissegundos que o tooltip é exibido
            tooltip.InitialDelay = 1000; // Tempo em milissegundos antes de exibir o tooltip
            tooltip.ReshowDelay = 500;   // Tempo em milissegundos antes de exibir novamente o tooltip
            tooltip.BackColor = Color.Yellow; // Cor de fundo do tooltip
            tooltip.ForeColor = Color.Black;  // Cor do texto do tooltip
        }

        //criar local clickavel :D
        private void FrmMain_MouseHover(object sender, EventArgs e)
        {
            Debug.WriteLine("size=> {0};{1}", MousePosition.X, MousePosition.Y);
            if (MousePosition.X == 436)
            {
                if (MousePosition.Y > 339)
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnPaint(object sender, PaintEventArgs e)
        {
            Button button = (Button)sender;
            GraphicsPath path = new GraphicsPath();
            int borderRadius = 8; // Valor para controlar o raio dos cantos arredondados

            // Cria um retângulo com os mesmos limites do botão
            RectangleF rectangle = new RectangleF(0, 0, button.Width, button.Height);

            // Adiciona um arco para cada canto do retângulo
            path.AddArc(rectangle.X, rectangle.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rectangle.X, rectangle.Height - borderRadius, borderRadius, borderRadius, 90, 90);

            // Cria uma região a partir do caminho para definir a forma do botão
            button.Region = new Region(path);
            path = new GraphicsPath();
            borderRadius = 8; // Valor para controlar o raio dos cantos arredondados

            // Cria um retângulo com os mesmos limites do formulário
            rectangle = new Rectangle(0, 0, Banner.Width, Banner.Height);

            // Adiciona um arco para cada canto do retângulo
            path.AddArc(rectangle.X, rectangle.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rectangle.X, rectangle.Height - borderRadius, borderRadius, borderRadius, 90, 90);
            Banner.Region = new Region(path);
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            int borderRadius = 10; // Valor para controlar o raio dos cantos arredondados

            // Cria um retângulo com os mesmos limites do formulário
            Rectangle rectangle = new Rectangle(0, 0, Width, Height);

            // Adiciona um arco para cada canto do retângulo
            path.AddArc(rectangle.X, rectangle.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rectangle.Width - borderRadius, rectangle.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rectangle.X, rectangle.Height - borderRadius, borderRadius, borderRadius, 90, 90);

            // Cria uma região a partir do caminho para definir a forma do formulário
            Region = new Region(path);
        }
        protected void BarOnPaint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            Graphics g = e.Graphics;
            var a = (ProgressBar)sender;
            GraphicsPath path = new GraphicsPath();
            int borderRadius = 8; // Valor para controlar o raio dos cantos arredondados

            path.AddArc(rect.X, rect.Y, borderRadius, borderRadius, 180, 90);
            path.AddArc(rect.Width - borderRadius, rect.Y, borderRadius, borderRadius, 270, 90);
            path.AddArc(rect.Width - borderRadius, rect.Height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(rect.X, rect.Height - borderRadius, borderRadius, borderRadius, 90, 90);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            if (a.Value > 0)
            {
                Rectangle clipRect = new Rectangle(rect.X, rect.Y, (int)(rect.Width * ((double)a.Value / a.Maximum)) - 1, rect.Height);
                Region region = new Region(path);
                g.Clip = region;
                g.FillRectangle(new SolidBrush(ForeColor), clipRect);
            }

            g.ResetClip();

            g.DrawPath(new Pen(ForeColor), path);
            g.Dispose();
        }

        private void TimeUpdate_Tick(object sender, EventArgs e)
        {
            lblPatchVer.Visible = true;
            UpdateUnit.SetValues(BarProcess, BarUpdate, lbProcessDesc, lblPatchVer, BtnAbrirProjectG, BtnResetPatch);
            UpdateUnit.DownloadUpdateList();
            timer2.Stop();
        }

        private void BtnAbrirProjectG_VisibleChanged(object sender, EventArgs e)
        {
            if (!BtnAbrirProjectG.Enabled) //mudar a cor do botao
            {
                BtnAbrirProjectG.BackgroundImage = Properties.Resources.BtnJogar_Block;
            }
        }
    }
}
