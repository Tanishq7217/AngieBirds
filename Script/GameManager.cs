using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    public int MaxNumberOfShots = 3;

    [SerializeField] private float secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject restartScreenObject;
    [SerializeField] private SlingShotHandler SlingShotHandler;
    [SerializeField] private Image nextLevelImage;

    private int usedNumberOfShots;

    private IconHandler iconHandler;

    private List<Baddie> _baddies = new List<Baddie>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindObjectOfType<IconHandler>();

        Baddie[] baddies = FindObjectsOfType<Baddie>();
        for (int i = 0; i < baddies.Length; i++)
        {
            _baddies.Add(baddies[i]);
        }

        nextLevelImage.enabled = false;
    }

    public void usedShot()
    {
        usedNumberOfShots++;
        iconHandler.usedShot(usedNumberOfShots);

        checkForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void checkForLastShot()
    {
        if (usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDeathCheck);

        if (_baddies.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame(); 
        }

       
    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        _chechkForAllDeadBAddies();
    }

    private void _chechkForAllDeadBAddies()
    {
        if (_baddies.Count == 0)
        {
            WinGame();

        }
    }

    #region win/lose

    private void WinGame()
    {
        restartScreenObject.SetActive(true);
        SlingShotHandler.enabled = false;

        //do we any more levels to load

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels =SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex + 1 < maxLevels)
        {
            nextLevelImage.enabled = true;
        }

    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
       
    }
    #endregion winlose

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
