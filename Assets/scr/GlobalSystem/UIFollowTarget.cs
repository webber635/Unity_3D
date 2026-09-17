using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    [Header("Pengaturan Target")]
    [SerializeField] private Transform target; // Masukkan objek Player/Robot ke sini
    [SerializeField] private Vector3 offset = new Vector3(0, 2.2f, 0); // Jarak tinggi dari player

    private Quaternion fixedRotation;

    void Start()
    {
        // Kunci rotasi agar statis menghadap kamera isometrik sejak awal
        if (Camera.main != null)
        {
            fixedRotation = Camera.main.transform.rotation;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Hanya update pergerakan koordinat (Posisi)
            transform.position = target.position + offset;

            // 2. Pastikan rotasi tidak berubah (Statis)
            transform.rotation = fixedRotation;
        }
    }
}