using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/detail")]
    public class DetailController : ApiController
    {
        private static readonly List<Detail> _defaultDetail = new List<Detail>()
        {
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "範例",
                Count = 0,
                Date = new DateTimeOffset(2022, 1, 4, 08, 45, 00, TimeSpan.Zero)
            },
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "範例",
                Count = 100,
                Date = new DateTimeOffset(2022, 1, 4, 08, 53, 00, TimeSpan.Zero)
            },
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "未分類",
                Count = 50,
                Date = new DateTimeOffset(new DateTime(2022, 1, 5, 12, 05, 30))
            }
        };

        private static readonly List<Detail> _foodDetails = new List<Detail>()
        {
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "範例",
                Count = 0,
                Date = new DateTimeOffset(2022, 1, 4, 08, 45, 00, TimeSpan.Zero)
            },
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "範例",
                Count = 100,
                Date = new DateTimeOffset(2022, 1, 4, 08, 53, 00, TimeSpan.Zero)
            },
            new Detail()
            {
                Guid = Guid.NewGuid(),
                Name = "飲食",
                Count = 50,
                Date = new DateTimeOffset(new DateTime(2022, 1, 5, 12, 05, 30))
            }
        };

        private readonly MemoryCache _cache = MemoryCache.Default;

        [HttpGet]
        [Route("list/{guid}")]
        public IHttpActionResult List(Guid guid)
        {
            var cards = JsonConvert.DeserializeObject<List<Card>>(_cache["cards"].ToString());
            var card = cards.FirstOrDefault(c => c.Guid == guid);
            if (card == null)
            {
                return BadRequest("卡片分類不存在");
            }
            
            if (card.Name == "未分類")
            {
                return Ok(_defaultDetail);
            }
            else
            {
                return Ok(_foodDetails);
            }
        }
    }

    internal class Detail
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}