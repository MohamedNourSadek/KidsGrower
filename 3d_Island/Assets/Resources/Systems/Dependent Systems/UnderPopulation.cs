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
            OnStart();
        }

        ServicesProvider.instance.StartCoroutine(EndConition());
    }
    protected override void OnStart()
    {
        base.OnStart();

        ServicesProvider.instance.StartCoroutine(StartLogic());
    }


    IEnumerator StartLogic()
    {
        UIController.instance.countDownText.text = "";

        gameManager.LockPlayer(true);

        yield return new WaitForSeconds(1f);

        controller.ShowUIMessage("Under Population", 1.5f ,new Vector3(1,1,1), 1f);

        yield return new WaitForSeconds(1.5f);

        controller.ShowUIMessage("3", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("2", 1f, new Vector3(1, 1, 1), 1f);
        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("1", 1f, new Vector3(1, 1, 1), 1f);

        gameManager.SpawnEgg();
        data.gameStarted = true;

        yield return new WaitForSeconds(1f);
        controller.ShowUIMessage("Go !", 1f, new Vector3(1, 1, 1), 1f);

        gameManager.LockPlayer(false);
    }
    IEnumerator EndConition()
    {
        float time = countdownTime - data.timeSinceStart;

        while(true)
        {
            if(NPC.NPCsCount <= 0)
            {
                UIController.instance.countDownText.text = "No Children!! \n" + Helpers.GetTimeFormated(time);

                time -= Time.fixedDeltaTime;

                if(time <= 0)
                {
                    UIController.instance.countDownText.text = "";
                    UIController.instance.CloseAllPanels();
                    gameManager.SetPlaying(false);
                    gameManager.SetBlur(true);

                    UIMessage message =  UIController.instance.PopUpMessage();

                    DataManager.instance.Remove(DataManager.instance.GetCurrentSession().sessionName);

                    message.header.text = "You've Lost !";
                    message.message.text = "You've Survived for " + Helpers.GetTimeFormated(data.timeSinceStart);
                    message.button.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";
                    message.button.onClick.AddListener(gameManager.ExitWithoutSaving);

                    break;
                }

            }
            else
            {
                if(NPC.NPCsCount == 1)
                    UIController.instance.countDownText.text = NPC.NPCsCount + " Child ";
                else
                    UIController.instance.countDownText.text = NPC.NPCsCount + " Children ";


                time = countdownTime;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
