using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private readonly List<CardModel> _defaultCard = new List<CardModel>()
        {
            new CardModel()
            {
                Guid = Guid.NewGuid(),
                Name = "未分類",
                Total = 0
            },
            new CardModel()
            {
                Guid = Guid.NewGuid(),
                Name = "飲食",
                Total = 0
            }
        };

        [HttpGet]
        [Route("list")]
        public IHttpActionResult List()
        {
            return Ok(_defaultCard);
        }

        [HttpGet]
        [Route("{guid}")]
        public IHttpActionResult Item(Guid guid)
        {
            var existCard = _defaultCard.FirstOrDefault(c => c.Guid == guid);
            if (existCard != null)
            {
                _defaultCard.Remove(existCard);
                return Ok(_defaultCard);
            }

            return BadRequest("查無此張卡片");
        }

        [HttpPost]
        [Route("new/{name}")]
        public IHttpActionResult Add(string name)
        {
            var card = _defaultCard.FirstOrDefault(c => c.Name == name);
            if (card != null)
            {
                return BadRequest("卡片名稱已存在");
            }
            
            card = new CardModel()
            {
                Guid = Guid.NewGuid(),
                Name = name,
                Total = 0
            };
            _defaultCard.Add(card);
            
            return Ok(card);
        }
    }

    public class CardModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
}