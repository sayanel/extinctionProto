using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponEvent : MonoBehaviour, IWeaponEvent
{
    [SerializeField]
    private bool m_hasParticleSystem = false;

    [SerializeField]
    private bool m_hasLight = false;

    [SerializeField]
    private bool m_hasSound = false;

    /// <summary>
    /// Delay before the light switch off
    /// </summary>
    [SerializeField]
    private float m_lightDelay = 0.1F;

    private ParticleSystem m_thisParticleSystem;
    private Light m_thisLight;
    private AudioSource m_thisAudioSource;


    void Awake()
	{
        if( m_hasParticleSystem )
        {
            m_thisParticleSystem = GetComponent<ParticleSystem>();
            if( m_thisParticleSystem == null )
                m_hasParticleSystem = false;
        }

        if( m_hasLight )
        {
            m_thisLight = GetComponent<Light>();
            if( m_thisLight == null )
                m_hasLight = false;
        }

        if( m_hasSound )
        {
            m_thisAudioSource = GetComponent<AudioSource>();
            if( m_thisAudioSource == null )
                m_hasSound = false;
        }
    }

    public void OnFire()
    {
        //wake up components and play effects 
        if( m_hasSound )
        {
            m_thisAudioSource.enabled = true;
            m_thisAudioSource.Play();
        }
        if( m_hasLight )
        {
            m_thisLight.enabled = true;
            StartCoroutine( animLightFlare_on() );
        }
        if( m_hasParticleSystem )
        {
            m_thisParticleSystem.enableEmission = true;
            m_thisParticleSystem.Play();
        }

        //sleep components afters a delay
        StartCoroutine(DestroyAfterSeconds(1));
    }

    IEnumerator DestroyAfterSeconds(float delta)
    {
        yield return new WaitForSeconds( delta );
        
        m_thisAudioSource.enabled = false;
        m_thisLight.enabled = false;
        m_thisParticleSystem.enableEmission = false;
    }

    IEnumerator animLightFlare_on()
    {
        m_thisLight.intensity = 10;

        yield return new WaitForSeconds( m_lightDelay );

        animLightFlare_off();
    }

    void animLightFlare_off()
    {
        m_thisLight.intensity = 0;
    }

}
