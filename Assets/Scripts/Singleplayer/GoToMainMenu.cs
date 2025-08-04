using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    public string mainmenu;
    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(mainmenu);
    }
}
