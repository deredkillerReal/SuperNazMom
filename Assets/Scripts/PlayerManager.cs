using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static Player[] players;
    public int playerNum = 0;
    [SerializeField] GameObject winscreen;

    private void Awake()
    {
        if (players == null) players = new Player[4];


    }

    public Slider AddPlayer(Player player)
    {
        bool hasPlaced = false;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) { players[i] = player; hasPlaced = true; playerNum = i; break; }
        }
        if (!hasPlaced) Debug.LogError("player array full too many players");
        Slider healthSlider;
        healthSlider = PlayerManager.spawnHealthBar(playerNum).GetComponent<Slider>();
        healthSlider.maxValue = 100;
        healthSlider.value = healthSlider.maxValue;
        return healthSlider;    

    }
    public void checkForWin()
    {
        int playersAlive = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                playersAlive++;

            }
        }
        if (playersAlive == 1)
        {            
            winscreen.SetActive(true);
            Debug.Log(winscreen.gameObject.name+" "+winscreen.name);
            Debug.LogError("YOU.WIN");
        }

    }
    public static GameObject spawnHealthBar(int player)
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        
        GameObject HealthBar = Resources.Load<GameObject>("HealthBar");
        //Instantiate(HealthBar,canvas.transform);
        GameObject bar = Instantiate(HealthBar,
            new Vector3(
              canvas.GetComponent<RectTransform>().rect.width * (player * 2 + 1) / 4 // set position relative to the left
            , canvas.GetComponent<RectTransform>().rect.height * 15 / 16, 0)       //  set position relative to the bottom
            , Quaternion.identity, canvas.transform);                            // rotate 0 and set canvas as parent

        return bar;

    }

}
