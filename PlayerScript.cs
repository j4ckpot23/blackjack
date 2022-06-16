using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // For player and dealer

    public CardScript cardScript;
    public DeckScript deckScript;

    public int handValue = 0;

    private int money = 1000;

    public GameObject[] hand;

    public int cardIndex = 0;

    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    public int GetCard()
    {
        //Get card, use deal card to assign sprite and value to the card
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        //Show on screen
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        handValue += cardValue;
        //cardValue = 1 is an ace
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        AceCheck();
        cardIndex++;
        return handValue;
    }

    public void AceCheck() //handles what value aces will be
    {
        foreach(CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetCardValue() == 1) //when we want Ace value equal to 11
            {
                ace.SetValue(11);
                handValue += 10;
            }

            else if (handValue > 21 && ace.GetCardValue() == 11) //when we want Ace value equal to 1
            {
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    public int GetMoney()
    {
        return money;
    }

    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
    }
}
