using UnityEngine;
using Random = UnityEngine.Random;

public class StoneTable : MonoBehaviour
{
    [SerializeField] private GameObject[] stoneChairs;

    private void Awake()
    {
        EnableRandomNumberOfChairs();
    }

    private void EnableRandomNumberOfChairs()
    {
        int randomCount = Random.Range(2, 4);
        
        for (int i = 0; i < randomCount; i++)
        {
            int randomValue = Random.Range(0, stoneChairs.Length);

            GameObject randomChair = stoneChairs[randomValue];
            RotateRandomlyChair(randomChair);
            
            randomChair.SetActive(true);
        }
    }

    private void RotateRandomlyChair(GameObject chair)
    {
        chair.transform.localRotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
    }
}