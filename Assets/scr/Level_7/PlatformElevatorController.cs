using System.Collections;
using UnityEngine;

public class PlatformElevatorController : MonoBehaviour
{
    [Header("Pengaturan Gerak Lift")]
    [SerializeField] private float stepHeight = 1f;
    [SerializeField] private float stepDuration = 0.5f;
    [SerializeField] private float delayBetweenSteps = 0.2f;

    [Header("Pengaturan Kamera")]
    [Tooltip("Masukkan Main Camera atau Pivot Kamera agar pandangan ikut naik")]
    [SerializeField] private Transform cameraTransform;

    private Vector3 initialPosition;
    private Transform playerTransform;
    private Vector3 initialCameraPosition;

    void Start()
    {
        initialPosition = transform.position;
        if (cameraTransform != null)
        {
            initialCameraPosition = cameraTransform.position;
        }
    }

    public void StartElevation(int steps, Transform player, System.Action onComplete)
    {
        playerTransform = player;
        StartCoroutine(ElevateRoutine(steps, onComplete));
    }

    private IEnumerator ElevateRoutine(int steps, System.Action onComplete)
    {
        // Kunci posisi robot agar menempel di atas pulau
        if (playerTransform != null)
        {
            playerTransform.SetParent(transform);
        }

        int maxSteps = Mathf.Min(steps, 8);

        for (int i = 0; i < maxSteps; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + new Vector3(0, stepHeight, 0);

            // Siapkan juga posisi kamera
            Vector3 camStartPos = Vector3.zero;
            Vector3 camTargetPos = Vector3.zero;

            if (cameraTransform != null)
            {
                camStartPos = cameraTransform.position;
                camTargetPos = camStartPos + new Vector3(0, stepHeight, 0);
            }

            float timer = 0f;
            while (timer < stepDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, timer / stepDuration);

                // Gerakkan pulau
                transform.position = Vector3.Lerp(startPos, targetPos, progress);

                // Gerakkan kamera secara bersamaan
                if (cameraTransform != null)
                {
                    cameraTransform.position = Vector3.Lerp(camStartPos, camTargetPos, progress);
                }

                yield return null;
            }

            // Kunci posisi akhir untuk frame ini
            transform.position = targetPos;
            if (cameraTransform != null) cameraTransform.position = camTargetPos;

            yield return new WaitForSeconds(delayBetweenSteps);
        }

        // Lepaskan robot
        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
        }

        onComplete?.Invoke();
    }


}