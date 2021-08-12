using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Match3.Controller
{
    public class ControllerGameOver : MonoBehaviour
    {
        public event Action OnInitializing;
        [SerializeField] private ControllerGameplay gameplayController;
        [SerializeField] private Model.ModelInput modelInput;
        [SerializeField] private Model.ModelGameplay modelGameplay;
        [SerializeField] private View.ViewHUD view;
        private void Awake()
        {
            gameplayController.OnGameOver += EndGame;
        }
        private void OnDestroy()
        {
            gameplayController.OnGameOver -= EndGame;
        }
        public void EndGame()
        {
            StopAnimation();
            modelGameplay.moves = modelGameplay.initialMoves;
            modelInput.allowed = false; 
            StartCoroutine(DeleteGrid());
        }
        void StopAnimation()
        {
            for (int i = 0; i < modelGameplay.Height; i++)
            {
                for (int j = 0; j < modelGameplay.Width; j++)
                {
                    modelGameplay.tokens[i, j].Prefab.GetComponent<Animator>().enabled = false;
                }
            }
        }
        IEnumerator DeleteGrid()
        {
            for (int i = modelGameplay.Height - 1; i >= 0; i--)
            {
                for (int j = modelGameplay.Width - 1; j >= 0; j--)
                {
                    Destroy(modelGameplay.tokens[i, j].Prefab);
                    yield return new WaitForSeconds(modelGameplay.spawnTime);
                }
            }
            yield return null;
            StartCoroutine(RestartGame());
        }
        IEnumerator RestartGame()
        {
            Vector3 scoreTextPos = view.scoreText.rectTransform.anchoredPosition;
            Vector3 scorePos = view.score.rectTransform.anchoredPosition;
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            // canvas.GetComponent<CanvasScaler>().referenceResolution.x/2.5f = half width
            view.scoreText.rectTransform.anchoredPosition = -(new Vector2(canvas.GetComponent<CanvasScaler>().referenceResolution.x / 2.5f, canvas.GetComponent<CanvasScaler>().referenceResolution.y));
            view.score.rectTransform.anchoredPosition = -((new Vector2(canvas.GetComponent<CanvasScaler>().referenceResolution.x / 2.5f, canvas.GetComponent<CanvasScaler>().referenceResolution.y)) - new Vector2(0, view.scoreText.rectTransform.sizeDelta.y));
            yield return new WaitForSeconds(3);
            modelGameplay.score = 0;
            view.score.rectTransform.anchoredPosition = scorePos;
            view.scoreText.rectTransform.anchoredPosition = scoreTextPos;
            OnInitializing?.Invoke();
            yield return null;
        }
    }
}
