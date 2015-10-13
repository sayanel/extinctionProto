using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficLight : MonoBehaviour
{
    private enum LightBehaviour { RANDOM, ONLY_ONE, STABLE}; //the behaviour of the lights : -Random : lighting is completly random between the 3 lights, -Only_one : only one lights is on, -Stable : alternative lighting between the 3 lights

    [SerializeField]
    private float m_lightingDelay = 2; //the delay beteew two flashs

    [SerializeField]
    LightBehaviour m_lightBehaviour; //the behaviour of the lights.

    [SerializeField]
    private List<Light> m_lights; //references to the lights in the sceen.

    [SerializeField]
    private int m_currentLight = 0; //current light on

    [SerializeField]
    private bool m_active; //is the lighting effect active ? 
    public bool Active {
        get { return m_active; }
        set { m_active = value;
            if( m_active )
            {
                startLightBehaviour();
            }
        }
    }

    [SerializeField]
    private float m_intensity; //intensity of the light when it's on

    [SerializeField]
    private bool m_useFlare; // is the lighting system using flare ? 

    [SerializeField]
    Flare m_flare; // if m_useFlare is true, use this flare on the lights. 

    private IEnumerator m_coroutine_SwitchLighting; //simply store the current lighting coroutine (in order to properly stop it)


    void Start()
    {
        //start the animation
        if( m_active )
            startLightBehaviour();
    }

    //choose the right coroutine, based on the current behaviour, and play it
    void startLightBehaviour()
    {
        if(m_coroutine_SwitchLighting != null)
            StopCoroutine( m_coroutine_SwitchLighting ); // stop the current lightBehaviour

        switch(m_lightBehaviour)
        {
            case LightBehaviour.RANDOM:
                m_coroutine_SwitchLighting = switchLighting_random();
                break;
            case LightBehaviour.ONLY_ONE:
                m_coroutine_SwitchLighting = switchLighting_onlyOne();
                break;
            case LightBehaviour.STABLE:
                m_coroutine_SwitchLighting = switchLighting_stable();
                break;
        }
        StartCoroutine( m_coroutine_SwitchLighting );
    }
    void startLightBehaviour(LightBehaviour behaviour)
    {
        m_lightBehaviour = behaviour;
        startLightBehaviour();
    }

    //switch light on
    void switchOn() //applied on m_currentLight
    {
        m_lights[m_currentLight].intensity = m_intensity;
        if(m_useFlare)
        m_lights[m_currentLight].flare = m_flare;
    }
    void switchOn(int i)
    {
        m_lights[i].intensity = m_intensity;
        if(m_useFlare)
        m_lights[i].flare = m_flare;
    }

    //switch light off
    void switchOff() //applied on m_currentLight
    {
        m_lights[m_currentLight].intensity = 0;
        m_lights[m_currentLight].flare = null;
    }
    void switchOff(int i)
    {
        m_lights[i].intensity = 0;
        m_lights[i].flare = null;
    }

    IEnumerator switchLighting_random()
    {
        while( m_active )
        {
            switchOff();
            m_currentLight = Random.Range( 0, 3 );
            switchOn();

            yield return new WaitForSeconds( m_lightingDelay );

        }
    }

    IEnumerator switchLighting_onlyOne()
    {
        while( m_active )
        {

            if( m_lights[m_currentLight].intensity < 0.2F )
                switchOn();
            else
                switchOff();

            yield return new WaitForSeconds( m_lightingDelay );

        }
    }

    IEnumerator switchLighting_stable()
    {
        while(m_active)
        {

            switchOff();
            if( ++m_currentLight > 2 )
                m_currentLight = 0;
            switchOn();

            yield return new WaitForSeconds( 2 );

        }
    }

}
