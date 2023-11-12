using UnityEngine;
using TMPro;


public class TextMeshProUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProComponent;
    //private int number;
    
    // private TextMeshProUGUI textComponent;

    void Start()
    {
        textMeshProComponent = GetComponent<TextMeshProUGUI>();
        //number = 0;
        //UpdateText();
    }

    void Update()
    {
        textMeshProComponent.text = "Damage: " + PlayerStats.Instance.damagePoints;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("false"))
        {
            // number--;
            // UpdateText();
        }
    }
}
