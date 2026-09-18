using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SecuredJetExitController : MonoBehaviour
{
    public enum LevelIdentifier
    {
        Level1,
        Level2,
        Level3 // Tambahan untuk Level 3
    }

    [Header("Identitas Level")]
    [SerializeField] private LevelIdentifier currentLevel = LevelIdentifier.Level3;

    [Header("Pengaturan Jet")]
    [SerializeField] private float warmUpDelay = 1.0f;  
    [SerializeField] private float flySpeed = 5f;       
    [SerializeField] private float flyDuration = 1.5f;   
    
    [Header("Referensi Visual")]
    [SerializeField] private GameObject jetFireEffect;  
    [SerializeField] private MeshRenderer tileRenderer; // Tarik komponen MeshRenderer dari Tile ke sini
    
    [Header("Status Keamanan")]
    public bool isLocked = true; // Awalnya terkunci

    private Color originalColor;
    private bool isActivated = false;
    private bool isFlashing = false; // Mencegah spam animasi kedip

    void Start()
    {
        if (jetFireEffect != null) jetFireEffect.SetActive(false);
        if (tileRenderer != null) originalColor = tileRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            if (isLocked)
            {
                // Jika terkunci, mainkan efek Access Denied
                if (!isFlashing) StartCoroutine(AccessDeniedRoutine());
            }
            else
            {
                // Jika terbuka, jalankan sekuens terbang
                isActivated = true;
                Transform player = other.transform;

                // Matikan pergerakan agar robot tidak bisa jalan saat terbang
                var movement = player.GetComponent<PlayerGridMovement>();
                if (movement != null) movement.enabled = false;

                StartCoroutine(JetSequenceRoutine(player));
            }
        }
    }

    private IEnumerator AccessDeniedRoutine()
    {
        isFlashing = true;

        // Menampilkan pesan error UI menggunakan sistem HUD yang sudah ada
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.ShowHint("ACCESS DENIED: Required Level 2");
        }

        // Efek visual tile berkedip merah 3 kali
        if (tileRenderer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                tileRenderer.material.color = Color.red;
                yield return new WaitForSeconds(0.15f);
                tileRenderer.material.color = originalColor;
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        yield return new WaitForSeconds(1.0f);
        if (HUDManager.Instance != null) HUDManager.Instance.HideHint();
        
        isFlashing = false;
    }

    public void UnlockExit()
    {
        isLocked = false;
        // Opsional: Beri warna hijau pudar atau biarkan kembali ke warna asli sebagai tanda terbuka
        if (tileRenderer != null) tileRenderer.material.color = originalColor;
        Debug.Log("Security Bypass: Jet Exit Terbuka!");
    }

    // Fungsi terbang yang sama persis dengan buatan Anda sebelumnya
    IEnumerator JetSequenceRoutine(Transform player)
    {
        Debug.Log("Mesin jet menyala... bersiap akselerasi!");
        if (jetFireEffect != null) jetFireEffect.SetActive(true);

        yield return new WaitForSeconds(warmUpDelay);

        Vector3 oldLocalPos = player.localPosition;
        player.SetParent(transform);
        
        Vector3 fixedPos = player.localPosition;
        fixedPos.y = 0.7f; 
        player.localPosition = fixedPos;

        float timer = 0f;
        while (timer < flyDuration)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.up * flySpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        int errors = 0;
        int warnings = 0;

        if (currentLevel == LevelIdentifier.Level3)
        {
            errors = TerminalController2.totalErrors;
            warnings = TerminalController2.totalWarnings;
        }

        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.ShowVictoryPanel(errors, warnings);
        }
    }
}