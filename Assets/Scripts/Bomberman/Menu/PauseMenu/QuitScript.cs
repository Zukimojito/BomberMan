using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bomberman.Menu.PauseMenu
{
    public class QuitScript : MonoBehaviour
    {
        [SerializeField] public string SceneToLoadQuit;
    
        public void ChangeSceneQuit()
        {
            SceneManager.LoadScene(SceneToLoadQuit);
        }
    }
}
