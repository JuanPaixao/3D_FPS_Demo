using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Slider HPSlider; // barra de vida do jogador
    public Text arrowNumberUI; // número de flechas exibido na interface
    public Text countdown; // contador inicial para início do jogo
    public Text finishGameCountdown; // contador final para respectivamente, finalizar o jogo
    public Text scoreText; // texto da pontuação
    public GameManager gameManager; // gerenciador de jogo
    public GameObject pausePanel; // menu de pause
    private int _countdownNumber; // número representando o valor atual da contagem regressiva para início do jogo
    public GameObject gameOverPanel; // painel de game over
    public string sceneName; //nome da cena
    public Text endScoreText, endKilledText, endGameMessage; // textos que aparecem na tela de game over 

    public void Start()
    {
        if (this.sceneName == "Game") // se a cena for a de jogo, pego os valores para as contagens 
        {
            _countdownNumber = gameManager.GetCountdownNumber();
            UpdateFinishCountdown((int)gameManager.GetTimeToFinish());
            StartCoroutine(InitialCountdownRoutine());
        }
        if (this.sceneName == "Menu") // se for o menu, asseguro que o cursor estará visível
        {
            Cursor.visible = true;
        }
    }
    public void OpenGameOverPanel(string message, int endScore, int endKilled) // abrir painel de game over, recebendo as pontuações e a mensagem ao jogador 
    {
        gameOverPanel.SetActive(true);
        this.endGameMessage.text = message.ToString();
        this.endScoreText.text = endScore.ToString() + " pontos!";
        this.endKilledText.text = endKilled.ToString() + " inimigos!";
        Cursor.visible = true;
    }
    public void UpdateSlideHP(float hp) // atualizar barra de vida do jogador
    {
        this.HPSlider.value = hp;
    }
    public void UpdateArrowNumber(int arrowNumber) // atualizar número de flechas
    {
        this.arrowNumberUI.text = arrowNumber.ToString();
    }
    public void UpdateCountdown(int countdown) // atualizar contagem regressiva para começar o jogo
    {
        if (countdown >= 1)
        {
            this.countdown.text = countdown.ToString();
        }
        else if (countdown < 1)
        {
            this.countdown.text = "START";
            gameManager.StartGame();
        }
    }
    public void UpdateFinishCountdown(int finishCountdown) // atualizar contagem para terminar o jogo
    {
        this.finishGameCountdown.text = " " + finishCountdown.ToString() + " segundos";
    }
    public void UpdateScore(int score) // atualizar pontuação na interface
    {
        this.scoreText.text = score.ToString();
    }
    public void OpenPauseMenu() // abrar menu de pause
    {
        pausePanel.SetActive(true);
    }
    public void ClosePauseMenu() // fechar menu de pause
    {
        pausePanel.SetActive(false);
    }
    public int GetCountdownNumber() // pegar contagem regressiva do início do jogo
    {
        return _countdownNumber;
    }
    private IEnumerator InitialCountdownRoutine() // faz a contagem regressiva do começo do jogo, e exibe ao jogador os valores da mesma até que se acabe a contagem
    {
        Cursor.visible = false;
        countdown.text = _countdownNumber.ToString();
        UpdateCountdown(_countdownNumber);
        while (_countdownNumber > 0)
        {
            yield return new WaitForSeconds(1f);

            _countdownNumber--;
            countdown.text = _countdownNumber.ToString();
            UpdateCountdown(_countdownNumber);
        }
        if (_countdownNumber <= 0)
        {
            UpdateCountdown(_countdownNumber);
            yield return new WaitForSeconds(0.75f);
            this.countdown.enabled = false;
            gameManager.StartGame();
        }
    }
}
