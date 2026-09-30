using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))] // Memastikan objek ini memiliki komponen Image
public class AudioButtonLinker : MonoBehaviour
{
    public enum AudioType { Sound, Music }
    
    [Header("Pengaturan Fungsi")]
    [Tooltip("Pilih jenis fungsi untuk tombol ini")]
    public AudioType buttonType;

    [Header("Pengaturan Gambar (Sprite)")]
    [Tooltip("Gambar saat suara/musik menyala")]
    public Sprite iconOn;
    [Tooltip("Gambar saat suara/musik dimatikan (Mute)")]
    public Sprite iconOff;

    private Image buttonImage;

    void Start()
    {
        Button btn = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // Sesuaikan gambar saat pertama kali menu/scene dimuat
        UpdateButtonVisual();

        // Hubungkan secara dinamis ke Instance yang bertahan antar scene
        if (buttonType == AudioType.Sound)
        {
            btn.onClick.AddListener(() => 
            {
                if (AudioManager.Instance != null) 
                {
                    AudioManager.Instance.ToggleSound();
                    UpdateButtonVisual(); // Perbarui gambar setelah diklik
                }
            });
        }
        else if (buttonType == AudioType.Music)
        {
            btn.onClick.AddListener(() => 
            {
                if (AudioManager.Instance != null) 
                {
                    AudioManager.Instance.ToggleMusic();
                    UpdateButtonVisual(); // Perbarui gambar setelah diklik
                }
            });
        }
    }

    // Fungsi untuk mengganti sprite berdasarkan status dari AudioManager
    private void UpdateButtonVisual()
    {
        if (AudioManager.Instance == null || buttonImage == null) return;

        if (buttonType == AudioType.Sound)
        {
            // Jika isSoundMuted true, pakai iconOff. Jika false, pakai iconOn.
            buttonImage.sprite = AudioManager.Instance.isSoundMuted ? iconOff : iconOn;
        }
        else if (buttonType == AudioType.Music)
        {
            // Jika isMusicMuted true, pakai iconOff. Jika false, pakai iconOn.
            buttonImage.sprite = AudioManager.Instance.isMusicMuted ? iconOff : iconOn;
        }
    }
}