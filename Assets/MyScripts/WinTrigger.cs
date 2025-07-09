using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // تأكد أن اللاعب عليه Tag "Player"
        {
            SceneManager.LoadScene("WinScene"); // اسم مشهد الفوز
        }
    }
}
