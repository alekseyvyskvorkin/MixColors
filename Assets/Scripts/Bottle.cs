using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class Bottle : MonoBehaviour
{
    private const float MoveBottleDuration = 0.15f;

    private static Bottle CurrentBottle;

    [SerializeField] private List<BottleSegment> _segments = new List<BottleSegment>();

    private Vector3 _startPosition;
    private Vector3 _onSelectPosition => transform.position + transform.up / 2;

    private void Awake()
    {
        _startPosition = transform.localPosition;
        foreach (var segment in _segments)
        {
            segment.Init();
        }
    }

    public void OnClick()
    {
        if (CurrentBottle == null)
        {
            if (_segments[0].gameObject.activeInHierarchy == false) return;

            CurrentBottle = this;
            transform.DOMove(_onSelectPosition, MoveBottleDuration);
        }
        else
        {
            CurrentBottle.transform.DOMove(CurrentBottle._startPosition, MoveBottleDuration);

            if (CurrentBottle != this)
            {
                MixColors();
            }

            CurrentBottle = null;
        }
    }

    private async void MixColors()
    {
        List<BottleSegment> emptySegments = NearestEmptySegment();
        List<BottleSegment> fullSegments = CurrentBottle.FullSegments();
        List<BottleSegment> nearestEqualSegments = NearestEqualSegments();

        if (CanMixColors(emptySegments, fullSegments))
        {
            Color mixColor = fullSegments[fullSegments.Count - 1].CurrentColor;

            if (nearestEqualSegments.Count > 0)
            {
                mixColor = nearestEqualSegments[0].CurrentColor;
                foreach (var segment in nearestEqualSegments)
                {
                    segment.ChangeColor(segment.CurrentColor, fullSegments[fullSegments.Count - 1].CurrentColor);
                }
            }
            for (int i = 0; i < fullSegments.Count; i++)
            {
                emptySegments[i].ChangeColor(mixColor, fullSegments[i].CurrentColor);
                await SwipeSegmentsSize(emptySegments[i], fullSegments[i]);
            }
        }
    }

    private bool CanMixColors(List<BottleSegment> emptySegments, List<BottleSegment> fullSegments)
    {
        return fullSegments.Count > 0 && emptySegments.Count > 0 && fullSegments.Count <= emptySegments.Count;
    }

    private async Task SwipeSegmentsSize(BottleSegment emptySegment, BottleSegment fullSegment)
    {
        emptySegment.Activate();
        fullSegment.Deactivate();
        await UniTask.WaitWhile(() => fullSegment.gameObject.activeInHierarchy);
    }

    private List<BottleSegment> FullSegments()
    {
        var segments = new List<BottleSegment>();
        for (int i = _segments.Count - 1; i >= 0; i--)
        {
            if (_segments[i].gameObject.activeInHierarchy == true)
            {
                if (segments.Count > 0 && segments[segments.Count - 1].CurrentColor == _segments[i].CurrentColor)
                {
                    segments.Add(_segments[i]);
                }
                else if (segments.Count == 0)
                {
                    segments.Add(_segments[i]);
                }
                else
                {
                    break;
                }
            }
        }

        return segments;
    }

    private List<BottleSegment> NearestEmptySegment()
    {
        var segments = new List<BottleSegment>();
        for (int i = 0; i < _segments.Count; i++)
        {
            if (_segments[i].gameObject.activeInHierarchy == false)
            {
                segments.Add(_segments[i]);
            }
        }

        return segments;
    }

    private List<BottleSegment> NearestEqualSegments()
    {
        List<BottleSegment> segments = new List<BottleSegment>();
        for (int i = 0; i < _segments.Count; i++)
        {
            if (segments.Count > 0 && _segments[i].gameObject.activeInHierarchy == true)
            {
                if (segments[segments.Count - 1].CurrentColor != _segments[i].CurrentColor)
                {
                    segments.Clear();
                    segments.Add(_segments[i]);
                }
                else
                {
                    segments.Add(_segments[i]);
                }    
            }
            else if (segments.Count == 0 && _segments[i].gameObject.activeInHierarchy == true)
            {
                segments.Add(_segments[i]);
            }
        }

        return segments;
    }
}
