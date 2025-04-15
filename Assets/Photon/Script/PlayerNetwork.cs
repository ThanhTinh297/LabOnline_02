using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class PlayerNetwork : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private List<Transform> SpawnPoint;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            if ((SpawnPoint == null) || (SpawnPoint.Count < 2))
            {
                return;
            }

            List<int> sortPlayerIds = Runner.ActivePlayers
                .Select(p => p.PlayerId)
                .OrderBy(id => id).ToList();

            int playerIndexInSortList = sortPlayerIds.IndexOf(player.PlayerId);
            if (playerIndexInSortList == -1)
            {
                playerIndexInSortList = 0;
            }

            int spawPointIndex = playerIndexInSortList % SpawnPoint.Count;
            if ((spawPointIndex < 0) || (spawPointIndex >= SpawnPoint.Count)
                || (SpawnPoint[spawPointIndex] == null))
            {
                spawPointIndex = 0;
                if (SpawnPoint.Count == 0 || SpawnPoint[spawPointIndex] == null)
                {
                    return;
                }
            }

            Transform selectedSpawnPoint = SpawnPoint[spawPointIndex];
            Vector3 spawnPosition = selectedSpawnPoint.position;
            Quaternion spawnRotation = selectedSpawnPoint.rotation;

            Runner.SetPlayerObject(player, Runner.Spawn(PlayerPrefab,
                spawnPosition,
                spawnRotation,
                player,
                (Runner, obj) =>
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
                }));
        }
    }
}
