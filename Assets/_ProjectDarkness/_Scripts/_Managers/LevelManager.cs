using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        public event Action OnRoomTransitionStart;
        public event Action OnRoomTransitionEnd;
        public event Action OnRoomSetActive;
        public event Action<Vector2Int> OnRoomSpawned;

        [Header("Level Creation")]
        [SerializeField] private int _level = 1;
        [SerializeField] private int _roomLength = 22;
        public int RoomLength => _roomLength;

        [SerializeField] private int _floorPlanGridXLength = 8;
        [SerializeField] private int _floorPlanGridYLength = 9;
        
        [Header("Room")]
        [SerializeField] private Room _startingRoomPrefab;
        [SerializeField] private Room _bossRoomPrefab;
        [SerializeField] private RoomPoolData _combatRoomPool;
        
        [Header("UI")]
        [SerializeField] private TransitionPanelUI _transitionPanelUI;
        [SerializeField] private float _fadeToBlackDuration = 0.5f;
        [SerializeField] private float _fadeToClearDuration = 0.5f;

        private Vector2Int _startPos = new();
        public Vector2Int StartPos => _startPos;
        
        private Dictionary <Vector2Int, RoomEntry> _floorPlan;
        public Dictionary<Vector2Int, RoomEntry> FloorPlan => _floorPlan;
        
        private Dictionary<Vector2Int, Room> _spawnedRooms = new();
        public Dictionary<Vector2Int, Room> SpawnedRooms => _spawnedRooms;
        
        private RoomEntry _currentActiveRoomEntry;
        public RoomEntry CurrentActiveRoomEntry => _currentActiveRoomEntry;
        
        private bool _isTransitioning;
        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            Instance = this;

            _floorPlan = new Dictionary<Vector2Int, RoomEntry>();
        }
        
        private void Start()
        {
            if(GameManager.Instance != null && GameManager.Instance.GameStarted)
            {
                GameManager.Instance.GameStarted = false;
                GenerateLevel();
            }
        }

        #region Level Generation Functions

        [Button("Generate Level")]
        private void GenerateLevel()
        {
            DestroyRooms();
            GenerateFloorPlanData();
            GenerateConnectionData();
            GenerateRoomTypeData();
            GenerateRooms();
        }

        private void GenerateFloorPlanData()
        {
            _startPos = new Vector2Int(_floorPlanGridXLength / 2, _floorPlanGridYLength / 2);

            int numberOfRooms = Mathf.FloorToInt(UnityEngine.Random.Range(0, 2) + 5 + _level * 2.6f);

            int maxRetries = 2000;
            int retries = 0;
            while (retries < maxRetries)
            {
                _floorPlan = new Dictionary<Vector2Int, RoomEntry>
                {
                    { _startPos, new RoomEntry(_startPos) }
                };

                Queue<Vector2Int> queue = new Queue<Vector2Int>();
                queue.Enqueue(_startPos);

                int reseedCount = 0;

                Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

                while (queue.Count > 0 && _floorPlan.Count < numberOfRooms)
                {
                    Vector2Int current = queue.Dequeue();

                    // Shuffle directions to prevent directional bias
                    for (int i = 0; i < directions.Length; i++)
                    {
                        Vector2Int temp = directions[i];
                        int randomIndex = UnityEngine.Random.Range(i, directions.Length);
                        directions[i] = directions[randomIndex];
                        directions[randomIndex] = temp;
                    }

                    foreach (Vector2Int dir in directions)
                    {
                        if (_floorPlan.Count >= numberOfRooms)
                            break;

                        Vector2Int neighbor = current + dir;

                        // Bounds check 
                        if (neighbor.x < 0 || neighbor.x >= _floorPlanGridXLength || neighbor.y < 0 || neighbor.y >= _floorPlanGridYLength)
                            continue;

                        // If already occupied, give up
                        if (_floorPlan.ContainsKey(neighbor))
                            continue;

                        // If neighbor has more than 1 filled neighbor, give up (prevents loops/blocks of rooms)
                        int filledNeighbors = 0;
                        foreach (Vector2Int checkDir in directions)
                        {
                            if (_floorPlan.ContainsKey(neighbor + checkDir))
                            {
                                filledNeighbors++;
                            }
                        }

                        if (filledNeighbors > 1)
                            continue;

                        // Random 50% chance to give up
                        if (UnityEngine.Random.value < 0.5f)
                            continue;

                        // Mark neighbor and add to queue
                        _floorPlan.Add(neighbor, new RoomEntry(neighbor));
                        queue.Enqueue(neighbor);
                    }

                    // Reseed queue if it empties before reaching numberOfRooms
                    if (queue.Count == 0 && _floorPlan.Count < numberOfRooms)
                    {
                        reseedCount++;
                        if (reseedCount > 20) break; // Break out to force a completely new retry

                        // The original Isaac periodically reseeds the start room
                        // but random reseeding from existing rooms is a robust fallback
                        List<Vector2Int> allRooms = new List<Vector2Int>(_floorPlan.Keys);
                        queue.Enqueue(allRooms[UnityEngine.Random.Range(0, allRooms.Count)]);
                    }
                }

                // Floorplan consistency checks
                if (_floorPlan.Count < numberOfRooms)
                {
                    continue; // Retry from the start
                }

                Debug.Log($"Generated Room Level with {_floorPlan.Count} rooms in {retries} attempts.");
                break;
            }
        }

        private void GenerateConnectionData()
        {
            foreach (var kvp in _floorPlan)
            {
                Vector2Int pos = kvp.Key;
                RoomEntry room = kvp.Value;

                Vector2Int northPos = pos + Vector2Int.up;
                room.NorthDoorConnection.UpdateData(_floorPlan.ContainsKey(northPos), _floorPlan.ContainsKey(northPos) ? _floorPlan[northPos] : null);

                Vector2Int southPos = pos + Vector2Int.down;
                room.SouthDoorConnection.UpdateData(_floorPlan.ContainsKey(southPos), _floorPlan.ContainsKey(southPos) ? _floorPlan[southPos] : null);

                Vector2Int eastPos = pos + Vector2Int.right;
                room.EastDoorConnection.UpdateData(_floorPlan.ContainsKey(eastPos), _floorPlan.ContainsKey(eastPos) ? _floorPlan[eastPos] : null);

                Vector2Int westPos = pos + Vector2Int.left;
                room.WestDoorConnection.UpdateData(_floorPlan.ContainsKey(westPos), _floorPlan.ContainsKey(westPos) ? _floorPlan[westPos] : null);
            }
        }

        private void GenerateRoomTypeData()
        {
            Dictionary<Vector2Int, RoomEntry> deadEnds = new();
        
            foreach (var kvp in _floorPlan)
            {
                if(kvp.Value.RoomCoord == _startPos)
                {
                    kvp.Value.SetRoomType(RoomType.StartingRoom);
                    continue;
                }
                else if(RoomIsDeadEnd(kvp.Value))
                {
                    deadEnds.Add(kvp.Key, kvp.Value);
                }
                
                kvp.Value.SetRoomType(RoomType.CombatRoom);
            }
            
            // Find the furthest dead end and make it always a boss room
            RoomEntry furthestDeadEnd = null;
            float maxDistance = -1f;
            
            foreach (var kvp in deadEnds)
            {
                Vector2Int deadEndPos = kvp.Key;
                RoomEntry deadEnd = kvp.Value;

                float distance = (deadEndPos - _startPos).sqrMagnitude;
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestDeadEnd = deadEnd;
                }
            }

            furthestDeadEnd?.SetRoomType(RoomType.BossRoom);         
        }

        private bool RoomIsDeadEnd(RoomEntry value)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
            
            int connections = 0;
            
            foreach (Vector2Int dir in directions)
            {
                if(_floorPlan.ContainsKey(value.RoomCoord + dir))
                {
                    connections++;
                }
            }
            
            return connections == 1;
        }

        private void GenerateRooms()
        {
            foreach (var kvp in _floorPlan)
            {
                SpawnRoom(kvp.Key);
            }
            
            SetActiveRoom(_floorPlan[_startPos]);
        }

        #endregion

        #region Helper Functions

        public void TransitionRoom(RoomEntry targetRoomEntry, CardinalDirection comingFromDirection)
        {
            StartCoroutine(TransitionRoutine(targetRoomEntry, comingFromDirection));
        }

        private IEnumerator TransitionRoutine(RoomEntry targetRoomEntry, CardinalDirection comingFromDirection)
        {
            _isTransitioning = true;
            OnRoomTransitionStart?.Invoke();
            
            yield return _transitionPanelUI.FadeToBlack(_fadeToBlackDuration).WaitForCompletion();

            SetActiveRoom(targetRoomEntry);
            
            switch(comingFromDirection)
            {
                case CardinalDirection.North:
                    Player.Instance.Movement.Teleport(_spawnedRooms[_currentActiveRoomEntry.RoomCoord].SouthWall.OnEnterPlacementPoint.transform.position, Quaternion.LookRotation(Vector3.forward));
                    break;
                case CardinalDirection.South:
                    Player.Instance.Movement.Teleport(_spawnedRooms[_currentActiveRoomEntry.RoomCoord].NorthWall.OnEnterPlacementPoint.transform.position, Quaternion.LookRotation(Vector3.back));
                    break;
                case CardinalDirection.East:
                    Player.Instance.Movement.Teleport(_spawnedRooms[_currentActiveRoomEntry.RoomCoord].WestWall.OnEnterPlacementPoint.transform.position, Quaternion.LookRotation(Vector3.right));
                    break;
                case CardinalDirection.West:
                    Player.Instance.Movement.Teleport(_spawnedRooms[_currentActiveRoomEntry.RoomCoord].EastWall.OnEnterPlacementPoint.transform.position, Quaternion.LookRotation(Vector3.left));
                    break;
            }
            
            yield return _transitionPanelUI.FadeToClear(_fadeToClearDuration).WaitForCompletion();

            OnRoomTransitionEnd?.Invoke();
            _isTransitioning = false;
        }

        private void SpawnRoom(Vector2Int roomCoord)
        {
            if (_spawnedRooms.ContainsKey(roomCoord))
            {
                Debug.LogError($"Room at {roomCoord} cannot be spawned because it already is spawned");
                return;
            }

            float worldX = (roomCoord.x - _startPos.x) * _roomLength;
            float worldZ = (roomCoord.y - _startPos.y) * _roomLength;
            Vector3 worldPos = new Vector3(worldX, 0f, worldZ);

            Room newRoom = Instantiate(GetRoomPrefab(roomCoord), worldPos, Quaternion.identity);
            newRoom.Initialize(_floorPlan[roomCoord]);
            newRoom.gameObject.SetActive(false);

            _spawnedRooms.Add(roomCoord, newRoom);
            OnRoomSpawned?.Invoke(roomCoord);
        }
        
        private Room GetRoomPrefab(Vector2Int roomCoord)
        {
            RoomEntry room = _floorPlan[roomCoord];

            switch(room.RoomType)
            {
                case RoomType.StartingRoom:
                    return _startingRoomPrefab;
                case RoomType.CombatRoom:
                    return _combatRoomPool.GetRandomRoomFromPool();
                case RoomType.BossRoom:
                    // Falling back to combat room until you have a Boss prefab
                    return _bossRoomPrefab;
                case RoomType.Shoproom:
                    break;
            }
            
            Debug.LogError($"No room found to spawn. Should be impossible to see this message.");
            return null;
        }

        private void DestroyRooms()
        {
            foreach (var kvp in _spawnedRooms)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }

            _spawnedRooms.Clear();
        }

        private void SetActiveRoom(RoomEntry incomingRoomEntry)
        {
            if(_currentActiveRoomEntry != null && _currentActiveRoomEntry != incomingRoomEntry)
            {
                SetRoomActive(_currentActiveRoomEntry.RoomCoord, false);
            }
        
            _currentActiveRoomEntry = incomingRoomEntry;
            SetRoomActive(incomingRoomEntry.RoomCoord, true);

            OnRoomSetActive?.Invoke();
        }
        
        private void SetRoomActive(Vector2Int roomCoord, bool isActive)
        {
            Room room = _spawnedRooms[roomCoord];
            
            if(isActive)
            {
                room.gameObject.SetActive(isActive);
                room.OnRoomEnter();
            }
            else
            {
                room.OnRoomExit();
                room.gameObject.SetActive(isActive);
            }
        }

        #endregion


    }
}
