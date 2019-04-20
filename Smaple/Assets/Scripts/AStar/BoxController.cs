using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class BoxController : MonoBehaviour {
        public MapController.TerrainType[] boxTypes; // box 包涵的所有类型

        private GameObject[,] boxs;

        private void Awake()
        {

        }
        // Use this for initialization
        void Start () {
            
        }
        
        // Update is called once per frame
        void Update () {
            
        }

        public void InstantiatePrefab(MapController.TerrainType type,int row,int col)
        {

        }

        // 修复层级
        public void FixOrder()
        {

        }
    }
}
