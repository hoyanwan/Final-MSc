using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class NPCController : Agent
{
    // Reference to the target's transform (goal position)
    [SerializeField] private Transform targetTransform;

    // Reference to the TrackCheckpoints script
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    // Player's ID
    [SerializeField] private int playerID;

    // Reference to the player's transform
    [SerializeField] private Transform PlayerTransform;

    private void Start()
    {

    }

    // Called when the episode begins (resetting the agent's state)
    public override void OnEpisodeBegin()
    {
        // Reset the checkpoint index for the TrackCheckpoints script
        trackCheckpoints.ResetCheckpointIndex();

        // Generate a random offset for the NPC's initial position near the player
        float xOffset = Random.Range(-2.5f, 2.5f); 
        float zOffset = Random.Range(-2.5f, 2.5f);

        // Calculate the new random position for the NPC
        Vector3 randomPosition = new Vector3(PlayerTransform.localPosition.x + xOffset, 0, PlayerTransform.localPosition.z + zOffset);
        
        // Set the NPC's position to the newly calculated random position
        transform.localPosition = randomPosition;
    }

    // Collect observations from the environment for decision making
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add NPC's position and target's position to the observation
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

        // Update NPC's position based on actions and time
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    // This method provides a way to control the agent manually for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Get input from keyboard or other controls for manual testing
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Called when the NPC collides with other objects (triggers)
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(5000.0f);
            EndEpisode();
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
                Debug.Log("Wrong");
            }
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-15.0f);
            EndEpisode();
        }
    }
}
