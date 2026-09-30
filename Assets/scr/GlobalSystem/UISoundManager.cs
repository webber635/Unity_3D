using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip terminalOpenSound;
    [SerializeField] private AudioClip inventoryOpenSound;

    [SerializeField] private AudioClip buttonClickSound;

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- TAMBAHAN: Amankan referensi AudioSource UI ---
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    else
    {
        Destroy(gameObject);
    }
}

    public void PlayTerminalOpen()
    {
        // --- TAMBAHAN: Jika sound dimatikan, batalkan suara ---
        if (AudioManager.Instance != null && AudioManager.Instance.isSoundMuted) return;

        if (audioSource != null && terminalOpenSound != null)
        {
            audioSource.PlayOneShot(terminalOpenSound);
        }
    }

    public void PlayInventoryOpen()
    {
        // --- TAMBAHAN: Jika sound dimatikan, batalkan suara ---
        if (AudioManager.Instance != null && AudioManager.Instance.isSoundMuted) return;

        if (audioSource != null && inventoryOpenSound != null)
        {
            audioSource.PlayOneShot(inventoryOpenSound);
        }
    }

    public void PlayButtonClick()
    {
        // --- TAMBAHAN: Jika sound dimatikan, batalkan suara ---
        if (AudioManager.Instance != null && AudioManager.Instance.isSoundMuted) return;

        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}