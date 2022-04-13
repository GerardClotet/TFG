using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RoomChange : MonoBehaviour
{

    [SerializeField]
    private CinemachineVirtualCamera virtualCam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            virtualCam.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            virtualCam.gameObject.SetActive(false);
        }
    }
}
