using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConversationBattleSystem : MonoBehaviour
{
    public enum ConversationType
    {
        None = -1,
        Meh = 0,
        Bueno = 1,
        MuyBueno = 2
    }

    public enum BattlePhase
    {
        Conversation,
        Attack
    }

    [Header("Player Choices")]
    public ConversationType player1Choice = ConversationType.None;
    public ConversationType player2Choice = ConversationType.None;

    [Header("Scores")]
    public int player1Score;
    public int player2Score;

    [Header("Conversation Time")]
    public int roundTime = 10;

    [Header("Attack Time")]
    public float attackTime = 10f;

    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text player1StatusText;
    public TMP_Text player2StatusText;
    public TMP_Text resultText;

    [Header("Countdown Animation")]
    public float countdownNormalScale = 1f;
    public float countdownBigScale = 1.35f;
    public float countdownAnimSpeed = 8f;

    [Header("Attack System")]
    public PlayerAttackInput attackSystem;

    private BattlePhase currentPhase;

    private bool roundActive;
    private bool roundFinished;
    private int currentTime;

    private void Start()
    {
        StartNewRound();
    }

    private void Update()
    {
        if (currentPhase != BattlePhase.Conversation)
            return;

        if (!roundActive || roundFinished)
            return;

        ReadPlayer1Input();
        ReadPlayer2Input();

        if (player1Choice != ConversationType.None &&
            player2Choice != ConversationType.None)
        {
            FinishRound();
        }
    }

    private void ReadPlayer1Input()
    {
        if (player1Choice != ConversationType.None)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.Meh);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.Bueno);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.MuyBueno);
        }
    }

    private void ReadPlayer2Input()
    {
        if (player2Choice != ConversationType.None)
            return;

        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.Meh);
        }
        else if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.Bueno);
        }
        else if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.MuyBueno);
        }
    }

    private void LockPlayer1Choice(ConversationType choice)
    {
        player1Choice = choice;
        player1StatusText.text = "Player 1 locked";
    }

    private void LockPlayer2Choice(ConversationType choice)
    {
        player2Choice = choice;
        player2StatusText.text = "Player 2 locked";
    }

    private void StartNewRound()
    {
        StopAllCoroutines();

        currentPhase = BattlePhase.Conversation;

        player1Choice = ConversationType.None;
        player2Choice = ConversationType.None;

        roundActive = true;
        roundFinished = false;

        currentTime = roundTime;

        player1StatusText.text = "";
        player2StatusText.text = "";
        resultText.text = "";

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0 && !roundFinished)
        {
            countdownText.text = currentTime.ToString();

            yield return StartCoroutine(AnimateCountdownNumber());

            yield return new WaitForSeconds(1f);

            currentTime--;
        }

        if (!roundFinished)
        {
            FinishRound();
        }
    }

    private IEnumerator AnimateCountdownNumber()
    {
        countdownText.transform.localScale = Vector3.one * countdownBigScale;

        while (countdownText.transform.localScale.x > countdownNormalScale + 0.01f)
        {
            countdownText.transform.localScale = Vector3.Lerp(
                countdownText.transform.localScale,
                Vector3.one * countdownNormalScale,
                Time.deltaTime * countdownAnimSpeed);

            yield return null;
        }

        countdownText.transform.localScale = Vector3.one * countdownNormalScale;
    }

    private void FinishRound()
    {
        if (roundFinished)
            return;

        roundFinished = true;
        roundActive = false;

        StopAllCoroutines();

        string winnerText = GetWinnerText();

        resultText.text =
            "P1: " + GetChoiceText(player1Choice) + "\n" +
            "P2: " + GetChoiceText(player2Choice) + "\n\n" +
            winnerText;

        StartCoroutine(AttackPhaseRoutine());
    }

    private IEnumerator AttackPhaseRoutine()
    {
        currentPhase = BattlePhase.Attack;

        int winner = GetWinner();

        if (winner == 1)
        {
            attackSystem.EnablePlayer1Attack();
        }
        else if (winner == 2)
        {
            attackSystem.EnablePlayer2Attack();
        }
        else
        {
            countdownText.text = "DRAW";

            yield return new WaitForSeconds(2f);

            StartNewRound();
            yield break;
        }

        float timer = attackTime;

        while (timer > 0f && !attackSystem.attackPerformed)
        {
            countdownText.text = "ATTACK " + Mathf.CeilToInt(timer);

            timer -= Time.deltaTime;

            yield return null;
        }

        attackSystem.DisableAttacks();

        yield return new WaitForSeconds(1f);

        StartNewRound();
    }

    private int GetWinner()
    {
        if (player1Choice == ConversationType.None &&
            player2Choice == ConversationType.None)
            return 0;

        if (player1Choice == ConversationType.None)
            return 2;

        if (player2Choice == ConversationType.None)
            return 1;

        if (player1Choice > player2Choice)
            return 1;

        if (player2Choice > player1Choice)
            return 2;

        return 0;
    }

    private string GetChoiceText(ConversationType choice)
    {
        switch (choice)
        {
            case ConversationType.Meh:
                return "Meh";

            case ConversationType.Bueno:
                return "Bueno";

            case ConversationType.MuyBueno:
                return "Muy Bueno";

            default:
                return "No Answer";
        }
    }

    private string GetWinnerText()
    {
        int winner = GetWinner();

        if (winner == 1)
        {
            player1Score++;
            return "Player 1 Wins";
        }

        if (winner == 2)
        {
            player2Score++;
            return "Player 2 Wins";
        }

        return "Draw";
    }
}