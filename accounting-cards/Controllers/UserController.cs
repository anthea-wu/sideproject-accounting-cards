using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace accounting_cards.Controllers
{
    [RoutePrefix(("api/user"))]
    public class UserController : ApiController
    {
        private readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        
        [HttpGet]
        [Route("session/{account}")]
        public async Task<IHttpActionResult> Session(string account)
        {
            var jsonString = File.ReadAllText(_jsonPath);
            var builder = new FirestoreClientBuilder {JsonCredentials = jsonString};
            var db = await FirestoreDb.CreateAsync("accounting-cards", await builder.BuildAsync());

            var result = await db.Collection("users")
                .WhereEqualTo("account", account).GetSnapshotAsync();
            if (result.Documents.Count != 1)
            {
                return BadRequest("使用者帳號不正確");
            }
            
            var user = result.Documents[0].ToDictionary();
            var userInfo = new UserInfo()
            {
                Name = user["name"].ToString(),
                UserId = result.Documents[0].Id,
            };

            return Ok(userInfo);
        }
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}