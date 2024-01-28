using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PangyaUpdate
{
    public partial class FormOptions : Form
    {
        string vScreenWidth = "800";
        string vScreenHeight = "600";
        public FormOptions()
        {
            InitializeComponent();
           
            //verificar antes se tem as chaves do app.exe, se tem ou nao,
            string subkey = "SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option";
            var Opt = Registry.LocalMachine.OpenSubKey(subkey, true);
            if (Opt == null)//aqui eu seto as opcoes
            {
                Opt = Registry.LocalMachine.CreateSubKey("SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option");
                Opt.SetValue("vWindowMode", "0");
                Opt.SetValue("vFillMode", "0");
                Opt.SetValue("vWideMode", "0");
                Opt.SetValue("vOverall", "-1");
                Opt.SetValue("vScreenWidth", "1024");
                Opt.SetValue("vScreenHeight", "768");
                Opt.SetValue("vScreenColor", "32");
                Opt.SetValue("vTnLMode", "1");
                Opt.SetValue("vEffectLevel", "1");
                Opt.SetValue("vProjectionShadow", "1");
                Opt.SetValue("vShadowmap", "1");
                Opt.SetValue("vReflection", "0");
                Opt.SetValue("vLod", "0");
                Opt.SetValue("vNpc", "1");
                Opt.SetValue("vCoordX", "0");
                Opt.SetValue("vCoordY", "0");
                Opt.SetValue("vGamma", "1.3");
                Opt.SetValue("viHdr", "0");
                Opt.SetValue("viHdrStarEffect", "0");
                Opt.SetValue("viDof", "0");
                Opt.SetValue("viRimLight", "0");
                Opt.SetValue("viAmbientControl", "0");
                Opt.SetValue("viFXAA", "0");
                Opt.SetValue("vfBright", "3.00");
                Opt.SetValue("viShaderOnOff", "0");
                Opt.SetValue("aMssOn", "1");
                Opt.SetValue("aOverall", "0");
                Opt.SetValue("aMssFreq", "44100");
                Opt.SetValue("aMssBits", "16");
                Opt.SetValue("aMssChannels", "2");
                Opt.SetValue("aMssEnableHwSound", "1");
                Opt.SetValue("aMssBalance", "128");
                Opt.SetValue("aMssSpeaker", "0");
                Opt.SetValue("aSfxVolume", "1.00");
                Opt.SetValue("aBGMVolume", "1.00");
                Opt.SetValue("gMouseSensitivity", "1.00");
                Opt.SetValue("gWhisper", "1");
                Opt.SetValue("gInvitation", "1");
                Opt.SetValue("gFriendConfirm", "1");
                Opt.SetValue("gTransChatWin", "1");
                Opt.SetValue("gUiEffect", "1");
                Opt.SetValue("gLastLoginID", "");         //user id
                Opt.SetValue("gIdentity", "0"); //user uid
                Opt.SetValue("gExtendChatLine", "0");
                Opt.SetValue("gUserSort", "0");
                Opt.SetValue("gPPL_Enable", "1");
                Opt.SetValue("gPPL_Size", "0");
                Opt.SetValue("gPowerGauge", "1");
                Opt.SetValue("gFullyProportionalPowerGaugeSize", "0");
                Opt.SetValue("gCenteredPowerGauge", "0");
                Opt.SetValue("gCaptureLogo", "1");
                Opt.SetValue("gRestore", "0");
                Opt.SetValue("gChatWnd", "0");
                Opt.SetValue("gUnderTabBtn", "0");
                Opt.SetValue("gAvatarNewbie", "1");
                Opt.SetValue("gCaptureHideGUI", "0");
                Opt.SetValue("gCaptureHidePI", "0");
                Opt.SetValue("gTerrainTooltip", "1");
                Opt.SetValue("gCutinDisplay", "1");
                Opt.SetValue("gBindMouseCursor", "1");
                Opt.SetValue("mMacro1", "Eu Amo a Unogames (e7)");
                Opt.SetValue("mMacro2", "Nice to meet you.");
                Opt.SetValue("mMacro3", "Good Luck!");
                Opt.SetValue("mMacro4", "Eu (e64) Pangya!");
                Opt.SetValue("mMacro5", "Go for a Hole in One!");
                Opt.SetValue("mMacro6", "Nice Shot! (e19)");
                Opt.SetValue("mMacro7", "Very Close!");
                Opt.SetValue("mMacro8", "Nice Try.");
                Opt.SetValue("mSaveMacros", "1");
            }
            else
            {
                //aqui eu puxo
                vScreenWidth = Convert.ToString(Opt.GetValue("vScreenWidth"));
                vScreenHeight = Convert.ToString(Opt.GetValue("vScreenHeight"));
                cmbScreen.Text = vScreenWidth + " x " + vScreenHeight;
                rdWindows.Checked = !(Convert.ToString(Opt.GetValue("vWindowMode")) == "1" ? true: false);
                rdFull.Checked = (Convert.ToString(Opt.GetValue("vWindowMode")) == "0" ? true : false);
                ckSound.Checked = (Convert.ToString(Opt.GetValue("aMssEnableHwSound")) == "1" ? true : false);
                ckLod.Checked = (Convert.ToString(Opt.GetValue("vLod")) == "1" ? true : false);
                ckTnl.Checked = (Convert.ToString(Opt.GetValue("vTnLMode")) == "1" ? true : false);
                ckShader.Checked = (Convert.ToString(Opt.GetValue("viShaderOnOff")) == "1" ? true : false);
                //
                Opt.Close();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            //salvar as opcoes aqui
            string subkey = "SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option";
            var keys = Registry.LocalMachine.OpenSubKey(subkey, true);
            if (keys != null)
            {
                //sets aqui
                if (cmbScreen.SelectedIndex ==0)
                {
                    vScreenWidth = "800";
                    vScreenHeight = "600";
                }
                else if (cmbScreen.SelectedIndex == 1)
                {
                    vScreenWidth = "1024";
                    vScreenHeight = "768";
                }
                else if (cmbScreen.SelectedIndex == 2)
                {
                    vScreenWidth = "1280";
                    vScreenHeight = "960";
                }
                keys.SetValue("vScreenWidth", vScreenWidth.ToString());
                keys.SetValue("vScreenHeight", vScreenHeight.ToString());
                keys.SetValue("vWindowMode", Convert.ToInt32(rdWindows.Checked).ToString());
                keys.SetValue("aMssEnableHwSound", Convert.ToInt32(ckSound.Checked).ToString());
                keys.SetValue("vLod", Convert.ToInt32(ckLod.Checked).ToString());
                keys.SetValue("vTnLMode", Convert.ToInt32(ckTnl.Checked).ToString());
                keys.SetValue("viShaderOnOff", Convert.ToInt32(ckShader.Checked).ToString());
                keys.Close();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
