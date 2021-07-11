using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
    GameManager class 
    Contains all the app workflow
*/
namespace Match3
{
    public class GameManager : ComponentsModelViewController
    {
        //GameManager act as a controller BUT static thats why it is a singleton
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
            //Ends match and restart variables *no parameters*
            StopAnimation();
            model.moves = model.initialMoves;
            model.instantiateFinalised = false;
            model.firstInstantiateFinalised = false;
            model.audioSrc.pitch = 1;
            StartCoroutine(DeleteGrid());
        }
        void StopAnimation()
        {
            //Stops all token animation *no parameters*
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
            //Coroutine that de-instantiates all tokens in the scene with the spawn animation up-down *no parameters*
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
            //Coroutine that moves the score text and score to the center of the screen while is wating to reset *no parameters*
            Vector3 scoreTextPos = view.scoreText.rectTransform.anchoredPosition;
            Vector3 scorePos= view.score.rectTransform.anchoredPosition;
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            // canvas.GetComponent<CanvasScaler>().referenceResolution.x/2.5f = half width
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
