using UnityEngine;
using TMPro;

public class TerminalController5 : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputField;   
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Hot Path)")]
    [SerializeField] private HotPathController targetPath; 

    [Header("Aturan Puzzle Level 5")]
    [SerializeField] private int thresholdTemp = 80;

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
            // Nilai awal berbahaya
            inputField.text = "80"; 
            feedbackText.text = "> Checking path...\nTemperature: 80°C\nWARNING: Path temperature critical.\nPath: HOT";
            feedbackText.color = Color.yellow;
        }
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;

        string cleanInput = inputField.text.Trim();

        // 1. CEK SYNTAX
        if (!int.TryParse(cleanInput, out int currentTemp))
        {
            LevelStats.totalErrors++; 
            OnPuzzleFailed("Syntax Error: 'temperature' must be an integer value.", Color.red);
            return;
        }

        // 2. CEK LOGIKA (IF / ELSE)
        if (currentTemp < thresholdTemp) // else (aman)
        {
            OnPuzzleSuccess(currentTemp);
        }
        else // if temperature >= 80 (panas)
        {
            LevelStats.totalWarnings++; 
            OnPuzzleFailed($"> Checking path...\nTemperature: {currentTemp}°C\nWARNING: Path temperature critical.\nPath: HOT", Color.yellow);
        }
    }

    private void OnPuzzleSuccess(int temp)
    {
        feedbackText.color = Color.green;
        feedbackText.text = $"> Checking path...\nTemperature: {temp}°C\nPath temperature: NORMAL\nPath: SAFE";
        isSolved = true;

        PlayerPrefs.SetInt("Level6Unlocked", 1);
        PlayerPrefs.Save();

        // Mendinginkan jalur
        if (targetPath != null)
        {
            targetPath.SetPathSafe();
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