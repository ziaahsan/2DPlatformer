using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX.Objects {

    public class Portal : MonoBehaviour {
        public new string name;

        [Header("Layers")]
        public LayerMask WhatIsPlayer;

        [Header("Scene Info")]
        public string jumpToScene;
        public string jumpToPortalName;

        [Header("Offsets")]
        public Vector2 offset;
        public Vector2 portalDimensions;

        [Header("States")]
        public bool hasPlayer;

        private GameManager gameManager;
        private float cooldownTimers = 2.0f;

        private void Awake() {
            gameManager = FindObjectOfType<GameManager>();
        }

        private void Update() {
            if (cooldownTimers > 0.01f) {
                cooldownTimers -= Time.deltaTime;
                return;
            }

            hasPlayer = Physics2D.OverlapBox((Vector2) transform.position + offset, portalDimensions, 0, WhatIsPlayer);
            if (!hasPlayer || !Input.GetKeyDown(KeyCode.W)) {
                return;
            }

            gameManager.SwitchScene(jumpToScene, this);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube((Vector2)transform.position + offset, portalDimensions);
        }
    }


}