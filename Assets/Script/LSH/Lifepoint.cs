using UnityEngine;
using UnityEngine.UI;

public class Lifepoint : MonoBehaviour
{
    public GameObject heartPrefab; 
    public Sprite filledHeart; 
    public Sprite emptyHeart; 
    public int maxLife = 10; 

    private Image[] hearts; 
    private int currentLife;

    void Start()
    {
        currentLife = maxLife;
        hearts = new Image[maxLife];

        // 하트 생성
        for (int i = 0; i < maxLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform); 
            hearts[i] = heart.GetComponent<Image>(); 
            hearts[i].sprite = filledHeart; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DecreaseLife();
        }
    }

    public void DecreaseLife()
    {
        //if (currentLife > 0)
        //{
        //    currentLife--; 
        //    hearts[currentLife].sprite = emptyHeart; 
        //}
        if (currentLife > 0)
        {
            int deletedir = maxLife - currentLife; // 왼쪽부터 빈 하트를 설정
            hearts[deletedir].sprite = emptyHeart;
            currentLife--;
        }
    }

    public void ResetLife()
    {
        currentLife = maxLife; 
        for (int i = 0; i < maxLife; i++)
        {
            hearts[i].sprite = filledHeart; 
        }
    }
}
