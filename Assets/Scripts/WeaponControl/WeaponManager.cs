using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlayerControl.MecanimBehaviour;

public class WeaponManager : MonoBehaviour, IWeaponFire
{
    public GameObject UnaimedWeapons;
    public GameObject AimedWeapons;
    public Animator animator;
    public GameObject activeWeapon
    {
        get;
        private set;
    }

    private int i, n;
    private bool fireEnabled;
    private IWeaponFire fireInterface;
    private UnityAction aimOn, aimOff, midAir;

    private void AimOff()
    {        
        AimedWeapons.SetActive(false);
        animator.SetBool("holdWeapon", true);
        UnaimedWeapons.SetActive(true);
    }

    private void MidAir()
    {        
        AimedWeapons.SetActive(false);
        animator.SetBool("holdWeapon", false);
        UnaimedWeapons.SetActive(true);
    }

    private void AimOn()
    {
        UnaimedWeapons.SetActive(false);
        animator.SetBool("holdWeapon", false);
        AimedWeapons.SetActive(true);
    }

    private void DisableFire()
    {
        fireEnabled = false;
    }

    private void EnableFire()
    {
        fireEnabled = true;
    }

    // Update is called once per frame
    public void ChangeWeapon()
    {
        UnaimedWeapons.transform.GetChild(i).gameObject.SetActive(false);
        activeWeapon.SetActive(false);
        i = ++i % n;
        UnaimedWeapons.transform.GetChild(i).gameObject.SetActive(true);
        activeWeapon = AimedWeapons.transform.GetChild(i).gameObject;
        activeWeapon.SetActive(true);
        fireInterface = activeWeapon.GetComponent<IWeaponFire>() as IWeaponFire;

        if (activeWeapon.GetComponent<WeaponProperties>().twoHanded)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 1f);
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 0f);
        }

    }

    public void FireWeapon() {
        if(fireEnabled)
        {
            fireInterface.FireWeapon();
        }
    }

    public void FireWeaponDown() {
        if (fireEnabled)
        {
            fireInterface.FireWeaponDown();
        }
    }

    public void FireWeaponUp() {
        if (fireEnabled)
        {
            fireInterface.FireWeaponUp();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fireEnabled = true;
        aimOn = new UnityAction(AimOn);
        aimOff = new UnityAction(AimOff);
        midAir = new UnityAction(MidAir);
        EventManager.StartListening<AimingEvent>(aimOn);
        EventManager.StartListening<FreeRoamEvent>(aimOff);
        EventManager.StartListening<SprintEvent>(aimOff);
        EventManager.StartListening<JumpEvent>(midAir);
        EventManager.StartListening<FallingEvent>(midAir);
        EventManager.StartListening<ThrowStartEvent>(new UnityAction(DisableFire));
        EventManager.StartListening<ThrowEndEvent>(new UnityAction(EnableFire));

        n = AimedWeapons.transform.childCount;
        for (i = 0; i < n; i++)
        {
            UnaimedWeapons.transform.GetChild(i).gameObject.SetActive(false);
            AimedWeapons.transform.GetChild(i).gameObject.SetActive(false);
        }
        i = 0;
        UnaimedWeapons.transform.GetChild(i).gameObject.SetActive(true);
        activeWeapon = AimedWeapons.transform.GetChild(i).gameObject;
        activeWeapon.SetActive(true);
        fireInterface = activeWeapon.GetComponent<IWeaponFire>() as IWeaponFire;

        if (activeWeapon.GetComponent<WeaponProperties>().twoHanded)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 1f);
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 0f);
        }
    }
}
