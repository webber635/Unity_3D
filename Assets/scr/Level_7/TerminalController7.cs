using UnityEngine;
using TMPro;
using System.Text;

public class TerminalController7 : MonoBehaviour
{
    [Header("Referensi UI (Dua Input)")]
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TMP_InputField loopInput;
    [SerializeField] private TMP_InputField securityInput;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target")]
    [SerializeField] private PlatformElevatorController targetElevator;
    [SerializeField] private LaserController targetLaser;

    [Header("Aturan Puzzle Level 7")]
    [SerializeField] private int requiredLoops = 3;
    [SerializeField] private string expectedSecurity = "false";

    [Header("Pengaturan Checkpoint")]
    [Tooltip("Geser titik respawn fase 2. X = -1 untuk mundur 1 petak")]
    [SerializeField] private Vector3 checkpointOffset = new Vector3(-1, 0, 0);

    private bool isPlayerInRange = false;
    private bool isTerminalOpen = false;

    // SISTEM FASE & CACHE
    private int currentPhase = 1;
    private Transform playerTransform;
    private PlayerGridMovement playerMovement;

    // Referensi Highlighter
    private SyntaxHighlighter loopHighlighter;
    private SyntaxHighlighter securityHighlighter;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        LevelStats.ResetStats();

        // Ambil komponen highlighter dari masing-masing input field
        if (loopInput != null) loopHighlighter = loopInput.GetComponent<SyntaxHighlighter>();
        if (securityInput != null) securityHighlighter = securityInput.GetComponent<SyntaxHighlighter>();

