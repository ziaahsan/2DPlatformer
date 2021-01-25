using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX.Character.Adventurer {
    public class AnimationScript : MonoBehaviour {

        [HideInInspector]
        public SpriteRenderer spriteRenderer;

        private Animator animator;
        
        private Movement movement;
        private Collision collision;
        private Abilities abilities;

        private string previousTrigger;

        void Awake() {
            animator = GetComponent<Animator>();
            movement = GetComponentInParent<Movement>();
            collision = GetComponentInParent<Collision>();
            abilities = GetComponentInParent<Abilities>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            animator.SetBool("IsJumping", (movement.isJumping || movement.isWallJumping) && !abilities.isPerformingSkill);
            animator.SetBool("IsFalling", movement.isFalling && !abilities.isPerformingSkill);
            animator.SetBool("IsWallSliding", movement.isWallSliding);
            animator.SetBool("IsSliding", movement.isSliding);
        }

        public void SetStateID(int id) {
            if (abilities.isPerformingSkill) {
                animator.SetInteger("StateID", -1);
            } else {
                animator.SetInteger("StateID", id);
            }
        }

        public void SetTrigger(string trigger) {
            animator.SetTrigger(trigger);
            previousTrigger = trigger;
        }

        public void ResetTrigger() {
            if (previousTrigger == null)
                return;

            animator.ResetTrigger(previousTrigger);
        }

        public void ResetTrigger(string trigger) {
            animator.ResetTrigger(trigger);
        }

        public float GetAnimationClipLength(int clipIndex) {
            return animator.runtimeAnimatorController.animationClips[clipIndex].length;
        }

        public void Flip(int side) {
            // -1 = facing left, 1 = facing right
            bool state = (side == 1) ? false : true;
            spriteRenderer.flipX = state;

            // Flip the object attack hit boxes
            transform.Find(Constants.AttakHitBoxes).transform.eulerAngles = state ? new Vector2(0, 180) : new Vector2(0, 0);
        }
    }
}