using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxReveries : MonoBehaviour
{
    public GameObject Box3D;
    public bool IsLean;
    //
    public GameObject BoxShadowAtRest;
    public GameObject BoxShadowHorizontal;
    //
    private float _leanAnimationTime;
    private Vector3 _restRotation;
    private float _degreeDecline;
    //
    private IEnumerator _shakeRoutine;
    public bool ShakeTheBox;
    private float jumpIter = 9.5f;
    private float _shakePeriodTime; // The period of each shake

    // TODO: make this dropOffTime be a bit random.
    private float dropOffTime = 1.0f; // How long it takes the shaking to settle down to nothing

    //
    private Dir _lastDir;

    // compile time constants
    //-----------------------------------------------------------------------------------------------------
   
    public void Init()
    {
        _leanAnimationTime = 0.15f;
        _restRotation = Box3D.transform.eulerAngles;
        _degreeDecline = 20f;
    }

    public IEnumerator Decline(Dir dir, float? animT = null)
    {
        Vector3 vDir;

        switch (dir)
        {
            case Dir.None:
                vDir = new Vector3(_restRotation.x - _degreeDecline, _restRotation.y, _restRotation.z);
                _lastDir = dir;
                break;
            case Dir.Up:
                vDir = new Vector3(_restRotation.x - _degreeDecline, _restRotation.y, _restRotation.z);
                //
                _lastDir = dir;
                break;
            case Dir.Right:
                vDir = new Vector3(_restRotation.x, _restRotation.y, _restRotation.z + _degreeDecline);
                _lastDir = dir;
                break;
            case Dir.Down:
                vDir = new Vector3(_restRotation.x + _degreeDecline, _restRotation.y, _restRotation.z);
                //
                _lastDir = dir;
                break;
            case Dir.Left:
                vDir = new Vector3(_restRotation.x, _restRotation.y, _restRotation.z - _degreeDecline);
                _lastDir = dir;
                break;
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }

        LeanTween.rotateLocal(Box3D, vDir, _leanAnimationTime);
        
        yield return new WaitForSeconds(_leanAnimationTime);
        
        _shakeRoutine = Shake();
        ShakeTheBox = true;

        StartCoroutine(_shakeRoutine);
    }
    
    private IEnumerator Shake()
    {
        float height = Mathf.PerlinNoise(jumpIter, 0f) * 10f;
        height = height * height * 0.3f;
        //Debug.Log("height:" + height + " jumpIter:" + jumpIter);

        /**************
        * Camera Shake
        **************/

        Vector3 vectorToShake;
        //if (_lastDir == Dir.Up || _lastDir == Dir.Down)
        //    vectorToShake = Vector3.up;
        //else
            vectorToShake = Vector3.right;
        
        float shakeAmt = height * 0.3f; // the degrees to shake the camera
        _shakePeriodTime = 0.42f; // The period of each shake
        LTDescr shakeTween = LeanTween.rotateAroundLocal(
            Box3D, 
            vectorToShake, 
            shakeAmt, 
            _shakePeriodTime
            ).setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
            .setLoopClamp()
            .setRepeat(-1);

        // Slow the camera shake down to zero
        LeanTween.value(Box3D, shakeAmt, 0f, dropOffTime).setOnUpdate(
            (float val) => { shakeTween.setTo(vectorToShake * val); }
            ).setEase(LeanTweenType.easeOutQuad);

        yield return new WaitForSeconds(dropOffTime);

        if (ShakeTheBox)
        {
            StopCoroutine(_shakeRoutine);
            _shakeRoutine = Shake();
            LeanTween.cancel(Box3D);
            StartCoroutine(_shakeRoutine);
        }
    }
    
    public void OnRest()
    {
        IsLean = false;

        ShakeTheBox = false;
        StopCoroutine(_shakeRoutine);
        LeanTween.cancel(Box3D);

        LeanTween.rotateLocal(Box3D, _restRotation, _leanAnimationTime);

        // LeanTween.cancel(BoxShadowHorizontal);
        // StartCoroutine(LeanShadow(Dir.None));
    }
}
