using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Webmap.Controllers
{ 

    public class HomeController : Controller
    {
        static string NewestFileofDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (directoryInfo == null || !directoryInfo.Exists)
                return null;

            FileInfo[] files = directoryInfo.GetFiles();
            DateTime recentWrite = DateTime.MaxValue;
            FileInfo recentFile = null;

            int iCounter = 0;
            foreach (FileInfo file in files)
            {


                if (file.Name.StartsWith("Syslog"))
                {
                    if (file.LastWriteTime < recentWrite)
                    {
                        recentWrite = file.LastWriteTime;
                        recentFile = file;
                        iCounter++;
                    }

                }
                else
                {
                    Console.WriteLine(file.Name);

                }

            }
            if (iCounter <= 0) { return null; }
            return recentFile.Name;

        }
        RuckusLogEntities6 obj = new RuckusLogEntities6();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            
            
            return View();
        }

        public ActionResult Contact()
        {
           

            return View();
        }
      

        public ActionResult JsonResultGetAllEmployee()

        {
            var results = from e in obj.Device
                          select new
                          {
                              DeviceID = e.DeviceId,
                              DeviceMac = e.DeviceMac,
                              DeviceName = e.DeviceName,
                              GeoLat = e.GeoLat,
                              GeoLong = e.GeoLong
                          };
            return Json(new { data = results }, JsonRequestBehavior.AllowGet );
        }
        public ActionResult Upit(string start, string end, string date1, string date2)
        {

            RuckusLogEntities6 obj = new RuckusLogEntities6();
            DateTime startDate;
            DateTime endDate;
            DateTime startTime;
            DateTime EndTime;
            long DDF = obj.Device.LongCount();
            List<HetmapIndex> lista = new List<HetmapIndex>();
            List<DeviceList> lista1 = new List<DeviceList>();          

            if (date1 == null)
            {
                startDate = Convert.ToDateTime("01/09/2017" + " " + "00:00:00");
                endDate = DateTime.Now;
               
                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }
            }
            else
            {
                startDate = Convert.ToDateTime(date1 + " " + "00:00:00");
                endDate = Convert.ToDateTime(date2 + " " + "23:59:59");
                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }

            }


            var results2 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate)                           
                           group e by e.Device.DeviceId into g                         
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),
                               

                           };

            var results3 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate) && (e.TimeStamp.Value.Hour>= startTime.Hour && e.TimeStamp.Value.Hour <= EndTime.Hour)
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),


                           };
            var results4 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate && e.User.UserMac== "94:F6:65:3E:A3:D0")
                           group e by e.Device.DeviceName into g
                           select new
                           {
                               DeviceName = g.Key,
                               UsersNumLogs = g.Count(),


                           };
            foreach (var a in results3)
            {
                HetmapIndex jerko = new HetmapIndex();
                jerko.DeviceId = a.DeviceId;
                jerko.DeviceNumLogs = a.DeviceNumLogs;            
                lista.Add(jerko);               
                    }
            
          



            var results1 = from dev in obj.Device
                           select new 
                           {
                               DeviceID = dev.DeviceId,
                               DeviceMac = dev.DeviceMac,
                               DeviceName = dev.DeviceName,
                               GeoLat = dev.GeoLat,
                               GeoLong = dev.GeoLong,
                             
                            
                           
                           };
            foreach (var d in results1)
            {
                DeviceList jerko1 = new DeviceList();
                jerko1.DeviceID = d.DeviceID;
                jerko1.DeviceMac = d.DeviceMac;
                jerko1.DeviceName = d.DeviceName;
                jerko1.GeoLat = d.GeoLat;
                jerko1.GeoLong = d.GeoLong;                
                lista1.Add(jerko1);
            }
            foreach(DeviceList dev1 in lista1)
            {
                foreach(HetmapIndex dev2 in lista)
              if ( dev1.DeviceID == dev2.DeviceId)
                    {
                        dev1.NumLogs = dev2.DeviceNumLogs;
                    }
                             
            }

            var test = from p in lista1
                       orderby p.NumLogs
                       select  new
                       {
                           DeviceID = p.DeviceID,
                           DeviceMac = p.DeviceMac,
                           DeviceName = p.DeviceName,
                           GeoLat = p.GeoLat,
                           GeoLong = p.GeoLong,
                           NumLogs = p.NumLogs,
                       };
            var firstFiveArrivals = lista1.OrderBy(i => i.NumLogs).Take(5);
            var lastFiveArrivals = lista1.OrderByDescending(i => i.NumLogs).Take(5);
           
           
            var ZaPoslati = from rez in lista1
                            orderby rez.NumLogs descending
                            select new
                            {
                                DeviceID = rez.DeviceID,
                                DeviceMac = rez.DeviceMac,
                                DeviceName = rez.DeviceName,
                                GeoLat = rez.GeoLat,
                                GeoLong = rez.GeoLong,
                                NumLogs=rez.NumLogs,
                            };
            var ZaPoslati1 = from rez1 in lista1
                            orderby rez1.DeviceName
                            select new
                            {
                                DeviceID = rez1.DeviceID,
                                DeviceMac = rez1.DeviceMac,
                                DeviceName = rez1.DeviceName,
                                GeoLat = rez1.GeoLat,
                                GeoLong = rez1.GeoLong,
                                NumLogs = rez1.NumLogs,
                            };
            var sortiranjeNumlogs = ZaPoslati.OrderBy(i => i.NumLogs);
            var sortiranjeIme = ZaPoslati.OrderBy(i => i.DeviceName);
            return Json(new {data= ZaPoslati, data1= results2 }, JsonRequestBehavior.AllowGet);
          
        }
        public ActionResult Upit1(string start, string end,string date1,string date2)
        {
           
           
            RuckusLogEntities6 obj = new RuckusLogEntities6();
            DateTime startDate;
            DateTime endDate;
            long DDF = obj.Device.LongCount();
            List<HetmapIndex> lista = new List<HetmapIndex>();
            List<DeviceList> lista1 = new List<DeviceList>();
            DateTime startTime;
            DateTime EndTime;
            if (date1 == null)
            {
                startDate = Convert.ToDateTime("01/09/2017" + " " + "00:00:00");
                endDate = DateTime.Now;

                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }
            }
            else
            {
                startDate = Convert.ToDateTime(date1 + " " + "00:00:00");
                endDate = Convert.ToDateTime(date2 + " " + "23:59:59");
                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }

            }



            var results2 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate)
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),


                           };

            var results3 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate) && (e.TimeStamp.Value.Hour >= startTime.Hour && e.TimeStamp.Value.Hour <= EndTime.Hour)
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),


                           };
            var results4 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate && e.User.UserMac == "70:70:0D:63:32:37")
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceName = g.Key,
                               DeviceNumLogs = g.Count(),



                           };
            foreach (var a in results3)
            {
                HetmapIndex jerko = new HetmapIndex();
                jerko.DeviceId = a.DeviceId;
                jerko.DeviceNumLogs = a.DeviceNumLogs;
                lista.Add(jerko);
            }





            var results1 = from dev in obj.Device
                           select new
                           {
                               DeviceID = dev.DeviceId,
                               DeviceMac = dev.DeviceMac,
                               DeviceName = dev.DeviceName,
                               GeoLat = dev.GeoLat,
                               GeoLong = dev.GeoLong,



                           };
            foreach (var d in results1)
            {
                DeviceList jerko1 = new DeviceList();
                jerko1.DeviceID = d.DeviceID;
                jerko1.DeviceMac = d.DeviceMac;
                jerko1.DeviceName = d.DeviceName;
                jerko1.GeoLat = d.GeoLat;
                jerko1.GeoLong = d.GeoLong;
                lista1.Add(jerko1);
            }
            foreach (DeviceList dev1 in lista1)
            {
                foreach (HetmapIndex dev2 in lista)
                    if (dev1.DeviceID == dev2.DeviceId)
                    {
                        dev1.NumLogs = dev2.DeviceNumLogs;
                    }

            }

            var test = from p in lista1
                       orderby p.NumLogs
                       select new
                       {
                           DeviceID = p.DeviceID,
                           DeviceMac = p.DeviceMac,
                           DeviceName = p.DeviceName,
                           GeoLat = p.GeoLat,
                           GeoLong = p.GeoLong,
                           NumLogs = p.NumLogs,
                       };
            var firstFiveArrivals = test.OrderBy(i => i.NumLogs).Take(5);
            var lastFiveArrivals = test.OrderByDescending(i => i.NumLogs).Take(5);
            var ZaPoslati = from rez in lista1
                            select new
                            {
                                DeviceID = rez.DeviceID,
                                DeviceMac = rez.DeviceMac,
                                DeviceName = rez.DeviceName,
                                GeoLat = rez.GeoLat,
                                GeoLong = rez.GeoLong,
                                NumLogs = rez.NumLogs,
                            };
            return Json(new { data = ZaPoslati, data1 = results2 }, JsonRequestBehavior.AllowGet);

        }
          public ActionResult Upit2()
        {
            string putanja = @"\\WS-VENTEX\files\";
            string filename = NewestFileofDirectory(putanja);

            var status = new  
            {
                text = filename,
                j = "DDD",
            };

            return Json(new {data=status }, JsonRequestBehavior.AllowGet);
          
        }
        public ActionResult Upit3(string start, string end, string date1, string date2)
        {

            RuckusLogEntities6 obj = new RuckusLogEntities6();
            DateTime startDate;
            DateTime endDate;
            long DDF = obj.Device.LongCount();
            List<HetmapIndex> lista = new List<HetmapIndex>();
            List<DeviceList> lista1 = new List<DeviceList>();
            DateTime startTime;
            DateTime EndTime;
            if (date1 == null)
            {
                startDate = Convert.ToDateTime("01/09/2017" + " " + "00:00:00");
                endDate = DateTime.Now;

                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }
            }
            else
            {
                startDate = Convert.ToDateTime(date1 + " " + "00:00:00");
                endDate = Convert.ToDateTime(date2 + " " + "23:59:59");
                if (start == null)
                {
                    startTime = Convert.ToDateTime(date1 + " " + "00:00:00");
                    EndTime = Convert.ToDateTime(date2 + " " + "23:59:59");
                }
                else
                {
                    startTime = Convert.ToDateTime(date1 + " " + start);
                    EndTime = Convert.ToDateTime(date2 + " " + end);
                }

            }



            var results2 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate)
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),


                           };

            var results3 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate) && (e.TimeStamp.Value.Hour >= startTime.Hour && e.TimeStamp.Value.Hour <= EndTime.Hour)
                           group e by e.Device.DeviceId into g
                           select new
                           {
                               DeviceId = g.Key,
                               DeviceNumLogs = g.Count(),


                           };
            var results4 = from e in obj.Ruckus_Log
                           where (e.TimeStamp >= startDate && e.TimeStamp <= endDate && e.User.UserMac == "4C:7C:5F:78:52:F5")
                           group e by e.Device.DeviceName into g
                           select new
                           {
                               DeviceName = g.Key,
                               UsersNumLogs = g.Count(),


                           };
            foreach (var a in results3)
            {
                HetmapIndex jerko = new HetmapIndex();
                jerko.DeviceId = a.DeviceId;
                jerko.DeviceNumLogs = a.DeviceNumLogs;
                lista.Add(jerko);
            }





            var results1 = from dev in obj.Device
                           select new
                           {
                               DeviceID = dev.DeviceId,
                               DeviceMac = dev.DeviceMac,
                               DeviceName = dev.DeviceName,
                               GeoLat = dev.GeoLat,
                               GeoLong = dev.GeoLong,



                           };
            foreach (var d in results1)
            {
                DeviceList jerko1 = new DeviceList();
                jerko1.DeviceID = d.DeviceID;
                jerko1.DeviceMac = d.DeviceMac;
                jerko1.DeviceName = d.DeviceName;
                jerko1.GeoLat = d.GeoLat;
                jerko1.GeoLong = d.GeoLong;
                lista1.Add(jerko1);
            }
            foreach (DeviceList dev1 in lista1)
            {
                foreach (HetmapIndex dev2 in lista)
                    if (dev1.DeviceID == dev2.DeviceId)
                    {
                        dev1.NumLogs = dev2.DeviceNumLogs;
                    }

            }

            var test = from p in lista1
                       orderby p.NumLogs
                       select new
                       {
                           DeviceID = p.DeviceID,
                           DeviceMac = p.DeviceMac,
                           DeviceName = p.DeviceName,
                           GeoLat = p.GeoLat,
                           GeoLong = p.GeoLong,
                           NumLogs = p.NumLogs,
                       };
            var firstFiveArrivals = lista1.OrderBy(i => i.NumLogs).Take(5);
            var lastFiveArrivals = lista1.OrderByDescending(i => i.NumLogs).Take(5);


            var ZaPoslati = from rez in lista1
                            orderby rez.DeviceName 
                            select new
                            {
                                DeviceID = rez.DeviceID,
                                DeviceMac = rez.DeviceMac,
                                DeviceName = rez.DeviceName,
                                GeoLat = rez.GeoLat,
                                GeoLong = rez.GeoLong,
                                NumLogs = rez.NumLogs,
                            };
            var ZaPoslati1 = from rez1 in lista1
                             orderby rez1.DeviceName
                             select new
                             {
                                 DeviceID = rez1.DeviceID,
                                 DeviceMac = rez1.DeviceMac,
                                 DeviceName = rez1.DeviceName,
                                 GeoLat = rez1.GeoLat,
                                 GeoLong = rez1.GeoLong,
                                 NumLogs = rez1.NumLogs,
                                
                             };
            var sortiranjeNumlogs = ZaPoslati.OrderBy(i => i.NumLogs);
            var sortiranjeIme = ZaPoslati.OrderBy(i => i.DeviceName);
            return Json(new { data = ZaPoslati, data1 = results2 }, JsonRequestBehavior.AllowGet);

        }

        private class HetmapIndex
        {
            public int DeviceId { get; set; }
            public int DeviceNumLogs { get; set; }
        }

        private class DeviceList
        {
            public int DeviceID { get; set; }
            public string DeviceMac { get; set; }
            public string DeviceName { get; set; }
            public int NumLogs { get; set; }
            public double? GeoLat { get; set; }
            public double? GeoLong { get; set; }
        }
    }
}