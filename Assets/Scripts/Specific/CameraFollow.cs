using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool DebugThis;

    public Sphere Target;
    private Collider2D _targetCollider;

    public Vector2 FocusAreaSize;

    private FocusArea _focusArea;

    private float _sphereScaleX;

    public float VerticalOffset;

    private float _lookAheadDistanceX;
    private float _lookAheadDistanceY;

    public float LookSmoothTimeX;
    public float LookSmoothTimeY;

    private float _currentLookAheadX;
    private float _currentLookAheadY;

    private float _targetLookAheadX;
    private float _targetLookAheadY;

    private float _lookAheadDirectionX;
    private float _lookAheadDirectionY;

    private float _smoothLookVelocityX;
    private float _smoothLookVelocityY;

    private bool _lookAheadStoppedX;
    private bool _lookAheadStoppedY;

    private static CameraFollow _instance;
    public static CameraFollow Instance
    {
        get { return _instance; }
    }
    void Awake()
    {
        _instance = this;
    }

    public void Init(Vector2 sphereScale)
    {
        _sphereScaleX = sphereScale.x;
        FocusAreaSize = new Vector2(_sphereScaleX * 2, _sphereScaleX * 2);

        _lookAheadDistanceX = _sphereScaleX * 1.5f;
        _lookAheadDistanceY = _sphereScaleX * 1.5f;

        _targetCollider = Target.GetComponent<Collider2D>();
        _focusArea = new FocusArea(_targetCollider.bounds, FocusAreaSize);
    }

    void LateUpdate()
    {
        if (_targetCollider != null)
        {
            _focusArea.Update(_targetCollider.bounds);

            Vector2 focusPosition = _focusArea.Center + Vector2.up * VerticalOffset;

            if (_focusArea.Velocity.x != 0f)
            {
                _lookAheadDirectionX = Mathf.Sign(_focusArea.Velocity.x);

                if (GameController.Instance.Directions != null && GameController.Instance.Directions.Count > 0 &&
                    ((Mathf.Sign(_focusArea.Velocity.x) > 0 && GameController.Instance.Directions[0] == Dir.Right)
                    || (Mathf.Sign(_focusArea.Velocity.x) < 0 && GameController.Instance.Directions[0] == Dir.Left)))
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

            if (_focusArea.Velocity.y != 0f)
            {
                _lookAheadDirectionY = Mathf.Sign(_focusArea.Velocity.y);

                if (GameController.Instance.Directions != null && GameController.Instance.Directions.Count > 0 &&
                    ((Mathf.Sign(_focusArea.Velocity.y) > 0 && GameController.Instance.Directions[0] == Dir.Up)
                    || (Mathf.Sign(_focusArea.Velocity.y) < 0 && GameController.Instance.Directions[0] == Dir.Down)))
                {
                    _lookAheadStoppedY = false;

                    _targetLookAheadY = _lookAheadDirectionY * _lookAheadDistanceY;
                }
                else
                {
                    if (_lookAheadStoppedY == false)
                    {
                        _targetLookAheadY = _currentLookAheadY +
                                            (_lookAheadDirectionY * _lookAheadDistanceY - _currentLookAheadY) / 4f;

                        _lookAheadStoppedY = true;
                    }
                }
            }

            _currentLookAheadX = Mathf.SmoothDamp(
                _currentLookAheadX,
                _targetLookAheadX,
                ref _smoothLookVelocityX,
                LookSmoothTimeX);

            //  - vertical smoothing

            _currentLookAheadY = Mathf.SmoothDamp(
                _currentLookAheadY,
                _targetLookAheadY,
                ref _smoothLookVelocityY,
                LookSmoothTimeY);

            //focusPosition.y = Mathf.SmoothDamp(
            //    transform.position.y,
            //    focusPosition.y,
            //    ref _smoothLookVelocityY,
            //    LookSmoothTimeY);

            focusPosition += Vector2.up * _currentLookAheadY;

            //----

            focusPosition += Vector2.right * _currentLookAheadX;

            transform.position = (Vector3)focusPosition + Vector3.forward * -10;
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
        public Vector2 Center;
        public Vector2 Velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            Velocity = Vector2.zero;
            Center = new Vector2((left + right) / 2, (top + bottom) / 2);
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

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            Center = new Vector2((left + right) / 2, (top + bottom) / 2);
            Velocity = new Vector2(shiftX, shiftY);
        }
    }
}
