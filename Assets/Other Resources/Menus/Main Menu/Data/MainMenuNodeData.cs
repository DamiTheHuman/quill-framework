using UnityEngine;
/// <summary>
/// Defines the generic data available to a node to be read by the <see cref="MainMenuController"/>
/// </summary>
public class MainMenuNodeData : MonoBehaviour
{
    [SerializeField]
    private MainMenuController mainMenuController;
    [Tooltip("Whether to display the back button when this node is active")]
    public bool backButton = true;
    [Tooltip("Whether to display the confirm button when this node is active")]
    public bool confirmButton = true;
    [Tooltip("Whether to display the delete button when this node is active")]
    public bool deleteButton = false;
    [Tooltip("Whether to display the debug button when this node is active")]
    public bool debugButton = false;
    public GameObject firstSelectedButton;
}
