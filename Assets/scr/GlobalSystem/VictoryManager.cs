using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("Referensi UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Pengaturan Navigasi Level")]
    [Tooltip("Ketik nama Scene Main Menu utama")]
    [SerializeField] private string mainMenuName = "MainMenu";

    // Variabel nextLevelName dihapus karena kita akan menggunakan urutan otomatis

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Menghancurkan duplikat
        }
    }

    private void Start()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void ShowVictoryPanel(int errors, int warnings)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);

        if (errorText != null) errorText.text = ""+errors;
        if (warningText != null) warningText.text = ""+warnings;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false); 

        // Mengambil index scene saat ini, lalu ditambah 1 untuk memuat scene berikutnya
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        // Memastikan scene berikutnya ada di dalam daftar Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Jika tidak ada scene lagi (game tamat), kembalikan ke Main Menu
            Debug.Log("Level terakhir diselesaikan! Kembali ke Main Menu.");
            SceneManager.LoadScene(mainMenuName);
        }
    }

    public void LoadMainMenu()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false); 
        SceneManager.LoadScene(mainMenuName);
    }

    public void RestartLevel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}