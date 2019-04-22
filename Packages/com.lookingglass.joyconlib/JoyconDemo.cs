﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDemo : MonoBehaviour {

	private List<Joycon> joycons;

  public float[] stick;
  public Vector3 gyro1;
	public Vector3 gyro2;
  public Vector3 accel1;
	public Vector3 accel2;
  public Quaternion orientation1;
	public Quaternion orientation2;

  void Start () {
    // get the public Joycon array attached to the JoyconManager in scene
    joycons = JoyconManager.Instance.j;
	}

  // Update is called once per frame
  void Update () {
		if (joycons.Count == 2) {
			Joycon j1 = joycons [0];
			Joycon j2 = joycons [1];
			// GetButtonDown checks if a button has been pressed (not held)
	    if (j1.GetButtonDown(Joycon.Button.SHOULDER_2)) {
				// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
				j1.Recenter();
			}

			if (j2.GetButtonDown(Joycon.Button.SHOULDER_2)) {
				// Joycon has no magnetometer, so it cannot accurately determine its yaw value. Joycon.Recenter allows the user to reset the yaw value.
				j2.Recenter();
			}

			if (j1.GetButtonDown(Joycon.Button.DPAD_DOWN)) {
				StartCoroutine(performRumble(j1, j2));
			}

			if (j2.GetButtonDown(Joycon.Button.DPAD_DOWN)) {
				StartCoroutine(performRumble(j2, j1));
			}

	    // Gyro values: x, y, z axis values (in radians per second)
	    gyro1 = j1.GetGyro();
			gyro2 = j2.GetGyro();

	    // Accel values:  x, y, z axis values (in Gs)
	    accel1 = j1.GetAccel();
			accel2 = j2.GetAccel();

			orientation1 = j1.GetVector();
	    orientation2 = j2.GetVector();
	  }
	}

	// Simulate a throw of a ball by decreasing rumble intensity, then increasing it again
	IEnumerator performRumble(Joycon throw_j, Joycon catch_j) {
		for (float val = 1f; val >= 0f; val -= 0.1f) {
			throw_j.SetRumble(160, 320, val, 100);
			yield return new WaitForSeconds(0.1f);
		}
		for (float val = 0f; val <= 1f; val += 0.1f) {
			catch_j.SetRumble(160, 320, val, 100);
			yield return new WaitForSeconds(0.1f);
		}
	}
}
