using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Resources;
using Enums;

[CustomEditor(typeof(ItemController))]
public class ItemControllerEditor : Editor
{
    public bool Consumable;
    public int Uses;
    public bool inCooldown;
    public float CooldownTime;
    public Sprite Sprite;
    ItemType Type;
    Item item = new Item();

    public override void OnInspectorGUI()
    {
        ItemController main = (ItemController)target;
        base.OnInspectorGUI();
    }
}
