using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerControl {
    public class Game : MonoBehaviour {

        public AnimationCurve fallDamageCurve;

        private int healthPoints = 100;

        public void FallDamage(float verticalVelocity) {
            int damage = Mathf.RoundToInt(fallDamageCurve.Evaluate(verticalVelocity));

            if (healthPoints - damage <= 1) {
                healthPoints = 1;
            }
            else {
                healthPoints -= damage;
            }
        }
    }
}
