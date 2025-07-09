using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // يشغل مشهد اللعب (من القائمة أو من أي مكان)
    public void PlayGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }

    // إعادة اللعب (من مشهد الفوز أو الخسارة)
    public void RestartGame()
    {
        SceneManager.LoadScene("Main_Scene");
    }

    // يفتح مشهد الإعدادات
    public void GoToSettings()
    {
        SceneManager.LoadScene("SettingScene");
    }

    // يفتح القائمة الرئيسية
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // خروج من اللعبة
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // عشان يوقف التشغيل في محرر يونيتي
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
