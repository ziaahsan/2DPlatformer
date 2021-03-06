﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX {
    public class Collision : MonoBehaviour {

        [Header("Layers")]
        public LayerMask WhatIsGround;

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
        public bool headOnGround;
        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        public int wallSide;

        private void Update() {
            headOnGround = Physics2D.OverlapCircle(HeadOffset.position, HeadOffsetRadius, WhatIsGround);

            onGround = Physics2D.OverlapCircle(FeetOffset.position, FeetOffsetRadius, WhatIsGround);

            onRightWall = Physics2D.OverlapCircle(RightArmOffset.position, RightArmOffsetRadius, WhatIsGround);
            onLeftWall = Physics2D.OverlapCircle(LeftArmOffset.position, LeftArmOffsetRadius, WhatIsGround);

            onWall = onRightWall || onLeftWall;

            wallSide = onRightWall ? 1 : -1;
        }

        private void OnDrawGizmos() {
            if (headOnGround) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(HeadOffset.position, HeadOffsetRadius);

            Gizmos.color = Color.white;
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
        }
    }
}