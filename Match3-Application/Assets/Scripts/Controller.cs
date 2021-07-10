using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        private void Start()
        {
            
        }
        void CheckIfIsMatches()
        {
            model.instantiateFinalised = false;
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (!model.instantiateFinalised&& model.tokens[i, j].prefab!=null)
                    {
                        model.tokensSelection.Add(model.tokens[i, j]);
                        model.tokens[i, j].prefab.GetComponent<SpriteRenderer>().color = Color.red;
                        GoOverGrid(i, j);
                    }
                }
            }
            if (!model.instantiateFinalised)
            {
                model.firstInstantiateFinalised = false;
            }
        }
        private void GoOverGrid(int i,int j)
        {
            if ((model.tokens[i, j].prefab != null) &&i < model.gridHeight-1 && (model.tokens[i + 1, j].prefab != null)&& model.tokens[i, j].prefab.CompareTag(model.tokens[i + 1, j].prefab.tag))
            {
                model.tokens[i + 1, j].prefab.GetComponent<SpriteRenderer>().color = Color.red;
                model.tokensSelection.Add(model.tokens[i + 1, j]);
                GoOverGrid(i + 1, j);
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                DestroyTokens();
                if (!model.firstInstantiateFinalised)
                {
                    model.score += model.tokensSelection.Count * model.scoreMultiplier;
                }
                model.tokensSelection.Clear();
                model.instantiateFinalised  = true;
            }
            if ((model.tokens[i, j].prefab != null) && j < model.gridWidth-1 && (model.tokens[i, j + 1].prefab != null)&& model.tokens[i, j].prefab.CompareTag(model.tokens[i, j + 1].prefab.tag))
            {
                model.tokens[i, j+1].prefab.GetComponent<SpriteRenderer>().color = Color.red;
                model.tokensSelection.Add(model.tokens[i, j + 1]);
                GoOverGrid(i, j + 1);
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                DestroyTokens();
                if (!model.firstInstantiateFinalised)
                {
                    model.score += model.tokensSelection.Count * model.scoreMultiplier;
                }
                model.tokensSelection.Clear();
                model.instantiateFinalised = true;
            }
            else if (model.tokensSelection.Count > 1)
            {
                foreach (var token in model.tokensSelection)
                {
                    if (token.prefab != null)
                    {
                        token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                model.tokensSelection.Clear();
            }
            else
            {
                foreach (var token in model.tokensSelection)
                {
                    if (token.prefab != null)
                    {
                        token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                model.tokensSelection.Clear();
            }
        }
        private void InputSelection()
        {
            model.firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapPoint(model.firstTouchPosition, model.layer) && Input.GetMouseButton(0))
            {
                GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
                if (!IsOnList(token) && IsTheSameType(token) && CanAdd(token))
                {
                    AddToList(token);
                    model.audioSrc.PlayOneShot(model.selected);
                    token.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (IsOnList(token) && CanAdd(token))
                {
                    model.tokensSelection[model.tokensSelection.Count - 1].prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    model.tokensSelection.RemoveAt(model.tokensSelection.Count - 1);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (model.minChainLength <= model.tokensSelection.Count)
                {
                    DestroyTokens();
                    model.instantiateFinalised = true;
                    model.moves--;
                    model.score += model.tokensSelection.Count * model.scoreMultiplier;
                    if (model.moves==0)
                    {
                        GameManager.GetInstance().EndGame();
                    }
                    else if (model.moves < model.minMovesAudioPitch)
                    {
                        model.audioSrc.pitch = 1.3f; //if this proyect has a Wwise implementation this would be more smoother
                    }
                    model.audioSrc.PlayOneShot(model.goodInput);
                }
                else
                {
                    model.audioSrc.PlayOneShot(model.wrongInput);
                }
                foreach (var token in model.tokensSelection)
                {
                    token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                }
                model.tokensSelection.Clear();
            }
        }
        private void Update()
        {
            if (model.instantiateFinalised)
            {
                CheckIfIsMatches();
            }
            else if (!model.firstInstantiateFinalised)
            {
                InputSelection();
            }
        } 
        private bool IsOnList(GameObject tokenObject)
        {
            return model.tokensSelection.Exists(o => o.prefab == tokenObject);
        }
        private void AddToList(GameObject tokenObject)
        {
            Model.Model.Token token = new Model.Model.Token();
            token.prefab = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            token.pos = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject.transform.position;
            token.type=LookForID(token.prefab);
            model.tokensSelection.Add(token);
        }
        private bool CanAdd(GameObject tokenObject)
        {
            if (model.tokensSelection.Count == 0)
            {
                return true;
            }
            else if (IsAdyacent(tokenObject,model.user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsAdyacent(GameObject tokenObject,bool input)
        {
            bool sameY = tokenObject.transform.position.y - model.tokensSelection[model.tokensSelection.Count - 1].pos.y == 0;
            bool sameX = tokenObject.transform.position.x - model.tokensSelection[model.tokensSelection.Count - 1].pos.x == 0;
            bool left =tokenObject.transform.position.x - model.tokensSelection[model.tokensSelection.Count - 1].pos.x == -1;
            bool right = tokenObject.transform.position.x - model.tokensSelection[model.tokensSelection.Count - 1].pos.x == 1;
            bool up = tokenObject.transform.position.y - model.tokensSelection[model.tokensSelection.Count - 1].pos.y == 1;
            bool down = tokenObject.transform.position.y - model.tokensSelection[model.tokensSelection.Count - 1].pos.y == -1;
            bool topLeft=left&&up;
            bool topRight=right&&up;
            bool buttomLeft=left&&down;
            bool buttomRight=right&&down;
            if (input) {
                return (left && sameY) || (right && sameY) || (up && sameX) || (down && sameX)||topLeft || topRight || buttomLeft || buttomRight;
            }
            else
            {
                return (left && sameY)|| (right && sameY) || (up && sameX) || (down && sameX);
            }
        }
        private bool IsTheSameType(GameObject tokenObject)
        {
            if (model.tokensSelection.Count == 0)
            {
                return true;
            }
            else
            {
                return tokenObject.CompareTag(model.tokensSelection[0].prefab.tag);
            }
        }
        Model.Model.TOKEN_TYPE LookForID(GameObject token)
        {
            for (int i = 0; i < model.tokenPrefabs.Length; i++)
            {
                if (token.CompareTag(model.tokenPrefabs[i].tag))
                {
                    return (Model.Model.TOKEN_TYPE)i;
                }
            }
            return InstantiateNewID();//default
        }
        Model.Model.TOKEN_TYPE InstantiateNewID()
        {
            return (Model.Model.TOKEN_TYPE)Random.Range(0, model.tokenPrefabs.Length);
        }
        void DestroyTokens()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    for (int t = 0; t < model.tokensSelection.Count; t++)
                    {
                        if (model.tokens[i,j]!=null&&(model.tokens[i, j].prefab.transform.position == model.tokensSelection[t].prefab.transform.position))
                        {
                            Destroy(model.tokens[i, j].prefab);
                            model.tokens[i, j].prefab = Instantiate(model.tokenPrefabs[(int)InstantiateNewID()], model.tokens[i, j].pos, Quaternion.identity);
                            model.tokens[i, j].prefab.transform.parent = view.transform;
                        }
                    }
                }
            }
        }
    }
}
