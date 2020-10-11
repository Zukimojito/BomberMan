using UnityEngine;

namespace Bomberman.Character.MCTS
{
	public class CharacterState
	{
		public Vector2Int Position;
		public float BombFuze;
		public float BombRadius;
		public BombState Bomb;
		public int DestroyedWalls;

		public CharacterState(Vector2Int position, float bombFuze, float bombRadius, BombState bomb)
		{
			Position = position;
			BombFuze = bombFuze;
			BombRadius = bombRadius;
			Bomb = bomb;
			DestroyedWalls = 0;
		}

		public CharacterState(CharacterState other)
		{
			Position = other.Position;
			BombFuze = other.BombFuze;
			BombRadius = other.BombRadius;
			Bomb = other.Bomb != null ? new BombState(other.Bomb) : null;
			DestroyedWalls = other.DestroyedWalls;
		}
	}
}