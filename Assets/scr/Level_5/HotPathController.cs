using UnityEngine;

public class HotPathController : MonoBehaviour
{
    [Header("Referensi Visual")]
    [SerializeField] private MeshRenderer pathRenderer;
    
    [Header("Pengaturan Warna")]
    [SerializeField] private Color hotColor = Color.red;
    [SerializeField] private Color safeColor = Color.white;

    private void Start()
    {
        // Kondisi awal selalu panas
        SetPathHot();
    }

    public void SetPathHot()
    {
        if (pathRenderer != null)
        {
            pathRenderer.material.color = hotColor;
        }
        
        // Mengubah tag menjadi Hazard agar dideteksi oleh PlayerGridMovement
        gameObject.tag = "Hazard";
    }

    public void SetPathSafe()
    {
        if (pathRenderer != null)
        {
            pathRenderer.material.color = safeColor;
        }

        // Menghilangkan tag Hazard agar robot aman melintas
        gameObject.tag = "Untagged";
        
        Debug.Log("Jalur mendingin dan sekarang aman dilewati.");
    }
}