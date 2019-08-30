using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftNavImageManager : MonoBehaviour
{

    public static LeftNavImageManager leftNavImageManager = null;
    Image selfImage;
    void Awake()
    {
        leftNavImageManager = this;
        selfImage = GetComponent<Image>();
    }

    public void ReJudgeNavImageEnable()
    {
        if (transform.childCount > 0)
        {
            int activesCount = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeInHierarchy)
                    activesCount++;
            }

            if(activesCount > 0)
            {
                selfImage.enabled = true;
            }
            else
            {
                selfImage.enabled = false;
            }
        }
        else
        {
            selfImage.enabled = false;
        }
    }
}
