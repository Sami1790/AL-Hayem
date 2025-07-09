using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // يشغل مشهد السينما (Intro) أولاً
    public void PlayIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }

    // زر السكب (أو Play داخل الانترو) يوديك مباشرة لمشهد اللعب
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
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
