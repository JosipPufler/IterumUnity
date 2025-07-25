// Assets/Scripts/Utils/TurnOrderManager.cs
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class TurnOrderManager : NetworkBehaviour
    {
        public static TurnOrderManager Instance;

        private void Awake() => Instance = this;
        public bool InCombat { get; set; }
        public readonly SyncList<CharacterToken> currentRound = new();
        public List<CharacterToken> allCombatants = new();
        public readonly Dictionary<CharacterToken, int> initiativeRolls = new();

        public override void OnStartServer()
        {
            Instance = this;
        }

        public override void OnStartClient()
        {
            if (Instance == null) Instance = this;
        }

        private void Update()
        {
            if (!isServer) return;

            for (int i = currentRound.Count - 1; i >= 0; i--)
            {
                var token = currentRound[i];
                if (token.creature.IsDead)
                {
                    currentRound.RemoveAt(i);
                    allCombatants.Remove(token);
                    RpcRemovePortrait(token);

                    if (i == 0)
                    {
                        StartTurn();
                    }
                }
            }
        }

        [Server]
        public void StartCombat(List<CharacterToken> combatants)
        {
            InCombat = true;
            Debug.Log("[StartCombat] Starting combat...");
            allCombatants = combatants.Where(x => !x.creature.IsDead).ToList();
            Debug.Log($"[StartCombat] Number of combatants: {allCombatants.Count}");
            RerollInitiative();
        }

        [Server]
        private void RerollInitiative()
        {
            Debug.Log("[RerollInitiative] Starting initiative roll...");

            initiativeRolls.Clear();
            allCombatants = allCombatants.Where(x => !x.creature.IsDead).ToList();

            foreach (var token in allCombatants)
            {
                int roll = token.creature.RollInitiative();
                initiativeRolls[token] = roll;
                Debug.Log($"[RerollInitiative] Token {token.name} rolled {roll} (netId={token.netId})");
            }

            var sorted = initiativeRolls
                .GroupBy(x => x.Value)
                .OrderByDescending(g => g.Key)
                .SelectMany(g => g.OrderBy(_ => Random.value))
                .Select(g => g.Key)
                .ToList();

            currentRound.Clear();
            foreach (var token in sorted)
            {
                currentRound.Add(token);
                Debug.Log($"[RerollInitiative] Added {token.name} (netId={token.netId}) to currentRound");
            }

            var tokenNetIds = sorted.Select(t => t.netId).ToArray();
            Debug.Log($"[RerollInitiative] Sending netIds to clients: {string.Join(", ", tokenNetIds)}");

            RpcStartCombatClientSync(tokenNetIds);
            StartTurn();
        }

        [Server]
        public void EndTurn()
        {
            currentRound[0].creature.EndTurn();
            currentRound.RemoveAt(0);

            RpcTurnEnded(currentRound.ToArray());

            if (currentRound.Count == 0)
            {
                RerollInitiative();
            }
            else
            {
                StartTurn();
            }
        }

        [Server]
        private void StartTurn()
        {
            if (currentRound.Count == 0)
                return;

            currentRound[0].creature.StartTurn();
        }

        [ClientRpc]
        private void RpcStartCombatClientSync(uint[] tokenNetIds)
        {
            Debug.Log($"[RpcStartCombatClientSync] Received {tokenNetIds.Length} netIds: {string.Join(", ", tokenNetIds)}");
            StartCoroutine(WaitAndSyncPortraits(tokenNetIds));
        }

        private IEnumerator WaitAndSyncPortraits(uint[] tokenNetIds)
        {
            const int maxAttempts = 10;
            const float retryDelay = 0.2f;

            List<CharacterToken> tokens = new();

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                tokens.Clear();
                bool allFound = true;

                foreach (var id in tokenNetIds)
                {
                    if (NetworkClient.spawned.TryGetValue(id, out var identity))
                    {
                        if (identity.TryGetComponent<CharacterToken>(out var token))
                        {
                            tokens.Add(token);
                        }
                        else
                        {
                            Debug.LogWarning($"[WaitAndSyncPortraits] Missing CharacterToken on identity {id} (attempt {attempt + 1})");
                            allFound = false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[WaitAndSyncPortraits] NetworkIdentity not yet spawned for netId={id} (attempt {attempt + 1})");
                        allFound = false;
                    }
                }

                if (allFound)
                    break;

                yield return new WaitForSeconds(retryDelay);
            }

            if (tokens.Count > 0)
            {
                Debug.Log($"[WaitAndSyncPortraits] Synced {tokens.Count} tokens to portrait manager.");
                GeneralManager.Instance.SyncPortraitsWithOrder(tokens);
            }
            else
            {
                Debug.LogError("[WaitAndSyncPortraits] Failed to resolve any tokens after retries.");
            }
        }

        [ClientRpc]
        private void RpcTurnEnded(CharacterToken[] newOrder)
        {
            GeneralManager.Instance.SyncPortraitsWithOrder(newOrder);
        }

        [ClientRpc]
        void RpcRemovePortrait(CharacterToken token)
        {
            if (token != null)
                GeneralManager.Instance.RemovePortrait(token);
        }

        public CharacterToken GetCurrentCharacter() {
            if (currentRound == null || currentRound.Count == 0)
            {
                return null;
            }
            return currentRound.First();
        }
    }
}
