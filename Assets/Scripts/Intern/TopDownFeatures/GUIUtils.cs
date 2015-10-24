using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIUtils : MonoBehaviour 
{
    //simple 1*1 pixel texture with white color
    private static Texture2D m_whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if( m_whiteTexture == null )
            {
                m_whiteTexture = new Texture2D( 1, 1 );
                m_whiteTexture.SetPixel( 0, 0, Color.white );
                m_whiteTexture.Apply();
            }

            return m_whiteTexture;
        }
    }

    //draw a rectangle on screen
    public static void DrawScreenRect( Rect rect, Color color )
    {
        GUI.color = color;
        GUI.DrawTexture( rect, WhiteTexture );
        GUI.color = Color.white;
    }

    //draw 4 rectangle to make a rectangle box on screen
    public static void DrawScreenRectBorder( Rect rect, float thickness, Color color )
    {
        // Top
        DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
        // Left
        DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
        // Right
        DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color );
        // Bottom
        DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
    }
}
