using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public class ItemController : MonoBehaviour
{
    public Item item;

    private void Awake()
    {
        item = new Item();
    }
}
