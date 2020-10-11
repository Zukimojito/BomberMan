using Bomberman.Character;

namespace Bomberman.Item
{
	public interface IItem
	{
		void ApplyBonus(CharacterScript character);
	}
}