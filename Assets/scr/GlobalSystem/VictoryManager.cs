using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("Referensi UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI errorText;   
    [SerializeField] private TextMeshProUGUI warningText; 

    [Header("Pengaturan Scene")]
    [SerializeField] private string nextLevelSceneName;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HideVictoryPanel();
    }

    public void ShowVictoryPanel(int totalErrors, int totalWarnings)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        
        if (errorText != null) errorText.text = $"{totalErrors}";
        if (warningText != null) warningText.text = $"{totalWarnings}";
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Fungsi khusus untuk menyembunyikan panel
    public void HideVictoryPanel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void LoadNextLevel()
    {
        HideVictoryPanel(); // Sembunyikan panel sebelum pindah scene!
        
        Time.timeScale = 1f; 
        PlayerGridMovement.isTerminalActive = false; 
        SceneManager.LoadScene(nextLevelSceneName);
    }

    public void LoadMainMenu()
    {
        HideVictoryPanel(); // Sembunyikan panel sebelum pindah scene!
        
        Time.timeScale = 1f; 
        PlayerGridMovement.isTerminalActive = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}