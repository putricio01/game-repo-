
using UnityEngine;
using Unity.Netcode;




public class PlayerRespawner : NetworkBehaviour
{
    public Transform playerOneSpawnPoint; // Assign in the editor
    public Transform playerTwoSpawnPoint; // Assign in the editor
    public Ball ball;
    
    public gol lol;
    public gol2 lol2;

    public void RespawnPlayersAfterGoal()
    {
        RespawnPlayersServerRpc();
    }
    
    public void pi()
    {
    
       lol.ResetScoreServerRpc();
       lol2.ResetScoreServerRpc();
       RespawnPlayersServerRpc();
       ball.Respawn();
    }    

   // [ServerRpc]
    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayersServerRpc()
    {
        // Respawn the host player
        RespawnPlayer(NetworkManager.Singleton.LocalClientId, true);

        // Respawn the client player
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key != NetworkManager.Singleton.LocalClientId)
            {
                RespawnPlayer(client.Key, false);
                break; // Since it's 1v1, we can break after finding the one other client
            }
        }
    }

    private void RespawnPlayer(ulong clientId, bool isHost)
    {
        // Get the NetworkObject for the player
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            // Assuming the player component is attached to the network object
            var player = networkClient.PlayerObject.GetComponent<CubeController>();
            if (player)
            {
                // Move the player to the correct spawn point
                Vector3 spawnPosition = isHost ? playerOneSpawnPoint.position : playerTwoSpawnPoint.position;
                Quaternion spawnRotation = isHost ? playerOneSpawnPoint.rotation : playerTwoSpawnPoint.rotation;
                
                player.transform.position = spawnPosition;
                player.transform.rotation = spawnRotation;

                // Reset other player states as needed

                // Inform the client of their new position
                MovePlayerClientRpc(spawnPosition, spawnRotation, clientId);
            }
        }
    }

    [ClientRpc]
    private void MovePlayerClientRpc(Vector3 position, Quaternion rotation, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // The client knows this is their player and can move them accordingly
            transform.position = position;
            transform.rotation = rotation;
            // Reset other necessary states here
        }
    }
}


