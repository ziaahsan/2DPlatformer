using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectX {
    public class Constants {

        /// <summary>
        /// Errors
        /// </summary>
        public const string SceneDuplicateLoadError = "A scene already pending for load";

        /// <summary>
        /// Enums References
        /// </summary>
        public enum SceneName { SceneNone, SceneMain, Scene0, Scene1, Scene2 };
        public enum PortalNames { SMP1, SMP2, S0P1, S0P2, S1P1, S1P2, S2P1, S2P2, Map1P1, Map1P2 }

        public enum Tag { Player };

        /// <summary>
        /// Object References
        /// </summary>
        public const string AttakHitBoxes = "Attack Hit Boxes";
        public const string CameraBoundary = "Camera Boundary";
        public const string Canvas = "Canvas";
        public const string Trash = "Trash";

        public static SceneName GetSceneName(string name) {
            SceneName sceneName = SceneName.SceneNone;
            
            switch(name) {
                case "Scene Main":
                    sceneName = SceneName.SceneMain;
                    break;
                case "Scene 0":
                    sceneName = SceneName.Scene0;
                    break;
                case "Scene 1":
                    sceneName = SceneName.Scene1;
                    break;
                case "Scene 2":
                    sceneName = SceneName.Scene2;
                    break;
            }

            return sceneName;
        }
    }
}