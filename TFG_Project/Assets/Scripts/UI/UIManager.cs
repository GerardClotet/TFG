using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject DisconnectedController;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private Button primaryButton;
    [SerializeField] private GameObject postGamePanel;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private GameObject collectiblesCounter;

    private List<Button> menuButtonList = new List<Button>();
    private TestClass questions;
    private GameObject questionsPrefab;
    private GameObject questionButtonPrefab;
    private int questionCounter = 0;
    private int currentCollectibles = 0;
    private int totalCollectibles = 0;
    private void Awake()
    {
        Player.Instance.endGame += PostGameState;
        UserInputManager.Instance.openMenu += context => OpenMenu();
        UserInputManager.Instance.closeMenu += OnResumeButtonClicked;
        SceneManager.activeSceneChanged += NewLevelLoaded;

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

        if(Player.Instance.GetPlayerState() == PLAYER_STATE.DEATH)
        {
            UserInputManager.Instance.DisableUiInput();
            UserInputManager.Instance.EnablePlayerInput();
            return;
        }

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

        AudioManager.Instance.OnResumeMenu();
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
        Player.Instance.GetComponent<Collider2D>().enabled = false;
        UserInputManager.Instance.DisablePlayerInput();
        UserInputManager.Instance.EnableUiInput();

        PausePanel.SetActive(false);
        DisconnectedController.SetActive(false);
        postGamePanel.SetActive(true);

        LeanTween.move(postGamePanel.GetComponent<RectTransform>(), Vector3.zero, 0.1f).setIgnoreTimeScale(true);
        InputField inputField = postGamePanel.GetComponentInChildren<InputField>();

        questions = JSONManager.ReadTests();
        ReportGatherer.Instance.SetQuestions(questions.questions.Length);

        if (inputField)
        {
            inputField.onEndEdit.AddListener(delegate
            {
                ReportGatherer.Instance.SetReportGathererUserName(inputField.text);
                LeanTween.move(inputField.GetComponent<RectTransform>(), new Vector3(1000f, inputField.transform.localPosition.y, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCirc().setOnComplete(
                    func => { inputField.gameObject.SetActive(false); CreateQuestionAnswer(); });
            });
        }
        else //Meaning it's another scene and we have finished it.
        {
            questionCounter = 0;
            CreateQuestionAnswer();
        }
        
    }

    private void CreateQuestionAnswer()
    {
        if (questionCounter < questions.questions.Length)
        {
            int achiever = ReportGatherer.Instance.Achiever ? 1 : 0;
            if (questions.questions[questionCounter].achiever == -1 || questions.questions[questionCounter].achiever == achiever)
            {
                var currQuestion = Instantiate(questionsPrefab, gameObject.transform);
                currQuestion.GetComponent<Text>().text = questions.questions[questionCounter].question;
                for (int i = 0; i < questions.questions[questionCounter].answers.Length; i++)
                {
                    if (i == 0)
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
        }
        FadeFromToBlack(1, true);
        return;
    }

    public void FadeFromToBlack(float time = 1, bool endGame = false)
    {
        Image img = fadePanel.GetComponent<Image>();
        LeanTween.value(gameObject, 0, 1, time).setIgnoreTimeScale(true).setOnUpdate((float val) =>
        {
            Color c = img.color;
            c.a = val;
            img.color = c;
        }).setOnComplete(
            func =>
            {
                if (endGame)
                {
                    GameManager.Instance.EndScene();
                }
                LeanTween.value(gameObject, 1, 0, time).setIgnoreTimeScale(true).setOnUpdate((float val) =>
                {
                    Color c = img.color;
                    c.a = val;
                    img.color = c;
                });
            });
    }

    public void TestButtonCallback(string answer, GameObject go)
    {
        ReportGatherer.Instance.AddQuestionAnswer(questionCounter,go.GetComponent<Text>().text, answer);
        questionCounter++;
        LeanTween.moveLocalY(go, -600, 0.3f).setIgnoreTimeScale(true).setOnComplete(func => { Destroy(go); CreateQuestionAnswer();});
    }

    public void CollectibleGot()
    {
        currentCollectibles += 1;
        collectiblesCounter.GetComponentInChildren<Text>().text = $"{currentCollectibles}/{totalCollectibles}";
    }

    private void NewLevelLoaded(Scene current, Scene next)
    {
        Debug.Log($"Changed from {current.name} to {next.name}");

        RoomChange[] rooms = FindObjectsOfType<RoomChange>();
        totalCollectibles = 0;
        currentCollectibles = 0;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].GetComponentInChildren<MultiplierCollectible>())
            {
                totalCollectibles += 1;
            }
        }
        collectiblesCounter.GetComponentInChildren<Text>().text = $"0/{totalCollectibles}";
        Time.timeScale = 1f;
        UserInputManager.Instance.DisableUiInput();
        UserInputManager.Instance.EnablePlayerInput();
        postGamePanel.SetActive(false);
    }

    public void UpdateCollectiblesList()
    {
        var obj = FindObjectsOfType<MultiplierCollectible>();
        totalCollectibles = obj.Length;
        collectiblesCounter.GetComponentInChildren<Text>().text = $"0/{totalCollectibles}";
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnControlsButtonClicked()
    {
        //Debug.Log("Controls button pressed");
        //GameManager.Instance.EndScene();//TODO QUIT
    }
}
