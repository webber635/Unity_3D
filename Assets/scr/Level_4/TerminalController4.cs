using UnityEngine;
using TMPro;

public class TerminalController4 : MonoBehaviour
{
    [Header("Referensi UI (Wajib diisi)")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Laser)")]
    [SerializeField] private LaserController targetLaser; 

    [Header("Aturan Puzzle Level 4")]
    [SerializeField] private int currentEnergy = 60;
    [SerializeField] private int requiredEnergy = 50;
    [SerializeField] private string expectedSecurityStatus = "false"; 

    private bool isPlayerInRange = false;
    private bool isSolved = false;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        LevelStats.ResetStats(); // Menggunakan sistem statis yang baru
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
        PlayerGridMovement.isTerminalActive = isActive; // Blokir gerakan player[cite: 5]

        if (isActive)
        {
            // Set nilai default awal ke "True" sesuai rancangan puzzle
            inputField.text = "true"; 
            feedbackText.text = "> Disabling laser...\nERROR: Security mode is active.\nEnergy: 60\nRequired: 50\nSecurity: ON";
            feedbackText.color = Color.yellow;
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

        // 1. CEK SYNTAX: Memastikan input adalah boolean yang valid
        if (cleanInput != "true" && cleanInput != "false")
        {
            LevelStats.totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'security_mode' value must be 'True' or 'False'.", Color.red);
            return;
        }

        // 2. CEK LOGIKA: IF (energy >= 50 AND security_mode == false)
        bool isEnergySufficient = currentEnergy >= requiredEnergy;
        bool isSecurityDisabled = (cleanInput == expectedSecurityStatus);

        if (isEnergySufficient && isSecurityDisabled)
        {
            OnPuzzleSuccess();
        }
        else
        {
            LevelStats.totalWarnings++; 
            
            // Memberikan pesan error spesifik jika energi yang kurang (meskipun di level ini energi sudah di-hardcode cukup)
            if (!isEnergySufficient)
            {
                OnPuzzleFailed($"> Disabling laser...\nERROR: Insufficient Energy.\nEnergy: {currentEnergy}\nRequired: {requiredEnergy}", Color.yellow);
            }
            // Memberikan pesan error jika security masih menyala
            else if (!isSecurityDisabled)
            {
                OnPuzzleFailed("> Disabling laser...\nERROR: Security mode is active.\nEnergy: 60\nRequired: 50\nSecurity: ON", Color.yellow);
            }
        }
    }

    private void OnPuzzleSuccess()
    {
        feedbackText.color = Color.green;
        feedbackText.text = "> Conditions satisfied.\nLaser: OFF";
        isSolved = true;

        // Buka akses Level 5 di Main Menu
        PlayerPrefs.SetInt("Level5Unlocked", 1);
        PlayerPrefs.Save();

        // Matikan Laser
        if (targetLaser != null)
        {
            targetLaser.TurnOffLaser(); // Memanggil fungsi dari LaserController.cs
        }

        Invoke("CloseTerminal", 2.0f); 
    }

    private void OnPuzzleFailed(string message, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = message;
        inputField.text = ""; 
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