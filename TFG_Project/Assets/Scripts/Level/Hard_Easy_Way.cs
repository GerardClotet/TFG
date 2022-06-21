using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hard_Easy_Way : MonoBehaviour
{
    public bool hasEntered { get; private set; } = false;

    [SerializeField] private bool hardWay = false;

    private void Start()
    {
        Player.Instance.dieAction += ResetPlayer;
    }
    private void OnDestroy()
    {
        Player.Instance.dieAction -= ResetPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() && !hasEntered)
        {
            hasEntered = true;
            //ReportGatherer.Instance.
            if (hardWay)
            {
                ReportGatherer.Instance.TryHardWay();
            }
            else
            {
                ReportGatherer.Instance.TryEasyWay();
            }
        }
    }

    public void ResetPlayer()
    {
        hasEntered = false;
    }

}
