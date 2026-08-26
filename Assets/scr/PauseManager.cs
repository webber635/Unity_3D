using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton; // << TAMBAHKAN REFERENSI TOMBOL INI
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static bool isPaused = false; 

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        // 1. AUTO-HIDE TOMBOL PAUSE
        // Sembunyikan tombol jika game di-pause, Terminal aktif, ATAU Task Manager aktif
        if (pauseButton != null)
        {
            pauseButton.SetActive(!isPaused && !PlayerGridMovement.isTerminalActive && !TaskManagerUI.isTaskManagerActive);
        }

        // 2. KONTROL TOMBOL ESCAPE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Cegah Pause Menu menimpa layar jika Terminal ATAU Task Manager sedang aktif
            if (PlayerGridMovement.isTerminalActive || TaskManagerUI.isTaskManagerActive) return; 

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void RestartLevel()
    {
        ResumeGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        ResumeGame(); 
        PlayerGridMovement.isTerminalActive = false; 
        SceneManager.LoadScene(mainMenuSceneName);
    }
}