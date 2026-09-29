using UnityEngine;
using TMPro;

public class TerminalController4 : MonoBehaviour
{
    [Header("Referensi UI (Dua Input)")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField energyInput;   // Gantikan inputField lama
    [SerializeField] private TMP_InputField securityInput; // InputField kedua
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Laser)")]
    [SerializeField] private LaserController targetLaser; 

    [Header("Aturan Puzzle Level 4")]
    [Tooltip("Ubah di sini untuk expected answer")]
    [SerializeField] private int requiredEnergy = 50;
    [SerializeField] private string expectedSecurityStatus = "false"; 

    private bool isPlayerInRange = false;
    private bool isSolved = false;
    private PlayerGridMovement playerMovement; // Cache movement untuk cinematic

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        LevelStats.ResetStats(); 
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
            // Cek agar tidak toggle saat sedang mengetik di salah satu input
            if (terminalUI.activeSelf && 
               (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == energyInput.gameObject ||
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == securityInput.gameObject))
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
            if (UISoundManager.Instance != null)
                {
                    UISoundManager.Instance.PlayTerminalOpen();
                }

            // Set nilai default awal di kedua slot
            energyInput.text = "60"; 
            securityInput.text = "true"; 
            feedbackText.text = "> Disabling laser...\nERROR: Security mode is active.\nEnergy: 60\nRequired: 50\nSecurity: ON";
            feedbackText.color = Color.yellow;
        }
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(energyInput.text) || string.IsNullOrEmpty(securityInput.text)) return;

        string cleanEnergy = energyInput.text.Trim();
        string cleanSecurity = securityInput.text.Trim().ToLower();

        // 1. CEK SYNTAX
        if (!int.TryParse(cleanEnergy, out int currentEnergy))
        {
            LevelStats.totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'energy' value must be a valid integer number.", Color.red);
            return;
        }

        if (cleanSecurity != "true" && cleanSecurity != "false")
        {
            LevelStats.totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'security_mode' value must be 'True' or 'False'.", Color.red);
            return;
        }

        // 2. CEK LOGIKA: IF (energy >= requiredEnergy AND security_mode == expectedSecurityStatus)
        bool isEnergySufficient = currentEnergy >= requiredEnergy;
        bool isSecurityDisabled = (cleanSecurity == expectedSecurityStatus);

        if (isEnergySufficient && isSecurityDisabled)
        {
            OnPuzzleSuccess();
        }
        else if (isEnergySufficient ^ isSecurityDisabled) // XOR: Jika HANYA SALAH SATU yang benar
        {
            LevelStats.totalWarnings++;
            OnPuzzleFailed("> Disabling laser...\nERROR: Conditions not met.", Color.yellow);
        }
        else // Jika KEDUANYA salah (Feedback dipertahankan mirip sebelumnya dengan sedikit konteks tambahan)
        {
            LevelStats.totalWarnings++; 
            OnPuzzleFailed($"> Disabling laser...\nERROR: Insufficient Energy AND Security mode is active.\nEnergy: {currentEnergy}\nRequired: {requiredEnergy}\nSecurity: ON", Color.yellow);
        }
    }

    private void OnPuzzleSuccess()
    {
        feedbackText.color = Color.green;
        feedbackText.text = "> Conditions satisfied.\nLaser: OFF";
        isSolved = true;

        PlayerPrefs.SetInt("Level5Unlocked", 1);
        PlayerPrefs.Save();

        // Jeda untuk membaca terminal sebelum cinematic
        Invoke("ExecuteCinematic", 1.5f); 
    }

    // ====== FUNGSI CINEMATIC (Disamakan dengan Level 1) ======
    private void ExecuteCinematic()
    {
        // 1. Bersihkan layar dari UI
        if (terminalUI != null) terminalUI.SetActive(false);

        // 2. Kunci gerakan agar pemain terdiam menonton adegan
        if (playerMovement != null) playerMovement.enabled = false;

        // 3. Mulai animasi kedip laser mati
        if (targetLaser != null) targetLaser.TurnOffLaser();

        // 4. Jeda selama laser berkedip (2 detik)
        StartCoroutine(WaitAndUnlockPlayer(2f));
    }

    private System.Collections.IEnumerator WaitAndUnlockPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepas flag global
    }
    // =========================================================

    private void OnPuzzleFailed(string message, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = message;
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