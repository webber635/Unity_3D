using UnityEngine;

public class HelpMenuController : MonoBehaviour
{
    [Header("Referensi Panel Bantuan")]
    [SerializeField] private GameObject helpPanel; // Satu panel utama yang berisi seluruh informasi

    void Start()
    {
        // Sembunyikan panel bantuan di awal permainan
        if (helpPanel != null) 
        {
            helpPanel.SetActive(false);
        }
    }

    // Dipanggil oleh tombol logo tanda tanya (?) di Main Menu
    public void OpenHelpMenu()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
        }
    }

    // Dipanggil oleh tombol Close (X) di dalam panel bantuan
    public void CloseHelpMenu()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }
}