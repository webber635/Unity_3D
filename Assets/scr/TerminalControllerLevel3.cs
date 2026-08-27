using UnityEngine;
using TMPro;

public class TerminalControllerLevel3 : MonoBehaviour
{
    public static TerminalControllerLevel3 Instance;

    [Header("Referensi UI & Target")]
    [SerializeField] private GameObject terminalUI; // Tambahkan ini untuk menyembunyikan UI
    [SerializeField] private TMP_InputField inputMove; 
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private ConveyorController conveyor;
    
    [Header("Pengaturan Kamera Sinematik")]
    [SerializeField] private GameObject playerCamera;   // Kamera utama (Isometrik Player)
    [SerializeField] private GameObject conveyorCamera; // Kamera khusus yang menyorot map conveyor

    private int targetDistance = 5; 
    private bool isExecuting = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Pastikan kamera conveyor mati saat game dimulai
        if (conveyorCamera != null) conveyorCamera.SetActive(false);
    }

    public void EvaluateAnswer()
    {
        if (isExecuting) return; 

        string inputText = inputMove.text.Trim();
        int playerMoveValue;

        if (!int.TryParse(inputText, out playerMoveValue))
        {
            // (Kode Error Syntax tetap sama...)
            TerminalController1.totalErrors++;
            feedbackText.color = Color.red;
            feedbackText.text = "Syntax Error: Variabel 'move' harus berupa angka (Integer).";
            return;
        }

        isExecuting = true;
        
        // 1. SEMBUNYIKAN UI TERMINAL SEMENTARA
        if (terminalUI != null) terminalUI.SetActive(false);

        // 2. PINDAH KAMERA KE CONVEYOR
        if (playerCamera != null) playerCamera.SetActive(false);
        if (conveyorCamera != null) conveyorCamera.SetActive(true);
        
        conveyor.MoveKey(playerMoveValue);
    }

    // Dipanggil oleh ConveyorController saat gagal (kurang atau kelewatan)
    public void ResetTerminal()
    {
        isExecuting = false;
        TerminalController1.totalWarnings++; 
        
        // 1. KEMBALIKAN KAMERA KE PLAYER
        if (conveyorCamera != null) conveyorCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

        // 2. MUNCULKAN UI TERMINAL LAGI
        if (terminalUI != null) terminalUI.SetActive(true);
        
        int lastInput = int.Parse(inputMove.text.Trim());
        feedbackText.color = Color.yellow;
        if (lastInput < targetDistance)
            feedbackText.text = $"Logic Warning: {lastInput} < Target. Kunci rusak (belum matang).";
        else
            feedbackText.text = $"Logic Warning: {lastInput} > Target. Kunci jatuh ke lava.";
    }

    // Dipanggil oleh ConveyorController saat berhasil
    public void TriggerSuccess()
    {
        // Kembalikan kamera ke player agar mereka bisa lari ke Jet
        if (conveyorCamera != null) conveyorCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

        // Munculkan UI sesaat untuk kasih lihat tulisan Success
        if (terminalUI != null) terminalUI.SetActive(true);

        feedbackText.color = Color.green;
        feedbackText.text = $"Success: {inputMove.text} == Target. Kunci berhasil dicetak!";
        
        // Tutup terminal otomatis setelah 1.5 detik
        Invoke("CloseTerminal", 1.5f);
    }

    private void CloseTerminal()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        PlayerGridMovement.isTerminalActive = false; 
    }
}