using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {

        bool CheckIfIsMatches()
        {
            bool areMatches = false;
            if (model.instantiateFinalised)
            {
                for (int i = 0; i < model.gridHeight; i++)
                {
                    for (int j = 0; j < model.gridWidth; j++)
                    {
                        model.tokensSelection.Add(model.tokens[i, j]);
                        GoOverGrid(i, j,areMatches);
                    }
                }
                model.instantiateFinalised = false;
            }
            return areMatches;
        }
        private void GoOverGrid(int i,int j,bool areMatches)
        {
            if (i < model.gridHeight-1&& model.tokens[i, j].prefab.CompareTag(model.tokens[i + 1, j].prefab.tag))
            {
                model.tokensSelection.Add(model.tokens[i + 1, j]);
                GoOverGrid(i + 1, j,areMatches);
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                DestroyTokens();
                model.tokensSelection.Clear();
                areMatches = true;
            }
            else if(model.tokensSelection.Count>1)
            {
                model.tokensSelection.Clear();
            }
            if (j < model.gridWidth-1&& model.tokens[i, j].prefab.CompareTag(model.tokens[i, j + 1].prefab.tag))
            {
                model.tokensSelection.Add(model.tokens[i, j + 1]);
                GoOverGrid(i, j + 1,areMatches);
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                DestroyTokens();
                model.tokensSelection.Clear();
                areMatches = true;
            }
            else if (model.tokensSelection.Count > 1)
            {
                model.tokensSelection.Clear();
            }
            else
            {
                model.tokensSelection.Clear();
            }
        }
        private void Update()
        {
            while (!CheckIfIsMatches())
            {

            }
            model.time += Time.deltaTime;
            model.firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapPoint(model.firstTouchPosition, model.layer) && Input.GetMouseButton(0)&&IsSpawnDone())
            {
                GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
                if (!IsOnList(token)&&IsTheSameType(token)&&CanAdd(token)) {
                    AddToList(token);
                    token.GetComponent<SpriteRenderer>().color = Color.red;
                }else if (IsOnList(token) && CanAdd(token))
                {
                    model.tokensSelection[model.tokensSelection.Count-1].prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    model.tokensSelection.RemoveAt(model.tokensSelection.Count-1);
                }
            }else if (Input.GetMouseButtonUp(0))
            {
                if (model.minChainLength <= model.tokensSelection.Count)
                {
                    DestroyTokens();
                    model.moves--;
                    model.score += model.tokensSelection.Count * 15;
                }
                foreach (var token in model.tokensSelection)
                {
                    token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                }
                model.tokensSelection.Clear();
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
        private bool IsSpawnDone()
        {
            return model.time > (model.spawnTime * (model.gridHeight + model.gridWidth));
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
        private bool IsOutsideOfGrid(GameObject tokenObject)
        {
            return ((tokenObject.transform.position.x != 0 || tokenObject.transform.position.x != model.gridWidth) || (tokenObject.transform.position.y != 0 || tokenObject.transform.position.y != model.gridHeight));
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
            foreach (var token in model.tokens)
            {
                foreach (var tokenList in model.tokensSelection)
                {
                    if (token.pos == tokenList.pos)
                    {
                        Destroy(token.prefab);
                        token.prefab=Instantiate(model.tokenPrefabs[(int)InstantiateNewID()],token.pos,Quaternion.identity);
                    }
                }
            }
        }
        /*
        void DeleteUpwards(int i,int j)
        {
            int nullCount = 0;
            while (i< model.gridHeight)
            {
                if (model.tokens[i,j].prefab != null)
                {
                    model.tokens[i,j].pos = new Vector2(j,i-nullCount);
                    model.tokens[i,j].prefab.transform.position = model.tokens[i,j].pos;
                    model.tokens[i,j].prefab.name = i-nullCount + "," + j;
                    model.tokens[i, j] = model.tokens[i - nullCount, j];
                }
                else
                {
                    nullCount++;
                }
                i++;
            }
        }
        void SpawnNewToken(Model.Model.Token tokenDestroyed,int j)
        {
            float yPos = 0;

            tokenDestroyed.prefab = new GameObject();
            tokenDestroyed.pos = new Vector2(tokenDestroyed.pos.x, model.gridHeight - 1);
            tokenDestroyed.type = InstantiateNewID();
            tokenDestroyed.prefab = Instantiate(model.tokenPrefabs[(int)tokenDestroyed.type], tokenDestroyed.pos, Quaternion.identity);
            tokenDestroyed.prefab.name = tokenDestroyed.pos.y + "," + tokenDestroyed.pos.x;
            tokenDestroyed.prefab.transform.parent = view.transform;
            model.tokens[model.gridHeight - 1, j] = tokenDestroyed;
        }*/
    }
}
