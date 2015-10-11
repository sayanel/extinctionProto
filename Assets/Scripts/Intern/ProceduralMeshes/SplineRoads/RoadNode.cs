using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadNode : MonoBehaviour
{
    Mesh m_mesh;

    // property roadWidth //
    [SerializeField]
    float m_roadWidth = 1;
    public float RoadWidth {
        get { return m_roadWidth; }
        set { m_roadWidth = value; }
    }
    // endProperty //

    // property roadHeight //
    [SerializeField]
    float m_roadHeight = 1;
    public float RoadHeight
    {
        get { return m_roadHeight; }
        set { m_roadHeight = value; }
    }
    // endProperty //

    [SerializeField]
    //List<Transform> m_anchorPoints = new List<Transform>();
    Dictionary<RoadAnchor.AnchorNames, Transform> m_anchorPoints = new Dictionary<RoadAnchor.AnchorNames, Transform>();

    //return an anchor, based on the AnchorName (ie : anchor position). If no anchor found, return null
    public Transform getAnchor( RoadAnchor.AnchorNames anchorName)
    {
        if( m_anchorPoints.ContainsKey( anchorName ) )
            return m_anchorPoints[anchorName];
        else
            return null;
        /*
        switch( anchorName )
        {
            case AnchorNames.CENTER:
                return getCenter();
                break;
            case AnchorNames.TOP:
                return getTop();
                break;
            case AnchorNames.BOTTOM:
                return getBottom();
                break;
            case AnchorNames.LEFT:
                return getLeft();
                break;
            case AnchorNames.RIGHT:
                return getRight();
                break;
            default:
                return getCenter();
                break;
        }*/
    }

    
    public Transform getCenter()
    {
        return m_anchorPoints[RoadAnchor.AnchorNames.CENTER];
    }
    public Transform getTop()
    {
        return m_anchorPoints[RoadAnchor.AnchorNames.TOP];
    }
    public Transform getBottom()
    {
        return m_anchorPoints[RoadAnchor.AnchorNames.BOTTOM];
    }
    public Transform getLeft()
    {
        return m_anchorPoints[RoadAnchor.AnchorNames.LEFT];
    }
    public Transform getRight()
    {
        return m_anchorPoints[RoadAnchor.AnchorNames.RIGHT];
    }


    public void Init()
    {
        //generate anchors :
        initAnchors();
    }

    //create 5 anchors (Transform) and place them on the extremities of the node
    void initAnchors()
    {
        m_anchorPoints.Clear();
        //search for the RoadAnchor which are child of this node
        RoadAnchor[] roadAnchors = GetComponentsInChildren<RoadAnchor>();
        //put the transform of the RoadAnchors found in the map (once put in the map, we need onlyt the Transform of the RoadAnchor, not the RoadAnchor itself)
        foreach(RoadAnchor anchor in roadAnchors)
        {
            m_anchorPoints[anchor.AnchorName] = anchor.transform;
        }
    }

    //set the material of the road
    public void setMaterial( Material material )
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }

    public void setMaterial( Material roadMaterial, Material pavementMaterial )
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();
        if(filter.sharedMesh.subMeshCount > 1)
        {
            Material[] materials = { roadMaterial, pavementMaterial };
            renderer.sharedMaterials = materials;
        }
        else
        {
            renderer.sharedMaterial = roadMaterial;
        }
        
    }

}
