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
    // private Vector2 collapsedPosition; // <-- HAPUS ATAU KOMENTARI INI
    private bool isExpanded = false;
    private Coroutine animationCoroutine;

    void Start()
    {
        expandedPositions = new Vector2[subButtons.Length];
        for (int i = 0; i < subButtons.Length; i++)
        {
            expandedPositions[i] = subButtons[i].anchoredPosition;
        }
        
        // Sembunyikan tombol-tombol di awal game dengan menempelkannya ke posisi gearButton
        for (int i = 0; i < subButtons.Length; i++)
        {
            subButtons[i].anchoredPosition = gearButton.anchoredPosition;
            subButtons[i].localScale = Vector3.zero; 
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
        
        Vector2[] startPositions = new Vector2[subButtons.Length];
        Vector3 startScale = subButtons[0].localScale;
        Vector3 targetScale = expand ? Vector3.one : Vector3.zero;
        
        Quaternion startRotation = gearButton.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, expand ? gearRotationAngle : 0);

        for (int i = 0; i < subButtons.Length; i++)
        {
            startPositions[i] = subButtons[i].anchoredPosition;
        }

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            
            float smoothStep = Mathf.SmoothStep(0, 1, t);

            // 1. Putar tombol gear
            gearButton.localRotation = Quaternion.Lerp(startRotation, targetRotation, smoothStep);

            // BACA DINAMIS: Ambil posisi gear saat ini (berjaga-jaga jika gear sedang meluncur dari bawah)
            Vector2 dynamicCollapsedPos = gearButton.anchoredPosition;

            // 2. Pindahkan dan ubah ukuran sub-buttons
            for (int i = 0; i < subButtons.Length; i++)
            {
                Vector2 targetPos = expand ? expandedPositions[i] : dynamicCollapsedPos;
                subButtons[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPos, smoothStep);
                subButtons[i].localScale = Vector3.Lerp(startScale, targetScale, smoothStep);
            }

            yield return null;
        }

        gearButton.localRotation = targetRotation;
        Vector2 finalCollapsedPos = gearButton.anchoredPosition;

        for (int i = 0; i < subButtons.Length; i++)
        {
            subButtons[i].anchoredPosition = expand ? expandedPositions[i] : finalCollapsedPos;
            subButtons[i].localScale = targetScale;
        }
    }
}