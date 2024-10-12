﻿using CavrnusSdk.API;
using UnityEngine;

namespace CavrnusSdk.CollaborationExamples
{
    public class CavrnusSpaceSwitchingUI : MonoBehaviour
    {
        [SerializeField] private CavrnusSpaceLevelData levelData;
        
        [Space]
        [SerializeField] private CavrnusSpaceSwitchingEntry entryPrefab;
        [SerializeField] private Transform entriesContainer;

        private CavrnusSpaceSwitchingEntry currentSelectedEntry;
        private CavrnusSpaceConnection spaceConnection;

        public void Start()
        {
            SetupUI();    
        }

        private void SetupUI()
        {
            levelData.Levels.ForEach(data => {
                var entry = Instantiate(entryPrefab, entriesContainer, false);
                entry.Setup(data, SpaceSelected);
            });
        }

        private void SpaceSelected(CavrnusSpaceSwitchingEntry entry)
        {
            if (currentSelectedEntry != null)
                currentSelectedEntry.SetSelectedState(false);
            
            entry.SetSelectedState(true);
            currentSelectedEntry = entry;
            
            levelData.LoadLevel(spaceConnection, entry.SpaceData, OnSpaceLevelLoaded);
        }

        private void OnSpaceLevelLoaded()
        {
            print("Hey we loaded a level and joined a cavrnus space!...NICE!");
        }
    }
}