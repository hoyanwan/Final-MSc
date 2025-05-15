using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class CheckpointMoveToGoal : Agent
{
    // Reference to the goal's transform
    [SerializeField] private Transform targetTransform;

    // Materials for changing the floor appearance
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    // MeshRenderer for the floor
    [SerializeField] private MeshRenderer floorMeshRender;

    // Reference to the TrackCheckpoints script
    [SerializeField] private TrackCheckpoints trackCheckpoints; 

    // Player ID
    [SerializeField] private int playerID;


    private void Start()
    {

    }

    // Called when the episode begins (resetting the agent's state)
    public override void OnEpisodeBegin()
    {
        // Reset the checkpoints for the agent
        trackCheckpoints.ResetCheckpointIndex();

        // Reset the agent's position to a random starting position
        transform.localPosition = new Vector3(Random.Range(-120f, -111f), 0, Random.Range(-120f, -104f));
    }

    // Collect observations from the environment for decision making
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add agent's position and goal's position to the observation
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    // Called when actions are received from the agent
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 45.0f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    // This method provides a way to control the agent manually for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Get the index of the current correct checkpoint for the agent
        int currentCheckpointIndex = trackCheckpoints.GetCurrentCheckpointIndex(transform);

        // If a correct checkpoint is found, move directly towards it
        if (currentCheckpointIndex != -1)
        {
            Vector3 checkpointPosition = trackCheckpoints.GetCheckpointPosition(currentCheckpointIndex);
            Vector3 moveDirection = (checkpointPosition - transform.position).normalized;

            continuousActions[0] = moveDirection.x;
            continuousActions[1] = moveDirection.z;
        }
        else
        {
            // No correct checkpoint found, continue with default heuristic
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");
        }
    }

    // Called when the agent collides with other objects (triggers)
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(5000.0f);
            floorMeshRender.material = winMaterial;
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
            floorMeshRender.material = loseMaterial;
            EndEpisode();
        }
    }
}
