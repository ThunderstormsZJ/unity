using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameObject[,] spArr;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    public float dropTime;

    private GameModel _model;
    private GameObject pressItemObj;
    private GameObject enterItemObj;
    private List<GameObject> curMatchObjArr;

    private void Awake()
    {
        _instance = this;
        _model = GetComponent<GameModel>();
        Debug.Log("GameManager Awake");
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("GameManager Start");
        int rowCount = _model.m_RowCount;
        int colCount = _model.m_ColCount;
        spArr = new GameObject[rowCount, colCount];
        curMatchObjArr = new List<GameObject>();

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // 底部方块
                Instantiate(_model.getSpPrefabByType(GameModel.SpType.SQUARE), calPos(row, col), Quaternion.identity, transform);
            }
        }

        // 生成障碍
        createNewItems(5, 5, GameModel.SpType.OBSTACLE);
        createNewItems(5, 6, GameModel.SpType.OBSTACLE);
        createNewItems(5, 0, GameModel.SpType.OBSTACLE);

        StartCoroutine(AllDrop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 全部下落
    private IEnumerator AllDrop()
    {
        while (!ItemsDrop())
        {
            // 每一步做一个延迟操作
            Debug.Log("Drop");
            yield return new WaitForSeconds(dropTime);
        }


        // 重新匹配下落
        if (allMatchDestory())
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(AllDrop());
        }
    }

    // 分布下落
    private bool ItemsDrop()
    {
        int rowCount = _model.m_RowCount;
        int colCount = _model.m_ColCount;
        bool isFinish = true;
        // 逐行遍历
        for (int row = 0; row < rowCount; row++)
        {
            // 逐列遍历
            for (int col = 0; col < colCount; col++)
            {
                // 当前元素是否为空
                if (spArr[row, col] == null)
                {
                    if (row < rowCount - 1)
                    {
                        // 查找上一个行的元素是否可以下落
                        if (spArr[row + 1, col] && spArr[row + 1, col].GetComponent<GameSp>().canMove())
                        {
                            // 下移操作
                            spArr[row + 1, col].GetComponent<SpMove>().move(row, col, dropTime);
                            spArr[row, col] = spArr[row + 1, col];
                            spArr[row + 1, col] = null;
                        } else
                        {
                            // 被上方障碍物阻挡
                            // 判断上方是否有障碍物
                            bool isBlocked = false;
                            for (int checkRow = row; checkRow < rowCount; checkRow++)
                            {
                                if (spArr[checkRow, col] != null && !spArr[checkRow, col].GetComponent<GameSp>().canMove())
                                {
                                    isBlocked = true;
                                    break;
                                }
                            }

                            if (isBlocked)
                            {
                                // 左右 斜向填充
                                for (int aboveIndex = -1; aboveIndex <= 1; aboveIndex++)
                                {
                                    int aboveCol = col + aboveIndex;
                                    // 左右两列已经填满
                                    if (aboveIndex != 0)
                                    {
                                        if (aboveCol < 0 || aboveCol >= colCount || spArr[row + 1, aboveCol] == null) continue;
                                        if (!spArr[row + 1, aboveCol].GetComponent<GameSp>().canMove()) continue;

                                        // 先确认前一列已经填满
                                        bool isFill = true;
                                        for (int checkRow = 0; checkRow < rowCount; checkRow++)
                                        {
                                            if (spArr[checkRow, aboveCol] == null)
                                            {
                                                isFill = false;
                                                break;
                                            }
                                        }
                                        if (isFill && spArr[row, col] == null)
                                        {
                                            // 开始斜向填充
                                            GameObject aboveObj = spArr[row + 1, aboveCol];
                                            aboveObj.GetComponent<SpMove>().move(row, col, dropTime);
                                            spArr[row + 1, aboveCol] = null;
                                            spArr[row, col] = aboveObj;
                                        }
                                    }
                                }
                            }
                        }
                    } else
                    {
                        // 最后一行元素为空 直接生成一个元素往下落
                        GameObject newSp = createNewItems(rowCount, col, GameModel.SpType.NORMAL, true);
                        newSp.GetComponent<SpMove>().move(rowCount - 1, col, dropTime);
                        spArr[row, col] = newSp;
                    }

                    isFinish = false;
                }
            }
        }

        return isFinish;
    }

    // 创建新元素
    private GameObject createNewItems(int row, int col, GameModel.SpType type, bool isTemp = false)
    {
        GameObject newSp = Instantiate(_model.getSpPrefabByType(type), calPos(row, col), Quaternion.identity, transform);
        newSp.GetComponent<GameSp>().init(row, col, type, Instance);

        if (!isTemp)
        {
            spArr[row, col] = newSp;
        }
        return newSp;
    }

    // 交换两个元素
    private IEnumerator exchangeItem(GameObject currItem, GameObject otherItem)
    {
        GameSp currSp = currItem.GetComponent<GameSp>();
        GameSp otherSp = otherItem.GetComponent<GameSp>();
        if (currSp.isFriend(otherItem) && currSp.canMove() && otherSp.canMove())
        {
            exchangeMove(currItem, otherItem);

            if (!destoryMatchItems(currItem) && !destoryMatchItems(otherItem))
            {
                yield return new WaitForSeconds(dropTime);

                exchangeMove(currItem, otherItem);
            }else
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(AllDrop());
            }

        }
    }

    // 两个元素移动交换
    private void exchangeMove(GameObject currItem, GameObject otherItem)
    {
        GameSp currSp = currItem.GetComponent<GameSp>();
        GameSp otherSp = otherItem.GetComponent<GameSp>();

        int tempRow = currSp.Row;
        int tempCol = currSp.Col;

        currSp.GetComponent<SpMove>().move(otherSp.Row, otherSp.Col, dropTime);
        otherSp.GetComponent<SpMove>().move(tempRow, tempCol, dropTime);

        spArr[currSp.Row, currSp.Col] = currItem;
        spArr[otherSp.Row, otherSp.Col] = otherItem;
    }

    // 所有元素消除匹配
    private bool allMatchDestory()
    {
        int rowCount = _model.m_RowCount;
        int colCount = _model.m_ColCount;
        bool isDestory = false;
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                // 逐个匹配
                GameObject curObj = spArr[row, col];

                if (destoryMatchItems(curObj))
                {
                    isDestory = true;
                }
            }
        }

        return isDestory;
    }

    /// <summary>
    /// 匹配当前元素周围的销毁项
    /// </summary>
    /// <param name="curObj">当前元素</param>
    /// <returns>是否有销毁项</returns>
    private bool destoryMatchItems(GameObject curObj)
    {
        // 匹配
        bool isDestory = false;
        List<GameObject> destoryItems = stepMath(curObj);
        curMatchObjArr.Clear();

        foreach (GameObject obj in destoryItems)
        {
            GameSp objSp = obj.GetComponent<GameSp>();
            objSp.clear();
            spArr[objSp.Row, objSp.Col] = null;

            isDestory = true;
        }

        return isDestory;
    }

    // 单步匹配元素
    // 返回 需要消除的元素
    private List<GameObject> stepMath(GameObject curObj)
    {
        findMatchItem(curObj);

        List<GameObject> rowList = new List<GameObject>(); // 同行
        List<GameObject> colList = new List<GameObject>(); // 同列
        foreach (GameObject obj in curMatchObjArr)
        {
            rowList.Clear();
            colList.Clear();
            rowList.Add(obj);
            colList.Add(obj);
           
            // 挑选同行 和 同列
            foreach(GameObject tObj in curMatchObjArr)
            {
                if (obj != tObj && obj.GetComponent<GameSp>().Row == tObj.GetComponent<GameSp>().Row)
                {
                    rowList.Add(tObj);
                }

                if (obj != tObj && obj.GetComponent<GameSp>().Col == tObj.GetComponent<GameSp>().Col)
                {
                    colList.Add(tObj);
                }
            }

            if (rowList.Count < 3 && colList.Count < 3) continue;
            else if (rowList.Count >= 3)
            {
                // 排序
                rowList.Sort(
                        delegate (GameObject obj1, GameObject obj2)
                        {
                            return obj1.GetComponent<GameSp>().Col.CompareTo(obj2.GetComponent<GameSp>().Col);
                        }
                    );

                // 这一步是为了检测元素是否为连续的
                for (int x = rowList.Count - 1; x > 0; x--)
                {
                    if (rowList[x].GetComponent<GameSp>().Col - rowList[x-1].GetComponent<GameSp>().Col != 1)
                    {
                        rowList.Remove(rowList[x]);
                    }
                }

                if (rowList.Count >= 3) return rowList; else continue;

            }
            else if (colList.Count >= 3)
            {
                // 操作与上一步类似
                colList.Sort(
                        delegate (GameObject obj1, GameObject obj2)
                        {
                            return obj1.GetComponent<GameSp>().Row.CompareTo(obj2.GetComponent<GameSp>().Row);
                        }
                    );

                for (int x = colList.Count - 1; x > 0; x--)
                {
                    if (colList[x].GetComponent<GameSp>().Row - colList[x - 1].GetComponent<GameSp>().Row != 1)
                    {
                        colList.Remove(colList[x]);
                    }
                }

                if (colList.Count >= 3) return colList; else continue;
            }
        }

        return new List<GameObject>();
    }

    // 遍历该元素周围的元素
    private void findMatchItem(GameObject curObj)
    {
        if (curObj == null) return;
        if (!curObj.GetComponent<GameSp>().canMove()) return;
        if (!curMatchObjArr.Contains(curObj)) curMatchObjArr.Add(curObj);
        else return;

        GameSp curSp = curObj.GetComponent<GameSp>();
        int rowCount = _model.m_RowCount;
        int colCount = _model.m_ColCount;
        // top
        if (curSp.Row + 1 < rowCount &&
            spArr[curSp.Row + 1, curSp.Col] != null &&
            spArr[curSp.Row + 1, curSp.Col].GetComponent<GameSp>().StyleType == curSp.StyleType)
        {
            findMatchItem(spArr[curSp.Row + 1, curSp.Col]);
        }

        // right
        if (curSp.Col + 1 < colCount &&
            spArr[curSp.Row, curSp.Col + 1] != null &&
            spArr[curSp.Row, curSp.Col + 1].GetComponent<GameSp>().StyleType == curSp.StyleType)
        {
            findMatchItem(spArr[curSp.Row, curSp.Col + 1]);
        }

        // left
        if (curSp.Col - 1 >= 0 &&
            spArr[curSp.Row, curSp.Col - 1] != null &&
            spArr[curSp.Row, curSp.Col - 1].GetComponent<GameSp>().StyleType == curSp.StyleType)
        {
            findMatchItem(spArr[curSp.Row, curSp.Col - 1]);
        }

        // down
        if (curSp.Row - 1 >= 0 &&
            spArr[curSp.Row - 1, curSp.Col] != null &&
            spArr[curSp.Row - 1, curSp.Col].GetComponent<GameSp>().StyleType == curSp.StyleType)
        {
            findMatchItem(spArr[curSp.Row - 1, curSp.Col]);
        }
    }


    public void pressItem(GameObject item)
    {
        pressItemObj = item;
    }

    public void enterItem(GameObject item)
    {
        enterItemObj = item;
    }

    public void relaseItem()
    {
        StartCoroutine(exchangeItem(pressItemObj, enterItemObj));

        pressItemObj = null;
        enterItemObj = null;
    }

    public Vector3 calPos(int row, int col)
    {
        float posX = col - _model.m_ColCount / 2 + 0.5f;
        float posY = row - _model.m_RowCount / 2 + 0.5f;
        return new Vector3(posX, posY, 0);
    }
}
