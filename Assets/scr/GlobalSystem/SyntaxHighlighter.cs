using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_InputField))]
public class SyntaxHighlighter : MonoBehaviour, IPointerDownHandler
{
    private TMP_InputField inputField;
    private Coroutine blinkCoroutine;

    [Header("Pengaturan Kedipan")]
    [SerializeField] private Color color1 = Color.green;
    [SerializeField] private Color color2 = Color.yellow;
    [SerializeField] private float blinkSpeed = 0.3f;

    private Color originalColor;
    private bool hasInteracted = false;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        
        if (inputField.textComponent != null)
        {
            originalColor = inputField.textComponent.color; 
        }
    }

    private void OnEnable()
    {
        hasInteracted = false; 
        StartBlinking();
        
        // Beri jeda 1 frame agar pengisian teks dari TerminalController
        // tidak dianggap sebagai ketikan oleh pemain.
        StartCoroutine(DelayAddListener());
    }

    private void OnDisable()
    {
        StopBlinking();
        // Bersihkan listener saat ditutup
        inputField.onValueChanged.RemoveListener(OnInputValueChanged);
    }

    private IEnumerator DelayAddListener()
    {
        // Tunggu sampai seluruh proses di frame ini selesai (termasuk inputField.text = "...")
        yield return new WaitForEndOfFrame();
        
        inputField.onValueChanged.RemoveListener(OnInputValueChanged); // Mencegah dobel
        inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void StopBlinking()
    {
        hasInteracted = true;
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        
        if (inputField != null && inputField.textComponent != null)
        {
            inputField.textComponent.color = originalColor;
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (!hasInteracted)
        {
            inputField.textComponent.color = color1;
            // Gunakan Realtime agar kedipan tetap jalan jika suatu saat game di-pause
            yield return new WaitForSecondsRealtime(blinkSpeed); 
            
            inputField.textComponent.color = color2;
            yield return new WaitForSecondsRealtime(blinkSpeed);
        }
    }

    // Berhenti berkedip jika diklik dengan Mouse
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasInteracted) StopBlinking();
    }

    // Berhenti berkedip jika pemain mengetik di keyboard
    private void OnInputValueChanged(string newValue)
    {
        if (!hasInteracted) StopBlinking();
    }
}