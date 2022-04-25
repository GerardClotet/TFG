using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum RUMBLE_PATTERN
{
    CONSTANT,
    PULSE,
    LINEAR
}
public class Rumbler : MonoBehaviour
{
    [SerializeField] private float lowF = 0f;
    [SerializeField] private float highF = 0f;

    private void Awake()
    {
        
       
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    //gamepad = GetGamepad();
                    break;

                case InputDeviceChange.Removed:
                    //Debug.Log("Removed " + device);
                    break;
            }
        };
        UserInputManager.Instance.dashEvent += StartDashRumble;
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.current;
    }

    void StartDashRumble()
    {
        Gamepad gamepad = GetGamepad();
        StartCoroutine(DashRumble());
    }

    IEnumerator DashRumble()
    {
        float time = 0.5f;
        float lowA = 50f;
        float highA = 1000f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            var gamepad = GetGamepad();
            gamepad.SetMotorSpeeds(lowF, highF);
            lowA += 5 * Time.deltaTime;
            lowA += 10 * Time.deltaTime;

            yield return null;
        }
        StopRumble();
        yield return null;
    }

    private void StopRumble()
    {
        Gamepad gamepad = GetGamepad();
        if(gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);

        }
    }
}
