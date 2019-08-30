using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace liu
{
    /// <summary>
    /// 移动和旋转的切换
    /// </summary>
    public class MoveAndRotateSwitch : MonoBehaviour
    {
        public GameObject moveBtn, rotateBtn;
        void Start()
        {
            UGUIEventListener.Get(moveBtn).onClick += MoveOnClick;
            UGUIEventListener.Get(rotateBtn).onClick += RotateOnClick;
        }

        void MoveOnClick(GameObject go)
        {
            moveBtn.SetActive(false);
            rotateBtn.SetActive(true);
        }

        void RotateOnClick(GameObject go)
        {
            rotateBtn.SetActive(false);
            moveBtn.SetActive(true);
        }

        private void OnDestroy()
        {
            UGUIEventListener.Get(moveBtn).onClick -= MoveOnClick;
            UGUIEventListener.Get(rotateBtn).onClick -= RotateOnClick;
        }
    }
}