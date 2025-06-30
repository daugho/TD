using UnityEngine;
using UnityEngine.UI;

public class StageStar : MonoBehaviour
{
    [SerializeField]
    private Image[] _stars;
    [SerializeField]
    private Sprite _yellowStar;
    [SerializeField]
    private Sprite _grayStar;

    private int _curStarCount = 0;

    private void Start()
    {
        ResetStars();
    }

    private void ResetStars()
    {
        foreach (Image star in _stars)
        {
            star.sprite = _grayStar;
        }

        _curStarCount = 0;
    }

    private void AddStar()
    {
        if (_curStarCount < _stars.Length)
        {
            _stars[_curStarCount].sprite = _yellowStar;
            _curStarCount++;
        }
    }
}
