using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace huang.module.ui.rightkeyguideui
{
    public class UIEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        Transform tip;
        Transform Tip
        {
            get
            {
                if(tip==null) tip= transform.Find("Tip");
                return tip;
            }
        }

        private void OnDisable()
        {
            if (Tip)
                Tip.gameObject.SetActive(false);
        }

        bool isExite = false;
        public void OnPointerEnter(PointerEventData eventData)
        {
            isExite = false;
            Invoke("DelayCallBack", 0.5f);
        }

        void DelayCallBack()
        {
            if (!isExite)
            {
                transform.localScale = Vector3.one * 1.2f;
                if (Tip)
                    Tip.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isExite = true;
            transform.localScale = Vector3.one;
            if (Tip)
                Tip.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (Tip)
                Tip.gameObject.SetActive(false);
        }
    }
}