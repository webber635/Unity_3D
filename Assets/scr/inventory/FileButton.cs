using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    [Header("Referensi UI Tombol")]
    [SerializeField] private TMP_Text fileNameText; 
    [SerializeField] private Button buttonComponent;  
    
    // --- TAMBAHAN: Referensi untuk Titik Merah ---
    [SerializeField] private GameObject redDotIndicator;
    [SerializeField] private GameObject inventoryRedDot; 

    private string fileName;
    private TaskManagerUI taskManager;

    // --- TAMBAHAN: Tambahkan parameter bool 'isNew' ---
    public void Setup(string name, TaskManagerUI manager, bool isNew)
    {
        fileName = name;
        taskManager = manager;
        
        if (fileNameText != null)
        {
            fileNameText.text = fileName;
        }

        // Aktifkan titik merah jika file ini berstatus 'baru'
        if (redDotIndicator != null)
        {
            redDotIndicator.SetActive(isNew);
        }

        if (buttonComponent == null) buttonComponent = GetComponent<Button>();

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Sembunyikan titik merah secara instan saat tombol diklik
        if (redDotIndicator != null) 
        {
            redDotIndicator.SetActive(false);
        }
        
        taskManager.SelectFile(fileName);
    }
}