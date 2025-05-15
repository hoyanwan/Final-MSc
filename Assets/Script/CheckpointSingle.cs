using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    // Reference to the TrackCheckpoints script
    private TrackCheckpoints trackCheckpoints;

    // Method to set the reference to the TrackCheckpoints script
    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }

    // Property to get if the checkpoint is the correct one
    public bool IsCorrectCheckpoint { get; private set; }

    // Method to set whether the checkpoint is the correct one
    public void SetCorrectCheckpoint(bool value)
    {
        IsCorrectCheckpoint = value;
    }

}
