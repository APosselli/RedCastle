using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Character. Dialogue. Trigger
public class MoveScene : MonoBehaviour {

    [SerializeField] private string LoadLevel;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(LoadLevel);
        }
    }
}
