using System.Collections;
using UnityEngine;

public class PlayerGridMovement : MonoBehaviour
{

    public static bool isTerminalActive = false;

    [Header("Pengaturan Gerak")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;  
    [SerializeField] private LayerMask obstacleLayer; 

    [Header("Animasi Procedural")]
    [SerializeField] private Transform visualBody; 
    [SerializeField] private float hopHeight = 0.4f; 
    [SerializeField] private float tiltAngle = 15f;  

    private bool isMoving = false;

    private Vector3 respawnPosition; // Menyimpan titik awal spawn

    void Start()
    {
        // Wajib di-reset jadi false setiap kali level baru dimuat
        isTerminalActive = false; 

        respawnPosition = transform.position;
        transform.LookAt(transform.position + new Vector3(1, 0, 0));
    }

    void Update()
    {
        // --- CEK SAKLAR GLOBAL (Sangat ringan dan tidak hardcode) ---
        if (isTerminalActive) return;
        
        if (isMoving) return;

        // --- KONTROL ISOMETRIC (Sumbu X Positif sebagai Depan) ---
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            StartCoroutine(MovePlayer(new Vector3(1, 0, 0)));
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            StartCoroutine(MovePlayer(new Vector3(-1, 0, 0)));
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            StartCoroutine(MovePlayer(new Vector3(0, 0, 1)));
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            StartCoroutine(MovePlayer(new Vector3(0, 0, -1)));
        }
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * gridSize);

        // Posisi laser di area dada
        Vector3 rayStart = startPosition + new Vector3(0, 0.5f, 0);

        // Putar robot menghadap tujuan
        transform.LookAt(targetPosition);

        // Cek Rintangan
        if (Physics.Raycast(rayStart, direction, gridSize, obstacleLayer))
        {
            // Jika nabrak, mainkan animasi Bump, lalu hentikan fungsi MovePlayer ini
            StartCoroutine(BumpIntoWall(direction));
            yield break; 
        }

        isMoving = true;
        float elapsedTime = 0;
        float timeToMove = 1f / moveSpeed;

        while (elapsedTime < timeToMove)
        {
            float percent = elapsedTime / timeToMove; 

            // Gerak maju dan lompat (Normal)
            Vector3 basePosition = Vector3.Lerp(startPosition, targetPosition, percent);
            float currentHop = hopHeight * Mathf.Sin(percent * Mathf.PI);
            transform.position = basePosition + new Vector3(0, currentHop, 0);

            if (visualBody != null)
            {
                float currentTilt = tiltAngle * Mathf.Sin(percent * Mathf.PI);
                visualBody.localRotation = Quaternion.Euler(currentTilt, 0, 0);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset Posisi Akhir
        transform.position = targetPosition;
        if (visualBody != null)
        {
            visualBody.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        isMoving = false;
    }

    // --- FUNGSI BARU: ANIMASI MENABRAK TEMBOK ---
    // --- FUNGSI BARU: LOMPAT DI TEMPAT SAAT NABRAK ---
    private IEnumerator BumpIntoWall(Vector3 direction)
    {
        isMoving = true;
        
        // Simpan posisi awal (X, Y, Z mutlak)
        Vector3 startPosition = transform.position;

        float elapsedTime = 0;
        // Waktu animasi lebih cepat dari jalan biasa
        float bumpTime = (1f / moveSpeed) * 0.7f; 

        while (elapsedTime < bumpTime)
        {
            float percent = elapsedTime / bumpTime; 

            // Trik Matematika: Mathf.Sin(percent * PI) akan bernilai 0 -> 1 -> 0
            float pingPong = Mathf.Sin(percent * Mathf.PI);

            // 1. POSISI: X dan Z dikunci ke startPosition. Hanya Y yang naik turun!
            float currentHop = (hopHeight * 0.5f) * pingPong; // Lompatan lebih pendek
            transform.position = startPosition + new Vector3(0, currentHop, 0);

            // 2. ROTASI: Tetap menunduk ke depan
            if (visualBody != null)
            {
                float currentTilt = (tiltAngle * 0.7f) * pingPong;
                visualBody.localRotation = Quaternion.Euler(currentTilt, 0, 0);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- PENGUNCIAN MUTLAK ---
        // Pastikan posisi dikembalikan PERSIS ke angka awal tanpa ada desimal yang melenceng
        transform.position = startPosition;
        
        if (visualBody != null)
        {
            visualBody.localRotation = Quaternion.Euler(0, 0, 0);
        }

        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jika robot menyentuh objek ber-tag "Hazard" (Laser aktif)
        if (other.CompareTag("Hazard"))
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        // Hentikan semua coroutine gerak agar tidak bug
        StopAllCoroutines();
        isMoving = false;

        // Kembalikan posisi robot ke titik awal spawn
        transform.position = respawnPosition;

        Debug.Log("Player terkena laser! Kembali ke posisi awal.");
    }

    // Tambahkan fungsi ini untuk memperbarui titik respawn
    public void SetNewCheckpoint(Vector3 newPosition)
    {
        // Ganti "startPosition" dengan nama variabel yang Anda gunakan di script ini
        // untuk menyimpan koordinat respawn pemain.
        respawnPosition = newPosition;

        Debug.Log("Checkpoint baru telah diset di: " + newPosition);
    }
}