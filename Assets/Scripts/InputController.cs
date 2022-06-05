using UnityEngine;

public class InputController : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hit;
    private Touch _touch;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            _ray = _camera.ScreenPointToRay(_touch.position);
            Physics.Raycast(_ray, out _hit);

            if (_touch.phase == TouchPhase.Began)
            {
                if (_hit.collider != null && _hit.collider.TryGetComponent<Bottle>(out var bottle))
                {
                    bottle.OnClick();
                }
            }            
        }
    }
}
