using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] PlayerData playerData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement uiWaterLevel = uiDocument.rootVisualElement.Q<VisualElement>("WaterValue");
        uiWaterLevel.dataSource = playerData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
