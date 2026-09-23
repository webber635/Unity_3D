using UnityEngine;
using TMPro;

public class TerminalController1 : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target (Laser)")]
    [SerializeField] private LaserController targetLaser;

    [Header("Aturan Puzzle")]
    [SerializeField] private string expectedStatus = "false";

    private bool isPlayerInRange = false;
    private bool isSolved = false;

    // CACHE MOVEMENT
    private PlayerGridMovement playerMovement;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        LevelStats.ResetStats();
    }

    void Update()
    {
        if (isPlayerInRange && !isSolved && Input.GetKeyDown(KeyCode.E))
        {
            if (terminalUI.activeSelf && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputField.gameObject) return;
            ToggleTerminal();
        }

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
            inputField.text = "true";
            feedbackText.text = "> Security_mode: ON\nWARNING: Laser is active.";
            feedbackText.color = Color.yellow;
        }
    }

    public void EvaluateAnswer()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;
        string cleanInput = inputField.text.Trim().ToLower();

        if (cleanInput != "true" && cleanInput != "false")
        {
            LevelStats.totalErrors++;
            OnPuzzleFailed("Syntax Error: Value must be 'True' or 'False'.", Color.red);
            return;
        }

        if (cleanInput == expectedStatus) // (Sesuaikan dengan nama variabel expected jawaban Anda)
        {
            OnPuzzleSuccess();
        }
        else
        {
            LevelStats.totalWarnings++;
            OnPuzzleFailed("> Security_mode: ON\nWARNING: Laser is still active.", Color.yellow);
        }
    }

    private void OnPuzzleSuccess()
    {
        feedbackText.color = Color.green;
        feedbackText.text = "> Security_mode: OFF\nLaser deactivation sequence initiated...";
        isSolved = true;

        PlayerPrefs.SetInt("Level2Unlocked", 1);
        PlayerPrefs.Save();

        // TUNGGU 1.5 DETIK AGAR PEMAIN BISA MEMBACA TEKS HIJAU
        Invoke("ExecuteCinematic", 1.5f);
    }

    // ====== FUNGSI CINEMATIC BARU ======
    private void ExecuteCinematic()
    {
        // 1. Bersihkan layar dari UI
        if (terminalUI != null) terminalUI.SetActive(false);

        // 2. Kunci gerakan agar pemain terdiam menonton adegan
        if (playerMovement != null) playerMovement.enabled = false;

        // 3. Mulai animasi kedip laser
        if (targetLaser != null) targetLaser.TurnOffLaser();

        // 4. Jeda selama laser berkedip 
        StartCoroutine(WaitAndUnlockPlayer(2f));
    }

    private System.Collections.IEnumerator WaitAndUnlockPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerMovement != null) playerMovement.enabled = true;
        PlayerGridMovement.isTerminalActive = false; // Lepas flag global
    }
    // ===================================

    private void OnPuzzleFailed(string message, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = message;
        inputField.text = "";
    }

    public void CloseTerminal()
    {
        // Fungsi ini sekarang hanya digunakan jika pemain menekan tombol Close manual (X)
        if (terminalUI != null) terminalUI.SetActive(false);
        if (playerMovement != null && playerMovement.enabled)
        {
            PlayerGridMovement.isTerminalActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // CACHE SCRIPT MOVEMENT SAAT MASUK ZONA
            playerMovement = other.GetComponent<PlayerGridMovement>();

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