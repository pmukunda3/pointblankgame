using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlyingCarBackground : MonoBehaviour {

    public BansheeGz.BGSpline.Components.BGCcMath targetSpline;
    public Rigidbody vehicle;
    public AudioSource audioSource;
    public FlyingCarTrigger trigger;

    public float traverseTime = 6.0f;

    private bool animating = false;
    private float elapsedTime = 0.0f;

    private UnityAction callback;

    public void Start() {
        trigger.SetCallback(new UnityAction(StartAnimating));
    }

    public void Update() {
        if (animating) {
            AnimateVehicle(elapsedTime);

            if (elapsedTime > traverseTime) {
                animating = false;
                audioSource.Stop();

                trigger.StartTimer();
            }
        }

        elapsedTime += Time.deltaTime;
    }

    private void StartAnimating() {
        animating = true;
        elapsedTime = 0.0f;
        audioSource.Play();
    }

    private void AnimateVehicle(float time) {
        Vector3 position = targetSpline.CalcByDistanceRatio(BansheeGz.BGSpline.Curve.BGCurveBaseMath.Field.Position, time / traverseTime);
        Vector3 tangent = targetSpline.CalcByDistanceRatio(BansheeGz.BGSpline.Curve.BGCurveBaseMath.Field.Tangent, time / traverseTime);
        vehicle.MovePosition(position);
        vehicle.MoveRotation(Quaternion.LookRotation(tangent));
    }
}
