using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProjectX.AI.Simple {
    public class Brain : MonoBehaviour {
        [Header("Layers")]
        public LayerMask WhatIsDamageable;

        [Header("Fonts")]
        public TextMesh CustomText;

        [Header("Status")]
        public int Level = 1;
        public int ExpAmount = 35;
        public float CurrentHealth = 10.0f;
        public float MoveSpeed = 1.0f;
        public float Stamina = 1.0f;
        public float Damage = 1.0f;
        public float SpawnTime = 3.0f;
        public float VisionAngleAbove = 0.0f;
        public float VisionAngleBelow = 0.0f;
        public float VisionDistance = 5.0f;

        [Header("Attack References")]
        public Transform BasicAttack1Hitbox;
        public float BasicAttack1HitboxRadius;

        [Header("States")]
        public State activeState;
        public int side = -1;
        public int coroutineCount;

        public enum State { Idle, Resting, Move, Hurt, BasicAttack1, Dead };

        private GameObject player;
        private Collider2D playerCollider;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private new Rigidbody2D rigidbody2D;
        private new Collider2D collider2D;

        private Collision collision;

        private RaycastHit2D straightView;
        private RaycastHit2D topView;
        private RaycastHit2D downView;

        private Transform trash;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<Collider2D>();

            collision = GetComponent<Collision>();
        }

        private void Start() {
            activeState = State.Idle;
            trash = GameObject.Find(Constants.Trash).transform;

            // Face the firection of side
            FaceToSideDirection();

            // Setup font
            CustomText.characterSize = 0.5f;
        }

        private void Update() {
            DetermineState();
            
            BasicAttack1();
            Wandner();

            PlayStateAnimation();
        }

        /// <summary>
        /// Event Functions
        /// </summary>
        private void CurrentAnimationComplete() {
            activeState = State.Idle;
        }

        private void BasicAttack1HitBox() {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(BasicAttack1Hitbox.position, BasicAttack1HitboxRadius, WhatIsDamageable);
            foreach (Collider2D collider in detectedObjects) {
                collider.transform.SendMessage("Hurt", Damage);
            }
        }

        private void Hurt(float amount) {
            if (activeState == State.Idle || activeState == State.Resting) {
                activeState = State.Hurt;
            }
            
            CustomText.text = amount.ToString();
            
            TextMesh dmgText = Instantiate(CustomText, transform.position, Quaternion.identity, trash);
            dmgText.transform.DOMoveY(transform.position.y + Random.Range(0.5f, 0.7f), 0.3f, false);
            Destroy(dmgText.gameObject, 0.7f);

            CurrentHealth -= amount;
        }

        private void Wandner() {
            if (!collision.onGround || activeState != State.Move) {
                return;
            }

            if (Stamina < 0.01f) {
                StartCoroutine(Rest(Random.Range(0.3f, 0.8f)));
                return;
            }

            rigidbody2D.velocity = new Vector2(MoveSpeed * side, rigidbody2D.velocity.y);
            Stamina -= 0.2f * Time.deltaTime;
        }

        private void BasicAttack1() {
            if (!collision.onGround || activeState != State.BasicAttack1) {
                return;
            }

            Stamina -= 0.5f * Time.deltaTime;
        }

        private void VisionCast() {
            Vector2 origin = (side == 1) ? collision.RightArmOffset.position : collision.LeftArmOffset.position;

            // Look straight ahead ray
            straightView = Physics2D.Raycast(origin, side * transform.right, VisionDistance);

            // Look up ray
            Vector2 upDirection = new Vector2(side * transform.right.x, transform.right.y + VisionAngleAbove);
            topView = Physics2D.Raycast(origin, upDirection, VisionDistance);

            // Look down ray
            Vector2 downDirection = new Vector2(side * transform.right.x, transform.right.y - VisionAngleBelow);
            downView = Physics2D.Raycast(origin, downDirection, VisionDistance);

            // Draw the above rays
            Debug.DrawRay(origin, upDirection * VisionDistance, Color.red);
            Debug.DrawRay(origin, side * transform.right * VisionDistance, Color.green);
            Debug.DrawRay(origin, downDirection * VisionDistance, Color.blue);
        }

        private void DetermineState() {
            VisionCast();

            // Dead
            if (CurrentHealth < 0.01f) {
                activeState = State.Dead;
                return;
            }

            if (activeState == State.Resting || activeState == State.Hurt || activeState == State.Dead) {
                return;
            }

            // Resting
            if (Stamina < 0.01f) {
                activeState = State.Resting;
                StartCoroutine(Rest(Random.Range(0.3f, 0.8f)));
            } else if (straightView.collider != null) {
                // Attack
                if (straightView.collider.tag.Equals(Constants.Tag.Player.ToString())) {
                    activeState = State.BasicAttack1;
                }
            } else {
                // Roam
                activeState = State.Move;
            }
        }

        private void PlayStateAnimation() {
            switch(activeState) {
                case State.Idle:
                case State.Resting:
                    animator.SetBool("IsMoving", false);
                    break;
                case State.Move:
                    animator.SetBool("IsMoving", true);
                    break;
                case State.Hurt:
                    animator.SetTrigger("Hurt");
                    break;
                case State.Dead:
                    animator.SetTrigger("Die");
                    break;
                case State.BasicAttack1:
                    animator.SetTrigger("BasicAttack1");
                    break;
            }
        }

        private void FaceToSideDirection() {
            // -1 = facing left, 1 = facing right
            bool state = (side == 1) ? false : true;
            spriteRenderer.flipX = state;

            // Flip the object attack hit boxes
            transform.Find(Constants.AttakHitBoxes).transform.eulerAngles = state ? new Vector2(0, 180) : new Vector2(0, 0);
        }

        private IEnumerator Rest(float time) {
            coroutineCount++;

            while (time > 0.0f) {
                if (Stamina < 1.0f) {
                    Stamina += 0.15f * Time.deltaTime;
                }
                time -= 0.1f * Time.deltaTime;
                yield return null;
            }

            activeState = State.Idle;
            coroutineCount--;
        }

        /// <summary>
        /// Drawing stuff
        /// </summary>
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(BasicAttack1Hitbox.position, BasicAttack1HitboxRadius);
        }
    }
}