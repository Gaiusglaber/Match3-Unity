using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        private void Update()
        {
            model.time += Time.deltaTime;
            model.firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapPoint(model.firstTouchPosition, model.layer) && Input.GetMouseButton(0)&&IsSpawnDone())
            {
                GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
                if (!IsOnList(token)&&IsTheSameType(token)&&IsAdyacent(token)) {
                    AddToList(token);
                }/*
                else if (IsOnList(token))
                {
                    model.tokens.Remove(model.tokens.Find(o => o.prefab == token));
                }*/
            }else if (Input.GetMouseButtonUp(0))
            {
                if (model.minChainLength <= model.tokensSelection.Count)
                {
                    StartCoroutine("DestroyTokens");
                    StartCoroutine("ArrangeTokens");
                    model.moves--;
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
            //Debug.Log("Added");
        }
        private bool IsSpawnDone()
        {
            return model.time > (model.spawnTime * (model.gridHeight + model.gridWidth));
        }
        private bool IsAdyacent(GameObject tokenObject)
        {
            return true;
        }
        private bool IsTheSameType(GameObject tokenObject)
        {
            return true;
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
        IEnumerator DestroyTokens()
        {
            foreach (var token in model.tokens)
            {
                foreach (var tokenList in model.tokensSelection)
                {
                    if (token.pos == tokenList.pos)
                    {
                        Destroy(token.prefab);
                    }
                }
            }
            yield return null;
        }
        IEnumerator ArrangeTokens()
        {
            yield return new WaitForSeconds(0.1f); /* DestroyTokens() is a double for so it takes more miliseconds to get to the next coroutine
                                                            and doesnt get to read the Object that DestroyToken() destroys*/
            int j=-1;
            bool done = false;
            for (int i=0;i< model.tokens.Count; i++)
            {
                if (model.tokens[i].prefab == null)
                {
                    if (!done)
                    {
                        j = i;
                        done = true;
                    }
                    StartCoroutine(DeleteUpwards(model.tokens[i]));
                    StartCoroutine(SpawnNewToken(model.tokens[i]));
                    j = i;
                }
            }
            yield return null;
        }
        IEnumerator DeleteUpwards(Model.Model.Token tokenDestroyed)
        {
            yield return new WaitForSeconds(0.2f);
            int nullCount = 1;
            //toDestroyed.Reverse();
            for (int i=0;i< model.tokens.Count; i++)
            {
                if ((model.tokens[i].pos.x == tokenDestroyed.pos.x) && (model.tokens[i].pos.y > tokenDestroyed.pos.y)&& model.tokens[i].prefab!=null)
                {
                    model.tokens[i].pos = new Vector2(model.tokens[i].pos.x, model.tokens[i].pos.y - 1);
                    model.tokens[i].prefab.transform.position = model.tokens[i].pos;
                    model.tokens[i].prefab.name = model.tokens[i].pos.y + "," + model.tokens[i].pos.x;
                }
            }
            yield return null;
        }
        IEnumerator SpawnNewToken(Model.Model.Token tokenDestroyed)
        {
            yield return new WaitForSeconds(0.4f);
            tokenDestroyed.prefab = new GameObject();
            tokenDestroyed.pos = new Vector2(tokenDestroyed.pos.x, model.gridHeight - 1);
            tokenDestroyed.type = InstantiateNewID();
            tokenDestroyed.prefab = Instantiate(model.tokenPrefabs[(int)tokenDestroyed.type], tokenDestroyed.pos, Quaternion.identity);
            tokenDestroyed.prefab.name = tokenDestroyed.pos.y + "," + tokenDestroyed.pos.x;
            tokenDestroyed.prefab.transform.parent = view.transform;
            yield return null;
        }
    }
}
