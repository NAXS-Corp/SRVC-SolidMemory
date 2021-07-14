using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NXTool_Benchmark : MonoBehaviour
{
    public GameObject Prefab;
    public int SpawnNum = 100;
    public Vector3 RandomPos = new Vector3(50,0,50);
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < SpawnNum; i++){
            Vector3 spawnPoint = new Vector3(Random.Range(-RandomPos.x, RandomPos.x), 0, Random.Range(-RandomPos.z, RandomPos.z));
            GameObject.Instantiate(Prefab, spawnPoint, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
