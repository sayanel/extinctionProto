using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( SplineRoads ) )]

public class SplineRoadsEditor : Editor
{

    int selectorControlPointValue = 0;
    //int lastSelectorControlPointValue = 0;

    public override void OnInspectorGUI()
    {
        SplineRoads Target = (SplineRoads)target;


        DrawDefaultInspector();

        if( GUILayout.Button( "Init" ) )
        {
            Target.init();
        }
        if( GUILayout.Button( "Generate" ) )
        {
            Target.generate();
        }
        if( GUILayout.Button( "FlipNormals" ) )
        {
            Target.flipNormals();
        }
        if( GUILayout.Button( "AddControlPoint" ) )
        {
            Target.addControlPoint();
        }

        if( Target.getControlPointCount() > 1 )
        {
            selectorControlPointValue = (int)GUILayout.HorizontalSlider( selectorControlPointValue, 0, Target.getControlPointCount() - 1 );

            if( GUILayout.Button( "Select" ) && Target.getControlPoint( selectorControlPointValue ) != null )
            {
                Selection.activeGameObject = Target.getControlPoint( selectorControlPointValue );
            }
        }

        if( GUI.changed )
        {
            if( Target.AutoGenerate )
                Target.generate();
        }

    }
}
