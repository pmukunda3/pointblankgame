using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum RobotState
{
    Idle,
    ChasePlayer,
    Patrol,
    Meleeattack,
    DeadWait,
    InVehicle,
    Destroy

};
public class Robot_AI_Ctrl : MonoBehaviour
{
    private int curr_point = 0;
    // Start is called before the first frame update
    public NavMeshAgent nav_agent;
    public GameObject[] patrol_points;
    public Animator ai_animator;
    public float lifetime;
    private int DeathwaitCnt = 0;
    public float MarginFromPlayerXY = 1.5f;
    public float AttackEnableDistance = 10.0f;
    public GameObject WeaponCtrl;
    public ProjectileWeaponController Weapon;
    public bool RobotInVehicle = false;
    private bool dropLocationReached =false;


    public GameObject player;
    private RobotState ai_state;

    void OnEnable ()
	{ 
	    EventManager.StartListening<RobotDropOff, GameObject>(new UnityEngine.Events.UnityAction<GameObject>(SetDropLocationReached));
    }


    private void Patrol()
    {

        nav_agent.SetDestination(patrol_points[curr_point].transform.position);


    }

    private void chasePlayer()
    {
        Vector3 offset = new Vector3(MarginFromPlayerXY, 0, MarginFromPlayerXY);
        nav_agent.SetDestination(player.transform.position-offset);
    }

    private void meleeattack(Animator ai_animator)
    {
        //ai_animator.SetTrigger("Attack");
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void shoot()
    {
        Weapon.FireWeapon();
        EventManager.TriggerEvent<WeaponFirePrimary>();

    }

    private void SetRobotInvehicle()
    {
        RobotInVehicle = true;

    }

    private void SetDropLocationReached(GameObject obj)
    {
        dropLocationReached = true;

    }

    //    public override void AnimatorIK()
    //    {
    //LookTarget = cameraState.target;
    //animator.SetLookAtWeight(1f);
    //animator.SetLookAtPosition(LookTarget);

    //        LeftHandIKTarget = weaponManager.activeWeapon.GetComponent<WeaponIK>().LeftHandIKTarget;
    //animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
    //animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
    //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
    //animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);

    //        RightHandIKTarget = weaponManager.activeWeapon.GetComponent<WeaponIK>().RightHandIKTarget;
    //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
    //    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
    //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
    //    animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);

    //  }




    private void Start()
    {
        //ai_animator = gameObject.GetComponent<Animator>();
        nav_agent = gameObject.GetComponent<NavMeshAgent>();

        //Weapon = WeaponCtrl.GetComponent<WeaponManager>();

        ai_animator.SetBool("Idle", true);
        //Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        //ai_animator.SetBool("Idle", true);
        float dist_to_player = Vector3.Distance(nav_agent.transform.position,
    player.transform.position);

        float dist_to_patrol = Vector3.Distance(nav_agent.transform.position,
    patrol_points[curr_point].transform.position);

        ai_animator.SetFloat("Shoot", 0);

        switch (ai_state)
        {
            case RobotState.Idle:
                ai_animator.SetBool("Idle", true);
                if (RobotInVehicle == true)
                {
                    ai_state = RobotState.InVehicle;
                }
                else
                {
                    Patrol();
                    ai_state = RobotState.Patrol;
                }

                break;

            case RobotState.InVehicle:
                ai_animator.SetBool("Idle", true);
                if (dropLocationReached == true)
                {
                    ai_state = RobotState.Idle;
                }
                break;

            case RobotState.Patrol:
                ai_animator.SetBool("Idle", false);

                if (dist_to_player < AttackEnableDistance)
                {
                    ai_state = RobotState.ChasePlayer;
                    transform.LookAt(player.transform.position);
                }

                else
                {
                    if (nav_agent.remainingDistance <= 0.5)
                    {
                        //curr_point = (curr_point + 1) % patrol_points.Length;
                        curr_point = Random.Range(0, patrol_points.Length);
                        Patrol();
                    }
                    //
                }
                break;

            case RobotState.ChasePlayer:
                ai_animator.SetBool("Idle", false);
                if (dist_to_player > AttackEnableDistance+0.5)
                {
                    Patrol();
                    ai_state = RobotState.Patrol;
                }
                else if (dist_to_player < MarginFromPlayerXY+2)
                {
                    meleeattack(ai_animator);
                    ai_state = RobotState.Meleeattack;
                    transform.LookAt(player.transform.position);
                }
                else
                {
                    chasePlayer();
                }
                break;

            case RobotState.Meleeattack:
                ai_animator.SetBool("Idle", false);
                transform.LookAt(player.transform.position);
                if (dist_to_player > MarginFromPlayerXY + 2.1)
                {
                    chasePlayer();
                    ai_state = RobotState.ChasePlayer;
                }
                else
                {
                    meleeattack(ai_animator);
                    ai_animator.SetFloat("Shoot", 0.5f);
                    shoot();

                }
                break;

            case RobotState.DeadWait:
                if (DeathwaitCnt == 0)
                {
                    ai_animator.SetTrigger("Death");
                }
                else
                {
                    if (DeathwaitCnt > 50)
                    {
                        ai_state = RobotState.Destroy;
                    }
                    DeathwaitCnt++;
                }
                break;

            case RobotState.Destroy:
                DestroyObj();
                break;

        }
        //Debug.Log(dist_to_player);
        Debug.Log(ai_state);
        //update animation
        ai_animator.SetFloat("Forward", nav_agent.velocity.magnitude / nav_agent.speed);
        float angle = Vector3.Angle(nav_agent.velocity.normalized, this.transform.forward);
        if (nav_agent.velocity.normalized.x < this.transform.forward.x)
        {
            angle *= -1;
        }
        angle = angle % 360.0f;

        float normalized_angle = angle / 360;

        ai_animator.SetFloat("Turn", normalized_angle);


        Vector3 dir = nav_agent.pathEndPosition - player.transform.position;
    }//end of switch


}

