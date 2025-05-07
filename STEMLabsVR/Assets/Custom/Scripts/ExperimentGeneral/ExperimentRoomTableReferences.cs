using System.Collections.Generic;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Singleton class to hold references to table transforms in the experiment room
    public class ExperimentRoomTableReferences : MonoBehaviour
    {
        public static ExperimentRoomTableReferences Instance { get; private set; }
    
        [SerializeField]
        private List<Transform> tableTransforms;
    
        public List<Transform> TableTransforms
        {
            get { return tableTransforms; }
            set { tableTransforms = value; }
        }
    
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}
