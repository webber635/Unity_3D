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

    void Start()
    {
        // 1. Pastikan blocker aktif di awal saat jembatan putus
        if (invisibleBlocker != null)
        {
            invisibleBlocker.SetActive(true);
        }

        targetPositions = new Vector3[tilesToRestore.Length];
        startPositions = new Vector3[tilesToRestore.Length];

        for (int i = 0; i < tilesToRestore.Length; i++)
        {
            if (tilesToRestore[i] != null)
            {
                // Simpan posisi target (posisi sejajar grid saat ini)
                targetPositions[i] = tilesToRestore[i].position;
                
                // Tentukan posisi awal (jatuh ke bawah)
                startPositions[i] = targetPositions[i] - new Vector3(0, dropDistance, 0);
                
                // Pindahkan tile ke posisi jatuh saat game dimulai
                tilesToRestore[i].position = startPositions[i];
            }
        }
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