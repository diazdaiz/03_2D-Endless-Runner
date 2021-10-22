using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    private bool isJumping;
    private bool isOnGround;

    private Rigidbody2D rig;

    private Animator anim;

    private CharacterSoundController sound;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;


    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    [Header("Night")]
    public Animator nightUI;
    private string currentState;

    private void Start() {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }

    private void FixedUpdate() {
        Vector2 velocityVector = rig.velocity;
        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;

        // raycast ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit) {
            if (!isOnGround && rig.velocity.y <= 0) {
                isOnGround = true;
            }
        }
        else {
            isOnGround = false;
        }

        if (isJumping) {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void Update() {
        // read input
        if (Input.GetMouseButtonDown(0)) {
            if (isOnGround) {
                isJumping = true;
                sound.PlayJump();
            }
        }

        // change animation
        anim.SetBool("isOnGround", isOnGround);

        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0) {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        if ((score.GetCurrentScore() + 20) % 40 == 0) {
            Debug.Log("hmm");
            TurnNight();
        }
        else if ((score.GetCurrentScore()) % 40 == 0 && score.GetCurrentScore()>35) {
            TurnDay();
        }

        void ChangeDayOrNight(string dayOrNight) {
            if (currentState == dayOrNight) return;
            nightUI.Play(dayOrNight);
            currentState = dayOrNight;
        }

        void TurnNight() {
            ChangeDayOrNight("Night");
        }

        void TurnDay() {
            ChangeDayOrNight("Day");
        }

        // game over
        if (transform.position.y < fallPositionY) {
            GameOver();
        }
    }

    private void GameOver() {
        // set high score
        score.FinishScoring();

        // stop camera movement
        gameCamera.enabled = false;

        // show gameover
        gameOverScreen.SetActive(true);

        // disable this too
        this.enabled = false;
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}
