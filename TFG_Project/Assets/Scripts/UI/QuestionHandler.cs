using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class QuestionHandler : MonoBehaviour
{
    int answerCounter = 0;
    public void SetDelegates()
    {
        List<Button> buttonsList = GetComponentsInChildren<Button>().ToList();
        for (int i = 0; i < buttonsList.Count(); i++)
        {
            int s = answerCounter;
            buttonsList[i].onClick.AddListener(delegate { GetComponentInParent<UIManager>().TestButtonCallback(s, gameObject); });
            answerCounter++;
        }
    }
}
