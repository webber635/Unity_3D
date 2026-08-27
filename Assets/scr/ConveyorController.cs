using UnityEngine;
using System.Collections;

public class ConveyorController : MonoBehaviour
{
    [Header("Pengaturan Conveyor")]
    [SerializeField] private Transform keyObject;       // Objek kunci yang akan bergerak
    [SerializeField] private Transform startPoint;      // Titik awal spawn kunci
    [SerializeField] private float tileSize = 1.0f;     // Jarak 1 tile/kotak di Unity
    [SerializeField] private float moveSpeed = 2.0f;    // Kecepatan animasi gerak
    
    [Header("Aturan Jarak")]
    [SerializeField] private int requiredSteps = 5;     // Jawaban yang benar (misal: 5 kotak)

    private Vector3 initialPosition;

    void Start()
    {
        if (startPoint != null && keyObject != null)
        {
            keyObject.position = startPoint.position;
            initialPosition = startPoint.position;
        }
    }

    // Fungsi ini dipanggil oleh Terminal
    public void MoveKey(int playerSteps)
    {
        StartCoroutine(MoveKeyRoutine(playerSteps));
    }

    private IEnumerator MoveKeyRoutine(int steps)
    {
        // Hitung posisi target berdasarkan jumlah langkah * ukuran per tile
        // Asumsi conveyor bergerak searah sumbu Z (bisa diganti ke X atau Y sesuai layoutmu)
        Vector3 targetPos = initialPosition + (Vector3.forward * (steps * tileSize));

        // Animasi pergerakan step-based
        while (Vector3.Distance(keyObject.position, targetPos) > 0.01f)
        {
            keyObject.position = Vector3.MoveTowards(keyObject.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        keyObject.position = targetPos; // Pastikan posisi pas di akhir

        // Evaluasi Hasil 3 Kondisi
        EvaluateResult(steps);
    }

    private void EvaluateResult(int steps)
    {
        if (steps == requiredSteps)
        {
            Debug.Log("BERHASIL: Kunci sampai di Key Maker!");
            // Panggil animasi mesin pembuat kunci atau trigger kemenangan di sini
            TerminalControllerLevel3.Instance.TriggerSuccess();
        }
        else if (steps < requiredSteps)
        {
            Debug.Log("GAGAL: Kunci kurang jauh, belum sampai mesin finishing.");
            StartCoroutine(RespawnKey());
        }
        else if (steps > requiredSteps)
        {
            Debug.Log("GAGAL: Kunci kelewatan dan hancur di void/lava!");
            // Opsional: Mainkan partikel api/lava hancur di sini
            StartCoroutine(RespawnKey());
        }
    }

    private IEnumerator RespawnKey()
    {
        // Tunggu sebentar agar pemain bisa melihat akibat dari input mereka
        yield return new WaitForSeconds(1.5f);
        
        // Kembalikan ke titik awal
        keyObject.position = initialPosition;
        
        // Beri sinyal ke terminal bahwa mesin sudah di-reset dan siap menerima input lagi
        TerminalControllerLevel3.Instance.ResetTerminal();
    }
}