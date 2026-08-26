using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    private string fileName;
    private TaskManagerUI taskManager;

    public void Setup(string name, TaskManagerUI manager)
    {
        fileName = name;
        taskManager = manager;
        buttonText.text = $"📄 {fileName}";

        // Menambahkan listener klik secara otomatis
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        taskManager.SelectFile(fileName);
    }
}