using UnityEngine;
using TMPro;

public class TerminalController1 : MonoBehaviour
{
    [Header("Referensi UI (Wajib diisi)")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Laser/Pintu)")]
    [SerializeField] private LaserController targetLaser; 

    [Header("Aturan Puzzle")]
    [SerializeField] private string expectedAnswer = "false"; 

    private bool isPlayerInRange = false;
    private bool isSolved = false;

    // ==========================================
    // TAMBAHAN SISTEM SKOR / STATISTIK
    // ==========================================
    public static int totalErrors = 0;
    public static int totalWarnings = 0;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        
        // Reset skor setiap kali level dimulai ulang
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
            if (terminalUI.activeSelf && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputField.gameObject)
            {
                return; 
            }
            ToggleTerminal();
        }
    }

    private void HandleTerminalInput()
    {
        if (terminalUI.activeSelf && !isSolved)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                EvaluateAnswer(); 
            }
        }
    }

    private void ToggleTerminal()
    {
        bool isActive = !terminalUI.activeSelf;
        terminalUI.SetActive(isActive);
        PlayerGridMovement.isTerminalActive = isActive;

        if (isActive)
        {
            inputField.text = "true"; 
            feedbackText.text = "Status: Locked. Change variable to deactive laser.";
            feedbackText.color = Color.white;
            inputField.ActivateInputField(); 
            inputField.caretPosition = inputField.text.Length; 
        }
    }

    public void EvaluateAnswer()
    {
        if (inputField == null) return;
        EvaluateAnswer(inputField.text); 
    }

    public void EvaluateAnswer(string playerInput)
    {
        if (string.IsNullOrEmpty(playerInput)) return;

        string cleanInput = playerInput.Trim().ToLower();

        // 1. CEK SYNTAX (Mendeteksi Typo / Invalid value)
        if (cleanInput != "true" && cleanInput != "false")
        {
            totalErrors++; // Menambahkan skor Error
            OnPuzzleFailed("Syntax Error: Value must be 'true' or 'false'.", Color.red);
            return;
        }

        // 2. CEK LOGIKA JAWABAN
        if (cleanInput == expectedAnswer)
        {
            OnPuzzleSuccess();
        }
        else
        {
            totalWarnings++; // Menambahkan skor Warning
            OnPuzzleFailed("Logic Warning: Laser is still active.", Color.yellow);
        }
    }

    private void OnPuzzleSuccess()
    {
        feedbackText.color = Color.green;
        feedbackText.text = "Success: Laser Deactivated!";
        isSolved = true;

        // Buka akses Level 2 di Main Menu
        PlayerPrefs.SetInt("Level2Unlocked", 1);
        PlayerPrefs.Save();

        if (targetLaser != null)
        {
            targetLaser.TurnOffLaser();
        }

        // Cukup tutup terminal, karena Jet yang akan memanggil Victory Panel
        Invoke("CloseTerminal", 1.5f); 
    }

    // Fungsi gagal sekarang menerima warna agar bisa dibedakan (Merah/Kuning)
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
            if (HUDManager.Instance != null)
                HUDManager.Instance.ShowHint("Tekan [E] untuk Mengakses Terminal");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (terminalUI != null) terminalUI.SetActive(false);
            if (HUDManager.Instance != null)
                HUDManager.Instance.HideHint();
            PlayerGridMovement.isTerminalActive = false; 
        }
    }

    public bool IsTerminalOpen()
    {
        return terminalUI != null && terminalUI.activeSelf;
    }
}