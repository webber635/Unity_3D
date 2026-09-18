using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class JetExitController : MonoBehaviour
{
    [Header("Pengaturan Jet")]
    [SerializeField] private float warmUpDelay = 1.0f;  
    [SerializeField] private float flySpeed = 5f;       
    [SerializeField] private float flyDuration = 1.5f;   
    
    [Header("Referensi Visual (Opsional)")]
    [SerializeField] private GameObject jetFireEffect;  

    private bool isActivated = false;

    void Start()
    {
        if (jetFireEffect != null) jetFireEffect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            Transform player = other.transform;

            var movement = player.GetComponent<PlayerGridMovement>();
            if (movement != null) movement.enabled = false;

            StartCoroutine(JetSequenceRoutine(player));
        }
    }

    IEnumerator JetSequenceRoutine(Transform player)
    {
        // TAHAP 1: WARM-UP
        if (jetFireEffect != null) jetFireEffect.SetActive(true);
        yield return new WaitForSeconds(warmUpDelay);

        // TAHAP 2: MENGIKAT PLAYER
        player.SetParent(transform);
        Vector3 fixedPos = player.localPosition;
        fixedPos.y = 0.7f; 
        player.localPosition = fixedPos;

        // TAHAP 3: TERBANG
        float timer = 0f;
        while (timer < flyDuration)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector3.up * flySpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // TAHAP 4: TAMPILKAN PANEL KEMENANGAN DARI LEVEL STATS GLOBAL
        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.ShowVictoryPanel(LevelStats.totalErrors, LevelStats.totalWarnings);
        }
    }
}