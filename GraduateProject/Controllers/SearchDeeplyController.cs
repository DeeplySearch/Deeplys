using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GraduateProject.Models;
using Newtonsoft.Json;
using PagedList;
using PagedList.Mvc;

namespace GraduateProject.Controllers
{
    public class SearchDeeplyController : Controller
    {
        MyDatabaseEntities db = new MyDatabaseEntities();
        // GET: SearchDeeply
        [Authorize]
        public ActionResult SearchAdvance()
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            return View();
        }
        public async Task<ActionResult> SearchResult(SearchResult SearchResult, int? Page_No)
        {
            SearchData data = new SearchData();

            List<SearchData> Result = new List<SearchData>();
            HttpClient client = new HttpClient { BaseAddress = new Uri("http://osamaalsad.pythonanywhere.com/") };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("search/?drop1=" + SearchResult.Category + "&format=json&q=" + SearchResult.Name);
            var responseResult = response.Content.ReadAsStringAsync().Result;
            Result = JsonConvert.DeserializeObject<List<SearchData>>(responseResult);

            int Size_Of_Page = 5;
            int No_Of_Page = (Page_No ?? 1);


            return View(Result.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        /// <summary>
        /// ////////////////////////////
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchInUserData()
        {
            var Categories = new List<Catygory>()
            { new Catygory(){Id=1,Text="api"},
              new Catygory(){Id=2,Text="CV"},
              new Catygory(){Id=3,Text="Online Work"},
              new Catygory(){Id=4,Text="Program SetUp"},
              new Catygory(){Id=5,Text="Learning Website"},
              new Catygory(){Id=6,Text="Online Tool"}
            };
            ViewBag.list = Categories;
            return View();
        }
        public ActionResult SearchResultUserData(SearchResult result, int? Page_No, string sort_order)
        {
            List<SearchData> Result = new List<SearchData>();
            ViewBag.ID = string.IsNullOrEmpty(sort_order) ? "id" : "";
            ViewBag.Name = string.IsNullOrEmpty(sort_order) ? "Name" : "";
            ViewBag.Url = string.IsNullOrEmpty(sort_order) ? "Url" : "";
            ViewBag.Description = string.IsNullOrEmpty(sort_order) ? "Description" : "";

            var res = from item in db.DataInfoes
                      where (item.CategoryId == result.Category && (item.Name.Contains(result.Name) || item.description.Contains(result.Name))) && item.Accepted == true
                      select item;
            switch (sort_order)
            {
                case "id":
                    res = res.OrderBy(d => d.Id);
                    break;
                case "Name":
                    res = res.OrderBy(d => d.Name);
                    break;
                case "Url":
                    res = res.OrderBy(d => d.URL);
                    break;
                case "Description":
                    res = res.OrderBy(d => d.description);
                    break;
                default: // Not: case "Default"
                    res = res.OrderBy(x => x.Id);
                    break;
            }
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);

            return View(res.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public JsonResult getNames(string Name)
        {
            List<AutoTags> auto = new List<AutoTags>();
            //db = new MyDatabaseEntities();

            foreach (var item in db.AutoTags)
            {
                auto.Add(new AutoTags { Id = item.id, Name = item.name });
            }


            return Json(auto.ToList(), System.Web.Mvc.JsonRequestBehavior.AllowGet);

        }

    }
}
