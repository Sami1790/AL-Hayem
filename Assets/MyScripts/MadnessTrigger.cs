using UnityEngine;

/// <summary>
/// Trigger لتفعيل الجنون إذا دخل اللاعب منطقة البداية.
/// </summary>
public class MadnessTrigger : MonoBehaviour
{
    [Tooltip("اسحب هنا سكربت MadnessManager من المشهد")]
    public MadnessManager madnessManager;

    private void OnTriggerEnter(Collider other)
    {
        // يفعل الجنون فقط للاعب (تأكد التاج صحيح)
        if (other.CompareTag("Player"))
        {
            if (madnessManager != null)
            {
                madnessManager.ActivateMadness(); // يشغل عداد الجنون والـ UI
                Destroy(gameObject); // يحذف الكولايدر عشان ما يتكرر التفعيل
            }
            else
            {
                Debug.LogWarning("MadnessManager مو مربوط بالتريجر! اسحبه في الانسبكتور.");
            }
        }
    }
}
