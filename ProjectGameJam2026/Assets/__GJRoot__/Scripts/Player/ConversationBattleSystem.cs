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

    [Header("Player Choices")]
    public ConversationType player1Choice = ConversationType.None;
    public ConversationType player2Choice = ConversationType.None;

    [Header("Scores")]
    public int player1Score;
    public int player2Score;

    [Header("Round Settings")]
    public int roundTime = 10;
    public float nextRoundDelay = 3f;

    [Header("UI References")]
    public TMP_Text countdownText;
    public TMP_Text player1StatusText;
    public TMP_Text player2StatusText;
    public TMP_Text resultText;

    [Header("Countdown Animation")]
    public float countdownNormalScale = 1f;
    public float countdownBigScale = 1.35f;
    public float countdownAnimSpeed = 8f;

    private bool roundActive;
    private bool roundFinished;
    private int currentTime;

    private void Start()
    {
        StartNewRound();
    }

    private void Update()
    {
        if (!roundActive || roundFinished) return;

        ReadPlayer1Input();
        ReadPlayer2Input();

        if (player1Choice != ConversationType.None && player2Choice != ConversationType.None)
        {
            FinishRound();
        }
    }

    private void ReadPlayer1Input()
    {
        if (player1Choice != ConversationType.None) return;

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
        if (player2Choice != ConversationType.None) return;

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
        player1StatusText.text = "Player 1 locked in its answer";
        Debug.Log("Player 1 chose: " + choice);
    }

    private void LockPlayer2Choice(ConversationType choice)
    {
        player2Choice = choice;
        player2StatusText.text = "Player 2 locked in its answer";
        Debug.Log("Player 2 chose: " + choice);
    }

    private void StartNewRound()
    {
        StopAllCoroutines();

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
                Time.deltaTime * countdownAnimSpeed
            );

            yield return null;
        }

        countdownText.transform.localScale = Vector3.one * countdownNormalScale;
    }

    private void FinishRound()
    {
        if (roundFinished) return;

        roundFinished = true;
        roundActive = false;

        StopAllCoroutines();

        countdownText.text = "0";

        string player1Result = GetChoiceText(player1Choice);
        string player2Result = GetChoiceText(player2Choice);

        string winnerText = GetWinnerText();

        resultText.text =
            "Player 1 chose: " + player1Result + "\n" +
            "Player 2 chose: " + player2Result + "\n\n" +
            winnerText;

        Debug.Log("----- ROUND FINISHED -----");
        Debug.Log("Player 1 chose: " + player1Result);
        Debug.Log("Player 2 chose: " + player2Result);
        Debug.Log(winnerText);

        StartCoroutine(NextRoundRoutine());
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
        if (player1Choice == ConversationType.None && player2Choice == ConversationType.None)
        {
            return "Nobody answered";
        }

        if (player1Choice == ConversationType.None)
        {
            player2Score++;
            return "Player 2 wins because Player 1 did not answer";
        }

        if (player2Choice == ConversationType.None)
        {
            player1Score++;
            return "Player 1 wins because Player 2 did not answer";
        }

        if (player1Choice > player2Choice)
        {
            player1Score++;
            return "Player 1 wins";
        }

        if (player2Choice > player1Choice)
        {
            player2Score++;
            return "Player 2 wins";
        }

        return "Draw";
    }

    private IEnumerator NextRoundRoutine()
    {
        yield return new WaitForSeconds(nextRoundDelay);
        StartNewRound();
    }
}