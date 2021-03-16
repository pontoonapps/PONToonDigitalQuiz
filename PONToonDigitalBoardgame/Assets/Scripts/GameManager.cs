using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum CardDeckType
{
    TECHNOLOGY,
    PERSONAL_SERVICES,
    FOOD_INDUSTRY
};

[System.Serializable]
public struct CardInfo
{
    public CardDeckType Deck;
    public string path;
    public string DisplayName;
};

[System.Serializable]
public class QuestionInfo
{
    public QuestionInfo(Dictionary<string, object> theData)
    {
        //ID = int.Parse((string)theData["ID"]);
        Difficulty = (string)theData["Difficulty"];
        Question = (string)theData["Question"];
        CorrectAnswer = (string)theData["CorrectAnswer"];

        WrongAnswers = new List<string>();

        for(int i = 2; i <= 6; i++)
        {
            string val = (string)theData["Answer" + i];
            if (val.Length != 0)
            {
                //Not empty
                WrongAnswers.Add(val);
            }
        }
    }

    public int ID;
    public string Difficulty;
    public string Question;
    public string CorrectAnswer;
    public List<string> WrongAnswers;
}

[System.Serializable]
public class CardDeck
{
    public CardDeckType DeckType;


    public List<QuestionInfo> QuestionsData;
    public List<QuestionInfo> DiscardPile;
    public CardDeck(CardInfo cardDeck)
    {
        DiscardPile = new List<QuestionInfo>();
        QuestionsData = new List<QuestionInfo>();
        DeckType = cardDeck.Deck;

        //Load questions
        List<Dictionary<string, object>> data = CSVReader.Read(cardDeck.path);

        for (int i = 0; i < data.Count; i++)
        {
            QuestionsData.Add(new QuestionInfo(data[i]));
        }

        //Shuffle
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = QuestionsData.Count-1; i > 0; i--)
        {
            int j = Random.Range(0, i+1);
            var temp = QuestionsData[i];
            QuestionsData[i] = QuestionsData[j];
            QuestionsData[j] = temp;
        }
    }

    public QuestionInfo DrawCard()
    {
        QuestionInfo rv = null;

        if (QuestionsData.Count > 0)
        {
            rv = QuestionsData[QuestionsData.Count - 1];
            DiscardPile.Add(rv); // not in the main deck anymore, but still keep reference to it in discard pile
            QuestionsData.RemoveAt(QuestionsData.Count - 1);
        }

        return rv;
    }

    public List<QuestionInfo> DrawCards(int amount)
    {
        List<QuestionInfo> rv = new List<QuestionInfo>();

        for(int i = 0; i < amount; i++)
        {
            QuestionInfo drawnCard = DrawCard();

            if(drawnCard != null)
            {
                rv.Add(drawnCard);
            }
        }

        return rv;
    }


}

public class GameManager : MonoBehaviour
{
    public int Score = 0;
    public int Round = 0;
    private float timer;
    public bool IsCountingDown = false;
    public float TimeToAnswer = 120.0f;

    public CardInfo[] GameInfo;

    public List<CardDeck> Decks;

    public GameObject CardBackPrefab;

    public GameObject[] CanvasPanelRef;

    public GameObject TimerTextRef;

    public GameObject ScoreScreenRef;

    public static GameManager GMRef = null;
    public static GameManager Instance
    {
        get
        {
            if (GMRef == null)
            {
                GMRef = GameObject.Find("Main Camera").GetComponent<GameManager>();
            }

            return GMRef;
        }
    }

    private GameObject QuestionPanelRef;

    // Start is called before the first frame update
    void Start()
    {
        Decks = new List<CardDeck>();
        foreach(CardInfo ci in GameInfo)
        {
            Decks.Add(new CardDeck(ci));
        }


        //Need to instantiate 2 cards from each deck (3 decks, so 6 cards total)
        DrawCardsFromDeck(2);

        timer = TimeToAnswer;
        
    }

    private void Update()
    {
        //Updating UI text for timer
        if (IsCountingDown)
        {
            timer -= Time.deltaTime;
            TimerTextRef.GetComponent<Text>().text = timer.ToString("#.0") + " seconds";
        }
    }

    public void StartTimer()
    {
        if (!IsInvoking("TimesUp"))
        {
            Invoke("TimesUp", TimeToAnswer);
            IsCountingDown = true;
        }
    }

    void TimesUp()
    {
        //Need to hide QuestionPanel if open
        Destroy(QuestionPanelRef);

        Round++;
        IsCountingDown = false;
        timer = TimeToAnswer;
        
        foreach (GameObject canvas in CanvasPanelRef)
        {
            for (int i = canvas.transform.childCount-1; i >= 0; i--)
            {
                Destroy(canvas.transform.GetChild(i).gameObject);
            }
        }

        if (Round > 2)
        {
            TimerTextRef.GetComponent<Text>().text = "FIN";

            //Display score
            ScoreScreenRef.SetActive(true);
            ScoreScreenRef.transform.GetChild(1).GetComponent<Text>().text = Score.ToString();
        }
        else
        {
            //Draw new cards for next round
            DrawCardsFromDeck(2);
            TimerTextRef.GetComponent<Text>().text = "NEXT ROUND!";
        }

        
    }

    void DrawCardsFromDeck(int count)
    {
        for(int j = 0; j < Decks.Count; j++)
        {
            for(int i = 0; i < count; i++)
            {
                QuestionInfo qi = Decks[j].DrawCard();

                if (qi != null)
                {
                    GameObject go = Instantiate(CardBackPrefab);
                    go.GetComponent<CardUI>().Init(qi, GameInfo[j].DisplayName);
                    go.transform.SetParent(CanvasPanelRef[j].transform);
                }
            }
        }
    }

    public void CardWasPressed(GameObject QuestionPanelRef)
    {
        this.QuestionPanelRef = QuestionPanelRef;
        StartTimer();
    }

    public void QuestionAnswered(bool Iscorrect)
    {
        if(Iscorrect)
        {
            Score++;
        }

        bool AnyCardsLeft = false;

        foreach (GameObject canvas in CanvasPanelRef)
        {
            if(canvas.transform.childCount > 0)
            {
                AnyCardsLeft = true;
                break;
            }
        }

        //If no more cards left to select, then finish the round
        if(!AnyCardsLeft)
        {
            Debug.Log("Round finished");
            CancelInvoke("TimesUp");
            TimesUp();
        }
    }


}
