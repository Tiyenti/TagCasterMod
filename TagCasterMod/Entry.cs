using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactor.API;
using Reactor.API.Attributes;
using Reactor.API.Interfaces.Systems;
using UnityEngine;
using Centrifuge.Distance.Game;

namespace TagCasterMod
{
    [ModEntryPoint("com.github.tiyenti/TagCasterMod")]
    public class Entry : MonoBehaviour
    {
        internal static UILabel watermark = null;

        bool showDataInWatermark = true;
        bool showTimeToWin = true;

        bool autoEnterSpectate = true;
        bool autoEnterSpectateHideMenu = true;

        bool autoEnterDualSpectate = true;
        bool autoShowMenuAfterFinish = true;

        bool fixPauseUnhide = true;

        public void Initialize(IManager manager)
        {
            DontDestroyOnLoad(this);
        }

        bool firstMenuLoad = true;

        public void LateInitialize(IManager manager)
        {
            Events.MainMenu.Initialized.Subscribe((data) =>
            {
                if (firstMenuLoad)
                {
                    watermark = GameObject.Find("AlphaVersion").GetComponent<UILabel>();
                    watermark.width = 500;
                    watermark.enabled = true;
                    watermark.effectColor = new Color(0, 0, 0, 1);
                    watermark.effectStyle = UILabel.Effect.Outline ;
                    firstMenuLoad = false;
                }
                else
                {
                    
                }

                watermark.text = "[c][00FF8C]TAG CASTER MOD ENABLED[-][/c]";

                if (showDataInWatermark) watermark.text += "\nShow Tag Scoreboard |";
                if (showDataInWatermark) watermark.text += "\nShow Time To Win --|";
                if (autoEnterSpectate) watermark.text += "\n\nAuto Enter Spectate |";
                if (autoEnterSpectateHideMenu) watermark.text += "\nHide Finish Menu Automatically --|";
                if (autoEnterDualSpectate) watermark.text += "\nAuto Enter Dual Spectate --|";
                if (autoShowMenuAfterFinish) watermark.text += "\nAuto Show Menu After Finish --|";
                if (fixPauseUnhide) watermark.text += "\nFix Pause Unhinde --|";
            });

            Events.Scene.BeginSceneSwitchFadeOut.Subscribe((data) =>
            {
                if (watermark != null)
                {
                    watermark.text = "";
                }
                DualSpectateMode.Reset();
            });

            Events.GameMode.ModeStarted.Subscribe((data) =>
            {
                if (G.Sys.NetworkingManager_.IsOnline_ && autoEnterSpectate)
                {
                    Console.WriteLine("Auto-enter spectate activated");
                    G.Sys.PlayerManager_.Current_.playerData_.Spectate();
                    if (autoEnterSpectateHideMenu)
                    {
                        var menu = FindObjectOfType<FinishMenuLogic>();
                        if (menu != null)
                        {
                            menu.menuPanel_.TempHide(menu.mainPanel_.gameObject);
                            menu.SetState(MenuWithToggleVisibility.VisibleState.HiddenAndNamesHidden);
                        }

                    }
                    if (autoEnterDualSpectate)
                    {
                        activateDualSpectateSoon = true;
                    }
                }
            });

            /*Events.GameMode.Go.Subscribe((data) =>
            {
                
            });*/

            Events.Game.PauseToggled.Subscribe((data) =>
            {
                if (G.Sys.NetworkingManager_.IsOnline_ && G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true)
                {
                    if (fixPauseUnhide && data.paused_ == false)
                    {
                        var menu = FindObjectOfType<FinishMenuLogic>();
                        if (menu != null)
                        {
                            if (menu.visibleState_ != MenuWithToggleVisibility.VisibleState.Visible)
                            {
                                menu.menuPanel_.TempHide(menu.mainPanel_.gameObject);
                            }
                        }
                    }
                }
            });

            Events.ChatWindow.ChatVisibilityChanged.Subscribe((data) =>
            {
                if (G.Sys.NetworkingManager_.IsOnline_ && G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true)
                {
                    if (fixPauseUnhide && data.isShowing_ == false)
                    {
                        var menu = FindObjectOfType<FinishMenuLogic>();
                        if (menu != null)
                        {
                            if (menu.visibleState_ != MenuWithToggleVisibility.VisibleState.Visible)
                            {
                                menu.menuPanel_.TempHide(menu.mainPanel_.gameObject);
                            }
                        }
                    }
                }
            });

            Events.Player.Finished.SubscribeAll((instance, data) =>
            {
                if (G.Sys.NetworkingManager_.IsOnline_ && G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true)
                {
                    if (autoShowMenuAfterFinish)
                    {
                        var menu = FindObjectOfType<FinishMenuLogic>();
                        if (menu != null)
                        {
                            menu.SetState(MenuWithToggleVisibility.VisibleState.Visible);
                        }
                    }
                }
            });

            manager.Hotkeys.BindHotkey("RightControl+M", () => {
                if (G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true)
                {
                    var cam = G.Sys.PlayerManager_.Current_.playerData_.CarCamera_;
                    DualSpectateMode.ActivateDualSpectate(this, cam, G.Sys.PlayerManager_.Current_.playerData_.CarCamera_.GetComponent<SpectatorCameraLogic>());
                }
            });
        }

