using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class Abilities : MonoBehaviour {

        [Header("Stats")]
        public float ChainComboTimer = 0.3f;

        public enum Skill { None, BasicAttack1, BasicAirAttack1, BasicAttack2, BasicAttack3 };

        [Header("State Checks")]
        public Skill activeSkill;
        public bool isPerformingSkill;
        public bool waitingForChainAttack;
        public int CoroutineCount;

        private AnimationScript animationScript;
        private Movement movement;
        private Collision collision;

        void Awake() {
            activeSkill = Skill.None;

            animationScript = GetComponent<AnimationScript>();
            collision = GetComponentInParent<Collision>();
            movement = GetComponentInParent<Movement>();
        }

        private void Update() {
            if (!isPerformingSkill && !waitingForChainAttack)
                activeSkill = Skill.None;

            if (!isPerformingSkill)
                PerformBasicAttack1();
            
            if (!isPerformingSkill)
                PerformBasicAttack2();

            if (!isPerformingSkill)
                PerformBasicAttack3();

            if (!isPerformingSkill)
                PerformBasicAirAttack1();
        }

        private void PerformBasicAttack1() {
            if (waitingForChainAttack && activeSkill != Skill.BasicAttack3)
                return;

            if (!Input.GetMouseButton(0) || !collision.onGround || collision.onWall) 
                return;

            isPerformingSkill = true;

            float clipLength = animationScript.GetAnimationClipLength(5);
            StartCoroutine(PerformSkill(Skill.BasicAttack1, clipLength));
        }

        private void PerformBasicAttack2() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAttack1)
                return;
            
            if (!Input.GetMouseButton(0) || !collision.onGround || collision.onWall)
                return;

            isPerformingSkill = true;
            
            float clipLength = animationScript.GetAnimationClipLength(6);
            StartCoroutine(PerformSkill(Skill.BasicAttack2, clipLength));
        }

        private void PerformBasicAttack3() {
            if (!waitingForChainAttack || activeSkill != Skill.BasicAttack2)
                return;

            if (!Input.GetMouseButton(0) || !collision.onGround || collision.onWall)
                return;

            isPerformingSkill = true;

            float clipLength = animationScript.GetAnimationClipLength(6);
            StartCoroutine(PerformSkill(Skill.BasicAttack3, clipLength));
        }

        private void PerformBasicAirAttack1() {
            if (!Input.GetMouseButtonDown(0) || collision.onGround || collision.onWall)
                return;

            isPerformingSkill = true;
            movement.isJumping = false;

            float clipLength = animationScript.GetAnimationClipLength(7);
            StartCoroutine(PerformSkill(Skill.BasicAirAttack1, clipLength));
        }

        private IEnumerator PerformSkill(Skill skill, float time) {
            CoroutineCount++;
            activeSkill = skill;
            animationScript.SetTrigger(skill.ToString());

            yield return new WaitForSeconds(time);
            
            isPerformingSkill = false;

            if (skill == Skill.BasicAttack1 || skill == Skill.BasicAttack2) {
                StartCoroutine(StartChainComboTimer(ChainComboTimer));
            } else {
                activeSkill = Skill.None;
            }

            CoroutineCount--;
        }

        private IEnumerator StartChainComboTimer(float time) {
            CoroutineCount++;
            waitingForChainAttack = true;
            
            while(!isPerformingSkill && time > 0.0f) {
                time -= Time.deltaTime;
                yield return null;
            }

            waitingForChainAttack = false;
            CoroutineCount--;
        }
    }
}