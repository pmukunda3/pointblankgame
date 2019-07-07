using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeaponController : MonoBehaviour
{
    public GameObject Beam;
    public GameObject Impact;
    public float beamDuration;
    public float fireRate;
    public float range = 100f;
    public float impactOffset;

    private LineRenderer lineRenderer;
    private GameObject NewImpact;
    private Color initColor, beamColor;
    private float clock, fireInterval, t, alpha;
    private UserInput userInput;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = Beam.GetComponent<LineRenderer>();
        initColor = lineRenderer.endColor;
        Beam.SetActive(false);
        fireInterval = 1 / fireRate;
        userInput = gameObject.GetComponentInParent<UserInput>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (userInput.actions.primaryFire.active)
        {
            if (clock >= 0f)
            {                
                clock = -fireInterval;
                Beam.SetActive(true);
                beamColor = initColor;
                lineRenderer.startColor = beamColor;
                lineRenderer.endColor = beamColor;
                lineRenderer.SetPosition(0, Vector3.zero);
                Debug.Log(lineRenderer.GetPosition(0));
                if (Physics.Raycast(Beam.transform.position, Beam.transform.forward, out RaycastHit hit, range))
                {
                    lineRenderer.SetPosition(1, new Vector3(0,0,hit.distance));
                    Vector3 pos = hit.point + hit.normal * impactOffset;
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    NewImpact = Instantiate(Impact, pos, rot);
                    NewImpact.SetActive(true);
                    NewImpact.transform.parent = null;
                    //NewImpact.GetComponent<ParticleSystem>().StopAction
                }
                else
                {
                    lineRenderer.SetPosition(1, new Vector3(0,0,range));
                }
            }
        }
        if (clock < 0f)
        {
            clock += Time.deltaTime;
            if (clock >= -fireInterval + beamDuration)
            {
                Beam.SetActive(false);
            }
            else
            {
                t = Mathf.InverseLerp(-fireInterval, -fireInterval + beamDuration, clock);
                beamColor = Color.Lerp(initColor, Color.black, t);
                lineRenderer.startColor = beamColor;
                lineRenderer.endColor = beamColor;
            }
        }
    }
}
