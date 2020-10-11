using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bomberman.Menu.VictoryMenu
{
    public class VictoryMenuScript : MonoBehaviour
    {
        [SerializeField]
        private Text victoryTextComponent;
        
        [SerializeField]
        private string _gameScene;
        [SerializeField]
        private string _menuScene;

        public void OpenMenu(string winnerName)
        {
            gameObject.SetActive(true);
        
            // if winnerName == null, draw
            victoryTextComponent.text = winnerName != null ? $"Victory of {winnerName}!" : "Draw";
        }

        public void Restart()
        {
            SceneManager.LoadScene(_gameScene);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(_menuScene);
        }
    }
}
