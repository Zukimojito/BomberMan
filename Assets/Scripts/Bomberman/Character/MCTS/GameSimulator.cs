using System;
using System.Collections.Generic;
using System.Linq;
using Bomberman.GameManager;
using Bomberman.Terrain;
using UnityEngine;
using Random = System.Random;

namespace Bomberman.Character.MCTS
{
	public static class GameSimulator
	{
		private static Random _random = new Random((int)~DateTime.Now.Ticks);
		private static GameState _state;

		public const float TIME_PER_TURN = 0.51f;

		public static void Simulate(GameState state, bool simulateSelf)
		{
			_state = state;

			if (simulateSelf)
				SimulateCharacter(_state.Self);
			
			SimulateEnemies();
			CheckBonuses();
			SimulateBombs();

			state.Turn++;
		}

		private static void SimulateEnemies()
		{
			for (int i = 0; i < _state.Characters.Count; i++)
			{
				if (_state.Characters[i] == _state.Self) continue;
				
				SimulateCharacter(_state.Characters[i]);
			}
		}
		
		private static void SimulateCharacter(CharacterState character)
		{
			Vector2Int[] possibleDirections =
			{
				new Vector2Int(1, 0),
				new Vector2Int(-1, 0),
				new Vector2Int(0, 1),
				new Vector2Int(0, -1)
			};

			List<Vector2Int> validDirections = new List<Vector2Int>();

			foreach (Vector2Int direction in possibleDirections)
			{
				Vector2Int pos = character.Position + direction;
				if (_state.IsPosWalkable(pos))
				{
					validDirections.Add(direction);
				}
			}

			int randomValue = _random.Next(validDirections.Count + (character.Bomb == null ? 1 : 0));

			if (randomValue < validDirections.Count)
			{
				character.Position += validDirections[_random.Next(validDirections.Count)];
			}
			else
			{
				character.Bomb = new BombState(character.BombFuze, character.BombRadius, character.Position);
			}
		}

		private static void CheckBonuses()
		{
			for (int i = 0; i < _state.Characters.Count; i++)
			{
				CharacterState character = _state.Characters[i];
				foreach (Vector2Int pos in _state.Bonuses.Keys.ToArray())
				{
					if (pos == character.Position)
					{
						switch (_state.Bonuses[pos])
						{
							case BonusType.Fuze:
								character.BombFuze *= 0.9f;
								break;
							case BonusType.Radius:
								character.BombRadius += 1;
								break;
						}

						_state.Bonuses.Remove(pos);
					}
				}
			}
		}

		private static void SimulateBombs()
		{
			for (int i = 0; i < _state.Characters.Count; i++)
			{
				CharacterState character = _state.Characters[i];
				
				if (character.Bomb == null) continue;

				SimulateBomb(character);
			}
		}

		private static void SimulateBomb(CharacterState character)
		{
			BombState bomb = character.Bomb;

			bomb.RemainingFuze -= TIME_PER_TURN;

			if (bomb.RemainingFuze <= 0)
			{
				int x = bomb.Position.x;
				int y = bomb.Position.y;

				// Center
				character.DestroyedWalls += ExplodeTile(x, y);

				// Right
				for (int i = 1; i < bomb.Radius + 1; i++)
				{
					if (_state.GetTerrainTypeAtPos(x + i, y) == TerrainType.Wall) break;

					character.DestroyedWalls += ExplodeTile(x + i, y);
				}

				// Left
				for (int i = 1; i < bomb.Radius + 1; i++)
				{
					if (_state.GetTerrainTypeAtPos(x - i, y) == TerrainType.Wall) break;

					character.DestroyedWalls += ExplodeTile(x - i, y);
				}

				// Top
				for (int i = 1; i < bomb.Radius + 1; i++)
				{
					if (_state.GetTerrainTypeAtPos(x, y + i) == TerrainType.Wall) break;

					character.DestroyedWalls += ExplodeTile(x, y + i);
				}

				// Bottom
				for (int i = 1; i < bomb.Radius + 1; i++)
				{
					if (_state.GetTerrainTypeAtPos(x, y - i) == TerrainType.Wall) break;

					character.DestroyedWalls += ExplodeTile(x, y - i);
				}

				character.Bomb = null;
			}
		}

		private static int ExplodeTile(int x, int y)
		{
			Vector2Int pos = new Vector2Int(x, y);

			for (int i = 0; i < _state.Characters.Count; i++)
			{
				if (_state.Characters[i].Position == pos)
				{
					if (_state.Characters[i] == _state.Self)
					{
						_state.Self = null;
					}
					
					_state.Characters.RemoveAt(i);
					i--;
				}
			}
			
			if (_state.GetTerrainTypeAtPos(x, y) == TerrainType.BreakableWall)
			{
				_state.Map[x, y] = TerrainType.Floor;

				if (_random.NextDouble() < GameManagerScript.Instance.Map.ItemDropProbability)
				{
					BonusType item = (BonusType)_random.Next(2);
					_state.Bonuses.Add(new Vector2Int(x, y), item);
				}

				return 1;
			}

			return 0;
		}
	}
}