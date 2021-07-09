using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.View 
{ // the view class i understood it as something similar as a UIManager becouse Unity already "draws" all elements in scene 
    public class View : ComponentsModelViewController
    {
        [SerializeField] public TMPro.TMP_Text scoreText; //onlty reference to ui elements
        [SerializeField] public TMPro.TMP_Text movesText;
        private void Update()
        {
            scoreText.text = model.score.ToString();
            movesText.text = model.moves.ToString();
        }
    }
}
