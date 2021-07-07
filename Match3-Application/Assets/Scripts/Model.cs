using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Model
{
    public class Model : ComponentsModelViewController
    {
        public struct Token
        {
            Sprite sprite;
            TOKEN_TYPE type;
        }
        [SerializeField]private enum TOKEN_TYPE {ORANGE,BLUE,RED,PINK,WHITE,BROWN }
        [SerializeField] public int gridHeight = 10;
        [SerializeField] public int gridWidth = 10;
        [SerializeField] public List<Token> tokens= new List<Token>();
        [SerializeField] public GameObject[,] grid;
        [SerializeField] public GameObject tilePrefab;

        private void Start()
        {
            InstantiateGrid();
        }
        private void InstantiateGrid()
        {
            model.grid = new GameObject[model.gridHeight, model.gridWidth];
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    GameObject tile;
                    Vector2 pos = new Vector2(j, i);
                    tile = Instantiate(model.tilePrefab, pos, Quaternion.identity);
                    tile.transform.parent = view.transform;
                }
            }
        }
        private void InstantiateTokens()
        {
            foreach(var element in grid)
            {

            }
        }
    }
}
