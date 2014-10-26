using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{
        class TelemetryStation
        {
                List<string> FlightLog = new List<string>();




                void Receive(APGCSDataPacket datapacket)
                { 



                        if(datapacket.type == APGCSDecoder.FLIGHT)
                        {
                        
                        }
                
                }



        }
}
