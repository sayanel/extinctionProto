using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierRoad : AProceduralMesh
{
    public enum ShapeType { SIMPLE, COMPLEXE };

    [Header( "Components" )]
    [SerializeField]
    Bezier m_spline;

    [SerializeField]
    Mesh m_mesh;

    [Header( "Utils" )]
    // property autoGenerate
    [SerializeField]
    bool m_autoGenerate = false;
    public bool AutoGenerate
    {
        get { return m_autoGenerate; }
    }
    // endProperty autoGenerate//

    [Header( "Main Mesh Attributs" )]
    // property shapeType
    [SerializeField]
    ShapeType m_shapeType;
    public ShapeType Shape{
        get { return m_shapeType; }
        set { m_shapeType = value; }
    }
    // endProperty shapeType

    // property pavementWidth
    [SerializeField]
    [Range( 0, 1 )]
    float m_pavementWidth = 1;
    public float PavementWidth {
        get { return m_pavementWidth; }
        set { m_pavementWidth = value; }
    }
    //endProperty pavementWidth

    //property pavementHeight
    [SerializeField]
    [Range( 0, 5 )]
    float m_pavementHeight = 0;
    public float PavementHeight {
        get { return m_pavementHeight; }
        set { m_pavementHeight = value; }
    }
    //endProperty pavementHeight

    // property meshSubdivision //
    [SerializeField]
    [Range( 1, 10 )]
    int m_meshSubdivision = 10;
    public int MeshSubdivision
    {
        get { return m_meshSubdivision; }
        set { m_meshSubdivision = value; }
    }
    // endProperty meshSubdivision //

    // property uvRepetition
    [SerializeField]
    Vector2 m_UVRepetition = new Vector2( 1F, 1F );
    public Vector2 UVRepetition {
        get {return m_UVRepetition; }
        set { m_UVRepetition = value; }
    }
    // endProperty uvRepetition//

    // property uvRepetition
    [SerializeField]
    Vector2 m_UVRepetitionPavement = new Vector2( 1F, 1F );
    public Vector2 UVRepetitionPavement
    {
        get { return m_UVRepetitionPavement; }
        set { m_UVRepetitionPavement = value; }
    }
    // endProperty uvRepetition//

    [SerializeField]
    [Range( 0, 10 )]
    int m_seed = 1;

    [SerializeField]
    float m_height = 1;

    [SerializeField]
    float m_width01 = 1;

    [SerializeField]
    float m_width02 = 1;
    

    public override void Init()
    {
        if( m_spline == null )
        {
            m_spline = ScriptableObject.CreateInstance<Bezier>();
        }

        if( m_mesh == null )
        {
            m_mesh = new Mesh();
            m_mesh.name = "road";
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

    }

    //set the 4 control points of the bezier curve
    public void setControlPoints(Vector3 v1, Vector3 v2 , Vector3 v3 , Vector3 v4 )
    {
        m_spline.setControlPoints( v1, v2, v3, v4 );
    }

    //set the width of all control points 
    public void setRoadwidth( float width )
    {
        m_width01 = width;
        m_width02 = width;
    }

    //generate a new Mesh. If the GameObjet isn't initialyze, it will be initialysed. 
    public override void Generate()
    {
        if( m_mesh == null )
            Init();
        if( m_shapeType == ShapeType.SIMPLE )
            buildSimpleMesh();
        else
            buildComplexMesh();

        //resolve a small bug : set the right center of each roads.
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<MeshCollider>().enabled = true;
    }

    Vector3 getSplinePoint( int index )
    {
        return m_spline.GetPointAtTime( (float)index/(float)m_meshSubdivision );
    }

    Vector3 getSplineDirection( int index )
    {
        return m_spline.GetDirectionAtTime( (float)index / (float)m_meshSubdivision, 1.0f/(float)m_meshSubdivision );
    }

    void generateRoad( ref int i, ref int p, ref int c, ref int v, ref float a, ref Vector3[] newVertices, ref Vector3[] newNormals, ref Vector2[] newUV, ref int[] newTriangles, int subMeshIndice, float decalX, float decalY )
    {
        Vector3 splinePoint_A = getSplinePoint( p ); //m_spline.getSplinePoint( c, a );
        Vector3 splinePointDirection_A = getSplineDirection( p ); //m_spline.getSplinePointDirection( c, a );
        Vector3 splinePointNormal_A = Vector3.Cross( transform.up, splinePointDirection_A ).normalized;

        Vector3 splinePoint_B = getSplinePoint( p + 1 ); //m_spline.getSplinePoint( c, a + splineStep );
        Vector3 splinePointDirection_B = getSplineDirection( p + 1 ); //m_spline.getSplinePointDirection( c, a + splineStep );
        Vector3 splinePointNormal_B = Vector3.Cross( transform.up, splinePointDirection_B ).normalized;

        float previousHeight = m_height;
        float roadWeight = m_width01;  Mathf.Lerp( m_width01, m_width02, ( (float)p / m_meshSubdivision ) );
        float roadWeight01 = m_width02;  Mathf.Lerp( m_width01, m_width02, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
        float currentWeight = 0;
        float currentWeight01 = 0;
        Vector2 uvRepetition;
        if( subMeshIndice == 0 )
        {
            currentWeight = Mathf.Lerp( m_width01, m_width02, ( (float)p / m_meshSubdivision ) );
            currentWeight01 = Mathf.Lerp( m_width01, m_width02, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
            uvRepetition = m_UVRepetition;
        }
        else
        {
            currentWeight = m_pavementWidth * Mathf.Lerp( m_width01, m_width02, ( (float)p / m_meshSubdivision ) );
            currentWeight01 = m_pavementWidth * Mathf.Lerp( m_width01, m_width02, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
            uvRepetition = m_UVRepetitionPavement;

            m_height += decalY;
        }

        int T0 = 0, T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0;
        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A + splinePointNormal_A * ( decalX * ( roadWeight + currentWeight )) + decalY*Vector3.up;
        newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A );
        newUV[v] = new Vector2( -uvRepetition.x * 0.45f + 0.5f, p * uvRepetition.y );
        T0 = v;
        v++;

        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A + splinePointNormal_A * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A ).normalized;
        newUV[v] = new Vector2( uvRepetition.x * 0.45f + 0.5f, p * uvRepetition.y );
        T1 = v;
        v++;

        newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B + splinePointNormal_B * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
        newUV[v] = new Vector2( uvRepetition.x * 0.45f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T2 = v;
        v++;

        newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B + splinePointNormal_B * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
        newUV[v] = new Vector2( -uvRepetition.x * 0.45f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T3 = v;
        v++;

        //height
        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A - new Vector3( 0, m_height, 0 ) + splinePointNormal_A * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = new Vector3( 1, 0, 0 );
        newUV[v] = new Vector2( -uvRepetition.x * 0.5f + 0.5f, p * uvRepetition.y );
        T4 = v;
        v++;

        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A - new Vector3( 0, m_height, 0 ) + splinePointNormal_A * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = new Vector3( 1, 0, 0 );
        newUV[v] = new Vector2( uvRepetition.x * 0.5f + 0.5f, p * uvRepetition.y );
        T5 = v;
        v++;

        newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B - new Vector3( 0, m_height, 0 ) + splinePointNormal_B * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = new Vector3( -1, 0, 0 );
        newUV[v] = new Vector2( uvRepetition.x * 0.5f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T6 = v;
        v++;

        newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B - new Vector3( 0, m_height, 0 ) + splinePointNormal_B * ( decalX * ( roadWeight + currentWeight ) ) + decalY * Vector3.up;
        newNormals[v] = new Vector3( -1, 0, 0 );
        newUV[v] = new Vector2( -uvRepetition.x * 0.5f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T7 = v;
        v++;


        newTriangles[i++] = T1;
        newTriangles[i++] = T0;
        newTriangles[i++] = T2;
        newTriangles[i++] = T2;
        newTriangles[i++] = T0;
        newTriangles[i++] = T3;

        newTriangles[i++] = T0;
        newTriangles[i++] = T4;
        newTriangles[i++] = T3;
        newTriangles[i++] = T3;
        newTriangles[i++] = T4;
        newTriangles[i++] = T7;

        newTriangles[i++] = T1;
        newTriangles[i++] = T2;
        newTriangles[i++] = T6;
        newTriangles[i++] = T1;
        newTriangles[i++] = T6;
        newTriangles[i++] = T5;


        //a += splineStep;
        p++;
        if( p % m_meshSubdivision == 0 )
            c++;

        m_height = previousHeight;
    }

    Mesh buildComplexMesh()
    {
        //global to local
        //Vector3 savedPosition = transform.position;
        //Quaternion savedRotation = transform.rotation;
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.identity;

        //Mesh initialisation
        int nbOfTriangle = ( m_meshSubdivision * 6 * 3 ) * 3;
        int nbOfPoints = ( m_meshSubdivision * 8 ) * 3;

        Vector3[] newVertices = new Vector3[nbOfPoints];
        Vector2[] newUV = new Vector2[nbOfPoints];
        Vector3[] newNormals = new Vector3[nbOfPoints];

        int[] newTriangles01 = new int[nbOfTriangle / 3];
        int[] newTriangles02 = new int[nbOfTriangle / 3];
        int[] newTriangles03 = new int[nbOfTriangle / 3];


        //Generate mesh

        int j = 0; // arondie
        int v = 0; // vertice index
        int c = 1; // controlPoint
        int p = 0;
        float a = 0F; // fraction 0 -> meshSubdivision
        float splineStep = 1F / (float)( m_meshSubdivision - 1 );
        for( int subMesh = 0; subMesh < 3; subMesh++ )
        {
            if( subMesh == 0 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles01, subMesh, 0, 0 );
                }
                p = 0;
                c = 1;
                a = 0;
            }

            else if( subMesh == 1 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles02, subMesh, -1, m_pavementHeight );
                }
                p = 0;
                c = 1;
                a = 0;
            }

            else if( subMesh == 2 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles03, subMesh, 1, m_pavementHeight );
                }
                p = 0;
                c = 1;
                a = 0;
            }
        }

        //Mesh update
        m_mesh.Clear();

        m_mesh.vertices = newVertices;
        m_mesh.normals = newNormals;
        m_mesh.uv = newUV;

        m_mesh.subMeshCount = 3;
        m_mesh.SetTriangles( newTriangles01, 0 );
        m_mesh.SetTriangles( newTriangles02, 1 );
        m_mesh.SetTriangles( newTriangles03, 2 );

        m_mesh.RecalculateBounds();
        // m_mesh.RecalculateNormals();

        //GetComponent<MeshFilter>().mesh = caveMesh;

        //local to global 
        //transform.position = savedPosition;
        //transform.rotation = savedRotation;

        return m_mesh;

    }


    Mesh buildSimpleMesh()
    {
        //global to local
        //Vector3 savedPosition = transform.position;
        //Quaternion savedRotation = transform.rotation;
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.identity;

        //Random
        int tmpSeed = Random.seed;
        Random.seed = m_seed;
        Vector2 seedVector = Vector2.zero;

        if( Random.Range( 0F, 6F ) >= 4F )
            seedVector = new Vector2( 0, m_seed );
        else if( Random.Range( 0F, 6F ) < 4F && Random.Range( 0F, 6F ) >= 2F )
            seedVector = new Vector2( m_seed, m_seed );
        else if( Random.Range( 0F, 6F ) < 2F )
            seedVector = new Vector2( m_seed, 0 );

        Random.seed = tmpSeed;

        //Mesh initialisation
        int nbOfTriangle =  m_meshSubdivision * 6 * 3;
        int nbOfPoints = m_meshSubdivision * 8;

        Vector3[] newVertices = new Vector3[nbOfPoints];
        Vector2[] newUV = new Vector2[nbOfPoints];
        Vector3[] newNormals = new Vector3[nbOfPoints];
        int[] newTriangles = new int[nbOfTriangle];

        //Generate mesh

        int j = 0; // arondie
        int v = 0; // vertice index
        int c = 1; // controlPoint
        int p = 0;
        float a = 0F; // fraction 0 -> meshSubdivision
        float splineStep = 1F / (float)( m_meshSubdivision - 1 );
        for( int i = 0; i < nbOfTriangle; ) //i : triangle index
        {

            Vector3 splinePoint_A = getSplinePoint( p ); //m_spline.getSplinePoint( c, a );
            Vector3 splinePointDirection_A = getSplineDirection( p ); //m_spline.getSplinePointDirection( c, a );
            Vector3 splinePointNormal_A = Vector3.Cross( transform.up, splinePointDirection_A ).normalized;

            Vector3 splinePoint_B = getSplinePoint( p + 1 ); //m_spline.getSplinePoint( c, a + splineStep );
            Vector3 splinePointDirection_B = getSplineDirection( p + 1 ); //m_spline.getSplinePointDirection( c, a + splineStep );
            Vector3 splinePointNormal_B = Vector3.Cross( transform.up, splinePointDirection_B ).normalized;

            float currentWeight = Mathf.Lerp( m_width01, m_width02, ( ( p % m_meshSubdivision ) / (float)m_meshSubdivision ) );
            float currentWeight01 = Mathf.Lerp( m_width01, m_width02, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );

            int T0 = 0, T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0;
            newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A;
            newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A );
            newUV[v] = new Vector2( -m_UVRepetition.x * 0.45f + 0.5f, p * m_UVRepetition.y );
            T0 = v;
            v++;

            newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A;
            newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A ).normalized;
            newUV[v] = new Vector2( m_UVRepetition.x * 0.45f + 0.5f, p * m_UVRepetition.y );
            T1 = v;
            v++;

            newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B;
            newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
            newUV[v] = new Vector2( m_UVRepetition.x * 0.45f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
            T2 = v;
            v++;

            newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B;
            newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
            newUV[v] = new Vector2( -m_UVRepetition.x * 0.45f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
            T3 = v;
            v++;

            //height
            newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A - new Vector3( 0, m_height, 0 );
            newNormals[v] = new Vector3( 1, 0, 0 );
            newUV[v] = new Vector2( -m_UVRepetition.x * 0.5f + 0.5f, p * m_UVRepetition.y );
            T4 = v;
            v++;

            newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A - new Vector3( 0, m_height, 0 );
            newNormals[v] = new Vector3( 1, 0, 0 );
            newUV[v] = new Vector2( m_UVRepetition.x * 0.5f + 0.5f, p * m_UVRepetition.y );
            T5 = v;
            v++;

            newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B - new Vector3( 0, m_height, 0 );
            newNormals[v] = new Vector3( -1, 0, 0 );
            newUV[v] = new Vector2( m_UVRepetition.x * 0.5f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
            T6 = v;
            v++;

            newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B - new Vector3( 0, m_height, 0 );
            newNormals[v] = new Vector3( -1, 0, 0 );
            newUV[v] = new Vector2( -m_UVRepetition.x * 0.5f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
            T7 = v;
            v++;


            newTriangles[i++] = T1;
            newTriangles[i++] = T0;
            newTriangles[i++] = T2;
            newTriangles[i++] = T2;
            newTriangles[i++] = T0;
            newTriangles[i++] = T3;

            newTriangles[i++] = T0;
            newTriangles[i++] = T4;
            newTriangles[i++] = T3;
            newTriangles[i++] = T3;
            newTriangles[i++] = T4;
            newTriangles[i++] = T7;

            newTriangles[i++] = T1;
            newTriangles[i++] = T2;
            newTriangles[i++] = T6;
            newTriangles[i++] = T1;
            newTriangles[i++] = T6;
            newTriangles[i++] = T5;


            a += splineStep;
            p++;
            if( p % m_meshSubdivision == 0 )
                c++;

        }


        //Mesh update
        m_mesh.Clear();

        m_mesh.vertices = newVertices;
        m_mesh.normals = newNormals;
        m_mesh.uv = newUV;
        m_mesh.triangles = newTriangles;

        m_mesh.RecalculateBounds();
        // m_mesh.RecalculateNormals();

        //GetComponent<MeshFilter>().mesh = caveMesh;

        //local to global 
        //transform.position = savedPosition;
        //transform.rotation = savedRotation;

        return m_mesh;

    }

    public void flipNormals()
    {
        MeshFilter filter = GetComponent( typeof( MeshFilter ) ) as MeshFilter;
        if( filter != null )
        {
            Mesh mesh = filter.sharedMesh;

            Vector3[] normals = mesh.normals;
            for( int i = 0; i < normals.Length; i++ )
                normals[i] = -normals[i];
            mesh.normals = normals;

            for( int m = 0; m < mesh.subMeshCount; m++ )
            {
                int[] triangles = mesh.GetTriangles( m );
                for( int i = 0; i < triangles.Length; i += 3 )
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles( triangles, m );
            }
        }
    }

    //set the material of the road
    public void setMaterial( Material material )
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }
    //in case there is road + pavement, give txo material
    public void setMaterial( Material roadMaterial, Material pavementMaterial)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material[] materials = { roadMaterial, pavementMaterial, pavementMaterial };
        renderer.sharedMaterials = materials;
        //renderer.sharedMaterials [0] = roadMaterial;
        //renderer.sharedMaterials[1] = pavementMaterial;
        //renderer.sharedMaterials[2] = pavementMaterial;
    }

    /*
    void OnDrawGizmos()
    {

        Gizmos.color = Color.white;

        for( int i = 0; i < 2; i++ )
        {
            Vector3 pos = m_spline.getControlPoint( 0 );
            for( int j = 0; j < 100; j++ )
            {
                Vector3 begin = pointTr.right;
                Vector3 end = pointTr.right;

                begin *= getControlPoint( i ).GetComponent<CatmullRomPoint>().Weight;
                end *= getControlPoint( i ).GetComponent<CatmullRomPoint>().Weight;

                Quaternion rotation = Quaternion.AngleAxis( ( 360F / 100F ) * j, pointTr.forward );
                begin = rotation * begin;

                rotation = Quaternion.AngleAxis( ( 360F / 100F ) * ( j + 1 ), pointTr.forward );
                end = rotation * end;

                begin += pointTr.position;
                end += pointTr.position;

                Gizmos.DrawLine( begin, end );
            }
        }

    }*/
}
