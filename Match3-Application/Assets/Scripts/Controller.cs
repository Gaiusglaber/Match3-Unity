using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        private void Update()
        {
            model.firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapPoint(model.firstTouchPosition, model.layer) && Input.GetMouseButton(0)&&!IsOnList()&&IsSpawnDone())
            {
                AddToList();
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
        private bool IsOnList()
        {
            GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            return model.tokensSelection.Exists(o => o == token);
        }
        private void AddToList()
        {
            GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            model.tokensSelection.Add(Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject);
            token.GetComponent<SpriteRenderer>().color= new Color(255,255,255,128);
            Debug.Log("estoyadentro");
        }
        private bool IsSpawnDone()
        {
            model.time += Time.deltaTime;
            return model.time > ((model.spawnTime*2) * (model.gridHeight + model.gridWidth));
        }
        IEnumerator DestroyTokens()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (model.tokensSelection.Exists(o => o == model.tokens[i,j]))
                    {
                        if (model.tokens[i, j] != null)
                        {
                            Destroy(model.tokens[i, j]);
                        }
                    }
                }
            }
            yield return null;
        }
        IEnumerator ArrangeTokens()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (model.tokensSelection.Exists(o => o == model.tokens[i, j]))
                    {
                        for (int t= model.gridHeight - 1; t > i; t--)
                        {
                            model.tokens[t, j].transform.position = model.tokens[t-1, j].transform.position;
                            //model.tokens[t, j] = model.tokens[t - 1, j];
                        }
                        //model.tokens[i, j] = Instantiate(model.tokenPrefab[Random.Range(0, model.tokenPrefab.Length)], model.tokens[model.gridHeight - 2, j].transform.position, Quaternion.identity);
                    }
                }
            }
            yield return null;
        }
    }
}
