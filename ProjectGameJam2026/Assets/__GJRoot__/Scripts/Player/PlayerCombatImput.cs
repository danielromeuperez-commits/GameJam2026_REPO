using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerAttackInput : MonoBehaviour
{
    [System.Serializable]
    public class WordDifficulty
    {
        public string difficultyName;
        public int damage;

        [TextArea(2, 5)]
        public string[] words;
    }

    [System.Serializable]
    public class WordVisualRow
    {
        public string rowName;
        public ComboLetterUI[] letters;
    }

    private class ActiveWord
    {
        public string word;
        public Key[] sequence;
        public int damage;
        public string difficultyName;
    }

    [Header("Health")]
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    [Header("Word Libraries")]
    public WordDifficulty easyWords = new WordDifficulty
    {
        difficultyName = "Easy",
        damage = 20,
        words = new string[]
        {
            "RIMA",
            "LUNA",
            "SOL",
            "MAR",
            "FLOR"
        }
    };

    public WordDifficulty mediumWords = new WordDifficulty
    {
        difficultyName = "Medium",
        damage = 40,
        words = new string[]
        {
            "DESTINO",
            "CAMINO",
            "POEMA",
            "SILENCIO",
            "ESTRELLA"
        }
    };

    public WordDifficulty hardWords = new WordDifficulty
    {
        difficultyName = "Hard",
        damage = 70,
        words = new string[]
        {
            "INSPIRACION",
            "MELANCOLIA",
            "CONSTELACION",
            "ESPERANZA",
            "ESTERNOCLEIDOMASTOIDEO"
        }
    };

    [Header("Visual Rows")]
    public WordVisualRow easyRow;
    public WordVisualRow mediumRow;
    public WordVisualRow hardRow;

    [Header("UI")]
    public TMP_Text attackInfoText;

    [Header("SFX")]
    public int typedLetterSFXIndex = 3;
    public int wrongLetterSFXIndex = 3;

    public bool useRandomPitchForTypingSFX = true;

    public float typedLetterMinPitch = 0.95f;
    public float typedLetterMaxPitch = 1.08f;

    public float wrongLetterMinPitch = 0.65f;
    public float wrongLetterMaxPitch = 0.8f;

    public bool attackPerformed;

    private bool player1CanAttack;
    private bool player2CanAttack;

    private List<Key> inputBuffer = new List<Key>();

    private ActiveWord[] activeWords = new ActiveWord[3];

    private int activeRowIndex = -1;

    private void Update()
    {
        if (!player1CanAttack && !player2CanAttack) return;
        if (attackPerformed) return;

        RegisterInput();
    }

    private void RegisterInput()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        foreach (KeyControl keyControl in kb.allKeys)
        {
            if (keyControl.wasPressedThisFrame)
            {
                AddKey(keyControl.keyCode);
                return;
            }
        }
    }

    private void AddKey(Key key)
    {
        int previousRowIndex = activeRowIndex;
        int previousExpectedIndex = inputBuffer.Count;

        inputBuffer.Add(key);

        Debug.Log("Input: " + key);
        Debug.Log("Buffer: " + string.Join(",", inputBuffer));

        CheckWords(previousRowIndex, previousExpectedIndex);
    }

    private void PlayTypedLetterSFX()
    {
        if (AudioManager.Instance == null)
            return;

        if (useRandomPitchForTypingSFX)
        {
            AudioManager.Instance.PlaySFXRandomPitch(
                typedLetterSFXIndex,
                typedLetterMinPitch,
                typedLetterMaxPitch
            );
        }
        else
        {
            AudioManager.Instance.PlaySFX(typedLetterSFXIndex);
        }
    }

    private void PlayWrongLetterSFX()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySFXRandomPitch(
            wrongLetterSFXIndex,
            wrongLetterMinPitch,
            wrongLetterMaxPitch
        );
    }

    private void CheckWords(int previousRowIndex, int previousExpectedIndex)
    {
        List<int> matchingRows = GetMatchingRowsWithCurrentInput();

        if (matchingRows.Count == 0)
        {
            PlayWrongLetterSFX();
            WrongInput();
            return;
        }

        activeRowIndex = matchingRows[0];

        PlayTypedLetterSFX();

        UpdateWordVisuals(matchingRows);

        int completedLetterIndex = inputBuffer.Count - 1;

        for (int i = 0; i < matchingRows.Count; i++)
        {
            int rowIndex = matchingRows[i];

            if (IsValidVisualLetter(rowIndex, completedLetterIndex))
            {
                GetRowByIndex(rowIndex).letters[completedLetterIndex].PlayTypedWave();
            }
        }

        for (int i = 0; i < matchingRows.Count; i++)
        {
            int rowIndex = matchingRows[i];
            ActiveWord selectedWord = activeWords[rowIndex];

            if (selectedWord != null && inputBuffer.Count == selectedWord.sequence.Length)
            {
                activeRowIndex = rowIndex;
                ExecuteWord(selectedWord);
                return;
            }
        }
    }

    private List<int> GetMatchingRowsWithCurrentInput()
    {
        List<int> matchingRows = new List<int>();

        for (int i = 0; i < activeWords.Length; i++)
        {
            ActiveWord activeWord = activeWords[i];

            if (activeWord == null || activeWord.sequence == null || activeWord.sequence.Length == 0)
                continue;

            if (WordStartsWithCurrentInput(activeWord.sequence))
            {
                matchingRows.Add(i);
            }
        }

        return matchingRows;
    }

    private bool WordStartsWithCurrentInput(Key[] wordSequence)
    {
        if (inputBuffer.Count > wordSequence.Length)
            return false;

        for (int i = 0; i < inputBuffer.Count; i++)
        {
            if (inputBuffer[i] != wordSequence[i])
                return false;
        }

        return true;
    }

    private void ExecuteWord(ActiveWord word)
    {
        if (word == null || attackPerformed)
            return;

        PlayCompletedWordAnimation(activeRowIndex);

        if (player1CanAttack)
        {
            if (player2Health != null)
                player2Health.TakeDamage(word.damage);

            FinishAttack("Player 1 used " + word.word + " | Damage: " + word.damage);
        }
        else if (player2CanAttack)
        {
            if (player1Health != null)
                player1Health.TakeDamage(word.damage);

            FinishAttack("Player 2 used " + word.word + " | Damage: " + word.damage);
        }
    }

    private void FinishAttack(string msg)
    {
        attackPerformed = true;

        player1CanAttack = false;
        player2CanAttack = false;

        inputBuffer.Clear();
        activeRowIndex = -1;

        if (attackInfoText != null)
            attackInfoText.text = msg;

        Debug.Log(msg);
    }

    private void WrongInput()
    {
        Debug.Log("Wrong word input");

        if (attackInfoText != null)
            attackInfoText.text = "Wrong word! Try again.";

        List<int> previousMatchingRows = GetRowsMatchingPreviousInput();

        for (int i = 0; i < previousMatchingRows.Count; i++)
        {
            PlayWrongWordFeedback(previousMatchingRows[i]);
        }

        inputBuffer.Clear();
        activeRowIndex = -1;

        Invoke(nameof(ResetAllWordVisuals), 0.35f);
    }

    private List<int> GetRowsMatchingPreviousInput()
    {
        List<int> matchingRows = new List<int>();

        if (inputBuffer.Count <= 1)
        {
            for (int i = 0; i < activeWords.Length; i++)
            {
                if (activeWords[i] != null)
                    matchingRows.Add(i);
            }

            return matchingRows;
        }

        int lastIndex = inputBuffer.Count - 1;
        Key wrongKey = inputBuffer[lastIndex];
        inputBuffer.RemoveAt(lastIndex);

        for (int i = 0; i < activeWords.Length; i++)
        {
            ActiveWord activeWord = activeWords[i];

            if (activeWord == null || activeWord.sequence == null || activeWord.sequence.Length == 0)
                continue;

            if (WordStartsWithCurrentInput(activeWord.sequence))
            {
                matchingRows.Add(i);
            }
        }

        inputBuffer.Add(wrongKey);

        return matchingRows;
    }

    public void EnablePlayer1Attack()
    {
        player1CanAttack = true;
        player2CanAttack = false;
        attackPerformed = false;

        inputBuffer.Clear();
        activeRowIndex = -1;

        if (activeWords[0] == null || activeWords[1] == null || activeWords[2] == null)
        {
            PrepareAttackWords();
        }

        SetFirstLettersActive();

        if (attackInfoText != null)
            attackInfoText.text = "Player 1 attack phase";

        Debug.Log("Player 1 can attack");
    }

    public void EnablePlayer2Attack()
    {
        player2CanAttack = true;
        player1CanAttack = false;
        attackPerformed = false;

        inputBuffer.Clear();
        activeRowIndex = -1;

        if (activeWords[0] == null || activeWords[1] == null || activeWords[2] == null)
        {
            PrepareAttackWords();
        }

        SetFirstLettersActive();

        if (attackInfoText != null)
            attackInfoText.text = "player 2 attack phase";

        Debug.Log("Player 2 can attack");
    }

    public void PrepareAttackWords()
    {
        inputBuffer.Clear();
        activeRowIndex = -1;
        attackPerformed = false;

        GenerateRandomWords();
        SetupWordVisuals();
        ResetAllWordVisuals();
        SetFirstLettersActive();
    }

    public void DisableAttacks()
    {
        player1CanAttack = false;
        player2CanAttack = false;

        inputBuffer.Clear();
        activeRowIndex = -1;

        ResetAllWordVisuals();
    }

    private void GenerateRandomWords()
    {
        activeWords[0] = CreateActiveWordFromLibrary(easyWords);
        activeWords[1] = CreateActiveWordFromLibrary(mediumWords);
        activeWords[2] = CreateActiveWordFromLibrary(hardWords);
    }

    private ActiveWord CreateActiveWordFromLibrary(WordDifficulty library)
    {
        if (library == null || library.words == null || library.words.Length == 0)
        {
            Debug.LogWarning("Word library is empty.");
            return null;
        }

        string randomWord = library.words[Random.Range(0, library.words.Length)];
        string cleanedWord = CleanWord(randomWord);

        Key[] sequence = ConvertWordToKeys(cleanedWord);

        return new ActiveWord
        {
            word = cleanedWord,
            sequence = sequence,
            damage = library.damage,
            difficultyName = library.difficultyName
        };
    }

    private string CleanWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return "";

        string cleaned = word.ToUpper();
        cleaned = cleaned.Replace("Á", "A");
        cleaned = cleaned.Replace("É", "E");
        cleaned = cleaned.Replace("Í", "I");
        cleaned = cleaned.Replace("Ó", "O");
        cleaned = cleaned.Replace("Ú", "U");
        cleaned = cleaned.Replace("Ü", "U");
        cleaned = cleaned.Replace("Ñ", "N");
        cleaned = cleaned.Replace(" ", "");

        return cleaned;
    }

    private Key[] ConvertWordToKeys(string word)
    {
        List<Key> keys = new List<Key>();

        for (int i = 0; i < word.Length; i++)
        {
            Key key = CharToKey(word[i]);

            if (key != Key.None)
                keys.Add(key);
        }

        return keys.ToArray();
    }

    private Key CharToKey(char character)
    {
        switch (character)
        {
            case 'A': return Key.A;
            case 'B': return Key.B;
            case 'C': return Key.C;
            case 'D': return Key.D;
            case 'E': return Key.E;
            case 'F': return Key.F;
            case 'G': return Key.G;
            case 'H': return Key.H;
            case 'I': return Key.I;
            case 'J': return Key.J;
            case 'K': return Key.K;
            case 'L': return Key.L;
            case 'M': return Key.M;
            case 'N': return Key.N;
            case 'O': return Key.O;
            case 'P': return Key.P;
            case 'Q': return Key.Q;
            case 'R': return Key.R;
            case 'S': return Key.S;
            case 'T': return Key.T;
            case 'U': return Key.U;
            case 'V': return Key.V;
            case 'W': return Key.W;
            case 'X': return Key.X;
            case 'Y': return Key.Y;
            case 'Z': return Key.Z;
            default: return Key.None;
        }
    }

    private void SetupWordVisuals()
    {
        SetupRowVisuals(easyRow, activeWords[0]);
        SetupRowVisuals(mediumRow, activeWords[1]);
        SetupRowVisuals(hardRow, activeWords[2]);
    }

    private void SetupRowVisuals(WordVisualRow row, ActiveWord activeWord)
    {
        if (row == null || row.letters == null || activeWord == null)
            return;

        string word = activeWord.word;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] == null)
                continue;

            if (i < word.Length)
            {
                row.letters[i].SetLetter(word[i].ToString());
            }
            else
            {
                row.letters[i].HideLetter();
            }
        }

        if (word.Length > row.letters.Length)
        {
            Debug.LogWarning(
                "The word " + word + " is longer than the visual row " + row.rowName +
                ". Add more letter slots to the row."
            );
        }
    }

    private void UpdateWordVisuals(List<int> activeRows)
    {
        for (int rowIndex = 0; rowIndex < activeWords.Length; rowIndex++)
        {
            WordVisualRow row = GetRowByIndex(rowIndex);

            if (row == null || row.letters == null)
                continue;

            bool rowIsActive = activeRows.Contains(rowIndex);

            for (int i = 0; i < row.letters.Length; i++)
            {
                ComboLetterUI letter = row.letters[i];

                if (letter == null || !letter.gameObject.activeSelf)
                    continue;

                if (!rowIsActive)
                {
                    letter.SetNormal();
                    continue;
                }

                if (i < inputBuffer.Count)
                {
                    letter.SetCompleted();
                }
                else if (i == inputBuffer.Count)
                {
                    letter.SetActive();
                }
                else
                {
                    letter.SetNormal();
                }
            }
        }
    }

    private void SetFirstLettersActive()
    {
        SetFirstLetterActiveInRow(easyRow);
        SetFirstLetterActiveInRow(mediumRow);
        SetFirstLetterActiveInRow(hardRow);
    }

    private void SetFirstLetterActiveInRow(WordVisualRow row)
    {
        if (row == null || row.letters == null || row.letters.Length == 0)
            return;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] != null && row.letters[i].gameObject.activeSelf)
            {
                row.letters[i].SetActive();
                return;
            }
        }
    }

    private void PlayWrongWordFeedback(int rowIndex)
    {
        WordVisualRow row = GetRowByIndex(rowIndex);

        if (row == null || row.letters == null)
            return;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] == null || !row.letters[i].gameObject.activeSelf)
                continue;

            row.letters[i].SetWrong();
            row.letters[i].PlayWrongFeedback();
        }
    }

    private void PlayCompletedWordAnimation(int rowIndex)
    {
        WordVisualRow row = GetRowByIndex(rowIndex);

        if (row == null || row.letters == null)
            return;

        int visibleCount = 0;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] != null && row.letters[i].gameObject.activeSelf)
                visibleCount++;
        }

        for (int i = 0; i < visibleCount; i++)
        {
            if (row.letters[i] == null)
                continue;

            int distanceFromEdge = Mathf.Min(i, visibleCount - 1 - i);
            float delay = distanceFromEdge * 0.08f;

            row.letters[i].PlaySuccessWave(delay);
        }
    }

    private void ResetAllWordVisuals()
    {
        ResetRowVisuals(easyRow);
        ResetRowVisuals(mediumRow);
        ResetRowVisuals(hardRow);
    }

    private void ResetRowVisuals(WordVisualRow row)
    {
        if (row == null || row.letters == null)
            return;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] != null && row.letters[i].gameObject.activeSelf)
                row.letters[i].SetNormal();
        }
    }

    private bool IsValidVisualLetter(int rowIndex, int letterIndex)
    {
        WordVisualRow row = GetRowByIndex(rowIndex);

        if (row == null || row.letters == null)
            return false;

        if (letterIndex < 0 || letterIndex >= row.letters.Length)
            return false;

        return row.letters[letterIndex] != null;
    }

    private WordVisualRow GetRowByIndex(int index)
    {
        switch (index)
        {
            case 0:
                return easyRow;

            case 1:
                return mediumRow;

            case 2:
                return hardRow;

            default:
                return null;
        }
    }
}