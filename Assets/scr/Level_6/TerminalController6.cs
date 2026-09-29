using UnityEngine;
using TMPro;
using System.Text;

public class TerminalController6 : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Moving Island)")]
    [SerializeField] private MovingIslandController targetIsland; 

    [Header("Aturan Puzzle Level 6")]
    [SerializeField] private int basePower = 20;
    [SerializeField] private int powerPerIteration = 20;
    [SerializeField] private int requiredPower = 80;

    private bool isPlayerInRange = false;
    private bool isSolved = false;
    private PlayerGridMovement playerMovement;

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
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) EvaluateAnswer(); 
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

            inputField.text = "1"; 
            feedbackText.text = "> Powering platform thrusters...\nIteration 1: Power 40\n\nPower: 40\nRequired: 80\nERROR: Insufficient power.";
            feedbackText.color = Color.yellow;
        }
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;
        string cleanInput = inputField.text.Trim();

        // 1. CEK SYNTAX
        if (!int.TryParse(cleanInput, out int iterations))
        {
            LevelStats.totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'range' value must be a valid integer.", Color.red);
            return;
        }

        if (iterations < 0) iterations = 0;

        // 2. CEK LOGIKA (Simulasi FOR LOOP)
        int currentPower = basePower;
        StringBuilder outputMsg = new StringBuilder("> Powering platform thrusters...\n");

        for (int i = 0; i < iterations; i++)
        {
            currentPower += powerPerIteration;
            
            if (i < 5) 
            {
                outputMsg.AppendLine($"Iteration {i + 1}: Power {currentPower}");
            }
            else if (i == 5)
            {
                outputMsg.AppendLine("... [Output log truncated] ...");
            }
        }

        outputMsg.AppendLine($"\nPower: {currentPower}");
        outputMsg.AppendLine($"Required: {requiredPower}");

        // 3. EVALUASI HASIL AKHIR
        bool isSuccess = (currentPower == requiredPower); 

        if (isSuccess)
        {
            outputMsg.AppendLine("Platform: CONNECTED");
            OnPuzzleSuccess(iterations, outputMsg.ToString());
        }
        else
        {
            LevelStats.totalWarnings++; 
            
            if (currentPower > requiredPower)
            {
                outputMsg.AppendLine("ERROR: Power overload. Platform overshot.");
            }
            else
            {
                outputMsg.AppendLine("ERROR: Insufficient power. Platform failed to connect.");
            }
            
            // HANYA tampilkan pesan error di layar terminal tanpa menggerakkan pulau
            // Pemain bisa langsung mengetik ulang tanpa harus menunggu animasi
            OnPuzzleFailed(outputMsg.ToString(), Color.yellow);
        }
    }

    private void OnPuzzleSuccess(int iterations, string finalMessage)
    {
        feedbackText.color = Color.green;
        feedbackText.text = finalMessage;
        isSolved = true;

        PlayerPrefs.SetInt("Level7Unlocked", 1);
        PlayerPrefs.Save();

        // GANTI BAGIAN INI: Panggil cinematic menahan pemain
        StartCoroutine(ExecuteCinematicRoutine(1.5f, iterations, true));
    }

    // ====== FUNGSI CINEMATIC BARU ======
    private System.Collections.IEnumerator ExecuteCinematicRoutine(float delay, int iterations, bool isSuccess)
    {
        // 1. Beri waktu pemain membaca teks terminal
        yield return new WaitForSeconds(delay);

        // 2. Tutup UI dan kunci pemain
        if (terminalUI != null) terminalUI.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = false;

        // 3. Panggil pergerakan pulau
        if (targetIsland != null)
        {
            targetIsland.StartMovingSequence(iterations, isSuccess);
        }

        // 4. Hitung estimasi waktu pulau bergerak (0.5 durasi + 0.2 delay = 0.7 per langkah)
        float animDuration = iterations * 0.7f;
        
        // Jeda sampai pulau selesai bergerak sebelum membuka kontrol
        yield return new WaitForSeconds(animDuration + 0.5f);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepas flag global
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