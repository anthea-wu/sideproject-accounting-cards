using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using accounting_cards.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using WriteResult = Google.Cloud.Firestore.WriteResult;

namespace accounting_cards.Controllers
{
    [RoutePrefix(("api/user"))]
    public class UserController : ApiController
    {
        private readonly ICardRepository _cardRepo;
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo, ICardRepository cardRepo)
        {
            _userRepo = userRepo;
            _cardRepo = cardRepo;
        }

        [HttpGet]
        [Route("session/{id}")]
        public async Task<IHttpActionResult> Session(string id)
        {
            var result = await _userRepo.GetUser(id);
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
            var user = _userRepo.CreateUser();
            var newUser = new User
            {
                CreateTime = DateTimeOffset.Now,
                Id = user.Id
            };
            await _userRepo.UpdateUser(user, newUser);

            var cards = _cardRepo.CreateCard(user.Id);
            var defaultCard = new Card()
            {
                Id = cards.Id,
                Name = "未分類",
                Total = 0,
                UserId = newUser.Id,
                CreateTime = DateTimeOffset.Now
            };
            await _cardRepo.UpdateCard(defaultCard);
            
            return Ok(newUser);
        }

    }
}