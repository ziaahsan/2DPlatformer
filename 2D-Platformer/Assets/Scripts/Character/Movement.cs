using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class Movement : MonoBehaviour {

        [Header("Stats")]
        public float MoveSpeed = 4.0f;
        public float SlideSpeed = 2.0f;
        public float JumpForce = 12.0f;
        public float FallMultiplier = 4.0f;
        public float LowJumpMultiplier = 3.0f;

        [Header("Particles Effects")]
        public ParticleSystem GrassDust;

        private float inputX, inputRawX, inputY, inputRawY;

        private Collision collision;

        private AnimationScript animationScript;
        private new Rigidbody2D rigidbody2D;

        [Header("State Checks")]
        public bool canMove;
        public bool isJumping;
        public bool isWallJumping;
        public bool isFalling;
        public bool isWallSliding;
        public bool wallGrab;
        public int side = -1;

        private void Awake() {
            canMove = true;

            collision = GetComponent<Collision>();
            animationScript = GetComponent<AnimationScript>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update() {
            CheckUserInput();
            // Reset the animator @StateID's before we do anything with it
            animationScript.SetStateID(-1);

            // Time.timeScale = 0.6f;

            // See if any of the following can be performed
            Run();
            WallSlide();

            if (Input.GetButtonDown("Jump")) {
                if (collision.onGround && !collision.onWall)
                    Jump(Vector2.up);
                else if (isWallSliding)
                    WallJump();
            }

            JumpModifier();
            Fall();

            WallGrab();
            Particles();
        }

        private void CheckUserInput() {
            inputX = Input.GetAxis("Horizontal");
            inputRawX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxis("Vertical");
            inputRawY = Input.GetAxisRaw("Vertical");

            if (!canMove || isWallSliding || wallGrab)
                return;

            if (inputRawX > 0) {
                side = 1;
                animationScript.Flip(side);
            } else if (inputRawX < 0) {
                side = -1;
                animationScript.Flip(side);
            }
        }

        private void Run() {
            if (!canMove || wallGrab)
                return;

            float velocity = inputRawX * MoveSpeed;

            if (collision.onGround && !collision.onWall)
                animationScript.SetStateID(velocity != 0 ? 1 : 0);

            if (!isWallJumping)
                rigidbody2D.velocity = new Vector2(velocity, rigidbody2D.velocity.y);
            else
                rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, (new Vector2(velocity, rigidbody2D.velocity.y)), 10 * Time.deltaTime);
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
            isWallJumping = true;

            StopCoroutine(DisableMovement(0));
            StartCoroutine(DisableMovement(.1f));

            Vector2 wallDirection = collision.onRightWall ? Vector2.left : Vector2.right;
            Vector2 direction = Vector2.up / 1.5f + wallDirection / 1.5f;

            Jump(direction);
        }

        private void WallSlide() {
            if (!canMove)
                return;

            if (!collision.onWall || collision.onGround) {
                isWallSliding = false;
                return;
            }

            bool pushingWall = false;
            if ((rigidbody2D.velocity.x > 0 && collision.onRightWall) || (rigidbody2D.velocity.x < 0 && collision.onLeftWall)) {
                pushingWall = true;
            }

            float push = pushingWall ? 0 : rigidbody2D.velocity.x;

            rigidbody2D.velocity = new Vector2(push, -SlideSpeed);

            if (isFalling)
                isWallSliding = true;
        }

        private void WallGrab() {
            
        }

        private void Fall() {
            if (!isWallSliding && !collision.onGround && rigidbody2D.velocity.y < 0)
                isFalling = true;
            else
                isFalling = false;
        }

        private void Particles() {
            bool play = false;

            // On ground switching sides or about to move
            if (collision.onGround && Input.GetButtonDown("Horizontal"))
                play = true;

            // On ground about to jump
            else if (collision.onGround && rigidbody2D.velocity.y > 0)
                play = true;

            // On wall about to leave it
            else if (collision.onWall && rigidbody2D.velocity.x != 0)
                play = true;

            if (play)
                GrassDust.Play();
        }

        IEnumerator DisableMovement(float time) {
            canMove = false;
            yield return new WaitForSeconds(time);
            canMove = true;
        }
    }
}