using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool DebugThis;

    public Collider2D Target;
    private Collider2D _targetCollider;

    public Vector3 FocusAreaSize;

    private FocusArea _focusArea;

    private float _sphereScaleX;

    public float VerticalOffset;
    public float HorizontalOffset;

    private float _lookAheadDistanceX;
    private float _lookAheadDistanceZ;

    public float LookSmoothTimeX;
    public float LookSmoothTimeZ;

    private float _currentLookAheadX;
    private float _currentLookAheadZ;

    private float _targetLookAheadX;
    private float _targetLookAheadZ;

    private float _lookAheadDirectionX;
    private float _lookAheadDirectionZ;

    private float _smoothLookVelocityX;
    private float _smoothLookVelocityZ;

    private bool _lookAheadStoppedX;
    private bool _lookAheadStoppedZ;

    private static CameraFollow _instance;
    public static CameraFollow Instance
    {
        get { return _instance; }
    }
    void Awake()
    {
        _instance = this;
    }

    public void Init(Vector3 sphereScale)
    {
        _sphereScaleX = sphereScale.x;
        FocusAreaSize = new Vector3(_sphereScaleX * 4, 1, _sphereScaleX * 2);

        _lookAheadDistanceX = _sphereScaleX * 1.5f;
        _lookAheadDistanceZ = _sphereScaleX * 1.5f;

        _targetCollider = Target;
        _focusArea = new FocusArea(_targetCollider.bounds, FocusAreaSize);
    }

    void LateUpdate()
    {
        if (_targetCollider != null)
        {
            _focusArea.Update(_targetCollider.bounds);

            Vector3 focusPosition = _focusArea.Center + (Vector3.up * VerticalOffset) + (Vector3.right * HorizontalOffset);

            if (_focusArea.Velocity.x != 0f)
            {
                _lookAheadDirectionX = Mathf.Sign(_focusArea.Velocity.x);

                if (Game.Instance.Directions != null && Game.Instance.Directions.Count > 0 &&
                    ((Mathf.Sign(_focusArea.Velocity.x) > 0 && Game.Instance.Directions[0] == Dir.Right)
                    || (Mathf.Sign(_focusArea.Velocity.x) < 0 && Game.Instance.Directions[0] == Dir.Left)))
                {
                    _lookAheadStoppedX = false;

                    _targetLookAheadX = _lookAheadDirectionX * _lookAheadDistanceX;
                }
                else
                {
                    if (_lookAheadStoppedX == false)
                    {
                        _targetLookAheadX = _currentLookAheadX +
                                            (_lookAheadDirectionX * _lookAheadDistanceX - _currentLookAheadX) / 4f;

                        _lookAheadStoppedX = true;
                    }
                }
            }

            if (_focusArea.Velocity.z != 0f)
            {
                _lookAheadDirectionZ = Mathf.Sign(_focusArea.Velocity.z);

                if (Game.Instance.Directions != null && Game.Instance.Directions.Count > 0 &&
                    ((Mathf.Sign(_focusArea.Velocity.z) > 0 && Game.Instance.Directions[0] == Dir.Up)
                    || (Mathf.Sign(_focusArea.Velocity.z) < 0 && Game.Instance.Directions[0] == Dir.Down)))
                {
                    _lookAheadStoppedZ = false;

                    _targetLookAheadZ = _lookAheadDirectionZ * _lookAheadDistanceZ;
                }
                else
                {
                    if (_lookAheadStoppedZ == false)
                    {
                        _targetLookAheadZ = _currentLookAheadZ +
                                            (_lookAheadDirectionZ * _lookAheadDistanceZ - _currentLookAheadZ) / 4f;

                        _lookAheadStoppedZ = true;
                    }
                }
            }

            _currentLookAheadX = Mathf.SmoothDamp(
                _currentLookAheadX,
                _targetLookAheadX,
                ref _smoothLookVelocityX,
                LookSmoothTimeX);

            //  - vertical smoothing

            _currentLookAheadZ = Mathf.SmoothDamp(
                _currentLookAheadZ,
                _targetLookAheadZ,
                ref _smoothLookVelocityZ,
                LookSmoothTimeZ);

            //focusPosition.y = Mathf.SmoothDamp(
            //    transform.position.y,
            //    focusPosition.y,
            //    ref _smoothLookVelocityZ,
            //    LookSmoothTimeZ);

            focusPosition += Vector3.up * _currentLookAheadZ;

            //----

            focusPosition += Vector3.right * _currentLookAheadX;
            
            transform.position = new Vector3(focusPosition.x, 12, focusPosition.y)
                + Vector3.forward * -10;
        }
    }

    void OnDrawGizmos()
    {
        if (DebugThis)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(_focusArea.Center, FocusAreaSize);
        }
    }

    struct FocusArea
    {
        public Vector3 Center;
        public Vector3 Velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector3 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.z;
            top = targetBounds.min.z + size.z;

            Velocity = Vector3.zero;
            Center = new Vector3((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftZ = 0;
            if (targetBounds.min.z < bottom)
            {
                shiftZ = targetBounds.min.z - bottom;
            }
            else if (targetBounds.max.z > top)
            {
                shiftZ = targetBounds.max.z - top;
            }
            top += shiftZ;
            bottom += shiftZ;

            Center = new Vector3((left + right) / 2, (top + bottom) / 2);
            Velocity = new Vector3(shiftX, shiftZ);
        }
    }
}
