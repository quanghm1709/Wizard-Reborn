using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactive : MonoBehaviour
{
    [SerializeField] private float resetLifeTime;
    private float lifeTime;

    private void OnEnable()
    {
        lifeTime = resetLifeTime;
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
