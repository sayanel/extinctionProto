using UnityEngine;
using System.Collections;

//entities who implement selectable have to provide few elements to be render on a gui.
public interface ISelectable
{
    Sprite getIcone();
    string getName();
    string getDescription();
}
