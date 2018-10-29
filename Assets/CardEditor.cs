using UnityEngine;
using System.Collections.Generic;
using Cards;

public class CardEditor : MonoBehaviour
{
    public List<CardCollection> CollectionLibrary = new List<CardCollection>();

    public CardCollection CurrentCollection = new CardCollection();

    public WhiteCard CreateWhiteCard()
    {
        string text = "";
        


        return new WhiteCard(CollectionLibrary,new Description("us", text));
    }
}
