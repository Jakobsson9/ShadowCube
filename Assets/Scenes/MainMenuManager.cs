﻿using Cubes;
using ShadowCube.Setting;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CameraMoveOne cameraMoveOne;
    [SerializeField] private List<CubeLogic> Cubes;
    [Header("Controllers")]
    [SerializeField] private ControllerMainMenu mainMenu;
    [SerializeField] private ControllerPlayMenu menuPlay;
    [SerializeField] private ControllerOnlineMenu menuOnline;
    [SerializeField] private ControllerPersonMenu menuPerson;
    [SerializeField] private ControllerOptionMenu menuOptions;
    [SerializeField] private ControllerAbout menuAbout;

    [Header("Other")]
    [SerializeField] private CPC_CameraPath cameraPath;
    [SerializeField] private GameObject person;

    private string _namePanel;

    [Inject] GameSetting gameSetting;

    private void Start()
	{
        mainMenu.EventButtonClick += Event_ButtonClick;
        mainMenu.EventClose.AddListener(Handler_MainMenu_Close);

        menuPlay.EventButtonPlayClick.AddListener(Event_ButtonPlayClick);
        menuPlay.EventClose.AddListener(Event_Menu_Close);
        menuOnline.EventClose.AddListener(Event_Menu_Close);
        menuPerson.EventClose.AddListener(Event_MenuPerson_Close);
        menuOptions.EventClose.AddListener(Event_Menu_Close);
        menuAbout.EventClose.AddListener(Event_Menu_Close);

        ShowCube(0, gameSetting.indexCube);
    }

    private void Event_ButtonPlayClick()
	{
        End();
        menuPlay.Deactive();
        Cubes[gameSetting.indexCube].OpenDoor(2);
        cameraMoveOne.gameObject.SetActive(false);
        Camera.main.transform.localEulerAngles = new Vector3(0, 90, 0);
        StartCoroutine(Animation_Camera());
    }

    private void Event_ButtonClick(string name)
	{
        _namePanel = name;
        mainMenu.Deactive();
    }

    private void Handler_MainMenu_Close()
	{
        switch (_namePanel)
        {
            case "PlayMenu": menuPlay.Init(new ModelPlayMenu() { mainMenuManager = this }); break;
            case "OnlineMenu": menuOnline.Init(new ModelOnlineMenu()); break;
            case "PersonMenu":
                cameraPath.points.Reverse();
                cameraPath.PlayPath(2);
                person.SetActive(true);
                cameraMoveOne.gameObject.SetActive(false);
                menuPerson.Init(new ModelPersonMenu(gameSetting.playerDTO)); 
                break;
            case "OptionMenu": menuOptions.Init(new ModelOptionMenu()); break;
            case "AboutMenu": menuAbout.Init(new IModel()); break;
        }
    }

    private void Event_Menu_Close()
    {
        mainMenu.Init(new IModel());
    }

    private void Event_MenuPerson_Close()
    {
        cameraPath.points.Reverse();
        cameraPath.PlayPath(2);
        person.SetActive(false);
        cameraMoveOne.gameObject.SetActive(true);
        Observable.Timer(System.TimeSpan.FromSeconds(2f))
        .Subscribe(_ => { mainMenu.Init(new IModel()); });
    }

    public void ShowCube(int index, int index2)
	{
        Cubes[index].gameObject.SetActive(false);
        Cubes[index2].gameObject.SetActive(true);
    }

    private void End()
	{
        mainMenu.EventButtonClick -= Event_ButtonClick;

        menuPlay.EventButtonPlayClick.RemoveListener(Event_ButtonPlayClick);
        menuPlay.EventClose.RemoveListener(Event_Menu_Close);
        menuOnline.EventClose.RemoveListener(Event_Menu_Close);
        menuPerson.EventClose.RemoveListener(Event_Menu_Close);
        menuOptions.EventClose.RemoveListener(Event_Menu_Close);
        menuAbout.EventClose.RemoveListener(Event_Menu_Close);
    }

    private IEnumerator Animation_Camera()
    {
        yield return new WaitForSecondsRealtime(4f);
        for (int i = 0; i < 120; i++)
        {
            Camera.main.transform.position += new Vector3(0.02f, 0f, 0f);
            yield return new WaitForSecondsRealtime(0.03f);
        }
        SceneManager.LoadScene(1);
    }
}
