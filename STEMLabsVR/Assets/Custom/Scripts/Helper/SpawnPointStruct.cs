using UnityEngine;
namespace Custom.Scripts.Helper
{
    // Structure for storing spawn point data
    public struct SpawnPointStruct
    {
        public Vector3 position;
        public Quaternion rotation;
        
        public SpawnPointStruct(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
