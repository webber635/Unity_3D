using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI World Space Reference")]
    [SerializeField] private TMP_Text floatingHintText;
    [SerializeField] private GameObject worldCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ShowHint("Tekan [TAB] Task Manager");
        Invoke(nameof(HideHint), 4f);
    }

    public void ShowHint(string message)
    {
        CancelInvoke(nameof(HideHint));
        floatingHintText.text = message;
        worldCanvas.SetActive(true);
    }

    public void HideHint()
    {
        worldCanvas.SetActive(false);
    }
}