using Bomberman.Terrain;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Bomberman.Character.MCTS
{
	public class MCTSCharacterController : ICharacterController
	{
		private float _delay;
		private const int TREE_DEPTH = 5;
		
		public RequestedActions Update(CharacterScript character)
		{
			_delay += Time.deltaTime;

			if (_delay <= GameSimulator.TIME_PER_TURN)
				return new RequestedActions();
			
			_delay = 0;

			return SimulateBranch(TREE_DEPTH, new GameState(character)).Item1;
		}

		private Score SimulateLeaf(GameState state)
		{
			while (!state.IsGameFinished())
			{
				GameSimulator.Simulate(state, true);

				if (state.Turn >= 100)
					break;
			}

			return state.CalculateScore();
		}

		private (RequestedActions, Score) SimulateBranch(int remainingDepth, GameState state)
		{
			if (state.IsGameFinished())
			{
				return (null, state.CalculateScore());
			}
			
			Score bestScore = Score.MinValue();

			RequestedActions bestAction = null;
			
			if (state.IsPosWalkable(state.Self.Position + new Vector2Int(1, 0)))
			{
				GameState newState = new GameState(state);
				newState.Self.Position += new Vector2Int(1, 0);
				
				GameSimulator.Simulate(newState, false);

				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions
					{
						Move = new Vector2Int(1, 0)
					};
				}
			}
			
			if (state.IsPosWalkable(state.Self.Position + new Vector2Int(-1, 0)))
			{
				GameState newState = new GameState(state);
				newState.Self.Position += new Vector2Int(-1, 0);
				
				GameSimulator.Simulate(newState, false);
				
				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions
					{
						Move = new Vector2Int(-1, 0)
					};
				}
			}
			
			if (state.IsPosWalkable(state.Self.Position + new Vector2Int(0, 1)))
			{
				GameState newState = new GameState(state);
				newState.Self.Position += new Vector2Int(0, 1);
				
				GameSimulator.Simulate(newState, false);

				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions
					{
						Move = new Vector2Int(0, 1)
					};
				}
			}
			
			if (state.IsPosWalkable(state.Self.Position + new Vector2Int(0, -1)))
			{
				GameState newState = new GameState(state);
				newState.Self.Position += new Vector2Int(0, -1);
				
				GameSimulator.Simulate(newState, false);

				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions
					{
						Move = new Vector2Int(0, -1)
					};
				}
			}

			{
				GameState newState = new GameState(state);

				GameSimulator.Simulate(newState, false);

				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions();
				}
			}
			
			if (state.Self.Bomb == null)
			{
				GameState newState = new GameState(state)
				{
					Self = {Bomb = new BombState(state.Self.BombFuze, state.Self.BombRadius, state.Self.Position)}
				};

				GameSimulator.Simulate(newState, false);

				Score score;
				if (remainingDepth > 0)
				{
					score = SimulateBranch(remainingDepth - 1, newState).Item2;
				}
				else
				{
					score = SimulateLeaf(newState);
				}
				
				if (score >= bestScore)
				{
					bestScore = score;
					bestAction = new RequestedActions
					{
						DropBomb = true
					};
				}
			}

			return (bestAction, bestScore);
		}
	}
}