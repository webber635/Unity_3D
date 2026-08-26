using UnityEngine;
using System.Collections;

public class JembatanController : MonoBehaviour
{
    [Header("Referensi Objek")]
    [SerializeField] private Transform[] tilesVisual;   
    [SerializeField] private GameObject jurangBlocker;  

    [Header("Pengaturan Animasi")]
    [SerializeField] private float targetY_Atas = 0f;        
    [SerializeField] private float kecepatanNaik = 3f; 
    [SerializeField] private float delayAntarTile = 0.3f; 

    private float targetY_Bawah; 
    private float currentTargetY;
    private bool[] tileAktif; 

    void Start()
    {
        if (jurangBlocker != null) jurangBlocker.SetActive(true);
        tileAktif = new bool[tilesVisual.Length];
        
        // Simpan posisi Y awal (di dasar jurang) sebagai target saat jembatan dimatikan
        if (tilesVisual.Length > 0 && tilesVisual[0] != null)
        {
            targetY_Bawah = tilesVisual[0].position.y; 
        }
    }

    void Update()
    {
        for (int i = 0; i < tilesVisual.Length; i++)
        {
            if (tileAktif[i] && tilesVisual[i] != null)
            {
                // Bergerak menuju currentTargetY (bisa ke atas atau ke bawah)
                Vector3 targetPos = new Vector3(tilesVisual[i].position.x, currentTargetY, tilesVisual[i].position.z);
                tilesVisual[i].position = Vector3.Lerp(tilesVisual[i].position, targetPos, Time.deltaTime * kecepatanNaik);

                if (Vector3.Distance(tilesVisual[i].position, targetPos) < 0.01f)
                {
                    tilesVisual[i].position = targetPos;
                    tileAktif[i] = false; 
                }
            }
        }
    }

    public void TurnOnBridge()
    {
        if (jurangBlocker != null) jurangBlocker.SetActive(false);
        currentTargetY = targetY_Atas;
        StartCoroutine(MulaiAnimasi());
    }

    public void TurnOffBridge()
    {
        // Tutup jalan lagi karena jembatan akan turun
        if (jurangBlocker != null) jurangBlocker.SetActive(true);
        currentTargetY = targetY_Bawah; 
        StartCoroutine(MulaiAnimasi());
    }

    private IEnumerator MulaiAnimasi()
    {
        for (int i = 0; i < tilesVisual.Length; i++)
        {
            tileAktif[i] = true;
            yield return new WaitForSeconds(delayAntarTile);
        }
    }
}