using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Don't use "System.Collections.Generic" if you want to export your app to flash

public class Tetrimo : MonoBehaviour {
    #region Shapes and Colors
    static Vector2[, ,] Shapes = new Vector2[7,4,4] {
        // #
        // ###
		{
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0) },
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0,-1) },
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 0) },
            {new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1), new Vector2(0,-1) },
        },
        //   #
        // ###
		{
            {new Vector2(2, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(1,-1)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1)}
        },
        //  #
        // ###
		{
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 0)},
            {new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1,-1)}
        },
        // ##
        // ##
		{
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)}
        },
        //
        // ####
		{
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(0,-2)},
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(0,-1), new Vector2(0,-2)}
        },                                                                           
        //  ##                                                                       
        // ##                                                                        
		{                                                                            
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1,-1)},
            {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)},
            {new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1,-1)}
        },                                                                           
		// ##                                                                        
        //  ##                                                                       
		{                                                                            
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,-1)},
            {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0)},
            {new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,-1)}
        }
	};
    static Color[] Colors = new Color[7] {
        Color.red,
        Color.green,
        Color.blue,
        Color.white,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };
    #endregion
    #region NestedTypes
    public struct CFieldSize {
        public int Left, Right, Bottom, Top;
    }
    /// <summary>
    /// <list type="">
    ///     <item>Falling</item>
    ///     <item>Landed </item>
    ///     <item>Fixed< /item>
    /// </list>
    /// </summary>
    public enum TetrimoState {
        Spawning,   // Tetrimo is about to become "Falling"
        Falling,
        Landed,
        Fixed,
        Preview
    }
    #endregion

    #region Class instances data members
    /// <summary>All tetrimos in Preview State</summary>
    public static Tetrimo NextTetrimo;
    public static uint TetrimoCount = 1;
    /// <summary>border position of the game field (inclusive values)</summary>
    static CFieldSize FieldSize = new CFieldSize() {
        Left    =   0,
        Right   =  10,
        Top     =  18,  // always start at top-1, cause the center of each tetrimo is the second element on the lower row
        Bottom  =   0
    };
    /// <summary>Start position of new Tetrimos in 'Preview' state</summary>
    static Vector2 PreviewHUD = new Vector2() {
        x = 16,
        y = 16
    };
    /// <summary>
    /// <list type="">
    ///     <item>null,         if position is free</item>
    ///     <item>gameObject,   if position is occupied </item>
    /// </list>
    /// </summary>
    static GameObject[,] FieldMatrix = new GameObject[19, 11];
    #endregion

    #region Fields (Set in Unity Editor)
    /// <summary>Pointer to itselfs => create new Tetrimos</summary>
    public GameObject   TetrimoPrefab;
    public GameObject   TetrimoPartPrefab;
    public GameObject   TetrimoExplosionPrefab;
    public GameObject   FourLinesLabelPrefab;

    public TetrimoState State;
    /// <summary>Swap action cooldown</summary>
    public float        SwapCooldown;
    /// <summary>Horizontal movement speed</summary>
    public float        HorizontalSpeed;
    public float        FallingCooldown;
    public float        FallingSpeed;
    #endregion

    #region Private Fields
    /// <summary>Countdown for next swap</summary>
    float NextSwap = 0.0f;
    /// <summary>Countdown for next fall</summary>
    float NextFall;
    int   ShapeIndex, RotationIndex;
    bool  IsMovingHorizontal = false;
    #endregion

    #region Properties
    bool CanMoveDown {
        get {
            if(IsMovingHorizontal)
                return false;

            foreach (Transform child in transform) {
                if (Mathf.RoundToInt(child.position.y - 1) < 0 || FieldMatrix[Mathf.RoundToInt(child.position.y - 1), Mathf.RoundToInt(child.position.x)] != null)
                    return false;
            }
            return true;
        }
    }
    bool CanMoveRight {
        get {
            bool canMoveRight = true;
            foreach (Transform child in transform) {
                canMoveRight &= Mathf.RoundToInt(child.position.x + 1) <= Tetrimo.FieldSize.Right && FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x + 1)] == null;
            }
            return canMoveRight;
        }
    }
    bool CanMoveLeft {
        get {
            bool canMoveLeft = true;
            foreach (Transform child in transform) {
                canMoveLeft &= Mathf.RoundToInt(child.position.x - 1) >= Tetrimo.FieldSize.Left && FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x - 1)] == null;
            }
            return canMoveLeft;
        }
    }
    bool CanRotate {
        get {
            // Iterate through each TetrimoParts
            for (int index = 0; index < Shapes.GetLength(2); index++) {
                Vector2 tmp = new Vector2(transform.position.x, transform.position.y) + Shapes[ShapeIndex, (RotationIndex + 1) % Shapes.GetLength(2), index];

                if (tmp.x < FieldSize.Left || tmp.x > FieldSize.Right || tmp.y < FieldSize.Bottom || tmp.y > FieldSize.Top)
                    return false;

                if (FieldMatrix[Mathf.RoundToInt(tmp.y), Mathf.RoundToInt(tmp.x)] != null)
                    return false;
            }
            return true;
        }
    }
    #endregion

    #region Event Handler
    void Start () {
        switch (State) {
        case TetrimoState.Spawning:
            NextFall = FallingCooldown;

            // Create shape and translate it at the center top of the game field.
            // REMARK: Do not use translate here! Instantiate create a copy of the current object.
            CreateShape();
            transform.position = new Vector3((int)(FieldSize.Right / 2), FieldSize.Top - 1, 0);

            // Check if the player has lost the game
            foreach (Transform child in transform) {
                if (FieldMatrix[(int)child.position.y, (int)child.position.x] != null) {
                    Application.LoadLevel("GameOver");
                }
            }
            name = "Tetrimo#" + (TetrimoCount-1);
            State = TetrimoState.Falling;
            break;

        case TetrimoState.Preview:
            RotationIndex = Random.Range(0, Shapes.GetLength(1));
            ShapeIndex    = Random.Range(0, Shapes.GetLength(0));
            CreateShape();
            transform.position = new Vector3(PreviewHUD.x, PreviewHUD.y);

            NextTetrimo = this;

            TetrimoCount++;
            name = "Tetrimo#" + TetrimoCount;
            break;
        }
	}
    void Update() {
        if (State != TetrimoState.Fixed && State != TetrimoState.Preview && State != TetrimoState.Spawning) {
            if (State != TetrimoState.Landed && Input.GetAxis("Vertical") < 0)
                StartCoroutine(FallingDown());

            if (Input.GetButtonDown("Horizontal"))
                StartCoroutine(MoveHorizontal());

            // Set "up" as alternative button for Jump (Project => Input)
            if ((Input.GetButton("Jump")) && Time.time > NextSwap) {    
                if (this.CanRotate) {
                    StartCoroutine(RotateTetrimo());
                    NextSwap = Time.time + SwapCooldown;
                }
            }

            // Automatic falling down
            if (NextFall < 0) {
                StartCoroutine(FallingDown());
                NextFall = FallingCooldown;
            }
            NextFall -= FallingSpeed * Time.deltaTime;
        }

        if (State == TetrimoState.Preview) {
            transform.Rotate(0, 1f, 0, Space.World);
        }

        // Debugging Feature
        if (Input.GetKeyDown(KeyCode.F2)) {
            KillAndReload();
        }
    }
    #endregion

    #region Coroutines: http://unitygems.com/coroutines/
    /// <summary>
    /// Active the Tetrimo in the preview hub
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivateTetrimoInPreview() {
        yield return 0;
    }
    /// <summary>
    /// Create a new Tetrimo and place it in the preview hub.
    /// </summary>
    /// <returns></returns>
    IEnumerator CreatePreviewStateTetrimo() {
        yield return 0;
    }
    IEnumerator MoveHorizontal() {
        IsMovingHorizontal = true;

        float moved     = 0.0f;
        float direction = Input.GetAxis("Horizontal");

        if ((this.CanMoveRight && direction > 0) || (this.CanMoveLeft && direction < 0)) {
            while (moved <= 1.0f) {
                float moveStep = Mathf.Min(HorizontalSpeed * Time.deltaTime, 1.1f - moved);   // 1.1f since float has some rounding problems!

                if (direction > 0)
                    transform.Translate(Vector3.right * moveStep, Space.World);
                else if (direction < 0)
                    transform.Translate(Vector3.left  * moveStep, Space.World);

                moved += moveStep;
                yield return 0;
            }

            // We will correct the actual position of each stone when it landed
            transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }

        IsMovingHorizontal = false;
    }
    IEnumerator FallingDown() {
        if (this.CanMoveDown) {
            transform.Translate(Vector3.down, Space.World);
        }
        else {
            if (State == TetrimoState.Falling) {
                // After the Tetrimo has landed, the player can move it in the first 400ms.
                this.audio.Play();
                State = TetrimoState.Landed;
                yield return new WaitForSeconds(0.4f);
                while (IsMovingHorizontal)  // Wait for end of possible MoveHorizontal - calls
                    yield return new WaitForEndOfFrame();

                if (this.CanMoveDown) {
                    State = TetrimoState.Falling;
                }
                else {
                    State = TetrimoState.Fixed;
                    foreach (Transform child in transform) {
                        Tetrimo.FieldMatrix[Mathf.RoundToInt(child.position.y), Mathf.RoundToInt(child.position.x)] = child.gameObject;
                    }
                    ArrayList lines = FindLines();

                    if (lines.Count > 0) {
                        int FourLineBonus = 0;

                        // Destruction animation
                        foreach (int line in lines) {
                            int y = Mathf.CeilToInt(line);

                            for (int i = FieldSize.Left; i <= FieldSize.Right; i++) {
                                Instantiate(TetrimoExplosionPrefab, FieldMatrix[y, i].transform.position, Quaternion.identity);
                                Destroy(FieldMatrix[y, i]);
                                FieldMatrix[y, i] = null;
                            }
                        }
                        if (lines.Count == 4) {
                            FourLineBonus = FieldMatrix.GetLength(1);
                            Instantiate(FourLinesLabelPrefab, new Vector3(transform.position.x, transform.position.y + 1.0f), Quaternion.identity);
                        }

                        // update logic
                        yield return new WaitForEndOfFrame();
                        lines.Sort();
                        lines.Reverse();
                        foreach (int line in lines) {
                            if (line >= FieldSize.Top)
                                continue;
                            LetLinesAboveFalling(line + 1);
                        }

                        // adding scores
                        MatchCamera.Scores += FieldMatrix.GetLength(1) * lines.Count + MatchCamera.Continuous + FourLineBonus;
                        MatchCamera.Continuous++;

                        // check if next level has been reached
                        if (MatchCamera.Scores >= MatchCamera.Level * MatchCamera.ScoresForNextLevel) {
                            MatchCamera.Level = 1 + Mathf.FloorToInt(((float)MatchCamera.Scores) / MatchCamera.ScoresForNextLevel);
                            FallingSpeed++;
                        }
                    }
                    else {
                        MatchCamera.Continuous = 0;
                    }

                    // Create the new Tetrimo as previewed by destroying the previewed Tetrimo, and place a new created Tetrimo
                    // yield return new WaitForSeconds(0.5f);
                    ActivateAndCreateNewPreview();

                    // Extract children (TetrimoParts) from Tetrimo. We will delete the Tetrimo itself.
                    Transform[] children = GetComponentsInChildren<Transform>();
                    for (int i = 0; i < children.Length; i++)
                        children[i].parent = null;
                    Destroy(gameObject);
                }
            }
        }
    }

    void ActivateAndCreateNewPreview() {
        GameObject newGameObject = (GameObject)Instantiate(TetrimoPrefab, Vector3.zero, Quaternion.identity);
        Tetrimo newFallingTetrimo       = newGameObject.GetComponent<Tetrimo>();
        newFallingTetrimo.RotationIndex = NextTetrimo.RotationIndex;
        newFallingTetrimo.ShapeIndex    = NextTetrimo.ShapeIndex;
        newFallingTetrimo.State         = TetrimoState.Spawning;

        foreach (Transform child in newFallingTetrimo.transform) {
            Destroy(child.gameObject);
        }
        Destroy(NextTetrimo.gameObject);

        newGameObject = (GameObject)Instantiate(TetrimoPrefab, Vector3.zero, Quaternion.identity);
        Tetrimo newPreviewTetrimo = newGameObject.GetComponent<Tetrimo>();
        newPreviewTetrimo.State = TetrimoState.Preview;
    }
    IEnumerator RotateTetrimo() {
        this.audio.Play();

        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        RotationIndex = (RotationIndex + 1) % Shapes.GetLength(2);
        CreateShape();
        yield return 0;
    }
    void LetLinesAboveFalling(float lineToBegin) {
        int bottom = Mathf.RoundToInt(lineToBegin);

        for (int y = bottom; y <= FieldSize.Top; y++) {
            for (int x = FieldSize.Left; x <= FieldSize.Right; x++) {
                FieldMatrix[y-1, x] = FieldMatrix[y, x];
                if (FieldMatrix[y-1, x] != null)
                    FieldMatrix[y-1, x].transform.Translate(Vector3.down);

                FieldMatrix[y, x] = null;
            }
        }
    }
    #endregion

    #region Helper functions
    void CreateShape() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < Shapes.GetLength(2); index++) {
            GameObject tmp = (GameObject)Instantiate(TetrimoPartPrefab, Shapes[ShapeIndex, RotationIndex, index], Quaternion.identity);

            tmp.transform.Translate(transform.position);
            tmp.transform.parent = gameObject.transform;
            tmp.renderer.material.color = Colors[ShapeIndex];
        }
    }
    /// <summary>
    /// Check matrix for full filled lines, where the current Tetrimo is part of it.
    /// </summary>
    /// <returns>y-indices of type int</returns>
    ArrayList FindLines() {
        ArrayList lines = new ArrayList();
        Hashtable open  = new Hashtable(); // Never use classes or methods, that aren't listened in the Unity reference, if you aim for cross-plattform-support.

        foreach (Transform child in transform) {
            int line = Mathf.CeilToInt(child.position.y);
            if(!open.ContainsKey(line))
                open.Add(line, line);
        }

        foreach (int y in open.Keys) {
            bool fullline = true;
            for (int i = FieldSize.Left; i <= FieldSize.Right; i++)
                fullline &= FieldMatrix[Mathf.RoundToInt(y), i] != null;
            if (fullline)
                lines.Add((int) y);
        }
        return lines;
    }
    #endregion

    #region Debugging helper function
    private void KillAndReload() {
        for (int c = FieldSize.Left; c <= FieldSize.Right; c++) {
            for (int r = FieldSize.Bottom; r <= FieldSize.Top; r++) {
                if (FieldMatrix[r, c] != null) {
                    Destroy(FieldMatrix[r, c]);
                }
            }
        }
        Tetrimo[] gameObjects = GetComponents<Tetrimo>();
        foreach (Tetrimo go in gameObjects) {
            if (go.State == TetrimoState.Falling)
                continue;

            Destroy(go.gameObject);
        }

        for (int c = FieldSize.Left; c <= FieldSize.Right; c++) {
            for (int r = FieldSize.Bottom; r <= FieldSize.Top; r++) {
                if (FieldMatrix[r, c] != null) {
                    FieldMatrix[r, c] = (GameObject) Instantiate(TetrimoPartPrefab, new Vector2(c,r), Quaternion.identity);
                    FieldMatrix[r, c].renderer.material.color = Color.white;
                }
            }
        }
    }
    #endregion
}
