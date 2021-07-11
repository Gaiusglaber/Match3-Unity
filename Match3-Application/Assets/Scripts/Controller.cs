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
        void UpdateTokenPos()
        {
            foreach (var token in model.tokens)
            {
                if (token.prefab != null)
                {
                    token.pos = new Vector2(token.prefab.transform.position.x, token.prefab.transform.position.y);
                }
            }
        }
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
        void InstanceNewToken()
        {
            List<Vector2> nullTokensPos = ReturnTokenLogicPosition();
            foreach(var pos in nullTokensPos)
            {
                if (pos.x != -1)
                {
                    model.tokens[(int)pos.x, (int)pos.y].prefab = Instantiate(model.tokenPrefabs[(int)InstantiateNewID()], new Vector2(model.tokens[(int)pos.x, (int)pos.y].pos.x, model.tokens[(int)pos.x, (int)pos.y].pos.y + model.gridHeight), Quaternion.identity);
                    model.tokens[(int)pos.x, (int)pos.y].prefab.transform.parent = view.transform;
                }
            }
        }
        private void CheckIfIsMatches()
        {
            string tag;
            if (StopMoving())
            {
                //Function that check if there are any matches in the scene *no parameters*
                model.instantiateFinalised = false;
                for (int i = 0; i < model.gridHeight; i++)
                {
                    for (int j = 0; j < model.gridWidth; j++)
                    {
                        //Adds every element to the selection list
                        model.tokensSelection.Clear();
                        model.tokensSelection.Add(model.tokens[i, j]);
                        tag = model.tokensSelection[0].prefab.tag;
                        model.tokensSelection[0].toDestroy = true;
                        GoOverGridUp(model.tokens[i, j]);
                        if (model.instantiateFinalised)
                        {
                            for (int t = 0; t < model.tokensSelection.Count; t++)
                            {
                                if (model.tokensSelection[t].prefab.CompareTag(tag) && (model.tokensSelection.Count >= model.minChainLength))
                                {
                                    Destroy(model.tokensSelection[t].prefab);
                                    //The pos is stored if in the future other dev wants to make the tokens fall and using a grid
                                }
                            }
                            if (!model.firstInstantiateFinalised)
                            {
                                //If is not the first game match
                                model.score += model.tokensSelection.Count * model.scoreMultiplier;
                            }
                            model.tokensSelection.Clear();
                            //model.instantiateFinalised = false;
                            return;
                        }
                        model.tokensSelection.Clear();
                        model.tokensSelection.Add(model.tokens[i, j]);
                        tag = model.tokensSelection[0].prefab.tag;
                        model.tokensSelection[0].toDestroy = true;
                        GoOverGridRight(model.tokens[i, j]);
                        if (model.instantiateFinalised)
                        {
                            for (int t = 0; t < model.tokensSelection.Count; t++)
                            {
                                if (model.tokensSelection[t].prefab.CompareTag(tag) && (model.tokensSelection.Count >= model.minChainLength))
                                {
                                    Destroy(model.tokensSelection[t].prefab);
                                    //The pos is stored if in the future other dev wants to make the tokens fall and using a grid
                                }
                            }
                            if (!model.firstInstantiateFinalised)
                            {
                                //If is not the first game match
                                model.score += model.tokensSelection.Count * model.scoreMultiplier;
                            }
                            model.tokensSelection.Clear();
                            //model.instantiateFinalised = false;
                            return;
                        }
                    }
                }
                UpdateTokenPos();
                model.instantiateFinalised = false;
            }
        }
        bool StopMoving()
        {
            bool stoped = true;
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (model.tokens[i, j].prefab != null)
                    {
                        float velocity = model.tokens[i, j].prefab.GetComponent<Rigidbody2D>().velocity.y;
                        if (velocity != 0)
                        {
                            stoped = false;
                            return stoped;
                        }
                    }
                }
            }
            return stoped;
        }
        public void GoOverGridRight(Model.Model.Token tokenToAdd)
        {
            float rayCastDistance = 1f;
            RaycastHit2D hitRight = Physics2D.Raycast(tokenToAdd.prefab.transform.position + new Vector3(0.3f, 0,0), tokenToAdd.prefab.transform.right, rayCastDistance, model.layer);
            //Debug.DrawRay(tokenToAdd.prefab.transform.position + new Vector3(0.3f, 0, 0), tokenToAdd.prefab.transform.right,Color.red);
            if (hitRight.collider != null)
            {
                Model.Model.Token rightToken = LookForToken(hitRight.collider.gameObject);

                if (rightToken.prefab.CompareTag(tokenToAdd.prefab.tag))
                {
                    //Same but in the right one
                    model.tokensSelection.Add(rightToken);
                    GoOverGridRight(rightToken);
                }
                else if (model.minChainLength <= model.tokensSelection.Count)
                {
                    model.instantiateFinalised = true;
                }
                else if (model.tokensSelection.Count < model.minChainLength)
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
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                model.instantiateFinalised = true;
            }
            else //if (model.tokensSelection.Count < model.minChainLength)
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
        public void GoOverGridUp(Model.Model.Token tokenToAdd)
        {
            //model.instantiateFinalised&&stopmoving
            //Recursive function that calls itself whenever every right or up token has the same type of the first one
            //parameters:
            //i=y
            //j=x
            float rayCastDistance=1f;
            //Checks up position
            RaycastHit2D hitUp= Physics2D.Raycast(tokenToAdd.prefab.transform.position + tokenToAdd.prefab.transform.up * rayCastDistance, tokenToAdd.prefab.transform.up, rayCastDistance, model.layer);
            if (hitUp.collider != null)
            {
                Model.Model.Token upToken = LookForToken(hitUp.collider.gameObject);

                if (upToken.prefab.CompareTag(tokenToAdd.prefab.tag))
                {
                    model.tokensSelection.Add(upToken);
                    GoOverGridUp(upToken);
                }
                else if (model.minChainLength <= model.tokensSelection.Count)
                {
                    model.instantiateFinalised = true;
                }
                else
                {
                    model.tokensSelection.Clear();
                }
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                model.instantiateFinalised = true;
            }
            else
            {
                model.tokensSelection.Clear();
            }
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
            if (model.firstInstantiateFinalised)
            {
                if (model.instantiateFinalised)
                {
                    InstanceNewToken();
                    CheckIfIsMatches();
                }
                else
                {
                    InputSelection();
                }
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
            token.pos = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject.transform.position;
            token.type=LookForID(token.prefab);
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
            bool sameY = (int)tokenObject.transform.position.y - (int)model.tokensInput[model.tokensInput.Count - 1].pos.y == 0;
            bool sameX = (int)tokenObject.transform.position.x - (int)model.tokensInput[model.tokensInput.Count - 1].pos.x == 0;
            bool left = (int)tokenObject.transform.position.x - (int)model.tokensInput[model.tokensInput.Count - 1].pos.x == -1;
            bool right = (int)tokenObject.transform.position.x - (int)model.tokensInput[model.tokensInput.Count - 1].pos.x == 1;
            bool up = (int)tokenObject.transform.position.y - (int)model.tokensInput[model.tokensInput.Count - 1].pos.y == 1;
            bool down = (int)tokenObject.transform.position.y - (int)model.tokensInput[model.tokensInput.Count - 1].pos.y == -1;
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
        Model.Model.TOKEN_TYPE LookForID(GameObject token)
        {
            //Returns the id comparing it to the GameObject tag
            //parameters:
            //takenObject= object selected
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
            //Returns a random ID *no parameters*
            return (Model.Model.TOKEN_TYPE)Random.Range(0, model.tokenPrefabs.Length);
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
