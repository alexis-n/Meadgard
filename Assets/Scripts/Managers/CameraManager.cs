using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public float panSpeed = 30f, lowestDragPositionSpeed = 30f, highestDragPositionSpeed = 90f;
    public float dragRotationSpeed;
    public float DragPositionSpeed
    {
        get
        {
            var percentage = (Camera.main.transform.position.y - freeLookCamera.m_Orbits[2].m_Height) / (freeLookCamera.m_Orbits[0].m_Height - freeLookCamera.m_Orbits[2].m_Height);
            return (highestDragPositionSpeed * percentage) + lowestDragPositionSpeed;
        }
        set{}
    }
    public float panBorderThickness = 5f;
    public Vector2 panLimit;
    private Vector2 dragOrigin;

    public Vector3 minPosition, maxPostion;

    public float scrollSpeed = 3f;
    public Vector2 scrollLimit;

    public static CameraManager instance;

    public GameObject target, followed;
    public bool followingTarget = false, inTransition = false, controlsEnabled = false;
    public CinemachineFreeLook freeLookCamera;

    public float movementSpeed = 50f, rotationSpeed = 100f;

    public bool mouseCameraInput = false;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        if (!PauseMenu.instance.paused)
        {
            Vector3 position = transform.position;
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() ||
                 Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject()) mouseCameraInput = true;
            else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2)) mouseCameraInput = false;


            //ici, on gère la rotation de la caméra en fonction des touches du clavier
            if (controlsEnabled)
            {
                float y = 0;
                //si on essaie de faire glisser la caméra en appuyant sur le bouton de la souris
                if (Input.GetMouseButton(2) && mouseCameraInput)
                {
                    y = -Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * dragRotationSpeed;
                }
                //ou si on essaie de faire une rotation grâce aux touches du clavier...
                else if (Input.GetAxis("Rotation") != 0)
                {
                    y = Input.GetAxis("Rotation") * Time.unscaledDeltaTime * rotationSpeed;
                }
                target.transform.Rotate(0, y, 0);
            }

            //ici, on gère le mouvement de la caméra en fonction des touches du clavier, de la souris au bord de l'écran, et du mouvement de la souris quand on enfonce le clique gauche
            if (!followingTarget && !inTransition && controlsEnabled)
            {
                float x = 0f;
                float z = 0f;

                //si on essaie de se déplacer grâce aux touches du clavier...
                if (Input.GetAxis("Horizontal") != 0)
                {
                    x = Input.GetAxis("Horizontal") * Time.unscaledDeltaTime * movementSpeed;
                }
                //ou si on essaie de se déplacer avec la souris au bord de l'écran...
                else if (Input.mousePosition.x >= (Screen.width - panBorderThickness))
                {
                    x = 1 * Time.unscaledDeltaTime * movementSpeed;
                }
                else if (Input.mousePosition.x <= panBorderThickness)
                {
                    x = -1 * Time.unscaledDeltaTime * movementSpeed;
                }

                //si on essaie de se déplacer grâce aux touches du clavier...
                if (Input.GetAxis("Vertical") != 0)
                {
                    z = Input.GetAxis("Vertical") * Time.unscaledDeltaTime * movementSpeed;
                }
                //ou si on essaie de se déplacer avec la souris au bord de l'écran...
                else if (Input.mousePosition.y >= (Screen.height - panBorderThickness))
                {
                    z = 1 * Time.unscaledDeltaTime * movementSpeed;
                }
                else if (Input.mousePosition.y <= panBorderThickness)
                {
                    z = -1 * Time.unscaledDeltaTime * movementSpeed;
                }

                target.transform.Translate(x, 0, z);
                target.transform.position = new Vector3(Mathf.Clamp(target.transform.position.x, minPosition.x, maxPostion.x), 0, Mathf.Clamp(target.transform.position.z, minPosition.z, maxPostion.z));
            }
            else if (controlsEnabled && followingTarget)
            {
                //si la caméra suit une cible, le moindre input pour déplacer la caméra annule le suivi
                target.transform.position = followed.transform.position;
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 ||
                    Input.mousePosition.y >= (Screen.height - panBorderThickness) ||
                    Input.mousePosition.y <= panBorderThickness ||
                    Input.mousePosition.x >= (Screen.width - panBorderThickness) ||
                    Input.mousePosition.x <= panBorderThickness)
                {
                    followingTarget = false;
                    followed = null;
                }
            }

            if (EventSystem.current.IsPointerOverGameObject()) freeLookCamera.m_YAxis.m_InputAxisName = null;
            else freeLookCamera.m_YAxis.m_InputAxisName = "Mouse ScrollWheel";

            position.x = Mathf.Clamp(position.x, -panLimit.x, panLimit.x);
            position.y = Mathf.Clamp(position.y, scrollLimit.x, scrollLimit.y);
            position.z = Mathf.Clamp(position.z, -panLimit.y, panLimit.y);
        }
    }

    public void InitCameraPosition(Transform position)
    {
        target.transform.position = new Vector3(
            position.transform.position.x,
            instance.target.transform.position.y,
            position.transform.position.z
        );
    }

    public void ViewTarget(GameObject newTarget)
    {
        inTransition = true;
        target.transform.DOMove(newTarget.transform.position, 1f).SetEase(Ease.InQuint)
            .OnComplete(() => inTransition = false);
        followed = newTarget;
        followingTarget = true;
    }
}
