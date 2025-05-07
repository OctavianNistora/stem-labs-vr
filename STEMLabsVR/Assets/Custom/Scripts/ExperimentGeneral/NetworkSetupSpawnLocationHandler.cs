using System.Collections.Generic;
using Unity.Netcode;
namespace Custom.Scripts.ExperimentGeneral
{
    // This class handles server-side the spawning location of each player's setup in the experiment room.
    public class NetworkSetupSpawnLocationHandler : NetworkBehaviour
    {
        private int _spawnPointIndex;
    
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        
            // Check if the current game instance is the server.
            if (IsServer)
            {
                // Retrieve the list of available spawn point indices from the singleton instance.
                var indicesList = AvailableSpawnPointsSingleton.GetInstance().availableSpawnPointIndices;
            
                // Retrieve the last spawn point index from the list and remove it from the list.
                _spawnPointIndex = indicesList[indicesList.Count - 1];
                indicesList.RemoveAt(indicesList.Count - 1);
        
                // Set the spawn point for the player based on the retrieved index.
                var spawnPoint = ExperimentRoomTableReferences.Instance.TableTransforms[_spawnPointIndex];
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
        }

        public override void OnNetworkDespawn()
        {
            // Check if the current game instance is the server.
            if (IsServer)
            {
                // Readd the spawn point index back to the list of available spawn points.
                var indicesList = AvailableSpawnPointsSingleton.GetInstance().availableSpawnPointIndices;
                indicesList.Add(_spawnPointIndex);
            }
            
            base.OnNetworkDespawn();
        }
    }

    // Singleton class to manage available spawn points for players in the experiment room.
    public class AvailableSpawnPointsSingleton
    {
        private static AvailableSpawnPointsSingleton _instance;
        public List<int> availableSpawnPointIndices { get; private set; }
    
        private AvailableSpawnPointsSingleton()
        {
            availableSpawnPointIndices = new List<int>();
        
            var indices = ExperimentRoomTableReferences.Instance.TableTransforms.Count;
            for (int i = 0; i < indices; i++)
            {
                availableSpawnPointIndices.Add(indices - 1 - i);
            }
        }
    
        public static AvailableSpawnPointsSingleton GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AvailableSpawnPointsSingleton();
            }
            return _instance;
        }
    }
}