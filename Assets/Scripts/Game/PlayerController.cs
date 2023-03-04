using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    LeanFinger _finger;
    Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
            LeanTouch.OnFingerUpdate += LeanTouch_OnFingerUpdate;
            LeanTouch.OnFingerUp += LeanTouch_OnFingerUp;
        }
        else
        {
            LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
            LeanTouch.OnFingerUpdate -= LeanTouch_OnFingerUpdate;
            LeanTouch.OnFingerUp -= LeanTouch_OnFingerUp;
        }
    }

    private void LeanTouch_OnFingerUp(LeanFinger finger)
    {
        if (finger != _finger) return;
        _finger = null;

        _player.Shoot();
    }

    private void LeanTouch_OnFingerDown(LeanFinger finger)
    {
        if (_finger != null) return;
        _finger = finger;
    }

    private void LeanTouch_OnFingerUpdate(LeanFinger finger)
    {
        if (finger != _finger) return;
        Vector3 fingerPos = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
        fingerPos.z = 0;

        _player.SetShootAim(fingerPos);
    }
}
