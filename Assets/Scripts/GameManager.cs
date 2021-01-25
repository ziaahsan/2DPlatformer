using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// External Packages
using Cinemachine;


// Our Current object pacakge
using ProjectX.Objects;

namespace ProjectX {
    public class GameManager : MonoBehaviour {

        public static GameManager Instance;
        public static Transform Canvas;

        [Header("Player Info")]
        public GameObject PlayerObject;
        public float playerSpawnDistance;

        [Header("Cinemachine")]
        [SerializeField]
        private CinemachineVirtualCamera CVM;

        [Header("States")]
        public int coroutineCount;
        public Constants.SceneName currentScene;

        [Header("Canvas")]
        [SerializeField] Transform mainMenuCanvas;
        [SerializeField] Transform playerCanvas;
        [SerializeField] Transform loadingCanvas;

        private GameObject player;

        private Portal[] scenePortals;
        private Portal previousPortal;


        private void Awake() {
            currentScene = Constants.GetSceneName(SceneManager.GetActiveScene().name);

            if (Instance) {
                DestroyImmediate(gameObject);
                return;
            }            
            
            Instance = this;

            // Setup canvas
            Canvas = Instance.transform.Find(Constants.Canvas);

            mainMenuCanvas = Canvas.Find("Main Menu Canvas");
            mainMenuCanvas.GetComponent<Canvas>().enabled = false;

            playerCanvas = Canvas.Find("Player Canvas");
            playerCanvas.GetComponent<Canvas>().enabled = false;
            
            loadingCanvas = Canvas.Find("Loader 01 Canvas");
            loadingCanvas.GetComponent<Canvas>().enabled = false;

            CVM = FindObjectOfType<CinemachineVirtualCamera>();

            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
            SetupCamera();
            SetupSceneCanvas();
        }

        private void SetupSceneCanvas() {
            switch (currentScene) {
                case Constants.SceneName.SceneMain:
                    mainMenuCanvas.GetComponent<Canvas>().enabled = true;
                    playerCanvas.GetComponent<Canvas>().enabled = false;
                    break;
                case Constants.SceneName.Scene0:
                case Constants.SceneName.Scene1:
                case Constants.SceneName.Scene2:
                    mainMenuCanvas.GetComponent<Canvas>().enabled = false;
                    // playerCanvas.GetComponent<Canvas>().enabled = true;
                    break;

            }
        }

        private void SetupCamera() {
            CVM.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find(Constants.CameraBoundary).GetComponent<PolygonCollider2D>();

            if (player) {
                CVM.LookAt = player.transform;
                CVM.Follow = player.transform;
            }
        }

        private void SetupPlayer(Vector2 position) {
            player = Instantiate(PlayerObject, position, Quaternion.identity);
            SetupCamera();
        }

        private void SceneSetup() {
            // Get the portal on the scene.
            scenePortals = FindObjectsOfType<Portal>();

            foreach (Portal portal in scenePortals) {
                if (portal.name == previousPortal.jumpToPortalName) {
                    // Spawn the player
                    SetupPlayer(portal.transform.position + portal.transform.right * playerSpawnDistance);
                    break;
                }
            }
        }

        public void SwitchScene(string name, Portal portal) {
            if (loadingCanvas.GetComponent<Canvas>().enabled) {
                Debug.Log(Constants.SceneDuplicateLoadError);
                return;
            }

            Destroy(player);

            previousPortal = portal;
            currentScene = Constants.GetSceneName(name);

            loadingCanvas.GetComponent<Canvas>().enabled = true;
            StartCoroutine(BeginSceneLoad(name, LoadSceneMode.Single));
        }

        private IEnumerator BeginSceneLoad(string name, LoadSceneMode mode) {
            coroutineCount++;

            yield return null;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name, mode);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone) {
                if (asyncOperation.progress >= 0.9f) {
                    //yield return new WaitForSeconds(1f);
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            loadingCanvas.GetComponent<Canvas>().enabled = false;
            asyncOperation = null;

            SetupSceneCanvas();
            SceneSetup();
            
            coroutineCount--;
        }
    }
}