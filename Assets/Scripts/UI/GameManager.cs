﻿using Assets.DTOs;
using Assets.Scripts.GameLogic.models.enums;
using Assets.Scripts.Utils.Managers;
using Iterum.DTOs;
using Iterum.models.interfaces;
using Iterum.Scripts.Utils;
using Iterum.Scripts.Utils.Managers;
using UnityEngine;

namespace Iterum.Scripts.UI
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public MapDto SelectedMap;
        public ICreature SelectedCreature { get; set; }
        public Team Team { get; set; } = Team.ENEMY;
        public ActionDto SelectedAction { get; set; }

        void Awake()
        {
            if (Instance == null)
            {
                AssetManager.EnsureExists();
                MapManager.EnsureExists();
                ActionManager.EnsureExists();
                UserManager.EnsureExists();
                JournalManager.EnsureExists();
                CharacterManager.EnsureExists();
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
