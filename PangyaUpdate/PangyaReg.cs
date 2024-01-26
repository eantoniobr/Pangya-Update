using Microsoft.Win32;
using System;
using System.IO;
//class para seta o registro, depois vejo se esta okay
namespace PangyaUpdate
{
    public class PangyaReg
    {
        RegistryKey Main { get; set; }
        RegistryKey Opt { get; set; }
        RegistryKey Mip { get; set; }
        public PangyaReg()
        {
            //registrar os dados iniciais
            string subkey = "SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya";
            Main = Registry.LocalMachine.OpenSubKey(subkey, true);
            if (!ExistRegMain())
            {
                Main = Registry.LocalMachine.CreateSubKey("SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya");
            }
            subkey = "SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option";
            Opt = Registry.LocalMachine.OpenSubKey(subkey, true);           
            subkey = "SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option\\Mip";
            Mip = Registry.LocalMachine.OpenSubKey(subkey, true);
        }

        public void CreateMainReg(string patch_version, uint patch_num)
        {
            if (ExistRegMain())
            {
                Main.SetValue("Launcher Version", "v6.0");
                Main.SetValue("Argument", "not_used");
                Main.SetValue("Patcher", "update.exe");
                Main.SetValue("Launcher", "update.exe");
                Main.SetValue("Ver", patch_version);
                Main.SetValue("PatchNum", patch_num);
                Main.SetValue("Install_Dir", Directory.GetCurrentDirectory());//local da execucao?, seta a primeira vez e nada mais?
                Main.SetValue("IntegratedPak", "projectg700gb+.pak");
            }            
        }
        public void CreateOptReg(string vScreenWidth, string vScreenHeight, string WindowsMode, string aMssEnableHwSound, string vLod, string vTnLMode, string viShaderOnOff)
        {
            if (ExistRegMain())
            {
                //             
                //sets aqui
                Opt.SetValue("vScreenWidth", vScreenWidth);
                Opt.SetValue("vScreenHeight", vScreenHeight);
                Opt.SetValue("vWindowMode", WindowsMode);
                Opt.SetValue("aMssEnableHwSound",aMssEnableHwSound);
                Opt.SetValue("vLod", vLod);
                Opt.SetValue("vTnLMode", vTnLMode);
                Opt.SetValue("viShaderOnOff", viShaderOnOff);
            }
            else
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
                Opt.SetValue("gLastLoginID", "");
                Opt.SetValue("gIdentity", "0");
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
                //padrao
            }
            Opt.Close();
        }

        public void CreateMipReg(string vDDSRes, string vUseMipmap, string vMaxMipLvl, string vMipCreateFilter)
        {
            if (ExistRegMain())
            {
            }
            else
            {
                Opt = Registry.LocalMachine.CreateSubKey("SOFTWARE\\WOW6432Node\\Ntreev USA\\Pangya\\Option\\Mip");
                //padrao !
                Mip.SetValue("vDDSRes", "0");
                Mip.SetValue("vUseMipmap", "0");
                Mip.SetValue("vMaxMipLvl", "1");
                Mip.SetValue("vMipCreateFilter", "2");
            }
            Mip.Close();
        }
        public string setMainValue(string key, string value)
        {
            Main.SetValue(key, value);
            return getMainValue(key);
        }
        public string getMainValue(string key)
        {
            var _obj = Main.GetValue(key);
            if (_obj == null && key == "Ver") _obj = "Pangya Update Ver";

            return Convert.ToString(_obj);
        }

        public string setOptValue(string key, string value)
        {
            Main.SetValue(key, value);
            return getOptValue(key);
        }
        public string getOptValue(string key)
        {
            return Convert.ToString(Main.GetValue(key));
        }
        public string setMipValue(string key, string value)
        {
            Mip.SetValue(key, value);
            return getMipValue(key);
        }
        public string getMipValue(string key)
        {
            return Convert.ToString(Main.GetValue(key));
        }
        public bool ExistRegMain()
        { return Main != null; }
        public bool ExistRegOption()
        { return Opt != null; }

        public bool ExistRegMip()
        { return Mip != null; }
    }
}