        bool activateDualSpectateSoon = false;
        float activateDualSpecateDelay = 3f;
        float activateDualSpectateTime = 0f;

        void Update()
        {
            DualSpectateMode.UpdateDualSpectate();

            if (activateDualSpectateSoon)
            {
                activateDualSpectateTime += Time.deltaTime;
                if (activateDualSpectateTime > activateDualSpecateDelay)
                {
                    var cam = G.Sys.PlayerManager_.Current_.playerData_.CarCamera_;
                    DualSpectateMode.ActivateDualSpectate(this, cam, G.Sys.PlayerManager_.Current_.playerData_.CarCamera_.GetComponent<SpectatorCameraLogic>());

                    activateDualSpectateSoon = false;
                    activateDualSpectateTime = 0f;
                }
            }

            if (G.Sys.PlayerManager_?.Current_?.playerData_?.Finished_ == true &&
                G.Sys.GameManager_?.Mode_?.IsStarted_ == true &&
                G.Sys.NetworkingManager_.IsOnline_)
            //if (G.Sys.GameManager_?.Mode_?.IsStarted_ == true && G.Sys.NetworkingManager_.IsOnline_)
            {
                if (G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true && showDataInWatermark
                    && G.Sys.GameManager_.ModeID_ == GameModeID.ReverseTag)
                //if (showDataInWatermark)
                {

                    watermark.enabled = true;

                    var watermarkTextToSetTo = "[c][ffffff]";

                    if (G.Sys.GameManager_.ModeID_ == GameModeID.ReverseTag && showTimeToWin)
                    {
                        watermarkTextToSetTo += GUtils.GetFormattedTime(((ReverseTagMode)G.Sys.GameManager_.Mode_).TimeLimit_, true) + " to win\n";
                    }

                        var list = new List<ModePlayerInfoBase>();
                    G.Sys.GameManager_.Mode_.GetSortedListOfModeInfosNotFinished(list);
                    var index = 0;
                    foreach (var a in list)
                    {
                        if (G.Sys.GameManager_.ModeID_ == GameModeID.ReverseTag)
                        {
                            if (a == ((ReverseTagMode)G.Sys.GameManager_.Mode_).TaggedPlayer_)
                            {
                                watermarkTextToSetTo += "[D1A7FF]";
                            }
                            else
                            {
                                watermarkTextToSetTo += "[ffffff]";
                            }
                        }

                        watermarkTextToSetTo += $"{a.Name_}[-] | {a.pData_.PrimaryColor_.ToFormattedHex()}█[-]" +
                                                $"{a.pData_.SecondaryColor_.ToFormattedHex()}█[-]" +
                                                $"{a.pData_.OriginalGlowColor_.ToFormattedHex()}█[-]" ;

                        if (G.Sys.GameManager_.ModeID_ == GameModeID.ReverseTag)
                        {
                            watermarkTextToSetTo += $" | {GUtils.GetFormattedTime(a.modeData_, true)}";
                        }
                        else if (G.Sys.GameManager_.ModeID_ == GameModeID.Sprint)
                        {
                            watermarkTextToSetTo += $" | {GUtils.GetFormattedDistance(a.modeData_)}";
                        }
                        else
                        {
                            watermarkTextToSetTo += $" | {a.modeData_}";
                        }

                        if (index == 0)
                        {
                            watermarkTextToSetTo += " [EFD034]| 1[-]";
                        }
                        else if (index == 1)
                        {
                            watermarkTextToSetTo += " [A0A0A0]| 2[-]";
                        }
                        else if (index == 2)
                        {
                            watermarkTextToSetTo += " [BC5C1C]| 3][-]";
                        }
                        else
                        {
                            watermarkTextToSetTo += $"| {index+1}";
                        }
                        watermarkTextToSetTo += "[/c]\n";
                        index++;
                    }

                    watermark.text = watermarkTextToSetTo;
                }
            }
        }
    }
}
