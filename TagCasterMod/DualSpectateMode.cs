﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace TagCasterMod
{
    class DualSpectateMode
    {
        static bool dualSpectateActive = false;

        static Camera secondCam;

        public static void ActivateDualSpectate(Entry e, CarCamera initialCarCam, SpectatorCameraLogic spc)
        {
            if (dualSpectateActive)
            {
                Console.WriteLine("Dual spectate already active!");
                return;
            }


            Console.WriteLine("Dual spectate mode is being activated");
            dualSpectateActive = true;

            //var secondCamObj = UnityEngine.Object.Instantiate(initialCarCam.gameObject);
            //var secondCamObj = new GameObject();
            //var secondCam = secondCamObj.AddComponent<Camera>();

            var pdf = PlayerDataFake.InitializeFakePlayerData(G.Sys.ProfileManager_.CurrentProfile_, false);
            pdf.carCamera_.isSpectating_ = true;
            pdf.carCamera_.camera_.cullingMask |= 1 << 1;

            pdf.carCamera_.GetOrAddComponent<SpectatorCameraLogic>();


            initialCarCam.camera_.rect = new Rect(0, 0, 0.5f, 1);
            pdf.CarCamera_.camera_.rect = new Rect(0.5f, 0, 0.5f, 1);

            

            //secondCamObj.RemoveComponent<AdjustRadialBlur>();
            //secondCamObj.RemoveComponent<PlayerSpecificRenderingCamera>();

        }

        public static void UpdateDualSpectate()
        {
            if (secondCam && dualSpectateActive)
            {
                /*List<PlayerDataBase> a = new List<PlayerDataBase>();
                if (G.Sys.NetworkingManager_.IsOnline_)
                {
                    G.Sys.GameManager_.Mode_.GetSortedListOfUnfinishedPlayers(a);
                }
                else
                {
                    foreach (var b in PlayerDataReplay.ReplayPlayers_)
                    {
                        if (b != null && !b.Finished_)
                        {
                            a.Add(b);
                        }
                    }
                }

                secondCam.transform.position = a[0].transform.position;*/
                
            }
        }
    }
}
