using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    public static GameResult Instance { get; private set; }

    [SerializeField] private RectTransform[] _stars;
    [SerializeField] private Image[] _starImages;
    [SerializeField] private Sprite _filledStarSprite;
    [SerializeField] private Sprite _emptyStarSprite;

    [SerializeField] private Vector3[] _targetPositions;

    [SerializeField] private float _moveDuration = 1.0f;
    [SerializeField] private float _delayBetweenStars = 0.3f;

    [SerializeField] private float _textDropDuration = 0.5f;
    [SerializeField] private Vector2 _textOffset = new Vector2(0, 100);

    private Transform _gameResultPanel;

    private Vector2 _lifeTextStartPos;
    private Vector2 _goldTextStartPos;
    private Vector2 _huntTextStartPos;
    private Vector2 _buildTextStartPos;

    [SerializeField] private TextMeshProUGUI _remainLifeText;
    [SerializeField] private TextMeshProUGUI _consumeGoldText;
    [SerializeField] private TextMeshProUGUI _huntCountText;
    [SerializeField] private TextMeshProUGUI _builtCountText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _remainLife = 0;
    private int _consumeGold = 0;
    private int _huntCount = 0;
    private int _buildCount = 0;

    private const int _scorePerStar = 5000;

    private void Awake()
    {
        _lifeTextStartPos = _remainLifeText.rectTransform.anchoredPosition;
        _goldTextStartPos = _consumeGoldText.rectTransform.anchoredPosition;
        _huntTextStartPos = _huntCountText.rectTransform.anchoredPosition;
        _buildTextStartPos = _builtCountText.rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        CalculateScoreAndShowResult();
    }
    public void CalculateScoreAndShowResult()
    {
        _remainLife = GameResultData.Instance.GetLife();
        _consumeGold = GameResultData.Instance.GetUsedGold();
        _huntCount = GameResultData.Instance.GetMonsterKill();
        _buildCount = GameResultData.Instance.GetTowerBuilt();

        _remainLifeText.text = $"남은 목숨: {_remainLife}";
        _consumeGoldText.text = $"소모 골드: {_consumeGold}";
        _huntCountText.text = $"처치 수: {_huntCount}";
        _builtCountText.text = $"설치 수: {_buildCount}";

        StartCoroutine(PlayTextDropAnimation(_remainLifeText.rectTransform, _lifeTextStartPos));
        StartCoroutine(PlayTextDropAnimation(_consumeGoldText.rectTransform, _goldTextStartPos));
        StartCoroutine(PlayTextDropAnimation(_huntCountText.rectTransform, _huntTextStartPos));
        StartCoroutine(PlayTextDropAnimation(_builtCountText.rectTransform, _buildTextStartPos));

        int finalScore =
            (_remainLife * 1000) +
            (_huntCount * 300) +
            (_buildCount * 200) -
            _consumeGold;

        StartCoroutine(PlayScoreCountUp(finalScore));
    }

    private IEnumerator PlayTextDropAnimation(RectTransform textTransform, Vector2 targetPos)
    {
        Vector2 startPos = targetPos + _textOffset;
        textTransform.anchoredPosition = startPos;

        float time = 0f;
        while (time < _textDropDuration)
        {
            float t = time / _textDropDuration;
            textTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        textTransform.anchoredPosition = targetPos;
    }

    private IEnumerator PlayScoreCountUp(int finalScore)
    {
        float duration = 1.5f;
        float elapsed = 0f;
        float currentScore = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.Pow(t, 0.7f); 
            currentScore = Mathf.Lerp(0, finalScore, t);
            _scoreText.text = $"점수: {Mathf.RoundToInt(currentScore)}";

            elapsed += Time.deltaTime;
            yield return null;
        }

        _scoreText.text = $"총 점수: {finalScore}";

        int starCount = 1;
        if (finalScore >= 15000) starCount = 3;
        else if (finalScore >= 10000) starCount = 2;

        PlayStarAnimation(starCount);
    }
    public void PlayStarAnimation(int starCount)
    {
        starCount = Mathf.Clamp(starCount, 0, _stars.Length);

        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].gameObject.SetActive(true);
            _stars[i].anchoredPosition = Vector3.zero;
            _stars[i].localScale = Vector3.zero;

            _starImages[i].sprite = (i < starCount) ? _filledStarSprite : _emptyStarSprite;

            StartCoroutine(AnimateStar(_stars[i], _targetPositions[i], _delayBetweenStars * i));
        }
    }
    private IEnumerator AnimateStar(RectTransform star, Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startPos = star.anchoredPosition;
        Vector3 startScale = Vector3.zero;
        Vector3 middleScale = Vector3.one * 1.5f;
        Vector3 endScale = Vector3.one;

        float rotateSpeed = 360f / _moveDuration;
        float time = 0f;

        while (time < _moveDuration)
        {
            float t = time / _moveDuration;

            star.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);

            if (t < 0.5f)
            {
                star.localScale = Vector3.Lerp(startScale, middleScale, t * 2); // 0 ~ 1
            }
            else
            {
                star.localScale = Vector3.Lerp(middleScale, endScale, (t - 0.5f) * 2); // 0 ~ 1
            }

            star.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        star.anchoredPosition = targetPos;
        star.localScale = endScale;
        star.localRotation = Quaternion.identity;

        StartCoroutine(EndScene(5.0f));
    }

    private IEnumerator EndScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        PhotonNetwork.LoadLevel("StageScene");
    }
}
