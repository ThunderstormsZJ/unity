using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    // 地形生成
    public class MapController: MonoBehaviour
    {

        public TerrainStruct[] Terrains;

        public enum TerrainType : int
        {
            Normal = 1<<0, //1
            Earth = 1<<1,  //2
            BoxNormal = 1<<2,//4
        }

        [System.Serializable]
        public struct TerrainStruct
        {
            public TerrainType type;
            public GameObject prefab;
        }

        // 地形数据
        private int[,] TerrainDatas;
        private Dictionary<TerrainType, GameObject> TerrainKindDict;
        private Terrain[,] TerrainArr;

        private int RowCount = 9;
        private int ColCount = 12;
        private int CellWidth = 1;
        private int CellHeight = 1;

        private BoxController BoxControllerCom;

        private void Awake()
        {
            TerrainArr = new Terrain[RowCount, ColCount];
            TerrainKindDict = new Dictionary<TerrainType, GameObject>();
            TerrainDatas = new int[,]{
                {2,2,2,2,2,2,2,2,2,2,2,2},
                {2,1,1,1,1,1,1,1,1,1,1,2},
                {2,1,1,1,1,1,1,5,1,1,1,2},
                {2,1,1,1,5,5,5,5,5,1,1,2},
                {2,1,1,1,1,1,5,5,1,1,1,2},
                {2,1,1,1,1,1,1,5,1,1,1,2},
                {2,1,1,1,1,1,1,5,1,1,1,2},
                {2,1,1,1,1,1,1,1,1,1,1,2},
                {2,2,2,2,2,2,2,2,2,2,2,2},
            };

            foreach (TerrainStruct ter in Terrains)
            {
                if (!TerrainKindDict.ContainsKey(ter.type))
                {
                    TerrainKindDict[ter.type] = ter.prefab;
                }
            }

            BoxControllerCom = GetComponent<BoxController>();
        }

        // Use this for initialization
        void Start()
        {
            GenerateMap();

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void GenerateMap()
        {
            // 生成基础地形
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColCount; col++)
                {
                    List<TerrainType> types = GetTerrainTypes(TerrainDatas[row, col]);
                    foreach (TerrainType type in types)
                    {
                        if (((IList)BoxControllerCom.boxTypes).Contains(type))
                        {
                            BoxControllerCom.InstantiatePrefab(type, row, col);
                        }
                        else
                        {
                            InstantiatePrefab(type, row, col);
                        }
                    }
                }
            }
        }

        // 根据值获取一个格子中包涵多少类型元素
        private List<TerrainType> GetTerrainTypes(int v)
        {
            List<TerrainType> ttList = new List<TerrainType>();

            foreach(int code in System.Enum.GetValues(typeof(TerrainType)))
            {
                if ((v & code) == code)
                {
                    ttList.Add((TerrainType)System.Enum.ToObject(typeof(TerrainType), code));
                }
            }

            return ttList;
        }

        private GameObject InstantiatePrefab(TerrainType type, int row, int col)
        {
            GameObject normalObj = Instantiate(TerrainKindDict[type], new Vector3(0, 0, 0), Quaternion.identity, transform);
            normalObj.transform.localPosition = GetLocalPos(row, col);

            Terrain terrain = normalObj.GetComponent<Terrain>();
            terrain.Init(row, col, type);

            return normalObj;
        }

        private Vector3 GetLocalPos(int row, int col)
        {
            int totalWidth = CellWidth * ColCount;
            int totalHeight = CellHeight * RowCount;
            float x = col * CellWidth;
            float y = row * CellHeight;

            Vector3 pos = new Vector3(x - totalWidth / 2 + CellWidth / 2, y - totalHeight / 2 + CellHeight / 2, 0);

            return pos;
        }
    }
}

