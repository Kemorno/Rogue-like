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

    // Start is called before the first frame update
    void Start()
    {
        FileHandler.ImportFiles(game);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
