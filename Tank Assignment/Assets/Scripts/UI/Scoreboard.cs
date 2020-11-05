using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour //Script for the UI scoreboard 
{
    public GameObject blue_tank;        //GameObjects containing the UiImage that will instantiate
    public GameObject red_tank;
    public int blue_lives;              //Keeps track of the score
    public int red_lives;
    public GameObject[] b_tank_instants;    //we create buffers to store the instantiates
    public GameObject[] r_tank_instants;
    Transform b_transform;  //blue initial position
    Transform r_transform;  //red initial position
    public float offset;    //distance among the tank

    // Start is called before the first frame update
    void Start()
    {

        b_transform = blue_tank.GetComponent<Transform>();
        r_transform = red_tank.GetComponent<Transform>();

        b_tank_instants = new GameObject[blue_lives];
        r_tank_instants = new GameObject[red_lives];

        //Initialize scoreboard
        UpdateBlueTanks();
        UpdateRedTanks();
    }
    public void UpdateBlueTanks()   //Instantiates a new tank image for every life in the origin position - offset
    {
        ResetTankBuffer(b_tank_instants); //first we reset the buffer

        if (blue_lives > 0)
        {
            b_tank_instants = new GameObject[blue_lives];
            for (int i = 0; i < blue_lives; i++) //we loop for every life and instantiate
            {
                b_tank_instants[i] = Instantiate(blue_tank, new Vector3(b_transform.position.x + (-offset * i), b_transform.position.y, b_transform.position.z), b_transform.rotation, this.transform);
            }
        }
        else
        {
            Destroy(blue_tank); //If there's no lives left, just destroy the tank left
        }

    }
    public void UpdateRedTanks()    //Instantiates a new tank image for every life in the origin position + offset
    {
        ResetTankBuffer(r_tank_instants); //first we reset the buffer

        if (red_lives > 0)
        {
            r_tank_instants = new GameObject[red_lives];
            for (int i = 0; i < red_lives; i++)//we loop for every life and instantiate
            {
                r_tank_instants[i] = Instantiate(red_tank, new Vector3(r_transform.position.x + (offset * i), r_transform.position.y, r_transform.position.z), r_transform.rotation, this.transform);
            }
        }
        else
        {
            Destroy(red_tank); //If there's no lives left, just destroy the tank left
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
