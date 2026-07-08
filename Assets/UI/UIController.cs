using UnityEngine;
using Unity.Properties;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] UIDocument m_uiDocument;
    [SerializeField] SO_GameData m_playerDataTemplate;

    VisualElement m_root;
    SO_GameData m_playerData;

    Label m_boostlabel;

    public SO_GameData RuntimePlayerData => m_playerData;

    private void OnEnable()
    {
        if (m_uiDocument == null || m_playerDataTemplate == null)//I'm sure this is good practice and all but really i think it should just crash if this isn't set right
            return;

        m_root = m_uiDocument.rootVisualElement;
        m_boostlabel = m_root.Q<Label>(name: "BoostBarPercentage");//Search for the specific named element

        m_boostlabel?.SetBinding("text", new DataBinding()//set the text field of the element to be the same as the CurrentBoost property
        {
            dataSourcePath = 
                new PropertyPath(nameof(SO_GameData.CurrentBoost)),
            bindingMode = BindingMode.ToTarget
        });

        m_playerData = Instantiate(m_playerDataTemplate); //create a new instance of player data at runtime, NO FUCKING IDEA HOW THIS IS GONNA WORK WITH SAVE/LOAD
        m_root.dataSource = m_playerData;
    }
}
