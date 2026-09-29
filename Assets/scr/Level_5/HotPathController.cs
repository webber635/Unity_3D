using UnityEngine;
using System.Collections; // Tambahkan ini untuk IEnumerator

public class HotPathController : MonoBehaviour
{
    [Header("Referensi Visual")]
    [SerializeField] private MeshRenderer pathRenderer;
    
    [Header("Pengaturan Warna & Animasi")]
    [SerializeField] private Color hotColor = Color.red;
    [SerializeField] private Color safeColor = Color.white;
    [SerializeField] private float transitionDuration = 1.5f; // Durasi animasi warna

    private bool hasActivated = false;
    private PlayerGridMovement playerMovement;
    private Vector3 playerInitialPosition;
    private bool isReadyToCheck = false;

    private void Start()
    {
        // Kondisi awal aman (putih) dan tidak berbahaya
        if (pathRenderer != null) pathRenderer.material.color = safeColor;
        gameObject.tag = "Untagged";

        playerMovement = FindFirstObjectByType<PlayerGridMovement>();
    }

    void Update()
    {
        if (hasActivated || playerMovement == null) return;

        // 1. Tunggu Jet Entrance selesai dan catat posisi awal
        if (!isReadyToCheck)
        {
            if (playerMovement.enabled) 
            {
                playerInitialPosition = playerMovement.transform.position;
                isReadyToCheck = true;
            }
            return;
        }

        // 2. Cek pergerakan sumbu X dan Z saat mengambil langkah pertama
        Vector3 currentPos = playerMovement.transform.position;
        float distance = Vector2.Distance(
            new Vector2(currentPos.x, currentPos.z), 
            new Vector2(playerInitialPosition.x, playerInitialPosition.z)
        );

        if (distance > 0.1f)
        {
            hasActivated = true;
            StartCoroutine(SetPathHotCinematic());
        }
    }

    private IEnumerator SetPathHotCinematic()
    {
        // Kunci input player, tapi biarkan animasi langkah pertamanya selesai
        if (playerMovement != null) playerMovement.enabled = false;
        
        // Langsung ubah tag jadi Hazard agar mematikan jika tersentuh
        gameObject.tag = "Hazard";

        // Animasi transisi warna perlahan dari putih ke merah
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            if (pathRenderer != null)
            {
                pathRenderer.material.color = Color.Lerp(safeColor, hotColor, timer / transitionDuration);
            }
            yield return null;
        }

        if (pathRenderer != null) pathRenderer.material.color = hotColor;

        // Kembalikan kendali pemain
        if (playerMovement != null) playerMovement.enabled = true;
    }

    // Dipanggil oleh Terminal saat jawaban benar
    public void SetPathSafeCinematic()
    {
        StartCoroutine(SetPathSafeRoutine());
    }

    private IEnumerator SetPathSafeRoutine()
    {
        // Langsung lepas tag Hazard agar robot aman
        gameObject.tag = "Untagged";
        
        // Animasi transisi warna perlahan dari merah ke putih
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            if (pathRenderer != null)
            {
                pathRenderer.material.color = Color.Lerp(hotColor, safeColor, timer / transitionDuration);
            }
            yield return null;
        }

        if (pathRenderer != null) pathRenderer.material.color = safeColor;
        Debug.Log("Jalur mendingin dan sekarang aman dilewati.");
    }
}