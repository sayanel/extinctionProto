using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface that should be implemented for every Component linked to a Weapon Event
/// example : a light turned on when firing
/// </summary>
public interface IWeaponEvent
{
    /// <summary>
    /// This method is called when a weapon is shooting
    /// </summary>
    void OnFire();
}
