using UnityEngine;

public class BlockController : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find(IdToBlockName(0)).SetActive(false);
        GameObject.Find(IdToBlockName(1)).SetActive(false);
        GameObject.Find(IdToBlockName(2)).SetActive(false);
        GameObject.Find(IdToBlockName(3)).SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private string IdToBlockName(int id) => id == 0 ? "BLock" : $"BLock_{id}";
}
