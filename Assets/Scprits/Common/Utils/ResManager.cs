using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResManager
{
    /// <summary>
    /// resources获取ui精灵
    /// </summary>
    /// <param name="UIPath"></param>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public static Sprite GetSprite(string path)
    {
        Sprite sp = null;
        try
        {
            sp = Resources.Load<Sprite>(path);
        }
        catch (System.Exception)
        {
            Debug.LogError("没有这样的sprite    " + path);
        }
        return sp;
    }
    /// <summary>
    /// resources获取texture
    /// </summary>
    /// <param name="texturePath"></param>
    /// <param name="texture_name"></param>
    /// <returns></returns>
    public static Texture GetTexture(string path)
    {
        Texture texture = null;
        try
        {
            texture = Resources.Load<Texture>(path);
        }
        catch (System.Exception)
        {
            Debug.LogError("没有这样的texture   " + path);
        }
        return texture;
    }
    /// <summary>
    /// resources获取预制体并实例化
    /// </summary>
    /// <param name="assetname"></param>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public static GameObject GetPrefab(string path)
    {
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
        {
            Debug.LogError("没有prefab    " + path);
            return null;
        }
        else
        {
            GameObject go = GameObject.Instantiate(obj);
            string str = go.name.Split('(')[0];
            string[] nameArr = str.Split('/');
            str = nameArr[nameArr.Length - 1];
            go.name = str;
            return go;
        }
    }
    /// <summary>
    ///获取状态机
    /// </summary>
    /// <param name="clipPath"></param>
    /// <param name="Target"></param>
    //public static Animator GetAnimator(string path)
    //{
    //    Animator source = Resources.Load<GameObject>(path).GetComponent<Animator>();
    //    if (source == null)
    //    {
    //        Debug.LogError("源文件为空!");
    //    }
    //    return source;
    //}
}
