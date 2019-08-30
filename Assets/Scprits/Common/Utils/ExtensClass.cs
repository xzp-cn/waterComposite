using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensClass
{
    public static AnimationOper GetAnimatorOper(this GameObject go)
    {
        AnimationOper oper = go.GetComponent<AnimationOper>();
        if (oper == null)
        {
            oper = go.AddComponent<AnimationOper>();
        }
        return oper;
    }

    /// <summary>
    /// 获取UI高光
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    //public static UIFlah GetUIFlash(this GameObject go)
    //{
    //    UIFlah oper = go.GetComponent<UIFlah>();
    //    if (oper == null)
    //    {
    //        oper = go.AddComponent<UIFlah>();
    //    }
    //    return oper;
    //}
    /// <summary>
    /// 获取碰撞体
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static BoxCollider GetBoxCollider(this GameObject go)
    {
        BoxCollider oper = go.GetComponent<BoxCollider>();
        if (oper == null)
        {
            oper = go.AddComponent<BoxCollider>();
        }
        return oper;
    }

    public static LegacyAnimationOper GetLegacyAnimationOper(this GameObject go)
    {
        LegacyAnimationOper ak = go.GetComponent<LegacyAnimationOper>();
        if (ak == null)
        {
            ak = go.AddComponent<LegacyAnimationOper>();
        }
        return ak;
    }
    public static LiquidVolumeFX.LiquidVolume GetVolume(this GameObject go)
    {
        LiquidVolumeFX.LiquidVolume ak = go.GetComponent<LiquidVolumeFX.LiquidVolume>();
        if (ak == null)
        {
            ak = go.AddComponent<LiquidVolumeFX.LiquidVolume>();
        }
        return ak;
    }
    /// <summary>
    /// 得到继承mono的脚本。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetScript<T>(this GameObject go) where T : MonoBehaviour
    {
        T ak = go.GetComponent<T>();
        if (ak == null)
        {
            ak = go.AddComponent<T>();
        }
        return ak;
    }
}
