using huang.module.ui.settingui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using CertificateVerify;
using huang.common.screen;

namespace liu
{
    public enum OperationModel
    {
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 旋转
        /// </summary>
        Rotate,
        /// <summary>
        /// 呆在原地，处理点击状态
        /// </summary>
        Stay,
    }



    /// <summary>
    /// 公共属性存储的位置
    /// </summary>
    public class GlobalConfig : MonoBehaviour
    {

        public enum gEvent
        {
            AppQuit,
        }

        public UISetting uISetting;

        ///// <summary>
        ///// 证书输入面板
        ///// </summary>
        //public GameObject verifyPanel;

        /// <summary>
        /// 能否使用AR功能
        /// </summary>
        public static bool canUseCameraAR = true;

        /// <summary>
        /// 3D模式配置文件存放的位置
        /// </summary>
        public static string config3DPath
        {
            get
            {
                string path = "";
#if UNITY_EDITOR
                path = "./Config3D.ini";

#else
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/Config3D.ini").Replace("\\", "/");
#endif
                return path;
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static GlobalConfig Instance = new GlobalConfig();

        [HideInInspector]
        public string versionNO;

        private GlobalConfig()
        {
            Instance = this;
        }

        FView fView;
        private void Start()
        {
            versionNO = Application.version;
            fView = FindObjectOfType<FView>();
            fView.applicationQuit += OnApplicationQuit1;
            ScreenControlObj screenControlObj = FindObjectOfType<ScreenControlObj>();
            Debug.Log(screenControlObj.gameObject.name);
            screenControlObj.JudgeHasExternalCameraAction += CallJudgeCameraAction;
            //GlobalEntity.GetInstance().AddListener(gEvent.AppQuit, OnApplicationQuit1);
            Debug.Log(gameObject.name);
        }

        void CallJudgeCameraAction()
        {
            if (uISetting)
                uISetting.JudgeHasExternalCamera();
            //else
            //    Debug.Log("UISetting  ==  null");
        }


        [HideInInspector]
        public OperationModel operationModel = OperationModel.Move;

        [HideInInspector]
        /// <summary>
        /// 当前被点中或者操作的物体
        /// </summary>
        public GameObject _curOperateObj;

        public void SetOperationModel(int state)
        {
            operationModel = (OperationModel)state;
        }

        private void OnApplicationQuit1()
        {
            RecordScreenInfo.SaveScreenModeToLocal(config3DPath);
            FindObjectOfType<ScreenControlObj>().JudgeHasExternalCameraAction -= CallJudgeCameraAction;
            //GlobalEntity.GetInstance().RemoveListener(gEvent.AppQuit, OnApplicationQuit1);
            fView.applicationQuit -= OnApplicationQuit1;
        }
    }
}
