using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( AProceduralMesh ), true )]
public class ProceduralMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AProceduralMesh Target = (AProceduralMesh)target;

        DrawDefaultInspector();

        if( GUILayout.Button( "Init" ) )
        {
            Target.Init();
        }
        if( GUILayout.Button( "Generate" ) )
        {
            Target.Generate();
        }
        if( GUILayout.Button( "FlipNormals" ) )
        {
            Target.FlipNormals();
        }
    }
}