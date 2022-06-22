using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RoomChange : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool startPlayer = false;
    [Tooltip("If true this room will be counted in the report as an optional")]
    [SerializeField] private bool optionalRoom = false;

    private void Awake()
    {
        Player p = FindObjectOfType<Player>();
        if(startPlayer)
        {
            p.transform.position = spawnPoint.position;
            p.GetComponent<Collider2D>().enabled = true;
        }
        virtualCam.m_Follow = p.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()) //maybe do it through GetComponent<Player>()
        {
            if (collision.GetComponent<Player>().GetPlayerState() == PLAYER_STATE.DEATH)
            {
                return;
            }

            collision.gameObject.GetComponent<Player>().SetSpawnPoint(spawnPoint.position);
            virtualCam.gameObject.SetActive(true);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.StartCollectible();
            }

            KeyCollectible key = GetComponentInChildren<KeyCollectible>();
            if(key && !key.isOpening)
            {
                key.StartCollectible();
            }

            if (GetComponentInChildren<LevelEater>(true))
            {
                GetComponentInChildren<LevelEater>(true).gameObject.SetActive(true);

            }
            ReportGatherer.Instance.EnterRoom(this);

            foreach(PlatformPerpetualMove p in GetComponentsInChildren<PlatformPerpetualMove>())
            {
                p.StartMoving();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>()) 
        {
            if (collision.GetComponent<Player>().GetPlayerState() == PLAYER_STATE.DEATH)
            {
                return;
            }
            virtualCam.gameObject.SetActive(false);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.EndRoom();
            }
            if (GetComponentInChildren<LevelEater>())
            {
                GetComponentInChildren<LevelEater>().gameObject.SetActive(false);
            }

            foreach (PlatformPerpetualMove p in GetComponentsInChildren<PlatformPerpetualMove>())
            {
                p.StopPlatform();
            }

            KeyCollectible key = GetComponentInChildren<KeyCollectible>(); //TODO improve key way to open door
            if (key && key.taken && !key.isOpening)
            {
                Debug.Log("KeyCouroutineStart");
                StartCoroutine(key.OpenDoor());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Returns optionalRoom</returns>
    public bool GetRoomStatus() => optionalRoom;
}
