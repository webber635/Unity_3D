using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Referensi Tombol")]
    [SerializeField] private RectTransform gearButton;       // Tombol Setting Utama
    [SerializeField] private RectTransform[] subButtons;     // Tombol Tanya, Musik, Sound

    [Header("Pengaturan Animasi")]
    [SerializeField] private float animationDuration = 0.3f; // Kecepatan animasi
    [SerializeField] private float gearRotationAngle = 180f; // Derajat putaran gear

    private Vector2[] expandedPositions;
    private Vector2 collapsedPosition;
    private bool isExpanded = false;
    private Coroutine animationCoroutine;

    void Start()
    {
        // 1. Simpan posisi target (terbuka) dari masing-masing sub-button
        expandedPositions = new Vector2[subButtons.Length];
        for (int i = 0; i < subButtons.Length; i++)
        {
            expandedPositions[i] = subButtons[i].anchoredPosition;
        }
        
        // 2. Tentukan posisi tertutup (yaitu tersembunyi di balik tombol gear)
        collapsedPosition = gearButton.anchoredPosition;

        // 3. Sembunyikan tombol-tombol di awal game
        for (int i = 0; i < subButtons.Length; i++)
        {
            subButtons[i].anchoredPosition = collapsedPosition;
            subButtons[i].localScale = Vector3.zero; // Dikecilkan hingga hilang
        }
    }

    // Fungsi ini disambungkan ke Event OnClick() milik tombol Gear
    public void ToggleSettings()
    {
        // Hentikan animasi sebelumnya jika tombol ditekan dengan cepat (spam)
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        isExpanded = !isExpanded;
        animationCoroutine = StartCoroutine(AnimateSettings(isExpanded));
    }

    private IEnumerator AnimateSettings(bool expand)
    {
        float time = 0;
        
        // Ambil data posisi dan rotasi awal sebelum animasi dimulai
        Vector2[] startPositions = new Vector2[subButtons.Length];
        Vector3 startScale = subButtons[0].localScale;
        Vector3 targetScale = expand ? Vector3.one : Vector3.zero;
        
        Quaternion startRotation = gearButton.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, expand ? gearRotationAngle : 0);

        for (int i = 0; i < subButtons.Length; i++)
        {
            startPositions[i] = subButtons[i].anchoredPosition;
        }

        // Proses Lerp berjalan selama durasi animasi
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            
            // SmoothStep membuat gerakan melambat di akhir agar tidak kaku
            float smoothStep = Mathf.SmoothStep(0, 1, t);

            // 1. Putar tombol gear
            gearButton.localRotation = Quaternion.Lerp(startRotation, targetRotation, smoothStep);

            // 2. Pindahkan dan ubah ukuran sub-buttons
            for (int i = 0; i < subButtons.Length; i++)
            {
                Vector2 targetPos = expand ? expandedPositions[i] : collapsedPosition;
                subButtons[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPos, smoothStep);
                subButtons[i].localScale = Vector3.Lerp(startScale, targetScale, smoothStep);
            }

            yield return null;
        }

        // Pastikan posisi dan rotasi akhir terkunci dengan pas
        gearButton.localRotation = targetRotation;
        for (int i = 0; i < subButtons.Length; i++)
        {
            subButtons[i].anchoredPosition = expand ? expandedPositions[i] : collapsedPosition;
            subButtons[i].localScale = targetScale;
        }
    }
}