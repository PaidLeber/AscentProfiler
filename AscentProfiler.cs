using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AscentProfiler
{

        [KSPAddon(KSPAddon.Startup.Flight, false)]
        public class AscentProfiler : MonoBehaviour
        {
                internal static FlightProfile ActiveProfile;
                static string AscentProfilerDir;
                internal static string profilesDir;
                internal static string flightlogsDir;

                AscentProfilerGUI gui = null;
                bool mainWindowEnabled = true;
                IButton mainButton;

                void Awake()
                {
                        Log.Level(LogType.Info, "Is Awake!");
                }


                void Start()
                {


                        ActiveProfile = gameObject.AddComponent<FlightProfile>();
                        gui = new AscentProfilerGUI();

                        if (ToolbarManager.ToolbarAvailable)
                        {
                                mainWindowEnabled = false;
                                mainButton = ToolbarManager.Instance.add("AscentProfiler", "AscentProfiler");
                                mainButton.TexturePath = "AscentProfiler/Textures/icon_blizzy";
                                mainButton.ToolTip = "Open Ascent Profiler";
                                mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
                                mainButton.OnClick += (e) =>
                                {
                                        gui.ChangeState(!mainWindowEnabled);
                                        mainWindowEnabled = !mainWindowEnabled;
                                };
                        }

                        // Create user home directory paths for saves
                        AscentProfilerDir = Application.persistentDataPath + "/Ascent Profiler";

                        if (AscentProfilerDir == "/Ascent Profiler")
                        {
                                string homePath =
                                (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                                ? Environment.GetEnvironmentVariable("HOME")
                                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

                                AscentProfilerDir = homePath + "/Ascent Profiler";

                        }

                        profilesDir     = AscentProfilerDir + "/Profiles";
                        flightlogsDir   = AscentProfilerDir + "/Flight Logs";
                        
                        if (!Directory.Exists(AscentProfilerDir))
                        {
                                Directory.CreateDirectory(AscentProfilerDir);
                        }

                        if (!Directory.Exists(profilesDir))
                        {
                                Directory.CreateDirectory(profilesDir);
                        }

                        if (!Directory.Exists(flightlogsDir))
                        {
                                Directory.CreateDirectory(flightlogsDir);
                        }

                        Log.Level(LogType.Info, "User Directory: " + AscentProfilerDir);
                        Log.Level(LogType.Info, "Profiles Directory: " + profilesDir);
                        Log.Level(LogType.Info, "Flight Logs Directory: " + flightlogsDir); 
                }


                void OnGUI()
                {
                        gui.OnGUI();
                }

                void Update()
                {
                 
                }

                void FixedUpdate() {

                }

  
                void onDestroy()
                {
                        mainButton.Destroy();
                        Destroy(GetComponent<FlightProfile>());
                        
                }


    }
}
