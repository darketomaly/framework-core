using UnityEngine;

namespace Framework.Tools
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Color m_ColliderColor = new Color(0.737f, 0.22f, 0.235f, 0.824f);
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmosSelected()
        {
            BoxCollider[] colliders = GetComponents<BoxCollider>();

            foreach (BoxCollider boxCollider in colliders)
            {
                //Local
                Gizmos.matrix = boxCollider.transform.localToWorldMatrix;

                Vector3 center = boxCollider.center;
                Vector3 size = boxCollider.size;

                Gizmos.color = m_ColliderColor;
                Gizmos.DrawCube(center, size);

                Gizmos.color = m_ColliderColor * 1.5f;
                Gizmos.DrawWireCube(center, size);
            }

            //Reset
            Gizmos.matrix = Matrix4x4.identity;
        }
        
        #endif
    }
}