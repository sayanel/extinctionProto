using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface defining a target, which can take dammage.
/// </summary>
public interface ITargetable
{
    void TakeDammage(float amount);
}
