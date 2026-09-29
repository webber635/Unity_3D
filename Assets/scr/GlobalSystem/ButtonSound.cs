using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        
        // Menambahkan fungsi suara secara otomatis saat tombol diklik
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        if (UISoundManager.Instance != null)
        {
            UISoundManager.Instance.PlayButtonClick();
        }
    }
}