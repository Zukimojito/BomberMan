using UnityEngine;

namespace Bomberman.Menu.MainMenu
{
	public class MainMenuScript : MonoBehaviour
	{
		[SerializeField]
		private GameObject _characterSelectMenu;
		
		[SerializeField]
		private GameObject _optionMenu;

		public void CharacterSelect()
		{
			gameObject.SetActive(false);
			_characterSelectMenu.SetActive(true);
		}
		
		public void Option()
		{
			gameObject.SetActive(false);
			_optionMenu.SetActive(true);
		}
		
		public void Exit()
		{
			Debug.Log("Vous avez quitté le jeu");
			Application.Quit();
		}
	}
}