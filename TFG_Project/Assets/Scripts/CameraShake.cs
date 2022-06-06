using System.Collections;
using UnityEngine;
using Cinemachine;
public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBrain cBrain;

    [SerializeField] private Player player;

    [SerializeField] private float dashIntensity = 5f;
    [SerializeField] private float dashTime = 1f;

    private void Awake()
    {
        cBrain = GetComponent<CinemachineBrain>();
        player.dashAction += DashShake;
    }

    private void DashShake()
    {
        ShakeCamera(dashIntensity, dashTime);
    }

    private void ShakeCamera(float intensity,float time)
    {
        virtualCamera = cBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        StartCoroutine(ShakeCoroutine(time));
    }

    IEnumerator ShakeCoroutine(float time)
    {
        while(time >0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (cinemachineBasicMultiChannelPerlin != null)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        }
    }

    private void OnDestroy()
    {
        if(player != null)
        {
            player.dashAction -= DashShake;
        }
    }
}
