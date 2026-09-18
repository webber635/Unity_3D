using UnityEngine;
using TMPro;
using System.Text;

public class TerminalControllerLevel6 : MonoBehaviour
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
        PlayerGridMovement.isTerminalActive = isActive; 

        if (isActive)
        {
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
        bool isSuccess = (currentPower >= requiredPower);

        if (isSuccess)
        {
            outputMsg.AppendLine("Platform: CONNECTED");
            OnPuzzleSuccess(iterations, outputMsg.ToString());
        }
        else
        {
            LevelStats.totalWarnings++; 
            outputMsg.AppendLine("ERROR: Platform failed to connect.");
            OnPuzzleFailed(outputMsg.ToString(), Color.yellow);
            
            // Jalankan animasi gerak (meskipun gagal/tidak sampai)
            if (targetIsland != null) targetIsland.StartMovingSequence(iterations, false);
        }
    }

    private void OnPuzzleSuccess(int iterations, string finalMessage)
    {
        feedbackText.color = Color.green;
        feedbackText.text = finalMessage;
        isSolved = true;

        PlayerPrefs.SetInt("Level7Unlocked", 1);
        PlayerPrefs.Save();

        if (targetIsland != null)
        {
            targetIsland.StartMovingSequence(iterations, true);
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
            if (HUDManager.Instance != null) HUDManager.Instance.ShowHint("Tekan [E] Terminal");
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