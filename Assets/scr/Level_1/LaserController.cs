using System.Collections;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Referensi Visual")]
    [Tooltip("Masukkan semua objek visual (tabung/silinder) laser ke sini")]
    // Ubah menjadi Array (tanda kurung siku [])
    [SerializeField] private MeshRenderer[] laserRenderers;

    [Tooltip("Masukkan Collider yang membunuh pemain (Hazard) di sini")]
    [SerializeField] private Collider hazardCollider;

    [Header("Pengaturan Efek Kedip")]
    [SerializeField] private int blinkCount = 4;
    [SerializeField] private float blinkDuration = 1f;

    public void TurnOffLaser()
    {
        StartCoroutine(DeactivationRoutine());
    }

    private IEnumerator DeactivationRoutine()
    {
        // 1. Matikan bahayanya lebih dulu
        if (hazardCollider != null) hazardCollider.enabled = false;

        // 2. Mainkan efek kedip
        if (laserRenderers != null && laserRenderers.Length > 0)
        {
            float timePerBlink = blinkDuration / (blinkCount * 2);

            for (int i = 0; i < blinkCount; i++)
            {
                // Tetap nyala (atau nyalakan lagi)
                SetRenderersState(true);
                yield return new WaitForSeconds(timePerBlink);

                // Mati sejenak
                SetRenderersState(false);
                yield return new WaitForSeconds(timePerBlink);
            }

            // 3. Matikan visual secara permanen di akhir
            SetRenderersState(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Fungsi bantuan untuk mengubah status semua silinder sekaligus
    private void SetRenderersState(bool isEnabled)
    {
        foreach (var renderer in laserRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = isEnabled;
            }
        }
    }
}