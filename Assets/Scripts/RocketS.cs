using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketS : MonoBehaviour
{
    Rigidbody rocketRigidBody;
    AudioSource rocketAudioSource;

    //Serialized fields to adjust their values dynamic and/or access them from the inspector
    [SerializeField] float rotationThrustMagnitude = 200f;
    [SerializeField] float updwardThrustMagnitude = 25f;
    [SerializeField] float levelLoadDelay = 2f;

        //audio
    [SerializeField] AudioClip thrusterSound;
    [SerializeField] AudioClip progressedSound;
    [SerializeField] AudioClip crashSound;

    //particle effects
    [SerializeField] ParticleSystem thrusterEffect;
    [SerializeField] ParticleSystem progressedEffect;
    [SerializeField] ParticleSystem crashEffect;

    //enum GameState {Active, Crashed, Progressing}
    //GameState gameState = GameState.Active;
    bool completedLevel = false;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rocketRigidBody = GetComponent<Rigidbody>(); //access into the rocket's rigid body component
        rocketAudioSource = GetComponent<AudioSource>(); //access into the rocket's audio source component

    }

    // Update is called once per frame
    void Update()
    {
        if (!completedLevel) //(gameState == GameState.Active)
        {
            processThrustInput();
            processRotateInput();
        }

        if (Debug.isDebugBuild) //make sure this is not allowed in final product (only respond to debug keyes in debug build)
        {
            respondToDegubKeys();
        }
        
    }

    private void respondToDegubKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; 
        }
    }

    void OnCollisionEnter(Collision collision) //detect collision between rigid body of ship and other colliders
    {

        if(completedLevel /*gameState != GameState.Active*/ || collisionsDisabled) //if player is not active in level (not crashed)
        {
            return; //exit method
        }

        switch (collision.gameObject.tag)
        {
            case "Safe":
                {
                    //do nothing if rocket collides with a safe object
                    print("SAFE");
                    break;
                }
            case "Goal":
                {
                    //progress when in contact with a goal object
                    print("GOAL");
                    CompleteLevel();
                    break;
                }
            default:
                {
                    //lose when collide with a non safe object or a non fuel object 
                    print("CRASH");
                    LoseLevel();
                    break;
                }


        }
    }

    private void CompleteLevel()
    {
        //gameState = GameState.Progressing;
        completedLevel = true;
        rocketAudioSource.Stop();
        rocketAudioSource.PlayOneShot(progressedSound);
        progressedEffect.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //Load next scene and wait three seconds before doing so
    }

    private void LoseLevel()
    {
        //gameState = GameState.Crashed;
        completedLevel = true;
        rocketAudioSource.Stop();
        rocketAudioSource.PlayOneShot(crashSound);
        crashEffect.Play();
        Invoke("LoadFirstLevel", levelLoadDelay); //Load first scene wait three seconds before doing so
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //go back to first level after last one
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void processThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopThrust();
        }
    }

    private void StopThrust() //stop the audio and effect
    {
        rocketAudioSource.Stop();
        thrusterEffect.Stop();
    }

    private void ApplyThrust()
    {
        //vector3.up is a vector of one unit, multiplying it by a float will make another different vector in the same direction and location
        rocketRigidBody.AddRelativeForce(Vector3.up * updwardThrustMagnitude * Time.deltaTime); //thrust up in the local direction //* Time.deltaTime to make it frame independent
        if (!rocketAudioSource.isPlaying) //if the audioSource is not playing (avoid stacking)
        {
            rocketAudioSource.PlayOneShot(thrusterSound);
        }
        thrusterEffect.Play();
    }

    private void processRotateInput()
    {

        //freezing and unfreezing should be limited
        //rocketRigidBody.freezeRotation = true; //manually control of rotation 

        float rotationMagnitude = rotationThrustMagnitude * Time.deltaTime; //calculate rotation in this frame
        if (Input.GetKey(KeyCode.A)) //left
        {
            rocketRigidBody.freezeRotation = true; //manually control of rotation 
            transform.Rotate(Vector3.forward * rotationMagnitude); //rotate anticlockwise in z axis
            rocketRigidBody.freezeRotation = false; //resume physics control of rotation
        }
        else if (Input.GetKey(KeyCode.D)) //right
        {
            rocketRigidBody.freezeRotation = true; //manually control of rotation 
            transform.Rotate(-Vector3.forward * rotationMagnitude); //rotate clockwise in z axis
            rocketRigidBody.freezeRotation = false; //resume physics control of rotation
        }

        //rocketRigidBody.freezeRotation = false; //resume physics control of rotation 
    }

}
