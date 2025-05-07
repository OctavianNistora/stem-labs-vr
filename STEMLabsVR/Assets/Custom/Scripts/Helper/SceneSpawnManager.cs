using UnityEngine;
namespace Custom.Scripts.Helper
{
    // Singleton class to manage scene transitions and spawn points
    public class SceneSpawnManager : MonoBehaviour
    {
        public static SceneSpawnManager Instance;
        
        [SerializeField]
        public string previousScenePathName;
        [SerializeField]
        public float doorOrientation;
        [SerializeField]
        public bool isTransitioning;

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
        
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
