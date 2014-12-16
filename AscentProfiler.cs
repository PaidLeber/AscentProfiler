using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AscentProfiler
{

        [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
        public class AscentProfilerVAB : AscentProfiler
        {
                
                
        }

        [KSPAddon(KSPAddon.Startup.Flight, false)]
        public class AscentProfilerFlight: AscentProfiler
        {
                GUIAscentProfiler guiAscentProfiler = null;
                GUITelemetry guiTelemetry = null;

                IButton mainButton;
                IButton telemetryButton;

                bool mainWindowEnabled = false;
                bool telemetryWindowEnabled = false;

                internal static TelemetryReceiver telemetryReceiver;

                internal static Vessel currentVessel = null;

                void Start()
                {
                        guiAscentProfiler = new GUIAscentProfiler();
                        guiTelemetry = new GUITelemetry();
                        telemetryReceiver = new TelemetryReceiver();


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

                void OnGUI()
                {
                        guiAscentProfiler.OnGUI();
                        guiTelemetry.OnGUI();
                }

                void onDestroy()
                {
                        mainButton.Destroy();
                        telemetryButton.Destroy();

                }

        }
        
        public class AscentProfiler : MonoBehaviour
        {
                internal static string version = "0.01";
                //Loaded Assemblies list
                internal static List<RegisteredAddons> listRegisteredAddons = new List<RegisteredAddons>();

                internal static string AscentProfilerDir;
                internal static string sequenceDir;
                internal static string sequenceLoadPath = "";
                internal static string telemetryDir;
                internal static string sequenceExt = ".seq";
                

                void Awake()
                {
                        Log.Level(LogType.Info, "Is Awake!");
                }


                void Start()
                {
                        CheckforAPIs();
                       
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

                        sequenceLoadPath = sequenceDir;

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



    }





}
