using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace ProjectX.Character.Adventurer {
    public class Movement : MonoBehaviour {

        [Header("Stats")]
        public float MoveSpeed = 4.0f;
        public float SlideSpeed = 2.0f;
        public float JumpForce = 12.0f;
        public float FallMultiplier = 4.0f;
        public float LowJumpMultiplier = 3.0f;

        [Header("Particles Effects")]
        public ParticleSystem FeetParticles;

        private float inputX, inputRawX, inputY, inputRawY;

        private Collision collision;
        private AnimationScript animationScript;
        private Abilities abilities;

        private new Rigidbody2D rigidbody2D;

        [Header("State Checks")]
        public bool canMove;
        public bool isJumping;
        public bool isWallJumping;
        public bool isWallSliding;
        public bool isSliding;
        public bool isFalling;

        public int side = -1;

        private void Awake() {
            canMove = true;

            collision = GetComponent<Collision>();
            animationScript = GetComponent<AnimationScript>();
            abilities = GetComponent<Abilities>();

            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start() {
            // Flip the side
            animationScript.Flip(side);
        }
        // Update is called once per frame
        private void Update() {
            // Reset the animator @StateID's before we do anything with it
            animationScript.SetStateID(-1);
            
            ResetAllStates();
            
            CheckUserInput();
            // Time.timeScale = 0.7f;

            Run();
            WallSlide();

            if (Input.GetButtonDown("Jump")) {
                if (collision.onGround) {
                    Jump(Vector2.up);
                } else if (isWallSliding) {
                    WallJump();
                }
            }

            JumpModifier();
            Slide();
            Fall();

            Particles();
        }

        private void CheckUserInput() {
            inputX = Input.GetAxis("Horizontal");
            inputRawX = Input.GetAxisRaw("Horizontal");

            inputY = Input.GetAxis("Vertical");
            inputRawY = Input.GetAxisRaw("Vertical");

            if (!canMove || isWallSliding) {
                return;
            }

            if (inputRawX > 0) {
                side = 1;
                animationScript.Flip(side);
            } else if (inputRawX < 0) {
                side = -1;
                animationScript.Flip(side);
            }
        }

        private void Run() {
            if (!canMove) {
                return;
            }

            float velocity = inputRawX * MoveSpeed;
            animationScript.SetStateID(velocity != 0 ? 1 : 0);

            if (isWallJumping) {
                rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, (new Vector2(velocity, rigidbody2D.velocity.y)), 10 * Time.deltaTime);
            } else {
                rigidbody2D.velocity = new Vector2(velocity, rigidbody2D.velocity.y);
            }
        }

        private void JumpModifier() {
            if (rigidbody2D.velocity.y < 0) {
                rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime;
            } else if (rigidbody2D.velocity.y > 0 && !Input.GetButton("Jump")) {
                rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
            }

            if (isFalling) {
                isJumping = false;
                isWallJumping = false;
            }
        }

        private void Jump(Vector2 direction) {
            isJumping = true;

            if (isWallJumping)
                isJumping = false;

            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            rigidbody2D.velocity += direction * JumpForce;
        }

        private void WallJump() {
            isWallSliding = false;
            isWallJumping = true;

            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            Vector2 wallDirection = collision.onRightWall ? Vector2.left : Vector2.right;
            Vector2 direction = Vector2.up / 1.5f + wallDirection / 1.5f;

            Jump(direction);
        }

        private void WallSlide() {
            if (!canMove || isJumping)
                return;

            if (!collision.onWall || collision.onGround) {
                isWallSliding = false;
                return;
            }

            if (collision.wallSide != side) {
                side *= -1;
                animationScript.Flip(side);
            }

            bool pushingWall = false;
            
            if ((rigidbody2D.velocity.x > 0 && collision.onRightWall) || (rigidbody2D.velocity.x < 0 && collision.onLeftWall)) {
                pushingWall = true;
            }

            float push = pushingWall ? 0 : rigidbody2D.velocity.x;
            rigidbody2D.velocity = new Vector2(push, -SlideSpeed);

            if (isFalling) {
                isWallSliding = true;
            }
        }

        private void Slide() {
        
        }

        private void Fall() {
            if ((!isWallSliding && !collision.onGround && rigidbody2D.velocity.y < 0)) {
                isFalling = true;
            } else {
                if (isFalling) {
                    FeetParticles.Play();
                }
                isFalling = false;
            }
        }

        private void Hurt(float amount) {
            Debug.Log("Something hurt me " + amount);
        }

        private void Particles() {
            bool play = false;

            if (collision.onGround && Input.GetButtonDown("Horizontal")) {
                play = true;
            } else if (collision.onGround && rigidbody2D.velocity.y > 0) {
                play = true;
            } else if (collision.onWall && rigidbody2D.velocity.x != 0) {
                play = true;
            }

            if (play) {
                FeetParticles.Play();
            }
        }

        private void ResetAllStates() {
            if (rigidbody2D.velocity == Vector2.zero) {
                isJumping = false;
                isWallJumping = false;
                isWallSliding = false;
                isSliding = false;
                isFalling = false;
            }
        }

        public void RigidbodyDrag(float x) {
            rigidbody2D.drag = x;
        }

        public IEnumerator DisableMovement(float time) {
            canMove = false;
            rigidbody2D.velocity = Vector2.zero;

            yield return new WaitForSeconds(time);
            canMove = true;
        }
    }
}