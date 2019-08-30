using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace liu
{
    /// <summary>
    /// 修改底部Button选中的颜色
    /// </summary>
    public class BottomMiddleButtonTextColor : MonoBehaviour
    {

        void Start()
        {
            int index = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                index = i;
                transform.GetChild(i).GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
                    {
                        
                        if (isOn)
                        {
                            transform.GetChild(index).Find("Label").GetComponent<Text>().color = new Color32(255, 255, 0, 255);
                        }
                        else
                        {
                            transform.GetChild(index).Find("Label").GetComponent<Text>().color = new Color32(255, 234, 0, 255);
                        }
                    });
            }

        }
    }
}