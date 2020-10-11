using System;
using Bomberman.Character;
using Bomberman.GameManager;
using Bomberman.Terrain;
using UnityEngine;

namespace Bomberman.Bomb
{
	public class BombScript : MonoBehaviour
	{
		public float RemainingFuze { get; private set; }
		public int Radius { get; private set; }
		private Vector2Int _position;
		public Vector2Int Position => _position;
		private bool SoundIsStop = false;
		
		public bool IsReady => !gameObject.activeSelf;
		
		private CharacterScript _owner;
		public void Initialize(CharacterScript owner)
		{
			_owner = owner;
			gameObject.SetActive(false);
		}
		
		

		public void Drop(float fuze, int radius, Vector2Int position)
		{
			if (!IsReady) throw new InvalidOperationException("Cannot drop a bomb already dropped");
			
			RemainingFuze = fuze;
			Radius = radius;
			_position = position;
			
			gameObject.SetActive(true);
			transform.position = new Vector3(position.x, 0, position.y);
			SetSound.PlaySound("BombCharge");
		}
		
		private void Update()
		{
			if (!GameManagerScript.Instance.Running) return;
			
			RemainingFuze -= Time.deltaTime;
			
			
			if (RemainingFuze <= 0)
			{
				Explode();
			}
		}

		private void Explode()
		{
			MapScript map = GameManagerScript.Instance.Map;
			
			// Center
			map.ExplodeTile(_position.x, _position.y);
			
			// Right
			for (int i = 1; i < Radius + 1; i++)
			{
				if (map.GetTerrainTypeAtPos(_position.x + i, _position.y) == TerrainType.Wall) break;

				map.ExplodeTile(_position.x + i, _position.y);
			}
			
			// Left
			for (int i = 1; i < Radius + 1; i++)
			{
				if (map.GetTerrainTypeAtPos(_position.x - i, _position.y) == TerrainType.Wall) break;

				map.ExplodeTile(_position.x - i, _position.y);
			}
			
			// Top
			for (int i = 1; i < Radius + 1; i++)
			{
				if (map.GetTerrainTypeAtPos(_position.x, _position.y + i) == TerrainType.Wall) break;

				map.ExplodeTile(_position.x, _position.y + i);
			}
			
			// Bottom
			for (int i = 1; i < Radius + 1; i++)
			{
				if (map.GetTerrainTypeAtPos(_position.x, _position.y - i) == TerrainType.Wall) break;

				map.ExplodeTile(_position.x, _position.y - i);
			}
			gameObject.SetActive(false);
		}
	}
}