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

    private List<Button> menuButtonList = new List<Button>();

    private void Awake()
    {
        UserInputManager.Instance.openMenu += func => OpenMenu(); 
        UserInputManager.Instance.closeMenu += OnResumeButtonClicked;

        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    //gamepad = GetGamepad();
                    if(DisconnectedController != null)
                    {
                        //DisconnectedController.SetActive(false);
                        HideDeviceDisconnectedScreen();
                    }
                    break;

                case InputDeviceChange.Removed:
                    if (DisconnectedController != null)
                    {
                        DisconnectedController.SetActive(true);
                        ShowDeviceDisconenctedScreen();
                    }
                    break;
            }
        };

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

        if (!PausePanel.activeInHierarchy)
        {
            Time.timeScale = 1f;
        }
    }

    private void OpenMenu()
    {
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

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnControlsButtonClicked()
    {
        Debug.Log("Controls button pressed");
    }
}
