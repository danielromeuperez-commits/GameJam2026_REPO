using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackInput : MonoBehaviour
{
    [System.Serializable]
    public class Combo
    {
        public Key[] sequence;
        public int damage;
        public string name;
    }

    public PlayerHealth player1Health;
    public PlayerHealth player2Health;

    public Combo[] combos;

    public bool attackPerformed;

    private bool player1CanAttack;
    private bool player2CanAttack;

    private List<Key> inputBuffer = new List<Key>();

    public float bufferTime = 1f;
    private float lastInputTime;

    void Update()
    {
        RegisterInput();

        // limpiar buffer si pasa demasiado tiempo
        if (Time.time - lastInputTime > bufferTime)
        {
            inputBuffer.Clear();
        }

        CheckCombos();
    }

    void RegisterInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // INPUT CORRECTO (sin mapeos raros)
        if (kb.aKey.wasPressedThisFrame) AddKey(Key.A);
        if (kb.sKey.wasPressedThisFrame) AddKey(Key.S);
        if (kb.dKey.wasPressedThisFrame) AddKey(Key.D);
        if (kb.fKey.wasPressedThisFrame) AddKey(Key.F);
        if (kb.gKey.wasPressedThisFrame) AddKey(Key.G);
        if (kb.hKey.wasPressedThisFrame) AddKey(Key.H);
        if (kb.jKey.wasPressedThisFrame) AddKey(Key.J);
        if (kb.kKey.wasPressedThisFrame) AddKey(Key.K);
    }

    void AddKey(Key key)
    {
        inputBuffer.Add(key);
        lastInputTime = Time.time;

        Debug.Log("Input: " + key);
        Debug.Log("Buffer: " + string.Join(",", inputBuffer));
    }

    void CheckCombos()
    {
        // prioridad a combos largos (más correctos en fighting games)
        for (int i = combos.Length - 1; i >= 0; i--)
        {
            if (Match(combos[i].sequence))
            {
                ExecuteCombo(combos[i]);
                inputBuffer.Clear();
                return;
            }
        }
    }

    void ExecuteCombo(Combo combo)
    {
        if (player1CanAttack)
        {
            player2Health.TakeDamage(combo.damage);
            FinishAttack("Player 1 used combo: " + combo.name + " DMG: " + combo.damage);
        }
        else if (player2CanAttack)
        {
            player1Health.TakeDamage(combo.damage);
            FinishAttack("Player 2 used combo: " + combo.name + " DMG: " + combo.damage);
        }
        else
        {
            Debug.Log("Combo detected but no player is active");
        }
    }

    bool Match(Key[] combo)
    {
        if (inputBuffer.Count < combo.Length) return false;

        int start = inputBuffer.Count - combo.Length;

        for (int i = 0; i < combo.Length; i++)
        {
            if (inputBuffer[start + i] != combo[i])
                return false;
        }

        return true;
    }

    void FinishAttack(string msg)
    {
        attackPerformed = true;
        DisableAttacks();
        Debug.Log(msg);
    }

    public void EnablePlayer1Attack()
    {
        player1CanAttack = true;
        player2CanAttack = false;
        attackPerformed = false;

        Debug.Log("Player 1 can attack");
    }

    public void EnablePlayer2Attack()
    {
        player2CanAttack = true;
        player1CanAttack = false;
        attackPerformed = false;

        Debug.Log("Player 2 can attack");
    }

    public void DisableAttacks()
    {
        player1CanAttack = false;
        player2CanAttack = false;
    }
}