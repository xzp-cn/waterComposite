using LiquidVolumeFX;
using UnityEngine;
public class LiquidCtrl : MonoBehaviour
{
    LiquidVolume lv;
    float speed = 0.1f;
    public enum flowDirection//流动方向
    {
        none,
        up,
        down
    }
    public float Level//控制液面高度。
    {
        get
        {
            return lv.level;
        }
        set
        {
            lv.level = value;
        }
    }
    public flowDirection flowDir = flowDirection.none;//设置液体流动方向
    public bool flow = false;
    public Vector3 liquidBounds//设置容器轮廓
    {
        set
        {
            lv.extentsScale = value;
        }
    }

    //public Color32 c1, c2;

    private void Awake()
    {
        if (lv == null)
        {
            lv = gameObject.GetScript<LiquidVolume>();
            Level = 0;
            lv.smokeEnabled = false;
            lv.foamThickness = 0;

            //c1 = c2 = new Color32(112, 170, 218, 255);
            //lv.liquidColor1 = c1;
            //lv.liquidColor2 = c2;
        }

    }

    /// <summary>
    /// 控制水柱流动
    /// </summary>
    private void Update()
    {
        if (!flow)
        {
            return;
        }

        if (flowDir == flowDirection.up)
        {
            if (Level <= 1)
            {
                Level += Time.deltaTime * speed;
            }
        }
        else
        {
            if (Level > 0)
            {
                Level -= Time.deltaTime * speed;
            }
        }
    }

    public void Reset()
    {
        Level = 0;
        flow = false;
    }
}
