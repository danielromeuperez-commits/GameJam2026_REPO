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

    [Header("Combo Panel")]
    public RectTransform comboPanel;
    public Vector2 comboPanelHiddenRightPosition = new Vector2(2000f, 0f);
    public Vector2 comboPanelVisiblePosition = new Vector2(0f, 0f);
    public Vector2 comboPanelExitLeftPosition = new Vector2(-2000f, 0f);
    public float comboPanelMoveSpeed = 8f;

    [Header("Attack System")]
    public PlayerAttackInput attackSystem;

    private BattlePhase currentPhase;

    private bool roundActive;
    private bool roundFinished;
    private int currentTime;

    private void Start()
    {
        HideComboPanelInstant();
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

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit1Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.Meh);
        }
        else if (kb.digit2Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.Bueno);
        }
        else if (kb.digit3Key.wasPressedThisFrame)
        {
            LockPlayer1Choice(ConversationType.MuyBueno);
        }
    }

    private void ReadPlayer2Input()
    {
        if (player2Choice != ConversationType.None)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit7Key.wasPressedThisFrame || kb.numpad7Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.Meh);
        }
        else if (kb.digit8Key.wasPressedThisFrame || kb.numpad8Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.Bueno);
        }
        else if (kb.digit9Key.wasPressedThisFrame || kb.numpad9Key.wasPressedThisFrame)
        {
            LockPlayer2Choice(ConversationType.MuyBueno);
        }
    }

    private void LockPlayer1Choice(ConversationType choice)
    {
        player1Choice = choice;

        if (player1StatusText != null)
            player1StatusText.text = "Player 1 locked in its answer";

        Debug.Log("Player 1 chose: " + choice);
    }

    private void LockPlayer2Choice(ConversationType choice)
    {
        player2Choice = choice;

        if (player2StatusText != null)
            player2StatusText.text = "Player 2 locked in its answer";

        Debug.Log("Player 2 chose: " + choice);
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

        if (attackSystem != null)
            attackSystem.DisableAttacks();

        HideComboPanelInstant();

        if (player1StatusText != null)
            player1StatusText.text = "";

        if (player2StatusText != null)
            player2StatusText.text = "";

        if (resultText != null)
            resultText.text = "";

        if (countdownText != null)
            countdownText.text = "";

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0 && !roundFinished)
        {
            if (countdownText != null)
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
        if (countdownText == null) yield break;

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

        int winner = GetWinner();

        if (winner == 1)
            player1Score++;
        else if (winner == 2)
            player2Score++;

        if (resultText != null)
        {
            resultText.text =
                "P1: " + GetChoiceText(player1Choice) + "\n" +
                "P2: " + GetChoiceText(player2Choice) + "\n\n" +
                GetWinnerText(winner);
        }

        StartCoroutine(AttackPhaseRoutine(winner));
    }

    private IEnumerator AttackPhaseRoutine(int winner)
    {
        currentPhase = BattlePhase.Attack;

        if (winner == 0)
        {
            if (countdownText != null)
                countdownText.text = "DRAW";

            yield return new WaitForSeconds(2f);

            StartNewRound();
            yield break;
        }

        if (attackSystem == null)
        {
            Debug.LogWarning("Attack System is not assigned.");
            yield return new WaitForSeconds(2f);
            StartNewRound();
            yield break;
        }

        yield return StartCoroutine(OpenComboPanel());

        if (winner == 1)
        {
            attackSystem.EnablePlayer1Attack();
        }
        else if (winner == 2)
        {
            attackSystem.EnablePlayer2Attack();
        }

        float timer = attackTime;

        while (timer > 0f && !attackSystem.attackPerformed)
        {
            if (countdownText != null)
                countdownText.text = "ATTACK " + Mathf.CeilToInt(timer);

            timer -= Time.deltaTime;

            yield return null;
        }

        bool comboWasPerformed = attackSystem.attackPerformed;

        if (!comboWasPerformed)
        {
            attackSystem.DisableAttacks();
        }

        yield return new WaitForSeconds(1f);

        if (comboWasPerformed)
        {
            attackSystem.DisableAttacks();
        }

        yield return StartCoroutine(CloseComboPanel());

        StartNewRound();
    }

    private IEnumerator OpenComboPanel()
    {
        if (comboPanel == null) yield break;

        comboPanel.gameObject.SetActive(true);
        comboPanel.anchoredPosition = comboPanelHiddenRightPosition;

        while (Vector2.Distance(comboPanel.anchoredPosition, comboPanelVisiblePosition) > 1f)
        {
            comboPanel.anchoredPosition = Vector2.Lerp(
                comboPanel.anchoredPosition,
                comboPanelVisiblePosition,
                Time.deltaTime * comboPanelMoveSpeed);

            yield return null;
        }

        comboPanel.anchoredPosition = comboPanelVisiblePosition;
    }

    private IEnumerator CloseComboPanel()
    {
        if (comboPanel == null) yield break;

        while (Vector2.Distance(comboPanel.anchoredPosition, comboPanelExitLeftPosition) > 1f)
        {
            comboPanel.anchoredPosition = Vector2.Lerp(
                comboPanel.anchoredPosition,
                comboPanelExitLeftPosition,
                Time.deltaTime * comboPanelMoveSpeed);

            yield return null;
        }

        comboPanel.anchoredPosition = comboPanelExitLeftPosition;
        comboPanel.gameObject.SetActive(false);
    }

    private void HideComboPanelInstant()
    {
        if (comboPanel != null)
        {
            comboPanel.anchoredPosition = comboPanelExitLeftPosition;
            comboPanel.gameObject.SetActive(false);
        }
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

    private string GetWinnerText(int winner)
    {
        if (winner == 1)
            return "Player 1 Wins";

        if (winner == 2)
            return "Player 2 Wins";

        return "Draw";
    }
}