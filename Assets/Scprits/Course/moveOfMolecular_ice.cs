using DG.Tweening;
using System.Collections;
using UnityEngine;
/// <summary>
/// 冷水的分子运动
/// </summary>
public class moveOfMolecular_ice : MonoBehaviour
{
    public GameObject PrefabMolecular;
    GameObject[] gameObjects;
    Tween[] tweens;
    //Tween tween;
    ////保存所有自身旋转方向的组合（上下左右）
    Vector3[] molecularGroupRotateGrouop;
    ////每个无规则运动的分子的随机的自身旋转方向
    Vector3[] tempAxis;
    /// <summary>
    /// 移动的方向
    /// </summary>
    Vector3 moveDir;
    float delayTime = 3f;
    public int objNum = 40;
    private void Start()
    {
        if (PrefabMolecular == null)
        {
            PrefabMolecular = ResManager.GetPrefab("MicroScene/h2o");
        }
        gameObjects = new GameObject[objNum];
        tweens = new Tween[gameObjects.Length];
        tempAxis = new Vector3[gameObjects.Length];
        Random.InitState((int)Time.realtimeSinceStartup);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i] = GameObject.Instantiate(PrefabMolecular);
            gameObjects[i].transform.SetParent(transform);
            gameObjects[i].transform.localPosition = new Vector3(Random.Range(-0.44f, 0.44f), Random.Range(-0.38f, 0.36f), Random.Range(-0.417f, 0.418f));
            gameObjects[i].transform.localScale = new Vector3(1, 1, 0.5f);
        }
        PrefabMolecular.SetActive(false);
        StartCoroutine(MoveCtrl());
    }

    IEnumerator MoveCtrl()
    {
        WaitForSeconds wf = new WaitForSeconds(delayTime);
        while (true)
        {
            Move();
            yield return wf;
        }
    }
    void Move()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            //随机位置并移动
            Vector3 moveDir = new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.45f, 0.45f));
            Vector3 newPos = gameObjects[i].transform.localPosition + moveDir;
            if (Mathf.Abs(newPos.x) > 0.4f)//X轴边界设定。
            {
                moveDir.x = -moveDir.x;
            }
            //刚开始运动的范围是固定的，小小的
            if (newPos.y < -0.38f || newPos.y > 0.36f)//Y轴边界。
            {
                moveDir.y = -moveDir.y;
            }

            if (newPos.z < -0.417f || newPos.z > 0.35f)//z轴边界。
            {
                moveDir.z = -moveDir.z;
            }
            if (tweens[i] != null)
            {
                tweens[i].WaitForCompletion();//直接结束当前Tween
            }
            Vector3 endPos = gameObjects[i].transform.localPosition + moveDir;//终点坐标

            //Debug.DrawLine(gameObjects[i].transform.position, transform.TransformPoint(endPos), Color.red, 10000);

            tweens[i] = gameObjects[i].transform.DOLocalMove(endPos, delayTime);
            tweens[i].SetEase(Ease.Linear);
            gameObjects[i].transform.DOLocalRotate(new Vector3(0, Random.Range(0, 360), 0), delayTime);
        }

    }
    public void RemoveH2o(int _index)
    {

    }
}
