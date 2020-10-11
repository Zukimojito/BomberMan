using Bomberman.Character;
using UnityEngine;

namespace Bomberman.Item
{
	public class RadiusItemScript : MonoBehaviour, IItem
	{
		public void ApplyBonus(CharacterScript character)
		{
			character.BombRadius += 1;
		}
	}
}