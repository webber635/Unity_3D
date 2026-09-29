using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Referensi Panel UI")]
    [SerializeField] private GameObject mainMenuPanel;    
    [SerializeField] private GameObject levelSelectPanel; 
    [SerializeField] private GameObject creditsPanel; // --- TAMBAHAN: Panel Credits ---

    [Header("Animasi Tombol Main Menu")]
    [Tooltip("Masukkan tombol-tombol yang ingin dianimasikan")]
    [SerializeField] private RectTransform[] animatedButtons; 
    [SerializeField] private float slideOffset = 500f;        
    [SerializeField] private float animationDuration = 0.5f;  
    [SerializeField] private float delayBetweenButtons = 0.15f; 

    [Header("Animasi Panel (Level Select & Credits)")]
    [SerializeField] private RectTransform levelSelectRect; 
    [SerializeField] private RectTransform creditsRect; // --- TAMBAHAN: Rect untuk Credits ---
    [SerializeField] private float panelSlideOffset = 1000f; 
    [SerializeField] private float panelAnimDuration = 0.4f;

    // Variabel posisi
    private Vector2 levelSelectOriginalPos;
    private Vector2 levelSelectHiddenPos;
    private Coroutine levelSelectAnimCoroutine;

    private Vector2 creditsOriginalPos;
    private Vector2 creditsHiddenPos;
    private Coroutine creditsAnimCoroutine;

    void Start()
    {
        // 1. Setup Animasi Panel Level Select
        if (levelSelectRect != null)
        {
            levelSelectOriginalPos = levelSelectRect.anchoredPosition;
            levelSelectHiddenPos = levelSelectOriginalPos - new Vector2(0, panelSlideOffset);
            levelSelectRect.anchoredPosition = levelSelectHiddenPos; 
        }

        // 2. Setup Animasi Panel Credits
        if (creditsRect != null)
        {
            creditsOriginalPos = creditsRect.anchoredPosition;
            creditsHiddenPos = creditsOriginalPos - new Vector2(0, panelSlideOffset);
            creditsRect.anchoredPosition = creditsHiddenPos; 
        }

        // 3. Tampilan Awal Menu
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // 4. Mulai animasi tombol meluncur
        if (animatedButtons != null && animatedButtons.Length > 0)
        {
            StartCoroutine(AnimateButtonsRoutine());
        }
    }

    private System.Collections.IEnumerator AnimateButtonsRoutine()
    {
        Vector2[] originalPositions = new Vector2[animatedButtons.Length];
        
        for (int i = 0; i < animatedButtons.Length; i++)
        {
            if (animatedButtons[i] != null)
            {
                originalPositions[i] = animatedButtons[i].anchoredPosition;
                animatedButtons[i].anchoredPosition = originalPositions[i] - new Vector2(0, slideOffset);
            }
        }

        for (int i = 0; i < animatedButtons.Length; i++)
        {
            if (animatedButtons[i] != null)
            {
                StartCoroutine(SlideInButton(animatedButtons[i], originalPositions[i]));
                yield return new WaitForSeconds(delayBetweenButtons);
            }
        }
    }

    private System.Collections.IEnumerator SlideInButton(RectTransform button, Vector2 targetPosition)
    {
        Vector2 startPosition = button.anchoredPosition;
        float timer = 0f;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, timer / animationDuration);
            button.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        button.anchoredPosition = targetPosition;
    }

    // ================= FUNGSI LEVEL SELECT =================
    public void OpenLevelSelect()
    {
        if (levelSelectPanel != null) 
        {
            levelSelectPanel.SetActive(true);
            if (levelSelectAnimCoroutine != null) StopCoroutine(levelSelectAnimCoroutine);
            levelSelectAnimCoroutine = StartCoroutine(AnimateGenericPanelRoutine(levelSelectRect, levelSelectPanel, levelSelectOriginalPos, false));
        }
    }

    public void CloseLevelSelect()
    {
        if (levelSelectPanel != null) 
        {
            if (levelSelectAnimCoroutine != null) StopCoroutine(levelSelectAnimCoroutine);
            levelSelectAnimCoroutine = StartCoroutine(AnimateGenericPanelRoutine(levelSelectRect, levelSelectPanel, levelSelectHiddenPos, true));
        }
    }

    // ================= FUNGSI CREDITS =================
    public void OpenCredits()
    {
        if (creditsPanel != null) 
        {
            creditsPanel.SetActive(true);
            if (creditsAnimCoroutine != null) StopCoroutine(creditsAnimCoroutine);
            creditsAnimCoroutine = StartCoroutine(AnimateGenericPanelRoutine(creditsRect, creditsPanel, creditsOriginalPos, false));
        }
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) 
        {
            if (creditsAnimCoroutine != null) StopCoroutine(creditsAnimCoroutine);
            creditsAnimCoroutine = StartCoroutine(AnimateGenericPanelRoutine(creditsRect, creditsPanel, creditsHiddenPos, true));
        }
    }

    // ================= FUNGSI ANIMASI GENERIK =================
    // Fungsi ini sekarang bisa dipakai oleh panel mana saja (bisa hemat kode)
    private System.Collections.IEnumerator AnimateGenericPanelRoutine(RectTransform rect, GameObject panel, Vector2 targetPos, bool hideOnComplete)
    {
        if (rect == null) yield break;

        Vector2 startPos = rect.anchoredPosition;
        float timer = 0f;

        while (timer < panelAnimDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, timer / panelAnimDuration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        rect.anchoredPosition = targetPos;

        if (hideOnComplete && panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Game ditutup.");
        Application.Quit();
    }
}