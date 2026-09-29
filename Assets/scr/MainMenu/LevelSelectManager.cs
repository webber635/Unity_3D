using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Pengaturan Level")]
    [Tooltip("Masukkan ke-7 tombol level berurutan")]
    [SerializeField] private Button[] levelButtons;
    
    [Tooltip("Masukkan Image panel hitam yang menutupi level berurutan")]
    [SerializeField] private GameObject[] darkOverlays;

    [Header("Pengaturan Scene")]
    [SerializeField] private string scenePrefix = "lvl_";

    [Header("Debug")]
    [SerializeField] private bool autoResetOnStart = false;

    private void Awake()
    {
        if (autoResetOnStart)
        {
            ResetAllProgress();
        }
    }

    void Start()
    {
        CheckLevelUnlockStatus();
    }

    public void CheckLevelUnlockStatus()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; 
            
            // Level 1 selalu terbuka, sisanya baca dari PlayerPrefs
            bool isUnlocked = (levelNumber == 1) || (PlayerPrefs.GetInt("Level" + levelNumber + "Unlocked", 0) == 1);
            
            // 1. Terapkan ke interaksi tombol
            if (levelButtons[i] != null) 
            {
                levelButtons[i].interactable = isUnlocked;
            }

            // 2. Terapkan ke panel gelap (Muncul jika terkunci, Hilang jika terbuka)
            if (i < darkOverlays.Length && darkOverlays[i] != null)
            {
                darkOverlays[i].SetActive(!isUnlocked);
            }
        }
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber == 1 || PlayerPrefs.GetInt("Level" + levelNumber + "Unlocked", 0) == 1)
        {
            SceneManager.LoadScene(scenePrefix + levelNumber);
        }
    }

    [ContextMenu("🔴 RESET ALL PROGRESS")]
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("⚠️ WARNING: Seluruh data PlayerPrefs telah di-reset!");

        if (Application.isPlaying)
        {
            CheckLevelUnlockStatus();
        }
    }
}