using Bomberman.Character;
using Bomberman.GameManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bomberman.Menu.MainMenu
{
	public class CharacterSelectScript : MonoBehaviour
	{
		[SerializeField]
		private string _sceneToLoad;

		[SerializeField]
		private GameObject _mainMenu;

		[SerializeField]
		private Dropdown _topLeft;
		[SerializeField]
		private Dropdown _topRight;
		[SerializeField]
		private Dropdown _bottomLeft;
		[SerializeField]
		private Dropdown _bottomRight;
    
		public void StartGame()
		{
			if (_topLeft.value + _topRight.value + _bottomLeft.value + _bottomRight.value == 0) return;
			
			GameManagerScript.CharacterConfig = new []
			{
				(ControllerType)_topLeft.value,
				(ControllerType)_topRight.value,
				(ControllerType)_bottomLeft.value,
				(ControllerType)_bottomRight.value
			};
			
			SceneManager.LoadScene(_sceneToLoad);
		}

		public void Back()
		{
			gameObject.SetActive(false);
			_mainMenu.SetActive(true);
		}
	}
}