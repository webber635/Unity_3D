using UnityEngine;
using TMPro;

public class TerminalController2 : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target")]
    [SerializeField] private SecuredJetExitController targetJetExit; // Mengarah ke Jet Exit

    [Header("Aturan Puzzle Level 3")]
    [SerializeField] private int requiredLevel = 2;

    private PlayerGridMovement playerMovement;
    private bool isPlayerInRange = false;
    private bool isSolved = false;

    public static int totalErrors = 0;
    public static int totalWarnings = 0;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        totalErrors = 0;
        totalWarnings = 0;
    }

    void Update()
    {
        HandlePlayerInteraction();
        HandleTerminalInput();
    }

    private void HandlePlayerInteraction()
    {
        if (isPlayerInRange && !isSolved && Input.GetKeyDown(KeyCode.E))
        {
            if (terminalUI.activeSelf && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputField.gameObject) return; 
            ToggleTerminal();
        }
    }

    private void HandleTerminalInput()
    {
        if (terminalUI.activeSelf && !isSolved)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) EvaluateAnswer(); 
        }
    }

    private void ToggleTerminal()
    {
        bool isActive = !terminalUI.activeSelf;
        terminalUI.SetActive(isActive);
        PlayerGridMovement.isTerminalActive = isActive; // Blokir gerakan player saat ngetik

        if (isActive)
        {
            if (UISoundManager.Instance != null)
                {
                    UISoundManager.Instance.PlayTerminalOpen();
                }

            inputField.text = "1"; 
            feedbackText.text = "> Checking access...\nERROR: Access level too low.\nCurrent: 1\nRequired: 2";
            feedbackText.color = Color.yellow;
            inputField.ActivateInputField(); 
            inputField.caretPosition = inputField.text.Length; 
        }
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;
        string cleanInput = inputField.text.Trim();

        // 1. CEK SYNTAX
        if (!int.TryParse(cleanInput, out int accessLevel))
        {
            totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'access_level' must be an integer number.", Color.red);
            return;
        }

        // 2. CEK LOGIKA LOGICAL (IF)
        if (accessLevel >= requiredLevel)
        {
            OnPuzzleSuccess();
        }
        else
        {
            totalWarnings++; 
            OnPuzzleFailed($"> Checking access...\nERROR: Access level too low.\nCurrent: {accessLevel}\nRequired: {requiredLevel}", Color.yellow);
        }
    }

    private void OnPuzzleSuccess()
    {
        feedbackText.color = Color.green;
        feedbackText.text = "> Access granted.\nSecurity: BYPASSED";
        isSolved = true;

        PlayerPrefs.SetInt("Level3Unlocked", 1);
        PlayerPrefs.Save();

        // Tunda 1.5 detik agar pemain sempat membaca teks hijau terminal
        Invoke("ExecuteCinematic", 1.5f);
    }

    private void ExecuteCinematic()
    {
        // 1. Bersihkan layar dari UI Terminal
        if (terminalUI != null) terminalUI.SetActive(false);

        // 2. Kunci gerakan agar pemain terdiam menonton tile
        if (playerMovement != null) playerMovement.enabled = false;

        // 3. Panggil animasi kedip hijau di Jet Exit
        if (targetJetExit != null) targetJetExit.UnlockExit();

        // 4. Jeda selama durasi tile berkedip (2 detik) sebelum bisa jalan lagi
        StartCoroutine(WaitAndUnlockPlayer(2f));
    }

    private System.Collections.IEnumerator WaitAndUnlockPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepaskan flag blokir global
    }

    private void OnPuzzleFailed(string message, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = message;
        inputField.text = ""; 
        inputField.ActivateInputField();
    }

    public void CloseTerminal()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        PlayerGridMovement.isTerminalActive = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            
            // Cache pergerakan pemain (jangan dihapus)
            playerMovement = other.GetComponent<PlayerGridMovement>(); 
            
            // --- UBAH: Kirim teks "Tekan [E]" beserta transform terminal ini ---
            if (HUDManager.Instance != null) HUDManager.Instance.ShowHint("Tekan [E]", transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (terminalUI != null) terminalUI.SetActive(false);
            if (HUDManager.Instance != null) HUDManager.Instance.HideHint();
            PlayerGridMovement.isTerminalActive = false; 
        }
    }
}