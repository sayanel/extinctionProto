using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestWeaponEntity : MonoBehaviour 
{
    [SerializeField]
    private Weapon m_thisWeapon;


	void Update() 
	{
	    if(Input.GetMouseButtonDown(0))
        {
            m_thisWeapon.Fire();
        }
	}
}
