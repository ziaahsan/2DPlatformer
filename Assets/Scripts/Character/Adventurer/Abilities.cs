using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX.Character.Adventurer {
    public class Abilities : MonoBehaviour {
        [Header("Layers")]
        public LayerMask WhatIsDamageable;

        [Header("Attack References")]
        public Transform BasicAttack1Hitbox;
        public float BasicAttack1HitboxRadius;
        
        public Transform BasicAttack2Hitbox;
        public float BasicAttack2HitboxRadius;
        
        public Transform BasicAttack3Hitbox;
        public float BasicAttack3HitboxRadius;

        [Header("Stats")]
        public float ChainComboTimer = 0.3f;

        public enum Skill {
            None,
            BasicAttack1, BasicAirAttack1,
            BasicAttack2, BasicAirAttack2,
            BasicAttack3, BasicAirAttack3, BasicAirAttack3End
        };

        [Header("State Checks")]
        public Skill activeSkill;
        public bool isPerformingSkill;
        public bool waitingForChainAttack;
        public int CoroutineCount;

        private AnimationScript animationScript;
        private Movement movement;
        private Collision collision;

        private Transform trash;

        void Awake() {
            activeSkill = Skill.None;

            animationScript = GetComponent<AnimationScript>();
            collision = GetComponentInParent<Collision>();
            movement = GetComponentInParent<Movement>();

            trash = GameObject.Find(Constants.Trash).transform;
        }

        private void Update() {
            if (movement.isWallSliding) {
                isPerformingSkill = false;
                activeSkill = Skill.None;
            }

            if (isPerformingSkill) {
                return;
            }

            PerformBasicAttack1();
            PerformBasicAttack2();
            PerformBasicAttack3();

            PerformBasicAirAttack1();
            PerformBasicAirAttack2();
            PerformBasicAirAttack3();
        }

        /// <summary>
        /// Event Functions
        /// </summary>
        private void BasicAttack1HitBox() {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(BasicAttack1Hitbox.position, BasicAttack1HitboxRadius, WhatIsDamageable);
            foreach(Collider2D collider in detectedObjects) {
                collider.transform.SendMessage("Hurt", 1.0f);
            }
        }

        private void BasicAttack2HitBox() {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(BasicAttack2Hitbox.position, BasicAttack2HitboxRadius, WhatIsDamageable);
            foreach (Collider2D collider in detectedObjects) {
                collider.transform.SendMessage("Hurt", 1.0f);
            }
        }

        private void BasicAttack3HitBox() {
            Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(BasicAttack3Hitbox.position, BasicAttack3HitboxRadius, WhatIsDamageable);
            foreach (Collider2D collider in detectedObjects) {
                collider.transform.SendMessage("Hurt", 1.0f);
            }
        }

        /// <summary>
        /// Basic Attacks
        /// </summary>
        private void PerformBasicAttack1() {
            if (waitingForChainAttack || activeSkill != Skill.None) {
                return;
            }

            if (!Input.GetMouseButton(0) || !collision.onGround) {
                return;
            }

            activeSkill = Skill.BasicAttack1;
            StartCoroutine(PerformSkill());
        }

        private void PerformBasicAttack2() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAttack1) {
                return;
            }

            if (!Input.GetMouseButton(0) || !collision.onGround) {
                return;
            }

            activeSkill = Skill.BasicAttack2;
            StartCoroutine(PerformSkill());
        }

        private void PerformBasicAttack3() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAttack2)
                return;

            if (!Input.GetMouseButton(0) || !collision.onGround)
                return;

            activeSkill = Skill.BasicAttack3;
            StartCoroutine(PerformSkill());
        }

        /// <summary>
        /// Air Attacks
        /// </summary>
        private void PerformBasicAirAttack1() {
            if (waitingForChainAttack || activeSkill != Skill.None) {
                return;
            }

            if (!Input.GetMouseButton(0) || collision.onGround || collision.onWall)
                return;

            activeSkill = Skill.BasicAirAttack1;
            StartCoroutine(PerformSkill());
        }

        private void PerformBasicAirAttack2() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAirAttack1)
                return;

            if (!Input.GetMouseButton(0) || collision.onGround || collision.onWall)
                return;
            
            activeSkill = Skill.BasicAirAttack2;
            StartCoroutine(PerformSkill());
        }

        private void PerformBasicAirAttack3() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAirAttack2)
                return;

            if (!Input.GetMouseButton(0) || collision.onGround || collision.onWall)
                return;

            activeSkill = Skill.BasicAirAttack3;
            StartCoroutine(PerformSkill());
        }

        /// <summary>
        /// Events
        /// </summary>
        private void StopPerformingSkill() {
            isPerformingSkill = false;
        }

        private IEnumerator PerformBasicAirAttack3End() {
            CoroutineCount++;

            while (!collision.onGround) {
                yield return null;
            }

            animationScript.SetTrigger(Skill.BasicAirAttack3End.ToString());
            StartCoroutine(movement.DisableMovement(0.5f));

            CoroutineCount--;
        }


        /// <summary>
        /// Routines
        /// </summary>
        private IEnumerator PerformSkill() {
            CoroutineCount++;

            isPerformingSkill = true;
            waitingForChainAttack = false;

            animationScript.SetTrigger(activeSkill.ToString());

            while(isPerformingSkill) {
                yield return null;
            }

            // Reset Previous trigger & fall multiplier
            animationScript.ResetTrigger();

            if (activeSkill == Skill.BasicAttack1 || activeSkill == Skill.BasicAttack2) {
                StartCoroutine(StartChainComboTimer());
            } else if (activeSkill == Skill.BasicAirAttack1 || activeSkill == Skill.BasicAirAttack2) {
                StartCoroutine(StartChainComboTimer());
            } else {
                activeSkill = Skill.None;
            }

            CoroutineCount--;
        }

        private IEnumerator StartChainComboTimer() {
            CoroutineCount++;

            float time = ChainComboTimer;

            waitingForChainAttack = true;
            while(waitingForChainAttack && time > 0.0f) {
                time -= Time.deltaTime;
                yield return null;
            }

            waitingForChainAttack = false;

            if (!isPerformingSkill) {
                activeSkill = Skill.None;
            }

            CoroutineCount--;
        }

        /// <summary>
        /// Drawing stuff
        /// </summary>
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(BasicAttack1Hitbox.position, BasicAttack1HitboxRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(BasicAttack2Hitbox.position, BasicAttack2HitboxRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(BasicAttack3Hitbox.position, BasicAttack3HitboxRadius);
        }
    }
}