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

    [Header("Player UI Shufflers (Asignar en Inspector)")]
    public ShuffleBotones player1UIButtons;
    public ShuffleBotones player2UIButtons;

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

    [Header("Round Timer Modifier")]
    public AttackTimeModifier attackTimeModifier;

    private BattlePhase currentPhase;
    private bool roundActive;
    private bool roundFinished;
    private int currentTime;

    private ConversationType[] player1Mapping = new ConversationType[3];
    private ConversationType[] player2Mapping = new ConversationType[3];

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

    // ---------------- INPUT ----------------

    private void ReadPlayer1Input()
    {
        if (player1Choice != ConversationType.None)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit1Key.wasPressedThisFrame)
            LockPlayer1Choice(player1Mapping[0]);
        else if (kb.digit2Key.wasPressedThisFrame)
            LockPlayer1Choice(player1Mapping[1]);
        else if (kb.digit3Key.wasPressedThisFrame)
            LockPlayer1Choice(player1Mapping[2]);
    }

    private void ReadPlayer2Input()
    {
        if (player2Choice != ConversationType.None)
            return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit7Key.wasPressedThisFrame || kb.numpad7Key.wasPressedThisFrame)
            LockPlayer2Choice(player2Mapping[0]);
        else if (kb.digit8Key.wasPressedThisFrame || kb.numpad8Key.wasPressedThisFrame)
            LockPlayer2Choice(player2Mapping[1]);
        else if (kb.digit9Key.wasPressedThisFrame || kb.numpad9Key.wasPressedThisFrame)
            LockPlayer2Choice(player2Mapping[2]);
    }

    // ---------------- LOCK CHOICE ----------------

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

    // ---------------- ROUND ----------------

    private void StartNewRound()
    {
        StopAllCoroutines();

        currentPhase = BattlePhase.Conversation;

        player1Choice = ConversationType.None;
        player2Choice = ConversationType.None;

        roundActive = true;
        roundFinished = false;

        currentTime = roundTime;

        // 1. Forzamos un barajado de la lógica completamente fresco
        RandomizeMappings();

        // 2. Le mandamos el array caótico e independiente a los botones
        if (player1UIButtons != null) player1UIButtons.ActualizarTextosBotones(player1Mapping);
        if (player2UIButtons != null) player2UIButtons.ActualizarTextosBotones(player2Mapping);

        if (attackSystem != null)
            attackSystem.DisableAttacks();

        HideComboPanelInstant();

        if (player1StatusText != null) player1StatusText.text = "";
        if (player2StatusText != null) player2StatusText.text = "";
        if (resultText != null) resultText.text = "";
        if (countdownText != null) countdownText.text = "";

        StartCoroutine(CountdownRoutine());
    }

    private void RandomizeMappings()
    {
        // Forzamos la creación de instancias totalmente nuevas en cada llamada
        ConversationType[] baseP1 = { ConversationType.Meh, ConversationType.Bueno, ConversationType.MuyBueno };
        Shuffle(baseP1);
        player1Mapping = baseP1;

        ConversationType[] baseP2 = { ConversationType.Meh, ConversationType.Bueno, ConversationType.MuyBueno };
        Shuffle(baseP2);
        player2Mapping = baseP2;
    }

    private void Shuffle(ConversationType[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (array[i], array[rand]) = (array[rand], array[i]);
        }
    }

    // ---------------- TIMER ----------------

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
            FinishRound();
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

    // ---------------- FINISH ROUND ----------------

    private void FinishRound()
    {
        if (roundFinished) return;

        roundFinished = true;
        roundActive = false;

        StopAllCoroutines();

        int winner = GetWinner();

        if (winner == 1) player1Score++;
        else if (winner == 2) player2Score++;

        if (resultText != null)
        {
            resultText.text =
                "P1: " + GetChoiceText(player1Choice) + "\n" +
                "P2: " + GetChoiceText(player2Choice) + "\n\n" +
                GetWinnerText(winner);
        }

        StartCoroutine(AttackPhaseRoutine(winner));
    }

    // ---------------- ATTACK PHASE ----------------

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
            yield return new WaitForSeconds(2f);
            StartNewRound();
            yield break;
        }

        yield return StartCoroutine(OpenComboPanel());

        if (winner == 1)
            attackSystem.EnablePlayer1Attack();
        else
            attackSystem.EnablePlayer2Attack();

        float timer = attackTime;

        if (attackTimeModifier != null)
        {
            yield return StartCoroutine(
                attackTimeModifier.PrepareAttack(t =>
                {
                    timer = t;
                })
            );
        }

        while (timer > 0f && !attackSystem.attackPerformed)
        {
            if (countdownText != null)
                countdownText.text = "ATTACK " + Mathf.CeilToInt(timer);

            timer -= Time.deltaTime;
            yield return null;
        }

        attackSystem.DisableAttacks();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(CloseComboPanel());

        // Al terminar el ataque se reinicia el flujo completo desordenando todo otra vez
        StartNewRound();
    }

    // ---------------- UI PANEL ----------------

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

        comboPanel.anchoredPosition = comboPanelExitLeftPosition;
        comboPanel.gameObject.SetActive(false);

        yield return null;
    }

    private void HideComboPanelInstant()
    {
        if (comboPanel != null)
        {
            comboPanel.anchoredPosition = comboPanelExitLeftPosition;
            comboPanel.gameObject.SetActive(false);
        }
    }

    // ---------------- WIN LOGIC ----------------

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
        return choice switch
        {
            ConversationType.Meh => "Meh",
            ConversationType.Bueno => "Bueno",
            ConversationType.MuyBueno => "Muy Bueno",
            _ => "No Answer"
        };
    }

    private string GetWinnerText(int winner)
    {
        if (winner == 1) return "Player 1 Wins";
        if (winner == 2) return "Player 2 Wins";
        return "Draw";
    }
}