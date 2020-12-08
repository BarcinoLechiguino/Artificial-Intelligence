using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public int              m_NumRoundsToWin        = 5;            // Will set the amount of round wins needed to win the game.
    public float            m_StartDelay            = 3f;           // Will define the amount of delay before starting any given round. In seconds.
    public float            m_EndDelay              = 3f;           // Will define the amount of delay before finishing any given round. In seconds.
    public CameraControl    m_CameraControl;                        // Reference to the CameraControl class. Will be used to control the camera when the round starts, ends... 
    public Text             m_MessageText;                          // String that will be used to display the round the players are in as well as who has won and how many wins each has.
    public Scoreboard       m_Scoreboard;                           // Reference to the scoreboard class. The scoreboard script will show a scoreboard with the current game state on screen.
    public GameObject       m_TankPrefab;                           // Reference to the Tank Prefab. Will be used as the blueprint with which instantiate all the tanks in the game.
    public TankManager[]    m_Tanks;                                // Array of Tank Managers. Each element inside it holds the configuration information for a specific tank instance.

    public Transform        m_patrol_root_waypoint;                 // Will store the root waypoint of a patrol route. The childs will be iterated and stored in another array.
    //public Transform[]      m_patrol_root_waypoints;              // Array that will store the root waypoint of a patrol route. The childs will be iterated and stored in another array.

    private int             m_RoundNumber;                          // Will store the number of the round being currently played.
    private WaitForSeconds  m_StartWait;                            // WaitForSeconds will stop a coroutine for the given amount of seconds using scaled time.
    private WaitForSeconds  m_EndWait;                              // In this case, two will be created. One will stop the coroutine at the start of the round and the other at the end.
    private TankManager     m_RoundWinner;                          // Reference to the Tank Manager of the tank instance that has won the round.
    private TankManager     m_GameWinner;                           // Reference to the Tank Manager of the tank instance that has won the game.

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_Scoreboard.gameObject.SetActive(false);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        Transform[] patrol_waypoints = GetPatrolWaypoints();                                            // Called here so the iteration of the childs is only done once.
        
        if (m_Tanks.Length % 2 != 0)                                                                    // In case the number of tanks is not even.
        {
            // --------------------------------------------------------------------------
            // Spawn each tank individually.
            // Should make it so the target changes based on which tank is closest to it.
            // --------------------------------------------------------------------------

            for (int i = 0; i < m_Tanks.Length; ++i)
            {
                m_Tanks[i].m_Instance = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;

                if (i % 2 != 0)
                {
                    m_Tanks[i].m_AI_behaviour = "Wanderer";
                }
                else
                {
                    m_Tanks[i].m_AI_behaviour       = "Patroller";
                    m_Tanks[i].m_root_waypoint      = m_patrol_root_waypoint;
                    m_Tanks[i].m_patrol_waypoints   = patrol_waypoints;
                }

                m_Tanks[i].Setup();
            }
        }
        else
        {
            // --------------------------------------------------------------------------
            // Spawn tanks in pairs. Dirty fix to set an enemy target on each tank. 
            // Should make it so the target changes based on which tank is closest to it.
            // --------------------------------------------------------------------------

            for (int i = 0; i < m_Tanks.Length; i += 2)
            {
                m_Tanks[i].m_Instance               = Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i + 1].m_Instance           = Instantiate(m_TankPrefab, m_Tanks[i + 1].m_SpawnPoint.position, m_Tanks[i + 1].m_SpawnPoint.rotation) as GameObject;

                m_Tanks[i].m_PlayerNumber           = i + 1;
                m_Tanks[i + 1].m_PlayerNumber       = i + 2;

                m_Tanks[i].m_target                 = m_Tanks[i + 1].m_Instance;                // Adding the target of each tank to be the next/previous in the m_Tanks list.
                m_Tanks[i + 1].m_target             = m_Tanks[i].m_Instance;                    // Targets will be set in pairs: T0->T1 & T0->T1, T2->T3 & T3->T2...

                m_Tanks[i].m_AI_behaviour           = "Wanderer";
                m_Tanks[i + 1].m_AI_behaviour       = "Patroller";
                m_Tanks[i + 1].m_root_waypoint      = m_patrol_root_waypoint;
                m_Tanks[i + 1].m_patrol_waypoints   = patrol_waypoints;

                m_Tanks[i].Setup();
                m_Tanks[i + 1].Setup();
            }
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundWinner = null;
        ++m_RoundNumber;

        m_MessageText.text = "ROUND" + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = string.Empty;
        m_Scoreboard.gameObject.SetActive(true);
        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = GetRoundWinner();
         UpdateScoreBoard();
        m_GameWinner = GetGameWinner();
        m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
            {
                numTanksLeft++;
            }
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
            {
                if (m_Tanks[i].m_PlayerNumber == 1)
                {
                    m_Scoreboard.red_lives--;
                }
                else
                {
                    m_Scoreboard.blue_lives--;
                }
                
                m_Tanks[i].m_Wins++;
                
                return m_Tanks[i];
            }
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
            {
                return m_Tanks[i];
            }
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";


        if (m_RoundWinner != null)
        {
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
        }

        message += "\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
        {
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
        }

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    private void UpdateScoreBoard()
    {
        m_Scoreboard.UpdateBlueTanks();
        m_Scoreboard.UpdateRedTanks();
    }
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private Transform[] GetPatrolWaypoints()                                                                                            // Maybe put elsewhere? TankManager?
    {   
        int num_waypoints = m_patrol_root_waypoint.childCount;                                                                          // Getting the total amount of childs in the root.

        Transform[] waypoints = new Transform[num_waypoints];                                                                           // Allocating the required memory.

        for (int i = 0; i < num_waypoints; ++i)
        {
            waypoints[i] = m_patrol_root_waypoint.GetChild(i);                                                                          // Getting the child i inside the root transform.
        }

        return waypoints;
    }
}