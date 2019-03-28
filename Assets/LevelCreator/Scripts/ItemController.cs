using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public class ItemController : MonoBehaviour
{
    public Item item;
    public Sprite Sprite;

    private void Awake()
    {
        item = new Item();
        item.isConsumable(false);
        item.SetSprite(Sprite);
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Items";
    }
}
