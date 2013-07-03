using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class HighScore {
    /// <summary>
    /// HighScore Array must be sorted! With the #0-element has the lowest value.
    /// </summary>
    static int[] Scores = new int[3] { 0, 0, 0};

    static HighScore() {
        for (int i = 0; i < Scores.Length; i++)
            Scores[i] = PlayerPrefs.GetInt("HighScore#" + i, 0);
    }

    static public int get(int i) {
        return Scores[i];
    }

    /// <summary>
    /// Try to add the newScore to the HighScore list.
    /// If successed, the new highscore is saved automatically.
    /// </summary>
    /// <param name="newScore"></param>
    /// <returns>'true' if newScore has been placed in the highscore list. Otherwise false.</returns>
    static public bool TryAddHighScore(int newScore) {
        int found = 0;

        for (int i = 0; i < Scores.Length; i++) {
            if (newScore >= Scores[i])
                found++;
        }

        int newValue = newScore,
            tmp;
        if(found == 0)
            return false;
        while (found > 0) {
            int index = (found - 1);
            tmp           = Scores[index];
            Scores[index] = newValue;
            newValue      = tmp;
            PlayerPrefs.SetInt("HighScore#" + index, Scores[index]);
            found--;
        }
        return true;
    }
}