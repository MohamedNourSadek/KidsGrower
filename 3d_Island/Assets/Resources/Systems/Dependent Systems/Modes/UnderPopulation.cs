using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnderPopulation : AbstractMode
{
    float countdownTime = 20f;

    public UnderPopulation(Mode_Data data) : base(data)
    {

    }

    protected override void OnLoad()
    {
        base.OnLoad();


        if (!data.gameStarted)
        {
            data.timeSinceStart = 0;
            OnFirstLoad();
        }

        ServicesProvider.instance.StartCoroutine(EndConition());
    }
    protected override void OnFirstLoad()
    {
        base.OnFirstLoad();
        ServicesProvider.instance.StartCoroutine(StartLogic());
    }


    IEnumerator StartLogic()
    {
        UIGame.instance.countDownText.text = "";

        GameManager.instance.LockPlayer(true);

        yield return new WaitForSeconds(1f);

        UIGame.instance.ShowFloatingMessage("Under Population", 1.5f ,new Vector3(1,1,1), 1f);

        yield return new WaitForSeconds(1.5f);

        UIGame.instance.ShowFloatingMessage("3", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        UIGame.instance.ShowFloatingMessage("2", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        UIGame.instance.ShowFloatingMessage("1", 1f, new Vector3(1, 1, 1), 1f);

        GameManager.instance.SpawnEgg();
        data.gameStarted = true;

        yield return new WaitForSeconds(1f);
        UIGame.instance.ShowFloatingMessage("Go !", 1f, new Vector3(1, 1, 1), 1f);

        GameManager.instance.LockPlayer(false);
    }
    IEnumerator EndConition()
    {
        float time = countdownTime - data.timeSinceStart;

        while(true)
        {
            if(NPC.NPCsCount <= 0)
            {
                UIGame.instance.countDownText.text = "No Children!! \n" + Helpers.GetTimeFormated(time);

                time -= Time.fixedDeltaTime;

                if(time <= 0)
                {
                    UIGame.instance.countDownText.text = "";
                    UIGame.instance.CloseAllPanels();
                    GameManager.instance.SetPlaying(false);
                    GameManager.instance.SetBlur(true);

                    
                    UIGame.instance.ShowPopUpMessage(
                        "You've Lost !",
                        "You've Survived for " + Helpers.GetTimeFormated(data.timeSinceStart),
                        "Exit",
                        GameManager.instance.ExitWithoutSaving);

                    DataManager.instance.Remove(DataManager.instance.GetCurrentSession().sessionName);

                    break;
                }

            }
            else
            {
                if(NPC.NPCsCount == 1)
                    UIGame.instance.countDownText.text = NPC.NPCsCount + " Child ";
                else
                    UIGame.instance.countDownText.text = NPC.NPCsCount + " Children ";


                time = countdownTime;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
