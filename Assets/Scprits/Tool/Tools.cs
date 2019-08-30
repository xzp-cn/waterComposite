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
}
