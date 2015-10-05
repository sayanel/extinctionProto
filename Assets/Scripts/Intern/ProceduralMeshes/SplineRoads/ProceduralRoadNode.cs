using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralRoadNode : AProceduralMesh
{
    Mesh m_mesh;

    // property roadWidth //
    [SerializeField]
    float m_roadWidth = 1;
    public float RoadWidth
    {
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

    // property curvature //
    [SerializeField]
    float m_curvature = 0.1f;
    public float Curvature
    {
        get { return m_curvature; }
        set { m_curvature = value; }
    }
    // endProperty curvature //

    [SerializeField]
    List<Transform> m_anchorPoints = new List<Transform>();

    //return an anchor, based on the AnchorName (ie : anchor position). If no anchor found, return null
    public Transform getAnchor( RoadAnchor.AnchorNames anchorName )
    {
        switch( anchorName )
        {
            case RoadAnchor.AnchorNames.CENTER:
                return getCenter();
                break;
            case RoadAnchor.AnchorNames.TOP:
                return getTop();
                break;
            case RoadAnchor.AnchorNames.BOTTOM:
                return getBottom();
                break;
            case RoadAnchor.AnchorNames.LEFT:
                return getLeft();
                break;
            case RoadAnchor.AnchorNames.RIGHT:
                return getRight();
                break;
            default:
                return getCenter();
                break;
        }
    }


    public Transform getCenter()
    {
        return m_anchorPoints[0];
    }
    public Transform getTop()
    {
        return m_anchorPoints[1];
    }
    public Transform getBottom()
    {
        return m_anchorPoints[2];
    }
    public Transform getLeft()
    {
        return m_anchorPoints[3];
    }
    public Transform getRight()
    {
        return m_anchorPoints[4];
    }


    public override void Init()
    {

        //Mesh initialization
        if( m_mesh == null )
        {
            m_mesh = new Mesh();
            m_mesh.name = "roadNode";
        }
        else
        {
            m_mesh.Clear( false );
        }

        //get meshFilter
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if( meshFilter == null )
            meshFilter = gameObject.AddComponent<MeshFilter>();
        //get meshCollider
        MeshCollider meshCollider = this.GetComponent<MeshCollider>();
        if( meshCollider == null )
            meshCollider = gameObject.AddComponent<MeshCollider>();
        //add mesh filter if needed
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        if( meshRenderer == null )
            gameObject.AddComponent<MeshRenderer>();

        //link the new mesh to the GameObject
        meshFilter.sharedMesh = m_mesh;
        meshCollider.sharedMesh = m_mesh;

        //generate anchors :
        initAnchors();
    }

    //create 5 anchors (Transform) and place them on the extremities of the node
    void initAnchors()
    {

        //delete previous anchors
        for( int i = 0; i < m_anchorPoints.Count; i++ )
        {
            DestroyImmediate( m_anchorPoints[i].gameObject );
        }
        m_anchorPoints.Clear();

        //create new anchors
        Transform center = new GameObject( "anchorCenter" ).transform;
        center.SetParent( this.transform );
        center.localPosition = new Vector3( 0, 0, 0 );
        m_anchorPoints.Add( center );

        Transform top = new GameObject( "anchorTop" ).transform;
        top.SetParent( this.transform );
        top.localPosition = new Vector3( 0, 0, m_roadWidth + m_curvature );
        m_anchorPoints.Add( top );

        Transform bottom = new GameObject( "anchorBottom" ).transform;
        bottom.SetParent( this.transform );
        bottom.localPosition = new Vector3( 0, 0, -( m_roadWidth + m_curvature ) );
        m_anchorPoints.Add( bottom );

        Transform left = new GameObject( "anchorLeft" ).transform;
        left.SetParent( this.transform );
        left.localPosition = new Vector3( -( m_roadWidth + m_curvature ), 0, 0 );
        m_anchorPoints.Add( left );

        Transform right = new GameObject( "anchorRight" ).transform;
        right.SetParent( this.transform );
        right.localPosition = new Vector3( m_roadWidth + m_curvature, 0, 0 );
        m_anchorPoints.Add( right );


    }

    //set the material of the road
    public void setMaterial( Material material )
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }

    //construct the shape of the node
    public override void Generate()
    {
        //Init();

        ConstructShape();
    }

    public void ConstructShape()
    {
        //global to local
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        int nbOfvertices = 20 + 16 * 2;
        int nbOfTriangles = ( 18 + 32 ) * 3;

        Vector3[] vertices = new Vector3[nbOfvertices];
        Vector2[] uvMap = new Vector2[nbOfvertices];
        Vector3[] normals = new Vector3[nbOfvertices];
        int[] triangles = new int[nbOfTriangles];

        int t = 0; // triangle index
        int v = 0; // vertex index
        int n = 0; //normal index

        float l = m_curvature;
        float w = m_roadWidth;
        float L = w + l;

        vertices[v++] = new Vector3( -w, 0, w + l );
        vertices[v++] = new Vector3( -w, 0, w );
        vertices[v++] = new Vector3( -w - l, 0, w );
        vertices[v++] = new Vector3( -w - l, 0, -w );
        vertices[v++] = new Vector3( -w, 0, -w );
        vertices[v++] = new Vector3( -w, 0, -w - l );
        vertices[v++] = new Vector3( w, 0, -w - l );
        vertices[v++] = new Vector3( w, 0, -w );
        vertices[v++] = new Vector3( w + l, 0, -w );
        vertices[v++] = new Vector3( w + l, 0, w );
        vertices[v++] = new Vector3( w, 0, w );
        vertices[v++] = new Vector3( w, 0, w + l );

        int[] signX = { -1, -1, 1, 1 };
        int[] signY = { -1, 1, -1, 1 };
        int[] points = new int[2];

        triangles[t++] = 1;
        vertices[v] = new Vector3( -0.13f, 0, 0.5f ) + vertices[1];
        triangles[t++] = v;
        v++;
        triangles[t++] = 0;

        triangles[t++] = 1;
        vertices[v] = new Vector3( -0.5f, 0, 0.13f ) + vertices[1];
        triangles[t++] = v;
        v++;
        triangles[t++] = v - 2;

        triangles[t++] = 1;
        triangles[t++] = 2;
        triangles[t++] = v - 1;

        triangles[t++] = 4;
        vertices[v] = new Vector3( -0.5f, 0, -0.13f ) + vertices[4];
        triangles[t++] = v;
        v++;
        triangles[t++] = 3;

        triangles[t++] = 4;
        vertices[v] = new Vector3( -0.13f, 0, -0.5f ) + vertices[4];
        triangles[t++] = v;
        v++;
        triangles[t++] = v - 2;

        triangles[t++] = 4;
        triangles[t++] = 5;
        triangles[t++] = v - 1;

        triangles[t++] = 7;
        vertices[v] = new Vector3( 0.13f, 0, -0.5f ) + vertices[7];
        triangles[t++] = v;
        v++;
        triangles[t++] = 6;

        triangles[t++] = 7;
        vertices[v] = new Vector3( 0.5f, 0, -0.13f ) + vertices[7];
        triangles[t++] = v;
        v++;
        triangles[t++] = v - 2;

        triangles[t++] = 7;
        triangles[t++] = 8;
        triangles[t++] = v - 1;

        triangles[t++] = 10;
        vertices[v] = new Vector3( 0.5f, 0, 0.13f ) + vertices[10];
        triangles[t++] = v;
        v++;
        triangles[t++] = 9;

        triangles[t++] = 10;
        vertices[v] = new Vector3( 0.13f, 0, 0.5f ) + vertices[10];
        triangles[t++] = v;
        v++;
        triangles[t++] = v - 2;

        triangles[t++] = 10;
        triangles[t++] = 11;
        triangles[t++] = v - 1;


        //for(int p = 0; p < 4; p++)
        //{
        //    for(int i = 1; i < 3;  i++)
        //    {
        //        vertices[v] = new Vector3( (signX[p])*(L-Mathf.Cos(Mathf.PI *i / 6.0f)*l), 0, (signY[p])*(L-Mathf.Sin(Mathf.PI * i / 6.0f)*l) );
        //        points[i-1] = v;
        //        v++;
        //    }
        //    if( (signX[p] < 0 && signY[p] > 0 )|| ( signX[p] > 0 && signY[p] > 0 ) )
        //    {
        //        triangles[t++] = p * 3;
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = points[0];

        //        triangles[t++] = points[0];
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = points[1];

        //        triangles[t++] = points[1];
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = p * 3 + 2;
        //    }
        //    else
        //    {
        //        triangles[t++] = points[1];
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = p * 3 + 2;

        //        triangles[t++] = points[0];
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = points[1];

        //        triangles[t++] = p * 3;
        //        triangles[t++] = p * 3 + 1; // indices in vertices
        //        triangles[t++] = points[0];
        //    }

        //}

        triangles[t++] = 6;
        triangles[t++] = 5;
        triangles[t++] = 4;

        triangles[t++] = 7;
        triangles[t++] = 6;
        triangles[t++] = 4;

        triangles[t++] = 10;
        triangles[t++] = 1;
        triangles[t++] = 0;

        triangles[t++] = 11;
        triangles[t++] = 10;
        triangles[t++] = 0;

        triangles[t++] = 8;
        triangles[t++] = 3;
        triangles[t++] = 2;

        triangles[t++] = 9;
        triangles[t++] = 8;
        triangles[t++] = 2;

        for( int i = 0; i < 20; i++ )
        {
            normals[n++] = new Vector3( 0, 1, 0 );
        }

        int[] boundIndices = { 11, 0, 12, 13, 2, 3, 14, 15, 5, 6, 16, 17, 8, 9, 18, 19, 11 };

        // volume : 
        for( int i = 0; i < 20; ++i )
        {
            if( i != 1 && i != 4 && i != 7 && i != 10 )
            {
                vertices[v] = vertices[( v - 20 )];
                v++;
                vertices[v] = vertices[( v - 20 )] - new Vector3( 0, m_roadHeight, 0 );
                v++;

                if( i == 0 || i == 11 )
                {
                    normals[n++] = new Vector3( 0, 0, 1 );
                    normals[n++] = new Vector3( 0, 0, 1 );
                }
                else if( i == 2 || i == 3 )
                {
                    normals[n++] = new Vector3( -1, 0, 0 );
                    normals[n++] = new Vector3( -1, 0, 0 );
                }
                else if( i == 5 || i == 6 )
                {
                    normals[n++] = new Vector3( 0, 0, -1 );
                    normals[n++] = new Vector3( 0, 0, -1 );
                }
                else if( i == 8 || i == 9 )
                {
                    normals[n++] = new Vector3( 1, 0, 0 );
                    normals[n++] = new Vector3( 1, 0, 0 );
                }
                else
                {
                    normals[n++] = vertices[v - 20];
                    normals[n++] = vertices[v - 20];
                }

            }
        }

        for( int i = 0; i < 16; i++ )
        {
            triangles[t++] = boundIndices[i] + 1;
            triangles[t++] = boundIndices[i] + 20;
            triangles[t++] = boundIndices[i];

            triangles[t++] = boundIndices[i] + 19;
            triangles[t++] = boundIndices[i] + 1;
            triangles[t++] = boundIndices[i] + 20;
        }

        //finallize ; 
        m_mesh.Clear();

        m_mesh.vertices = vertices;
        m_mesh.normals = normals;
        m_mesh.uv = uvMap;
        m_mesh.triangles = triangles;

        m_mesh.RecalculateBounds();
        //m_mesh.RecalculateNormals();

        transform.position = savedPosition;
        transform.rotation = savedRotation;
    }


}

