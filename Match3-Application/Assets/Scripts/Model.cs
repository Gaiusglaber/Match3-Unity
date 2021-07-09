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
        [SerializeField] public int minMovesAudioPitch = 5;
        [SerializeField] public AudioSource audioSrc;
        [SerializeField] public AudioClip wrongInput, goodInput,selected;
        [SerializeField] [Range(0f, 0.5f)] public float spawnTime;
        [SerializeField] [Range(2, 6)] public int minChainLength;
        [SerializeField] public Vector2 firstTouchPosition;
        [SerializeField] public List<Token> tokensSelection= new List<Token>();
        [SerializeField] public LayerMask layer;
        [SerializeField] public int score=0;
        [SerializeField] [Range(5, 15)] public int scoreMultiplier= 15;
        [SerializeField] public int moves = 10;
        [SerializeField] public bool gameOver = false;
        [SerializeField] public bool control;
        [SerializeField] public bool user=true;
        [SerializeField] public bool IA = false;
        [SerializeField] public bool instantiateFinalised = false;
        [SerializeField] public bool firstInstantiateFinalised = false;
        [SerializeField] public enum TOKEN_TYPE {ORANGE,BLUE,RED,PINK,WHITE,BROWN }
        [SerializeField] [Range(5,8)] public int gridHeight = 10;
        [SerializeField] [Range(3, 5)] public int gridWidth = 10;
        [SerializeField] public Token[,] tokens;
        [SerializeField] public GameObject tilePrefab;
        [SerializeField] public GameObject[] tokenPrefabs;
        private void Start()
        {
            wrongInput = Resources.Load<AudioClip>("WrongInput");
            goodInput = Resources.Load<AudioClip>("GoodInput");
            selected = Resources.Load<AudioClip>("Selected");
            audioSrc = GetComponent<AudioSource>();
            control = user;
            tokens = new Token[gridHeight, gridWidth];
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
            instantiateFinalised = true;
            firstInstantiateFinalised = true;
        }
        void InstantiateToken(int i,int j)
        {
            Token token= new Token();
            token.pos = new Vector2(j, i);
            token.type = (TOKEN_TYPE)Random.Range(0, tokenPrefabs.Length);
            token.prefab = Instantiate(tokenPrefabs[(int)token.type], token.pos, Quaternion.identity);
            token.prefab.transform.parent = view.transform;
            token.prefab.name = i + "," + j;
            tokens[i,j]=token;
        }
    }
}
