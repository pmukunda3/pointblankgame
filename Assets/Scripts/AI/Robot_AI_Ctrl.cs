using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    Destroy

};
public class Robot_AI_Ctrl : MonoBehaviour
{
    private int curr_point = 0;
    // Start is called before the first frame update
    public NavMeshAgent nav_agent;
    public GameObject[] patrol_points;
    private Animator ai_animator;
    public float lifetime;
    private int DeathwaitCnt = 0;


    public GameObject player;
    private RobotState ai_state;




    private void Patrol()
    {
        nav_agent.SetDestination(patrol_points[curr_point].transform.position);


    }

    private void chasePlayer()
    {
        nav_agent.SetDestination(player.transform.position);
    }

    private void meleeattack(Animator ai_animator)
    {
        //ai_animator.SetTrigger("Attack");
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        ai_animator = gameObject.GetComponent<Animator>();
        nav_agent = gameObject.GetComponent<NavMeshAgent>();
        ai_animator.SetBool("Idle", false);
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        ai_animator.SetBool("Idle", false);
        float dist_to_player = Vector3.Distance(nav_agent.transform.position,
    player.transform.position);

        float dist_to_patrol = Vector3.Distance(nav_agent.transform.position,
    patrol_points[curr_point].transform.position);

        ai_animator.SetFloat("Shoot", 0);

        switch (ai_state)
        {
            case RobotState.Idle:
                ai_state = RobotState.Patrol;
                break;

            case RobotState.Patrol:
                ai_animator.SetBool("Idle", false);

                if (dist_to_player < 5)
                {
                    ai_state = RobotState.ChasePlayer;
                }

                else
                {
                    if (nav_agent.remainingDistance <= 0.5)
                    {
                        curr_point = (curr_point + 1) % patrol_points.Length;
                        Patrol();
                    }
                    //
                }
                break;

            case RobotState.ChasePlayer:
                ai_animator.SetBool("Idle", false);
                if (dist_to_player > 8)
                {
                    Patrol();
                    ai_state = RobotState.Patrol;
                }
                else if (dist_to_player < 1)
                {
                    meleeattack(ai_animator);
                    ai_state = RobotState.Meleeattack;
                }
                else
                {
                    chasePlayer();
                }
                break;

            case RobotState.Meleeattack:
                ai_animator.SetBool("Idle", false);
                if (dist_to_player > 1)
                {
                    chasePlayer();
                    ai_state = RobotState.ChasePlayer;
                }
                else
                {
                    meleeattack(ai_animator);
                    ai_animator.SetFloat("Shoot", 1-dist_to_player);
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
        Debug.Log(dist_to_player);
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

