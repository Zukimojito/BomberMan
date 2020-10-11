using System;
using System.Collections.Generic;
using Bomberman;
using Bomberman.Character;
using Bomberman.GameManager;
using Bomberman.Item;
using UnityEngine;
using Random = System.Random;

namespace Bomberman.Terrain
{
	public class MapScript : MonoBehaviour
	{
		[SerializeField]
		[OddRange(3, 30)]
		private int _width;
		public int Width => _width;
		
		[SerializeField]
		[OddRange(3, 30)]
		private int _height;
		public int Height => _height;

		[SerializeField]
		private GameObject _floorPrefab;
		[SerializeField]
		private GameObject _solidWallPrefab;
		[SerializeField]
		private GameObject _breakableWallPrefab;
		[SerializeField]
		private GameObject _solidWallBorderPrefab;
		[SerializeField]
		private GameObject _solidWallBorderCornerPrefab;
		
		[SerializeField]
		private GameObject _explosionPrefab;
		
		[SerializeField]
		private GameObject[] _itemsPrefab;

		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _breakableWallProbability;
		public float BreakableWallProbability => _breakableWallProbability;
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float _itemDropProbability;
		public float ItemDropProbability => _itemDropProbability;

		private GameObject[,] _mapData;
		public Dictionary<Vector2Int, IItem> Items { get; } = new Dictionary<Vector2Int, IItem>();

		private Random _random;

		private void Awake()
		{
			_mapData = new GameObject[_width+2, _height+2];
			
			
			// Border corners walls
			SetGOAtPos(-1, -1, Instantiate(_solidWallBorderCornerPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform));
			SetGOAtPos(-1, _height, Instantiate(_solidWallBorderCornerPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0), transform));
			SetGOAtPos(_width, _height, Instantiate(_solidWallBorderCornerPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0), transform));
			SetGOAtPos(_width, -1, Instantiate(_solidWallBorderCornerPrefab, Vector3.zero, Quaternion.Euler(0, 270, 0), transform));
			
			// Border walls
			for (int x = 0; x < _width; x++)
			{
				SetGOAtPos(x, -1, Instantiate(_solidWallBorderPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0), transform));
			}
			for (int y = 0; y < _height; y++)
			{
				SetGOAtPos(-1, y, Instantiate(_solidWallBorderPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0), transform));
			}
			for (int x = 0; x < _width; x++)
			{
				SetGOAtPos(x, _height, Instantiate(_solidWallBorderPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0), transform));
			}
			for (int y = 0; y < _height; y++)
			{
				SetGOAtPos(_width, y, Instantiate(_solidWallBorderPrefab, Vector3.zero, Quaternion.Euler(0, 270, 0), transform));
			}

			_random = new Random((int)DateTime.Now.Ticks);
			
			// Inner Elements
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					// Solid walls
					if ((x & 1) == 1 && (y & 1) == 1)
					{
						SetGOAtPos(x, y, Instantiate(_solidWallPrefab, transform));
						continue;
					}

					/*
					 * if ((x == 0 && y == 0) ||
					    (x == _width - 1 && y == 0) ||
					    (x == 0 && y == _height - 1) ||
					    (x == _width - 1 && y == _height - 1))
					 */
					
					// Spawn points
					if (Between(x, 0, 1) && Between(y, 0, 1) ||
					    Between(x, _width - 2, _width - 1) && Between(y, 0, 1) ||
					    Between(x, 0, 1) && Between(y, _height - 2, _height - 1) ||
					    Between(x, _width - 2, _width - 1) && Between(y, _height - 2, _height - 1))
					{
						SetGOAtPos(x, y, Instantiate(_floorPrefab, transform));
						continue;
					}
					
					// Floors or breakable walls
					if (_random.NextDouble() < _breakableWallProbability)
					{
						SetGOAtPos(x, y, Instantiate(_breakableWallPrefab, transform));
					}
					else
					{
						SetGOAtPos(x, y, Instantiate(_floorPrefab, transform));
					}
				}
			}
		}

		public bool CanMoveCharacterToPos(Vector2Int pos)
		{
			return CanMoveCharacterToPos(pos.x, pos.y);
		}
		
		public bool CanMoveCharacterToPos(int x, int y)
		{
			for (int i = 0; i < GameManagerScript.Instance.Characters.Count; i++)
			{
				if (!GameManagerScript.Instance.Characters[i].Bomb.IsReady && 
				    GameManagerScript.Instance.Characters[i].Bomb.Position == new Vector2Int(x, y))
				{
					return false;
				}
			}
			
			return GetTerrainTypeAtPos(x, y) == TerrainType.Floor;
		}

		public TerrainType GetTerrainTypeAtPos(int x, int y)
		{
			GameObject obj = GetGOAtPos(x, y);
			
			if (obj.CompareTag("Solid Wall"))
			{
				return TerrainType.Wall;
			}

			if (obj.CompareTag("Breakable Wall"))
			{
				return TerrainType.BreakableWall;
			}

			if (obj.CompareTag("Floor"))
			{
				return TerrainType.Floor;
			}

			throw new InvalidOperationException("Terrain GameObject does not have a valid tag");
		}

		private GameObject GetGOAtPos(int x, int y)
		{
			EnsurePosIsInBounds(x, y, true);
			
			return _mapData[x + 1, y + 1];
		}
		
		private void SetGOAtPos(int x, int y, GameObject go)
		{
			EnsurePosIsInBounds(x, y, true);

			GameObject currentGO = GetGOAtPos(x, y);
			if (!ReferenceEquals(currentGO, null))
			{
				Destroy(currentGO);
			}
			
			go.transform.position = new Vector3(x, 0, y);
			_mapData[x + 1, y + 1] = go;
		}

		public void TryApplyItemAtPos(int x, int y, CharacterScript character)
		{
			Vector2Int pos = new Vector2Int(x, y);
			
			if (Items.TryGetValue(pos, out IItem item))
			{
				item.ApplyBonus(character);
				Destroy(((MonoBehaviour)item).gameObject);
				Items.Remove(pos);
			}
		}

		public void ExplodeTile(int x, int y)
		{
			EnsurePosIsInBounds(x, y, false);
			
			if (GetTerrainTypeAtPos(x, y) == TerrainType.Wall)
				throw new InvalidOperationException("Cannot destroy non-breakable walls");

			for (int i = 0; i < GameManagerScript.Instance.Characters.Count; i++)
			{
				if (GameManagerScript.Instance.Characters[i].Position == new Vector2Int(x, y))
					GameManagerScript.Instance.Characters[i].gameObject.SetActive(false);
			}

			if (GetTerrainTypeAtPos(x, y) == TerrainType.BreakableWall)
			{
				SetGOAtPos(x, y, Instantiate(_floorPrefab, transform));

				if (_random.NextDouble() < _itemDropProbability)
				{
					IItem item = Instantiate(_itemsPrefab[_random.Next(_itemsPrefab.Length)], new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<IItem>();
					Items.Add(new Vector2Int(x, y), item);
				}
			}

			Instantiate(_explosionPrefab, new Vector3(x, 0.5f, y), Quaternion.identity, transform);
		}

		private void EnsurePosIsInBounds(int x, int y, bool includeBorder)
		{
			if (includeBorder)
			{
				if (!Between(x, -1, _width) || !Between(y, -1, _height))
					throw new InvalidCastException("Position is out of bounds");
			}
			else
			{
				if (!Between(x, 0, _width - 1) || !Between(y, 0, _height - 1))
					throw new InvalidCastException("Position is out of bounds");
			}
		}

		private static bool Between(int value, int min, int max)
		{
			return value >= min && value <= max;
		}
	}
}