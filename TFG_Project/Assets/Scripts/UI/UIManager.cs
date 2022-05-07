using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject DisconnectedController;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private Button primaryButton;
    [SerializeField] private GameObject postGamePanel;

    private List<Button> menuButtonList = new List<Button>();
    private TestClass questions;
    private GameObject questionsPrefab;
    private GameObject questionButtonPrefab;
    private int questionCounter = 0;
    private void Awake()
    {
        Player.Instance.endGame += PostGameState;

        UserInputManager.Instance.openMenu += func => OpenMenu(); 
        UserInputManager.Instance.closeMenu += OnResumeButtonClicked;
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    if(DisconnectedController != null && !postGamePanel.activeInHierarchy)
                    {
                        HideDeviceDisconnectedScreen();
                    }
                    break;

                case InputDeviceChange.Removed:
                    if (DisconnectedController != null && !postGamePanel.activeInHierarchy)
                    {
                        DisconnectedController.SetActive(true);
                        ShowDeviceDisconenctedScreen();
                    }
                    break;
            }
        };

        questions = JSONManager.ReadJson();
        questionButtonPrefab = Resources.Load<GameObject>("QuestionButton");
        questionsPrefab = Resources.Load<GameObject>("Question");
        menuButtonList = PausePanel.GetComponentsInChildren<Button>().ToList();
    }

    private void ShowDeviceDisconenctedScreen()
    {
        LeanTween.move(DisconnectedController.GetComponent<RectTransform>(), Vector3.zero, 0.1f).setIgnoreTimeScale(true).setOnComplete(
            func => LeanTween.scale(DisconnectedController.GetComponentInChildren<Text>().rectTransform, new Vector3(2f,2f,0f), 0.3f).setIgnoreTimeScale(true).setEaseInBounce());

        Time.timeScale = 0f;
    }

    private void HideDeviceDisconnectedScreen()
    {
        LeanTween.scale(DisconnectedController.GetComponentInChildren<Text>().rectTransform, Vector3.zero, 0.1f).setIgnoreTimeScale(true).setEaseInQuad().setOnComplete(
            func => LeanTween.moveLocalY(DisconnectedController, -1084, 0.05f).setIgnoreTimeScale(true).setOnComplete(
            func => DisconnectedController.SetActive(false)));

        if (!PausePanel.activeInHierarchy || !postGamePanel.activeInHierarchy)
        {
            Time.timeScale = 1f;
        }
    }

    private void OpenMenu()
    {
        if (postGamePanel.activeInHierarchy)
            return;

        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        LeanTween.move(PausePanel.GetComponent<RectTransform>(), Vector3.zero, 0.1f).setIgnoreTimeScale(true).setOnComplete(
            func => LeanTween.move(menuButtonList[0].GetComponent<RectTransform>(),new Vector3(137f,menuButtonList[0].transform.localPosition.y,0), 0.1f).setIgnoreTimeScale(true).setEaseOutCirc().setOnComplete(
                func => LeanTween.move(menuButtonList[1].GetComponent<RectTransform>(), new Vector3(137f, menuButtonList[1].transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setDelay(0.05f).setEaseOutCirc().setOnComplete(
                    func => LeanTween.move(menuButtonList[2].GetComponent<RectTransform>(), new Vector3(137f, menuButtonList[2].transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setDelay(0.05f).setEaseOutCirc()
                    )));
        primaryButton.Select();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), (1.0f / Time.smoothDeltaTime).ToString());
    }

    public void OnResumeButtonClicked()
    {
        if (postGamePanel.activeInHierarchy)
            return;

        UserInputManager.Instance.DisableUiInput();

        LeanTween.move(menuButtonList[2].GetComponent<RectTransform>(), new Vector3(-301f, menuButtonList[2].transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCirc().setOnComplete(
            func => LeanTween.move(menuButtonList[1].GetComponent<RectTransform>(), new Vector3(-301f, menuButtonList[1].transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setDelay(0.05f).setEaseOutCirc().setOnComplete(
                func => LeanTween.move(menuButtonList[0].GetComponent<RectTransform>(), new Vector3(-301f, menuButtonList[0].transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setDelay(0.05f).setEaseOutCirc().setOnComplete(
                   func => LeanTween.moveLocalY(PausePanel, -1084, 0.05f).setIgnoreTimeScale(true).setOnComplete(
                        func =>
                        {
                            PausePanel.SetActive(false);
                            UserInputManager.Instance.DisableUiInput();
                            UserInputManager.Instance.EnablePlayerInput();

                            if (!DisconnectedController.activeInHierarchy)
                            {
                                Time.timeScale = 1;
                            }
                        }))));
    }


    public void PostGameState()
    {
        Time.timeScale = 0f;
        UserInputManager.Instance.DisablePlayerInput();
        UserInputManager.Instance.EnableUiInput();

        PausePanel.SetActive(false);
        DisconnectedController.SetActive(false);
        postGamePanel.SetActive(true);

        LeanTween.move(postGamePanel.GetComponent<RectTransform>(), Vector3.zero, 0.1f).setIgnoreTimeScale(true);
        InputField inputField = postGamePanel.GetComponentInChildren<InputField>();
        inputField.onEndEdit.AddListener(delegate 
        {
            ReportGatherer.Instance.SetReportGatherer(inputField.text);
            LeanTween.move(inputField.GetComponent<RectTransform>(), new Vector3(1000f, inputField.transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCirc().setOnComplete(
                func => { inputField.gameObject.SetActive(false); TestMode();});
        });       
        
    }

    private void TestMode()
    {
        if(questionCounter < questions.questions.Length)
        {
            var currQuestion = Instantiate(questionsPrefab, gameObject.transform);
            currQuestion.GetComponent<Text>().text = questions.questions[questionCounter].question;
            for(int i = 0; i < questions.questions[questionCounter].answers.Length; i++ )
            {
                if(i ==0)
                {
                    currQuestion.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = questions.questions[questionCounter].answers[i];
                    currQuestion.GetComponentInChildren<Button>().Select();
                }
                else
                {
                    var currButton = Instantiate(questionButtonPrefab, currQuestion.transform);
                    currButton.GetComponentInChildren<Text>().text = questions.questions[questionCounter].answers[i];
                    currButton.gameObject.transform.position += new Vector3(0, -70f * i, 0);
                }
            }
            currQuestion.GetComponent<QuestionHandler>().SetDelegates();
            return;
        }
        //Here we make a report gatherer and end the game
        ReportGatherer.Instance.GatherAndSendInfo();
    }

    public void TestButtonCallback(int answer, GameObject go)
    {
        ReportGatherer.Instance.AddAnswerValue(questionCounter, answer);
        questionCounter++;
        Debug.Log(answer);
        LeanTween.moveLocalY(go, -600, 0.3f).setIgnoreTimeScale(true).setOnComplete(func => { Destroy(go); TestMode();});
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnControlsButtonClicked()
    {
        Debug.Log("Controls button pressed");
    }
}
