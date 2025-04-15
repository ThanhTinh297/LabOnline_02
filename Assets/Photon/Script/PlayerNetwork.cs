using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerNetwork : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private ChatManager chatManager;

    public void PlayerJoined(PlayerRef player)
    {
        if (chatManager == null)
        {
            chatManager = FindObjectOfType<ChatManager>();
            if (chatManager == null)
            {
                Debug.LogError("PlayerNetwork: Không tìm th?y ChatManager trong scene!");
            }
        }

        if (player == Runner.LocalPlayer)
        {
            Debug.Log($"PlayerNetwork: Local Player {player.PlayerId} joined. Initializing Chat.");
            if (chatManager != null)
            {
                chatManager.InitializeAndConnect(Runner); // G?i hàm m?i c?a ChatManager
            }

            if (spawnPoints == null || spawnPoints.Count < 2)
            {
                Debug.LogError("PlayerNetwork: Spawn points not configured correctly."); // Thêm log l?i rõ h?n
                return;
            }
            List<int> sortedPlayerIds = Runner.ActivePlayers
                .Select(p => p.PlayerId)
                .OrderBy(id => id)
                .ToList();
            int playerIndexInSortedList = sortedPlayerIds.IndexOf(player.PlayerId);
            if (playerIndexInSortedList == -1)
            {
                Debug.LogError($"Player {player.PlayerId} NOT FOUND in sorted list derived from ActivePlayers! This is highly unexpected. Defaulting index to 0.");
                playerIndexInSortedList = 0;
            }
            int spawnPointIndex = playerIndexInSortedList % spawnPoints.Count;
            if (spawnPointIndex < 0 || spawnPointIndex >= spawnPoints.Count ||
                spawnPoints[spawnPointIndex] == null)
            {
                Debug.LogError($"Selected spawnPointIndex ({spawnPointIndex}) is invalid or the Transform is null!");
                spawnPointIndex = 0;
                if (spawnPoints.Count == 0 || spawnPoints[spawnPointIndex] == null)
                {
                    Debug.LogError("Default spawn point (index 0) is also null/missing! Cannot spawn.");
                    return;
                }
                Debug.LogWarning("Falling back to spawn point index 0.");
            }
            Transform selectedSpawnPoint = spawnPoints[spawnPointIndex];
            Vector3 spawnPosition = selectedSpawnPoint.position;
            Quaternion spawnRotation = selectedSpawnPoint.rotation;
            Runner.Spawn(playerPrefab,
                spawnPosition,
                spawnRotation,
                player,
                (runner, obj) =>
                {
                CharacterController controller = obj.GetComponent<CharacterController>();
                if (controller != null)
                {
                        controller.enabled = false;
                        obj.transform.position = spawnPosition;
                        obj.transform.rotation = spawnRotation;
                        controller.enabled = true;
                    }
                    else
                    {
                        obj.transform.position = spawnPosition;
                        obj.transform.rotation = spawnRotation;
                    }
                    var playerSetup = obj.GetComponent<PlayerSetup>();
                    if (playerSetup != null)
                    {
                        playerSetup.SetUpCamera();
                    }
                    else
                    {
                        Debug.LogError($"Spawned player object for {player} is missing PlayerSetup component!");
                    }
                });
        }
        else
        {
            Debug.Log($"PlayerNetwork: Remote Player {player.PlayerId} joined.");
        }
    }
}