using huang.common.screen;
using IniParser;
using IniParser.Model;
using liu;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace huang.module.ui.settingui
{
    public class UISetting : MonoBehaviour
    {
        /// <summary>
        /// 投屏维数
        /// </summary>
        public enum ScreenDimensional
        {
            /// <summary>
            /// 2D
            /// </summary>
            TwoDimensional,
            /// <summary>
            /// 3D
            /// </summary>
            ThreeDimensional,
            None
        }

        public enum ScreenMode
        {
            /// <summary>
            /// VR投屏
            /// </summary>
            VR,
            /// <summary>
            /// AR投屏
            /// </summary>
            AR,
            None
        }

        //public openglwin5v4 vrOpenglWin;
        //public GameObject VRCamera;
        //public GameObject ARCamera;
        //public RawImage ARimg;

        public static bool isScreen = false;
        public static bool lastIsScreen = false;

        public static ScreenDimensional screenDimensional = ScreenDimensional.TwoDimensional;
        public static ScreenDimensional lastScreenDimensional = ScreenDimensional.TwoDimensional;
        public static ScreenMode screenmode = ScreenMode.VR;
        public static ScreenMode lastScreenmode = ScreenMode.VR;
        public static ScreenManger.DualScreenMode curScreenmode { get; set; } = ScreenManger.DualScreenMode.None;
        /// <summary>
        /// 主场景里面那个按投屏的按钮
        /// </summary>
        public static void SetDefaultScreen()
        {
            if (isScreen)
            {
                if (screenDimensional == ScreenDimensional.TwoDimensional && screenmode == ScreenMode.AR)
                {
                    ScreenManger.Instance.SetScreenMode(ScreenManger.DualScreenMode.None);
                    isScreen = false;
                }
                else
                {
                    ScreenManger.Instance.SetScreenMode(ScreenManger.DualScreenMode.AR_2D);
                    isScreen = true;
                }
            }
            else
            {
                ScreenManger.Instance.SetScreenMode(ScreenManger.DualScreenMode.AR_2D);
                isScreen = true;
            }

            screenDimensional = ScreenDimensional.TwoDimensional;
            screenmode = ScreenMode.AR;
            curScreenmode = GetScreenMode();
        }

        //void Update()
        //{
        //    //
        //    if (gameObject.activeSelf)
        //        JudgeHasExternalCamera();
        //}

        bool hasCameraFlag = true;

        /// <summary>
        /// 判断是否有外接相机
        /// 如果存在，则允许AR投屏
        /// 没有则AR投屏disable
        /// </summary>
        public void JudgeHasExternalCamera()
        {
            bool hasExternalCamera = false;
            bool canUseARFunc = liu.GlobalConfig.canUseCameraAR;
            var devices = WebCamTexture.devices;
            //UnityEngine.Debug.Log("devices.Length " + devices.Length);
            if (devices.Length > 4)
            {
                string _deviceName = "";
                foreach (var item in devices)
                {
                    if (item.name.EndsWith("C920"))
                    {
                        _deviceName = item.name;
                        hasExternalCamera = true;
                        break;
                    }
                }
            }


            if (!hasExternalCamera || !canUseARFunc)//没有外接相机
            {
                //ScreenManger.Instance.SetWebCameraIsCanntUse();
                if (isScreen) //如果投屏，则需要判断现在是否点选了AR投屏
                {
                    if (screenmode == ScreenMode.AR) //把VRToggle设为true，aRToggle.enable 设为false，字体和图片颜色都设置为灰色
                    {
                        screenmode = ScreenMode.VR;
                        aRToggle.isOn = false;
                        vRToggle.isOn = true;
                    }
                    aRToggle.interactable = false;
                    ARBackground.color = ARCheckmark.color = ARText.color = new Color32(140, 140, 140, 255);
                    //screenmode = ScreenMode.VR;
                    //lastScreenmode = ScreenMode.VR;
                }
            }
            else
            {
                if (isScreen)
                {
                    if (hasExternalCamera == hasCameraFlag)
                        return;

                    aRToggle.interactable = true;
                    ARBackground.color = ARCheckmark.color = ARText.color = Color.white;
                }
            }
            hasCameraFlag = hasExternalCamera == canUseARFunc ? canUseARFunc : false;
        }


        /// <summary>
        /// 存储路径
        /// </summary>
        public static string RecordPath
        {
            get; private set;
        }

        GameObject nonARCameraWarnning;
        Text nonARCameraWarnningText;
        // Use this for initialization
        void Awake()
        {
            ScreenManger.Instance.SetScreenState();
            int screenNum = liu.GetScreenMode.GetSreenNum();
            FindObjectOfType<ManagergEventInfo>().UISettingHandle();

            //GlobalConfig.Instance.UISetting = this;

            if (screenNum > 1)
            {
                Debug.Log("UISetting.Awake(): 开启扩展模式");
                //FSpace.FCore.SetDualScreenExtend();
                Common.ScreenHelper.SetProjection(Common.ScreenHelper.SDC_TOPOLOGY_EXTEND);
            }

            //Debug.Log("UISetting.Awake(): screenNum   " + screenNum);
            var openFildPath = transform.Find("FieldPath");

            SetScreenToggleListener();

            SetOtherListener();

            var versionText = transform.Find("Version/PathText").GetComponent<Text>();

            versionText.text = Application.version;

            //ScreenManger.Instance.InitScreenMode(VRCamera, ARCamera, vrOpenglWin, ARimg);

            //ScreenManger.Instance.SetScreenMode(ScreenManger.DualScreenMode.VR_2D);
            //curScreenmode = GetScreenMode();
            //RecordScreenMode.SetLocalScreenMode(GlobalConfig.config3DPath);

            Invoke("DelayCall", 0.2f);

            SetAllToggleState();
            AttributeGetValue();
            //ARCamera.SetActive(true);
            //RecordScreenMode.SetLocalScreenMode(GlobalConfig.config3DPath);
            EnableScreenMode(isScreen);

            nonARCameraWarnning = aRToggle.transform.parent.Find("NonARCameraWarnning").gameObject;
            nonARCameraWarnningText = nonARCameraWarnning.GetComponent<Text>();
            nonARCameraWarnning.SetActive(false);
            UGUIEventListener.Get(aRToggle.gameObject).onClick += ClickARToggle;
            //gameObject.SetActive(false);
        }

        /// <summary>
        /// 关闭投屏
        /// </summary>
        public void CloseProjection()
        {
            noneScreenToggle.isOn = true;
            lastIsScreen = false;
        }

        void DelayCall()
        {
            //gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
            RecordScreenInfo.SetLocalScreenMode(GlobalConfig.config3DPath);
        }


        private void ClickARToggle(GameObject go)
        {
            if (!hasCameraFlag || !GlobalConfig.canUseCameraAR)
            {
                nonARCameraWarnning.SetActive(true);
                if (!GlobalConfig.canUseCameraAR)
                    nonARCameraWarnningText.text = "AR功能不适用";
                else
                    nonARCameraWarnningText.text = "未检测到摄像头";

                nonARCameraWarnning.transform.DOMove(nonARCameraWarnning.transform.position, 3f).OnComplete
                    (() =>
                    {
                        nonARCameraWarnning.SetActive(false);
                    });
            }
        }

        //void DelayCloseARCamera()
        //{
        //    ARCamera.SetActive(false);
        //}
        private void OnEnable()
        {
            StartCoroutine(SetScreenModeOnOpen());

            SetRecordVidoPath();
        }


        private void SetRecordVidoPath()
        {
            var pathText = transform.Find("RecordPath/PathText").GetComponent<Text>();
            RecordPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (string.IsNullOrEmpty(RecordPath))
            {
                RecordPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                PlayerPrefs.SetString("RecordPath", RecordPath);
            }
            pathText.text = RecordPath;
        }

        /// <summary>
        /// 获取投屏模式
        /// </summary>
        /// <returns></returns>
        public static ScreenManger.DualScreenMode GetScreenMode()
        {
            if (isScreen == false)
            {
                return ScreenManger.DualScreenMode.None;
            }
            if (screenDimensional == ScreenDimensional.ThreeDimensional)
            {
                if (screenmode == ScreenMode.AR)
                {
                    return ScreenManger.DualScreenMode.AR;
                }
                else if (screenmode == ScreenMode.VR)
                {
                    return ScreenManger.DualScreenMode.VR;
                }
            }
            else if (screenDimensional == ScreenDimensional.TwoDimensional)
            {
                if (screenmode == ScreenMode.AR)
                {
                    return ScreenManger.DualScreenMode.AR_2D;
                }
                else if (screenmode == ScreenMode.VR)
                {
                    return ScreenManger.DualScreenMode.VR_2D;
                }
            }
            return ScreenManger.DualScreenMode.None;
        }
        Toggle noneScreenToggle;
        Toggle screenToggle;

        Toggle twoDimensionalToggle;
        Toggle threeDimensionalToggle;

        Toggle vRToggle;
        Toggle aRToggle;

        /// <summary>
        /// 设置投屏toggle事件
        /// </summary>
        void SetScreenToggleListener()
        {
            var isScreenTran = transform.Find("IsScreen");
            var screenDimensionalTran = transform.Find("2_3DMode");
            var vrOrARMode = transform.Find("VR_ARMode");

            noneScreenToggle = isScreenTran.Find("NoneScreenToggle").GetComponent<Toggle>();
            screenToggle = isScreenTran.Find("ScreenToggle").GetComponent<Toggle>();

            twoDimensionalToggle = screenDimensionalTran.Find("TwoDimensionalToggle").GetComponent<Toggle>();
            threeDimensionalToggle = screenDimensionalTran.Find("ThreeDimensionalToggle").GetComponent<Toggle>();

            vRToggle = vrOrARMode.Find("VRToggle").GetComponent<Toggle>();
            aRToggle = vrOrARMode.Find("ARToggle").GetComponent<Toggle>();
            //isScreenTran.GetComponent<ToggleGroup>().NotifyToggleOn(noneScreenToggle);
            //screenDimensionalTran.GetComponent<ToggleGroup>().NotifyToggleOn(twoDimensionalToggle);
            //vrOrARMode.GetComponent<ToggleGroup>().NotifyToggleOn(vRToggle);
            noneScreenToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    isScreen = false;
                    noneScreenToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                    EnableScreenMode(isScreen);
                    SetScreenMode();
                }
                else
                {
                    noneScreenToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                }
            }));

            screenToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isOn)
                {
                    //int screenNum = openglwinDll.GetMonitorCount(true);
                    int screenNum = liu.GetScreenMode.GetSreenNum();
                    if (screenNum == 1)
                    {
                        noneScreenToggle.isOn = true;
                    }
                    else
                    {
                        isScreen = true;
                        screenToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                        EnableScreenMode(isScreen);
                        SetScreenMode();
                    }
                }
                else
                {
                    screenToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                }
            }));

            twoDimensionalToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isScreen)
                {
                    if (isOn)
                    {
                        screenDimensional = ScreenDimensional.TwoDimensional;
                        twoDimensionalToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        twoDimensionalToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                    }
                }
            }));

            threeDimensionalToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isScreen)
                {
                    if (isOn)
                    {
                        screenDimensional = ScreenDimensional.ThreeDimensional;
                        threeDimensionalToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        threeDimensionalToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                    }
                }
            }));

            vRToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isScreen)
                {
                    if (isOn)
                    {
                        screenmode = ScreenMode.VR;
                        vRToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        vRToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                    }
                }
            }));

            aRToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((isOn) =>
            {
                if (isScreen)
                {
                    if (isOn)
                    {
                        screenmode = ScreenMode.AR;
                        aRToggle.transform.Find("Label").GetComponent<Text>().color = Color.yellow;
                        SetScreenMode();
                    }
                    else
                    {
                        aRToggle.transform.Find("Label").GetComponent<Text>().color = Color.white;
                    }
                }
            }));
        }

        void SetScreenMode()
        {
            var curMode = GetScreenMode();
            ScreenManger.Instance.SetScreenMode(curMode);
        }


        Image TwoDimensionalBackground, TwoDimensionalCheckmark, ThreeDimensionalBackground, ThreeDimensionalCheckmark,
            VRBackground, VRCheckmark, ARBackground, ARCheckmark;
        Text TwoDimensionalText, ThreeDimensionalText, VRText, ARText;

        /// <summary>
        /// 设置Setting面板的按钮状态
        /// </summary>
        /// <param name="isSupportScreen"></param>
        void EnableScreenMode(bool isSupportScreen)
        {
            AttributeGetValue();
            //如果投屏模式中投屏高亮，则2D/3D模式和投屏模式可以交互
            if (isSupportScreen)
            {
                twoDimensionalToggle.interactable = threeDimensionalToggle.interactable = vRToggle.interactable = aRToggle.interactable = true;

                TwoDimensionalBackground.color = TwoDimensionalCheckmark.color = ThreeDimensionalBackground.color = ThreeDimensionalCheckmark.color =
            VRBackground.color = VRCheckmark.color = ARBackground.color = ARCheckmark.color = Color.white;

                if (screenmode == ScreenMode.AR)
                {
                    VRText.color = Color.white;
                    ARText.color = Color.yellow;
                }
                else
                {
                    VRText.color = Color.yellow;
                    ARText.color = Color.white;
                }

                if (screenDimensional == ScreenDimensional.ThreeDimensional)
                {
                    TwoDimensionalText.color = Color.white;
                    ThreeDimensionalText.color = Color.yellow;
                }
                else
                {
                    TwoDimensionalText.color = Color.yellow;
                    ThreeDimensionalText.color = Color.white;
                }
                //TwoDimensionalText.color = ThreeDimensionalText.color = 

            }
            else //2D/3D模式和投屏模式不支持交互
            {
                twoDimensionalToggle.interactable = threeDimensionalToggle.interactable = vRToggle.interactable = aRToggle.interactable = false;

                TwoDimensionalBackground.color = TwoDimensionalCheckmark.color = ThreeDimensionalBackground.color = ThreeDimensionalCheckmark.color =
            VRBackground.color = VRCheckmark.color = ARBackground.color = ARCheckmark.color = new Color32(140, 140, 140, 255);

                TwoDimensionalText.color = ThreeDimensionalText.color = VRText.color = ARText.color = new Color32(140, 140, 140, 255);
            }
        }

        //给每个属性赋值
        void AttributeGetValue()
        {
            //-----------------------Background-----------------------
            if (null == TwoDimensionalBackground)
            {
                TwoDimensionalBackground = twoDimensionalToggle.transform.Find("Background").GetComponent<Image>();
            }

            if (null == ThreeDimensionalBackground)
            {
                ThreeDimensionalBackground = threeDimensionalToggle.transform.Find("Background").GetComponent<Image>();
            }

            if (null == VRBackground)
            {
                VRBackground = vRToggle.transform.Find("Background").GetComponent<Image>();
            }

            if (null == ARBackground)
            {
                ARBackground = aRToggle.transform.Find("Background").GetComponent<Image>();
            }
            //-----------------------Label-----------------------
            if (null == TwoDimensionalText)
            {
                TwoDimensionalText = twoDimensionalToggle.transform.Find("Label").GetComponent<Text>();
            }

            if (null == ThreeDimensionalText)
            {
                ThreeDimensionalText = threeDimensionalToggle.transform.Find("Label").GetComponent<Text>();
            }

            if (null == VRText)
            {
                VRText = vRToggle.transform.Find("Label").GetComponent<Text>();
            }

            if (null == ARText)
            {
                ARText = aRToggle.transform.Find("Label").GetComponent<Text>();
            }
            //-----------------------Checkmark-------------------------------------
            if (null == TwoDimensionalCheckmark)
            {
                TwoDimensionalCheckmark = TwoDimensionalBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

            if (null == ThreeDimensionalCheckmark)
            {
                ThreeDimensionalCheckmark = ThreeDimensionalBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

            if (null == VRCheckmark)
            {
                VRCheckmark = VRBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

            if (null == ARCheckmark)
            {
                ARCheckmark = ARBackground.transform.Find("Checkmark").GetComponent<Image>();
            }

        }

        /// <summary>
        /// 设置应用更新、确定、取消的按钮事件
        /// </summary>
        void SetOtherListener()
        {
            var updateBtn = transform.Find("Version/UpdateBtn").GetComponent<Button>();
            var okBtn = transform.Find("OKBtn").GetComponent<Button>();
            var cancelBtn = transform.Find("CancelBtn").GetComponent<Button>();
            var openFieldBtn = transform.Find("RecordPath/OpenFieldBtn").GetComponent<Button>();

            updateBtn.onClick.AddListener(delegate
            {

            });

            okBtn.onClick.AddListener(delegate
            {

                var curMode = GetScreenMode();

                curScreenmode = curMode;
                //ScreenManger.Instance.InitScreenMode(VRCamera, ARCamera, vrOpenglWin, ARimg);
                //if (curScreenmode != curMode)
                //{
                //    ScreenManger.Instance.SetScreenMode(curMode);
                //}
                ScreenManger.Instance.SetScreenMode(curMode);
                //}
                lastIsScreen = isScreen;
                lastScreenDimensional = screenDimensional;
                lastScreenmode = screenmode;

                CloseSettingUI();
            });

            cancelBtn.onClick.AddListener(delegate
            {
                isScreen = lastIsScreen;
                screenDimensional = lastScreenDimensional;
                screenmode = lastScreenmode;

                SetScreenMode();
                SetAllToggleState();
                CloseSettingUI();
            });

            openFieldBtn.onClick.AddListener(delegate
            {
                SetSavePath();
            });
        }


        void SetAllToggleState()
        {
            if (isScreen)
            {
                screenToggle.isOn = true;
            }
            else
            {
                noneScreenToggle.isOn = true;
            }

            if (screenDimensional == ScreenDimensional.TwoDimensional)
            {
                twoDimensionalToggle.isOn = true;
            }
            else
            {
                threeDimensionalToggle.isOn = true;
            }

            if (screenmode == ScreenMode.VR)
            {
                vRToggle.isOn = true;
            }
            else
            {
                aRToggle.isOn = true;
            }
        }
        /// <summary>
        /// 打开UI的时候设置当前投屏模式对应的UI
        /// </summary>
        IEnumerator SetScreenModeOnOpen()
        {
            yield return new WaitForEndOfFrame();
            var isScreenTran = transform.Find("IsScreen");
            var screenDimensionalTran = transform.Find("2_3DMode");
            var vrOrARMode = transform.Find("VR_ARMode");

            var noneScreenToggle = isScreenTran.Find("NoneScreenToggle").GetComponent<Toggle>();
            var screenToggle = isScreenTran.Find("ScreenToggle").GetComponent<Toggle>();

            var twoDimensionalToggle = screenDimensionalTran.Find("TwoDimensionalToggle").GetComponent<Toggle>();
            var threeDimensionalToggle = screenDimensionalTran.Find("ThreeDimensionalToggle").GetComponent<Toggle>();

            var vRToggle = vrOrARMode.Find("VRToggle").GetComponent<Toggle>();
            var aRToggle = vrOrARMode.Find("ARToggle").GetComponent<Toggle>();
            //Debug.Log(isScreen + " isScreen " + "     " + "screenDimensional " + screenDimensional + "    screenmode " + screenmode);
            if (isScreen)
            {
                //isScreenTran.GetComponent<ToggleGroup>().NotifyToggleOn(screenToggle);
                screenToggle.isOn = true;
                //noneScreenToggle.isOn = false;
            }
            else
            {
                //isScreenTran.GetComponent<ToggleGroup>().NotifyToggleOn(noneScreenToggle);
                //screenToggle.isOn = false;
                noneScreenToggle.isOn = true;
            }

            if (screenDimensional == ScreenDimensional.ThreeDimensional)
            {
                //screenDimensionalTran.GetComponent<ToggleGroup>().NotifyToggleOn(threeDimensionalToggle);
                threeDimensionalToggle.isOn = true;
                //twoDimensionalToggle.isOn = false;
            }
            else
            {
                //screenDimensionalTran.GetComponent<ToggleGroup>().NotifyToggleOn(twoDimensionalToggle);
                //threeDimensionalToggle.isOn = false;
                twoDimensionalToggle.isOn = true;
            }

            if (screenmode == ScreenMode.AR)
            {
                //vrOrARMode.GetComponent<ToggleGroup>().NotifyToggleOn(aRToggle);
                aRToggle.isOn = true;
                //vRToggle.isOn = false;
            }
            else
            {
                //vrOrARMode.GetComponent<ToggleGroup>().NotifyToggleOn(vRToggle);
                //aRToggle.isOn = false;
                vRToggle.isOn = true;
            }
        }

        GameObject fileBrowserObj;
        //FileBrowser fileBrowserScript;
        /// <summary>
        /// 设置文件存储路径
        /// </summary>
        void SetSavePath()
        {

            //UIEntity.ShowUI("SimpleFileBrowser", (e, ui) =>
            // {
            //     ui.gameObject.SetActive(false);
            //     FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            //     FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            //     StartCoroutine(ShowLoadDialogCoroutine());
            //     return ui;
            // });
            //FileBrowser.
            if (null == fileBrowserObj)
            {
                fileBrowserObj = Instantiate(Resources.Load<GameObject>("SimpleFileBrowser/SimpleFileBrowser"));
                fileBrowserObj.transform.parent = transform.parent;
                fileBrowserObj.transform.localPosition = Vector3.zero;
                fileBrowserObj.transform.localScale = Vector3.one;
                //fileBrowserScript = fileBrowserObj.GetComponent<FileBrowser>();
            }
            fileBrowserObj.SetActive(false);
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        IEnumerator ShowLoadDialogCoroutine()
        {
            yield return FileBrowser.WaitForSaveDialog(true, "C:\\Users", "录屏保存", "保存");
            var pathText = transform.Find("RecordPath/PathText").GetComponent<Text>();
            Debug.Log("huang.module.ui.setting.ShowLoadDialogCoroutine()" + FileBrowser.Success + " " + FileBrowser.Result);
            if (FileBrowser.Success)
            {
                pathText.text = FileBrowser.Result;
                RecordPath = FileBrowser.Result;
                PlayerPrefs.SetString("RecordPath", RecordPath);
            }
        }

        /// <summary>
        /// 保存配置到本地
        /// </summary>
        public static void SaveMode()
        {
#if UNITY_EDITOR
            string config3DPath = "./Config3D.ini";

#else
            string config3DPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/Config3D.ini").Replace("\\", "/");
#endif
        }

        float time = 0.66f;
        /// <summary>
        /// 快捷键切换VR2D
        /// </summary>
        public IEnumerator ShortcutKeySetVR2D()
        {
            Debug.Log("VR2D快捷键");
            IsScreenProjection();
            Projection2D();
            yield return new WaitForSeconds(time);
            ProjectionVR();
            SyncProjectionState();
        }

        /// <summary>
        /// 快捷键切换VR3D
        /// </summary>
        public IEnumerator ShortcutKeySetVR3D()
        {
            Debug.Log("VR3D快捷键");
            IsScreenProjection();
            Projection3D();
            yield return new WaitForSeconds(time);
            ProjectionVR();
            SyncProjectionState();
        }

        /// <summary>
        /// 快捷键切换AR2D
        /// </summary>
        public IEnumerator ShortcutKeySetAR2D()
        {
            Debug.Log("AR2D快捷键");
            IsScreenProjection();
            Projection2D();
            yield return new WaitForSeconds(time);
            ProjectionAR();
            SyncProjectionState();
        }

        /// <summary>
        /// 快捷键切换AR2D
        /// </summary>
        public IEnumerator ShortcutKeySetAR3D()
        {
            Debug.Log("AR3D快捷键");
            IsScreenProjection();
            Projection3D();
            yield return new WaitForSeconds(time);
            ProjectionAR();
            SyncProjectionState();
        }

        /// <summary>
        /// 开启投屏
        /// </summary>
        void IsScreenProjection()
        {
            //if (noneScreenToggle.isOn == true)
            //{
            //    noneScreenToggle.isOn = false;
            //}
            Debug.Log("ddddddddddddd");
            if (screenToggle.isOn == false)
            {
                screenToggle.isOn = true;
            }

        }

        /// <summary>
        /// 2D投屏模式
        /// </summary>
        void Projection2D()
        {
            if (!twoDimensionalToggle.isOn)
            {
                twoDimensionalToggle.isOn = true;
            }
        }

        /// <summary>
        /// 3D投屏模式
        /// </summary>
        void Projection3D()
        {
            if (!threeDimensionalToggle.isOn)
            {
                threeDimensionalToggle.isOn = true;
            }
        }

        /// <summary>
        /// VR投屏模式
        /// </summary>
        void ProjectionVR()
        {
            if (!vRToggle.isOn)
            {
                vRToggle.isOn = true;
            }
        }

        /// <summary>
        /// AR投屏模式
        /// </summary>
        void ProjectionAR()
        {
            if (!aRToggle.isOn)
            {
                aRToggle.isOn = true;
            }
        }

        void ShowSettingUI()
        {
            //gameObject.SetActive(true);
            transform.localScale = Vector3.one;
        }

        void CloseSettingUI()
        {
            //gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// 把settingPanel scale设置为1
        /// </summary>
        public void OpenSettingPanel()
        {
            transform.localScale = Vector3.one;
            OnEnable();
        }

        /// <summary>
        /// 同步投屏的状态
        /// </summary>
        void SyncProjectionState()
        {
            lastIsScreen = isScreen;
            lastScreenDimensional = screenDimensional;
            lastScreenmode = screenmode;
        }
    }
}