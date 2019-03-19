using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Resources;
using Enums;

public class cameraGUI : MonoBehaviour {

    CreateLevel main;

    public Text Seed;
    public Text TileInfo;
    public Text RoomInfo;
    public Slider NumberOfRooms;
    public Text NumberOfRoomsText;
    public Dropdown RoomSize;

    private void Awake()
    {
        main = GameObject.Find("Creator").GetComponent<CreateLevel>();
        NumberOfRooms.value = main.noRooms;
        switch (main.RoomSize)
        {
            case roomSize.Tiny:
                RoomSize.value = 0;
                break;
            case roomSize.Small:
                RoomSize.value = 1;
                break;
            case roomSize.Medium:
                RoomSize.value = 2;
                break;
            case roomSize.Large:
                RoomSize.value = 3;
                break;
            case roomSize.Big:
                RoomSize.value = 4;
                break;
        }
    }

    private void Update()
    {
        if (main != null)
        {
            Seed.text = main.globalSeed.ToString();
            if (main.Map != null)
            {
                NumberOfRoomsText.text = "Number of Rooms: " + main.noRooms;
                if (main.mousePos != null)
                {
                    CoordInt newMousePos = new CoordInt(Mathf.FloorToInt(main.mousePos.x + .5f), Mathf.FloorToInt(main.mousePos.y + .5f));
                    if (main.Map.ContainsKey(newMousePos))
                    {
                        TileInfo.text = main.Map[newMousePos].ToString();
                        RoomInfo.text = main.Rooms[main.Map[newMousePos].RoomId].ToString();
                    }
                    else
                    {
                        TileInfo.text = "";
                        RoomInfo.text = "";
                    }
                }
            }
        }
    }

    public void UpdateNumberOfRooms()
    {
        main.noRooms = (int)NumberOfRooms.value;
    }

    public void UpdateRoomSize()
    {
        switch (RoomSize.value)
        {
            case 0:
                main.RoomSize = roomSize.Tiny;
                break;
            case 1:
                main.RoomSize = roomSize.Small;
                break;
            case 2:
                main.RoomSize = roomSize.Medium;
                break;
            case 3:
                main.RoomSize = roomSize.Big;
                break;
            case 4:
                main.RoomSize = roomSize.Large;
                break;
        }
    }
}