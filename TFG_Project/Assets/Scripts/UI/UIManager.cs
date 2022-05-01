using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject DisconnectedController;
    private void Awake()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    //gamepad = GetGamepad();
                    if(DisconnectedController != null)
                    {
                        DisconnectedController.SetActive(false);
                    }
                    break;

                case InputDeviceChange.Removed:
                    if (DisconnectedController != null)
                    {
                        DisconnectedController.SetActive(true);
                    }
                    break;
            }
        };
    }

    private void ShowDeviceDisconenctedScreen()
    {

    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), (1.0f / Time.smoothDeltaTime).ToString());
    }
}
