using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpMove : MonoBehaviour {
    private GameSp gameSp;
    private IEnumerator mMoveCoroutine;

    private void Awake()
    {
        gameSp = GetComponent<GameSp>();
    }

    private void Start()
    {
        
    }

    // 移到新的行列
    public void move(int newRow, int newCol, float time)
    {
        if (mMoveCoroutine != null)
        {
            StopCoroutine(mMoveCoroutine);
        }

        mMoveCoroutine = moveCoroutine(newRow, newCol, time);
        StartCoroutine(mMoveCoroutine);
    }

    // 移动协程
    private IEnumerator moveCoroutine(int newRow, int newCol, float time)
    {
        // 更新行列
        gameSp.Row = newRow;
        gameSp.Col = newCol;

        Vector3 startPos = transform.position;
        Vector3 endPos = gameSp.Manager.calPos(newRow, newCol);

        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            gameSp.transform.position = Vector3.Lerp(startPos, endPos, t / time);

            yield return 0;
        }
        gameSp.transform.position = endPos;
    }
}
