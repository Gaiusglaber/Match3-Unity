using UnityEngine;

/*
    View class 
    Manages the game’s UI
*/
namespace Match3.View 
{
    public class View : ComponentsModelViewController
    {
        [SerializeField] public TMPro.TMP_Text scoreText; //Only reference to ui elements NOT FOR STORING DATA
        [SerializeField] public TMPro.TMP_Text movesText;
        [SerializeField] public TMPro.TMP_Text score;
        [SerializeField] public TMPro.TMP_Text moves;
        [SerializeField] public TMPro.TMP_Text version;
        private void Update()
        {
            scoreText.text = model.score.ToString();
            movesText.text = model.moves.ToString();
            version.text = Application.version;
        }
    }
}
