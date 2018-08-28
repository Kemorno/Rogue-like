using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Resources;

public class cameraGUI : MonoBehaviour {

    CreateLevel map;

    public Text toolTip;

    private void Awake()
    {
        map = GameObject.Find("Creator").GetComponent<CreateLevel>();
    }

    private void Update()
    {
        string tooltiptext = "";
        Vector3 mousePos = new Vector3(map.mousePos.x, map.mousePos.y);
        //toolTip.transform.position = map.guide.transform.position;

        if (map.Map.ContainsKey(new CoordInt(mousePos)))
        {
            Tile tile = map.Map[new CoordInt(mousePos)];
            tooltiptext += "Room ID:" + tile.RoomId;
            tooltiptext += "\nCoord:" + tile.Coord.ToString();
            tooltiptext += "\nType:" + tile.Type.ToString();
            toolTip.text = tooltiptext;
        }
        else
        {
            toolTip.text = "";
        }
    }
}
