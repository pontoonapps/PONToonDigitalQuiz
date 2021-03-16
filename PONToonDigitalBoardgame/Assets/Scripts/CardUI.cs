using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    private QuestionInfo qi;

    public GameObject QuestionDisplayPrefab;

    public void Init(QuestionInfo qi, string deckName)
    {
        this.qi = qi;
        transform.GetChild(0).GetComponent<Text>().text = deckName;
    }

    public void DisplayQuestion()
    {
        GameObject go = Instantiate(QuestionDisplayPrefab);
        go.GetComponent<QuestionPanelUI>().Init(qi);
        go.transform.SetParent(transform.parent.transform.parent.transform.parent, false);

        GameManager.Instance.CardWasPressed(go);

        Destroy(gameObject);
    }
}
