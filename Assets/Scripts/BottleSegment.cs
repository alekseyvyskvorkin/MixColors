using UnityEngine;
using DG.Tweening;

public class BottleSegment : MonoBehaviour
{
    private const float TransfusionTime = 0.1f;

    public Color CurrentColor => _meshRenderer.material.color;

    private MeshRenderer _meshRenderer;

    private Vector3 _fullSize = new Vector3(100, 100, 100);
    private Vector3 _noSize = new Vector3(100, 100, 1);

    public void Init()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColor(Color current, Color target)
    {
        Color result = (current + target) / 2;

        _meshRenderer.material.color = result;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        transform.localScale = _noSize;
        transform.DOScale(_fullSize, TransfusionTime);
    }

    public void Deactivate()
    {
        transform.DOScale(_noSize, TransfusionTime).OnComplete(() => gameObject.SetActive(false));
    }
}