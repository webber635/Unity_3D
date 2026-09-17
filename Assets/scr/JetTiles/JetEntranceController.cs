using UnityEngine;
using System.Collections;

public class JetEntranceController : MonoBehaviour
{
    [Header("Pengaturan Posisi")]
    [SerializeField] private float dropDistance = 5.0f; // Jarak turun ke bawah dari posisi petak
    [SerializeField] private float entranceDuration = 1.0f; // Durasi detik meluncur naik

    [Header("Referensi Objek")]
    [SerializeField] private GameObject playerObject;   // Tarik objek Player ke sini
    [SerializeField] private GameObject jetFireEffect;  // Objek api jet (opsional)

    private Vector3 targetPosition;
    private Vector3 startPosition;

    void Start()
    
    {

        if (playerObject == null) return;

        // 1. Ambil posisi Y asli petak ini di Editor (misal: 0.7 atau berapapun posisinya)
        targetPosition = transform.position;

        // 2. Tentukan posisi awal dengan menurunkan sumbu Y sejauh dropDistance dari posisi petak
        startPosition = targetPosition - new Vector3(0, dropDistance, 0);

        // Pindahkan petak langsung ke posisi bawah
        transform.position = startPosition;

        // 3. Amankan posisi player agar ikut di petak dan pas di atas permukaannya
        playerObject.transform.SetParent(transform);
        
        // Mengunci posisi lokal player agar pas di atas petak (misalnya 0.5 di atas pivot petak)
        Vector3 localPos = playerObject.transform.localPosition;
        localPos.y = 0.7f; 
        playerObject.transform.localPosition = localPos;

        // Kamu bisa mengganti angka 90f di bawah ini sesuai arah hadap robot yang kamu inginkan
        Vector3 localRot = playerObject.transform.localEulerAngles;
        localRot.y = 90f; 
        playerObject.transform.localEulerAngles = localRot;

        // Matikan pergerakan player sementara
        var movement = playerObject.GetComponent<PlayerGridMovement>();
        if (movement != null) movement.enabled = false;

        // Nyalakan efek api jet
        if (jetFireEffect != null) jetFireEffect.SetActive(true);

        // 4. Mulai animasi meluncur naik
        StartCoroutine(EntranceRoutine(movement));
    }

    IEnumerator EntranceRoutine(PlayerGridMovement movement)
    {
        float timer = 0f;

        while (timer < entranceDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / entranceDuration;

            // Meluncur mulus dari posisi bawah ke posisi target asli petak
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // Pastikan posisi mendarat tepat 100% di posisi asli petak
        transform.position = targetPosition;

        // Matikan api jet
        if (jetFireEffect != null) jetFireEffect.SetActive(false);

        // Lepas parent agar player bisa bergerak bebas di atas grid
        playerObject.transform.SetParent(null);

        // Aktifkan kembali kontrol pergerakan player
        if (movement != null) movement.enabled = true;

        Debug.Log("Entrance selesai! Robot mendarat mulus di atas petak.");
    }
}