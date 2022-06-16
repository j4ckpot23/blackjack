using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;
    private int standClicks = 0; 

    //Access both hands
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText; 
    public Text cashText;
    public Text mainText;
    public Text standBtnText;
    public Text titleText;
    public Text buyInText;

    public Image titleImage;

    public GameObject hideCard; //Hidden card
    int pot = 0;

    void Start()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        betBtn.onClick.AddListener(() => BetClicked());

        //Change visibility for a few game objects
        titleImage.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        hitBtn.gameObject.SetActive(false);      
        standBtn.gameObject.SetActive(false);    
        dealerScoreText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
    }

    private void DealClicked()
    {
        FindObjectOfType<AudioManager>().Play("Deal"); //sound effect

        playerScript.ResetHand();
        dealerScript.ResetHand();

        titleImage.gameObject.SetActive(false); //get rid of start menu stuff
        titleText.gameObject.SetActive(false);
        buyInText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false); 
        betBtn.gameObject.SetActive(false);

        scoreText.gameObject.SetActive(true); //add score coutner

        dealerScoreText.gameObject.SetActive(false); //Hides the dealer hand score at the start of the deal
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();

        scoreText.text = "Hand: " + playerScript.handValue.ToString();        //Updates the player score
        dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();  //Updates the dealer score

        hideCard.GetComponent<Renderer>().enabled = true;
        
        dealBtn.gameObject.SetActive(false);    //Hides deal button when deal is clicked
        hitBtn.gameObject.SetActive(true);      //Reveals hit button when deal is clicked
        standBtn.gameObject.SetActive(true);    //Reveals stand button when deal is clicked
        standBtnText.text = "Stand";

        pot += 20;                              //Add $20 when Deal is clicked
        betsText.text = "Bet: $" + pot.ToString();
        playerScript.AdjustMoney(-20);
        cashText.text = "$" + playerScript.GetMoney().ToString();

        if (playerScript.handValue == 21) //check for lucky player blackjack
        {
            RoundOver();
        }

    }

    private void HitClicked() //when player wants a hit
    {
        FindObjectOfType<AudioManager>().Play("Hit");

        //Check for room on the table
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) //either blackjack or bust
            {
                RoundOver();
            }
        }  
    }

    private void StandClicked() //when player chooses to stand
    {
        standClicks++;
        if (standClicks > 1) //if second time standing
        {
            RoundOver();
        }
        HitDealer();
        standBtnText.text = "Call";
        FindObjectOfType<AudioManager>().Play("Stand");
    }

    private void HitDealer() //hitting the dealer
    {
        while(dealerScript.handValue < 16 && dealerScript.cardIndex < 10) //if dealer has less than 16 score and 10 cards
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();

            if (dealerScript.handValue > 20)
            {
                RoundOver();
            }
        }
    }

    void RoundOver() //round over, checking for winner and loser
    {
        bool playerBust = playerScript.handValue > 21; //player bust
        bool dealerBust = dealerScript.handValue > 21; //dealer bust
        bool player21 = playerScript.handValue == 21; //player blackjack
        bool dealer21 = dealerScript.handValue == 21; //dealer blackjack

        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) //keep playing
        {
            return;
        }

        bool roundOver = true;

        if (playerBust && dealerBust) //double bust
        {
            mainText.text = "Double Bust!";
            playerScript.AdjustMoney(pot / 2);
        }

        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue)) //player busts
        {
            mainText.text = "Dealer Wins!";
        }

        else if (dealerBust || playerScript.handValue > dealerScript.handValue) //dealer busts
        {
            mainText.text = "Player Wins!";
            playerScript.AdjustMoney(pot * 2);
        }

        else if (playerScript.handValue == dealerScript.handValue) //tie
        {
            if (playerScript.handValue == 21 && dealerScript.handValue == 21)
            {
                mainText.text = "Wow! Double Blackjack Tie!";
                playerScript.AdjustMoney(pot / 2);
            }

            else
            {
                mainText.text = "Tie!";
                playerScript.AdjustMoney(pot / 2);
            }           
        }

        else
        {
            roundOver = false;
        }

        if (roundOver)
        {
            hitBtn.gameObject.SetActive(false); //change some visibility bindings
            standBtn.gameObject.SetActive(false);  
            dealBtn.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            betBtn.gameObject.SetActive(true);
            
            hideCard.GetComponent<Renderer>().enabled = false;       //add pot and reset
            cashText.text = "$" + playerScript.GetMoney().ToString(); 
            standClicks = 0;
            betsText.gameObject.SetActive(false);
            buyInText.gameObject.SetActive(true);
            pot = 0;
            
        }
    }

    void BetClicked() //when you wanna add more money
    {
        FindObjectOfType<AudioManager>().Play("Chip"); //plays the sound that people make when eating chips, get it??? no..?

        betsText.gameObject.SetActive(true); //for when you try to bet after a round is over

        Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text; //fun text parcing
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));

        playerScript.AdjustMoney(-intBet); //get rid of the pot money 
        cashText.text = "$" + playerScript.GetMoney().ToString();
        pot += (intBet);
        betsText.text = "Bet: $" + pot.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
