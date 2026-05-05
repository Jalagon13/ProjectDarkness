using UnityEngine;

namespace ProjectDarkness
{
    [CreateAssetMenu(fileName = "New Room Pool Data", menuName = "ProjectDarkness/RoomPoolData")]
    public class RoomPoolData : ScriptableObject
    {
        [field: SerializeField] public Room[] RoomPool { get; private set; }
        
        public Room GetRandomRoomFromPool()
        {
            return RoomPool[Random.Range(0, RoomPool.Length)];
        }
    }
}
