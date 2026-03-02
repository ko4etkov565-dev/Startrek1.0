using System;
using UnityEngine;

public class LanderVisuals : MonoBehaviour
{
  
  [SerializeField] private ParticleSystem leftThrusterParticleSystem;
  [SerializeField] private ParticleSystem middleThrusterParticleSystem;
  [SerializeField] private ParticleSystem rightThrusterParticleSystem;
  [SerializeField] private GameObject landerExplosionVfx;


    
    private Lander lander;
    private void Awake()
    {
      lander = GetComponent<Lander>();  

    // lander.OnUpForce += Lander_OnUpForce;
    // lander.OnLeftForce += Lander_OnLeftForce;
    // lander.OnRightForce += Lander_OnRightForce;
    // lander.OnBeforeForce += Lander_OnBeforeForce;   

    // //   on a start it(three fire) doesnt work
    //     SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
    //     SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, false);
    //     SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Start(){
    
    lander.OnUpForce += Lander_OnUpForce;
    lander.OnLeftForce += Lander_OnLeftForce;
    lander.OnRightForce += Lander_OnRightForce;
    lander.OnBeforeForce += Lander_OnBeforeForce;  
    lander.OnLanded += Lander_OnLanded; 

    //   on a start it(three fire) doesnt work
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
        
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)    {
       switch (e.landingType)        {
           case Lander.LandingType.TooFastLanding:
           case Lander.LandingType.TooSteepAngle:
           case Lander.LandingType.WrongLandingArea:
            //    CRASH
            Instantiate(landerExplosionVfx, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            break;
        }
    }

    private void Lander_OnBeforeForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Lander_OnRightForce(object sender, EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
        
    }

    private void Lander_OnLeftForce(object sender, EventArgs e)
    {
       
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void Lander_OnUpForce(object sender, EventArgs e)
    {
        // all fire if you press up 
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled; 
    }


     

    private void OnDestroy()
    {
        if (lander != null)
        {
            lander.OnUpForce -= Lander_OnUpForce;
            lander.OnLeftForce -= Lander_OnLeftForce;
            lander.OnRightForce -= Lander_OnRightForce;
            lander.OnBeforeForce -= Lander_OnBeforeForce;
            lander.OnLanded -= Lander_OnLanded;
        }
    }
    
}
