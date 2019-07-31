using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

// the third param is the impact game object, which contain the 
// hit point and hit rotation
public class HitEnemyEvent : UnityEvent<GameObject, float, GameObject> { 

}

