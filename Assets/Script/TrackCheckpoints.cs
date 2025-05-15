using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    // List to store individual checkpoint objects
    private List<CheckpointSingle> checkpointSingleList;

    // List to keep track of the index of the next checkpoint for each player
    private List<int> nextCheckpointSingleIndexList;

    // List of player transforms
    [SerializeField] private List<Transform> playerTransformList;

    private void Awake()
    {
        // Find the Transform that holds all the checkpoint objects
        Transform checkpointsTransform = transform.Find("Checkpoints");

        // Initialize the checkpoint list
        checkpointSingleList = new List<CheckpointSingle>();

        // Find all player GameObjects with the "Player" tag
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        // Initialize the player transform list
        playerTransformList = new List<Transform>();

        // Loop through each checkpoint in the checkpointsTransform
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            // Get the CheckpointSingle component from the current checkpoint Transform
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            // Set a reference to this TrackCheckpoints script in the CheckpointSingle
            checkpointSingle.SetTrackCheckpoints(this);

            // Add the checkpoint to the checkpoint list
            checkpointSingleList.Add(checkpointSingle);
        }

        // Loop through each player GameObject
        foreach (GameObject playerObject in playerObjects)
        {
            // Get the transform of the player GameObject and add it to the player transform list
            Transform playerTransform = playerObject.transform;
            playerTransformList.Add(playerTransform);
        }

        // Initialize the list that keeps track of the next checkpoint index for each player
        nextCheckpointSingleIndexList = new List<int>();

        // Loop through each player transform and set their initial next checkpoint index to 0
        foreach (Transform playerTransform in playerTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    // Called when a player passes through a checkpoint
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle, Transform playerTransform)
    {
        // Get the index of the player in the player transform list
        int playerIndex = playerTransformList.IndexOf(playerTransform);

        // Get the current next checkpoint index for this player
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[playerTransformList.IndexOf(playerTransform)];

        // Get the correct checkpoint object for the player's current next checkpoint index
        CheckpointSingle correctCheckpointSingle = checkpointSingleList[nextCheckpointSingleIndex];

        // Check if the passed checkpoint matches the expected checkpoint for the player
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            // Move the player's next checkpoint index forward and set the checkpoint as correct
            nextCheckpointSingleIndexList[playerIndex]++;
            checkpointSingle.SetCorrectCheckpoint(true);
        }
        else
        {
            //Debug.Log("Wrong");
        }
    }

    // Reset the checkpoint indices for all players
    public void ResetCheckpointIndex()
    {
        // Clear the next checkpoint index list and reset each player's index to 0
        nextCheckpointSingleIndexList.Clear();
        foreach (Transform playerTransform in playerTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    // Get the current checkpoint index for a specific player
    public int GetCurrentCheckpointIndex(Transform playerTransform)
    {
        // Get the index of the player in the player transform list
        int playerIndex = playerTransformList.IndexOf(playerTransform);

        // Return the player's current next checkpoint index
        return nextCheckpointSingleIndexList[playerIndex];
    }

    // Get the position of a specific checkpoint
    public Vector3 GetCheckpointPosition(int checkpointIndex)
    {
        // Return the position of the checkpoint at the specified index
        return checkpointSingleList[checkpointIndex].transform.position;
    }
}
