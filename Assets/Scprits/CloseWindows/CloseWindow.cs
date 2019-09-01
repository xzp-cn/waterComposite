using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace liu
{
    /// <summary>
    /// 关闭指定的窗口
    /// </summary>
    public class CloseWindow : MonoBehaviour,IPointerClickHandler
    {
        /// <summary>
        /// 关闭指定的窗口
        /// </summary>
        public GameObject closeWindow;


        /// <summary>
        /// 当前的BlockPanel被点击时执行
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            closeWindow.SetActive(false);
        }
    }
}