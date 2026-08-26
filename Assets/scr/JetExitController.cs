using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class JetExitController : MonoBehaviour
{
    // Membuat daftar pilihan level untuk ditampilkan di Inspector
    public enum LevelIdentifier
    {
        Level1,
        Level2
    }

    [Header("Identitas Level (Pilih Sesuai Scene!)")]
    [SerializeField] private LevelIdentifier currentLevel = LevelIdentifier.Level1;

    [Header("Pengaturan Jet")]
    [SerializeField] private float warmUpDelay = 1.0f;  
    [SerializeField] private float flySpeed = 5f;       
    [SerializeField] private float flyDuration = 1.5f;   
    
    [Header("Referensi Visual (Opsional)")]
    [SerializeField] private GameObject jetFireEffect;  

    private bool isActivated = false;

    void Start()
    {
        if (jetFireEffect != null)
        {
            jetFireEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            Transform player = other.transform;

            var movement = player.GetComponent<PlayerGridMovement>();
            if (movement != null) movement.enabled = false;

            StartCoroutine(JetSequenceRoutine(player));
        }
    }

    IEnumerator JetSequenceRoutine(Transform player)
    {
        // --- TAHAP 1: WARM-UP ---
        Debug.Log("Mesin jet menyala... bersiap akselerasi!");
        
        if (jetFireEffect != null) jetFireEffect.SetActive(true);

        yield return new WaitForSeconds(warmUpDelay);

        // --- TAHAP 2: MENGIKAT PLAYER ---
        Vector3 oldLocalPos = player.localPosition;
        player.SetParent(transform);
        
        Vector3 fixedPos = player.localPosition;
        fixedPos.y = 0.7f; 
        player.localPosition = fixedPos;

        // --- TAHAP 3: TERBANG ---
        float timer = 0f;
        while (timer < flyDuration)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.up * flySpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // --- TAHAP 4: MUNCULKAN PANEL KEMENANGAN DINAMIS ---
        int errors = 0;
        int warnings = 0;

        // Ambil data skor berdasarkan settingan dropdown di Inspector
        if (currentLevel == LevelIdentifier.Level1)
        {
            errors = TerminalController1.totalErrors;
            warnings = TerminalController1.totalWarnings;
        }
        else if (currentLevel == LevelIdentifier.Level2)
        {
            errors = TerminalControllerLevel2.totalErrors;
            warnings = TerminalControllerLevel2.totalWarnings;
        }

        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.ShowVictoryPanel(errors, warnings);
        }
        else
        {
            Debug.LogError("VictoryManager tidak ditemukan di scene!");
        }
    }
}