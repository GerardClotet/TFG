using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip bounceSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip groundedSFX;
    [SerializeField] AudioClip collectableSFX;
    [SerializeField] AudioClip dieSFX;

    private AudioSource MusicSource;
    private AudioSource playerSource;
    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        MusicSource = GetComponent<AudioSource>();
        playerSource = Player.Instance.GetComponent<AudioSource>();
        MusicSource.Play();
        Player.Instance.dieAction += DieSFX;
        Player.Instance.dashAction += DashSFX;
        Player.Instance.bounceAction += BounceSFX;
        Player.Instance.jumpAction += JumpSFX;
        Player.Instance.groundedAction += GroundedSFX;
        Player.Instance.collectibleGotAction += CollectibleSFX;
        UserInputManager.Instance.openMenu += func => { OnPauseMenu(); };
        UserInputManager.Instance.closeMenu += OnResumeMenu;
    }

    private void OnPauseMenu()
    {
        MusicSource.Pause();
        playerSource.Pause();
    }

    public void OnResumeMenu()
    {
        MusicSource.Play();
        playerSource.Play();
    }

    private void JumpSFX()
    {
        playerSource.clip = jumpSFX;
        playerSource.Play();
    }

    private void BounceSFX()
    {
        playerSource.clip = bounceSFX;
        playerSource.Play();
    }

    private void CollectibleSFX()
    {
        playerSource.clip = collectableSFX;
        playerSource.Play();
    }

    private void GroundedSFX()
    {
        playerSource.clip = groundedSFX;
        playerSource.Play();
    }

    private void DieSFX()
    {
        playerSource.clip = dieSFX;
        playerSource.Play();
    }

    private void DashSFX()
    {
        playerSource.clip = dashSFX;
        playerSource.Play();
    }
}
