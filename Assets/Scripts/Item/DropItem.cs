using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private List<GameObject> itemToDrop;
    [SerializeField] private float dropRate;

    private bool isCreated = true;

    private void OnDisable()
    {
        if (isCreated)
        {
            isCreated = false;
        }
        else
        {
            float rand = Random.Range(0f, 1f);

            if (rand < dropRate)
            {
                GameObject g = ItemManager.instance.itemPool.GetObject(itemToDrop[Random.Range(0, itemToDrop.Count)].name);
                g.transform.position = transform.position;
            }

            if(rand > .5f)
            {
                GameObject g = ItemManager.instance.itemPool.GetObject("Gold Bag");
                g.transform.position = transform.position;
            }
        }       
    }
}
