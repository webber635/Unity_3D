using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    [Header("Referensi UI Tombol")]
    [SerializeField] private TMP_Text fileNameText; // Slot untuk Teks Hijau di bawah kotak
    [SerializeField] private Button buttonComponent;  // Slot untuk komponen Button (kotak putih)

    private string fileName;
    private TaskManagerUI taskManager;

    public void Setup(string name, TaskManagerUI manager)
    {
        fileName = name;
        taskManager = manager;
        
        // Ubah teks hijau menjadi nama file (misal: File1.txt)
        if (fileNameText != null)
        {
            fileNameText.text = fileName;
        }

        if (buttonComponent == null) buttonComponent = GetComponent<Button>();

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        taskManager.SelectFile(fileName);
    }
}