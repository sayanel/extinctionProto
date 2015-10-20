using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface that a destructible object should implements. 
/// The interface allows you to check is this object is alive.
/// </summary>
public interface IDestructible
{
    bool isAlive();
    void destroy();
}
