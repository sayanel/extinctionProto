using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompareRoadNodes : IComparer<RoadNode>
{
    public int Compare( RoadNode x, RoadNode y )
    {
        return ( x.transform.position.x < y.transform.position.x ) ? -1 : 1;
    }
}

public class RoadGraph : AProceduralMesh 
{
    [Header("essentials")]
    [SerializeField]
    int m_columnCount = 1;
    [SerializeField]
    int m_meshSubdivisions = 5;
    [SerializeField]
    Material m_roadMaterial;
    [SerializeField]
    Material m_pavementMaterial;
    [SerializeField]
    Vector2 m_UVRepetition = new Vector2( 1F, 1F );
    [SerializeField]
    Vector2 m_UVRepetitionPavement = new Vector2( 1F, 1F );
    [SerializeField]
    BezierRoad.ShapeType m_shapeType;

    [Header("embellishment")]
    [SerializeField]
    float m_roadWidth = 1; // width of generated roads 
    [SerializeField]
    float m_roadEndsWeight = 4; // influence the "weight" of tangents of the roads, at extremities

    [Header("data")]
	[SerializeField]
    List<RoadNode> m_listNodes = new List<RoadNode>();
    [SerializeField]
    List<BezierRoad> m_listRoads = new List<BezierRoad>();


    public override void Generate()
    {
        foreach( RoadNode node in m_listNodes )
        {
            node.Init();
        }

        generateRoads(); // make roads between nodes

        generateNodes();
    }

    void generateNodes()
    {
        foreach( RoadNode node in m_listNodes )
        {
            node.RoadWidth = m_roadWidth;
            node.setMaterial( m_roadMaterial );
            //node.Generate();
        }
    }

    public override void Init()
    {
        m_listNodes.Clear();
        for(int i = 0; i < m_listRoads.Count; ++i )
        {
            DestroyImmediate(m_listRoads[i].gameObject);
        }
        m_listRoads.Clear();

        m_listNodes.AddRange(GetComponentsInChildren<RoadNode>());

        foreach(RoadNode node in m_listNodes)
        {
            node.Init();
        }
    }

    public void generateRoads()
    {
        int rowCount = m_listNodes.Count / m_columnCount;
        List<List<int>> sortedIndex = new List<List<int>>(); // represente all the datas of m_listNodes, ordered row by row
        //sort a first time by position in z
        m_listNodes.Sort(
            delegate(RoadNode node1, RoadNode node2 )
            {
                return (node1.transform.position.z > node2.transform.position.z) ? -1 : 1;
            }
        );

        for( int i = 0; i < rowCount; i++ )
        {
            m_listNodes.Sort( i * m_columnCount, m_columnCount, new CompareRoadNodes() );
        }

        int columnIndex = 0;
        for( int i = 0; i < m_listNodes.Count; i++)
        {
            if( columnIndex % m_columnCount == 0 )
            {
                List<int> rowList = new List<int>();
                sortedIndex.Add( rowList );
            }

            sortedIndex[sortedIndex.Count - 1].Add(i);

            columnIndex++;
        }

        for( int j = 0; j < rowCount; j++ )
        {
            for( int i = 0; i < m_columnCount; i++ )
            {
                if(i > 0 && (m_listNodes[sortedIndex[j][i-1]].getAnchor( RoadAnchor.AnchorNames.RIGHT ) != null) && ( m_listNodes[sortedIndex[j][i]].getAnchor( RoadAnchor.AnchorNames.LEFT ) != null ) )
                    linkTwoNodes(sortedIndex[j][i - 1], sortedIndex[j][i], RoadAnchor.AnchorNames.RIGHT, RoadAnchor.AnchorNames.LEFT);

                if(j > 0 && ( m_listNodes[sortedIndex[j - 1][i]].getAnchor( RoadAnchor.AnchorNames.BOTTOM ) != null ) && ( m_listNodes[sortedIndex[j][i]].getAnchor( RoadAnchor.AnchorNames.TOP ) != null ) )
                    linkTwoNodes( sortedIndex[j - 1][i], sortedIndex[j][i], RoadAnchor.AnchorNames.BOTTOM, RoadAnchor.AnchorNames.TOP );
            }
        }
    }

    //generate the road between two nodes
    void linkTwoNodes(int i, int j, RoadAnchor.AnchorNames iAnchor, RoadAnchor.AnchorNames jAnchor)
    {
        if( i == j )
            return;

        GameObject newSplineObject = new GameObject("road");
        newSplineObject.transform.SetParent( this.transform );
        BezierRoad newSplineRoad = newSplineObject.AddComponent<BezierRoad>();

        newSplineRoad.Init();

        Vector3 centerI = m_listNodes[i].getCenter().position;
        Vector3 centerJ = m_listNodes[j].getCenter().position;
        newSplineRoad.setControlPoints(
            m_listNodes[i].getAnchor( iAnchor ).position,
            m_listNodes[i].getAnchor( iAnchor ).position + m_roadEndsWeight * (m_listNodes[i].getAnchor( iAnchor ).position - centerI),
            m_listNodes[j].getAnchor( jAnchor ).position + m_roadEndsWeight * (m_listNodes[j].getAnchor( jAnchor ).position - centerJ),
            m_listNodes[j].getAnchor( jAnchor ).position

        );

        newSplineRoad.setRoadwidth( m_roadWidth );
        newSplineRoad.MeshSubdivision = m_meshSubdivisions;
        newSplineRoad.setMaterial( m_roadMaterial );
        newSplineRoad.UVRepetition = m_UVRepetition;
        newSplineRoad.UVRepetitionPavement = m_UVRepetitionPavement;
        newSplineRoad.Shape = m_shapeType;

        m_listRoads.Add( newSplineRoad );

        newSplineRoad.Generate();
    }

}
