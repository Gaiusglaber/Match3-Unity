using UnityEngine;


namespace Match3.View
{
    public class ViewHUD : MonoBehaviour
    {
        [SerializeField] private Model.ModelGameplay modelGameplay;
        [SerializeField] public TMPro.TMP_Text scoreText; //Only reference to ui elements NOT FOR STORING DATA
        [SerializeField] public TMPro.TMP_Text movesText;
        [SerializeField] public TMPro.TMP_Text score;
        [SerializeField] public TMPro.TMP_Text moves;
        [SerializeField] public TMPro.TMP_Text version;
        private void Update()
        {
            scoreText.text = modelGameplay.score.ToString();
            movesText.text = modelGameplay.moves.ToString();
            version.text = Application.version;
        }
    }
}
