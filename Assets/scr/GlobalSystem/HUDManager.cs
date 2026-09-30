using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI World Space Reference")]
    [SerializeField] private TMP_Text floatingHintText;
    [SerializeField] private GameObject worldCanvas;
    
    // --- TAMBAHAN: Cache script UIFollowTarget ---
    private UIFollowTarget uiFollower;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Cari komponen UIFollowTarget yang menempel di worldCanvas
        if (worldCanvas != null)
        {
            uiFollower = worldCanvas.GetComponent<UIFollowTarget>();
        }
    }

    private void Start()
    {
        // Dikosongkan agar tidak ada teks muncul di awal game
    }

    // --- UBAH: Tambahkan parameter targetTransform ---
    public void ShowHint(string message, Transform targetTransform = null)
{
    // --- TAMBAHAN: Cari ulang referensi UI jika hancur saat pindah level ---
    if (worldCanvas == null)
    {
        // Pastikan nama objek Canvas di dalam tiap scene benar-benar "WorldCanvas"
        worldCanvas = GameObject.Find("WorldCanvas"); 
        if (worldCanvas != null)
        {
            uiFollower = worldCanvas.GetComponent<UIFollowTarget>();
            floatingHintText = worldCanvas.GetComponentInChildren<TMP_Text>();
        }
    }

    // Batalkan eksekusi jika Canvas tidak ditemukan agar terhindar dari error
    if (worldCanvas == null || floatingHintText == null) return;

    CancelInvoke(nameof(HideHint));
    floatingHintText.text = message;
    
    if (uiFollower != null && targetTransform != null)
    {
        uiFollower.SetTarget(targetTransform);
    }

    worldCanvas.SetActive(true);
}

public void HideHint()
{
    // --- TAMBAHAN: Proteksi null sebelum menonaktifkan ---
    if (worldCanvas != null)
    {
        worldCanvas.SetActive(false);
    }
}

// --- TAMBAHAN: Bersihkan referensi statis saat scene/objek dihancurkan ---
private void OnDestroy()
{
    // Agar siap diisi ulang oleh objek manager baru di scene berikutnya
    if (Instance == this)
    {
        Instance = null;
    }
}
}