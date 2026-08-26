using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TaskManagerUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject taskManagerPanel;
    [SerializeField] private Transform fileListContainer; 
    [SerializeField] private TMP_Text fileContentViewer;   
    [SerializeField] private GameObject fileButtonPrefab;  

    // >>> TAMBAHKAN VARIABEL STATIS INI <<<
    public static bool isTaskManagerActive = false; 

    private Dictionary<string, string> textFiles = new Dictionary<string, string>();
    private string activeFileName = "";

    void Start()
    {
        taskManagerPanel.SetActive(false);
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
        // ... (Kode AddTextFile tetap sama seperti aslinya)
        if (!textFiles.ContainsKey(name))
        {
            textFiles.Add(name, content);
            if (string.IsNullOrEmpty(activeFileName))
            {
                activeFileName = name;
            }
        }
    }

    private void ToggleTaskManager()
    {
        bool isActive = !taskManagerPanel.activeSelf;
        taskManagerPanel.SetActive(isActive);

        // >>> UPDATE STATUS VARIABEL STATIS <<<
        isTaskManagerActive = isActive; 

        if (isActive)
        {
            RenderFileList();
            DisplayActiveFile();
        }
    }

    // ... (Sisa fungsi RenderFileList, SelectFile, dan DisplayActiveFile tetap sama)

    // Menampilkan daftar tombol file di panel kiri
    private void RenderFileList()
    {
        // Bersihkan tombol lama agar tidak menumpuk
        foreach (Transform child in fileListContainer)
        {
            Destroy(child.gameObject);
        }

        // Buat tombol baru untuk setiap file
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

    // Memilih file yang diklik pemain
    public void SelectFile(string fileName)
    {
        activeFileName = fileName;
        DisplayActiveFile();
    }

    // Menampilkan isi teks file yang sedang aktif di panel kanan
    private void DisplayActiveFile()
    {
        if (textFiles.Count == 0 || string.IsNullOrEmpty(activeFileName))
        {
            fileContentViewer.text = "// NO TXT FILES FOUND IN STORAGE.\nFind a USB Drive in the area.";
            return;
        }

        if (textFiles.ContainsKey(activeFileName))
        {
            fileContentViewer.text = $"<b>=== {activeFileName.ToUpper()} ===</b>\n\n{textFiles[activeFileName]}";
        }
    }
}