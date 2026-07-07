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
    }

    private void HandleGameStateChanged(GameState prevState, GameState currentState)
    {

    }
}
