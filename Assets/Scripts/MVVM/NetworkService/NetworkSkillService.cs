using UnityEngine;

public class NetworkSkillService
{
    private PlayerModel _playerModel;


    public void Init(PlayerModel playerModel)
    {
        _playerModel = playerModel;
    }


    public PlayerModel GetLocalPlayerModel()
    {
        if (_playerModel == null)
        {
            return new PlayerModel();
        }
        return _playerModel;
    }


}


