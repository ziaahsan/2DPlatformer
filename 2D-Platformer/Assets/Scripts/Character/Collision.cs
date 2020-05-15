using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class Collision : MonoBehaviour {

        [Header("Layers")]
        public LayerMask GroundLayer;

        [Header("Collision References")]
        public Transform HeadOffset;
        public float HeadOffsetRadius;

        public Transform FeetOffset;
        public float FeetOffsetRadius;

        public Transform RightArmOffset;
        public float RightArmOffsetRadius;

        public Transform LeftArmOffset;
        public float LeftArmOffsetRadius;

        [Header("State Checks")]
        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        public int wallSide;

        private void Update() {
            onGround = Physics2D.OverlapCircle(FeetOffset.position, FeetOffsetRadius, GroundLayer);

            onWall = Physics2D.OverlapCircle(RightArmOffset.position, RightArmOffsetRadius, GroundLayer) ||
                Physics2D.OverlapCircle(LeftArmOffset.position, LeftArmOffsetRadius, GroundLayer);

            onRightWall = Physics2D.OverlapCircle(RightArmOffset.position, RightArmOffsetRadius, GroundLayer);
            onLeftWall = Physics2D.OverlapCircle(LeftArmOffset.position, LeftArmOffsetRadius, GroundLayer);

            wallSide = onRightWall ? 1 : -1;
        }

        private void OnDrawGizmos() {
            if (onGround) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(FeetOffset.position, FeetOffsetRadius);

            Gizmos.color = Color.white;
            if (onRightWall) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(RightArmOffset.position, RightArmOffsetRadius);

            Gizmos.color = Color.white;
            if (onLeftWall) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(LeftArmOffset.position, LeftArmOffsetRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(HeadOffset.position, HeadOffsetRadius);
        }
    }
}