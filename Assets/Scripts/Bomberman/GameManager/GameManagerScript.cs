using System;
using System.Collections.Generic;
using Bomberman.Character;
using Bomberman.Character.MCTS;
using Bomberman.Menu.VictoryMenu;
using Bomberman.Terrain;
using UnityEngine;

namespace Bomberman.GameManager
{
    public class GameManagerScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject _mapPrefab;
        [SerializeField]
        private GameObject _characterPrefab;
        [SerializeField]
        private Material[] _characterMaterials;

        public static ControllerType[] CharacterConfig { get; set; }

        public MapScript Map { get; private set; }
        
        public List<CharacterScript> Characters { get; } = new List<CharacterScript>();

        public bool Running { get; set; } = true;

        public static GameManagerScript Instance { get; private set; }
        
        [SerializeField]
        private VictoryMenuScript _victoryMenu;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Map = Instantiate(_mapPrefab).GetComponent<MapScript>();
            
            CreateCharacter(CharacterConfig[0], new Vector2Int(0, Map.Height - 1), "Top Left Character", _characterMaterials[0]);
            CreateCharacter(CharacterConfig[1], new Vector2Int(Map.Width - 1, Map.Height - 1), "Top Right Character", _characterMaterials[1]);
            CreateCharacter(CharacterConfig[2], new Vector2Int(0, 0), "Bottom Left Character", _characterMaterials[2]);
            CreateCharacter(CharacterConfig[3], new Vector2Int(Map.Width - 1, 0), "Bottom Right Character", _characterMaterials[3]);
        }

        private void CreateCharacter(ControllerType type, Vector2Int position, string characterName, Material material)
        {
            if (type == ControllerType.None) return;

            ICharacterController controller = null;
            switch (type)
            {
                case ControllerType.PlayerZQSD:
                    controller = new PlayerCharacterController();
                    break;
                case ControllerType.PlayerKeypad:
                    controller = new Player2CharacterController();
                    break;
                case ControllerType.RandomAI:
                    controller = new RandomCharacterController();
                    break;
                case ControllerType.MCTSAI:
                    controller = new MCTSCharacterController();
                    break;
            }
            
            CharacterScript character = Instantiate(_characterPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity, Map.transform).GetComponent<CharacterScript>();
            character.Initialize(controller, material, characterName);
            Characters.Add(character);
        }

        private void LateUpdate()
        {
            CheckPlayers();
        }

        public void CheckPlayers()
        {
            int aliveCount = 0;
            CharacterScript lastAlive = null;
            for (int i = 0; i < Characters.Count; i++)
            {
                if (Characters[i].gameObject.activeSelf)
                {
                    aliveCount++;
                    lastAlive = Characters[i];
                }
            }

            if (aliveCount == 1)
            {
                Running = false;
                _victoryMenu.OpenMenu(lastAlive.name);
            }
            else if (aliveCount == 0)
            {
                Running = false;
                _victoryMenu.OpenMenu(null);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}