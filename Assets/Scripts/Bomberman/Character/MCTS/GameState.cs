using System;
using System.Collections.Generic;
using Bomberman.GameManager;
using Bomberman.Item;
using Bomberman.Terrain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bomberman.Character.MCTS
{
	public class GameState
	{
		public TerrainType[,] Map;
		public List<CharacterState> Characters;
		public CharacterState Self;
		public Dictionary<Vector2Int, BonusType> Bonuses;
		public int Turn;

		public GameState(CharacterScript self)
		{
			MapScript originalMap = GameManagerScript.Instance.Map;
			Map = new TerrainType[originalMap.Width, originalMap.Height];
            for (int y = 0; y < originalMap.Height; y++)
            {
            	for (int x = 0; x < originalMap.Width; x++)
            	{
	                Map[x, y] = originalMap.GetTerrainTypeAtPos(x, y);
            	}
            }
            
            List<CharacterScript> originalCharacters = GameManagerScript.Instance.Characters;
            Characters = new List<CharacterState>();
            for (int i = 0; i < originalCharacters.Count; i++)
            {
	            CharacterScript original = originalCharacters[i];
	            
            	BombState bomb = null;
                if (!original.Bomb.IsReady)
                {
	                bomb = new BombState(
		                original.Bomb.RemainingFuze,
		                original.Bomb.Radius,
		                original.Bomb.Position
	                );
                }
                
                CharacterState character = new CharacterState(original.Position, original.BombFuze, original.BombRadius, bomb);
                
                Characters.Add(character);

            	if (original == self)
            	{
            		Self = character;
            	}
            }

            Dictionary<Vector2Int, IItem> originalBonuses = GameManagerScript.Instance.Map.Items;
            Bonuses = new Dictionary<Vector2Int, BonusType>();
            foreach (KeyValuePair<Vector2Int, IItem> pair in originalBonuses)
            {
            	BonusType type;
            	switch (pair.Value)
            	{
            		case FuzeItemScript _:
            			type = BonusType.Fuze;
            			break;
            		case RadiusItemScript _:
            			type = BonusType.Radius;
            			break;
            		default:
            			throw new InvalidOperationException($"Unknown item type: {pair.Value.GetType()}");
            	}

            	Bonuses.Add(pair.Key, type);
            }

            Turn = 0;
		}

		public GameState(GameState other)
		{
			int width = other.Map.GetLength(0);
			int height = other.Map.GetLength(1);
			
			Map = new TerrainType[width, height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Map[x, y] = other.Map[x, y];
				}
			}
			
			Characters = new List<CharacterState>();
			for (int i = 0; i < other.Characters.Count; i++)
			{
				CharacterState copied = new CharacterState(other.Characters[i]);
				
				Characters.Add(copied);
				
				if (other.Characters[i] == other.Self)
					Self = copied;
			}
			
			Bonuses = new Dictionary<Vector2Int, BonusType>();
			foreach (KeyValuePair<Vector2Int, BonusType> pair in other.Bonuses)
			{
				Bonuses.Add(pair.Key, pair.Value);
			}

			Turn = other.Turn;
		}

		public bool IsPosWalkable(int x, int y)
		{
			return IsPosWalkable(new Vector2Int(x, y));
		}
		
		public bool IsPosWalkable(Vector2Int pos)
		{
			for (int i = 0; i < Characters.Count; i++)
			{
				if (Characters[i].Bomb != null && 
				    Characters[i].Bomb.Position == pos)
				{
					return false;
				}
			}
			
			return GetTerrainTypeAtPos(pos) == TerrainType.Floor;
		}

		public TerrainType GetTerrainTypeAtPos(Vector2Int pos)
		{
			return GetTerrainTypeAtPos(pos.x, pos.y);
		}

		public TerrainType GetTerrainTypeAtPos(int x, int y)
		{
			bool Between(int value, int min, int max)
			{
				return value >= min && value <= max;
			}

			if (!Between(x, 0, Map.GetLength(0) - 1) || !Between(y, 0, Map.GetLength(1) - 1))
			{
				return TerrainType.Wall;
			}

			return Map[x, y];
		}
		
		public bool IsGameFinished()
		{
			if (Self == null)
				return true;

			if (Characters.Count <= 1)
				return true;

			return false;
		}
		
		public Score CalculateScore()
		{
			Score score = new Score
			{
				SelfAlive = Self != null,
				Turns = Turn,
				DestroyedWalls = Self?.DestroyedWalls ?? 0,
				CharacterDiff = GameManagerScript.Instance.Characters.Count - Characters.Count
			};
			
			// if (Self != null)
			// {
			// 	score += 1000000;
			// }
			//
			// score -= Turn * 3;
			// score += (GameManagerScript.Instance.Characters.Count - Characters.Count) * 100000;
			// score += Self?.DestroyedWalls * 100 ?? 0;
				
			return score;
		}
	}

	public struct Score
	{
		private bool _minValue;
		public int Value
		{
			get
			{
				if (_minValue)
					return int.MinValue;
				
				int score = 0;
			
				if (SelfAlive)
				{
					score += 1000000;
					score -= Turns * 3;
				}
				else
				{
					score += Turns * 3;
				}
				
				score += CharacterDiff * 100000;
				score += DestroyedWalls * 100;
				
				return score;
			}
		}

		public bool SelfAlive;
		public int Turns;
		public int CharacterDiff;
		public int DestroyedWalls;

		public static Score MinValue()
		{
			return new Score
			{
				_minValue = true
			};
		}

		public static bool operator >(Score left, Score right)
		{
			return left.Value > right.Value;
		}

		public static bool operator <(Score left, Score right)
		{
			return left.Value < right.Value;
		}

		public static bool operator >=(Score left, Score right)
		{
			return left.Value >= right.Value;
		}

		public static bool operator <=(Score left, Score right)
		{
			return left.Value <= right.Value;
		}

		public static bool operator ==(Score left, Score right)
		{
			return left.Value == right.Value;
		}

		public static bool operator !=(Score left, Score right)
		{
			return !(left == right);
		}

		public override string ToString()
		{
			return $"{Value}";
		}
	}
}