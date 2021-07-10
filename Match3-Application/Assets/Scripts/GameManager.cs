using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Match3
{
    public class GameManager : ComponentsModelViewController
    {
        #region SINGLETON
        static private GameManager instance;
        static public GameManager GetInstance() { return instance; }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        public void EndGame()
        {
            /**/
            model.actualCoroutine = null;
            StopAnimation();
            model.moves = model.initialMoves;
            model.firstInstantiateFinalised = false;
            model.audioSrc.pitch = 1;
            StartCoroutine(DeleteGrid());
        }
        void StopAnimation()
        {
            for (int i = 0; i < model.gridHeight; i++)
            {
                for (int j = 0; j < model.gridWidth; j++)
                {
                    model.tokens[i, j].prefab.GetComponent<Animator>().enabled = false;
                }
            }
        }
        IEnumerator DeleteGrid()
        {
            for (int i = model.gridHeight - 1; i >= 0; i--)
            {
                for (int j = model.gridWidth - 1; j >= 0; j--)
                {
                    Destroy(model.tokens[i, j].prefab);
                    yield return new WaitForSeconds(model.spawnTime);
                }
            }
            yield return null;
            StartCoroutine(RestartGame());
        }
        IEnumerator RestartGame()
        {
            Vector3 scoreTextPos= view.scoreText.rectTransform.anchoredPosition;
            Vector3 scorePos= view.score.rectTransform.anchoredPosition;
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            view.scoreText.rectTransform.anchoredPosition = -(new Vector2(canvas.GetComponent<CanvasScaler>().referenceResolution.x/2.5f,canvas.GetComponent<CanvasScaler>().referenceResolution.y));
            view.score.rectTransform.anchoredPosition = -((new Vector2(canvas.GetComponent<CanvasScaler>().referenceResolution.x / 2.5f, canvas.GetComponent<CanvasScaler>().referenceResolution.y)) - new Vector2(0,view.scoreText.rectTransform.sizeDelta.y));
            yield return new WaitForSeconds(3);
            model.score = 0;
            view.score.rectTransform.anchoredPosition = scorePos;
            view.scoreText.rectTransform.anchoredPosition = scoreTextPos;
            model.LoadData();
            yield return null;
        }
    }
}
