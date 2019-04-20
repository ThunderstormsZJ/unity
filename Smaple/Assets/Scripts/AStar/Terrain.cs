using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Terrain : MonoBehaviour 
    {

        private int row;
        private int col;
        private MapController.TerrainType type;

        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }
        public int Col
        {
            get
            {
                return col;
            }

            set
            {
                col = value;
            }
        }
        public MapController.TerrainType Type
        {
            get
            {
                return type;
            }
        }

        public void Init(int _row, int _col, MapController.TerrainType _type)
        {
            row = _row;
            col = _col;
            type = _type;
        }
        
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

