using System;
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
    private void Awake()
    {
        Player.Instance.dashAction += StartDashRumble;
        Player.Instance.groundedAction += GroundRumble;
        Player.Instance.endGame += StopRumble;
        Player.Instance.endGame += StopAllCoroutines;
        UserInputManager.Instance.openMenu += context => {StopRumble();};
        Application.quitting += StopRumble;
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.current;
    }

    void GroundRumble()
    {
        StartCoroutine(ConstantRumbleOverTime(0.7f, 1.0f,0.1f));
    }

    void StartDashRumble()
    {
        StartCoroutine(LinealRumbleOverTime(1f,0.5f,1f,0.5f,0.4f));
    }

    IEnumerator ConstantRumbleOverTime(float lowF, float highF, float time)
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowF, highF);
        }

        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        StopRumble();
    }

    IEnumerator LinealRumbleOverTime(float lowF_Initial, float lowF_Final, float highF_Initial, float highF_Final, float time)
    {
        var gamepad = GetGamepad();
        float currentLowF = lowF_Initial;
        float currentHighF = highF_Initial;
        while(time >0)
        {
            gamepad.SetMotorSpeeds(currentLowF, currentHighF);
            currentLowF = Mathf.Lerp(lowF_Initial, lowF_Final, time);
            currentHighF = Mathf.Lerp(highF_Initial, highF_Final, time);
            time -= Time.deltaTime;
            yield return null;
        }
        StopRumble();
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
