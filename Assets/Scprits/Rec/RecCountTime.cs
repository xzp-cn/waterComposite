using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecCountTime : MonoBehaviour
{

    DateTime _timeNow = new DateTime();

    //显示时间的Text
    public Text timeText;
    private void OnEnable()
    {
        _timeNow = DateTime.Now;
    }

    TimeSpan _timeCount = new TimeSpan();
    void Update()
    {
        _timeCount = DateTime.Now - _timeNow;
        timeText.text = string.Format("{0}{1:00}:{2:00}:{3:00}", "正在录制 ", _timeCount.Hours, _timeCount.Minutes, _timeCount.Seconds);
    }
}