        // Set Kondisi Fase 1 di awal
        currentPhase = 1;
        loopInput.interactable = true;
        securityInput.interactable = false;
    }

    void Update()
    {
        // Cegah akses terminal saat robot dibekukan oleh cinematic
        if (playerMovement != null && !playerMovement.enabled) return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (terminalUI.activeSelf &&
               (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == loopInput.gameObject ||
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == securityInput.gameObject))
            {
                return;
            }
            ToggleTerminal();
        }

        if (terminalUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) EvaluateAnswer();
        }
    }

    private void ToggleTerminal()
    {
        isTerminalOpen = !terminalUI.activeSelf;
        terminalUI.SetActive(isTerminalOpen);
        PlayerGridMovement.isTerminalActive = isTerminalOpen;

        if (isTerminalOpen)
        {
            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayTerminalOpen();
            }
            // Atur mana yang berkedip sesuai fasenya
            UpdateHighlighters();

            if (currentPhase == 1)
            {
                loopInput.text = "0";
                securityInput.text = "true";
                feedbackText.text = "> SYSTEM STATUS...\nPlatform: GROUNDED\nLaser: ACTIVE\n\nERROR: Elevation required to reach exit.";
                feedbackText.color = Color.yellow;
            }
            else if (currentPhase == 2)
            {
                feedbackText.text = "> SYSTEM STATUS...\nPlatform: ELEVATED\nLaser: ACTIVE\n\nERROR: Security protocol blocking the exit.";
                feedbackText.color = Color.yellow;
            }
        }
    }

    // Fungsi khusus untuk mengatur kedipan (Highlight)
    private void UpdateHighlighters()
    {
        if (loopHighlighter != null) loopHighlighter.enabled = (currentPhase == 1);
        if (securityHighlighter != null) securityHighlighter.enabled = (currentPhase == 2);
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(loopInput.text) || string.IsNullOrEmpty(securityInput.text)) return;

        string loopStr = loopInput.text.Trim();
        string secStr = securityInput.text.Trim().ToLower();

        if (!int.TryParse(loopStr, out int loopsValue))
        {
            LevelStats.totalErrors++;
            OnPuzzleFailed("Syntax Error: Loop value must be a valid integer.", Color.red);
            return;
        }
        if (secStr != "true" && secStr != "false")
        {
            LevelStats.totalErrors++;
            OnPuzzleFailed("Syntax Error: 'security_mode' must be 'True' or 'False'.", Color.red);
            return;
        }

        // ==========================================
        // FASE 1: EVALUASI LIFT (LOOPING)
        // ==========================================
        if (currentPhase == 1)
        {
            StringBuilder outputMsg = new StringBuilder("> Executing platform elevation...\n");

            for (int i = 0; i < loopsValue; i++)
            {
                outputMsg.AppendLine($"Iteration {i + 1}: Elevating...");
            }

            if (loopsValue >= requiredLoops)
            {
                outputMsg.AppendLine("\nTarget altitude reached. Waiting for security override...");
                feedbackText.color = Color.cyan;
                feedbackText.text = outputMsg.ToString();

                // Panggil Cinematic Fase 1
                StartCoroutine(ExecutePhase1Cinematic(loopsValue));
            }
            else
            {
                LevelStats.totalWarnings++;
                outputMsg.AppendLine("\nERROR: Insufficient loops. Platform failed to reach the top.");
                OnPuzzleFailed(outputMsg.ToString(), Color.yellow);
            }
        }
        // ==========================================
        // FASE 2: EVALUASI LASER (LOGIKA AND)
        // ==========================================
        else if (currentPhase == 2)
        {
            if (secStr == expectedSecurity)
            {
                feedbackText.color = Color.green;
                feedbackText.text = "> Security: BYPASSED\nLaser: OFF\n\nAll systems cleared. You may proceed.";

                // Panggil Cinematic Fase 2
                StartCoroutine(ExecutePhase2Cinematic());
            }
            else
            {
                LevelStats.totalWarnings++;
                OnPuzzleFailed("> SYSTEM STATUS...\nPlatform: ELEVATED\nLaser: ACTIVE\n\nERROR: Security protocol is still active.", Color.yellow);
            }
        }
    }

    // ====== FUNGSI CINEMATIC FASE 1 (LIFT) ======
    private System.Collections.IEnumerator ExecutePhase1Cinematic(int loopsValue)
    {
        // 1. Kunci pemain dan beri waktu untuk membaca teks terminal
        if (playerMovement != null) playerMovement.enabled = false;
        yield return new WaitForSeconds(1.5f);

        // 2. Tutup UI Terminal
        if (terminalUI != null) terminalUI.SetActive(false);

        // 3. Panggil lift (Fungsi OnElevatorFinished bawaan script akan membuka kunci pemain di akhir)
        if (targetElevator != null)
        {
            targetElevator.StartElevation(loopsValue, playerTransform, OnElevatorFinished);
        }
    }

    // ====== FUNGSI CINEMATIC FASE 2 (LASER) ======
    private System.Collections.IEnumerator ExecutePhase2Cinematic()
    {
        // 1. Kunci pemain dan beri waktu untuk membaca teks terminal
        if (playerMovement != null) playerMovement.enabled = false;
        yield return new WaitForSeconds(1.5f);

        // 2. Tutup UI Terminal
        if (terminalUI != null) terminalUI.SetActive(false);

        // 3. Matikan laser (berkedip)
        if (targetLaser != null) targetLaser.TurnOffLaser();

        // 4. Jeda selama durasi laser berkedip (2 detik) lalu buka kontrol
        yield return new WaitForSeconds(2.0f);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepas flag global
    }

    private void OnElevatorFinished()
    {
        currentPhase = 2;
        loopInput.interactable = false;
        securityInput.interactable = true;

        // Update kedipan untuk Fase 2
        UpdateHighlighters();

        if (playerMovement != null)
        {
            // Set titik respawn baru di lantai atas
            Vector3 adjustedSpawnPos = playerTransform.position + checkpointOffset;
            playerMovement.SetNewCheckpoint(adjustedSpawnPos);

            // Buka kembali kunci pemain karena lift sudah berhenti
            playerMovement.enabled = true;
        }

        // --- TAMBAHAN PENTING: Paksa lepas flag blokir global ---
        PlayerGridMovement.isTerminalActive = false;

        Debug.Log("Lift tiba di atas. Checkpoint baru digeser ke: " + (playerTransform.position + checkpointOffset));
    }

    private void OnPuzzleFailed(string message, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = message;
    }

    public void CloseTerminal()
    {
        if (terminalUI != null) terminalUI.SetActive(false);

        // Lepas blokir interaksi secara mutlak (tanpa syarat)
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