using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AscentProfiler
{

        enum APGCSDecoder
        {
                FLIGHTLOG,
                TELEMETRY
        }

        struct APGCSDataPacket
        {
                internal Guid source;
                internal APGCSDecoder type;
                internal double transitime;
                internal int datacount;
                internal object data;

                internal APGCSDataPacket(Guid source, APGCSDecoder destination, double transitime, int datacount, object data)
                {
                        this.source = source;
                        this.type = destination;
                        this.transitime = transitime;
                        this.datacount = datacount;
                        this.data = data;
                }


        }

        class TelemetryStation : MonoBehaviour
        {
                List<string> FlightLog = new List<string>();
                int flightLogReadCount = 0;



                internal bool Receive(APGCSDataPacket datapacket)
                { 


                        if(datapacket.type == APGCSDecoder.FLIGHTLOG)
                        {
                                


                        }
                        return false;
                }



                void Update()
                {

                }

        }
}
