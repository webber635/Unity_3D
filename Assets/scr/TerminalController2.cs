using UnityEngine;
using TMPro;

public class TerminalControllerLevel2 : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private GameObject terminalUI;       
    [SerializeField] private TMP_InputField inputLaserInit;     
    [SerializeField] private TMP_InputField inputJembatanInit;  
    [SerializeField] private TMP_InputField inputJembatanIf;    
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Referensi Target")]
    [SerializeField] private LaserController targetLaser; 
    [SerializeField] private JembatanController targetJembatan; 

    private bool isPlayerInRange = false;
    private bool isSolved = false;

    // ==========================================
    // TAMBAHAN SISTEM SKOR / STATISTIK
    // ==========================================
    public static int totalErrors = 0;
    public static int totalWarnings = 0;

    void Start()
    {
        if (terminalUI != null) terminalUI.SetActive(false);

        if (inputLaserInit != null) inputLaserInit.text = "True";
        if (inputJembatanInit != null) inputJembatanInit.text = "False";
        if (inputJembatanIf != null) inputJembatanIf.text = "False";

        // Reset skor saat level 2 dimulai
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
            bool typing1 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputLaserInit.gameObject;
            bool typing2 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputJembatanInit.gameObject;
            bool typing3 = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == inputJembatanIf.gameObject;
            
            if (terminalUI.activeSelf && (typing1 || typing2 || typing3)) return; 
            
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
            feedbackText.text = "Status: Locked. Override the variables to proceed.";
            feedbackText.color = Color.white;
            
            inputLaserInit.ActivateInputField(); 
            inputLaserInit.caretPosition = inputLaserInit.text.Length; 
        }
    }

    public void RunButtonEvaluate()
    {
        EvaluateAnswer();
    }

    public void EvaluateAnswer()
    {
        string txtLaser = inputLaserInit.text.Trim();
        string txtJemInit = inputJembatanInit.text.Trim();
        string txtJemIf = inputJembatanIf.text.Trim();

        // 1. CEK SYNTAX
        if (CekErrorSyntax(txtLaser) || CekErrorSyntax(txtJemInit) || CekErrorSyntax(txtJemIf)) 
            return;

        // 2. PARSING KE BOOLEAN
        bool valLaser = (txtLaser.ToLower() == "true");
        bool valJemInit = (txtJemInit.ToLower() == "true");
        bool valJemIf = (txtJemIf.ToLower() == "true");

        // 3. SIMULASI LOGIKA
        bool statusAkhirLaser = valLaser;
        bool statusAkhirJembatan = valJemInit;
        bool ifBlockDieksekusi = false;
        
        if (statusAkhirLaser == false) 
        {
            statusAkhirJembatan = valJemIf; 
            ifBlockDieksekusi = true;
        }

        // 4. EKSEKUSI TARGET
        if (statusAkhirLaser == false) { if (targetLaser != null) targetLaser.TurnOffLaser(); }
        else { if (targetLaser != null) targetLaser.TurnOnLaser(); }

        if (statusAkhirJembatan == true) { if (targetJembatan != null) targetJembatan.TurnOnBridge(); }
        else { if (targetJembatan != null) targetJembatan.TurnOffBridge(); }

        // 5. EVALUASI MENANG & PENCATATAN WARNING
        if (valLaser == false && statusAkhirJembatan == true)
        {
            feedbackText.color = Color.green;
            feedbackText.text = "Success: Laser dimatikan dan jembatan aktif!";
            isSolved = true;
            Invoke("CloseTerminal", 1.5f);
        }
        else
        {
            totalWarnings++; // Tambah skor warning jika logika tidak memecahkan puzzle
            
            if (valLaser == false && statusAkhirJembatan == false)
            {
                OnPuzzleFailed("Logic Warning: Laser mati, tapi blok 'if' membuat Jembatan jadi False.", Color.yellow);
            }
            else if (!ifBlockDieksekusi)
            {
                OnPuzzleFailed("Logic Warning: Kondisi 'if' (laser == false) tidak terpenuhi.", Color.yellow);
            }
            else
            {
                OnPuzzleFailed("Logic Warning: Sistem berjalan, tapi kombinasi variabel belum tepat.", Color.yellow);
            }
        }
    }

    private bool CekErrorSyntax(string input)
    {
        string lowerInput = input.Trim().ToLower();
        if (lowerInput == "true" || lowerInput == "false") return false; 

        totalErrors++; // Tambah skor error jika salah ketik
        OnPuzzleFailed($"Syntax Error: Invalid value '{input}'. Only use 'True' or 'False'.", Color.red);
        return true; 
    }

    private void OnPuzzleFailed(string errorMessage, Color textColor)
    {
        feedbackText.color = textColor;
        feedbackText.text = errorMessage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (HUDManager.Instance != null) HUDManager.Instance.ShowHint("Tekan [E] untuk Mengakses Terminal");
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

    public bool IsTerminalOpen()
    {
        return terminalUI != null && terminalUI.activeSelf;
    }

    public void CloseTerminal()
    {
        if (terminalUI != null) terminalUI.SetActive(false);
        PlayerGridMovement.isTerminalActive = false; 
    }
}