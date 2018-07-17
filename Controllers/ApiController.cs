using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MarketMap.Data;
using MarketMap.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace MarketMap.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly IHostingEnvironment hostingEnv;
        RoleManager<IdentityRole> _roleManager;
        UserManager<ApplicationUser> _userManager;

        public ApiController(IHostingEnvironment hostingEnv, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.hostingEnv = hostingEnv;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Route("index")]
        public IActionResult Index()
        {
            var db = new ApplicationDbContext();

            ViewData["categories"] = db.Categories
                .Include(x => x.Color)
                .ToList();

            return View();
        }

        [HttpGet]
        [Route("index-data")]
        public string IndexData()
        {
            var db = new ApplicationDbContext();

            var query = db.Outlets
                .Include(x => x.Points)
                .Include(x => x.OutletCategories)
                    .ThenInclude(y => y.Category)
                    .ThenInclude(z => z.Color)
                .ToList();

            return JsonConvert.SerializeObject(query, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [Route("search")]
        public IActionResult Search()
        {
            return View();
        }

        [Route("filter")]
        public IActionResult Filter()
        {
            return View();
        }
        [Route("add-comment")]
        public IActionResult AddComment()
        {
            return View();
        }
        [Route("manage-categories")]
        public IActionResult ManageCategories()
        {
            return View();
        }

        [Route("create-category")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("get-category-list")]
        public JsonResult GetCategoryList(string category)
        {
            if (category == null) category = "";

            var list = new List<dynamic>();
            Regex r = new Regex(category, RegexOptions.IgnoreCase);

            using (var db = new ApplicationDbContext())
            {
                foreach (Category c in db.Categories.Include(c => c.Color))
                {
                    if (r.IsMatch(c.Name))
                        list.Add(new { c.Name, c.Color.R, c.Color.G, c.Color.B, c.Color.A, c.ColorName });
                }
            }

            return new JsonResult(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddCategory(string color, string category)
        {
            List<string> rgb = new List<string>
            {
                color.Substring(1, 2),
                color.Substring(3, 2),
                color.Substring(5, 2)
            };
            Models.Category Category = new Category();
            Category.Color.A = 1;
            Category.Color.R = (Byte)HexToInt(rgb[0]);
            Category.Color.G = (Byte)HexToInt(rgb[1]);
            Category.Color.B = (Byte)HexToInt(rgb[2]);
            Category.Name = category;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Categories.Add(Category);
                db.SaveChanges();
            }
        }

        private int HexToInt(string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        [Route("get-icon-url")]
        public string GetIconUrl()
        {
            //return @"https://httpsimage.com/v2/25319ce4-f21d-464e-a74a-f7a429c583fc.png";
            return @"https://httpsimage.com/v2/40c60b87-1f67-4fe6-a7e0-d577ef94f5fe.png";
        }

        [Route("add-outlet")]
        public IActionResult AddOutlet()
        {
            var db = new ApplicationDbContext();

            var categories = db.Categories.ToList();

            ViewData["categories"] = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("add-outlet")]
        public IActionResult AddOutlet(string name, string address, string hours, List<string> categories, string vertices)
        {
            var db = new ApplicationDbContext();
            var points = new List<Point>();
            JArray json = JArray.Parse(vertices);

            foreach (JToken obj in json)
            {
                JObject vertex = JObject.Parse(obj.ToString());
                double lat = vertex.Value<double>("lat");
                double lng = vertex.Value<double>("lng");
                int order  = vertex.Value<int>("order");
                points.Add(new Point { Order = order, Latitude = lat, Longtitude = lng });
            }

            var o = new Outlet
            {
                Name = name,
                Address = address,
                WorkingHours = hours,
                Points = points,
                Rating = 0
            };

            var oc = new List<OutletCategory>();
            db.Outlets.Add(o);

            foreach (string categoryName in categories)
            {
                db.OutletCategories.Add(new OutletCategory
                {
                    OutletId = o.Id,
                    CategoryName = categoryName
                });
            }

            db.SaveChanges();

            return Redirect(Url.Action("ManageOutlets", "Api"));
        }

        [Route("remove-outlet")]
        public void RemoveOutlet(int id)
        {
            var db = new ApplicationDbContext();

            var o = db.Outlets.FirstOrDefault(x => x.Id == id);

            if (o != null) db.Outlets.Remove(o);

            db.SaveChanges();
        }

        private async Task<IList<ApplicationUser>> GetAllManagers()
        {
            return await _userManager.GetUsersInRoleAsync("Manager");
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        [Route("manage-outlets")]
        [Authorize]
        public async Task<IActionResult> ManageOutlets()
        {
            var db = new ApplicationDbContext();

            var query = db.Outlets
                .Include(x => x.Points)
                .Include(x => x.OutletCategories)
                    .ThenInclude(y => y.Category)
                    .ThenInclude(z => z.Color)
                .ToList();

            ViewData["outlets"] = query;
            var managers = await GetAllManagers();
            var user = await GetCurrentUser();
            foreach (var u in managers)
            {
                if (u.Id.Equals(user.Id))
                {
                    ViewData["isman"] = true;
                    return View();
                }
            }
            ViewData["isman"] = false;
            return View();
        }

        [Route("db-debug")]
        public IActionResult DbDebug()
        {
            var db = new ApplicationDbContext();

            ViewData["categories"] = db.Categories.Include(c => c.Color).ToList();
                
            return View();  
        }
    }
}