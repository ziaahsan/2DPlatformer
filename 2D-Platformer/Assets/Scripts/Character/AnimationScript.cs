﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class AnimationScript : MonoBehaviour {

        [HideInInspector]
        public SpriteRenderer spriteRenderer;

        private Animator animator;
        private Movement movement;
        private Collision collision;

        private string previousTrigger;

        void Awake() {
            animator = GetComponent<Animator>();
            collision = GetComponentInParent<Collision>();
            movement = GetComponentInParent<Movement>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            animator.SetBool("IsJumping", movement.isJumping || (movement.isWallJumping && !movement.isWallSliding));
            animator.SetBool("IsFalling", movement.isFalling);
            animator.SetBool("IsWallSliding", movement.isWallSliding);
        }

        public void SetStateID(int id) {
            animator.SetInteger("StateID", id);
        }

        public void SetTrigger(string trigger) {
            animator.SetTrigger(trigger);
            previousTrigger = trigger;
        }

        public void ResetTrigger() {
            if (previousTrigger != null)
                return;
            animator.ResetTrigger(previousTrigger);
        }

        public void ResetTrigger(string trigger) {
            animator.ResetTrigger(trigger);
        }

        public void Flip(int side) {
            bool state = (side == 1) ? false : true;
            spriteRenderer.flipX = state;
        }
    }
}