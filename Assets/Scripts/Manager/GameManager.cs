using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState = GameState.None;

    public GameState CurrentState
    {
        get { return _currentState; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log($"중복된 GameManager가 발견되어 파괴합니다: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        ChangeGameState(GameState.Boot);
        
    }

    private void Start()
    {
        ChangeGameState(GameState.Title);
        ItemDataBase.LoadAllData();
    }

    public void ChangeGameState(GameState nextState)
    {
        if(_currentState == nextState)
        {
            return;
        }

        GameState prevState = _currentState;
        _currentState = nextState;

        Debug.Log($"게임 상태 변경: {prevState} 에서 {CurrentState}로 변경됩니다.");

        HandleGameStateChanged(prevState, _currentState);
    }

    private void HandleGameStateChanged(GameState prevState, GameState currentState)
    {
        switch(currentState)
        {
            case GameState.Boot:
                {
                    OnEnterBootState();
                    break;
                }
            case GameState.Title:
                {
                    OnEnterTitleState();
                    break;
                }
            case GameState.Loading:
                {
                    OnEnterLoadingState();
                    break;
                }
            case GameState.Playing:
                {
                    OnEnterPlayingState();
                    break;
                }
            case GameState.Paused:
                {
                    OnEnterPausedState();
                    break;
                }
            case GameState.GameOver:
                {
                    OnEnterGameOverState();
                    break;
                }
            case GameState.Clear:
                {
                    OnEnterClearState();
                    break;
                }

        }
    }

    private void OnEnterBootState()
    {
        InputManager.Instance?.SetGameplayInputState(false);
        Debug.Log("현재 초기화 상태에 돌입했습니다.");
    }

    private void OnEnterTitleState()
    {
        Time.timeScale = 1f;
        InputManager.Instance?.SetGameplayInputState(false);
        Debug.Log("현재 타이틀 상태에 돌입했습니다.");
    }

    private void OnEnterLoadingState()
    {
        InputManager.Instance?.SetGameplayInputState(false);
        Debug.Log("현재 로딩 상태에 돌입했습니다.");
    }

    private void OnEnterPlayingState()
    {
        Time.timeScale = 1f;
        InputManager.Instance?.SetGameplayInputState(true);

        Debug.Log("현재 진행중인 상태에 돌입했습니다.");
    }
    private void OnEnterPausedState()
    {
        Time.timeScale = 0f;
        InputManager.Instance?.SetGameplayInputState(false);
        Debug.Log("현재 일시 중지 상태에 돌입했습니다.");
    }
    private void OnEnterGameOverState()
    {
        Time.timeScale = 0f;
        Debug.Log("현재 게임 오버 상태에 돌입했습니다.");
    }
    private void OnEnterClearState()
    {
        Time.timeScale = 0f;
        Debug.Log("현재 게임 클리어 상태에 돌입했습니다.");

    }
    public void StartGame()
    {
        if (_currentState != GameState.Title)
        {
            Debug.LogWarning($"Title 상태가 아니므로 게임을 시작할 수 없습니다. 현재 상태: {_currentState}");
            return;
        }

        ChangeGameState(GameState.Loading);

        ChangeGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (_currentState != GameState.Playing)
        {
            Debug.LogWarning($"Playing 상태가 아니므로 일시정지할 수 없습니다. 현재 상태: {_currentState}");
            return;
        }

        ChangeGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if(_currentState != GameState.Paused)
        {
            Debug.LogWarning($"Paused 상태가 아니므로 게임을 재개할 수 없습니다. 현재 상태: {_currentState}");
            return;
        }
        ChangeGameState(GameState.Playing);
    }

    public void GameOver()
    {
        if(_currentState == GameState.GameOver)
        {
            return;
        }
        ChangeGameState(GameState.GameOver);
    }
    public void ClearGame()
    {
        if (_currentState == GameState.Clear)
        {
            return;
        }

        ChangeGameState(GameState.Clear);
    }

    public bool IsPlaying()
    {
        return _currentState == GameState.Playing;
    }

    public bool IsPaused()
    {
        return _currentState == GameState.Paused;
    }

    public bool IsGameEnded()
    {
        return _currentState == GameState.GameOver || _currentState == GameState.Clear;
    }

}
