using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl {
    public abstract class CameraState : MonoBehaviour {

        public abstract void CameraLateUpdate();
        public abstract void CameraUpdate();
    }
}