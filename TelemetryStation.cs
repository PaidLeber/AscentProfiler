using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscentProfiler
{

        enum APGCSDecoder
        {
                FLIGHT,
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
