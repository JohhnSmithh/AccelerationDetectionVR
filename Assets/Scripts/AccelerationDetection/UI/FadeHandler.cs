using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Triggers fade animations and causes delayed scene transition to wait for fade to finish.
/// </summary>
public class FadeHandler : MonoBehaviour
{
    public Animator animator;

    private string levelToLoad;

    private void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    public void FadeToLevel(string newLevel)
    {
        levelToLoad = newLevel;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
