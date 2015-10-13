using UnityEngine;
using System.Collections;

public class SplineRoads : MonoBehaviour
{
    private enum ShapeType {SIMPLE, COMPLEXE};

    [Header( "Components" )]
    [SerializeField]
    CatmullRomSpline m_spline;
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
    [SerializeField]
    ShapeType m_shapeType;
    [SerializeField]
    [Range(0,1)]
    float m_pavementWidth = 1;
    [SerializeField]
    [Range(0, 5)]
    float m_pavementHeight = 0;
    // property meshSubdivision //
    [SerializeField]
    [Range( 1, 10 )]
    int m_meshSubdivision = 10;
    public int MeshSubdivision {
        get { return m_meshSubdivision; }
        set { m_meshSubdivision = value; }
    }
    // endProperty meshSubdivision //
    [SerializeField]
    Vector2 m_UVRepetition = new Vector2(1F,1F);
    [SerializeField]
    Vector2 m_UVRepetitionPavement = new Vector2( 1F, 1F );
    [SerializeField]
    [Range( 0, 10 )]
    int m_seed = 1;
    [SerializeField]
    float m_height = 1;


    public void init()
    {
        if( m_spline == null )
        {
            GameObject tmp = new GameObject("spline");
            tmp.AddComponent<CatmullRomSpline>();
            m_spline = tmp.GetComponent<CatmullRomSpline>();
            m_spline.IsLooping = false;
        }

        if( m_spline.transform.parent == null || m_spline.transform.parent != this.transform )
        {
            m_spline.transform.SetParent( this.transform );
            m_spline.transform.localPosition = Vector3.zero;
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

    public void addControlPoint()
    {
        m_spline.addControlPoint();
    }

    public void addControlPoint(Transform tr)
    {
        m_spline.addControlPoint(tr);
    }

    public int getControlPointCount()
    {
        if( m_spline != null )
            return m_spline.getSize();
        else
            return 0;
    }

    public GameObject getControlPoint( int index )
    {
        if( m_spline != null )
            return m_spline.getSplineControlPoint( index ).gameObject;
        else
            return null;
    }

    //set the width of all control points 
    public void setRoadwidth(float width)
    {
        m_spline.setAllWidths( width );
    }

    //generate a new Mesh. If the GameObjet isn't initialyze, it will be initialysed. 
    public void generate()
    {
        if( m_mesh == null )
            init();
        if( m_shapeType == ShapeType.SIMPLE )
            buildSimpleMesh();
        else
            buildComplexMesh();
    }

    Vector3 getSplinePoint(int index)
    {
        int splineIndex = (int) (index / m_meshSubdivision) + 1;
        float subSplineIndex = index % m_meshSubdivision;
        subSplineIndex /= (float)m_meshSubdivision;

        return m_spline.getSplinePoint( splineIndex, subSplineIndex );
    }

    Vector3 getSplineDirection( int index )
    {
        int splineIndex = (int)( index / m_meshSubdivision ) + 1;
        float subSplineIndex = index % m_meshSubdivision;
        subSplineIndex /= (float)m_meshSubdivision;

        return m_spline.getSplinePointDirection( splineIndex, subSplineIndex );
    }

    Transform getSplineControlPoint(int index)
    {
        int splineIndex = (int)( index / m_meshSubdivision ) + 1;

        return m_spline.getSplineControlPoint( splineIndex );
    }

    void generateRoad( ref int i, ref int p, ref int c, ref int v, ref float a, ref Vector3[] newVertices, ref Vector3[] newNormals, ref Vector2[] newUV, ref int[] newTriangles, int subMeshIndice, float decalX, float decalY )
    {
        Transform splineControlPoint_A = getSplineControlPoint( p );
        Vector3 splinePoint_A = getSplinePoint( p ); //m_spline.getSplinePoint( c, a );
        Vector3 splinePointDirection_A = getSplineDirection( p ); //m_spline.getSplinePointDirection( c, a );
        Vector3 splinePointNormal_A = Vector3.Cross( splineControlPoint_A.up, splinePointDirection_A ).normalized;

        Transform splineControlPoint_B = getSplineControlPoint( p + 1 );
        Vector3 splinePoint_B = getSplinePoint( p + 1 ); //m_spline.getSplinePoint( c, a + splineStep );
        Vector3 splinePointDirection_B = getSplineDirection( p + 1 ); //m_spline.getSplinePointDirection( c, a + splineStep );
        Vector3 splinePointNormal_B = Vector3.Cross( splineControlPoint_B.up, splinePointDirection_B ).normalized;

        float previousHeight = m_height;
        float roadWeight = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( (float)p / m_meshSubdivision ) );
        float roadWeight01 = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
        float currentWeight = 0;
        float currentWeight01 = 0;
        Vector2 uvRepetition;
        if( subMeshIndice == 0 )
        {
            currentWeight = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( (float)p / m_meshSubdivision ) );
            currentWeight01 = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
            uvRepetition = m_UVRepetition;
        }
        else
        {
            currentWeight = m_pavementWidth * Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( (float)p / m_meshSubdivision ) );
            currentWeight01 = m_pavementWidth * Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( ( ( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : ( ( p + 1 ) % m_meshSubdivision ) / (float)m_meshSubdivision ) );
            uvRepetition = m_UVRepetitionPavement;

            m_height += decalY;
        }

        int T0 = 0, T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0;
        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A + new Vector3(decalX * ( roadWeight + currentWeight  ), decalY, 0);
        newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A );
        newUV[v] = new Vector2( -uvRepetition.x * 0.45f + 0.5f, p * uvRepetition.y );
        T0 = v;
        v++;

        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A + new Vector3( decalX * ( roadWeight + currentWeight ), decalY, 0 );
        newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A ).normalized;
        newUV[v] = new Vector2( uvRepetition.x * 0.45f + 0.5f, p * uvRepetition.y );
        T1 = v;
        v++;

        newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B + new Vector3( decalX * ( roadWeight01 + currentWeight01 ), decalY, 0 );
        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
        newUV[v] = new Vector2( uvRepetition.x * 0.45f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T2 = v;
        v++;

        newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B + new Vector3( decalX * ( roadWeight01 + currentWeight01 ), decalY, 0 );
        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
        newUV[v] = new Vector2( -uvRepetition.x * 0.45f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T3 = v;
        v++;

        //height
        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A - new Vector3( 0, m_height, 0 ) + new Vector3( decalX * ( roadWeight + currentWeight ), decalY, 0 );
        newNormals[v] = new Vector3( 1, 0, 0 );
        newUV[v] = new Vector2( -uvRepetition.x * 0.5f + 0.5f, p * uvRepetition.y );
        T4 = v;
        v++;

        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A - new Vector3( 0, m_height, 0 ) + new Vector3( decalX * ( roadWeight + currentWeight ), decalY, 0 );
        newNormals[v] = new Vector3( 1, 0, 0 );
        newUV[v] = new Vector2( uvRepetition.x * 0.5f + 0.5f, p * uvRepetition.y );
        T5 = v;
        v++;

        newVertices[v] = splinePointNormal_B * currentWeight01 + splinePoint_B - new Vector3( 0, m_height, 0 ) + new Vector3( decalX * ( roadWeight01 + currentWeight01 ), decalY, 0 );
        newNormals[v] = new Vector3( -1, 0, 0 );
        newUV[v] = new Vector2( uvRepetition.x * 0.5f + 0.5f, ( p + 1 ) * uvRepetition.y );
        T6 = v;
        v++;

        newVertices[v] = splinePointNormal_B * ( -currentWeight01 ) + splinePoint_B - new Vector3( 0, m_height, 0 ) + new Vector3( decalX * ( roadWeight01 + currentWeight01 ), decalY, 0 );
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
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

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
        int nbOfTriangle = (( getControlPointCount() - 3 ) * m_meshSubdivision * 6 * 3)*3;
        int nbOfPoints = (( getControlPointCount() - 3 ) * m_meshSubdivision * 8)*3;

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
        for(int subMesh = 0; subMesh < 3; subMesh++)
        {
            if( subMesh == 0 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles01, subMesh, 0, 0);
                }
                p = 0;
                c = 1;
                a = 0;
            }
                
            else if( subMesh == 1 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles02, subMesh, -0.9F, m_pavementHeight );
                }
                p = 0;
                c = 1;
                a = 0;
            }
                
            else if( subMesh == 2 )
            {
                for( int i = 0; i < nbOfTriangle / 3; ) //i : triangle index
                {
                    generateRoad( ref i, ref p, ref c, ref v, ref a, ref newVertices, ref newNormals, ref newUV, ref newTriangles03, subMesh, 0.9F, m_pavementHeight );
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
        transform.position = savedPosition;
        transform.rotation = savedRotation;

        return m_mesh;

    }


    Mesh buildSimpleMesh()
    {
        //global to local
        Vector3 savedPosition = transform.position;
        Quaternion savedRotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

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
        int nbOfTriangle = ( getControlPointCount() - 3 ) * m_meshSubdivision * 6 * 3;
        int nbOfPoints = ( getControlPointCount() - 3 ) * m_meshSubdivision * 8;

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

            Transform splineControlPoint_A = getSplineControlPoint( p );
            Vector3 splinePoint_A = getSplinePoint( p ); //m_spline.getSplinePoint( c, a );
            Vector3 splinePointDirection_A = getSplineDirection( p ); //m_spline.getSplinePointDirection( c, a );
            Vector3 splinePointNormal_A = Vector3.Cross( splineControlPoint_A.up, splinePointDirection_A ).normalized;

            Transform splineControlPoint_B = getSplineControlPoint( p + 1 );
            Vector3 splinePoint_B = getSplinePoint( p + 1 ); //m_spline.getSplinePoint( c, a + splineStep );
            Vector3 splinePointDirection_B = getSplineDirection( p + 1 ); //m_spline.getSplinePointDirection( c, a + splineStep );
            Vector3 splinePointNormal_B = Vector3.Cross( splineControlPoint_B.up, splinePointDirection_B ).normalized;

            float currentWeight = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( (p % m_meshSubdivision) / (float)m_meshSubdivision ) );
            float currentWeight01 = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ( (( p + 1 ) % m_meshSubdivision ) == 0 ? 1 : (( p + 1 ) % m_meshSubdivision) / (float)m_meshSubdivision ) );

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
        transform.position = savedPosition;
        transform.rotation = savedRotation;

        return m_mesh;

    }

    //Mesh buildSimpleMesh()
    //{
    //    //global to local
    //    Vector3 savedPosition = transform.position;
    //    Quaternion savedRotation = transform.rotation;
    //    transform.position = Vector3.zero;
    //    transform.rotation = Quaternion.identity;

    //    //Random
    //    int tmpSeed = Random.seed;
    //    Random.seed = m_seed;
    //    Vector2 seedVector = Vector2.zero;

    //    if( Random.Range( 0F, 6F ) >= 4F )
    //        seedVector = new Vector2( 0, m_seed );
    //    else if( Random.Range( 0F, 6F ) < 4F && Random.Range( 0F, 6F ) >= 2F )
    //        seedVector = new Vector2( m_seed, m_seed );
    //    else if( Random.Range( 0F, 6F ) < 2F )
    //        seedVector = new Vector2( m_seed, 0 );

    //    Random.seed = tmpSeed;

    //    //Mesh initialisation
    //    int nbOfTriangle = ( getControlPointCount() - 3 ) * m_meshSubdivision * 6 * 3;
    //    int nbOfPoints = ( getControlPointCount() - 3 ) * m_meshSubdivision * 8;

    //    Vector3[] newVertices = new Vector3[nbOfPoints];
    //    Vector2[] newUV = new Vector2[nbOfPoints];
    //    Vector3[] newNormals = new Vector3[nbOfPoints];
    //    int[] newTriangles = new int[nbOfTriangle];

    //    //Generate mesh

    //    int j = 0; // arondie
    //    int v = 0; // vertice index
    //    int c = 1; // controlPoint
    //    int p = 0;
    //    float a = 0F; // fraction 0 -> meshSubdivision
    //    float splineStep = 1F / (float)( m_meshSubdivision - 1 );
    //    for( int i = 0; i < nbOfTriangle;) //i : triangle index
    //    {

    //        Transform splineControlPoint_A = getSplineControlPoint( p );
    //        Vector3 splinePoint_A = getSplinePoint( p ); //m_spline.getSplinePoint( c, a );
    //        Vector3 splinePointDirection_A = getSplineDirection( p ); //m_spline.getSplinePointDirection( c, a );
    //        Vector3 splinePointNormal_A = Vector3.Cross( splineControlPoint_A.up, splinePointDirection_A ).normalized;

    //        Transform splineControlPoint_B = getSplineControlPoint( p + 1 );
    //        Vector3 splinePoint_B = getSplinePoint( p + 1 ); //m_spline.getSplinePoint( c, a + splineStep );
    //        Vector3 splinePointDirection_B = getSplineDirection( p + 1 ); //m_spline.getSplinePointDirection( c, a + splineStep );
    //        Vector3 splinePointNormal_B = Vector3.Cross( splineControlPoint_B.up, splinePointDirection_B ).normalized;

    //        float currentWeight = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, ((float)p / m_meshSubdivision) );

    //        int T0 = 0, T1 = 0, T2 = 0, T3 = 0, T4 = 0, T5 = 0, T6 = 0, T7 = 0;
    //        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A;
    //        newNormals[v] = Vector3.Cross( splinePointDirection_A, splinePointNormal_A );
    //        newUV[v] = new Vector2( -m_UVRepetition.x * 0.45f + 0.5f, p * m_UVRepetition.y );
    //        T0 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A;
    //        newNormals[v] = Vector3.Cross(splinePointDirection_A, splinePointNormal_A).normalized;
    //        newUV[v] = new Vector2( m_UVRepetition.x * 0.45f + 0.5f, p * m_UVRepetition.y );
    //        T1 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_B * currentWeight + splinePoint_B;
    //        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
    //        newUV[v] = new Vector2( m_UVRepetition.x * 0.45f + 0.5f, (p + 1) * m_UVRepetition.y );
    //        T2 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_B * ( -currentWeight ) + splinePoint_B;
    //        newNormals[v] = Vector3.Cross( splinePointDirection_B, splinePointNormal_B );
    //        newUV[v] = new Vector2( -m_UVRepetition.x * 0.45f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
    //        T3 = v;
    //        v++;

    //        //height
    //        newVertices[v] = splinePointNormal_A * ( -currentWeight ) + splinePoint_A - new Vector3(0, m_height, 0);
    //        newNormals[v] = new Vector3( 1, 0, 0 );
    //        newUV[v] = new Vector2( -m_UVRepetition.x * 0.5f + 0.5f, p * m_UVRepetition.y );
    //        T4 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_A * currentWeight + splinePoint_A - new Vector3( 0, m_height, 0 );
    //        newNormals[v] = new Vector3( 1, 0, 0 );
    //        newUV[v] = new Vector2( m_UVRepetition.x * 0.5f + 0.5f, p * m_UVRepetition.y );
    //        T5 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_B * currentWeight + splinePoint_B - new Vector3( 0, m_height, 0 );
    //        newNormals[v] = new Vector3( -1, 0, 0 );
    //        newUV[v] = new Vector2( m_UVRepetition.x * 0.5f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
    //        T6 = v;
    //        v++;

    //        newVertices[v] = splinePointNormal_B * ( -currentWeight ) + splinePoint_B - new Vector3( 0, m_height, 0 );
    //        newNormals[v] = new Vector3(-1, 0, 0 );
    //        newUV[v] = new Vector2( -m_UVRepetition.x * 0.5f + 0.5f, ( p + 1 ) * m_UVRepetition.y );
    //        T7 = v;
    //        v++;


    //        newTriangles[i++] = T1;
    //        newTriangles[i++] = T0;
    //        newTriangles[i++] = T2;
    //        newTriangles[i++] = T2;
    //        newTriangles[i++] = T0;
    //        newTriangles[i++] = T3;

    //        newTriangles[i++] = T0;
    //        newTriangles[i++] = T4;
    //        newTriangles[i++] = T3;
    //        newTriangles[i++] = T3;
    //        newTriangles[i++] = T4;
    //        newTriangles[i++] = T7;

    //        newTriangles[i++] = T1;
    //        newTriangles[i++] = T2;
    //        newTriangles[i++] = T6;
    //        newTriangles[i++] = T1;
    //        newTriangles[i++] = T6;
    //        newTriangles[i++] = T5;


    //        a += splineStep;
    //        p++;
    //        if( p % m_meshSubdivision == 0) 
    //            c++;

    //    }


    //    //Mesh update
    //    m_mesh.Clear();

    //    m_mesh.vertices = newVertices;
    //    m_mesh.normals = newNormals;
    //    m_mesh.uv = newUV;
    //    m_mesh.triangles = newTriangles;

    //    m_mesh.RecalculateBounds();
    //   // m_mesh.RecalculateNormals();

    //    //GetComponent<MeshFilter>().mesh = caveMesh;

    //    //local to global 
    //    transform.position = savedPosition;
    //    transform.rotation = savedRotation;

    //    return m_mesh;

    //}


    //Mesh buildCave()
    //{
    //    int nbOfPoints = ( getControlPointCount() - 2 ) * 10 * 10;
    //    int nbOfTriangle = ( nbOfPoints - ( ( getControlPointCount() - 2 ) * 10 ) ) * 2 * 3;

    //    Vector3[] newVertices = new Vector3[nbOfPoints];
    //    Vector2[] newUV = new Vector2[nbOfPoints];
    //    Vector3[] newNormals = new Vector3[nbOfPoints];
    //    int[] newTriangles = new int[nbOfTriangle];

    //    int k = 0; // longueur
    //    int j = 0; // arondie
    //    int c = 1; // control
    //    float a = 0F; // fraction
    //    for( int i = 0; i < nbOfPoints; )
    //    {

    //        Transform pointTr = getControlPoint( c ).transform;
    //        Vector3 pointVec = m_spline.getSplinePoint( c, a );
    //        Vector3 pointDir = m_spline.getSplinePointDirection( c, a );

    //        Vector3 P1 = Vector3.Cross( pointDir, pointTr.right ).normalized;

    //        float currentWeight = Mathf.Lerp( getControlPoint( c ).GetComponent<CatmullRomPoint>().Weight, getControlPoint( c + 1 ).GetComponent<CatmullRomPoint>().Weight, a );

    //        P1 *= currentWeight;

    //        Quaternion rotation = Quaternion.AngleAxis( ( 360F / 9F ) * j, pointDir );
    //        P1 = rotation * P1;

    //        P1 += ( pointVec - transform.position );

    //        newVertices[i] = P1;

    //        newNormals[i] = Vector3.Normalize( pointVec - P1 );
    //        newUV[i] = new Vector2( k * m_UVRepetition, j * m_UVRepetition );

    //        i++;
    //        j++;

    //        passage à un autre cercle
    //        if( i % 10 == 0 && i != 0 )
    //        {
    //            j = 0;
    //            k++;
    //            a += 1 / 9F;
    //        }
    //        Passage à un autre point de controle
    //        if( i % ( 10 * 10 ) == 0 && i != 0 )
    //        {
    //            c++;
    //            a = 0;
    //        }

    //    }

    //    for( int t = 0, n = 0; t < nbOfTriangle; t += 6, n++ )
    //    {
    //        if( n >= nbOfPoints )
    //            break;

    //        newTriangles[t] = n + 1;
    //        newTriangles[t + 1] = n;
    //        newTriangles[t + 2] = n + 10;

    //        newTriangles[t + 3] = n + 1;
    //        newTriangles[t + 1 + 3] = n + 10;
    //        newTriangles[t + 2 + 3] = n + 11;
    //    }

    //    m_mesh.Clear();


    //    m_mesh.vertices = newVertices;
    //    m_mesh.normals = newNormals;
    //    m_mesh.uv = newUV;
    //    m_mesh.triangles = newTriangles;

    //    m_mesh.RecalculateBounds();
    //    m_mesh.RecalculateNormals();

    //    GetComponent<MeshFilter>().mesh = m_mesh;

    //    return m_mesh;

    //}


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
    public void setMaterial(Material material)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.white;

        for( int i = 0; i < getControlPointCount(); i++ )
        {
            Transform pointTr = getControlPoint( i ).transform;

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

    }
}
