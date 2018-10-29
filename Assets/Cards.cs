using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class BlackCard : Card
    {
        public int NumberOfAnswers { get; private set; }

        public BlackCard(int _NumAnsw, List<CardCollection> CardList, Description CardText) : base(CardList, CardText)
        {
            NumberOfAnswers = _NumAnsw;
        }
    }
    public class WhiteCard : Card
    {
        public WhiteCard(List<CardCollection> CardList, Description CardText) : base(CardList, CardText)
        {
        }
    }
    public class Card
    {
        public float Id { get; private set; } = 0;
        public string Text { get; private set; }
        public List<string> AvailableLoc { get; private set; }
        public float Rating { get; private set; } = 2.5f;

        public Card(List<CardCollection> cardCollection, Description _Text)
        {

            foreach (CardCollection coll in cardCollection)
            {
                Id += coll.Count();
            }
            if (_Text.Text == "")
                Text = "Jamal forgot to add text.\n Niggers don't work as they did back in the Ol'Murica.";
            else
            {
                AddDesc(_Text);
            }
        }
        public void AddDesc(Description desc)
        {
            if (!HasLoc(desc.LocCode))
            {
                Text = desc.Text;
                AvailableLoc.Add(desc.LocCode);
            }
        }
        public bool HasLoc(string LocCode)
        {
            return AvailableLoc.Contains(LocCode);
        }
        public void UpdateRating()
        {
        }
    }
    public struct Description
    {
        public string LocCode { get; private set; }
        public string Text { get; private set; }

        public Description(string _LocCode, string _Text)
        {
            LocCode = _LocCode;
            Text = _Text;
        }
    }
    public class CardCollection
    {
        public List<BlackCard> BlackCards { get; private set; } = new List<BlackCard>();
        public List<WhiteCard> WhiteCards { get; private set; } = new List<WhiteCard>();

        public string author = "";
        
        public CardCollection()
        {
        }
        public int Count()
        {
            return BlackCards.Count + WhiteCards.Count;
        }
    }
}