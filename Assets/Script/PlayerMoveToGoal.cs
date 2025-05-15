using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class PlayerMoveToGoal : Agent
{
    // Reference to the target's transform (goal position)
    [SerializeField] private Transform targetTransform;

    // Reference to the TrackCheckpoints script
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    // Player's ID
    [SerializeField] private int playerID;

    // References to NPC transforms
    [SerializeField] private Transform NpcTransform1;
    [SerializeField] private Transform NpcTransform2;
    [SerializeField] private Transform NpcTransform3;
    [SerializeField] private Transform NpcTransform4;
    [SerializeField] private Transform NpcTransform5;
    [SerializeField] private Transform NpcTransform6;
    [SerializeField] private Transform NpcTransform7;
    [SerializeField] private Transform NpcTransform8;
    [SerializeField] private Transform NpcTransform9;
    [SerializeField] private Transform NpcTransform10;

    // Audio related variables
    public AudioSource audioSource;
    [SerializeField] private AudioClip Waiting_For_Path_Finding;
    [SerializeField] private AudioClip Move_Down;
    [SerializeField] private AudioClip Move_Up;
    [SerializeField] private AudioClip Move_Left;
    [SerializeField] private AudioClip Move_Right;
    [SerializeField] private AudioClip Wall_Collide;
    [SerializeField] private AudioClip To_Goal;

    private void Start()
    {

    }

    // Called when the episode begins (resetting the agent's state)
    public override void OnEpisodeBegin()
    {
        // Reset the player's position to a random starting position
        transform.localPosition = new Vector3(Random.Range(-120f, -111f), 0, Random.Range(-120f, -104f));
    }

    // Collect observations from the environment for decision making
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add player's position and target's position to the observation
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    // Called when actions are received from the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract continuous actions for movement
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Define movement speed
        float moveSpeed = 45.0f;

        // Update player's position based on actions and time
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        // Calculate distances between NPCs and player
        float PlayerX = transform.localPosition.x;
        float PlayerZ = transform.localPosition.z;

        float Npc1x = NpcTransform1.localPosition.x;
        float Npc1Z = NpcTransform1.localPosition.z;
        float Npc2x = NpcTransform2.localPosition.x;
        float Npc2Z = NpcTransform2.localPosition.z;
        float Npc3x = NpcTransform3.localPosition.x;
        float Npc3Z = NpcTransform3.localPosition.z;
        float Npc4x = NpcTransform4.localPosition.x;
        float Npc4Z = NpcTransform4.localPosition.z;
        float Npc5x = NpcTransform5.localPosition.x;
        float Npc5Z = NpcTransform5.localPosition.z;
        float Npc6x = NpcTransform6.localPosition.x;
        float Npc6Z = NpcTransform6.localPosition.z;
        float Npc7x = NpcTransform7.localPosition.x;
        float Npc7Z = NpcTransform7.localPosition.z;
        float Npc8x = NpcTransform8.localPosition.x;
        float Npc8Z = NpcTransform8.localPosition.z;
        float Npc9x = NpcTransform9.localPosition.x;
        float Npc9Z = NpcTransform9.localPosition.z;
        float Npc10x = NpcTransform10.localPosition.x;
        float Npc10Z = NpcTransform10.localPosition.z;

        float meanNpcx = (Npc1x + Npc2x + Npc3x + Npc4x + Npc5x + Npc6x + Npc7x + Npc8x + Npc9x + Npc10x) / 10;
        float meanNpcz = (Npc1Z + Npc2Z + Npc3Z + Npc4Z + Npc5Z + Npc6Z + Npc7Z + Npc8Z + Npc9Z + Npc10Z) / 10;

        float DistanceX = meanNpcx - PlayerX;
        float DistanceZ = meanNpcz - PlayerZ;

        // Check if the player is within a certain distance range of NPCs
        if (Mathf.Abs(DistanceX) <= 5 && Mathf.Abs(DistanceZ) <= 5)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(Waiting_For_Path_Finding);
            }
            Debug.Log("Within distance range, ignoring behavior.");
        }
        else
        {
            if (!audioSource.isPlaying) 
            {
                if (DistanceX > 5)
                {
                    audioSource.PlayOneShot(Move_Right);
                    Debug.Log("Mean NPCs are to the Right");
                }
                else if (DistanceX < -5)
                {
                    audioSource.PlayOneShot(Move_Left);
                    Debug.Log("Mean NPCs are to the Left");
                }

                if (DistanceZ > 5)
                {
                    audioSource.PlayOneShot(Move_Up);
                    Debug.Log("Mean NPCs are Upwards");
                }
                else if (DistanceZ < -5)
                { 
                    audioSource.PlayOneShot(Move_Down);
                    Debug.Log("Mean NPCs are Downwards");
                }
            }
           
        }


    }

    // This method provides a way to control the agent manually for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Get input from keyboard or other controls for manual testing
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Called when the player collides with other objects (triggers)
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(5000.0f);
            EndEpisode();
            audioSource.PlayOneShot(To_Goal);
        }
        if (other.TryGetComponent<CheckpointSingle>(out CheckpointSingle checkpointSingle))
        {
            trackCheckpoints.PlayerThroughCheckpoint(checkpointSingle, transform);

            if (checkpointSingle.IsCorrectCheckpoint)
            {
                AddReward(5.0f);
            }
            else
            {
                AddReward(-2.0f);
            }
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-15.0f);
            EndEpisode();
            audioSource.PlayOneShot(Wall_Collide);
        }
    }
}
