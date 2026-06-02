using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerAttackInput : MonoBehaviour
{
    [System.Serializable]
    public class Combo
    {
        public Key[] sequence;
        public int damage;
        public string name;
    }

    [System.Serializable]
    public class ComboVisualRow
    {
        public string rowName;
        public ComboLetterUI[] letters;
    }

    [Header("Health")]
    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    [Header("Combos")]
    public Combo[] combos;

    [Header("Combo Visual Rows")]
    public ComboVisualRow[] comboVisualRows;

    [Header("UI")]
    public TMP_Text attackInfoText;

    public bool attackPerformed;

    private bool player1CanAttack;
    private bool player2CanAttack;

    private List<Key> inputBuffer = new List<Key>();

    private int activeRowIndex = -1;
    private int lastExpectedLetterIndex = 0;

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

        CheckCombos(previousRowIndex, previousExpectedIndex);
    }

    private void CheckCombos(int previousRowIndex, int previousExpectedIndex)
    {
        int matchingComboIndex = -1;

        for (int i = 0; i < combos.Length; i++)
        {
            Combo combo = combos[i];

            if (combo == null || combo.sequence == null || combo.sequence.Length == 0)
                continue;

            if (ComboStartsWithCurrentInput(combo.sequence))
            {
                matchingComboIndex = i;
                break;
            }
        }

        if (matchingComboIndex == -1)
        {
            WrongInput(previousRowIndex, previousExpectedIndex);
            return;
        }

        activeRowIndex = matchingComboIndex;
        lastExpectedLetterIndex = inputBuffer.Count;

        UpdateComboVisuals(activeRowIndex);

        Combo activeCombo = combos[activeRowIndex];

        int completedLetterIndex = inputBuffer.Count - 1;

        if (IsValidVisualLetter(activeRowIndex, completedLetterIndex))
        {
            comboVisualRows[activeRowIndex].letters[completedLetterIndex].PlayCorrectPop();
        }

        if (inputBuffer.Count == activeCombo.sequence.Length)
        {
            ExecuteCombo(activeCombo);
        }
    }

    private bool ComboStartsWithCurrentInput(Key[] comboSequence)
    {
        if (inputBuffer.Count > comboSequence.Length)
            return false;

        for (int i = 0; i < inputBuffer.Count; i++)
        {
            if (inputBuffer[i] != comboSequence[i])
                return false;
        }

        return true;
    }

    private void ExecuteCombo(Combo combo)
    {
        if (combo == null || attackPerformed)
            return;

        PlayCompletedComboAnimation(activeRowIndex);

        if (player1CanAttack)
        {
            if (player2Health != null)
                player2Health.TakeDamage(combo.damage);

            FinishAttack("Player 1 used " + combo.name + " | Damage: " + combo.damage);
        }
        else if (player2CanAttack)
        {
            if (player1Health != null)
                player1Health.TakeDamage(combo.damage);

            FinishAttack("Player 2 used " + combo.name + " | Damage: " + combo.damage);
        }
    }

    private void FinishAttack(string msg)
    {
        attackPerformed = true;

        player1CanAttack = false;
        player2CanAttack = false;

        inputBuffer.Clear();
        activeRowIndex = -1;
        lastExpectedLetterIndex = 0;

        if (attackInfoText != null)
            attackInfoText.text = msg;

        Debug.Log(msg);
    }

    private void WrongInput(int previousRowIndex, int previousExpectedIndex)
    {
        Debug.Log("Wrong combo input");

        if (attackInfoText != null)
            attackInfoText.text = "Wrong combo! Try again.";

        if (IsValidVisualLetter(previousRowIndex, previousExpectedIndex))
        {
            comboVisualRows[previousRowIndex].letters[previousExpectedIndex].PlayWrongFeedback();
        }

        inputBuffer.Clear();
        activeRowIndex = -1;
        lastExpectedLetterIndex = 0;

        Invoke(nameof(ResetAllComboVisuals), 0.3f);
    }

    public void EnablePlayer1Attack()
    {
        player1CanAttack = true;
        player2CanAttack = false;
        attackPerformed = false;

        inputBuffer.Clear();
        activeRowIndex = -1;
        lastExpectedLetterIndex = 0;

        ResetAllComboVisuals();

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
        lastExpectedLetterIndex = 0;

        ResetAllComboVisuals();

        if (attackInfoText != null)
            attackInfoText.text = "Player 2 attack phase";

        Debug.Log("Player 2 can attack");
    }

    public void DisableAttacks()
    {
        player1CanAttack = false;
        player2CanAttack = false;

        inputBuffer.Clear();
        activeRowIndex = -1;
        lastExpectedLetterIndex = 0;

        ResetAllComboVisuals();
    }

    private void UpdateComboVisuals(int activeRow)
    {
        for (int row = 0; row < comboVisualRows.Length; row++)
        {
            ComboVisualRow visualRow = comboVisualRows[row];

            if (visualRow == null || visualRow.letters == null)
                continue;

            for (int i = 0; i < visualRow.letters.Length; i++)
            {
                ComboLetterUI letter = visualRow.letters[i];

                if (letter == null)
                    continue;

                if (row != activeRow)
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

    private void PlayCompletedComboAnimation(int rowIndex)
    {
        if (comboVisualRows == null) return;
        if (rowIndex < 0 || rowIndex >= comboVisualRows.Length) return;

        ComboVisualRow row = comboVisualRows[rowIndex];

        if (row == null || row.letters == null) return;

        for (int i = 0; i < row.letters.Length; i++)
        {
            if (row.letters[i] != null)
                row.letters[i].PlaySuccessBounce();
        }
    }
    private void ResetAllComboVisuals()
    {
        if (comboVisualRows == null) return;

        for (int row = 0; row < comboVisualRows.Length; row++)
        {
            ComboVisualRow visualRow = comboVisualRows[row];

            if (visualRow == null || visualRow.letters == null)
                continue;

            for (int i = 0; i < visualRow.letters.Length; i++)
            {
                if (visualRow.letters[i] != null)
                    visualRow.letters[i].SetNormal();
            }
        }
    }

    private bool IsValidVisualLetter(int rowIndex, int letterIndex)
    {
        if (comboVisualRows == null) return false;
        if (rowIndex < 0 || rowIndex >= comboVisualRows.Length) return false;

        ComboVisualRow row = comboVisualRows[rowIndex];

        if (row == null || row.letters == null) return false;
        if (letterIndex < 0 || letterIndex >= row.letters.Length) return false;

        return row.letters[letterIndex] != null;
    }
}