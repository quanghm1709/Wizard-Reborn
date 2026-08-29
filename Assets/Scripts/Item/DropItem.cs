using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private List<GameObject> itemToDrop;
    [SerializeField] private float dropRate;

    public void TryDrop()
    {
        if (ItemManager.instance == null || ItemManager.instance.itemPool == null)
        {
            return;
        }

        float rand = Random.Range(0f, 1f);

        if (rand < dropRate && itemToDrop.Count > 0)
        {
            GameObject item = ItemManager.instance.itemPool.GetObject(itemToDrop[Random.Range(0, itemToDrop.Count)].name);
            if (item != null)
            {
                item.transform.position = transform.position;
            }
        }

        if(rand > .5f)
        {
            GameObject gold = ItemManager.instance.itemPool.GetObject("Gold Bag");
            if (gold != null)
            {
                gold.transform.position = transform.position;
            }
        }
    }
}
