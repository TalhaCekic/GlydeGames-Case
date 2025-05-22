using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    private Animator _animatorComponent;
    public static LoadingScreenManager instance;

    private void Start()
    {
        instance = this;
        _animatorComponent = transform.GetComponent<Animator>();  
    }

    public void RevealLoadingScreen()
    {
       _animatorComponent.SetTrigger("Reveal");
    }

    public void HideLoadingScreen()
    {
       _animatorComponent.SetTrigger("Hide");
    }

    public void OnFinishedReveal()
    {
        // TODO: remove it and load your own scene !!
       // transform.parent.GetComponent<DemoSceneManager>().OnLoadingScreenRevealed();
    }

    public void OnFinishedHide()
    {
        // TODO: remove it and call your functions 
       // transform.parent.GetComponent<DemoSceneManager>().OnLoadingScreenHided();
    }

}
