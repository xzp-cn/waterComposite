using FSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CertificateVerify;

namespace liu
{
    /// <summary>
    /// 监听2、3D模式
    /// 开启和关闭相应的模式
    /// </summary>
    public class Monitor23DMode : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static Monitor23DMode instance { get; private set; } = null;
        /// <summary>
        /// 默认眼镜格式为非2，3D模式
        /// 一开始进行状态设置
        /// </summary>
        int lastGlassStatus = 2;
        EventSystem eventSystem;
        StandaloneInputModule standaloneInputModule;
        [HideInInspector]
        public F3dSpaceInputModule f3DSpaceInputModule;

        [HideInInspector]
        public bool is3D = false;

        private bool _is3D = true;

        /// <summary>
        /// 第一次进行检测
        /// </summary>
        bool flag = true;

        GameObject penRayObj;
        /// <summary>
        /// 触笔gameObject
        /// </summary>
        public GameObject PenRayObj
        {
            get
            {
                if (penRayObj == null) penRayObj = GameObject.FindObjectOfType<PenRay>()?.gameObject;
                return penRayObj;
            }
        }

        private Transform camera3DTrans;
        public Transform Camera3DTrans
        {
            get { if (camera3DTrans == null) camera3DTrans = FindObjectOfType<Camera3D>().transform; return camera3DTrans; }
        }

        public Camera camera2D;

        private Camera _cameraCenter;

        public Camera CameraCenter
        {
            get
            {
                if (_cameraCenter == null)
                    _cameraCenter = Camera3DTrans.Find("cam_c").GetComponent<Camera>();
                return _cameraCenter;
            }
        }

        private Camera _cameraLeft;

        public Camera CameraLeft
        {
            get
            {
                if (_cameraLeft == null)
                    _cameraLeft = Camera3DTrans.Find("cam_l").GetComponent<Camera>();
                return _cameraLeft;
            }
        }

        private Camera _cameraRight;

        public Camera CameraRight
        {
            get
            {
                if (_cameraRight == null)
                    _cameraRight = Camera3DTrans.Find("cam_r").GetComponent<Camera>();
                return _cameraRight;
            }
        }
        private void Awake()
        {
            instance = this;

            eventSystem = FindObjectOfType<EventSystem>();

            standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneInputModule)
            {
                Destroy(standaloneInputModule);
            }

            f3DSpaceInputModule = eventSystem.GetComponent<F3dSpaceInputModule>();
            if (f3DSpaceInputModule == null)
            {
                f3DSpaceInputModule = eventSystem.gameObject.AddComponent<F3dSpaceInputModule>();
            }

            //默认先设置为2D模式
            SetCameraAccordingTo23DState(false);
            PenRayObj?.SetActive(false);
            f3DSpaceInputModule.is3D = false;
        }
        public System.Action to3dAction;
        void Update()
        {

            //根据眼镜的状态
            //if (VerifyInfo.Instance.curOT == OperateType.Only_2D)
            //{
            //    is3D = false;
            //}
            //else if (VerifyInfo.Instance.curOT == OperateType.Only_3D)
            //{
            //    is3D = true;
            //}
            //else if (VerifyInfo.Instance.curOT == OperateType.Both2D_3D)
            //{
            //    if (OCVData._data.GlassStatus != lastGlassStatus)
            //    {
            //        if (OCVData._data.GlassStatus == 1)
            //        {
            //            is3D = true;
            //        }
            //        else
            //        {
            //            is3D = false;
            //        }
            //        lastGlassStatus = OCVData._data.GlassStatus;

            //    }
            //}


            if (OCVData._data.GlassStatus != lastGlassStatus)
            {
                if (OCVData._data.GlassStatus == 1)
                {
                    is3D = true;
                    to3dAction?.Invoke();
                    to3dAction = null;
                }
                else
                {
                    is3D = false;
                }
                lastGlassStatus = OCVData._data.GlassStatus;
            }



            Set23DUIModel();
        }


        /// <summary>
        /// 根据2/3D的状态
        /// 设置f3DSpaceInputModule visibale
        /// Cursor.visible
        /// 投屏状态
        /// </summary>
        void Set23DUIModel()
        {
            if (_is3D != is3D || flag) //默认第一次执行一次判断
            {
                flag = false;
                _is3D = is3D;
                PenRayObj?.SetActive(is3D);
                SetCameraAccordingTo23DState(is3D);
                f3DSpaceInputModule.is3D = is3D;

                if (is3D)
                {
#if UNITY_STANDALONE
                    Cursor.visible = false;
#endif
                    FCore.SetScreen3DSelf();
                }
                else
                {
#if UNITY_STANDALONE
                    Cursor.visible = true;
#endif
                    FCore.SetScreen2DSelf();
                }
            }
        }

        /// <summary>
        /// 设置2、3D相机的状态
        /// </summary>
        /// <param name="is3D"></param>
        void SetCameraAccordingTo23DState(bool is3D)
        {
            camera2D?.gameObject.SetActive(!is3D);
            CameraLeft?.gameObject.SetActive(is3D);
            CameraRight?.gameObject.SetActive(is3D);

            StartCoroutine("DelayToDraw");
        }

        IEnumerator DelayToDraw()
        {
            yield return new WaitForSeconds(0.04f);
            //重新绘制当前显示相机的投影矩阵

            ScreenCamera3D[] screenCamera3Ds = FindObjectsOfType<ScreenCamera3D>();
            for (int i = 0; i < screenCamera3Ds.Length; i++)
            {
                screenCamera3Ds[i].enabled = true;
            }
        }

    }
}
