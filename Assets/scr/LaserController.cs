using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Referensi Komponen Laser")]
    [SerializeField] private GameObject laserVisual; // Objek visual sinar laser (anak objek / sprite)
    [SerializeField] private Collider laserCollider; // Collider penghalang

    // Fungsi untuk mematikan laser
    public void TurnOffLaser()
    {
        if (laserVisual != null) laserVisual.SetActive(false);
        else gameObject.SetActive(false); // Cadangan: Jika visual tidak diisi, matikan objek utamanya

        if (laserCollider != null) laserCollider.enabled = false;
    }

    // Fungsi untuk menyalakan laser kembali
    public void TurnOnLaser()
    {
        if (laserVisual != null) laserVisual.SetActive(true);
        else gameObject.SetActive(true);

        if (laserCollider != null) laserCollider.enabled = true;
    }
}