using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [HideInInspector] public bool isSoundMuted = false;
    [HideInInspector] public bool isMusicMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // Muat pengaturan tersimpan saat game mulai
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Dipanggil oleh Tombol Sound
    public void ToggleSound()
    {
        isSoundMuted = !isSoundMuted;
        PlayerPrefs.SetInt("SoundMuted", isSoundMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("Sound Muted: " + isSoundMuted);
    }

    // Dipanggil oleh Tombol Music
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        // Jika kamu punya script BGM khusus di game, matikan/nyalakan AudioSource-nya di sini
        // Contoh: if (BGMManager.Instance != null) BGMManager.Instance.SetMute(isMusicMuted);

        Debug.Log("Music Muted: " + isMusicMuted);
    }

    private void LoadSettings()
    {
        // 1 = True (Muted), 0 = False (Nyala)
        isSoundMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
    }
}