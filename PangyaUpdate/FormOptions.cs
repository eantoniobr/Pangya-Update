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
            var keys = Registry.LocalMachine.OpenSubKey(subkey, true);
            if (keys == null)//aqui eu seto as opcoes
            {
//                "vWindowMode" , "0"
//"vFillMode" , "0"
//"vWideMode" , "0"
//"vOverall" , "-1"
//"vScreenWidth" , "1024"
//"vScreenHeight" , "768"
//"vScreenColor" , "32"
//"vTnLMode" , "1"
//"vEffectLevel" , "1"
//"vProjectionShadow" , "1"
//"vShadowmap" , "1"
//"vReflection" , "0"
//"vLod" , "0"
//"vNpc" , "1"
//"vCoordX" , "0"
//"vCoordY" , "0"
//"vGamma" , "1.3"
//"viHdr" , "0"
//"viHdrStarEffect" , "0"
//"viDof" , "0"
//"viRimLight" , "0"
//"viAmbientControl" , "0"
//"viFXAA" , "0"
//"vfBright" , "3.00"
//"viShaderOnOff" , "0"
//"aMssOn" , "1"
//"aOverall" , "0"
//"aMssFreq" , "44100"
//"aMssBits" , "16"
//"aMssChannels" , "2"
//"aMssEnableHwSound" , "1"
//"aMssBalance" , "128"
//"aMssSpeaker" , "0"
//"aSfxVolume" , "1.00"
//"aBGMVolume" , "1.00"
//"gMouseSensitivity" , "1.00"
//"gWhisper" , "1"
//"gInvitation" , "1"
//"gFriendConfirm" , "1"
//"gTransChatWin" , "1"
//"gUiEffect" , "1"
//"gLastLoginID" , ""
//"gIdentity" , "0"
//"gExtendChatLine" , "0"
//"gUserSort" , "0"
//"gPPL_Enable" , "1"
//"gPPL_Size" , "0"
//"gPowerGauge" , "1"
//"gFullyProportionalPowerGaugeSize" , "0"
//"gCenteredPowerGauge" , "0"
//"gCaptureLogo" , "1"
//"gRestore" , "0"
//"gChatWnd" , "0"
//"gUnderTabBtn" , "0"
//"gAvatarNewbie" , "1"
//"gCaptureHideGUI" , "0"
//"gCaptureHidePI" , "0"
//"gTerrainTooltip" , "1"
//"gCutinDisplay" , "1"
//"gBindMouseCursor" , "1"
//"mMacro1" , "Hello! "
//"mMacro2" , "Nice to meet you."
//"mMacro3" , "Good Luck!"
//"mMacro4" , "Eu (e64) Pangya!"
//"mMacro5" , "Go for a Hole in One!"
//"mMacro6" , "Nice Shot! (e19)"
//"mMacro7" , "Very Close!"
//"mMacro8" , "Nice Try."
//"mSaveMacros" , "1"
            }
            else
            {
                //aqui eu puxo
                vScreenWidth = Convert.ToString(keys.GetValue("vScreenWidth"));
                vScreenHeight = Convert.ToString(keys.GetValue("vScreenHeight"));
                var screen = vScreenWidth + " x " + vScreenHeight;
                rdWindows.Checked = Convert.ToString(keys.GetValue("vWindowMode")) == "1" ? true: false;
                rdFull.Checked = Convert.ToString(keys.GetValue("vWindowMode")) == "0" ? true : false;
                ckSound.Checked = Convert.ToString(keys.GetValue("aMssEnableHwSound")) == "1" ? true : false;
                ckLod.Checked = Convert.ToString(keys.GetValue("vLod")) == "1" ? true : false;
                ckTnl.Checked = Convert.ToString(keys.GetValue("vTnLMode")) == "1" ? true : false;
                ckShader.Checked = Convert.ToString(keys.GetValue("viShaderOnOff")) == "1" ? true : false;
                //
                cmbScreen.Text = screen;
                keys.Close();
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
                keys.SetValue("vScreenWidth", vScreenWidth.ToString());
                keys.SetValue("vScreenHeight", vScreenHeight.ToString());
                keys.SetValue("vWindowMode", rdWindows.Checked.ToString());
                keys.SetValue("aMssEnableHwSound", ckSound.Checked.ToString());
                keys.SetValue("vLod", ckLod.Checked.ToString());
                keys.SetValue("vTnLMode", ckTnl.Checked.ToString());
                keys.SetValue("viShaderOnOff", ckShader.Checked.ToString());
                keys.Close();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
