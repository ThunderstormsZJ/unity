using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    private static GameModel _instance;

    public static GameModel Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    public enum SpType
    {
        EMPTY,
        SQUARE,
        NORMAL,
        OBSTACLE,
    }
    public enum StyleType
    {
        BLUE,
        GREEN,
        PINK,
        PURPLE,
        RED,
        YELLOW,
        COLORS,
    }

    public int m_RowCount; // 行数
    public int m_ColCount; // 列数

    // 所有样式
    [System.Serializable]
    public struct StyleSprite
    {
        public StyleType type;
        public Sprite sprite;
    }

    // 所有包含类型
    [System.Serializable]
    public struct SpPrefab
    {
        public SpType type;
        public GameObject prefab;
        public StyleSprite[] styleSprites; // 可生成的样式
    }
    private Dictionary<SpType, SpPrefab> spPrefabDict;
    public SpPrefab[] spPrefabs;

    public GameObject getSpPrefabByType(SpType type)
    {
        return spPrefabDict[type].prefab;
    }

    public StyleSprite[] getStyleSpritesByType(SpType type)
    {
        return spPrefabDict[type].styleSprites;
    }

    public StyleSprite? getRandomStyleByType(SpType type)
    {
        StyleSprite[] styleSprites = getStyleSpritesByType(type);

        if(styleSprites.Length > 0)
        {
            return styleSprites[Random.Range(0, styleSprites.Length - 1)];
        }

        return null;
    }

    private void Awake()
    {
        _instance = this;

        Debug.Log("GameModel Awake");
        init();
    }

    private void Start()
    {
        Debug.Log("GameModel Start");
    }

    private void init()
    {
        spPrefabDict = new Dictionary<SpType, SpPrefab>();
        foreach (SpPrefab spb in spPrefabs)
        {
            if (!spPrefabDict.ContainsKey(spb.type))
            {
                spPrefabDict.Add(spb.type, spb);
            }
        }
    }
}
