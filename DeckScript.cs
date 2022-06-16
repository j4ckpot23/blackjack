using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    int[] cardValues = new int[53];
    int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetCardValues();
    }

    // Update is called once per frame
    void GetCardValues()
    {
        int num = 0;

        //Loop through cards assign values to them
        for (int i = 0; i < cardSprites.Length; i++)
        {
            num = i;
            //Count to 52
            num %= 13;
            //Assignes the royal cards to 10
            if (num > 10 || num == 0)
            {
                num = 10;
            }
            cardValues[i] = num++;
        }
    }

    public void Shuffle() //shuffle the cards
    {
        for (int i = cardSprites.Length - 1; i > 0; --i) //for however many cards are in the deck
        {
            int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * (cardSprites.Length - 1)) + 1; //random numbers
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int value = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = value;
        }
        currentIndex = 1;
    }

    public int DealCard(CardScript cardScript) //dealing a card
    {
        cardScript.SetSprite(cardSprites[currentIndex]);
        cardScript.SetValue(cardValues[currentIndex++]);
        return cardScript.GetCardValue();
    }

    public Sprite GetCardBack()
    {
        return cardSprites[0];
    }
}
