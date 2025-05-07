using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script used to set up the bounding box for experiment setup
    public class SetupBoundingBox : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;

        public Bounds GetBounds()
        {
            return boxCollider.bounds;
        }
    }
}
