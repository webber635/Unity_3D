using System.Collections;
using UnityEngine;

public class TileBridgeController : MonoBehaviour
{
    [Header("Referensi Tiles")]
    [Tooltip("Masukkan semua objek tile sesuai URUTAN mereka akan naik")]
    [SerializeField] private Transform[] tilesToRestore;

    [Header("Penghalang Jalan (Invisible Wall)")]
    [Tooltip("Masukkan objek blok transparan yang menghalangi player saat jembatan putus")]
    [SerializeField] private GameObject invisibleBlocker;

    [Header("Pengaturan Animasi")]
    [SerializeField] private float dropDistance = 5.0f; // Seberapa jauh tile jatuh ke bawah
    [SerializeField] private float riseDuration = 1.0f; // Durasi animasi naik (per tile)
    [SerializeField] private float cascadeDelay = 0.2f; // Jeda waktu antar tile mulai naik

    private Vector3[] targetPositions; 
    private Vector3[] startPositions;  
    private bool isRestored = false;
    
    // --- TAMBAHAN VARIABEL ---
    private bool hasDropped = false;
    private PlayerGridMovement playerMovement;
    private Vector3 playerInitialPosition;
    private bool isReadyToCheck = false; // Penanda Jet Entrance sudah selesai

    void Start()
    {
        // Blocker dinonaktifkan di awal karena jembatan masih utuh
        if (invisibleBlocker != null) invisibleBlocker.SetActive(false);

        targetPositions = new Vector3[tilesToRestore.Length];
        startPositions = new Vector3[tilesToRestore.Length];

        for (int i = 0; i < tilesToRestore.Length; i++)
        {
            if (tilesToRestore[i] != null)
            {
                targetPositions[i] = tilesToRestore[i].position;
                startPositions[i] = targetPositions[i] - new Vector3(0, dropDistance, 0);
                
                // BIARKAN POSISI TILE TETAP DI ATAS SAAT START
            }
        }

        // Cari player, tapi jangan catat posisinya dulu
        playerMovement = FindFirstObjectByType<PlayerGridMovement>();
    }

    void Update()
    {
        if (hasDropped || playerMovement == null) return;

        // 1. Tunggu animasi Jet Entrance selesai
        if (!isReadyToCheck)
        {
            // JetEntranceController akan meng-enable movement saat mendarat
            if (playerMovement.enabled) 
            {
                // Baru catat posisi asli di atas grid
                playerInitialPosition = playerMovement.transform.position;
                isReadyToCheck = true;
            }
            return;
        }

        // 2. Cek pergerakan hanya di sumbu X dan Z (Abaikan perubahan tinggi / Y)
        Vector3 currentPos = playerMovement.transform.position;
        float distance = Vector2.Distance(
            new Vector2(currentPos.x, currentPos.z), 
            new Vector2(playerInitialPosition.x, playerInitialPosition.z)
        );

        if (distance > 0.1f)
        {
            hasDropped = true;
            StartCoroutine(CascadeDropRoutine());
        }
    }

    private IEnumerator CascadeDropRoutine()
    {
        // 1. Matikan input player, tapi JANGAN hentikan Coroutines.
        // Dengan begini, animasi lompatan langkah pertama player akan tetap 
        // diselesaikan dengan mulus sebelum dia terdiam menunggu tiles jatuh.
        if (playerMovement != null) playerMovement.enabled = false;

        // 2. Aktifkan blocker agar player tertahan di tepi
        if (invisibleBlocker != null) invisibleBlocker.SetActive(true);

        // 3. Jalankan animasi jatuh berurutan
        for (int i = 0; i < tilesToRestore.Length; i++)
        {
            if (tilesToRestore[i] != null)
            {
                StartCoroutine(DropSingleTileRoutine(i));
                yield return new WaitForSeconds(cascadeDelay);
            }
        }

        // Tunggu hingga tile terakhir selesai jatuh
        yield return new WaitForSeconds(riseDuration); 

        // 4. Buka kembali kunci gerakan player
        if (playerMovement != null) playerMovement.enabled = true;
    }

    private IEnumerator DropSingleTileRoutine(int index)
    {
        float timer = 0f;
        Transform tile = tilesToRestore[index];

        while (timer < riseDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, timer / riseDuration);
            
            if (tile != null) 
                tile.position = Vector3.Lerp(targetPositions[index], startPositions[index], progress);
            
            yield return null;
        }

        if (tile != null) tile.position = startPositions[index];
    }

    public void RestoreTiles()
    {
        if (!isRestored)
        {
            isRestored = true;
            StartCoroutine(CascadeRiseRoutine());
        }
    }

    private IEnumerator CascadeRiseRoutine()
    {
        // Jalankan animasi untuk setiap tile secara berurutan
        for (int i = 0; i < tilesToRestore.Length; i++)
        {
            if (tilesToRestore[i] != null)
            {
                StartCoroutine(RiseSingleTileRoutine(i));
                yield return new WaitForSeconds(cascadeDelay);
            }
        }
    }

    private IEnumerator RiseSingleTileRoutine(int index)
    {
        float timer = 0f;
        Transform tile = tilesToRestore[index];

        while (timer < riseDuration)
        {
            timer += Time.deltaTime;
            
            float progress = Mathf.SmoothStep(0f, 1f, timer / riseDuration);

            if (tile != null)
            {
                tile.position = Vector3.Lerp(startPositions[index], targetPositions[index], progress);
            }
            yield return null;
        }

        // Kunci posisi persis di target
        if (tile != null)
        {
            tile.position = targetPositions[index];
        }
        
        // 2. MATIKAN BLOCKER JIKA INI ADALAH TILE TERAKHIR
        if (index == tilesToRestore.Length - 1)
        {
            if (invisibleBlocker != null)
            {
                invisibleBlocker.SetActive(false);
            }
            Debug.Log("Seluruh jalur cascade dipulihkan, Blocker dihilangkan!");
        }
    }
}