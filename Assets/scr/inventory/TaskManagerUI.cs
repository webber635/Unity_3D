using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TaskManagerUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject taskManagerPanel; 
    [SerializeField] private GameObject fileGridPanel;    
    [SerializeField] private GameObject fileContentPanel; 

    [Header("UI Elements")]
    [SerializeField] private Transform fileListContainer; 
    [SerializeField] private TMP_Text fileContentViewer;
    [SerializeField] private TMP_Text fileTitleViewer;   
    [SerializeField] private GameObject fileButtonPrefab; 
    [SerializeField] private GameObject inventoryRedDot; 

    public static bool isTaskManagerActive = false; 

    private Dictionary<string, string> textFiles = new Dictionary<string, string>();
    
    // --- TAMBAHAN: Penyimpan status file baru ---
    private HashSet<string> newFiles = new HashSet<string>(); 
    
    private string activeFileName = "";

    void Start()
    {
        if (taskManagerPanel != null) taskManagerPanel.SetActive(false);

        if (inventoryRedDot != null) inventoryRedDot.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTaskManager();
        }
    }

    public void AddTextFile(string name, string content)
    {
        if (!textFiles.ContainsKey(name))
        {
            textFiles.Add(name, content);

            newFiles.Add(name);

            if (inventoryRedDot != null) inventoryRedDot.SetActive(true); 
        }
    }

    public void CloseTaskManager()
    {
        if (taskManagerPanel != null) taskManagerPanel.SetActive(false);
        isTaskManagerActive = false; 
    }

    public void ToggleTaskManager()
    {
        bool isActive = !taskManagerPanel.activeSelf;
        taskManagerPanel.SetActive(isActive);
        isTaskManagerActive = isActive;

        if (isActive)
        {
            if (UISoundManager.Instance != null)
            {
                UISoundManager.Instance.PlayInventoryOpen();
            }

            if (fileGridPanel != null) fileGridPanel.SetActive(true);
            if (fileContentPanel != null) fileContentPanel.SetActive(false);

            RenderFileList();
        }
    }

    private void RenderFileList()
    {
        foreach (Transform child in fileListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var file in textFiles)
        {
            GameObject btnObj = Instantiate(fileButtonPrefab, fileListContainer);
            FileButton btnScript = btnObj.GetComponent<FileButton>();
            if (btnScript != null)
            {
                // --- TAMBAHAN: Cek apakah file ini ada di daftar file baru ---
                bool isNew = newFiles.Contains(file.Key);
                btnScript.Setup(file.Key, this, isNew);
            }
        }
    }

    public void SelectFile(string fileName)
    {
        activeFileName = fileName;
        
        // --- TAMBAHAN: Hapus status "baru" saat file ini dipilih/dibaca ---
        if (newFiles.Contains(fileName))
        {
            newFiles.Remove(fileName);
        }

        if (newFiles.Count == 0 && inventoryRedDot != null)
        {
            inventoryRedDot.SetActive(false);
        }
        
        if (fileGridPanel != null) fileGridPanel.SetActive(false);
        if (fileContentPanel != null) fileContentPanel.SetActive(true);
        
        DisplayActiveFile();
    }

    public void CloseFileViewer()
    {
        if (fileContentPanel != null) fileContentPanel.SetActive(false);
        if (fileGridPanel != null) fileGridPanel.SetActive(true);
        
        // --- TAMBAHAN: Render ulang list agar titik merah yang baru saja hilang tersimpan visualnya ---
        RenderFileList();
    }

    private void DisplayActiveFile()
    {
        if (textFiles.ContainsKey(activeFileName))
        {
            if (fileTitleViewer != null)
            {
                fileTitleViewer.text = activeFileName.ToUpper();
            }

            fileContentViewer.text = textFiles[activeFileName];
        }
    }
}