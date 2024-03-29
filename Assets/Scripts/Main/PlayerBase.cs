using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    protected Level _level;
    protected PlayerController _playerController;

    protected virtual void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public virtual void OnLevelInit(Level level)
    {
        _level = level;
    }

    public virtual void OnLevelStart()
    {
        _playerController.SetActive(true);
    }

    public virtual void OnLevelEnd()
    {
        _playerController.SetActive(false);
    }
}
