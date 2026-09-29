using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;

    [Header("Daftar Musik BGM")]
    [SerializeField] private AudioClip menuBGM;   // Musik untuk Main Menu
    [SerializeField] private AudioClip levelBGM;  // Musik untuk Level Scene

    [Header("Pengaturan Nama Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Sesuaikan dengan nama scene menu kamu

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // Daftarkan event saat scene selesai dimuat
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Lepaskan event saat script dimatikan
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Dipanggil otomatis setiap kali pindah scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayBGM(scene.name);
    }

    private void CheckAndPlayBGM(string sceneName)
    {
        if (bgmSource == null) return;

        AudioClip targetClip = null;

        // Jika nama scene mengandung "MainMenu" atau sesuai pengaturan, putar menuBGM
        if (sceneName == mainMenuSceneName)
        {
            targetClip = menuBGM;
        }
        else
        {
            // Selain itu (artinya masuk ke level), putar levelBGM
            targetClip = levelBGM;
        }

        // Jika klip yang harus diputar berbeda dengan yang sedang berjalan
        if (bgmSource.clip != targetClip)
        {
            bgmSource.clip = targetClip;
            if (targetClip != null)
            {
                bgmSource.Play();
            }
            else
            {
                bgmSource.Stop();
            }
        }
    }

    void Update()
    {
        // Menyesuaikan status Mute dari AudioManager yang sudah kita buat sebelumnya
        if (AudioManager.Instance != null && bgmSource != null)
        {
            bgmSource.mute = AudioManager.Instance.isMusicMuted;
        }
    }
}