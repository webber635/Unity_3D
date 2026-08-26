using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Referensi Panel UI")]
    [SerializeField] private GameObject mainMenuPanel;    
    [SerializeField] private GameObject levelSelectPanel; 

    void Start()
    {
        // Pastikan saat mulai, Main Menu aktif dan Level Select tertutup
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void OpenLevelSelect()
    {
        // Panel utama dibiarkan menyala, kita cukup memunculkan panel level di atasnya
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        // Cukup sembunyikan panel level saat tombol Back ditekan
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game ditutup.");
        Application.Quit();
    }
}