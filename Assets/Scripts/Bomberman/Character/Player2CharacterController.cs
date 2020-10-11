using UnityEngine;

namespace Bomberman.Character
{
	public class Player2CharacterController : ICharacterController
	{
		public RequestedActions Update(CharacterScript character)
		{
			RequestedActions actions = new RequestedActions();
			
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				actions.Move = new Vector2Int(0, 1);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				actions.Move = new Vector2Int(0, -1);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				actions.Move = new Vector2Int(-1, 0);
			}
			else if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				actions.Move = new Vector2Int(1, 0);
			}

			actions.DropBomb = Input.GetKeyDown(KeyCode.Keypad0);

			return actions;
		}
	}
}