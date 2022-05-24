using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RoomChange : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private bool startPlayer = false;
    private void Awake()
    {
        Player p = FindObjectOfType<Player>();
        if(startPlayer)
        {
            p.transform.position = spawnPoint.position;
        }
        virtualCam.m_Follow = p.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            collision.gameObject.GetComponent<Player>().SetSpawnPoint(spawnPoint.position);
            virtualCam.gameObject.SetActive(true);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.StartCollectible();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            virtualCam.gameObject.SetActive(false);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.EndRoom();
            }
        }
    }
}
