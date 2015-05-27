using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S3_LatencyQueue {

	private int capacity = 10;
	private Queue<float> latencyQueue;
	// Use this for initialization
	public S3_LatencyQueue () {
		latencyQueue = new Queue<float> (capacity);
	}


	public void Enqueue(float x)
	{
		if (latencyQueue.Count >= capacity) {
			latencyQueue.Dequeue();
		}

		latencyQueue.Enqueue (x);
	}

	public int GetMeanI()
	{
		int count = 0;
		int sum = 0;
		foreach (int i in latencyQueue) {
			sum += i;
			count++;
		}
		float average = (float)sum / (float)count;
		return Mathf.RoundToInt (average * 1000) ;
	}

	public float GetMeanF()
	{
		int count = 0;
		int sum = 0;
		foreach (int i in latencyQueue) {
			sum += i;
			count++;
		}
		return (float)sum / (float)count;
	}
}
