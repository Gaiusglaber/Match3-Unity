using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Match3.Model
{
    public class Model : ComponentsModelViewController
    {
        [SerializeField] [Range(0f, 0.5f)] public float spawnTime;
        [SerializeField] [Range(2, 6)] public int minChainLength;
        [SerializeField] public Vector2 firstTouchPosition;
        [SerializeField] public List<GameObject> tokensSelection= new List<GameObject>();
        [SerializeField] public LayerMask layer;
        [SerializeField] public int score=0;
        [SerializeField] public int moves = 10;
        [SerializeField] public float time;
        [SerializeField]private enum TOKEN_TYPE {ORANGE,BLUE,RED,PINK,WHITE,BROWN }
        [SerializeField][Range(5,8)] public int gridHeight = 10;
        [SerializeField][Range(3, 5)] public int gridWidth = 10;
        [SerializeField] public GameObject[,] tokens;
        [SerializeField] public GameObject[,] grid;
        [SerializeField] public GameObject tilePrefab;
        [SerializeField] public GameObject[] tokenPrefab;

        private void Start()
        {
            grid = new GameObject[model.gridHeight, model.gridWidth];
            tokens = new GameObject[model.gridHeight, model.gridWidth];
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
            GameObject token;
            token = Instantiate(tokenPrefab[Random.Range(0, tokenPrefab.Length)], new Vector2(j, i), Quaternion.identity);
            token.transform.parent = view.transform;
            token.name = i + "," + j;
            tokens[i, j] = token;
        }
    }
}
