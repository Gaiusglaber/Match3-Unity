using UnityEngine;

/*
    Controller class 
    Manages Unity’s scene workflow
*/
namespace Match3.Controller
{
    public class Controller : ComponentsModelViewController
    {
        void CheckIfIsMatches()
        {
            //Function that check if there are any matches in the scene *no parameters*
            model.instantiateFinalised = false;
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    if (!model.instantiateFinalised&& model.tokens[i, j].prefab!=null)
                    {
                        //Adds every element to the selection list
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
            //Recursive function that calls itself whenever every right or up token has the same type of the first one
            //parameters:
            //i=y
            //j=x
            if ((model.tokens[i, j].prefab != null) &&i < model.gridHeight-1 && (model.tokens[i + 1, j].prefab != null)&& model.tokens[i, j].prefab.CompareTag(model.tokens[i + 1, j].prefab.tag))
            {
                //Checks up position
                model.tokens[i + 1, j].prefab.GetComponent<SpriteRenderer>().color = Color.red;
                model.tokensSelection.Add(model.tokens[i + 1, j]);
                GoOverGrid(i + 1, j);
            }
            else if (model.minChainLength <= model.tokensSelection.Count)
            {
                //If there are no many tokens up and there are more than the minimum chain combination  it destroys every token in the list 
                DestroyTokens();
                if (!model.firstInstantiateFinalised)
                {
                    //If is not the first game match
                    model.score += model.tokensSelection.Count * model.scoreMultiplier;
                }
                model.tokensSelection.Clear();
                //Found a combination so it has to do it again becouse it spawns random types
                model.instantiateFinalised  = true;
            }
            if ((model.tokens[i, j].prefab != null) && j < model.gridWidth-1 && (model.tokens[i, j + 1].prefab != null)&& model.tokens[i, j].prefab.CompareTag(model.tokens[i, j + 1].prefab.tag))
            {   
                //Same but in the right one
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
            {   //If didnt get a match it clear the list
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
                    model.audioSrc.PlayOneShot(model.selected);
                    token.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if (IsOnList(token) && CanAdd(token))// If is on list and is adyacent
                {
                    //Removes it from list
                    model.tokensSelection[model.tokensSelection.Count - 1].prefab.GetComponent<SpriteRenderer>().color = Color.white;
                    model.tokensSelection.RemoveAt(model.tokensSelection.Count - 1);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //If stops pressing/touching
                if (model.minChainLength <= model.tokensSelection.Count)
                {
                    //If Input is more than the minimum input
                    DestroyTokens();
                    model.instantiateFinalised = true;
                    model.moves--;
                    model.score += model.tokensSelection.Count * model.scoreMultiplier;
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
                foreach (var token in model.tokensSelection)
                {
                    token.prefab.GetComponent<SpriteRenderer>().color = Color.white;
                }
                model.tokensSelection.Clear();
            }
        }
        private void Update()
        {
            if (model.instantiateFinalised)//If there are no matches
            {
                CheckIfIsMatches();
            }
            else if (!model.firstInstantiateFinalised)// If there are no matches and its not instantiating/despawning
            {
                InputSelection();
            }
        } 
        private bool IsOnList(GameObject tokenObject)
        {
            //Returns lambda expression to sort by an element from list and returning if it is on the list
            //parameters:
            //takenObject= object selected
            return model.tokensSelection.Exists(o => o.prefab == tokenObject); 
        }
        private void AddToList()
        {
            //Adds the selected object to list *no parameters*
            Model.Model.Token token = new Model.Model.Token();
            token.prefab = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject;
            token.pos = Physics2D.OverlapPoint(model.firstTouchPosition, model.layer).gameObject.transform.position;
            token.type=LookForID(token.prefab);
            model.tokensSelection.Add(token);
        }
        private bool CanAdd(GameObject tokenObject)
        {
            //Checks if there is the first element of if is adyacent
            //parameters:
            //takenObject= object selected
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
            //Checks if the object selected is adyacent to the last element of the list
            //parameters:
            //takenObject= object selected
            //input= User=true,IA= false
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
        void DestroyTokens()
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
                            model.tokens[i, j].prefab = Instantiate(model.tokenPrefabs[(int)InstantiateNewID()], model.tokens[i, j].pos, Quaternion.identity);
                            model.tokens[i, j].prefab.transform.parent = view.transform;
                        }
                    }
                }
            }
        }
    }
}
