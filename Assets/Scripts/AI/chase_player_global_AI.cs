
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public enum AIState
{
    ChasePlayer,
    Patrol,
    Meleeattack,
    Dead

};
public class chase_player_global_AI : MonoBehaviour
{
    UnityEvent aiEvent;
    private int curr_point = 0;
    // Start is called before the first frame update
    public NavMeshAgent nav_agent;
    public GameObject patrol_prefab;

    public Animator ai_animator;
    public float lifetime;


    public GameObject player;
    public AIState ai_state;

    private List<GameObject> patrol_points;


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
        player = GameObject.FindWithTag("Player");

        Debug.Log("Patrol prefab is ");
        Debug.Log(patrol_prefab.transform);

        //set up patrol point based on patrol prefab
        patrol_points = new List<GameObject>();
        foreach (Transform t in patrol_prefab.transform)
        {
            Debug.Log(patrol_points);
            Debug.Log(t);
            Debug.Log(t.gameObject);
            patrol_points.Add(t.gameObject);
        }

        chasePlayer();
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

                if (dist_to_player < 500)
                {
                    ai_state = AIState.ChasePlayer;
                }

                else
                {
                    if (nav_agent.remainingDistance <= 0.5)
                    {
                        curr_point = (curr_point + 1) % patrol_points.Count;
                        Patrol();
                    }
                    //
                }
                break;

            case AIState.ChasePlayer:
                if (dist_to_player > 800)
                {
                    Patrol();
                    ai_state = AIState.Patrol;
                }
                else if (dist_to_player < 2)
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
                if (dist_to_player > 2)
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


