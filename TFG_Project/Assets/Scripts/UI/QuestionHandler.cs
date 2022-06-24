using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class QuestionHandler : MonoBehaviour
{
    public void SetDelegates()
    {
        List<Button> buttonsList = GetComponentsInChildren<Button>().ToList();
        for (int i = 0; i < buttonsList.Count(); i++)
        {
            var textAnswer = buttonsList[i].GetComponentInChildren<Text>().text;
            buttonsList[i].onClick.AddListener(delegate { UnableButtons(); GetComponentInParent<UIManager>().TestButtonCallback(textAnswer, gameObject);});
        }
    }

    private void UnableButtons()
    {
        List<Button> buttonsList = GetComponentsInChildren<Button>().ToList();
        for (int i = 0; i < buttonsList.Count(); i++)
        {
            buttonsList[i].interactable = false;
        }
    }
}
