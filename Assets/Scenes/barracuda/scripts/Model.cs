using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System;


public class Model : MonoBehaviour
{
    // Model Info
    public int[] IMAGE_SIZE;
    string INPUT_NAME = "input";
    string OUTPUT_NAME = "output";
    public NNModel modelFile;
    List<float> output;

    // other components
    public CameraView CameraView;
    public Preprocess preprocess;

    // Inferarence engine
    IWorker worker;

    public void Setup(NNModel modelfile, int[] image_size, string input_name, string output_name)
    {
        this.modelFile = modelfile;
        this.INPUT_NAME = input_name;
        this.OUTPUT_NAME = output_name;
        this.IMAGE_SIZE = image_size;

        // Loading model
        var model = ModelLoader.Load(modelFile);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    public void RunModel(byte[] pixels)
    {
        StartCoroutine(RunModelRoutine(pixels));
    }

    public IEnumerator RunModelRoutine(byte[] pixels)
    {

        Tensor tensor = TransformInput(pixels);
        var inputs = new Dictionary<string, Tensor> {
            { INPUT_NAME, tensor }
        };

        worker.Execute(inputs);
        Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);
        

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }

    public List<float> GetResult()
    {
        return output;
    } 

    //transform from 0-255 to -1 to 1
    Tensor TransformInput(byte[] pixels)
    {
        float[] transformedPixels = new float[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            transformedPixels[i] = (pixels[i] - 127f) / 128f;
        }
        return new Tensor(1, IMAGE_SIZE[0], IMAGE_SIZE[1], 3, transformedPixels);
    }



}
