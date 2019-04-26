using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewcameraGUI : MonoBehaviour
{
    public List<GameObject> Buttons = new List<GameObject>();
    public NewGenerator main;
    public enum selectedButton { None, Noise , Room, Connect, Erase };
    selectedButton selected;

    private void Awake()
    {
        switch (main.selected)
        {
            case selectedButton.Connect:
                Connect();
                break;
            case selectedButton.Erase:
                Erase();
                break;
            case selectedButton.Noise:
                Noise();
                break;
            case selectedButton.Room:
                Room();
                break;
        }
    }

    private void Update()
    {
        main.selected = selected;
    }

    public void Room()
    {
        selected = selectedButton.Room;
        Buttons[0].GetComponent<Button>().interactable = true;
        Buttons[1].GetComponent<Button>().interactable = false;
        Buttons[2].GetComponent<Button>().interactable = true;
        Buttons[3].GetComponent<Button>().interactable = true;
    }
    public void Noise()
    {
        selected = selectedButton.Noise;
        Buttons[0].GetComponent<Button>().interactable = false;
        Buttons[1].GetComponent<Button>().interactable = true;
        Buttons[2].GetComponent<Button>().interactable = true;
        Buttons[3].GetComponent<Button>().interactable = true;
    }
    public void Connect()
    {
        selected = selectedButton.Connect;
        Buttons[0].GetComponent<Button>().interactable = true;
        Buttons[1].GetComponent<Button>().interactable = true;
        Buttons[2].GetComponent<Button>().interactable = false;
        Buttons[3].GetComponent<Button>().interactable = true;
    }
    public void Erase()
    {
        selected = selectedButton.Erase;
        Buttons[0].GetComponent<Button>().interactable = true;
        Buttons[1].GetComponent<Button>().interactable = true;
        Buttons[2].GetComponent<Button>().interactable = true;
        Buttons[3].GetComponent<Button>().interactable = false;
    }
}
