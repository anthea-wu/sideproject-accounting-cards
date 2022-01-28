using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace accounting_cards.Controllers
{
    [RoutePrefix(("api/user"))]
    public class UserController : ApiController
    {
        private readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        
        [HttpGet]
        [Route("session/{id}")]
        public async Task<IHttpActionResult> Session(string id)
        {
            var jsonString = File.ReadAllText(_jsonPath);
            var builder = new FirestoreClientBuilder {JsonCredentials = jsonString};
            var db = await FirestoreDb.CreateAsync("accounting-cards", await builder.BuildAsync());

            var result = await db.Collection("users").Document(id).GetSnapshotAsync();
            if (!result.Exists)
            {
                return BadRequest($"找不到ID為 {id} 的使用者");
            }

            var user = result.ConvertTo<User>();
            return Ok(user);
        }

        [HttpPost]
        [Route("new")]
        public async Task<IHttpActionResult> New()
        {
            var jsonString = File.ReadAllText(_jsonPath);
            var builder = new FirestoreClientBuilder {JsonCredentials = jsonString};
            var db = FirestoreDb.Create("accounting-cards", builder.Build());

            var document = db.Collection("users").Document();
            var newUser = new User
            {
                CreateTime = DateTimeOffset.Now,
                Id = document.Id
            };
            await document.SetAsync(newUser);
            
            var cards = db.Collection("users").Document(document.Id).Collection("cards").Document();
            var defaultCard = new Card()
            {
                Id = cards.Id,
                Name = "未分類",
                Total = 0,
                UserId = newUser.Id,
                CreateTime = DateTimeOffset.Now
            };
            await cards.SetAsync(defaultCard);
            
            return Ok(newUser);
        }
    }
}