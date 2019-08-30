using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    static Dictionary<string, GameObject> sceneObjDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 得到场景中的
    /// </summary>
    /// <param name="name"></param>
    public static GameObject GetScenesObj(string name)
    {
        GameObject go = null;
        if (sceneObjDic.Count > 0)
        {
            if (sceneObjDic.ContainsKey(name))
            {
                go = sceneObjDic[name];
                return go;
            }
        }

        GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootObjs.Length; i++)
        {
            string _name = rootObjs[i].name;
            if (_name == name)
            {
                go = rootObjs[i];
            }
            if (!sceneObjDic.ContainsKey(_name))
            {
                sceneObjDic.Add(_name, rootObjs[i]);
            }
        }
        return go;
    }
    /// <summary>
    /// 得到3d标签预制
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="canvasScale"></param>
    /// <returns></returns>
    public static PanelControl GetLine(Vector3 start, Vector3 end, float canvasScale = 0.001f)
    {
        PanelControl lineCtrl = ResManager.GetPrefab("SceneRes/Line").GetComponent<PanelControl>();
        lineCtrl.followStartPos.transform.localPosition = start;
        lineCtrl.followEndPos.transform.localPosition = end;
        lineCtrl.GetComponentInChildren<Canvas>().transform.localScale = Vector3.one * canvasScale;
        return lineCtrl;
    }
}
