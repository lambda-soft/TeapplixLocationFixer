using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace TeapplixLocationFixer
{
    class ProductItem
    {
        public string Location;
        public string ItemTitle;
        public string Upc;
    }
    class Products
    {
        public string ItemName;
        public ProductItem Product;
    }
    class Quantities
    {
        public string ItemName;
        public int Quantity;
        public string PostType;
        public string PostDate;
        public string PostComment;

        public Quantities()
        {
            PostType = "in-stock";
            PostDate = DateTime.Now.ToShortDateString();
            PostComment = "Updated via API";
        }
    }

    class Program
    {
        const string BASEURI = "https://api.teapplix.com/api2";

        static string TEAPPLIXAPIKEY = ConfigurationManager.AppSettings["TEAPPLIXAPIKEY"];
        

        static List<Quantities> quantityList = new List<Quantities>();
        static List<Products> prodList = new List<Products>();

        static void Main(string[] args)
        {
            GetProductList();

            UpdateLocations();

            UpdateQuantities();

            Console.ReadLine();
        }
        static async void UpdateQuantities()
        {
            var client = new HttpClient();
            var uri = new Uri(BASEURI + @"/ProductQuantity");

            client.DefaultRequestHeaders.Add("APIToken", TEAPPLIXAPIKEY);

            var qtyArray = quantityList.ToArray();

            var body = new
            {
                Quantities = qtyArray
            };

            string jsonBody = JsonConvert.SerializeObject(body);
            StringContent bodyContent = new StringContent(jsonBody);

            bodyContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(uri, bodyContent);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
        }

        static async void UpdateLocations()
        {
            var client = new HttpClient();
            var uri = new Uri(BASEURI + @"/Product");

            client.DefaultRequestHeaders.Add("APIToken", TEAPPLIXAPIKEY);

            var prodArray = prodList.ToArray();

            var body = new
            {
                Products = prodArray
            };

            string jsonBody = JsonConvert.SerializeObject(body);
            StringContent bodyContent = new StringContent(jsonBody);
            bodyContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PutAsync(uri, bodyContent);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
        }
        
        static void GetProductList()
        {
            var filePath = "D:/WirelessessentialsTeaFix.csv";
            using (var sr = new StreamReader(filePath))
            {
                string line;
                string[] row;

                while((line = sr.ReadLine()) != null)
                {
                    row = line.Split(',');
                    prodList.Add(new Products
                    {
                        ItemName = row[0],
                        Product = new ProductItem
                        {
                            Location = row[1],
                            ItemTitle = row[2],
                            Upc = row[3]
                        }
                    });
                    quantityList.Add(new Quantities
                    {
                        ItemName = row[0],
                        Quantity = Int32.Parse(row[4])
                    });
                }
            }
        }
    }
}
