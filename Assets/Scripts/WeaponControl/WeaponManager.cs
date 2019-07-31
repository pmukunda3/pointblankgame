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
    public float heatDecayRate, maxHeat;
    public CharacterOverheat overheatRef;

    private int currWeaponIndex, numWeapons;
    private float[] heatValues;
    private float heatPerShot;
    private bool fireEnabled;
    private IWeaponFire fireInterface;
    private UnityAction aimOn, aimOff, midAir;

    public float GetHeatPerShot()
    {
        return 0;
    }

    public void FireWeapon()
    {
        if(fireEnabled && heatValues[currWeaponIndex] <= maxHeat - heatPerShot)
        {
            fireInterface.FireWeapon();
        }
    }

    public void FireWeaponDown()
    {
        if (fireEnabled && heatValues[currWeaponIndex] <= maxHeat - heatPerShot)
        {
            fireInterface.FireWeaponDown();
        }
    }

    public void FireWeaponUp()
    {
        if (fireEnabled && heatValues[currWeaponIndex] <= maxHeat - heatPerShot)
        {
            fireInterface.FireWeaponUp();
        }
    }

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

    private void AddHeat(float heat)
    {
        heatValues[currWeaponIndex] += heat;
    }

    private void UpdateWeaponProperties()
    {
        fireInterface = activeWeapon.GetComponent<IWeaponFire>() as IWeaponFire;
        heatPerShot = fireInterface.GetHeatPerShot();
        WeaponProperties wp = activeWeapon.GetComponent<WeaponProperties>();
        if (wp.twoHanded)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 1f);
        }
        else
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Weapon Carry"), 0f);
        }
    }

    public void ChangeWeapon()
    {
        UnaimedWeapons.transform.GetChild(currWeaponIndex).gameObject.SetActive(false);
        activeWeapon.SetActive(false);
        currWeaponIndex = ++ currWeaponIndex % numWeapons;
        UnaimedWeapons.transform.GetChild(currWeaponIndex).gameObject.SetActive(true);
        activeWeapon = AimedWeapons.transform.GetChild(currWeaponIndex).gameObject;
        activeWeapon.SetActive(true);
        UpdateWeaponProperties();
    }

    // Start is called before the first frame update
    void Start()
    {
        aimOn = new UnityAction(AimOn);
        aimOff = new UnityAction(AimOff);
        midAir = new UnityAction(MidAir);
        EventManager.StartListening<AimingEvent>(aimOn);
        EventManager.StartListening<FreeRoamEvent>(aimOff);
        EventManager.StartListening<SprintEvent>(aimOff);
        EventManager.StartListening<JumpEvent>(midAir);
        EventManager.StartListening<FallingEvent>(midAir);
        EventManager.StartListening<PlayerControl.PlayerDeathEvent>(midAir);
        EventManager.StartListening<ThrowStartEvent>(new UnityAction(DisableFire));
        EventManager.StartListening<ThrowEndEvent>(new UnityAction(EnableFire));
        EventManager.StartListening<WeaponHeatEvent, float>(new UnityAction<float>(AddHeat));

        fireEnabled = true;
        numWeapons = AimedWeapons.transform.childCount;
        heatValues = new float[numWeapons];
        overheatRef.MaxHeat = maxHeat;
        for (int i = 0; i < numWeapons; i++)
        {
            heatValues[i] = 0;
            UnaimedWeapons.transform.GetChild(currWeaponIndex).gameObject.SetActive(false);
            AimedWeapons.transform.GetChild(currWeaponIndex).gameObject.SetActive(false);
        }
        currWeaponIndex = 0;
        UnaimedWeapons.transform.GetChild(currWeaponIndex).gameObject.SetActive(true);
        activeWeapon = AimedWeapons.transform.GetChild(currWeaponIndex).gameObject;
        activeWeapon.SetActive(true);
        UpdateWeaponProperties();
    }

    void Update()
    {
        for (int i = 0; i < numWeapons; i++)
        {
            if (heatValues[i] > 0)
            {
                heatValues[i] -= heatDecayRate * Time.deltaTime;
            }
        }
        overheatRef.CurrHeat = heatValues[currWeaponIndex];
    }
}
