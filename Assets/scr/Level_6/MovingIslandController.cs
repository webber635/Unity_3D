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

    void Start()
    {
        if (islandTransform != null)
        {
            initialPosition = islandTransform.position;
        }

        if (invisibleBlocker != null)
        {
            invisibleBlocker.SetActive(true);
        }
    }

    // Dipanggil oleh Terminal setelah pemain mengeksekusi kode
    public void StartMovingSequence(int steps, bool isSuccess)
    {
        StartCoroutine(MoveRoutine(steps, isSuccess));
    }

    private IEnumerator MoveRoutine(int steps, bool isSuccess)
    {
        // 1. Reset posisi ke awal dan aktifkan blocker (jika pemain mencoba ulang / revisi kode)
        if (islandTransform != null) islandTransform.position = initialPosition;
        if (invisibleBlocker != null) invisibleBlocker.SetActive(true);

        // 2. Batasi maksimal animasi langkah agar bongkahan tidak terbang keluar map jika pemain input angka 1000
        int maxAnimSteps = Mathf.Min(steps, 4); 

        // 3. Loop pergerakan sesuai jumlah iterasi
        for (int i = 0; i < maxAnimSteps; i++)
        {
            Vector3 startPos = islandTransform.position;
            
            // Hitung posisi target untuk maju 1 petak sesuai arah
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
            
            // Jeda sejenak sebelum langkah loop berikutnya
            yield return new WaitForSeconds(delayBetweenSteps);
        }

        // 4. Jika logika kodenya benar (power mencapai target), buka jalan
        if (isSuccess)
        {
            if (invisibleBlocker != null) invisibleBlocker.SetActive(false);
            Debug.Log("Platform berhasil tersambung!");
        }
    }
}