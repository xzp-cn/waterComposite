using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingleTon<UIManager>
{
    Dictionary<string, GameObject> uiDic = new Dictionary<string, GameObject>();
    Canvas canvas;
    //RectTransform uiRoot;
    public new void Awake()
    {
        if (canvas == null)
        {
            canvas = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas").GetComponent<Canvas>();
            //uiRoot = new GameObject("UIRoot").AddComponent<RectTransform>();
            //uiRoot.SetParent(canvas.transform, false);
            //uiRoot.anchorMax = Vector2.one;
            //uiRoot.anchorMin = Vector2.zero;
            //uiRoot.offsetMax = Vector2.zero;
            //uiRoot.offsetMin = Vector2.zero;
            //uiRoot.localScale = Vector3.zero;
            //uiRoot.anchoredPosition3D = Vector3.zero;
        }
    }
    void InitUI(GameObject obj)
    {
        Awake();
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// 获取ui
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public T GetUI<T>(string uiName) where T : MonoBehaviour
    {
        GameObject ui = null;
        if (uiDic.ContainsKey(uiName))
        {
            ui = uiDic[uiName];
        }
        else
        {
            string path = "SceneRes/" + uiName;
            ui = ResManager.GetPrefab(path);
            uiDic.Add(uiName, ui);
        }
        ui.name = uiName;
        InitUI(ui);
        T uiCom = ui.GetComponent<T>();
        if (uiCom == null)
        {
            uiCom = ui.AddComponent<T>();
        }
        return uiCom;
    }

    /// <summary>
    ///设置UI层级
    /// </summary>
    /// <param name="uiName"></param>
    public void SetUIDepth(string uiName, int _siblingIndex)
    {
        if (!uiDic.ContainsKey(uiName))
        {
            Debug.LogError("canvas下没有该UI");
            return;
        }
        else
        {
            GameObject ui = uiDic[uiName];
            ui.transform.SetSiblingIndex(_siblingIndex);
        }
    }
    /// <summary>
    /// 关闭所有UI显示
    /// </summary>
    public void CloseAllUI()
    {
        if (uiDic.Count == 0)
        {
            return;
        }
        foreach (KeyValuePair<string, GameObject> item in uiDic)
        {
            item.Value.SetActive(false);
        }
    }

    public void ClearDic()
    {
        Dictionary<string, GameObject>.ValueCollection collec = uiDic.Values;
        var iter = collec.GetEnumerator();

        while (iter.MoveNext())
        {
            Destroy(iter.Current);
        }
        uiDic.Clear();
    }

    public void RemoveDic(string key)
    {
        uiDic.Remove(key);
    }
    private void OnDestroy()
    {
        Debug.Log("UIManager  ::   UIManager");
    }
}
