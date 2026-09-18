using UnityEngine;
using TMPro;

public class TerminalController3 : MonoBehaviour
{
    [Header("Referensi UI (Wajib diisi)")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Tile Bridge)")]
    [SerializeField] private TileBridgeController targetBridge; 

    [Header("Aturan Puzzle Level 2")]
    [SerializeField] private int basePower = 20;
    [SerializeField] private int requiredPower = 50;

    private bool isPlayerInRange = false;
    private bool isSolved = false;

    // Statistik untuk Victory Panel
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
            // Set nilai default awal sesuai rancangan (20)
            inputField.text = "20"; 
            feedbackText.text = "> Checking power...\nERROR: Insufficient power for tile levitation.\nPower: 40\nRequired: 50";
            feedbackText.color = Color.yellow;
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

        string cleanInput = playerInput.Trim();

        // 1. CEK SYNTAX: Pastikan input adalah angka (Integer)
        if (!int.TryParse(cleanInput, out int batteryValue))
        {
            totalErrors++; 
            OnPuzzleFailed("Syntax Error: Value 'battery' must be a valid integer number.", Color.red);
            return;
        }

        // 2. CEK LOGIKA: Hitung power + battery
        int currentPower = basePower + batteryValue;

        if (currentPower >= requiredPower)
        {
            OnPuzzleSuccess(currentPower);
        }
        else
        {
            totalWarnings++; 
            OnPuzzleFailed($"> Checking power...\nERROR: Insufficient power for tile levitation.\nPower: {currentPower}\nRequired: {requiredPower}", Color.yellow);
        }
    }

    private void OnPuzzleSuccess(int finalPower)
    {
        feedbackText.color = Color.green;
        feedbackText.text = $"> Checking power...\nPower: {finalPower}\nRequired: {requiredPower}\n\nPath: RESTORED";
        isSolved = true;

        // Buka akses Level 3 di Main Menu
        PlayerPrefs.SetInt("Level3Unlocked", 1);
        PlayerPrefs.Save();

        // Jalankan animasi tiles naik
        if (targetBridge != null)
        {
            targetBridge.RestoreTiles();
        }

        Invoke("CloseTerminal", 2.0f); 
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
}