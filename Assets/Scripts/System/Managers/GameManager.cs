using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _timeToFinish; //variável que indica o tempo para terminar o jogo
    private UI_Manager _uiManager; // referência do gerenciador de interface
    private Player _player; // referência ao player
    public bool isPlaying; // informa se o jogador está jogando para subtração do tempo de jogo para terminar
    [SerializeField] private int _score; // pontuação do jogador
    [SerializeField] private int _enemiesKilled; // números de inimigos mortos pelo jogador
    public bool paused; // indicador de pause
    [SerializeField] private int _countdownToStart; // contagem para recomeçar o jogo
    public InputField countdownToStartInputField, gameDurationInputField; // campos de texto que indicarão o tempo para começar o jogo e para finalizar o jogo
    public string countdownToStartString, gameDurationString; // somente para organização e visualização dos campos digitados acima
    public string sceneName, sceneToLoad; // nome da cena atual, e a cena para carregar (somente em telas de loading), pois as mesmas já iniciarão executando a função de carregamento
    public GameObject menu, optionsMenu; // objeto representando o menu inicial e o menu de opcões // configurações
    public float loadProgress; // progresso do carregamento
    public Slider loadingBar; // barra de carregamento representando visualmente o progresso de carregamento

    private void Awake() // pegando as refências e verificando se já foi definido ou não os tempos que regem o jogo, se sim, pega-se o tempo e aplica-se tanto internamente quanto
                         // externamente deixando visível na interface do jogador. Caso não, seta com o valor padrão 
    {
        _uiManager = FindObjectOfType<UI_Manager>();
        _player = FindObjectOfType<Player>();

        if (PlayerPrefs.HasKey("CooldownToStart"))
        {
            _countdownToStart = PlayerPrefs.GetInt("CooldownToStart");
        }
        else
        {
            _countdownToStart = 3;
            PlayerPrefs.SetInt("CooldownToStart", _countdownToStart);

        }

        if (PlayerPrefs.HasKey("GameDuration"))
        {
            _timeToFinish = PlayerPrefs.GetFloat("GameDuration");
        }
        else
        {
            _timeToFinish = 180;
            PlayerPrefs.SetFloat("GameDuration", _timeToFinish);
        }
    }
    private void Start() // verifica-se no start se a cena atual é uma cena de transição/carregamento e não possui botão ou interação do usuário, logo 
                         //os parâmetros indicando a cena que deve ser carregada é passado diretamente
    {
        Time.timeScale = 1;
        if (this.sceneName == "LoadingScene")
        {
            LoadScene(this.sceneToLoad); // se sim, carrega manualmente a cena pré estabelecida pelo usuário
        }
    }
    public void LoadScene(string sceneToLoad) // chamar o carregamento de cenas
    {
        StartCoroutine(LoadAsync(sceneToLoad)); // iniciando de fato o carregamento
    }
    private IEnumerator LoadAsync(string sceneToLoad) //
    {
        AsyncOperation sceneLoadProgress = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!sceneLoadProgress.isDone)
        {
            loadProgress = Mathf.Clamp01(sceneLoadProgress.progress / .9f); // limita o valor de 0 a 1 e divide por 9, porque a função da unity vai de 0 a 0.9 então desse jeito
                                                                            // vai de 0 a 1 e a barra de progresso carrega de acordo
            if (this.sceneName == "LoadingScene")
            {
                loadingBar.value = loadProgress;
            }
            yield return null;
        }
    }
    public int GetCountdownNumber() // retorna o tempo para o gerenciador de UI do tempo para começar o jogo (contagem incial)
    {
        return _countdownToStart;
    }
    public float GetTimeToFinish() // retorna o tempo total de jogo
    {
        return _timeToFinish;
    }
    private void Update()
    {
        if (isPlaying) // se está jogando, subtrai o tempo de jogo total até chegar em 0, para encerrar o jogo
        {
            _timeToFinish -= Time.deltaTime; // subtração do tempo
            SetTimer(); // informa ao gerenciador de UI o tempo para terminar o jogo

            if (_timeToFinish <= 0) // se já terminou, afirma que o jogo já terminou e que o jogador não pode jogar mais, dá os parabéns e passa os pontos e 
                                    // a quantidade de inimigos derrotados ao jogador
            {
                _timeToFinish = 0;
                SetTimer();
                isPlaying = false;
                _player.canPlay = false;
                _uiManager.OpenGameOverPanel("Parabéns!", this._score, this._enemiesKilled);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) // pausa o jogo, abrindo o menu de pause
            {
                if (!paused)
                {
                    _uiManager.OpenPauseMenu();
                    _player.canPlay = false; // informando que o jogador não pode se mover
                    Cursor.visible = true; // recolocando a visibilidade do cursor do mouse;
                    Time.timeScale = 0; // pausando de fato
                }
            }
        }
    }
    public void StartGame() // começando o jogo, avisando que o jogo está rodando e que o jogador pode jogar
    {
        _player.canPlay = true;
        isPlaying = true;
    }
    public void SetTimer() // informa o timer da contagem regressiva para terminar o jogo
    {
        _uiManager.UpdateFinishCountdown((int)_timeToFinish);
    }
    public void SetScore(int score) // atualiza a pontuação do jogador
    {
        _enemiesKilled++;
        this._score += score;
        _uiManager.UpdateScore(this._score);
    }
    public void QuitApplication() // fecha a aplicação
    {
        Application.Quit();
    }
    public void ResumeGame() // continua o jogo quando está pausado, fecha o menu de pause e reverte as condições de pause
    {
        Time.timeScale = 1;
        _uiManager.ClosePauseMenu();
        _player.canPlay = true;
        Cursor.visible = false;
        paused = false;
    }
    public void SetTimeToFinish() // pega do campo de Input o tempo de jogo total (tempo para terminar o jogo) e salva o mesmo
                                  // verifica-se se o jogador não escreveu algo que não possa ser convertido (como letras ou símbolos)
                                  // caso não tenha erro, converte o tempo para segundos e assim atualiza no sistema, caso contrário
                                  // mantém o tempo antigo
    {
        gameDurationString = gameDurationInputField.text;
        bool tryParse = float.TryParse(gameDurationString, out _timeToFinish);

        if (tryParse)
        {
            PlayerPrefs.SetFloat("GameDuration", _timeToFinish);
        }
        else
        {
            float temporaryValue = PlayerPrefs.GetFloat("GameDuration");
            gameDurationInputField.text = temporaryValue.ToString();
        }
    }
    public void SetTimeToStart() // fazendo a mesma função de cima, porém atualizando o contador inicial ao invés do contador de tempo para o jogo finalizar
    {
        countdownToStartString = countdownToStartInputField.text;
        bool tryParse = int.TryParse(countdownToStartString, out _countdownToStart);

        if (tryParse)
        {
            PlayerPrefs.SetInt("CooldownToStart", _countdownToStart);
        }
        else
        {
            int temporaryValue = PlayerPrefs.GetInt("CooldownToStart");
            countdownToStartInputField.text = temporaryValue.ToString();
        }
    }
    public void OpenOptionsMenu() // abre menu de opções e desativa o menu inicial
    {
        menu.SetActive(false);
        optionsMenu.SetActive(true);
        countdownToStartInputField.text = _countdownToStart.ToString();
        gameDurationInputField.text = _timeToFinish.ToString();
    }
    public void CloseOptionsMenu() // fecha o menu de opções e abre o menu inicial novamente
    {
        menu.SetActive(true);
        optionsMenu.SetActive(false);
    }
    public void FinishGame() // finaliza o jogo (condição quando o player for derrotado)
    {
        _uiManager.OpenGameOverPanel("Você não sobreviveu ao ataque!", this._score, this._enemiesKilled);
    }
}

