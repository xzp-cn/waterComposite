using LiquidVolumeFX;
using UnityEngine;
public class LiquidCtrl : MonoBehaviour
{
    LiquidVolume lv;
    [Range(0.05f, 0.2f)]
    public float speed = 0.1f;
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
    Vector2 limit = new Vector2(0, 1);//最低点和最高点.


    public Vector2 Limit
    {
        get
        {
            return limit;
        }
        set
        {
            if (value.x <= 0)
            {
                value.x = 0;
            }
            if (value.y >= 1)
            {
                value.y = 1;
            }
            limit = value;
            flow = true;
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
            if (Level <= Limit.y)
            {
                Level += Time.deltaTime * speed;
            }
            else
            {
                flow = false;
            }
        }
        else
        {
            if (Level > Limit.x)
            {
                Level -= Time.deltaTime * speed;
            }
            else
            {
                flow = false;
            }
        }
    }

    public void Reset()
    {
        Level = 0;
        flow = false;
    }

    /// <summary>
    /// 设置溶液波动
    /// </summary>
    public void SolutionVibration(float level = 0.7f, float freq = 1f, float speed = 0.6f, params float[] turb)
    {
        //设置溶液波动
        Level = level;
        lv.frecuency = freq;
        lv.speed = speed;
        if (turb.Length >= 2)
        {
            lv.turbulence1 = turb[0];
            lv.turbulence2 = turb[1];
        }
    }
}
