using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Vector3 spawnPos = RandPos();

            Runner.SetPlayerObject(player, Runner.Spawn(PlayerPrefab,
                spawnPos,
                Quaternion.identity,
                player,
                (Runner, obj) =>
                {
                    var playerSetup = obj.GetComponent<PlayerSetup>();
                    if (playerSetup != null)
                    {
                        playerSetup.SetUpCamera();
                    }
                }));
        }
    }

    private Vector3 RandPos()
    {
        int randX = Random.Range(-15, 15);
        int randZ = Random.Range(-15, 15);

        return new Vector3(randX, 1, randZ);
    }
}
