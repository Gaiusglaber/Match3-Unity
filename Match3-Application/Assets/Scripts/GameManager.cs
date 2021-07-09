using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            model.score = 0;
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
            yield return new WaitForSeconds(3);
            model.LoadData();
            yield return null;
        }
    }
}
