using UnityEngine;

public class TurretRarity : MonoBehaviour
{
    [SerializeField] float _rotateSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime); 
    }

    public void SetRarityVisual(TowerRarity rarity)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color rarityColor = Color.white;
        switch (rarity)
        {
            case TowerRarity.Normal:
                rarityColor = Color.white;
                break;
            case TowerRarity.Rare:
                rarityColor = Color.green;
                break;
            case TowerRarity.Epic:
                rarityColor = Color.cyan;
                break;
            case TowerRarity.Legendary:
                rarityColor = Color.red;
                break;
            default:
                Debug.LogWarning("Unknown rarity: " + rarity);
                break;
        }
        spriteRenderer.color = rarityColor;
    }

    public Color GetRarityColor(TowerRarity rarity)
    {
        Color rarityColor = Color.white;
        switch (rarity)
        {
            case TowerRarity.Normal:
                rarityColor = Color.white;
                break;
            case TowerRarity.Rare:
                rarityColor = Color.green;
                break;
            case TowerRarity.Epic:
                rarityColor = Color.cyan;
                break;
            case TowerRarity.Legendary:
                rarityColor = Color.red;
                break;
            default:
                Debug.LogWarning("Unknown rarity: " + rarity);
                break;
        }
        return rarityColor;
    }
}
