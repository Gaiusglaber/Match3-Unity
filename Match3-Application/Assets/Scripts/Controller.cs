using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
    Controller class 
    Manages Unity’s scene workflow
*/
namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        List<Vector2> ReturnTokenLogicPosition()
        {
            List<Vector2> vectorList = new List<Vector2>();
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (model.tokens[i, j].prefab == null)
                    {
                        vectorList.Add (new Vector2(i, j));
                    }
                }
            }
            return vectorList;
        }
        void InstanceNewToken(int x)
        {
            List<Vector2> nullTokensPos = ReturnTokenLogicPosition();
            foreach(var pos in nullTokensPos)
            {
                if (pos.x != -1)
                {
                    model.tokens[(int)pos.x, (int)pos.y].prefab = Instantiate(model.tokenPrefab, new Vector2(model.tokens[(int)pos.x, (int)pos.y].prefab.transform.position.x, model.tokens[(int)pos.x, (int)pos.y].prefab.transform.position.y + model.gridHeight), Quaternion.identity);
                    model.InstantiateType(ref model.tokens[(int)pos.x, (int)pos.y]);
                    model.tokens[(int)pos.x, (int)pos.y].prefab.transform.parent = view.transform;
                }
            }
        }
        void CheckDown(int i, int j)
        {
            if (i > 0 && i < model.gridHeight - 1)
            {
                if (model.tokens[i - 1, j].prefab == null)
                {
                    for (int t = i - 1; t >= 0; t--)
                    {
                        if (t - 1 == -1 || model.tokens[t - 1, j].prefab != null)
                        {
                            StartCoroutine(LerpToPosition(i, j, t));
                            return;
                        }
                    }
                }
            }
        }
        IEnumerator LerpToPosition(int i, int j,int t)
        {
            float speed = 1f;
            float time = 0;

            while (time < 0.2f)
            {
                time += Time.deltaTime * speed;
                model.tokens[i, j].prefab.transform.position = Vector2.Lerp(model.tokens[i, j].prefab.transform.position, new Vector2(j, t), time);
                yield return new WaitForEndOfFrame();
            }
            model.tokens[t, j].prefab = model.tokens[i, j].prefab;
            if (i == model.gridHeight - 1)
            {
                model.tokens[i, j].prefab = null;
            }
        }
        private void CheckEmptySpaces()
        {
            //Function that check if there are any matches in the scene *no parameters*
            for (int i = model.gridHeight-1; i >=0; i--)
            {
                for (int j = model.gridWidth-1; j >= 0; j--)
                {
                    CheckDown(i,j);
                }
            }          
        }
        public void GoOverGridRight(Model.Model.Token tokenToAdd)
        {
            
        }
        public void GoOverGridUp(Model.Model.Token tokenToAdd)
        {
            

        }
        private void InputSelection()
        {
            //This function has all logics related to the inputs

            //Gets the mouse/touch position in the game
            model.firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Physics2D.OverlapPoint(model.firstTouchPosition, model.layer) && Input.GetMouseButton(0)) //If youre touchking/clicking a button and holding it down
            {

                GameObject token = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
                if (!IsOnList(token) && IsTheSameType(token) && CanAdd(token)) //Checks if is not on list, is the same type as other elements on the list and is adyacent
                {
                    //Adds it to list
                    AddToList();
                    Debug.Log(model.tokensInput.Count);
                    model.audioSrc.PlayOneShot(model.selected);
                    token.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (IsOnList(token) && CanAdd(token))// If is on list and is adyacent
                {
                    //Removes it from list
                    model.tokensInput[model.tokensInput.Count - 1].prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    model.tokensInput.RemoveAt(model.tokensInput.Count - 1);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //If stops pressing/touching
                if (model.minChainLength <= model.tokensInput.Count)
                {
                    tag = model.tokensInput[0].prefab.tag;
                    //If Input is more than the minimum input
                    for (int t = 0; t < model.tokensInput.Count; t++)
                    {
                        if (model.tokensInput[t].prefab.CompareTag(tag) && (model.tokensInput.Count >= model.minChainLength))
                        {
                            Destroy(model.tokensInput[t].prefab);
                            //The pos is stored if in the future other dev wants to make the tokens fall and using a grid
                        }
                    }
                    model.instantiateFinalised = true;
                    model.moves--;
                    model.score += model.tokensInput.Count * model.scoreMultiplier;
                    if (model.moves==0)//Gameover?
                    {
                        GameManager.GetInstance().EndGame();
                    }
                    else if (model.moves < model.minMovesAudioPitch)
                    {
                        model.audioSrc.pitch = 1.3f; //If this proyect has a Wwise implementation this would be more smoother
                    }
                    model.audioSrc.PlayOneShot(model.goodInput);
                }
                else
                {
                    model.audioSrc.PlayOneShot(model.wrongInput);
                }
                foreach (var token in model.tokensInput)
                {
                    token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                }
                model.tokensInput.Clear();
            }
        }
        private void Update()
        {
            if (model.firstInstantiateFinalised || model.instantiateFinalised)
            {
                CheckEmptySpaces();
            }
            else
            {
                InputSelection();
            }
        } 
        private bool IsOnList(GameObject tokenObject)
        {
            //Returns lambda expression to sort by an element from list and returning if it is on the list
            //parameters:
            //takenObject= object selected
            return model.tokensInput.Exists(o => o.prefab == tokenObject); 
        }
        Model.Model.Token LookForToken(GameObject token)
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (model.tokens[i, j].prefab!=null&&model.tokens[i, j].prefab.transform.position == token.transform.position)
                    {
                        return model.tokens[i, j];
                    }
                }
            }
            return model.tokens[0, 0];
        }
        private void AddToList()
        {
            //Adds the selected object to list *no parameters*
            Model.Model.Token token = new Model.Model.Token();
            token.prefab = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            model.tokensInput.Add(token);
        }
        private bool CanAdd(GameObject tokenObject)
        {
            //Checks if there is the first element of if is adyacent
            //parameters:
            //takenObject= object selected
            if (model.tokensInput.Count == 0)
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
            //Checks if the object selected is adyacent to the last element of the list
            //parameters:
            //takenObject= object selected
            //input= User=true,IA= false
            bool sameY = tokenObject.transform.position.y - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.y == 0;
            bool sameX = tokenObject.transform.position.x - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.x == 0;
            bool left = tokenObject.transform.position.x - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.x == -1;
            bool right = tokenObject.transform.position.x - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.x == 1;
            bool up = tokenObject.transform.position.y - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.y == 1;
            bool down = tokenObject.transform.position.y - model.tokensInput[model.tokensInput.Count - 1].prefab.transform.position.y == -1;
            bool topLeft=left&&up;
            bool topRight=right&&up;
            bool buttomLeft=left&&down;
            bool buttomRight=right&&down;
            if (input) {
                //User input = every adyacent direction
                return (left && sameY) || (right && sameY) || (up && sameX) || (down && sameX)||topLeft || topRight || buttomLeft || buttomRight;
            }
            else
            {
                //IA input = vertical and horizontal direction
                return (left && sameY)|| (right && sameY) || (up && sameX) || (down && sameX);
            }
        }
        private bool IsTheSameType(GameObject tokenObject)
        {
            //Compares tags between the first element on the list and the selected one
            //parameters:
            //takenObject= object selected
            if (model.tokensInput.Count == 0)
            {
                return true;
            }
            else
            {
                return tokenObject.CompareTag(model.tokensInput[0].prefab.tag);
            }
        }
        Model.Model.TOKEN_TYPE InstantiateNewID()
        {
            //Returns a random ID *no parameters*
            return (Model.Model.TOKEN_TYPE)Random.Range(0, Model.Model.maxTokens);
        }
        /*void DestroyTokens()
        {

            //Destroys the tokens that are on the list and reinstantiate them *no parameters*
            for (int i = 0; i < model.gridHeight; i++)//Every y
            {
                for (int j = 0; j < model.gridWidth; j++)//Every x
                {

                    for (int t = 0; t < model.tokensSelection.Count; t++)//Every element on list
                    {
                        if (model.tokens[i,j]!=null&&(model.tokens[i, j].prefab.transform.position == model.tokensSelection[t].prefab.transform.position))
                        {//If the element on the list has the same position* of the element on the grid 
                            //* I tryied to GameObject==GameObject but it doesnt have the proper references so i checked for position
                            Destroy(model.tokens[i, j].prefab);
                            //The pos is stored if in the future other dev wants to make the tokens fall and using a grid
                            model.tokens[i, j].prefab = Instantiate(model.tokenPrefabs[(int)InstantiateNewID()], new Vector2(model.tokens[i, j].pos.x, model.tokens[i, j].pos.y+model.gridHeight), Quaternion.identity);
                            model.tokens[i, j].prefab.transform.parent = view.transform;
                        }
                    }
                }
            }
        }*/
    }
}
