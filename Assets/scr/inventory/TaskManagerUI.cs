using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TaskManagerUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject taskManagerPanel; // Panel Utama (Background Hitam)
    [SerializeField] private GameObject fileGridPanel;    // Panel yang berisi kotak-kotak file
    [SerializeField] private GameObject fileContentPanel; // Panel yang muncul saat file dibaca

    [Header("UI Elements")]
    [SerializeField] private Transform fileListContainer; 
    [SerializeField] private TMP_Text fileContentViewer;
    [SerializeField] private TMP_Text fileTitleViewer;   
    [SerializeField] private GameObject fileButtonPrefab;  

    public static bool isTaskManagerActive = false; 

    private Dictionary<string, string> textFiles = new Dictionary<string, string>();
    private string activeFileName = "";

    void Start()
    {
        if (taskManagerPanel != null) taskManagerPanel.SetActive(false);
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
        }
    }

    // Dipanggil oleh Tombol "X" atau "Close" di sudut panel utama
    public void CloseTaskManager()
    {
        if (taskManagerPanel != null) taskManagerPanel.SetActive(false);
        isTaskManagerActive = false; 
    }

    private void ToggleTaskManager()
    {
        bool isActive = !taskManagerPanel.activeSelf;
        taskManagerPanel.SetActive(isActive);
        isTaskManagerActive = isActive; 

        if (isActive)
        {
            // Pastikan saat Tab ditekan, yang muncul adalah Grid Menu
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
                btnScript.Setup(file.Key, this);
            }
        }
    }

    // Dipanggil oleh FileButton saat pemain mengklik kotak putih
    public void SelectFile(string fileName)
    {
        activeFileName = fileName;
        
        // Sembunyikan Grid, Munculkan layar baca teks
        if (fileGridPanel != null) fileGridPanel.SetActive(false);
        if (fileContentPanel != null) fileContentPanel.SetActive(true);
        
        DisplayActiveFile();
    }

    // Dipanggil oleh tombol "BACK" di panel baca file
    public void CloseFileViewer()
    {
        if (fileContentPanel != null) fileContentPanel.SetActive(false);
        if (fileGridPanel != null) fileGridPanel.SetActive(true);
    }

    private void DisplayActiveFile()
    {
        if (textFiles.ContainsKey(activeFileName))
        {
            // Mengubah teks judul di atas (beserta format huruf kapital)
            if (fileTitleViewer != null)
            {
                fileTitleViewer.text = activeFileName.ToUpper();
            }

            // Mengubah isi teks di bawahnya (sekarang murni hanya konten file)
            fileContentViewer.text = textFiles[activeFileName];
        }
    }
}