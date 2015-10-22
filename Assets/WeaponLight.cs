using UnityEngine;
using System.Collections;

/// <summary>
/// Class that is used to turn the light on when firing
/// </summary>
public class WeaponLight : MonoBehaviour, IWeaponEvent {

    public float m_intensity = 4.6f;
    public float m_time = 0.5f;

    private Light m_light;
    
	void Start () {
        m_light = GetComponent<Light>();
        m_light.intensity = m_intensity;
        m_light.enabled = false;
	}
	
    public void OnFire()
    {
        StartCoroutine( "FireLight" );
    }

    IEnumerator FireLight()
    {
        m_light.enabled = true;
        
        yield return new WaitForSeconds( m_time );

        m_light.enabled = false;
    }
}
