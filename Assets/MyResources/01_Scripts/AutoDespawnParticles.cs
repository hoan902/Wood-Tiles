using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DESPAWN_TYPE
{
    WHEN_FINISH = 0,
    TIMER
}

[RequireComponent(typeof(ParticleSystem))]
public class AutoDespawnParticles : MonoBehaviour
{
    public DESPAWN_TYPE type = DESPAWN_TYPE.WHEN_FINISH;
    public float DelayDespawn = 3f;

    private float timerDespawn = 0f;
    public ParticleSystem ps { get; private set; }


    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        timerDespawn = 0f;
    }

    private void OnEnable()
    {
        timerDespawn = 0f;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (ps)
        {
            if (type == DESPAWN_TYPE.TIMER)
            {
                timerDespawn += deltaTime;
                if (timerDespawn >= DelayDespawn)
                {
                    timerDespawn = 0f;
                    Lean.LeanPool.Despawn(gameObject);
                }
            }
            else if (type == DESPAWN_TYPE.WHEN_FINISH)
            {
                if (!ps.IsAlive())
                {
                    Lean.LeanPool.Despawn(gameObject);
                }
            }

        }
    }

    public void PlayEffect()
    {
        timerDespawn = 0f;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }

    public void StopEffect()
    {
        ps.time = 0;
        ps.Stop(true);
        timerDespawn = 0;
    }

    public void ResetPar()
    {
        timerDespawn = 0f;
    }

    public void CleanUp()
    {
        ps.time = 0;
        ps.Stop(true);
        Lean.LeanPool.Despawn(gameObject);
    }

    


}
