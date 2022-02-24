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
        UILabel watermark = null;

        bool showDataInWatermark = true;
        bool showTimeToWin = true;

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
            });

            Events.Scene.BeginSceneSwitchFadeOut.Subscribe((data) =>
            {
                if (watermark != null)
                {
                    watermark.text = "";
                }
                DualSpectateMode.Reset();
            });

            manager.Hotkeys.BindHotkey("RightControl+M", () => {
                if (G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true)
                {
                    var cam = G.Sys.PlayerManager_.Current_.playerData_.CarCamera_;
                    DualSpectateMode.ActivateDualSpectate(this, cam, G.Sys.PlayerManager_.Current_.playerData_.CarCamera_.GetComponent<SpectatorCameraLogic>());
                }
            });
        }

        void Update()
        {
            DualSpectateMode.UpdateDualSpectate();

            if (G.Sys.PlayerManager_?.Current_?.playerData_?.Finished_ == true &&
                G.Sys.GameManager_?.Mode_?.IsStarted_ == true &&
                G.Sys.NetworkingManager_.IsOnline_)
            //if (G.Sys.GameManager_?.Mode_?.IsStarted_ == true && G.Sys.NetworkingManager_.IsOnline_)
            {
                if (G.Sys.PlayerManager_?.Current_?.playerData_?.CarCamera_?.HasComponent<SpectatorCameraLogic>() == true && showDataInWatermark
                    && G.Sys.GameManager_.ModeID_ != GameModeID.ReverseTag)
                //if (showDataInWatermark)
                {

                    watermark.enabled = true;

                    var watermarkTextToSetTo = "[c][ffffff]";)

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
