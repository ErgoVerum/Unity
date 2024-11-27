using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpawnEnemies : MonoBehaviour
{
    public Text Texto;
    public List<GameObject> EnemiesToSpawn;

    private string Texto1,Texto2;
    private bool CanSpawn;
    private float SpawnTimer,Offset;   
    private Camera mainCamera;
   
   void Awake(){DontDestroyOnLoad(gameObject);}

    void Start()
    {
        mainCamera = Camera.main;
        Texto1 = "Current Objective:";
        Texto2 = "Survive";
        StartCoroutine(SpawnEnemy());
        SpawnTimer = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(CanSpawn)
        {
            SpawnTimer -= Time.deltaTime;
            if (SpawnTimer <= 0)
            {SetSpawnEnemies();SpawnTimer = 3f;}
        }
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1f);
        Texto.gameObject.SetActive(true);
        Texto.text = Texto1;
        yield return new WaitForSeconds(1.5f);
        Texto.text = Texto2;
        yield return new WaitForSeconds(1.5f);
        Texto.gameObject.SetActive(false);
        CanSpawn = true;
    }

    private void SetSpawnEnemies()
    {
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(1 + Offset, Random.Range(0.1f, 0.9f), mainCamera.nearClipPlane));
        int SpawnRNG = Random.Range(0,EnemiesToSpawn.Count);
        Instantiate(EnemiesToSpawn[SpawnRNG], spawnPosition, Quaternion.identity);
    }
}
