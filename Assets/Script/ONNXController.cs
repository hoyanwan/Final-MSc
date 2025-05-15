using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class ONNXController : MonoBehaviour
{
    // Path to your Barracuda model asset
    public string modelPath;

    // Reference to the loaded Barracuda model
    private Model m_Model;

    // Reference to the Barracuda worker for inference
    private IWorker m_Worker;

    private void Start()
    {
        // Load the Barracuda model asset
        NNModel modelAsset = Resources.Load<NNModel>(modelPath);
        m_Model = ModelLoader.Load(modelAsset);

        // Create a Barracuda worker for inference
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, m_Model);
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Dispose of the Barracuda worker
        m_Worker.Dispose();
    }
}
