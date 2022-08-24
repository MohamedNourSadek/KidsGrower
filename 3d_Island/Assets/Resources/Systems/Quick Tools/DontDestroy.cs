using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        transform.parent = null;
        
        DontDestroyOnLoad(gameObject);
    }
}
