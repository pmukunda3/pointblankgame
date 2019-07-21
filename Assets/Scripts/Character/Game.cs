using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerControl {
    public class Game : MonoBehaviour {

        public CharacterOverheat overheat;
        public CharacterHealth health;

        public AnimationCurve fallDamageCurve;

        public float fallingRespawnInterval = 6.0f;
        public float fallingRespawnWaitTime = 1.2f;

        private UnityAction<int> damageAction;
        private UnityAction<float> fallDamageAction;
        private UnityAction outOfBoundsAction;

        private new Rigidbody rigidbody;

        private float respawnLocationTimer = 0.0f;
        private Vector3 fallingRespawnLocation;
        private Quaternion fallingRespawnRotation;

        private float fallingRespawnTimer = 0.0f;
        private bool outOfBounds = false;
        private float healthAtOutOfBoundsEvent = 0.0f;

        public void Start() {
            health.MaxHealth = 100f;

            rigidbody = gameObject.GetComponent<Rigidbody>();

            respawnLocationTimer = 9999.9f;
            Grounded();

            damageAction = new UnityAction<int>(TakeDamage);
            fallDamageAction = new UnityAction<float>(FallDamage);
            outOfBoundsAction = new UnityAction(OutOfBounds);

            EventManager.StartListening<PlayerDamageEvent, int>(damageAction);
            EventManager.StartListening<PlayerFallDamageEvent, float>(fallDamageAction);
            EventManager.StartListening<PlayerOutOfBoundsEvent>(outOfBoundsAction);
        }

        public void Update() {
            //if (rigidbody.position.y < -50.0f) {
            //    FallRespawn();
            //}

            if (outOfBounds) {
                if (fallingRespawnTimer >= fallingRespawnWaitTime) {
                    outOfBounds = false;
                    FallRespawn();
                    EventManager.TriggerEvent<MecanimBehaviour.FreeRoamEvent>();
                }
            }

            respawnLocationTimer += Time.deltaTime;
            fallingRespawnTimer += Time.deltaTime;
        }

        public void OnDestroy() {
            EventManager.StopListening<PlayerDamageEvent, int>(damageAction);
            EventManager.StopListening<PlayerFallDamageEvent, float>(fallDamageAction);
            EventManager.StopListening<PlayerOutOfBoundsEvent>(outOfBoundsAction);
        }

        public void Grounded() {
            if (respawnLocationTimer > fallingRespawnInterval && !outOfBounds) {
                fallingRespawnLocation = rigidbody.position + Vector3.up * 0.2f;
                fallingRespawnRotation = rigidbody.rotation;
                respawnLocationTimer = 0.0f;
            }
        }

        public void OutOfBounds() {
            Debug.Log("OutOfBounds called");
            healthAtOutOfBoundsEvent = health.CurrHealth;
            fallingRespawnTimer = 0.0f;
            outOfBounds = true;
        }

        public void FallRespawn() {
            health.CurrHealth = healthAtOutOfBoundsEvent;

            rigidbody.velocity = Vector3.zero;
            rigidbody.position = fallingRespawnLocation;
            rigidbody.rotation = fallingRespawnRotation;
        }

        public void FallDamage(float verticalVelocity) {
            float damage = Mathf.Round(fallDamageCurve.Evaluate(verticalVelocity));
            
            if (health.CurrHealth <= 1.0f) {
                damage = 1.0f;
            }
            else if (health.CurrHealth - damage <= 1) {
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
    public class PlayerOutOfBoundsEvent : UnityEvent { }
    public class PlayerDeathEvent : UnityEvent { }
}
