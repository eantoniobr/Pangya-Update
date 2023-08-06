using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PangyaUpdate
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //um pequeno codigo de seguranca
            var aProcName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;//pega o nome real !
                if (Process.GetProcessesByName("update.exe").Length == 0
               && Process.GetProcessesByName(aProcName).Length == 1)//aqui o processo esta rodando, entao o maximo deve 1
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());//roda
            }
            else //caso ele ja esteja iniciado !
            {
                MessageBox.Show("O programa já está em execução!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}
