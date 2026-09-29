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
        CancelInvoke(nameof(HideHint));
        floatingHintText.text = message;
        
        // Jika ada target yang dikirim, pindahkan UI ke target tersebut
        if (uiFollower != null && targetTransform != null)
        {
            uiFollower.SetTarget(targetTransform);
        }

        worldCanvas.SetActive(true);
    }

    public void HideHint()
    {
        worldCanvas.SetActive(false);
    }
}