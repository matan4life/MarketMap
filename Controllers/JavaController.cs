using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace MarketMap.Controllers
{
    [Produces("application/json")]
    [Route("api/Java")]
    public class JavaController : Controller
    {
        public string Outlets()
        {
            using (var db = new Data.ApplicationDbContext())
            {
                var query = db.Outlets.ToList();
                string json = "";
                json = JsonConvert.SerializeObject(query, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                return json;
            }
        }

        public string Outlets(string UserId)
        {
            using (var db = new Data.ApplicationDbContext())
            {
                var query = db.Outlets.Where(o => db.FavouriteOutlets
                    .Where(fo => fo.UserId==UserId).Select(fo => fo.OutletId).Contains(o.Id)
                ).ToList();
                string json = "";
                json = JsonConvert.SerializeObject(query, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                return json;
            }
        }

        public string Outlet(int OutletId)
        {
            using (var db = new Data.ApplicationDbContext())
            {
                var query = db.Outlets.Where(o => o.Id == OutletId).FirstOrDefault();
                string json = "";
                json = JsonConvert.SerializeObject(query, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                return json;
            }
        }

        public string Outlet(string UserId, int OutletId)
        {
            using (var db = new Data.ApplicationDbContext())
            {
                dynamic query = db.FavouriteOutlets.Where(fo => fo.UserId == UserId).Where(fo => fo.OutletId == OutletId).FirstOrDefault();
                if (query is null) query = false;
                else query = true;
                string json = "";
                json = JsonConvert.SerializeObject(query, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                return json;
            }
        }

        //public string Outlet(string UserId, int OutletId, bool type)
        //{
        //    using (var db = new Data.ApplicationDbContext())
        //    {
        //        if (type)
        //        {
        //            var user = db.Users.Where(u => u.Id == UserId).FirstOrDefault();
        //            if (!(user is null))
        //            {
        //                var outlet = db.Outlets.Where(o => o.Id == OutletId).FirstOrDefault();
        //                if (!(user is null))
        //                {
        //                    var fo = new Models.FavouriteOutlet()
        //                    {
                                
        //                    };
        //                }
        //            }
        //        }
        //    }
        //}
    }
}