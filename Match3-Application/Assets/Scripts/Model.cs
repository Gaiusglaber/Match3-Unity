using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Match3.Model
{
    public class Model : ComponentsModelViewController
    {

        public class Token
        {
            public GameObject prefab;
            public Vector2 pos;
            public TOKEN_TYPE type;
        }
        [SerializeField] [Range(0f, 0.5f)] public float spawnTime;
        [SerializeField] [Range(2, 6)] public int minChainLength;
        [SerializeField] public Vector2 firstTouchPosition;
        [SerializeField] public List<Token> tokensSelection= new List<Token>();
        [SerializeField] public LayerMask layer;
        [SerializeField] public int score=0;
        [SerializeField] public int moves = 10;
        [SerializeField] public float time;
        [SerializeField] public enum TOKEN_TYPE {ORANGE,BLUE,RED,PINK,WHITE,BROWN }
        [SerializeField] [Range(5,8)] public int gridHeight = 10;
        [SerializeField] [Range(3, 5)] public int gridWidth = 10;
        [SerializeField] public List<Token> tokens=new List<Token>();
        [SerializeField] public GameObject tilePrefab;
        [SerializeField] public GameObject[] tokenPrefabs;
        private void Start()
        {
            InstantiateGrid();
            StartCoroutine(InstantiateTokensNormal());
        }
        private void InstantiateGrid()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    GameObject tile;
                    Vector2 pos = new Vector2(j, i);
                    tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                    tile.transform.parent = view.transform;
                }
            }
        }
        IEnumerator InstantiateTokensNormal()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    InstantiateToken(i,j);
                    yield return new WaitForSeconds(spawnTime);
                }
            }
        }
        void InstantiateToken(int i,int j)
        {
            Token token= new Token();
            token.pos = new Vector2(j, i);
            token.type = (TOKEN_TYPE)Random.Range(0, tokenPrefabs.Length);
            token.prefab = Instantiate(tokenPrefabs[(int)token.type], token.pos, Quaternion.identity);
            token.prefab.transform.parent = view.transform;
            token.prefab.name = i + "," + j;
            tokens.Add(token);
        }
    }
}
