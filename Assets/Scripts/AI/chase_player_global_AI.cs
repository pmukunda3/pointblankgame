
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum AIState
{
    ChasePlayer,
    Patrol,
    Meleeattack,
    Dead

};
public class chase_player_global_AI : MonoBehaviour
{
    private int curr_point = 0;
    // Start is called before the first frame update
    public NavMeshAgent nav_agent;
    public GameObject[] patrol_points;
    public Animator ai_animator;
    public float lifetime;


    public GameObject player;
    public AIState ai_state;




    private void Patrol()
    {
        nav_agent.SetDestination(patrol_points[curr_point].transform.position);
        //Debug.Log(patrol_points[curr_point].transform.position);

    }

    private void chasePlayer()
    {
        nav_agent.SetDestination(player.transform.position);
    }

    private void meleeattack(Animator ai_animator)
    {
        ai_animator.SetTrigger("Attack");
    }

    private void setDeath(float time)
    {
        //yield WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void Start()
    {
        ai_animator = gameObject.GetComponent<Animator>();
        nav_agent = gameObject.GetComponent<NavMeshAgent>();
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {

        float dist_to_player = Vector3.Distance(nav_agent.transform.position,
    player.transform.position);

        float dist_to_patrol = Vector3.Distance(nav_agent.transform.position,
    patrol_points[curr_point].transform.position);

        switch (ai_state)
        {
            case AIState.Patrol:

                if (dist_to_player < 5)
                {
                    ai_state = AIState.ChasePlayer;
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

            case AIState.ChasePlayer:
                if (dist_to_player > 8)
                {
                    Patrol();
                    ai_state = AIState.Patrol;
                }
                else if (dist_to_player < 1)
                {
                    meleeattack(ai_animator);
                    ai_state = AIState.Meleeattack;
                }
                else
                {
                    chasePlayer();
                }
                break;

            case AIState.Meleeattack:
                if (dist_to_player > 1)
                {
                    chasePlayer();
                    ai_state = AIState.ChasePlayer;
                }
                else
                {
                    meleeattack(ai_animator);
                }
                break;

            case AIState.Dead:
                break;

        }

        Debug.Log("AIState " + ai_state);
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


    //update animation


