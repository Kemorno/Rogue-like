using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Resources;
using Enums;
using System;

public class Game : MonoBehaviour
{
    IGame game = new IGame();
    public Image Large;
    public Image Big;
    public Image Medium;
    public Image Small;
    public Image Tiny;
    public Image Minimal;

    // Start is called before the first frame update
    void Start()
    {
        LogHandler.ClearLog();
        FileHandler.ImportFiles(game);

        if (game.Effects.Count > 0)
        {
            Debug.Log("Setting Sprites");
            Large.sprite = game.Effects["Electric Effect"].Sprite.Sprites[512];
            Big.sprite = game.Effects["Electric Effect"].Sprite.Sprites[256];
            Medium.sprite = game.Effects["Electric Effect"].Sprite.Sprites[128];
            Small.sprite = game.Effects["Electric Effect"].Sprite.Sprites[64];
            Tiny.sprite = game.Effects["Electric Effect"].Sprite.Sprites[32];
            Minimal.sprite = game.Effects["Electric Effect"].Sprite.Sprites[16];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
