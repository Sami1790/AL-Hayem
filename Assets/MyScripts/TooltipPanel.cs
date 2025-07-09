using UnityEngine;

public class TooltipPanel : MonoBehaviour
{
    public GameObject panel; // اسحب البانل هنا في الانسبكتور
    public PlayerController playerController; // اسحب PlayerController يدوي في الانسبكتور

    // Static لتشييك الحالة من أي سكربت آخر
    public static bool TooltipActive = false;

    void Awake()
    {
        if (playerController == null)
            Debug.LogWarning("TooltipPanel: PlayerController غير مربوط في الانسبكتور!");
    }

    // إظهار البانل + تجميد اللاعب والكاميرا + إظهار الماوس
    public void ShowPanel()
    {
        if (panel != null)
            panel.SetActive(true);

        TooltipActive = true;

        if (playerController != null)
            playerController.isFrozen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // إخفاء البانل + إرجاع تحكم اللاعب + قفل الماوس
    public void HidePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        TooltipActive = false;

        if (playerController != null)
            playerController.isFrozen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
