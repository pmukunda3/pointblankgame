using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RobotVehicleCtrl : MonoBehaviour
{

    public BansheeGz.BGSpline.Components.BGCcMath targetSpline;
    public AudioSource audioSource;
    public RobotFloatTrigger trigger;
    public Transform End;
    public Transform DropDown;
    public float DropWaitDurr;
    private float clock = 0.0f;
    private VehicleState VhState;
    private Rigidbody rg;

    public float traverseTime = 16.0f;

    private bool animating = false;
    private float elapsedTime = 0.0f;

    private UnityAction callback;

    public enum VehicleState
    {
        Idle,
        GoToDrop,
        DropWait,
        GoToDropUpFromDn,
        GotoEnd
    };

    public void Start()
    {
        trigger.SetCallback(new UnityAction(StartAnimating));
        VhState = VehicleState.Idle;
        rg = gameObject.GetComponent<Rigidbody>();
        //VhState = VehicleState.GoToDrop;
    }

    private void StartAnimating()
    {
        VhState = VehicleState.GoToDrop;
        animating = true;
        elapsedTime = 0.0f;
        audioSource.Play();
    }

    private void AnimateVehicle(float time)
    {
        //Debug.Log(time / traverseTime);
        Vector3 position = targetSpline.CalcByDistanceRatio(BansheeGz.BGSpline.Curve.BGCurveBaseMath.Field.Position, time / traverseTime);
        Vector3 tangent = targetSpline.CalcByDistanceRatio(BansheeGz.BGSpline.Curve.BGCurveBaseMath.Field.Tangent, time / traverseTime);
        this.rg.MovePosition(position);
        this.rg.MoveRotation(Quaternion.LookRotation(tangent));
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log(VhState);
        float dist_to_End = Vector3.Distance(transform.position, End.position);
        float dist_to_DropDown = Vector3.Distance(transform.position, DropDown.position);
        float minf = 100000.0f;
        minf = Mathf.Min(minf, dist_to_DropDown);

        //Debug.Log(dist_to_End);
        switch (VhState)
        {
          case VehicleState.Idle:
                VhState = VehicleState.Idle;
                break;

            case VehicleState.GoToDrop:
                if (dist_to_DropDown <3)
                {
                    VhState = VehicleState.DropWait;
                    EventManager.TriggerEvent<RobotDropOff, int>(0);

                } else
                {
                    AnimateVehicle(elapsedTime);
                    elapsedTime += Time.deltaTime;
                }
                break;

            case VehicleState.DropWait:
                if (clock >= DropWaitDurr)
                {
                    VhState = VehicleState.GoToDropUpFromDn;
                    
                }
                clock += Time.deltaTime;
                break;

            case VehicleState.GoToDropUpFromDn:
                if (dist_to_End < 55)
                {
                    VhState = VehicleState.GotoEnd;
                }
                else
                {
                    AnimateVehicle(elapsedTime);
                    elapsedTime += Time.deltaTime;
                }
                break;

            case VehicleState.GotoEnd:
                {
                    audioSource.Stop();
                    Destroy(gameObject);
                    break;
                }

        }
    }
}
