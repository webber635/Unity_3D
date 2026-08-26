using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Referensi Tombol Level")]
    [SerializeField] private Button btnLevel1;
    [SerializeField] private Button btnLevel2;
    
    [Header("Indikator Visual Terkunci (Opsional)")]
    [SerializeField] private GameObject lockIconLevel2; // Icon gembok atau overlay abu-abu

    void Start()
    {
        CheckLevelUnlockStatus();
    }

    public void CheckLevelUnlockStatus()
    {
        // Cek apakah PlayerPrefs "Level2Unlocked" bernilai 1 (artinya Level 1 sudah selesai)
        int isLevel2Unlocked = PlayerPrefs.GetInt("Level2Unlocked", 0);

        if (isLevel2Unlocked == 1)
        {
            // Buka kunci Level 2
            btnLevel2.interactable = true;
            if (lockIconLevel2 != null) lockIconLevel2.SetActive(false); // Sembunyikan gembok/abu-abu
        }
        else
        {
            // Kunci Level 2 (Abu-abu / Tidak bisa diklik)
            btnLevel2.interactable = false;
            if (lockIconLevel2 != null) lockIconLevel2.SetActive(true); // Tampilkan gembok/abu-abu
        }
    }

    // Fungsi untuk tombol-tombol level
    public void LoadLevel1()
    {
        SceneManager.LoadScene("lvl_1"); // Sesuaikan nama scene Level 1 kamu
    }

    public void LoadLevel2()
    {
        // Pengaman ekstra: Pastikan hanya bisa dimuat jika sudah terbuka
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
        {
            SceneManager.LoadScene("lvl_2"); // Sesuaikan nama scene Level 2 kamu
        }
        else
        {
            Debug.Log("Level 2 masih terkunci!");
        }
    }
}