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
    private PlayerGridMovement playerMovement;

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
        // --- TAMBAHAN BARU: Cegah akses jika robot sedang dibekukan oleh cinematic ---
        if (playerMovement != null && !playerMovement.enabled) 
        {
            return; 
        }

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

        if (isActive)
        {
            // 1. ISI TEKS SEBELUM PANEL DIAKTIFKAN
            // (Mencegah bug TextMeshPro yang mencari posisi kursor saat Canvas belum siap)
            if (inputField != null) inputField.text = "20"; 
            
            if (feedbackText != null)
            {
                feedbackText.text = "> Checking power...\nERROR: Insufficient power for tile levitation.\nPower: 40\nRequired: 50";
                feedbackText.color = Color.yellow;
            }

            // 2. BARU AKTIFKAN PANEL UI & KUNCI GERAKAN
            if (terminalUI != null) terminalUI.SetActive(true);
            PlayerGridMovement.isTerminalActive = true;

            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayTerminalOpen();
            }            
        }
        else
        {
            // JIKA MENU DITUTUP
            if (terminalUI != null) terminalUI.SetActive(false);
            PlayerGridMovement.isTerminalActive = false;
        }
    }

    // --- TAMBAHAN BARU: Jeda 1 frame sebelum mengaktifkan kursor ---
    private System.Collections.IEnumerator FocusInputField()
    {
        // Tunggu sampai layar selesai dirender di frame ini
        yield return new WaitForEndOfFrame();
        
        if (inputField != null)
        {
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

        PlayerPrefs.SetInt("Level4Unlocked", 1);
        PlayerPrefs.Save();

        // Tunda 1.5 detik agar pemain dapat membaca terminal
        Invoke("ExecuteCinematic", 1.5f); 
    }

    private void ExecuteCinematic()
    {
        // 1. Bersihkan UI terminal dari layar
        if (terminalUI != null) terminalUI.SetActive(false);

        // 2. Kunci gerakan player untuk menonton jembatan naik
        if (playerMovement != null) playerMovement.enabled = false;

        // 3. Panggil animasi jembatan naik
        if (targetBridge != null)
        {
            targetBridge.RestoreTiles();
        }

        // 4. Jeda selama durasi jembatan cascade (3 detik untuk amannya)
        StartCoroutine(WaitAndUnlockPlayer(3.0f));
    }

    private System.Collections.IEnumerator WaitAndUnlockPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepaskan pembatas global
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
            if (HUDManager.Instance != null)
                HUDManager.Instance.HideHint();
            PlayerGridMovement.isTerminalActive = false; 
        }
    }
}