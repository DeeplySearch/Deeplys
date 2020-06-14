using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraduateProject.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduateProject.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PagedList;

namespace GraduateProject.Controllers.Tests
{
    [TestClass()]
    public class SearchDeeplyControllerTests
    {
        MyDatabaseEntities db = new MyDatabaseEntities();
        [TestMethod()]
        public async Task SearchAdvanceAsync()
        {
            SearchData data = new SearchData();
            SearchResult searchResult = new SearchResult();
            searchResult.Category = 1;
            searchResult.Name = "k";

            List<SearchData> Result = new List<SearchData>();
            HttpClient client = new HttpClient { BaseAddress = new Uri("http://osamaalsad.pythonanywhere.com/") };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("search/?drop1=" + searchResult.Category + "&format=json&q=" + searchResult.Name);
            var responseResult = response.Content.ReadAsStringAsync().Result;
            Result = JsonConvert.DeserializeObject<List<SearchData>>(responseResult);

            int Page_No = 10;
            int Size_Of_Page = 5;

            var result = Result.ToPagedList(Page_No, Size_Of_Page);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void SearchInUserData()
        {

            SearchResult result = new SearchResult();
            result.Category = 1;
            result.Name = "i";
            string sort_order = "id";

            List<SearchData> Result = new List<SearchData>();

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
            int Page_No = 20;

            Console.WriteLine(res.FirstOrDefault().Name);
            Console.WriteLine(res.FirstOrDefault().URL);


            Assert.IsNotNull(res.ToPagedList(Page_No, Size_Of_Page));
        }

        [TestMethod()]
        public void getNames()
        {
            List<AutoTags> auto = new List<AutoTags>();
            //db = new MyDatabaseEntities();

            foreach (var item in db.AutoTags)
            {
                auto.Add(new AutoTags { Id = item.id, Name = item.name });
            }


             Assert.IsNotNull(auto.ToList());
        }
    }
}