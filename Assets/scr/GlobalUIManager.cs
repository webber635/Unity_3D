using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance;

    private void Awake()
    {
        // Jika belum ada Global UI, jadikan ini sebagai yang utama dan lindungi dari kehancuran
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Jika sudah ada Global UI dari level sebelumnya, hancurkan yang baru agar tidak dobel
        else
        {
            Destroy(gameObject);
        }
    }
}