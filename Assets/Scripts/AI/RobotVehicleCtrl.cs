using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotVehicleCtrl : MonoBehaviour
{
    public Transform StartLoc;
    public Transform End;
    public Transform DropUp;
    public Transform DropDown;
    public float DropWaitDurr;
    private float clock;


    private VehicleState VhState;

    public enum VehicleState
    {
        Idle,
        GoToDropUP,
        GoToDropDn,
        DropWait,
        GoToDropUpFromDn,
        GotoEnd
    };

    // Start is called before the first frame update
    void Start()
    {
        VhState = VehicleState.Idle;
        transform.position = StartLoc.position;
    }

    // Update is called once per frame
    void Update()
    {
        float dist_to_End = Vector3.Distance(transform.position, End.position);
        float dist_to_DropUp = Vector3.Distance(transform.position, DropUp.position);
        float dist_to_DropDown = Vector3.Distance(transform.position, DropDown.position);

        switch (VhState)
        {
            case VehicleState.Idle:
                VhState = VehicleState.GoToDropUP;
                break;

            case VehicleState.GoToDropUP:
                if (dist_to_DropUp < 0.1)
                {
                    VhState = VehicleState.GoToDropDn;

                } else
                {

                }
                break;

            case VehicleState.GoToDropDn:
                if (dist_to_DropDown < 0.1)
                {
                    VhState = VehicleState.DropWait;

                }
                else
                {

                }
                break;

            case VehicleState.DropWait:
                if (clock >= DropWaitDurr)
                {
                    clock += Time.deltaTime;
                    VhState = VehicleState.GoToDropUpFromDn;
                }
                break;

            case VehicleState.GoToDropUpFromDn:
                if (dist_to_DropUp < 0.1)
                {
                    VhState = VehicleState.GoToDropDn;
                }
                else
                {

                }
                break;

            case VehicleState.GotoEnd:
                if (dist_to_End < 0.1)
                {
                    Destroy(this);
                }
                else
                {

                }
                break;

        }
    }
}
