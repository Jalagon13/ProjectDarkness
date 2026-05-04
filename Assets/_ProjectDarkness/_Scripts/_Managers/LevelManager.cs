using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [SerializeField] private int _level = 1;
        [SerializeField] private int _roomLength = 22;
        [SerializeField] private int _floorPlanGridXLength = 8;
        [SerializeField] private int _floorPlanGridYLength = 9;
        [SerializeField] private Room _roomPrefab;

        private Dictionary <Vector2Int, RoomEntry> _floorPlan;
        private readonly List<Room> _spawnedRooms = new();

        private void Awake()
        {
            Instance = this;

            _floorPlan = new Dictionary<Vector2Int, RoomEntry>();
        }

        [Button("Generate Level")]
        private void GenerateLevel()
        {
            GenerateFloorPlanData();
            GenerateDoorwayData();
            SpawnRooms();
        }

        private void GenerateFloorPlanData()
        {
            Vector2Int startPos = new Vector2Int(_floorPlanGridXLength / 2, _floorPlanGridYLength / 2);

            int numberOfRooms = Mathf.FloorToInt(UnityEngine.Random.Range(0, 2) + 5 + _level * 2.6f);
            Debug.Log($"numberOfRooms: {numberOfRooms}");

            int maxRetries = 2000;
            int retries = 0;
            while (retries < maxRetries)
            {
                _floorPlan = new Dictionary<Vector2Int, RoomEntry>
                {
                    { startPos, new RoomEntry() }
                };

                List<Vector2Int> deadEnds = new List<Vector2Int>();
                Queue<Vector2Int> queue = new Queue<Vector2Int>();
                queue.Enqueue(startPos);

                int reseedCount = 0;

                Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

                while (queue.Count > 0 && _floorPlan.Count < numberOfRooms)
                {
                    Vector2Int current = queue.Dequeue();
                    bool addedAny = false;

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
                        _floorPlan.Add(neighbor, new RoomEntry());
                        queue.Enqueue(neighbor);
                        addedAny = true;
                    }

                    if (!addedAny)
                    {
                        deadEnds.Add(current);
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
                
                
                Debug.Log($"Generated Isaac Dungeon with {_floorPlan.Count} rooms in {retries} attempts.");
                break;
            }
        }

        private void GenerateDoorwayData()
        {
            foreach (var kvp in _floorPlan)
            {
                Vector2Int pos = kvp.Key;
                RoomEntry room = kvp.Value;

                // Check cardinal directions for neighbors. If a room exists in the dictionary at that offset, we need a door!
                room.HasNorthDoor = _floorPlan.ContainsKey(pos + Vector2Int.up);
                room.HasSouthDoor = _floorPlan.ContainsKey(pos + Vector2Int.down);
                room.HasEastDoor = _floorPlan.ContainsKey(pos + Vector2Int.right);
                room.HasWestDoor = _floorPlan.ContainsKey(pos + Vector2Int.left);
            }
        }

        private void SpawnRooms()
        {
            foreach (Room room in _spawnedRooms)
            {
                if (room != null)
                {
                    Destroy(room.gameObject);
                }
            }

            _spawnedRooms.Clear();

            Vector2Int startPos = new Vector2Int(_floorPlanGridXLength / 2, _floorPlanGridYLength / 2);

            foreach (var kvp in _floorPlan)
            {
                Vector2Int gridPos = kvp.Key;

                float worldX = (gridPos.x - startPos.x) * _roomLength;
                float worldZ = (gridPos.y - startPos.y) * _roomLength;
                Vector3 worldPos = new Vector3(worldX, 0f, worldZ);

                Room newRoom = Instantiate(_roomPrefab, worldPos, Quaternion.identity);
                newRoom.Initialize(kvp.Value);
                _spawnedRooms.Add(newRoom);
            }
        }
    }
}
