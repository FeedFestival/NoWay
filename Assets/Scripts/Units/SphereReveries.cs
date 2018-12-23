using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereReveries : MonoBehaviour
{
    //public GameObject SphereOutline;
    private Vector3 _origSPos;
    private Vector3 _origOutlinePos;
    private Vector3 _sideSPos;
    private Vector3 _sideOutlinePos;

    private float _leanAnimationTime = 0.2f;
    
    public Dir LeanDir;

    public bool IsLeaned;
    public bool IsAtRest;

    private GameObject _sphere;

    public void Init(GameObject sphere)
    {
        _sphere = sphere;
        _origSPos = sphere.transform.localPosition;
        //_origOutlinePos = SphereOutline.transform.localPosition;

        IsAtRest = true;
    }

    public IEnumerator Ignition(Dir dir)
    {
        IsLeaned = true;
        IsAtRest = false;

        LeanDir = dir;
        
        SetLeanPositions();
        LeanTween.moveLocal(_sphere, _sideSPos, _leanAnimationTime).setEase(LeanTweenType.linear);
        //LeanTween.moveLocal(SphereOutline, _sideOutlinePos, _leanAnimationTime).setEase(LeanTweenType.linear);

        Game.Instance.Sphere.RollArround(LeanDir, percent: 8f, animT: _leanAnimationTime);

        yield return new WaitForSeconds(_leanAnimationTime);


    }

    // TODO move this in the sphere.
    public void AtRest()
    {
        IsLeaned = false;
        IsAtRest = true;

        Game.Instance.Sphere.RollArround(LeanDir, percent: 8f, animT: _leanAnimationTime, reverse: true);

        LeanDir = Dir.None;

        LeanTween.moveLocal(_sphere, _origSPos, _leanAnimationTime).setEase(LeanTweenType.linear);
        //LeanTween.moveLocal(SphereOutline, _origOutlinePos, _leanAnimationTime).setEase(LeanTweenType.linear);

        //if (GameController.Instance.Sphere.IsPushingSomething && GameController.Instance.Sphere.PushedObject != null)
        //{
        //    var pushedObject = GameController.Instance.Sphere.PushedObject;

        //    if (pushedObject.GetType() == typeof(Box))
        //    {
        //        (pushedObject as Box).OnRest();
        //    }
        //}
    }
    
    private void SetLeanPositions()
    {
        switch (LeanDir)
        {
            case Dir.Up:
                _sideSPos = new Vector3(_origSPos.x, _origSPos.y + 0.1f, _origSPos.z);
                _sideOutlinePos = new Vector3(_origOutlinePos.x, _origOutlinePos.y + 0.1f, _origOutlinePos.z);

                break;
            case Dir.Right:
                _sideSPos = new Vector3(_origSPos.x + 0.1f, _origSPos.y, _origSPos.z);
                _sideOutlinePos = new Vector3(_origOutlinePos.x + 0.1f, _origOutlinePos.y, _origOutlinePos.z);
                break;
            case Dir.Down:
                _sideSPos = new Vector3(_origSPos.x, _origSPos.y - 0.1f, _origSPos.z);
                _sideOutlinePos = new Vector3(_origOutlinePos.x, _origOutlinePos.y - 0.1f, _origOutlinePos.z);
                break;
            case Dir.Left:
                _sideSPos = new Vector3(_origSPos.x - 0.1f, _origSPos.y, _origSPos.z);
                _sideOutlinePos = new Vector3(_origOutlinePos.x - 0.1f, _origOutlinePos.y, _origOutlinePos.z);
                break;
            default:
                _sideSPos = _origSPos;
                _sideOutlinePos = _origOutlinePos;
                break;
        }
    }
}