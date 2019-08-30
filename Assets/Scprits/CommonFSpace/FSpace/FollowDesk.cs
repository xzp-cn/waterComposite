// **********************************************************************
// Copyright (C) 2017 The company name
//
// 文件名(File Name):             FollowDesk.cs
// 作者(Author):                  Zhang
// 创建时间(CreateTime):           2018/6/12 18:1:31
// 修改者列表(modifier):
// 模块描述(Module description):
// **********************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSpace;

public class FollowDesk : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.localEulerAngles = new Vector3(FCore.slantAngle, 0, 0);
        //transform.localEulerAngles = new Vector3(FCore.slantAngle, 0, 0);
        transform.rotation = FCore.screenRotation;
    }

}
