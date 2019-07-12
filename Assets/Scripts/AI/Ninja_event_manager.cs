using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ninjaEventManager : MonoBehaviour
{
    public Animator ai_animator;

    // Use this for initialization
    void OnEnable()
    {
        EventManager.StartListening<HitEnmeyEvent, GameObject, float, Vector3>
        (new UnityEngine.Events.UnityAction<GameObject, Vector3, float>
            (GotHit));
    }
    void OnDisable()
    {
        EventManager.StopListening<HitEnmeyEvent, GameObject, float, Vector3>
        (new UnityEngine.Events.UnityAction<GameObject, Vector3, float>
            (GotHit));

    }

    // Update is called once per frame
    public void GotHit(GameObject hit_obj,float hit_point, Vector3 hit_transform)
    {
        var delay = 5.0;
        if(hit_obj == this)
        {
            ai_animator.SetTrigger("Dying");
            yield WaitForSeconds(delay);
            Destroy(gameObject);
        }

    }
}
