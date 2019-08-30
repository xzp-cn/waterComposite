using huang.common.recordscreentool;
using huang.common.screen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace liu
{

    /// <summary>
    /// 录屏控制
    /// </summary>
    public class RecordOperate : MonoBehaviour
    {
        /// <summary>
        /// 桌面的路径
        /// </summary>
        string deskPath;

        /// <summary>
        /// 记录录屏开始时间的UI
        /// </summary>
        RecEvent RecEvent;

        private void Start()
        {
            deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            RecEvent = FindObjectOfType<RecEvent>();
            //FView.Instance.applicationQuit += Instance_applicationQuit;
        }

        private void Instance_applicationQuit()
        {
            StopRec();
        }

        ScreenControlObj screenControlObj
        {
            get { return FindObjectOfType<ScreenControlObj>(); }
        }

        bool isKeyCore_LeftAltPress = false;
        bool isKeyCore_SPress = false;
        [HideInInspector]
        public bool startFlag = false;
        void Update()
        {

        }

        /// <summary>
        /// 开始录屏
        /// </summary>
        public bool StartRec()
        {
            string ffmpegPath = Application.streamingAssetsPath + "/ffmpeg.exe";
            string closeprocessPath = Application.streamingAssetsPath + "/sendsignal.exe";
            if (File.Exists(ffmpegPath) && File.Exists(closeprocessPath))
            {
                int screenCount = GetScreenMode.GetSreenNum();
                if (screenControlObj != null)
                {
                    if (screenControlObj.curMode != ScreenManger.DualScreenMode.None && screenCount > 1)//开了投屏就开始录，否则不录
                    {
                        //RecUI.SetActive(true);
                        RecEvent.ShowRecUI(true);
                        StartCoroutine(DelayStartRec(ffmpegPath, deskPath, closeprocessPath, 0.5f));
                        //RecordScreenTool.Instance.StartRecordScreen(ffmpegPath, deskPath, closeprocessPath);
                        //screenControlObj.OpenRecWin();
                        startFlag = true;

                        Debug.Log("startFlag = true");
                        return true;
                    }
                    else
                    {
                        screenControlObj.curMode = ScreenManger.DualScreenMode.None;
                        OperateWarnning.Instance.ShowWarnningPanel("只有开启投屏才能录屏！");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogError("RecordScreenTool.Update()    程序ffmpeg.exe不在  " + ffmpegPath + "  路径下   或者 closeprocess.exe程序不在 + " + closeprocessPath + "   路径下");
                return false;
            }
        }

        IEnumerator DelayStartRec(string ffmpegPath, string deskPath, string closeprocessPath, float time)
        {
            yield return new WaitForSeconds(time);
            RecordScreenTool.Instance.StartRecordScreen(ffmpegPath, deskPath, closeprocessPath);
        }
        public void StopRec()
        {
            RecordScreenTool.Instance.StopRecoreScreen();
            //screenControlObj.CloseRecWin();
            RecEvent.ShowRecUI(false);
            Debug.Log("StopRec startFlag = false");
            startFlag = false;
        }
    }
}
