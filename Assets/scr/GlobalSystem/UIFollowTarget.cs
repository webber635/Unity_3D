using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    [Header("Pengaturan Target")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2.2f, 0); // Atur tinggi dari terminal
    
    private Transform currentTarget;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // --- FUNGSI BARU: Untuk menerima target dinamis (Terminal) ---
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    void LateUpdate()
    {
        // 1. Ikuti posisi target (Terminal)
        if (currentTarget != null)
        {
            transform.position = currentTarget.position + offset;
        }

        // 2. Selalu menghadap ke kamera (Billboard effect)
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
        else
        {
            mainCamera = Camera.main;
        }
    }
}