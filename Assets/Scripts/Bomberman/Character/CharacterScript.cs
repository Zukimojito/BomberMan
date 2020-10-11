using System;
using Bomberman.Bomb;
using Bomberman.GameManager;
using Bomberman.Terrain;
using UnityEngine;

namespace Bomberman.Character
{
	public class CharacterScript : MonoBehaviour
	{
		private ICharacterController _controller;

		[SerializeField]
		private MeshRenderer _meshRenderer;

		[SerializeField]
		private GameObject _bombPrefab;
		public BombScript Bomb { get; private set; }
		
		
		
		[SerializeField]
		[Range(0f, 5f)]
		private float _bombFuze;
		public float BombFuze
		{
			get => _bombFuze;
			set => _bombFuze = value;
		}
		
		[SerializeField]
		[Range(1, 5)]
		private int _bombRadius;
		public int BombRadius
		{
			get => _bombRadius;
			set => _bombRadius = value;
		}
		
		private Vector2Int _position;
		public Vector2Int Position
		{
			get => _position;
			set
			{
				Vector2Int posDiff = value - _position;
				if (posDiff.sqrMagnitude > 1) throw new InvalidOperationException("Cannot move in diagonal");
				if (posDiff.sqrMagnitude == 0) return;

				if (GameManagerScript.Instance.Map.CanMoveCharacterToPos(value.x, value.y))
				{
					_position = value;
					transform.position = new Vector3(value.x, 0, value.y);
					GameManagerScript.Instance.Map.TryApplyItemAtPos(value.x, value.y, this);
				}
			}
		}

		public void Initialize(ICharacterController controller, Material material, string name)
		{
			_controller = controller;
			
			Material[] materials = _meshRenderer.sharedMaterials;
			materials[8] = material;
			_meshRenderer.sharedMaterials = materials;

			this.name = name;
		}

		private void Start()
		{
			_position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
			Bomb = Instantiate(_bombPrefab, GameManagerScript.Instance.transform).GetComponent<BombScript>();
			Bomb.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (_controller == null) return;
			if (!GameManagerScript.Instance.Running) return;

			RequestedActions actions = _controller.Update(this);

			Position += actions.Move;
			if (actions.Move.sqrMagnitude != 0)
				transform.rotation = Quaternion.LookRotation(new Vector3(actions.Move.x, 0, actions.Move.y));

			if (Bomb.IsReady && actions.DropBomb)
			{
				Bomb.Drop(_bombFuze, _bombRadius, Position);
			}
		}
	}
}
