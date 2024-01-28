using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PangyaUpdate.Tools
{
    #region Arredondamento
    public static class Utils
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            var SizeSuffixes = new string[9] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }
            if (value < 0)
            {
                return "-" + Utils.SizeSuffix(-value);
            }
            if (value == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }
            int num;
            num = (int)Math.Log(value, 1024.0);
            decimal num2;
            num2 = (decimal)value / (decimal)(1L << num * 10);
            if (Math.Round(num2, decimalPlaces) >= 1000m)
            {
                num++;
                num2 /= 1024m;
            }
            return string.Format("{0:n" + decimalPlaces + "} {1}", num2, SizeSuffixes[num]);
        }
        public static GraphicsPath BorderRadius(Rectangle pRect, int pCanto, bool pTopo, bool pBase)
        {
            GraphicsPath gp = new GraphicsPath();

            if (pTopo)
            {
                gp.AddArc(pRect.X - 1, pRect.Y - 1, pCanto, pCanto, 180, 90);
                gp.AddArc(pRect.X + pRect.Width - pCanto, pRect.Y - 1, pCanto, pCanto, 270, 90);
            }
            else
            {
                // Se não arredondar o topo, adiciona as linhas para poder fechar o retangulo junto com
                // a base arredondada
                gp.AddLine(pRect.X - 1, pRect.Y - 1, pRect.X + pRect.Width, pRect.Y - 1);
            }

            if (pBase)
            {
                gp.AddArc(pRect.X + pRect.Width - pCanto, pRect.Y + pRect.Height - pCanto, pCanto, pCanto, 0, 90);
                gp.AddArc(pRect.X - 1, pRect.Y + pRect.Height - pCanto, pCanto, pCanto, 90, 90);
            }
            else
            {
                // Se não arredondar a base, adiciona as linhas para poder fechar o retangulo junto com
                // o topo arredondado. Adiciona da direita para esquerda pois é na ordem contrária que 
                // foi adicionado os arcos do topo. E pra fechar o retangulo tem que desenhar na ordem :
                //  1 - Canto Superior Esquerdo
                //  2 - Canto Superior Direito
                //  3 - Canto Inferior Direito 
                //  4 - Canto Inferior Esquerdo.
                gp.AddLine(pRect.X + pRect.Width, pRect.Y + pRect.Height, pRect.X - 1, pRect.Y + pRect.Height);
            }

            return gp;
        }
        private static GraphicsPath SuArredondaRect(Rectangle pRect, int pCanto, bool pTopo, bool pBase)
        {
            GraphicsPath gp = new GraphicsPath();

            if (pTopo)
            {
                gp.AddArc(pRect.X - 1, pRect.Y - 1, pCanto, pCanto, 180, 90);
                gp.AddArc(pRect.X + pRect.Width - pCanto, pRect.Y - 1, pCanto, pCanto, 270, 90);
            }
            else
            {
                // Se não arredondar o topo, adiciona as linhas para poder fechar o retangulo junto com
                // a base arredondada
                gp.AddLine(pRect.X - 1, pRect.Y - 1, pRect.X + pRect.Width, pRect.Y - 1);
            }

            if (pBase)
            {
                gp.AddArc(pRect.X + pRect.Width - pCanto, pRect.Y + pRect.Height - pCanto, pCanto, pCanto, 0, 90);
                gp.AddArc(pRect.X - 1, pRect.Y + pRect.Height - pCanto, pCanto, pCanto, 90, 90);
            }
            else
            {
                // Se não arredondar a base, adiciona as linhas para poder fechar o retangulo junto com
                // o topo arredondado. Adiciona da direita para esquerda pois é na ordem contrária que 
                // foi adicionado os arcos do topo. E pra fechar o retangulo tem que desenhar na ordem :
                //  1 - Canto Superior Esquerdo
                //  2 - Canto Superior Direito
                //  3 - Canto Inferior Direito 
                //  4 - Canto Inferior Esquerdo.
                gp.AddLine(pRect.X + pRect.Width, pRect.Y + pRect.Height, pRect.X - 1, pRect.Y + pRect.Height);
            }

            return gp;
        }

        /// <summary>
        /// Arredonda os cantos do Formulário passado como parâmetro (pFormulario).
        /// </summary>
        /// <param name="pCanto">Tamanho arredondamento do canto (Altura x Largura) em pixels.</param>
        /// <param name="pTopo">Indica se faz o arredondamento dos cantos superiores.</param>
        /// <param name="pBase">Indica se faz o arredondamento dos cantos inferiores.</param>
        public static void Arredonda(Form pFormulario, int pCanto, bool pTopo, bool pBase)
        {
            // pCanto -> Tamanho do Canto
            // pTopo -> Arredonda o Topo
            // pBase -> Arredonda a Base
            Rectangle r = new Rectangle();
            r = pFormulario.ClientRectangle;

            pFormulario.Region = new System.Drawing.Region(SuArredondaRect(r, pCanto, pTopo, pBase));
        }

        /// <summary>
        /// Arredonda os cantos do PictureBox passado como parâmetro (pPicture).
        /// </summary>
        /// <param name="pCanto">Tamanho arredondamento do canto (Altura x Largura) em pixels.</param>
        /// <param name="pTopo">Indica se faz o arredondamento dos cantos superiores.</param>
        /// <param name="pBase">Indica se faz o arredondamento dos cantos inferiores.</param>
        public static void Arredonda(PictureBox pPicture, int pCanto, bool pTopo, bool pBase)
        {
            // pCanto -> Tamanho do Canto
            // pTopo -> Arredonda o Topo
            // pBase -> Arredonda a Base
            Rectangle r = new Rectangle();
            r = pPicture.ClientRectangle;

            pPicture.Region = new System.Drawing.Region(SuArredondaRect(r, pCanto, pTopo, pBase));
        }

        /// <summary>
        /// Arredonda os cantos do Painel passado como parâmetro (pPanel).
        /// </summary>
        /// <param name="pCanto">Tamanho arredondamento do canto (Altura x Largura) em pixels.</param>
        /// <param name="pTopo">Indica se faz o arredondamento dos cantos superiores.</param>
        /// <param name="pBase">Indica se faz o arredondamento dos cantos inferiores.</param>
        public static void Arredonda(Panel pPainel, int pCanto, bool pTopo, bool pBase)
        {
            // pCanto -> Tamanho do Canto
            // pTopo -> Arredonda o Topo
            // pBase -> Arredonda a Base
            Rectangle r = new Rectangle();
            r = pPainel.ClientRectangle;

            pPainel.Region = new System.Drawing.Region(SuArredondaRect(r, pCanto, pTopo, pBase));
        }

        /// <summary>
        /// Arredonda os cantos do Botão passado como parâmetro (pButton).
        /// </summary>
        /// <param name="pCanto">Tamanho arredondamento do canto (Altura x Largura) em pixels.</param>
        /// <param name="pTopo">Indica se faz o arredondamento dos cantos superiores.</param>
        /// <param name="pBase">Indica se faz o arredondamento dos cantos inferiores.</param>
        public static void Arredonda(Button pButton, int pCanto, bool pTopo, bool pBase)
        {
            // pCanto -> Tamanho do Canto
            // pTopo -> Arredonda o Topo
            // pBase -> Arredonda a Base
            Rectangle r = new Rectangle();
            r = pButton.ClientRectangle;

            pButton.Region = new Region(SuArredondaRect(r, pCanto, pTopo, pBase));
        }

        public static void ArredondaCantosdoForm(ref Button pButton)
        {

            GraphicsPath PastaGrafica = new GraphicsPath();
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, 1, pButton.Size.Width, pButton.Size.Height));

            //Arredondar canto superior esquerdo        
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, 1, 10, 10));
            PastaGrafica.AddPie(1, 1, 20, 20, 180, 90);

            //Arredondar canto superior direito
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(pButton.Width - 12, 1, 12, 13));
            PastaGrafica.AddPie(pButton.Width - 24, 1, 24, 26, 270, 90);

            //Arredondar canto inferior esquerdo
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, pButton.Height - 10, 10, 10));
            PastaGrafica.AddPie(1, pButton.Height - 20, 20, 20, 90, 90);

            //Arredondar canto inferior direito
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(pButton.Width - 12, pButton.Height - 13, 13, 13));
            PastaGrafica.AddPie(pButton.Width - 24, pButton.Height - 26, 24, 26, 0, 90);

            PastaGrafica.SetMarkers();
            pButton.Region = new Region(PastaGrafica);
        }
        public static void ArredondaCantosdoForm(PictureBox pButton)
        {

            GraphicsPath PastaGrafica = new GraphicsPath();
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, 1, pButton.Size.Width, pButton.Size.Height));

            //Arredondar canto superior esquerdo        
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, 1, 10, 10));
            PastaGrafica.AddPie(1, 1, 20, 20, 180, 90);

            //Arredondar canto superior direito
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(pButton.Width - 12, 1, 12, 13));
            PastaGrafica.AddPie(pButton.Width - 24, 1, 24, 26, 270, 90);

            //Arredondar canto inferior esquerdo
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(1, pButton.Height - 10, 10, 10));
            PastaGrafica.AddPie(1, pButton.Height - 20, 20, 20, 90, 90);

            //Arredondar canto inferior direito
            PastaGrafica.AddRectangle(new System.Drawing.Rectangle(pButton.Width - 12, pButton.Height - 13, 13, 13));
            PastaGrafica.AddPie(pButton.Width - 24, pButton.Height - 26, 24, 26, 0, 90);

            PastaGrafica.SetMarkers();
            pButton.Region = new Region(PastaGrafica);
        }

        /// <summary>
        /// gera um data nova
        /// </summary>
        /// <param name="old">data antiga</param>
        /// <param name="hour">adiciona o tempo</param>
        /// <returns></returns>
        public static DateTime GetDate(this DateTime old, int hour)
        {
            var now = new DateTime(
                old.Year,
                old.Month,
                old.Day,
                old.Hour - hour,
                old.Minute,
                old.Second,
                old.Millisecond
            );
            return now;
        }
    }

    #endregion
}
