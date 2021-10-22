using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    public AudioClip jump;

    private AudioSource audioPlayer;

    public AudioClip scoreHighlight;

    private void Start() {
        audioPlayer = GetComponent<AudioSource>();
    }

    public void PlayScoreHighlight() {
        audioPlayer.PlayOneShot(scoreHighlight);
    }

    public void PlayJump() {
        audioPlayer.PlayOneShot(jump);
    }
}
