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
                internal static Vessel currentVessel = null;
                static string AscentProfilerDir;
                internal static string profilesDir;
                internal static string flightlogsDir;

                AscentProfilerGUI gui = null;
                bool mainWindowEnabled = true;
                IButton mainButton;

                //Loaded Assemblies check
                bool assemblycheck = false;
                bool FAR = false;
                bool RT = false;
                bool KAC = false;
                bool TRAJ = false;

                void Awake()
                {
                        Log.Level(LogType.Info, "Is Awake!");
                }


                void Start()
                {
                        CheckforAPIs();

                        
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

                void CheckforAPIs()
                {
                        foreach (var loadedAssembly in AssemblyLoader.loadedAssemblies)
                        { 
                                switch(loadedAssembly.name)
                                {
                                        case "FerramAerospaceResearch":
                                                FAR = true;
                                                break;

                                        case "RemoteTech":
                                                RT = true;
                                                break;
                                
                                }
                        }

                        assemblycheck = true;
                }

                void OnGUI()
                {
                        gui.OnGUI();
                }

                void Update()
                {                       
                        if (FlightGlobals.ActiveVessel != currentVessel)
                        {
                                currentVessel = FlightGlobals.ActiveVessel;
                                Log.Script(LogType.Info, "New Active Vessel is: " + FlightGlobals.ActiveVessel.vesselName);
                                return;
                        }
                }

                void FixedUpdate() {

                }

  
                void onDestroy()
                {
                        mainButton.Destroy();
                       
                        
                }


    }
}
