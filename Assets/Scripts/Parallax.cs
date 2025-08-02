using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	private float length, startpos;
	public GameObject cam;
	public float parallaxEffect;

	[SerializeField] private bool background;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		length = GetComponent<SpriteRenderer>().bounds.size.x;
		startpos = transform.position.x;
	}

	// Update is called once per frame
	void Update()
	{
		float dist = (cam.transform.position.x * parallaxEffect);
		if (background)
		{
			transform.position = new Vector3(startpos + dist, (float)(cam.transform.position.y-0.77), transform.position.z);
		}
		else
		{
			transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
		}
	}

    
}