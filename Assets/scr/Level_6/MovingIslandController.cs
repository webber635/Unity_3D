using System.Collections;
using UnityEngine;

public class MovingIslandController : MonoBehaviour
{
    [Header("Referensi Objek")]
    [Tooltip("Masukkan parent object dari bongkahan tiles yang terpisah")]
    [SerializeField] private Transform islandTransform;
    
    [Tooltip("Blok transparan penahan jalan agar robot tidak jatuh/menyeberang sebelum waktunya")]
    [SerializeField] private GameObject invisibleBlocker;

    [Header("Pengaturan Gerak")]
    [Tooltip("Arah gerak per step. Ubah nilainya ke 1 atau -1 di sumbu X atau Z sesuai desain level Anda")]
    [SerializeField] private Vector3 moveDirection = new Vector3(0, 0, 1); 
    [SerializeField] private float gridSize = 1f; // Sesuaikan dengan ukuran grid di pergerakan player
    [SerializeField] private float stepDuration = 0.5f; // Kecepatan geser per petak
    [SerializeField] private float delayBetweenSteps = 0.2f; // Jeda sebelum geser lagi

    private Vector3 initialPosition;

    [Header("Pengaturan Jebakan")]
    [Tooltip("Berapa petak pulau mundur saat pemain melangkah")]
    [SerializeField] private int trapSteps = 3;

    private Vector3 connectedPosition;
    private Vector3 disconnectedPosition;
    
    // --- Variabel Deteksi Pemain ---
    private bool hasTriggeredTrap = false;
    private PlayerGridMovement playerMovement;
    private Vector3 playerInitialPosition;
    private bool isReadyToCheck = false;

    void Start()
    {
        if (islandTransform != null)
        {
            connectedPosition = islandTransform.position; // Posisi di Editor (menyatu)
            
            // Hitung posisi menjauh (berlawanan arah dengan moveDirection)
            disconnectedPosition = connectedPosition - (moveDirection.normalized * gridSize * trapSteps);
        }

        // Blocker mati di awal karena jembatan menyatu
        if (invisibleBlocker != null) invisibleBlocker.SetActive(false);

        playerMovement = FindFirstObjectByType<PlayerGridMovement>();
    }

    void Update()
    {
        if (hasTriggeredTrap || playerMovement == null) return;

        // 1. Tunggu Jet Entrance selesai
        if (!isReadyToCheck)
        {
            if (playerMovement.enabled) 
            {
                playerInitialPosition = playerMovement.transform.position;
                isReadyToCheck = true;
            }
            return;
        }

        // 2. Cek pergerakan sumbu X dan Z
        Vector3 currentPos = playerMovement.transform.position;
        float distance = Vector2.Distance(
            new Vector2(currentPos.x, currentPos.z), 
            new Vector2(playerInitialPosition.x, playerInitialPosition.z)
        );

        if (distance > 0.1f)
        {
            hasTriggeredTrap = true;
            StartCoroutine(TrapSequenceRoutine());
        }
    }

    private IEnumerator TrapSequenceRoutine()
    {
        // Kunci input agar tidak bisa jalan, tapi biarkan lompatan pertama selesai
        if (playerMovement != null) playerMovement.enabled = false;
        
        // Aktifkan blocker agar player tertahan di tepi
        if (invisibleBlocker != null) invisibleBlocker.SetActive(true);

        // Animasi pulau mundur (menjauh)
        for (int i = 0; i < trapSteps; i++)
        {
            Vector3 startPos = islandTransform.position;
            Vector3 targetPos = startPos - (moveDirection.normalized * gridSize);
            
            float timer = 0f;
            while (timer < stepDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, timer / stepDuration);
                islandTransform.position = Vector3.Lerp(startPos, targetPos, progress);
                yield return null;
            }
            islandTransform.position = targetPos;
            yield return new WaitForSeconds(delayBetweenSteps);
        }

        if (playerMovement != null) playerMovement.enabled = true;
    }

    // ... (kode Update dan TrapSequenceRoutine di atasnya) ...

    // FUNGSI INI YANG HILANG DAN HARUS DITAMBAHKAN KEMBALI
    public void StartMovingSequence(int steps, bool isSuccess)
    {
        StartCoroutine(MoveRoutine(steps, isSuccess));
    }

    private IEnumerator MoveRoutine(int steps, bool isSuccess)
    {
        // Selalu mulai dari posisi terputus saat mengeksekusi terminal
        if (islandTransform != null) islandTransform.position = disconnectedPosition;
        if (invisibleBlocker != null) invisibleBlocker.SetActive(true);

        int maxAnimSteps = Mathf.Min(steps, trapSteps + 1); 

        for (int i = 0; i < maxAnimSteps; i++)
        {
            Vector3 startPos = islandTransform.position;
            Vector3 targetPos = startPos + (moveDirection.normalized * gridSize);
            
            float timer = 0f;
            while (timer < stepDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, timer / stepDuration);
                islandTransform.position = Vector3.Lerp(startPos, targetPos, progress);
                yield return null;
            }
            
            islandTransform.position = targetPos;
            yield return new WaitForSeconds(delayBetweenSteps);
        }

        if (isSuccess)
        {
            if (invisibleBlocker != null) invisibleBlocker.SetActive(false);
            Debug.Log("Platform berhasil tersambung!");
        }
    }
}
