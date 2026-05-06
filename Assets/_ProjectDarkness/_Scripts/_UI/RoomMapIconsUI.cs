using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDarkness
{
    public class RoomMapIconsUI : MonoBehaviour
    {
        [SerializeField] private RoomIconUI _roomIconUIPrefab;

        private Dictionary<Vector2Int, RoomIconUI> _roomIconUIs = new();
        private Vector2Int _currentActiveRoomCoord = new Vector2Int(-999, -999);

        private void Start()
        {
            LevelManager.Instance.OnRoomSpawned += GenerateRoomIcon;
            LevelManager.Instance.OnRoomSetActive += UpdateMiniMap;
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnRoomSpawned -= GenerateRoomIcon;
            LevelManager.Instance.OnRoomSetActive -= UpdateMiniMap;
        }

        private void GenerateRoomIcon(Vector2Int roomCoord)
        {
            RoomIconUI roomIcon = Instantiate(_roomIconUIPrefab, transform);
            roomIcon.Initialize(roomCoord);

            float worldX = (roomCoord.x - LevelManager.Instance.StartPos.x) * LevelManager.Instance.RoomLength;
            float worldZ = (roomCoord.y - LevelManager.Instance.StartPos.y) * LevelManager.Instance.RoomLength;

            roomIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(worldX, worldZ);

            _roomIconUIs.Add(roomCoord, roomIcon);
        }

        private void UpdateMiniMap()
        {
            RoomEntry currentRoomEntry = LevelManager.Instance.CurrentActiveRoomEntry;

            // Re-enable icon for previous room
            if (_roomIconUIs.ContainsKey(_currentActiveRoomCoord))
            {
                _roomIconUIs[_currentActiveRoomCoord].SetRoomIconDimmed(false);
            }

            _currentActiveRoomCoord = currentRoomEntry.RoomCoord;
            
            _roomIconUIs[_currentActiveRoomCoord].SetRoomIconState(RoomIconState.Visited);
            
            // Hide icon for current room
            _roomIconUIs[_currentActiveRoomCoord].SetRoomIconDimmed(true);

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = _currentActiveRoomCoord + dir;

                if (_roomIconUIs.ContainsKey(neighbor))
                {
                    if (_roomIconUIs[neighbor].RoomIconState == RoomIconState.UnDiscovered)
                    {
                        _roomIconUIs[neighbor].SetRoomIconState(RoomIconState.Unvisited);
                    }
                }
            }

        }



    }
}
