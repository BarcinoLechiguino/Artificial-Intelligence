using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public Canvas canvas;
    public GameObject blue_tank;
    public GameObject red_tank;
    public int blue_lives;
    public int red_lives;
    public GameObject[] b_tank_instants;
    public GameObject[] r_tank_instants;
    Transform b_transform;
    Transform r_transform;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {

        b_transform = blue_tank.GetComponent<Transform>();
        r_transform = red_tank.GetComponent<Transform>();

        b_tank_instants = new GameObject[blue_lives];
        r_tank_instants = new GameObject[red_lives];
        UpdateBlueTanks();
        UpdateRedTanks();
    }
    public void UpdateBlueTanks()
    {
        if (blue_lives > 0)
        {
            ResetTankBuffer(b_tank_instants);
            b_tank_instants = new GameObject[blue_lives];
            for (int i = 0; i < blue_lives; i++)
            {
                b_tank_instants[i] = Instantiate(blue_tank, new Vector3(b_transform.position.x + (-offset * i), b_transform.position.y, b_transform.position.z), b_transform.rotation, this.transform);
            }
        }
        else
        {
            Destroy(blue_tank);
        }

    }
    public void UpdateRedTanks()
    {
        if (red_lives > 0)
        {
            ResetTankBuffer(r_tank_instants);
            r_tank_instants = new GameObject[red_lives];
            for (int i = 0; i < red_lives; i++)
            {
                r_tank_instants[i] = Instantiate(red_tank, new Vector3(r_transform.position.x + (offset * i), r_transform.position.y, r_transform.position.z), r_transform.rotation, this.transform);
            }
        }
        else
        {
            Destroy(red_tank);
        }

    }

    public void ResetTankBuffer(GameObject[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            Destroy(buffer[i]);
            buffer[i] = null;
        }
    }
}
