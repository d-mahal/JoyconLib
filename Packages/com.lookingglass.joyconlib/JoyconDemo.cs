using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconDemo : MonoBehaviour
{

  private List<Joycon> joycons;

  public float[] stick;
  public Vector3 gyro1;
  public Vector3 gyro2;
  public Vector3 accel1;
  public Vector3 accel2;

  private float lastAZ1 = 0;
  private float lastAZ2 = 0;

  private float lastAY1 = 0;
  private float lastAY2 = 0;

  public Quaternion orientation1;
  public Quaternion orientation2;

  public float threshold = 0.5f;

  private bool isCon1 = true;
  private bool inAir = false;

  private int leftIndex = -1;
  private int rightIndex = -1;


  void Start()
  {
    // get the public Joycon array attached to the JoyconManager in scene
    joycons = JoyconManager.Instance.j;
  }

  // Update is called once per frame
  void Update()
  {
    if (joycons.Count == 2)
    {
      Joycon j1;
      Joycon j2;
      if (leftIndex == -1 || rightIndex == -1)
      {
        j1 = joycons[0];
        j2 = joycons[1];

        if (j1.GetButtonDown(Joycon.Button.DPAD_LEFT) && rightIndex != 0)
        {
          isCon1 = true;
          leftIndex = 0;
          rightIndex = 1;
        }
        if (j1.GetButtonDown(Joycon.Button.DPAD_RIGHT) && leftIndex != 0)
        {
          isCon1 = false;
          leftIndex = 1;
          rightIndex = 0;
        }
        if (j2.GetButtonDown(Joycon.Button.DPAD_LEFT) && rightIndex != 1)
        {
          isCon1 = true;
          leftIndex = 1;
          rightIndex = 0;
        }
        if (j2.GetButtonDown(Joycon.Button.DPAD_RIGHT) && leftIndex != 1)
        {
          isCon1 = false;
          leftIndex = 0;
          rightIndex = 1;
        }
      }
      else
      {
        j1 = joycons[leftIndex];
        j2 = joycons[rightIndex];

        if (!inAir)
        {
          if (isCon1)
          {
            j1.SetRumble(160, 320, .1f, 100);
          }
          else
          {
            j2.SetRumble(160, 320, .1f, 100);
          }
        }

        // Accel values:  x, y, z axis values (in Gs)
        accel1 = j1.GetAccel();
        accel2 = j2.GetAccel();

        float A1DeltaZ = Mathf.Abs(accel1.z - lastAZ1);
        float A2DeltaZ = Mathf.Abs(accel2.z - lastAZ2);

        lastAZ1 = accel1.z;
        lastAZ2 = accel2.z;

        float A1DeltaY = accel1.y - lastAY1;
        float A2DeltaY = -1 * (accel2.y - lastAY2);

        lastAY1 = accel1.y;
        lastAY2 = accel2.y;

        if (isCon1)
        {
          if (A1DeltaZ > A1DeltaY)
          {
            if (A1DeltaZ > threshold && !inAir)
            {
              inAir = true;
              StartCoroutine(performRumble(j1, j2, A1DeltaZ));
            }
          }
          else
          {
            if (A1DeltaY > threshold && !inAir)
            {
              inAir = true;
              StartCoroutine(performRumbleSide(j1, j2, A1DeltaY));
            }
          }
        }
        else
        {
          if (A2DeltaZ > A2DeltaY)
          {
            if (A2DeltaZ > threshold && !inAir)
            {
              inAir = true;
              StartCoroutine(performRumble(j2, j1, A2DeltaZ));
            }
          }
          else
          {
            if (A2DeltaY > threshold && !inAir)
            {
              inAir = true;
              StartCoroutine(performRumbleSide(j2, j1, A2DeltaY));
            }
          }
        }
      }
    }
  }

  // Simulate a throw of a ball by decreasing rumble intensity, then increasing it again
  IEnumerator performRumble(Joycon throw_j, Joycon catch_j, float speed)
  {
    for (float val = 1f; val >= 0f; val -= (0.1f * Mathf.Max((2 - speed), 1f)))
    {
      throw_j.SetRumble(160, 320, val, 100);
      yield return new WaitForSeconds(0.1f);
    }
    for (float val = 0f; val <= 1f; val += (0.2f * Mathf.Max((2 - speed), 1f)))
    {
      catch_j.SetRumble(160, 320, val, 100);
      yield return new WaitForSeconds(0.1f);
    }
    isCon1 = !isCon1;
    inAir = !inAir;
  }

  IEnumerator performRumbleSide(Joycon throw_j, Joycon catch_j, float speed)
  {
    for (float val = 1f; val >= 0f; val -= (0.1f * Mathf.Max((4 - speed), 1)))
    {
      if (val > .3f) {
        throw_j.SetRumble(160, 320, val, 100);
      }
      if (val < .5f)
      {
        catch_j.SetRumble(160, 320, 1 - (2 * val), 100);
      }
      yield return new WaitForSeconds(0.1f);
    }
    isCon1 = !isCon1;
    inAir = !inAir;
  }
}
