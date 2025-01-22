using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField]private LineRenderer leftlineRenderer;
    [SerializeField]private LineRenderer rightlineRenderer;

    [Header("Transform References")]
    [SerializeField]private Transform leftstartposition;
    [SerializeField] private Transform rightstartposition;
    [SerializeField] private Transform centerposition;
    [SerializeField] private Transform idleposition;
    [SerializeField] private Transform elasticTransform;

    [Header("SlingShotStats")]
    [SerializeField] private float maxdistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenBirdRespawns = 2f;
    [SerializeField] private float elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve elasticCurve;
    [SerializeField] private float maxAnimationTime = 1f;



    [Header("Scripts")]
    [SerializeField] private SlingShotArea _slingShotArea;
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    [SerializeField] private AngieBird angieBirdPrefab;
    [SerializeField] private float angieBirdPositionOffset = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip elasticPullClip;
    [SerializeField] private AudioClip[] elasticReleasedClips;


    private Vector2 slingshotlinesposition;
    private Vector2 direction;
    private Vector2 directionNormalized;

    private bool clickedWithinArea;
    private bool birdOnSlingShot;

    private AngieBird spawnedAngieBird;
    private object centerPosition;

    private AudioSource audiosource;

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();

        leftlineRenderer.enabled = false;
        rightlineRenderer.enabled = false;

        SpawnAngieBird();
    }



    private void Update()
    {
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingShotArea())
        {
            clickedWithinArea = true;

            if(birdOnSlingShot)
            {
                SoundManager.instance.Playclip(elasticPullClip, audiosource);
                _cameraManager.SwitchToFollowCam(spawnedAngieBird.transform);
            }
        }
        if(InputManager.IsLeftMousePressed && clickedWithinArea && birdOnSlingShot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();

        }
        if (InputManager.WasLeftMouseButtonReleased && birdOnSlingShot && clickedWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {

                clickedWithinArea = false;
                birdOnSlingShot = false;

                spawnedAngieBird.LaunchBird(direction, shotForce);

                SoundManager.instance.PlayRandomClip(elasticReleasedClips, audiosource);

                GameManager.instance.usedShot();
                AnimateSlingShot();
                

                if (GameManager.instance.HasEnoughShots())
                {
                     StartCoroutine(SpawnAngieBirdAfterTime());
                }
            }

        }
    }
    #region SlingShotMethod
    private void DrawSlingShot()
    {

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        slingshotlinesposition = centerposition.position + Vector3.ClampMagnitude(touchPosition - centerposition.position, maxdistance);
        
        SetLines(slingshotlinesposition);

        direction  = (Vector2)centerposition.position - slingshotlinesposition;
        directionNormalized = direction.normalized;

    }

    private void SetLines(Vector2 position)
    {
        if (!leftlineRenderer.enabled && !rightlineRenderer.enabled)
        {
            leftlineRenderer.enabled = true;
            rightlineRenderer.enabled = true;
        }

        leftlineRenderer.SetPosition(0, position);
        leftlineRenderer.SetPosition(1, leftstartposition.position);

        rightlineRenderer.SetPosition(0, position);
        rightlineRenderer.SetPosition(1, rightstartposition.position);

    }
    #endregion

    #region Angie Birds Method

    private void SpawnAngieBird()
    {
        elasticTransform.DOComplete();
        SetLines(idleposition.position);

        Vector2 dir = (centerposition.position - idleposition.position).normalized; 
        Vector2 spawnPosition = (Vector2) idleposition.position + dir * angieBirdPositionOffset;

        spawnedAngieBird =  Instantiate(angieBirdPrefab, idleposition.position, Quaternion.identity);
        spawnedAngieBird.transform.right = dir;

        birdOnSlingShot = true;

    }

    private void PositionAndRotateAngieBird()
    {
        spawnedAngieBird.transform.position = slingshotlinesposition + directionNormalized * angieBirdPositionOffset;
        spawnedAngieBird.transform.right = directionNormalized;
    }

    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenBirdRespawns);

        SpawnAngieBird();

       _cameraManager.SwitchToIdleCam();
    }

    #endregion

    #region AnimateSlingshot
    private void AnimateSlingShot()
    {
        elasticTransform.position = leftlineRenderer.GetPosition(0) ;

        float dist = Vector2.Distance(elasticTransform.position, centerposition.position);

        float time = dist / elasticDivider;

        elasticTransform.DOMove(centerposition.position , time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingShotLines(elasticTransform, time));

    }

    private IEnumerator AnimateSlingShotLines(Transform trans , float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime < time && elapsedTime < maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            
            SetLines(trans.position);

            yield return null;
        }

    }

    #endregion

}
