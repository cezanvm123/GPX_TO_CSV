using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Converter
{

    class TrackPoint
    {
        public String time;

        public decimal lat;
        public decimal lon;
        public decimal elevation;

        public int heartRate;
        public float temp;


        public TrackPoint()
        {

        }
        public TrackPoint(TrackPoint point)
        {
            time = point.time;
            lat = point.lat;
            lon = point.lon;
            elevation = point.elevation;
            heartRate = point.heartRate;
            temp = point.temp;
        }

        public void clearPoint()
        {
            time = "";
            lat = 0;
            lon = 0;
            elevation = 0;
            heartRate = 0;
            temp = 0;
        }
    }

    enum LineType
    {
        TRKPT,
        ELE, 
        TIME,
        TEMP,
        HR,
        END, 
        DEFAULT
    }

    class Program
    {

        ArrayList list = new ArrayList();
        TrackPoint currentPoint = new TrackPoint();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Program p = new Program();
            p.gatherData(@"../../../PersonalData/InputFile.gpx");

            p.writeToFile(@"../../../PersonalData/output.csv");

        }
        

        void gatherData(String path)
        {
           // path = Path.Combine(Environment., path);

            string[] text = File.ReadAllLines(path);

            foreach (var line in  text)
            {
                parseLine(line, determineLineType(line));
            }

            testData();
        }

        void testData()
        {
            for (int i = 0; i < list.Count; i++)
            {
                bool valid = true;
                TrackPoint point = (TrackPoint) list[i];

                if(point.time == "" )
                {
                    valid = false;
                }
                if(point.lat == 0)
                {
                    valid = false;
                }
                if(point.lon == 0)
                {
                    valid = false;
                }
                if(point.elevation == 0) // not false persee but needs checking for now to make sure the elevation is actually 0
                {
                    valid = false;
                }
                if(point.temp == 0)
                {
                    valid = false;
                }
                if(point.heartRate == 0) // R.I.P
                {
                    valid = false;
                }

                if(valid == false)
                {
                    Console.WriteLine("Found measurement that might not be valid at index: {0}", i);
                }
            }
        }

        void parseLine(String line, LineType type)
        { 
            if(type == LineType.TRKPT)
            {
                string[] split = line.Split('"');
                decimal lat = 0;
                decimal lon = 0;

                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains("lat"))
                    {
                        lat = decimal.Parse(split[i + 1]);
                    }
                    else if(split[i].Contains("lon"))
                    {
                        lon = decimal.Parse(split[i + 1]);
                    }
                }

                currentPoint.lon = lon;
                currentPoint.lat = lat;
            }
            else if(type == LineType.ELE)
            {
                decimal ele = decimal.Parse(line.Replace("<ele>", "").Replace("</ele>", ""));
                currentPoint.elevation = ele;
            }
            else if(type == LineType.TIME)
            {
                currentPoint.time = line.Replace("<time>", "").Replace("</time>", "").Replace("T", " ").Replace("Z"," ").Trim();
            }
            else if(type == LineType.TEMP)
            {
                currentPoint.temp = float.Parse(line.Replace("<ns3:atemp>", "").Replace("</ns3:atemp>", ""));
            }
            else if(type == LineType.HR)
            {
                currentPoint.heartRate = int.Parse(line.Replace("<ns3:hr>", "").Replace("</ns3:hr>", ""));
            }
            else if(type == LineType.END)
            {

                list.Add(new TrackPoint(currentPoint));
                currentPoint.clearPoint();
            }
        }


        LineType determineLineType(String line)
        {
            line = line.Trim();
            if(line.Contains("trkpt"))
            {
                if (line == "</trkpt>")
                    return LineType.END;
                else
                    return LineType.TRKPT;
            }
            else if(line.Contains("ele"))
            {
                return LineType.ELE;
            }
            else if(line.Contains("time"))
            {
                return LineType.TIME;
            }
            else if(line.Contains("atemp"))
            {
                return LineType.TEMP;
            }
            else if(line.Contains("hr"))
            {
                return LineType.HR;
            }
            else
            {
                return LineType.DEFAULT;
            }
        }


        void writeToFile(string fileName)
        {

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName))
            {
                String header = "time,latitude,longitude,altitude,temperature,heart rate";
                file.WriteLine(header);
                foreach (TrackPoint point in list)
                {
                    String line = point.time;
                    line += ",";
                    line += point.lat.ToString();
                    line += ",";
                    line += point.lon.ToString();
                    line += ",";
                    line += point.elevation.ToString();
                    line += ",";
                    line += point.temp.ToString();
                    line += ",";
                    line += point.heartRate.ToString();
                    file.WriteLine(line);
                }
            }
        }
    }
}
