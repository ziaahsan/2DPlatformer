using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class Abilities : MonoBehaviour {

        [Header("Stats")]
        public float DropComboTime = 0.1f;

        private AnimationScript animationScript;
        private Movement movement;
        private Collision collision;

        public enum Skill {None, BasicAttack1, BasicAttack2};
        
        [Header("State Checks")]
        public Skill activeSkill;
        public bool isPerformingSkill;
        public bool isChainAttack;

        void Awake() {
            activeSkill = Skill.None;

            animationScript = GetComponent<AnimationScript>();
            collision = GetComponentInParent<Collision>();
            movement = GetComponentInParent<Movement>();
        }

        private void Update() {
            if (isPerformingSkill)
                return;

            PerformBasicAttack1();
            PerformBasicAttack2();
        }

        private void PerformBasicAttack1() {
            if (!Input.GetMouseButton(0) || !collision.onGround || collision.onWall || isChainAttack)
                return;

            isPerformingSkill = true;

            float clipLength = animationScript.GetAnimationClipLength(5);
            StartCoroutine(PerformSkill(Skill.BasicAttack1, clipLength));
        }

        private void PerformBasicAttack2() {
            if (!isChainAttack || activeSkill != Skill.BasicAttack1)
                return;
            
            if (!Input.GetMouseButton(0) || !collision.onGround || collision.onWall)
                return;

            isPerformingSkill = true;

            float clipLength = animationScript.GetAnimationClipLength(6);
            StartCoroutine(PerformSkill(Skill.BasicAttack2, clipLength));
        }

        private IEnumerator PerformSkill(Skill skill, float time) {
            activeSkill = skill;
            
            animationScript.SetTrigger(activeSkill.ToString());
            yield return new WaitForSeconds(time);

            if (activeSkill == Skill.BasicAttack1 || activeSkill == Skill.BasicAttack2) {
                StopCoroutine(StartChainComboTimer());
                StartCoroutine(StartChainComboTimer());
            } else {
                activeSkill = Skill.None;
            }

            isPerformingSkill = false;
        }

        private IEnumerator StartChainComboTimer() {
            isChainAttack = true;
            yield return new WaitForSeconds(DropComboTime);
            
            isChainAttack = false;
            activeSkill = Skill.None;
        }
    }
}