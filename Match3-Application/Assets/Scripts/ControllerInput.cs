using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Match3.Controller
{
    public class ControllerInput : MonoBehaviour
    {
        public Model.ModelInput modelInput;
        public Model.ModelGameplay modelGameplay;
        public event Action<Model.Token> OnClicked;
        public event Action OnReleased;
        private void Awake()
        {
            
        }
        private void OnDestroy()
        {
            
        }
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D token = Physics2D.OverlapPoint(MousePosition, modelInput.layer);
                if (token)
                {
                    SelectToken(token.gameObject, MousePosition);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                DeSelectToken();
            }
        }
        public void SelectToken(GameObject selected,Vector2 mousePosition)
        {
            if (!IsOnList(selected) && IsTheSameType(selected) && IsAdyacent(ReturnTokenFromGameObject(selected), modelInput.allowed)&& modelInput.allowed)
            {
                OnClicked?.Invoke(ReturnTokenFromGameObject(selected));
            }
            else if (IsOnList(selected) && IsAdyacent(ReturnTokenFromGameObject(selected),modelInput.allowed))// If is on list and is adyacent
            {
                //Removes it from list
                modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Prefab.GetComponent<SpriteRenderer>().color = Color.white;
                modelInput.tokensSelection.RemoveAt(modelInput.tokensSelection.Count - 1);
            }
        }
        private bool IsOnList(GameObject selected)
        {
            return modelInput.tokensSelection.Exists(o => o.Prefab == selected);
        }
        private Model.Token ReturnTokenFromGameObject(GameObject selected)
        {
            Model.Token token=null;
            for (int i = 0; i < modelGameplay.Height; i++)
            {
                for (int j = 0; j < modelGameplay.Width; j++)
                {
                    if (selected!=null&&selected.GetInstanceID()== modelGameplay.tokens[i, j].Prefab.GetInstanceID())
                    {
                        token = modelGameplay.tokens[i, j];
                    }
                }
            }
            return token;
        }
        private bool IsTheSameType(GameObject selected)
        {
            if (modelInput.tokensSelection.Count == 0)
            {
                return true;
            }
            else
            {
                return selected.CompareTag(modelInput.tokensSelection[0].Prefab.tag);
            }
        }
        private bool IsAdyacent(Model.Token selected,bool allowed)
        {
            if (modelInput.tokensSelection.Count == 0)
            {
                return true;
            }
            else
            {
                bool sameY = selected.Row - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Row == 0;
                bool sameX = selected.Column - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Column == 0;
                bool left = selected.Column - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Column == -1;
                bool right = selected.Column - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Column == 1;
                bool up = selected.Row - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Row == 1;
                bool down = selected.Row - modelInput.tokensSelection[modelInput.tokensSelection.Count - 1].Row == -1;
                bool topLeft = left && up;
                bool topRight = right && up;
                bool buttomLeft = left && down;
                bool buttomRight = right && down;

                if (allowed)
                {
                    //User input = every adyacent direction
                    return (left && sameY) || (right && sameY) || (up && sameX) || (down && sameX) || topLeft || topRight || buttomLeft || buttomRight;
                }
                else
                {
                    //Automatic input = vertical and horizontal direction
                    return (left && sameY) || (right && sameY) || (up && sameX) || (down && sameX);
                }
            }
        }
        public void DeSelectToken()
        {
            OnReleased?.Invoke();
        }
    }
}
