using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System;

public class HumanPose : MonoBehaviour
{
	int[] IMAGE_SIZE = new int[2] { 256, 448};
	const string INPUT_NAME = "data";
	const string OUTPUT_NAME = "features";

	List<float> result;

	[Header("Model Stuff")]
	public NNModel modelFile;

	[Header("Scene Stuff")]
	public CameraView CameraView;
	public Preprocess preprocess;

	IWorker worker;

	void Start()
	{
		var model = ModelLoader.Load(modelFile);
		worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
	}

	void Update()
	{

		WebCamTexture webCamTexture = CameraView.GetCamImage();

		if (webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
		{
			preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE, RunModel);
		}
	}

	void RunModel(byte[] pixels)
	{
		StartCoroutine(RunModelRoutine(pixels));
	}

	IEnumerator RunModelRoutine(byte[] pixels)
	{

		Tensor tensor = TransformInput(pixels);

		var inputs = new Dictionary<string, Tensor> {
			{ INPUT_NAME, tensor }
		};

		worker.Execute(inputs);
		Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);
		

		//get largest output
		result = outputTensor.ToReadOnlyArray().ToList();

		//dispose tensors
		tensor.Dispose();
		outputTensor.Dispose();
		yield return null;
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
