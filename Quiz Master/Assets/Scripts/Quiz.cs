using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    private void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    private void Update() {
        timerImage.fillAmount = timer.fillFraction;

        if (timer.loadNextQuestion) {
            if (progressBar.value == progressBar.maxValue) {
                isComplete = true;
                return;
            }

            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        } else if (!hasAnsweredEarly && !timer.isAnsweringQuestion) {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index) {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    private void DisplayAnswer(int index) {
        Image buttonImage;
        correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();

        if (index == correctAnswerIndex)
        {
            questionText.text = "Acertou!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Sinto muito, mas a resposta correta era: \n" + correctAnswer;
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }
    }

    private void GetNextQuestion() {
        if (questions.Count > 0) {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();            
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

    private void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if (questions.Contains(currentQuestion)) {
            questions.Remove(currentQuestion);
        }        
    }

    private void DisplayQuestion() {
        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.GetAnswer(i);
        }
    }

    private void SetButtonState(bool state) {
        for (int i = 0; i < answerButtons.Length; i++) {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    private void SetDefaultButtonSprites() {
        for (int i = 0; i < answerButtons.Length; i++) {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }
}
