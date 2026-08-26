using UnityEngine;

public class FlashdiskItem : MonoBehaviour
{
    [Header("Identitas File")]
    [SerializeField] private string fileName = "laser_guide.txt";
    
    [TextArea(5, 10)]
    [SerializeField] private string fileContent = "SYSTEM LOG:\nUntuk mematikan rintangan laser, ubah variabel berikut di terminal:\n\nlaser = false";

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        TaskManagerUI taskManager = FindFirstObjectByType<TaskManagerUI>();
        if (taskManager != null)
        {
            taskManager.AddTextFile(fileName, fileContent);
            HUDManager.Instance.ShowHint($"Flashdisk diambil! Tekan [TAB] untuk membaca {fileName}");
            Destroy(gameObject);
        }
    }
}
}