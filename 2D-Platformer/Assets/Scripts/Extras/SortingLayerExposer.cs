﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Extras {
    public class SortingLayerExposer : MonoBehaviour {
        public string SortingLayerName = "Default";
        public int SortingOrder = 0;

        void Awake() {
            gameObject.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
            gameObject.GetComponent<MeshRenderer>().sortingOrder = SortingOrder;
        }
    }
}