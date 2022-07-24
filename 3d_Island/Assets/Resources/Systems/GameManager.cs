using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _lockFrameRate = 60;

    [Header("Game Design")]
    [SerializeField] bool _showFrameRate;
    [SerializeField] PlayerSystem _myPlayer;
    [SerializeField] GameObject _eggAsset;
    [SerializeField] GameObject _ballAsset;

    void Start()
    {
        Application.targetFrameRate = _lockFrameRate;

        if (_myPlayer == null)
            FindObjectOfType<PlayerSystem>();

        if (!_showFrameRate)
            UIController.uIController.ShowFrameRate("");

    }
    private void Update()
    {
        if (_showFrameRate)
            UIController.uIController.ShowFrameRate((1f / Time.deltaTime).ToString());
    }

    
    //for design Buttons
    public void SpawnBall()
    {
        Instantiate(_ballAsset.gameObject, _myPlayer.transform.position + _myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }
    public void SpawnEgg()
    {
        Instantiate(_eggAsset.gameObject, _myPlayer.transform.position + _myPlayer.transform.forward * 2f + Vector3.up * 5, Quaternion.identity);
    }


}
