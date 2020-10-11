using UnityEngine;

namespace Bomberman.Character.MCTS
{
	public class BombState
	{
		public float RemainingFuze;
		public float Radius;
		public Vector2Int Position;

		public BombState(float remainingFuze, float radius, Vector2Int position)
		{
			RemainingFuze = remainingFuze;
			Radius = radius;
			Position = position;
		}

		public BombState(BombState other)
		{
			RemainingFuze = other.RemainingFuze;
			Radius = other.Radius;
			Position = other.Position;
		}
	}
}