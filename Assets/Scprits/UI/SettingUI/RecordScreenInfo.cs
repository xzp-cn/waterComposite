using huang.common.screen;
using IniParser;
using IniParser.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace huang.module.ui.settingui
{
    public class RecordScreenInfo
    {

        public static void SetLocalScreenMode(string config3DPath)
        {
            //初始化INIParser
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.AllowDuplicateKeys = true;
            parser.Parser.Configuration.OverrideDuplicateKeys = true;
            parser.Parser.Configuration.AllowDuplicateSections = true;

            //如果没有ini路径。默认打开3D。
            FileInfo ini = new FileInfo(config3DPath);
            if (!ini.Exists)
            {
                if (!Directory.Exists(ini.Directory.FullName))
                    Directory.CreateDirectory(ini.Directory.FullName);
                var file = File.Create(config3DPath);
                file.Close();
                SectionDataCollection sec = new SectionDataCollection();
                IniData iniData = new IniData(sec);
                sec.AddSection("Screen");
                sec.AddSection("ScreenDimensional");
                sec.AddSection("ScreenMode");
                sec.AddSection("CanUseAR");
                iniData["Screen"].AddKey("IsScreen", "Open");
                iniData["ScreenDimensional"].AddKey("Dimensional", "2D");
                iniData["ScreenMode"].AddKey("ScreenMode", "VR");
                iniData["CanUseAR"].AddKey("AR", "True");

                parser.WriteFile(config3DPath, iniData);
                ScreenManger.Instance.SetScreenMode(ScreenManger.DualScreenMode.VR_2D);
                UISetting.isScreen = true;
                UISetting.screenDimensional = UISetting.ScreenDimensional.TwoDimensional;
                UISetting.screenmode = UISetting.ScreenMode.VR;
                UISetting.curScreenmode = ScreenManger.DualScreenMode.VR_2D;

            }

            else
            {
                IniData iniData = parser.ReadFile(ini.FullName);
                string isOpen = iniData["Screen"]["IsScreen"];
                string cDimensional = iniData["ScreenDimensional"]["Dimensional"];
                string cScreenMode = iniData["ScreenMode"]["ScreenMode"];
                string cAR = iniData["CanUseAR"]["AR"];

                bool isScreen = isOpen == "Open" ? true : false;

                UISetting.lastIsScreen = isScreen;

                int screenNum = liu.GetScreenMode.GetSreenNum();
                isScreen = screenNum > 1 ? isScreen : false;

                var dimensional = cDimensional == "2D" ? UISetting.ScreenDimensional.TwoDimensional : UISetting.ScreenDimensional.ThreeDimensional;
                var screenMode = cScreenMode == "VR" ? UISetting.ScreenMode.VR : UISetting.ScreenMode.AR;
                var arFunc = cAR == "True" ? true : false;

                UISetting.isScreen = isScreen;
                UISetting.screenDimensional = dimensional;
                UISetting.screenmode = screenMode;
                //UISetting.screenmode = UISetting.ScreenMode.VR;
                liu.GlobalConfig.canUseCameraAR = arFunc;

                UISetting.lastScreenDimensional = dimensional;
                UISetting.lastScreenmode = screenMode;
                //UISetting.lastScreenmode = UISetting.ScreenMode.VR; 

                var mode = UISetting.GetScreenMode();
                UISetting.curScreenmode = mode;
                ScreenManger.Instance.SetScreenMode(mode);


            }
        }


        public static void SaveScreenModeToLocal(string config3DPath)
        {
            //初始化INIParser
            var parser = new FileIniDataParser();
            parser.Parser.Configuration.AllowDuplicateKeys = true;
            parser.Parser.Configuration.OverrideDuplicateKeys = true;
            parser.Parser.Configuration.AllowDuplicateSections = true;

            //如果没有ini路径。默认打开3D。
            FileInfo ini = new FileInfo(config3DPath);
            if (!ini.Exists)
            {
                Debug.LogWarning("Config3D.ini 路径不存在" + config3DPath);
                return;
            }

            IniData iniData = parser.ReadFile(ini.FullName);
            string isOpen = UISetting.isScreen ? "Open" : "Close";
            string cDimensional = UISetting.screenDimensional == UISetting.ScreenDimensional.ThreeDimensional ? "3D" : "2D";
            string cScreenMode = UISetting.screenmode == UISetting.ScreenMode.AR ? "AR" : "VR";
            string cARFunc = liu.GlobalConfig.canUseCameraAR ? "True" : "False";

            iniData["Screen"]["IsScreen"] = isOpen;
            iniData["ScreenDimensional"]["Dimensional"] = cDimensional;
            iniData["ScreenMode"]["ScreenMode"] = cScreenMode;
            iniData["CanUseAR"]["AR"] = cARFunc;

            parser.WriteFile(config3DPath, iniData, Encoding.UTF8);

            Debug.Log("Config3D.ini 保存路径：" + config3DPath);
        }
    }
}