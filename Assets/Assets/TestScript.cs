using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private string testMessage = "Hello from MCP Unity!";
    
    void Start()
    {
        Debug.Log($"TestScript started: {testMessage}");
    }
    
    void Update()
    {
        // Test script functionality
    }
}