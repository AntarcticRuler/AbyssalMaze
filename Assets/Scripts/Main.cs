using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    System.Random random = new System.Random();

    int[,] map = new int[100, 100];

    int y, x;

    bool streak = false;

    public Transform playerTransform;
    public Transform endTransform;

    float speed = 0.05f;

    int steps = 0;
    int currSteps = 0;
    int stepsInCurrDirection = 0;
    bool w, a, s, d;
    public Text stepsText;
    int deaths = 0;
    int battlesWon = 0;

    int minutes = 0;
    int seconds = 0;
    public Text time;

    bool start = false, stop = true, inBattle = false;
    string introDialogue = "Shrouded in darkness, you shall reach liberation. The road may not be so clear... \n\n\nFind your way to the top right corner of this labyrinth and you shall be freed. Be careful; you may find yourself where you began... \n\n\nGood luck.";
    string endDialogue = "You have strayed further than I have thought from where you began. Beyond you is light, \nso walk forth with vison of what lies ahead.";
    string fightDialogue = "An unseeable foe has iniated combat against you.";
    public Text dialogue;
    public Text stats;
    public Text title;
    string currText;

    string[] remarks = { "Seems you still have a ways to go.", "Seems luck doeson't favor the bold.", "'I have been slain!' Ha.", "Nothing personal...", "Next time there's a possibility..." };

    float viewLen, viewWidth;

    // Battle Variables
    int baseEnemyHP = 20;
    int enemyHP = 20;
    int enemyAttack = 5;
    int enemyDefense = 1;
    int enemyDefend = 2;

    int playerHP = 35;
    int playerAttack = 7;
    int playerDefend = 3;
    int playerDefense = 1;
    int playerRestore = 4;

    int turn = 0;

    // Start is called before the first frame update
    void Start()
    {
        mapGeneration();
        x = 0;
        y = map.GetLength(1) - 1;

        viewLen = 9f;
        viewWidth = 9f;

        StopAllCoroutines();
        StartCoroutine(typewriteDialogue(introDialogue));
        StartCoroutine(abyssalMaze("Abyssal Maze\n\nMade by AntarcticRuler (Nick)"));
    }

    // Update is called once per frame
    void Update()
    {
        timeUpdate();
        speedUp();
        checkMovement();
        showSprite();
        showText();

        if (inBattle)
            battleTurn();
    }

    // Generates the map
    // Player = 2
    // Obstacle = 1
    // Empty = 0
    void mapGeneration ()
    {
        for (int py = 0; py < map.GetLength(0); py++) 
        {
            for (int px = 0; px < map.GetLength(1); px++) 
            {
                int rng = random.Next(0, 100);

                if (streak)
                    rng += random.Next (5, 15);

                if (rng <= 85)
                {
                    map[px, py] = 0;
                    streak = false;
                }

                else if (rng < 90)
                {
                    map[px, py] = 3;
                    streak = false;
                }

                else if (rng >= 90)
                {
                    map[px, py] = 1;
                    streak = true;
                }

            }
        }
        // Clears up player area
        map[map.GetLength(1) - 2, 1] = 2;
        map[map.GetLength(1) - 3, 1] = 0;
        map[map.GetLength(1) - 2, 3] = 0;

        // Clears up end area
        map[map.GetLength(1) - 1, map.GetLength(1) - 1] = 0;
        map[map.GetLength(1) - 2, map.GetLength(1) - 2] = 0;
        map[map.GetLength(1) - 1, map.GetLength(1) - 2] = 0;
        map[map.GetLength(1) - 2, map.GetLength(1) - 1] = 0;
        printMap();
    }

    // Kinda prints the map in the console I guess.
    void printMap()
    {
        string printMap = "";
        for (int px = 0; x < map.GetLength(0); x++)
        {
            for (int py = 0; y < map.GetLength(1); y++)
            {
                printMap += map[py, px] + " ";
            }
            printMap += "\r\n";
        }
        Debug.Log(printMap);
    }

    // Does the movement. If you run into something you restart. If not you continue.
    void checkMovement () {
        if (stop)
            return;

        if (Input.GetKeyDown("w"))
        {
            if (y > 0)
            {
                if (!checkObstacle(1))
                {
                    Debug.Log(x + " | " + y);
                    w = true;
                    map[y, x] = 0;
                    y--;
                    map[y, x] = 2;
                    steps++; currSteps++;

                    if (w)
                        stepsInCurrDirection++;
                    if (a || s || d)
                    {
                        stepsInCurrDirection = 1;
                        a = false; s = false; d = false;

                    }
                }
                else if (map[y - 1, x] == 3)
                {
                    map[y - 1, x] = 0;
                    initiateBattle();
                }
                else
                    restart();
            }
        }
        else if (Input.GetKeyDown("a"))
        {
            if (x > 0) {
                if (!checkObstacle(2))
                {
                    Debug.Log(x + " | " + y);
                    a = true;
                    map[y, x] = 0;
                    x--;
                    map[y, x] = 2;
                    steps++; currSteps++;

                    if (a)
                        stepsInCurrDirection++;
                    if (w || s || d)
                    {
                        stepsInCurrDirection = 1;
                        w = false; s = false; d = false;
                    }

                }
                else if (map[y, x - 1] == 3)
                {
                    map[y, x - 1] = 0;
                    initiateBattle();
                }
                else
                    restart();
            }
        }
        else if (Input.GetKeyDown("s"))
        {
            if (y < 99 && !checkObstacle(3))
            {
                if (!checkObstacle(3))
                {
                    Debug.Log(x + " | " + y);
                    s = true;
                    map[y, x] = 0;
                    y++;
                    map[y, x] = 2;
                    steps++; currSteps++;

                    if (s)
                        stepsInCurrDirection++;
                    if (a || w || d)
                    {
                        stepsInCurrDirection = 1;
                        a = false; w = false; d = false;
                    }

                }
                else if (map[y + 1, x] == 3)
                {
                    map[y + 1, x] = 0;
                    initiateBattle();
                }
                else
                    restart();
            }
        }
        else if (Input.GetKeyDown("d"))
        {
            if (x < 99)
            {
                if (!checkObstacle(4))
                {
                    Debug.Log(x + " | " + y);
                    d = true;
                    map[y, x] = 0;
                    x++;
                    map[y, x] = 2;
                    steps++; currSteps++;

                    if (d)
                        stepsInCurrDirection++;
                    if (a || s || w)
                    {
                        stepsInCurrDirection = 1;
                        a = false; s = false; w = false;
                    }

                }
                else if (map[y, x + 1] == 3)
                {
                    map[y, x + 1] = 0;
                    initiateBattle();
                }
                else
                    restart();
            }
        }

        Debug.Log(x + " | " + y);

        if (x > 97 && y < 2)
            win();
    }

    // Checks for obstacles in all four directions
    // 1 - 4 clockwise starting at 12 o' clock
    bool checkObstacle (int pos) {
        if (pos == 1 && map[y - 1, x] != 0)
            return true;
        else if (pos == 2 && map[y, x - 1] != 0)
            return true;
        else if (pos == 3 && map[y + 1, x] != 0)
            return true;
        else if (pos == 4 && map[y, x + 1] != 0)
            return true;
        else
            return false;
    }

    // Shows the sprite on screen
    void showSprite () {
        Vector3 pos = playerTransform.position;
        pos.x = viewWidth / map.GetLength(1) * x;
        pos.y = viewLen - viewLen / map.GetLength(0) * y;
        pos.z = 0;
        playerTransform.position = pos;

        if (!start)
            return;
        Vector3 endPos = endTransform.position;
        endPos.x = viewWidth / map.GetLength(1) * 99;
        endPos.y = viewLen / map.GetLength(0) * 99;
        endPos.z = 0;
        endTransform.position = endPos;
    }

    // Shows text on screen
    void showText () {
        if (inBattle)
        {
            stepsText.text = "";
            stats.text = "STATS:\nHP: " + playerHP + "\nATTACK: " + playerAttack + "\nDEFENSE: " + playerDefense + "\n\nCOMMANDS\nATTACK - E\nDEFEND - Q\nRESTORE - R";
        }
        else
            stats.text = "";
        if (stop)
        {
            stepsText.text = "";
            time.text = "";
            return;
        }
        time.text = minutes + " : " + seconds;
        if (!start)
            return;
        stepsText.text = "STEPS TAKEN: " + steps + "\nCURRENT STEPS: " + currSteps + "\nSTEPS IN CURR DIRECTION: " + stepsInCurrDirection + "\nTIMES PERISHED: " + deaths + "\nBATTLES WON: " + battlesWon + "\n\n\nMOVEMENT: WASD\nSPEED-UP TEXT: TAB";
    }

    void timeUpdate () {
        seconds = (int)Time.time % 60;
        minutes = (int)Time.time / 60;
    }

    // Restarts the game
    void restart() {
        map[y, x] = 0;
        x = 1;
        y = map.GetLength(1) - 2;
        map[y, x] = 2;
        currSteps = 0;
        stepsInCurrDirection = 0;
        deaths++;
        dialogue.text = "";
        stepsText.text = "";
        playerHP = 25;
        StopAllCoroutines();
        int rng = random.Next(0, remarks.Length);
        StartCoroutine(reboot(remarks[rng]));
    }

    // Winner, winner, chicken dinner!
    void win () {
        stop = true;
        start = false;
        StopAllCoroutines();
        StartCoroutine(typewriterDialogueM(endDialogue));
    }

    IEnumerator typewriteDialogue(string str)
    {
        stop = true;
        dialogue.text = "";
        for (int i = 0; i <= str.Length; i++)
        {
            currText = str.Substring(0, i);
            dialogue.text = currText;
            yield return new WaitForSeconds(speed);
        }
        yield return new WaitForSeconds(2);
        start = true;
        endBattle();
        stop = false;
        dialogue.text = "";
    }

    IEnumerator typewriterDialogueM(string str)
    {
        dialogue.text = "";
        for (int i = 0; i <= str.Length; i++)
        {
            currText = str.Substring(0, i);
            dialogue.text = currText;
            yield return new WaitForSeconds(speed);
        }
    }

    // When you restart, some snarky thing the narrator says
    IEnumerator reboot(string str)
    {
        stop = true;
        dialogue.text = "";
        for (int i = 0; i <= str.Length; i++)
        {
            currText = str.Substring(0, i);
            dialogue.text = currText;
            yield return new WaitForSeconds(speed / 3f);
        }
        yield return new WaitForSeconds(1f);
        dialogue.text = "";
        start = true;
        stop = false;
    }

    // The title for the game
    IEnumerator abyssalMaze(string str)
    {
        title.text = str;
        yield return new WaitForSeconds(5f);
        for (int i = str.Length-1; i >= 0; i--)
        {
            currText = str.Substring(0, i);
            title.text = currText;
            if (i > 12)
                yield return new WaitForSeconds(speed * 1.2f);
            else if (i == 12)
                yield return new WaitForSeconds(14);
            else
                yield return new WaitForSeconds(speed * 3.2f);
        }
        title.text = "";
    }

    // Speeds up text.
    void speedUp()
    {
        if (Input.GetKey("tab"))
            speed = 0.025f;
        else
            speed = 0.1f;
    }

    // Initiates the battle
    void initiateBattle () {
        if (!start)
            return;
        Debug.Log("Battle initiated");
        stop = true;
        inBattle = true;
        StopAllCoroutines();
        StartCoroutine(typewriterDialogueM(fightDialogue));

        if (playerHP < 15)
            playerHP += 10;
        else if (playerHP < 20)
            playerHP += 5;
        else if (playerHP < 25)
            playerHP += 2;

        playerDefense = 1;
        enemyDefense = 1;

        enemyHP = baseEnemyHP;
    }

    // Determines who's turn it is in battle
    void battleTurn () {
        Debug.Log("Turn: " + turn);
        if (turn % 2 == 0)
            enemyTurn();
        else
            battleCommands();
    }

    // Player commands for battles
    void battleCommands ()
    {
        if (Input.GetKeyDown("q"))
        {
            Debug.Log("Defend");
            playerDefense += playerDefend;
            playerDefense = Mathf.Clamp(playerDefense, 0, enemyAttack - 2);
            turn++;
        }
        if (Input.GetKeyDown("e"))
        {
            Debug.Log("Attack");
            playerDefense--;
            int calcAttack = playerAttack - enemyDefense;
            enemyHP -= (Mathf.Clamp(random.Next(calcAttack-1, calcAttack+1), 0, 25));
            turn++;
        }
        if (Input.GetKeyDown("r"))
        {
            Debug.Log("Restore");
            enemyDefense--;
            playerHP += playerRestore;
            turn++;
        }
        if (enemyHP <= 0)
        {
            battlesWon++;
            inBattle = false;
            StopAllCoroutines();
            StartCoroutine(typewriteDialogue("You have vanquished the foe."));

            baseEnemyHP += 2;
        }
        if (playerHP <= 0)
        {
            //StartCoroutine(typewriteDialogue("You have been slain..."));
            start = false;
            endBattle();
            restart();
        }

    }

    // Enemy commands for battle
    void enemyTurn() {
        int rng = random.Next(0, 5);
        if (rng <= 3) {
            Debug.Log("Enemy Attack");
            enemyDefense--;
            int calcAttack = enemyAttack - playerDefense;
            playerHP -= (Mathf.Clamp(random.Next(calcAttack-1, calcAttack+1), 0, 35));
        }
        else {
            Debug.Log("Enemy Defend");
            enemyDefense += enemyDefend;
            enemyDefense = Mathf.Clamp(enemyDefense, 0, playerAttack-3);
        }
        turn++;
    }

    void endBattle () {
        dialogue.text = "";
        stop = false;
        inBattle = false;
    }
}
