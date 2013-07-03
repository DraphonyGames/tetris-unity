using UnityEngine;
using System.Collections;

public class MatchCamera : MonoBehaviour {
    static public int Scores = 0;
    static public int Level  = 1;

    public const int ScoresForNextLevel = 100;
    /// <summary>Lines continuously created</summary>
    static public int Continuous = 0;

    #region Event Handler
    void Start() {
        NewGame();
    }

    
    void OnGUI() {
        // Use OnGUI rather than GUIObject inside the editor
        // http://docs.unity3d.com/Documentation/Components/class-GuiText.html

        // To have a complete dark scene. Go to Rendering Settings and turn "Ambient Light" to "black"

        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor  = Color.white;
        textStyle.fontSize          = 36;

        // Using Automatic Mode
        // http://docs.unity3d.com/Documentation/Components/gui-Layout.html
        GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height / 2, Screen.width/2, Screen.height / 2));
        
        GUILayout.Label("Scores:\t" + Scores, textStyle);
        GUILayout.Label("Level:\t" + Level,  textStyle);
        if (Continuous > 1)
            GUILayout.Label("Continuously:" + Continuous, textStyle);
        GUILayout.EndArea();
    }
    #endregion

    #region Helper function
    void NewGame() {
        Scores               = 0;
        Level                = 1;
        Continuous           = 0;
        Tetrimo.TetrimoCount = 0;
    }
    #endregion
}
