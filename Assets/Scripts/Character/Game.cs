using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl {
    public class Game : MonoBehaviour {

        public AnimationCurve fallDamageCurve;

        private int healthPoints = 100;
        private UnityAction<int> damageAction;
        private UnityAction<float> fallDamageAction;

        public void Start() {
            damageAction = new UnityAction<int>(TakeDamage);
            fallDamageAction = new UnityAction<float>(FallDamage);

            EventManager.StartListening<PlayerDamageEvent, int>(damageAction);
            EventManager.StartListening<PlayerFallDamageEvent, float>(fallDamageAction);
        }

        public void OnDestroy() {
            EventManager.StopListening<PlayerDamageEvent, int>(damageAction);
            EventManager.StopListening<PlayerFallDamageEvent, float>(fallDamageAction);
        }

        public void FallDamage(float verticalVelocity) {
            int damage = Mathf.RoundToInt(fallDamageCurve.Evaluate(verticalVelocity));

            if (healthPoints - damage <= 1) {
                healthPoints = 1;
            }
            else {
                healthPoints -= damage;
            }
        }

        public void TakeDamage(int amount) {
            healthPoints -= amount;

            if (healthPoints <= 0) {
                EventManager.TriggerEvent<PlayerDeathEvent>();
            }
        }
    }

    public class PlayerDamageEvent : UnityEvent<int> { }
    public class PlayerFallDamageEvent : UnityEvent<float> { }
    public class PlayerDeathEvent : UnityEvent { }
}
