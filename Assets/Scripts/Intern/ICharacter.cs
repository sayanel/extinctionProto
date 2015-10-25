using UnityEngine;
using System.Collections;

//Interface for a character
//a character is an animated entity which move and act on the world. 
//own generic functions to act on the world
public interface ICharacter
{
    void attack();
    void Move(Vector3 position);
    void pickUp();
}
