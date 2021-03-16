using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanelUI : MonoBehaviour
{
    public Transform AnswerPanel;
    public Text QuestionText;

    public GameObject AnswerButtonPrefab;

    public void Init(QuestionInfo qi)
    {
        QuestionText.text = qi.Question;

        List<GameObject> answerButtons = new List<GameObject>();

        //Do the correct answer
        GameObject correctAnswer = Instantiate(AnswerButtonPrefab);
        correctAnswer.GetComponent<AnswerButtonUI>().Init(qi.CorrectAnswer, true, this);
        answerButtons.Add(correctAnswer);
        
        //Incorrect answers
        foreach(string s in qi.WrongAnswers)
        {
            GameObject wrongAnswer = Instantiate(AnswerButtonPrefab);
            wrongAnswer.GetComponent<AnswerButtonUI>().Init(s, false, this);
            answerButtons.Add(wrongAnswer);
        }

        //shuffle
        for (int i = answerButtons.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = answerButtons[i];
            answerButtons[i] = answerButtons[j];
            answerButtons[j] = temp;
        }

        //Add to panel
        foreach(GameObject go in answerButtons)
        {
            go.transform.SetParent(AnswerPanel);
        }
    }

    public void AnswerSelected(bool IsCorrect)
    {
        
        GameManager.Instance.QuestionAnswered(IsCorrect);

        Destroy(gameObject);
    }


}
