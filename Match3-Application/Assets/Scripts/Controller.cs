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
            return model.tokensSelection.Exists(o => o.prefab == token);
        }
        private void AddToList()
        {
            Model.Model.Token token;
            token.prefab = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            token.pos = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject.transform.position;
            token.type=LookForID(token.prefab);
            model.tokensSelection.Add(token);
            Debug.Log("estoyadentro");
        }
        private bool IsSpawnDone()
        {
            model.time += Time.deltaTime;
            return model.time > ((model.spawnTime) * (model.gridHeight + model.gridWidth));
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
            return (Model.Model.TOKEN_TYPE)0;//default
        }
        IEnumerator DestroyTokens()
        {
            yield return null;
        }
        IEnumerator ArrangeTokens()
        {
            yield return null;
        }
    }
}
