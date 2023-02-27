using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    private void Start()
    {
        instance = this;
    }

    public IEnumerator GenerateEnemy(Transform spawnPoint, int totalWave)
    {
        yield return new WaitForSeconds(1f);
    }
}
