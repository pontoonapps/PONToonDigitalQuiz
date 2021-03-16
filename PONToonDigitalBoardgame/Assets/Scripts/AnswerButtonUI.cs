using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButtonUI : MonoBehaviour
{
    public Text theText;
    public bool IsCorrect;
    private QuestionPanelUI qpRef;
    public void Init(string answerText, bool IsCorrect, QuestionPanelUI qpRef)
    {
        this.IsCorrect = IsCorrect;
        theText.text = answerText;
        this.qpRef = qpRef;
    }

    public void ButtonClicked()
    {
        qpRef.AnswerSelected(IsCorrect);
    }
}
