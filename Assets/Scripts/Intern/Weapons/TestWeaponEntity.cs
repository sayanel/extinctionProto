using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestWeaponEntity : MonoBehaviour 
{
    Weapon m_thisWeapon;

	void Awake()
	{
        m_thisWeapon = GetComponent<Weapon>();
    }
	
	void Update() 
	{
	    if(Input.GetMouseButtonDown(0))
        {
            m_thisWeapon.Fire();
        }
	}
}
