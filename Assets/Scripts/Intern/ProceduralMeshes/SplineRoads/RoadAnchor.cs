using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadAnchor : MonoBehaviour 
{
    public enum AnchorNames { CENTER, TOP, BOTTOM, LEFT, RIGHT };

    [SerializeField]
    AnchorNames m_anchorName = AnchorNames.CENTER;
    public AnchorNames AnchorName
    {
        get {return m_anchorName; }
    }

}
