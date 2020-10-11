using Bomberman.Character;
using UnityEngine;

namespace Bomberman.Item
{
	public class FuzeItemScript : MonoBehaviour, IItem
	{
		public void ApplyBonus(CharacterScript character)
		{
			character.BombFuze *= 0.9f;
		}
	}
}