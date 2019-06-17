using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : MonoBehaviour, IMovementState {

    public PlayerController.CharacterMovementCharacteristics runningCharacteristics;

    void FixedUpdate() {
        // TODO: in here, perform checks for collisions or...
        //   maybe don't do it in fixed update...
    }

    public MovementChange CalculateAcceleration(Vector3 input, Vector3 localVelocity) {

        float forwardAcceleration = 0f;
        float lateralAcceleration = 0f;

        if (Mathf.Abs(input.x) > 0f) {
            if (localVelocity.x > PlayerController.EPSILON) {
                if (InputToTargetSpeedX(input.x) >= localVelocity.x) {
                    lateralAcceleration = runningCharacteristics.lateral.Acceleration(localVelocity.x);
                    if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime > InputToTargetSpeedX(input.x) + PlayerController.EPSILON) {
                        lateralAcceleration = (InputToTargetSpeedX(input.x) - localVelocity.x) / Time.fixedDeltaTime;
                    }
                }
                else if (InputToTargetSpeedX(input.x) < localVelocity.x) {
                    if (Mathf.Sign(input.x) != Mathf.Sign(localVelocity.x)) {
                        lateralAcceleration = -runningCharacteristics.lateral.Deceleration(localVelocity.x, true);
                    }
                    else {
                        lateralAcceleration = -runningCharacteristics.lateral.Deceleration(localVelocity.x, false);
                    }
                    if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime < InputToTargetSpeedX(input.x) + PlayerController.EPSILON) {
                        lateralAcceleration = (InputToTargetSpeedX(input.x) - localVelocity.x) / Time.fixedDeltaTime;
                    }
                }
            }
            else if (localVelocity.x < -PlayerController.EPSILON) {
                if (InputToTargetSpeedX(input.x) <= localVelocity.x) {
                    lateralAcceleration = -runningCharacteristics.lateral.Acceleration(-localVelocity.x);
                    if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime < InputToTargetSpeedX(input.x) - PlayerController.EPSILON) {
                        lateralAcceleration = (InputToTargetSpeedX(input.x) - localVelocity.x) / Time.fixedDeltaTime;
                    }
                }
                else if (InputToTargetSpeedX(input.x) > localVelocity.x) {
                    if (Mathf.Sign(input.x) != Mathf.Sign(localVelocity.x)) {
                        lateralAcceleration = runningCharacteristics.lateral.Deceleration(-localVelocity.x, true);
                    }
                    else {
                        lateralAcceleration = runningCharacteristics.lateral.Deceleration(-localVelocity.x, false);
                    }
                    if (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime > InputToTargetSpeedX(input.x) - PlayerController.EPSILON) {
                        lateralAcceleration = (InputToTargetSpeedX(input.x) - localVelocity.x) / Time.fixedDeltaTime;
                    }
                }
            }
            else {
                if (Mathf.Abs(InputToTargetSpeedX(input.x) - localVelocity.x) > PlayerController.EPSILON) {
                    lateralAcceleration = Mathf.Sign(input.x) * runningCharacteristics.lateral.Acceleration(Mathf.Abs(localVelocity.x));
                }
                else {
                    Debug.Log("This code should be impossible to reach. The input deadzone should be significantly (orders of magnitude) larger than EPSILON");
                    Debug.Break();
                    lateralAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                    //   You need to be able to maintain velocity
                    localVelocity.x = 0f;
                }
            }
        }
        else {
            if (Mathf.Abs(localVelocity.x) < PlayerController.EPSILON) {
                lateralAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                    //   You need to be able to maintain velocity
                localVelocity.x = 0f;
            }
            else {
                lateralAcceleration = Mathf.Sign(-localVelocity.x) * runningCharacteristics.lateral.Deceleration(Mathf.Abs(localVelocity.x), false);
                if (Mathf.Sign(localVelocity.x) * (localVelocity.x + lateralAcceleration * Time.fixedDeltaTime) < InputToTargetSpeedX(input.x) + PlayerController.EPSILON) {
                    lateralAcceleration = (InputToTargetSpeedX(input.x) - localVelocity.x) / Time.fixedDeltaTime;
                }
            }
        }

        if (Mathf.Abs(input.y) > 0f) {
            if (localVelocity.z > PlayerController.EPSILON) {
                if (InputToTargetSpeedY(input.y) >= localVelocity.z) {
                    forwardAcceleration = runningCharacteristics.forward.Acceleration(localVelocity.z);
                    if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime > InputToTargetSpeedY(input.y) + PlayerController.EPSILON) {
                        forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                    }
                }
                else if (InputToTargetSpeedY(input.y) < localVelocity.z) {
                    if (Mathf.Sign(input.y) != Mathf.Sign(localVelocity.z)) {
                        forwardAcceleration = -runningCharacteristics.forward.Deceleration(localVelocity.z, true);
                    }
                    else {
                        forwardAcceleration = -runningCharacteristics.forward.Deceleration(localVelocity.z, false);
                    }
                    if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime < InputToTargetSpeedY(input.y) + PlayerController.EPSILON) {
                        forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                    }
                }
            }
            else if (localVelocity.z < -PlayerController.EPSILON) {
                if (InputToTargetSpeedY(input.y) <= localVelocity.z) {
                    forwardAcceleration = -runningCharacteristics.reverse.Acceleration(-localVelocity.z);
                    if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime < InputToTargetSpeedY(input.y) - PlayerController.EPSILON) {
                        forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                    }
                }
                else if (InputToTargetSpeedY(input.y) > localVelocity.z) {
                    forwardAcceleration = runningCharacteristics.reverse.Deceleration(-localVelocity.z, false);
                    if (Mathf.Sign(input.y) != Mathf.Sign(localVelocity.z)) {
                        forwardAcceleration = runningCharacteristics.reverse.Deceleration(-localVelocity.z, true);
                    }
                    else {
                        forwardAcceleration = runningCharacteristics.reverse.Deceleration(-localVelocity.z, false);
                    }
                    if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime > InputToTargetSpeedY(input.y) - PlayerController.EPSILON) {
                        forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                    }
                }
            }
            else {
                if (Mathf.Abs(InputToTargetSpeedY(input.y) - localVelocity.z) > PlayerController.EPSILON) {
                    forwardAcceleration = Mathf.Sign(input.y) * runningCharacteristics.forward.Acceleration(Mathf.Abs(localVelocity.z));
                }
                else {
                    Debug.Log("This code should be impossible to reach. The input deadzone should be significantly (orders of magnitude) larger than EPSILON");
                    Debug.Break();
                    forwardAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                    //   You need to be able to maintain velocity
                    localVelocity.z = 0f;
                }
            }
        }
        else {
            if (localVelocity.z < -PlayerController.EPSILON) {
                forwardAcceleration = runningCharacteristics.reverse.Deceleration(-localVelocity.z, false);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime > InputToTargetSpeedY(input.y) - PlayerController.EPSILON) {
                    forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
            else if (localVelocity.z > PlayerController.EPSILON) {
                forwardAcceleration = -runningCharacteristics.forward.Deceleration(localVelocity.z, false);
                if (localVelocity.z + forwardAcceleration * Time.fixedDeltaTime < InputToTargetSpeedY(input.y) + PlayerController.EPSILON) {
                    forwardAcceleration = (InputToTargetSpeedY(input.y) - localVelocity.z) / Time.fixedDeltaTime;
                }
            }
            else {
                forwardAcceleration = 0f;
                    // TODO: THIS IS A MISTAKE, don't do this, replace in future
                    //   You need to be able to maintain velocity
                localVelocity.z = 0f;
            }
        }

        return new MovementChange(new Vector3(lateralAcceleration, 0f, forwardAcceleration), localVelocity);
    }

    private float InputToTargetSpeedY(float y) {
        if (y > 0) {
            return y * runningCharacteristics.forward.maxSpeed;
        }
        else {
            return y * runningCharacteristics.reverse.maxSpeed;
        }
    }

    private float InputToTargetSpeedX(float x) {
        return x * runningCharacteristics.lateral.maxSpeed;
    }

    public float MaxSpeed(int direction = 1) {
        switch (direction) {
            case 0:
                return runningCharacteristics.lateral.maxSpeed;
            case 1:
                return runningCharacteristics.forward.maxSpeed;
            default:
                return runningCharacteristics.forward.maxSpeed;
        }
    }
}
