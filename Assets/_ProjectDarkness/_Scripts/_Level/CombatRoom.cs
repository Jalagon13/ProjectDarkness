using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectDarkness
{
    public class CombatRoom : Room
    {
        private CombatRoomState _combatRoomState = CombatRoomState.HasEnemies;
        private List<Npc> _combatNpcs = new();

        protected override void Awake()
        {
            base.Awake();
            OnNavMeshBuildStarted += DisableCombatNpcAI;
            OnNavMeshBuildCompleted += EnableCombatNpcAI;
            OnNavMeshCleared += DisableCombatNpcAI;
        }

        protected override void OnDestroy()
        {
            OnNavMeshBuildStarted -= DisableCombatNpcAI;
            OnNavMeshBuildCompleted -= EnableCombatNpcAI;
            OnNavMeshCleared -= DisableCombatNpcAI;
            base.OnDestroy();
        }
        
        protected override void OnFirstVisit()
        {
            base.OnFirstVisit();
            
            _combatRoomState = CombatRoomState.HasEnemies;

            if(_combatNpcs.Count > 0)
            {
                Debug.Log($"Close all doors on first visit");
                CloseAllDoors();
            }
            else
            {
                Debug.Log($"Open all doors on first visit");
                OpenAllDoors();
            }
        }

        public override void OnRoomExit()
        {
            DisableCombatNpcAI();
            base.OnRoomExit();
        }
        
        public void RegisterCombatNpc(Npc npc)
        {
            // Debug.Log($"Registering NPC {npc.gameObject.name}");
            _combatNpcs.Add(npc);
            npc.OnDeath += OnNpcDeath;
        }

        private void EnableCombatNpcAI()
        {
            if (_combatRoomState == CombatRoomState.Cleared)
            {
                return;
            }

            foreach (Npc npc in _combatNpcs)
            {
                if (npc != null)
                {
                    npc.SetAiEnabled(true);
                }
            }
        }

        private void DisableCombatNpcAI()
        {
            foreach (Npc npc in _combatNpcs)
            {
                if (npc != null)
                {
                    npc.SetAiEnabled(false);
                }
            }
        }

        private void OnNpcDeath(object sender, EventArgs e)
        {
            Npc npc = sender as Npc;
            npc.OnDeath -= OnNpcDeath;
            
            _combatNpcs.Remove(npc);
            
            if (_combatNpcs.Count == 0)
            {
                OnRoomClear();
            }
        }

        protected virtual void OnRoomClear()
        {
            _combatRoomState = CombatRoomState.Cleared;
            OpenAllDoors();
        }

        private void CloseAllDoors()
        {
            NorthWall.SetDoorState(DoorState.Closed);
            SouthWall.SetDoorState(DoorState.Closed);
            EastWall.SetDoorState(DoorState.Closed);
            WestWall.SetDoorState(DoorState.Closed);
        }
        
        private void OpenAllDoors()
        {
            NorthWall.SetDoorState(DoorState.Open);
            SouthWall.SetDoorState(DoorState.Open);
            EastWall.SetDoorState(DoorState.Open);
            WestWall.SetDoorState(DoorState.Open);
        }

    }
}
