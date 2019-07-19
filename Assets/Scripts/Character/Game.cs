using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl {
    public class Game : MonoBehaviour {

        public CharacterOverheat overheat;
        public CharacterHealth health;

        public AnimationCurve fallDamageCurve;

        private UnityAction<int> damageAction;
        private UnityAction<float> fallDamageAction;

        public void Start() {
            health.MaxHealth = 100f;

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
            float damage = Mathf.Round(fallDamageCurve.Evaluate(verticalVelocity));
            
            if (health.CurrHealth - damage <= 1) {
                damage = health.CurrHealth - 1;
            }

            Debug.Log("Player takes " + damage + " damage");

            health.DealDamage(damage);
        }

        public void TakeDamage(int amount) {
            health.DealDamage(amount);
            health.CurrHealth -= amount;

            //if (healthPoints <= 0) {
            //    EventManager.TriggerEvent<PlayerDeathEvent>();
            //}
        }
    }

    public class PlayerDamageEvent : UnityEvent<int> { }
    public class PlayerFallDamageEvent : UnityEvent<float> { }
    public class PlayerDeathEvent : UnityEvent { }
}
