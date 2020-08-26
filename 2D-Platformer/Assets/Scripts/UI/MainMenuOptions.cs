using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// External Packages
using DG.Tweening;
using TMPro;

// Our Packages
using ProjectX.Objects;

namespace ProjectX.UI {
    public class MainMenuOptions : MonoBehaviour {

        [Header("Indicator")]
        public Transform activeReticle;

        [Header("State Checks")]
        public int buttonIndex;
        public int totalButtons;
        public bool keyDown;
        public bool submitKeyDown;
        public int coroutineCount;

        private GameManager gameManager;
        
        private Transform currentChild;
        private TextMeshProUGUI currentButtonText;

        private void Awake() {
            gameManager = FindObjectOfType<GameManager>();

            currentChild = transform.Find("Button " + buttonIndex);
            currentButtonText = currentChild.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        private void Update() {
            if (keyDown) {
                return;
            }

            CheckInput();
            ShakeActiveReticle();
        }

        private void CheckInput() {
            if (submitKeyDown) {
                return;
            }

            if (Input.GetAxis("Vertical") != 0) {
                if (!keyDown) {
                    if (Input.GetAxis("Vertical") < 0) {
                        if (buttonIndex < totalButtons) {
                            buttonIndex++;
                        } else {
                            buttonIndex = 0;
                        }
                    } else if (Input.GetAxis("Vertical") > 0) {
                        if (buttonIndex > 0) {
                            buttonIndex--;
                        } else {
                            buttonIndex = totalButtons;
                        }
                    }
                    keyDown = true;
                    StartCoroutine(AnimateReticleOnPress());
                }
            } else if (Input.GetAxis("Submit") == 1) {
                if (!submitKeyDown) {
                    submitKeyDown = true;
                    StartCoroutine(SubmitChoice());
                }
            }
        }

        private void ShakeActiveReticle() {
            activeReticle.DOComplete();
            activeReticle.DOShakePosition(0.05f, 1, 10, 0, false, true);
        }

        private void PlayGame() {
            Portal portal = FindObjectOfType<Portal>();
            gameManager.SwitchScene(portal.jumpToScene, portal);
        }

        private IEnumerator AnimateReticleOnPress() {
            coroutineCount++;

            currentChild = transform.Find("Button " + buttonIndex);
            currentButtonText = currentChild.GetChild(0).GetComponent<TextMeshProUGUI>();

            float timer = 0;
            while (timer < 0.7f) {
                activeReticle.DOMoveY(currentChild.position.y, 0.5f, true);
                timer += 2f * Time.deltaTime;
                yield return null;
            }

            keyDown = false;
            
            coroutineCount--;
        }

        private IEnumerator SubmitChoice() {
            coroutineCount++;

            switch (buttonIndex) {
                case 0:
                    PlayGame();
                    break;
            }

            yield return null;

            coroutineCount--;
        }
    }
}