using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MoveToGoal : Agent
{
    // Reference to the goal's transform
    [SerializeField] private Transform targetTransform;

    // Materials for changing the floor appearance
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    // MeshRenderer for the floor
    [SerializeField] private MeshRenderer floorMeshRender;

    // Called when the episode begins (resetting the agent's state)
    public override void OnEpisodeBegin()
    {
        // Reset the agent's position to the center
        transform.localPosition = Vector3.zero;
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
        // Extract continuous actions for movement
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Define movement speed
        float moveSpeed = 50f;

        // Update agent's position based on actions and time
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

    // Called when the agent collides with other objects (triggers)
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            floorMeshRender.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRender.material = loseMaterial;
            EndEpisode();
        }

    }

}
