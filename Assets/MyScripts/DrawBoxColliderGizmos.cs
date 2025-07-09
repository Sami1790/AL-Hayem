using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class DrawBoxColliderGizmos : MonoBehaviour
{
    public Color gizmoColor = new Color(0.8f, 0.3f, 0.3f, 0.7f); // أحمر شفاف

    void OnDrawGizmos()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
        {
            Gizmos.color = gizmoColor;
            // حساب مركز وحجم الكولايدر بالنسبة لموضع الجسم
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bc.center, bc.size);
            Gizmos.matrix = oldMatrix;
        }
    }
}
