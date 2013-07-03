using UnityEngine;
using System.Collections;

public class GameOverCamera : MonoBehaviour {
    const float BoxWidth    = 300.0f,
                BoxHeight   = 140.0f,

                // Button 'New Game'
                ButtonWidth = 160.0f,
                ButtonHeight= 30.0f;
    public bool NewHighScore = false;

    void Start() {
        NewHighScore = HighScore.TryAddHighScore(MatchCamera.Scores);
    }

    void OnGUI() {
        GUI.Box(CreateCenteredRect(BoxWidth, BoxHeight), "Game Over");
        GUI.Label(CreateCenteredRect(100.0f, BoxHeight, 20), MatchCamera.Scores.ToString(), new GUIStyle() {
            fontSize    = 36,
            alignment   = TextAnchor.UpperCenter,
            normal      = new GUIStyleState() {
                textColor = NewHighScore ? Color.green : Color.gray
            }
        });
        GUI.Label(CreateCenteredRect(100.0f, BoxHeight, 60.0f), "Scores", new GUIStyle() {
            fontSize  = 10,
            alignment = TextAnchor.UpperCenter,
            normal    = new GUIStyleState() {
                textColor = Color.white
            }
        });

        if (GUI.Button(CreateCenteredRect(ButtonWidth, ButtonHeight, BoxHeight/2 - ButtonHeight - 5), "Back to Main Menu")) {
            Application.LoadLevel("MainMenu");
        }
    }

    private static Rect CreateCenteredRect(float width, float height, float y_offset =0) {
        return new Rect((Screen.width - width) / 2, (Screen.height - height) / 2 + y_offset, width, height);
    }
}
