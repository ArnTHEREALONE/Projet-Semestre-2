using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Transform contentParent;
    public GameObject optionPrefab;

    public int OptionsNbr = 0;

    void Start()
    {
        GenerateOptions(OptionsNbr);
    }

    public void GenerateOptions(int amount)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < amount; i++)
        {
            Instantiate(optionPrefab, contentParent);
        }
    }
}