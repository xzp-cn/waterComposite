using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tag Demo Move Control
/// </summary>
public class MoveControl : MonoBehaviour {

    public Transform redCube0, blueCube1, whiteCube2;

    public void RedMove() { StartCoroutine("ContuniusRedCubeMove"); }
    public void BlueMove() { StartCoroutine("ContuniusBlueCubeMove"); }
    public void WhiteMove() { StartCoroutine("ContuniusWhiteCubeMove"); }

    IEnumerator ContuniusRedCubeMove()
    {
        while(redCube0.localPosition.x > -2)
        {
            redCube0.localPosition -= new Vector3(0.1f, 0, 0);
            yield return null;
        }

        while (redCube0.localPosition.x < 0)
        {
            redCube0.localPosition += new Vector3(0.1f, 0, 0);
            yield return null;
        }
    }

    IEnumerator ContuniusBlueCubeMove()
    {
        while (blueCube1.localPosition.x > -2)
        {
            blueCube1.localPosition -= new Vector3(0.1f, 0, 0);
            yield return null;
        }

        while (blueCube1.localPosition.x < 0)
        {
            blueCube1.localPosition += new Vector3(0.1f, 0, 0);
            yield return null;
        }
    }

    IEnumerator ContuniusWhiteCubeMove()
    {
        while (whiteCube2.localPosition.x > -2)
        {
            whiteCube2.localPosition -= new Vector3(0.1f, 0, 0);
            yield return null;
        }

        while (whiteCube2.localPosition.x < 0)
        {
            whiteCube2.localPosition += new Vector3(0.1f, 0, 0);
            yield return null;
        }
    }
}
