using UnityEngine;
using System.Collections;

public class MainMenuCamera : MonoBehaviour {
    public Texture2D Background;
    public Texture2D TetrisTitel;
    public Font      HighScoreFont;

    bool ShowHighScore = false;

    void OnGUI() {
        int ButtonWidth         = Mathf.CeilToInt(Screen.width * 0.25f),
            ButtonHeight        = Mathf.CeilToInt(ButtonWidth  * 0.15f),
            ButtonSpace         = Mathf.CeilToInt(ButtonHeight * 0.10f),
            ButtonBoxWidth      = Mathf.CeilToInt(ButtonWidth       + 40),
            ButtonBoxHeight     = Mathf.CeilToInt(ButtonHeight  * 3 + ButtonSpace * 2 + 40);
        int HighScoreBoxWidth   = Mathf.CeilToInt(Screen.width  * 0.4f),
            HighScoreBoxHeight  = Mathf.CeilToInt(Screen.height * 0.4f);
        int FontSize            = Mathf.CeilToInt(HighScoreBoxHeight / 5);

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background);
        GUI.DrawTexture(new Rect((Screen.width - TetrisTitel.width) / 2, 10, TetrisTitel.width, TetrisTitel.height), TetrisTitel);

        if (!ShowHighScore) {
            Rect box = new Rect(5, (Screen.height - ButtonBoxHeight) / 2, ButtonBoxWidth, ButtonBoxHeight);

            GUI.Box(box, "");
            GUILayout.BeginArea(box);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            if (GUILayout.Button("New Game", GUILayout.Height(ButtonHeight), GUILayout.Width(ButtonWidth))) {
                Application.LoadLevel("Match");
            }
            GUILayout.Space(ButtonSpace);
            if (GUILayout.Button("Highscores", GUILayout.Height(ButtonHeight), GUILayout.Width(ButtonWidth))) {
                ShowHighScore = true;
            }
            GUILayout.Space(ButtonSpace);
            if (GUILayout.Button("Visit Draphony Games online", GUILayout.Height(ButtonHeight), GUILayout.Width(ButtonWidth))) {
                Application.OpenURL("http://www.draphony.de");
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
        else {
            GUIStyle highScoreStyle = new GUIStyle() {
                alignment = TextAnchor.MiddleCenter,
                fontSize = FontSize,
                font     = HighScoreFont,
                fontStyle= FontStyle.Bold,
                normal   = new GUIStyleState() {
                    textColor = Color.white
                }
            };

            Rect HighScoreBox = new Rect((Screen.width - HighScoreBoxWidth) / 2, (Screen.height - HighScoreBoxHeight) / 2, HighScoreBoxWidth, HighScoreBoxHeight);
            GUI.Box(HighScoreBox,"");
            GUILayout.BeginArea(HighScoreBox);
            for (int i = 2; i >= 0; i--) {
                GUILayout.Label(HighScore.get(i).ToString("D10"), highScoreStyle);
                GUILayout.Space(5);
            }
            if (GUILayout.Button("Close", GUILayout.MinHeight(ButtonHeight), GUILayout.MinWidth(ButtonWidth))) {
                Application.LoadLevel("MainMenu");
            }
            GUILayout.EndArea();
        }
    }
}
