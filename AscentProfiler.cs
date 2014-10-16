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
                public static FlightProfile ActiveProfile;
                private AscentProfilerGUI gui = null;
                private bool mainWindowEnabled = true;
                private IButton mainButton;

                public static string AscentProfilerDir;
                public static string profilesDir;
                public static string flightlogsDir;

                void Awake()
                {


                }


                void Start()
                {
                        Debug.Log("Ascent Profiler: Is Awake!");

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

                        Debug.Log("Ascent Profiler: User Directory: " + AscentProfilerDir);
                        Debug.Log("Ascent Profiler: Profiles Directory: " + profilesDir);
                        Debug.Log("Ascent Profiler: Flight Logs Directory: " + flightlogsDir); 
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
