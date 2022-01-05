using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/card")]
    public class CardController : ApiController
    {
        private static readonly List<Card> _defaultCard = new List<Card>()
        {
            new Card()
            {
                Guid = Guid.NewGuid(),
                Name = "未分類",
                Total = 0
            },
            new Card()
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
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = new TimeSpan(0, 5, 0)
            };

            var serializeCards = JsonConvert.SerializeObject(_defaultCard);
            MemoryCache.Default.Set("cards", serializeCards, policy);
            
            return Ok(_defaultCard);
        }

        [HttpGet]
        [Route("{guid}")]
        public IHttpActionResult Item(Guid guid)
        {
            var existCard = _defaultCard.FirstOrDefault(c => c.Guid == guid);
            if (existCard == null)
            {
                return BadRequest("查無此張卡片");
            }

            return Ok(existCard);
        }

        [HttpPost]
        public IHttpActionResult Add(Card newCard)
        {
            var card = _defaultCard.FirstOrDefault(c => c.Name == newCard.Name);
            if (card != null)
            {
                return BadRequest("卡片名稱已存在");
            }
            
            card = new Card()
            {
                Guid = Guid.NewGuid(),
                Name = newCard.Name,
                Total = 0
            };
            _defaultCard.Add(card);
            
            return Ok(_defaultCard);
        }

        [HttpDelete]
        [Route("{guid}")]
        public IHttpActionResult Delete(Guid guid)
        {
            var existCard = _defaultCard.FirstOrDefault(c => c.Guid == guid);
            if (existCard == null)
            {
                return BadRequest("卡片不存在");
            }

            _defaultCard.Remove(existCard);
            return Ok(_defaultCard);
        }

        [HttpPut]
        public IHttpActionResult Update(Card card)
        {
            var existCard = _defaultCard.FirstOrDefault(c => c.Guid == card.Guid);
            if (existCard == null)
            {
                return BadRequest("查無此張卡片");
            }

            existCard.Name = card.Name;
            return Ok(_defaultCard);
        }
    }

    public class Card
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
}