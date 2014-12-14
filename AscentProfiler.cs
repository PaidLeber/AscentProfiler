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
                internal static string version = "0.01";
                //Loaded Assemblies list
                internal static List<RegisteredAddons> listRegisteredAddons = new List<RegisteredAddons>();

                internal static Vessel currentVessel = null;
                static string AscentProfilerDir;
                internal static string sequenceDir;
                internal static string telemetryDir;

                GUIAscentProfiler guiAscentProfiler = null;
                GUITelemetry guiTelemetry = null;

                bool mainWindowEnabled = false;
                bool telemetryWindowEnabled = false;
                IButton mainButton;
                IButton telemetryButton;


                internal static TelemetryReceiver telemetryReceiver;

                void Awake()
                {
                        Log.Level(LogType.Info, "Is Awake!");
                }


                void Start()
                {
                        CheckforAPIs();

                        
                        guiAscentProfiler = new GUIAscentProfiler();
                        guiTelemetry = new GUITelemetry();
                        telemetryReceiver = new TelemetryReceiver();

                        //telemetryReceiver = gameObject.AddComponent<TelemetryReceiver>();
                        
                        

                        if (ToolbarManager.ToolbarAvailable)
                        {
                                mainWindowEnabled = false;
                                mainButton = ToolbarManager.Instance.add("AscentProfiler", "AscentProfiler");
                                mainButton.TexturePath = "AscentProfiler/Textures/icon_blizzy";
                                mainButton.ToolTip = "Open Ascent Profiler";
                                mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
                                mainButton.OnClick += (e) =>
                                {
                                        guiAscentProfiler.ChangeState(!mainWindowEnabled);
                                        mainWindowEnabled = !mainWindowEnabled;
                                };

                                //Telemetry Station button
                                telemetryWindowEnabled = false;
                                telemetryButton = ToolbarManager.Instance.add("AscentProfiler", "TelemetryGraph");
                                telemetryButton.TexturePath = "AscentProfiler/Textures/telemetry_received_blizzy";
                                telemetryButton.ToolTip = "Open Telemetry Graph";
                                telemetryButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
                                telemetryButton.OnClick += (e) =>
                                {
                                        guiTelemetry.ChangeState(!telemetryWindowEnabled);
                                        telemetryWindowEnabled = !telemetryWindowEnabled;
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

                        sequenceDir     = AscentProfilerDir + "/Sequences";
                        telemetryDir   = AscentProfilerDir + "/Telemetry";
                        
                        if (!Directory.Exists(AscentProfilerDir))
                        {
                                Directory.CreateDirectory(AscentProfilerDir);
                        }

                        if (!Directory.Exists(sequenceDir))
                        {
                                Directory.CreateDirectory(sequenceDir);
                        }

                        if (!Directory.Exists(telemetryDir))
                        {
                                Directory.CreateDirectory(telemetryDir);
                        }

                        Log.Level(LogType.Info, "User Directory: " + AscentProfilerDir);
                        Log.Level(LogType.Info, "Sequence Directory: " + sequenceDir);
                        Log.Level(LogType.Info, "Telemetry Directory: " + telemetryDir); 
                }

                void CheckforAPIs()
                {
                        foreach (var loadedAssembly in AssemblyLoader.loadedAssemblies)
                        {
                                foreach (RegisteredAddons addon in (RegisteredAddons[])Enum.GetValues(typeof(RegisteredAddons)))
                                {
                                        if (loadedAssembly.name == addon.ToString())
                                        {
                                                listRegisteredAddons.Add(addon);
                                        }
                                }
     
                        }

                }

                void OnGUI()
                {
                        guiAscentProfiler.OnGUI();
                        guiTelemetry.OnGUI();
                }

                void Update()
                {
                        telemetryReceiver.Update();

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
