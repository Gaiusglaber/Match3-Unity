using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/*
    Model class 
    Contains all the game data
*/
namespace Match3.Model
{
    public class Model : ComponentsModelViewController
    {
        /*
            Token class 
            Contains all the token data
            Members:
            - prefab: stores the GameObject reference 
            - pos: contains the position for when the prefab is deleted
            - type: stores the type of each token
        */
        public const int maxTokens = 5;
        public class Token
        {
            public GameObject prefab;
            public TOKEN_TYPE type;
        }
        //Defines minimum movements for when the audio pitch goes up *User config*
        [SerializeField] public int minMovesAudioPitch = 5;
        //Stores the AudioSource component of the model GameObject
        [SerializeField] public AudioSource audioSrc;
        //Stores all sfx 
        [SerializeField] public AudioClip wrongInput, goodInput,selected;
        //Defines the spawn of the token when it spawns/despawns *User config*
        [SerializeField] [Range(0f, 0.5f)] public float spawnTime;
        //Defines the minimum chain combination *User config*
        [SerializeField] [Range(2, 6)] public int minChainLength;
        //It stores the collision between the input and the token
        [SerializeField] public Vector2 firstTouchPosition;
        //It stores the token that the user selected
        [SerializeField] public List<Token> tokensSelection= new List<Token>();

        [SerializeField] public List<Token> tokensInput = new List<Token>();
        //Has the layer of the token for input
        [SerializeField] public LayerMask layer;
        //Stores player score
        [SerializeField] public int score=0;
        //Defines the way that the score increases when sucess a valid combination *User config*
        [SerializeField] [Range(5, 15)] public int scoreMultiplier= 15;
        //Defines initial user movements *User config*
        [SerializeField] public int initialMoves = 10;
        //Has the actual movements
        [SerializeField] public int moves;
        //User input type
        [SerializeField] public bool user=true;
        //IA input type
        [SerializeField] public bool IA = false;
        //Flag for every token instantiaton
        [SerializeField] public bool instantiateFinalised = false;
        //Flag for first token instantiaton
        [SerializeField] public bool firstInstantiateFinalised = false;
        //Defines max token types
        [SerializeField] public bool canMove = false;
        [SerializeField] public enum TOKEN_TYPE {ORANGE,BLUE,RED,PINK,WHITE,BROWN }
        //Defines the amount of columns *User config*
        [SerializeField] [Range(5,8)] public int gridHeight = 10;
        //Defines the amount of rows *User config*
        [SerializeField] [Range(3, 5)] public int gridWidth = 10;
        //Stores all tokens
        [SerializeField] public Token[,] tokens;
        //Stores the tiles placeholder
        [SerializeField] public GameObject tilePrefab;
        //Stores all the token prefabs *User config*
        [SerializeField] public GameObject tokenPrefab;
        private void Start()
        {
            //Instantiate all model data
            InstantiateGrid();
            LoadData();
        }
        public void LoadData()
        {
            //Variables getting their reference *no parameters*
            moves = initialMoves;
            wrongInput = Resources.Load<AudioClip>("WrongInput");
            goodInput = Resources.Load<AudioClip>("GoodInput");
            selected = Resources.Load<AudioClip>("Selected");
            audioSrc = GetComponent<AudioSource>();
            tokens = new Token[gridHeight, gridWidth];
            StartCoroutine(InstantiateTokensNormal());
        }
        private void InstantiateGrid()
        {
            //Instantiate all tiles placeholder in the input of the user *no parameters*
            for (int i = 0; i < model.gridHeight; i++)
            {
                GameObject tile;
                for (int j = 0; j < model.gridWidth; j++)
                {
                    Vector2 pos = new Vector2(j, i);
                    tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                    tile.transform.parent = view.transform;
                    if (i == 0)
                    {
                        tile.AddComponent<EdgeCollider2D>();
                        tile.GetComponent<EdgeCollider2D>().offset = new Vector2(tile.GetComponent<EdgeCollider2D>().offset.x, -0.25f);
                    }
                }
            }
        }
        IEnumerator InstantiateTokensNormal()
        {
            //Coroutine that instantiates all tokens in the scene with the spawn animation down-up *no parameters*
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    InstantiateToken(i,j);
                    yield return new WaitForSeconds(spawnTime);
                }
            }
            //Gives the signal that the instantiation completed
            instantiateFinalised = true;
            firstInstantiateFinalised = true;
        }
        void InstantiateToken(int i,int j)
        {
            //Instantiate each token with a random type 
            //parameters:
            //i=y
            //j=x
            Token token= new Token();
            token.type = (TOKEN_TYPE)Random.Range(0, maxTokens);
            token.prefab = Instantiate(tokenPrefab, new Vector2(j, i), Quaternion.identity);
            InstantiateType(ref token);
            token.prefab.transform.parent = view.transform;
            token.prefab.name = i + "," + j;
            tokens[i,j]=token;
        }
        public void InstantiateType(ref Token token)
        {
            switch (token.type)
            {
                case TOKEN_TYPE.BLUE:
                    token.prefab.tag = "Blue";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy2");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy2");
                    break;
                case TOKEN_TYPE.BROWN:
                    token.prefab.tag = "Brown";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy6");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy6");
                    break;
                case TOKEN_TYPE.ORANGE:
                    token.prefab.tag = "Orange";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy1");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy1");
                    break;
                case TOKEN_TYPE.PINK:
                    token.prefab.tag = "Pink";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy4");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy4");
                    break;
                case TOKEN_TYPE.RED:
                    token.prefab.tag = "Red";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy3");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy3");
                    break;
                case TOKEN_TYPE.WHITE:
                    token.prefab.tag = "White";
                    token.prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy4");
                    token.prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy4");
                    break;
                default:
                    Debug.LogWarning("Invalid token type!");
                    break;
            }
        }
    }
}
