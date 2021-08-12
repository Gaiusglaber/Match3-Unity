using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/*
    Controller class 
    Manages Unity’s scene workflow
*/
namespace Match3.Controller
{
    public class ControllerGameplay: MonoBehaviour
    {
        public event Action OnInitialize;
        public event Action OnGameOver;
        [SerializeField] private ControllerInput inputController;
        [SerializeField] private ControllerGameOver gameOverController;
        [SerializeField] private Model.ModelInput modelInput;
        [SerializeField] private Model.ModelGameplay modelGameplay;
        public void AddCallBack(Action callBack)
        {
            OnInitialize += callBack;
        }
        public void RemoveCallBack(Action callBack)
        {
            OnInitialize -= callBack;
        }
        void Awake()
        {
            gameOverController.OnInitializing += Init;
            AddCallBack(Init);
        }
        private void Start()
        {
            inputController.OnClicked += AddTokenToList;
            inputController.OnReleased += ClearList;
            InstantiateGrid();
            OnInitialize?.Invoke();
        }
        public void ClearList()
        {
            if (modelGameplay.minChainLength <= modelInput.tokensSelection.Count)
            {
                foreach(var token in modelInput.tokensSelection)
                {
                    Destroy(token.Prefab);
                }
            }
            foreach (var token in modelInput.tokensSelection)
            {
                token.Prefab.GetComponent<SpriteRenderer>().color = Color.white;
            }
            modelGameplay.moves--;
            modelGameplay.score += modelInput.tokensSelection.Count * modelGameplay.scoreMultiplier;
            if (modelGameplay.moves == 0)//Gameover?
            {
                OnGameOver?.Invoke();
            }
            modelInput.tokensSelection.Clear();
        }
        public void AddTokenToList(Model.Token token)
        {
            modelInput.tokensSelection.Add(token);
            ChangeTokenColor(modelInput.tokensSelection[modelInput.tokensSelection.Count - 1]);
        }
        public void ChangeTokenColor(Model.Token token)
        {
            token.Prefab.GetComponent<SpriteRenderer>().color = Color.red;
        }
        public void DeInit()
        {
            inputController.OnClicked -= AddTokenToList;
            inputController.OnReleased -= ClearList;
            StartCoroutine(DeInstantiateTokens());
        }
        private void OnDestroy()
        {
            gameOverController.OnInitializing -= Init;
            RemoveCallBack(Init);
        }
        void Init()
        {
            modelInput.allowed = false;
            InstantiateList();
            switch (modelGameplay.SpawnType)
            {
                case Model.ModelGameplay.SPAWN_TYPE.Normal:
                    StartCoroutine(InstantiateTokensNormal());
                    break;
                case Model.ModelGameplay.SPAWN_TYPE.Advanced:

                    break;
                case Model.ModelGameplay.SPAWN_TYPE.Pro:

                    break;
            }
        }
        void InstantiateToken(int i, int j)
        {
            Model.Token token = new Model.Token();
            token.Type = (Model.Token.TOKEN_TYPE)UnityEngine.Random.Range(0, modelGameplay.maxTokens);
            token.Prefab = Instantiate(modelGameplay.tokenPrefab, new Vector2(j, i), Quaternion.identity);
            InstantiateType(ref token);
            token.Row = i;
            token.Column = j;
            token.Prefab.name = i + "," + j;
            modelGameplay.tokens[i, j] = token;
        }
        public void InstantiateList()
        {
            modelInput.tokensSelection = new List<Model.Token>();
        }
        public void InstantiateType(ref Model.Token token)
        {
            switch (token.Type)
            {
                case Model.Token.TOKEN_TYPE.BLUE:
                    token.Prefab.tag = "Blue";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy2");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy2");
                    break;
                case Model.Token.TOKEN_TYPE.BROWN:
                    token.Prefab.tag = "Brown";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy6");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy6");
                    break;
                case Model.Token.TOKEN_TYPE.ORANGE:
                    token.Prefab.tag = "Orange";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy1");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy1");
                    break;
                case Model.Token.TOKEN_TYPE.PINK:
                    token.Prefab.tag = "Pink";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy4");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy4");
                    break;
                case Model.Token.TOKEN_TYPE.RED:
                    token.Prefab.tag = "Red";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy3");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy3");
                    break;
                case Model.Token.TOKEN_TYPE.WHITE:
                    token.Prefab.tag = "White";
                    token.Prefab.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/Textures/Icons/candy4");
                    token.Prefab.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Art/Animation/Overridecandy4");
                    break;
                default:
                    Debug.LogWarning("Invalid token type!");
                    break;
            }
        }
        private void InstantiateGrid()
        {
            modelGameplay.tokens = new Model.Token[modelGameplay.Height, modelGameplay.Width];
            for (int i = 0; i < modelGameplay.Height; i++)
            {
                GameObject tile;
                for (int j = 0; j < modelGameplay.Width; j++)
                {
                    Vector2 pos = new Vector2(j, i);
                    tile = Instantiate(modelGameplay.tilePrefab, pos, Quaternion.identity);
                    if (i == 0)
                    {
                        tile.AddComponent<EdgeCollider2D>();
                        tile.GetComponent<EdgeCollider2D>().offset = new Vector2(tile.GetComponent<EdgeCollider2D>().offset.x, -0.25f);
                    }
                }
            }
        }
        IEnumerator DeInstantiateTokens()
        {

            for (int i = modelGameplay.Height-1; i >=0; i--)
            {
                for (int j = modelGameplay.Width-1; j >=0; j--)
                {
                    if (modelGameplay.tokens[i, j].Prefab)
                    {
                        Destroy(modelGameplay.tokens[i, j].Prefab);
                    }
                    else
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(modelGameplay.spawnTime);
                }
            }
        }
        IEnumerator InstantiateTokensNormal()
        {
            for (int i = 0; i < modelGameplay.Height; i++)
            {
                for (int j = 0; j < modelGameplay.Width; j++)
                {
                    InstantiateToken(i, j);
                    yield return new WaitForSeconds(modelGameplay.spawnTime);
                }
            }
            modelInput.allowed = true;
        }
    }
}
