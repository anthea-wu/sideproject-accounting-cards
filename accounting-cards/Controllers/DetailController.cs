using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Newtonsoft.Json;

namespace accounting_cards.Controllers
{
    [RoutePrefix("api/detail")]
    public class DetailController : ApiController
    {
        private static readonly string _jsonPath = Properties.Settings.Default.FirebaseJsonPath;
        static readonly string _jsonStr = File.ReadAllText(_jsonPath);
        private static readonly FirestoreClientBuilder _builder = new FirestoreClientBuilder(){JsonCredentials = _jsonStr};
        private readonly FirestoreDb _db = FirestoreDb.Create("accounting-cards", _builder.Build());
        
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
        [Route("{userId}/{cardId}")]
        public async Task<IHttpActionResult> List(string userId, string cardId)
        {
            var results = await _db
                .Collection("users").Document(userId)
                .Collection("cards").Document(cardId)
                .Collection("details").GetSnapshotAsync();
            
            foreach (var result in results)
            {
                var card = result.ConvertTo<UserCard>();
                
            }
            
            return Ok();
        }
        
        [HttpPost]
        [Route("item")]
        public IHttpActionResult Add(Detail newDetail)
        {
            var cards = JsonConvert.DeserializeObject<List<Card>>(_cache["cards"].ToString());
            var card = cards.FirstOrDefault(c => c.Guid == newDetail.CardGuid);
            if (card == null)
            {
                return BadRequest("卡片分類不存在");
            }
            
            newDetail.Guid = Guid.NewGuid();

            if (card.Name == "未分類")
            {
                _defaultDetail.Add(newDetail);
                return Ok(_defaultDetail);
            }
            else
            {
                _foodDetails.Add(newDetail);
                return Ok(_foodDetails);
            }
        }

        [HttpDelete]
        [Route("item/{guid}")]
        public IHttpActionResult Delete(Guid guid)
        {
            var defaultDetail = _defaultDetail.FirstOrDefault(d => d.Guid == guid);
            var foodDetail = _foodDetails.FirstOrDefault(d => d.Guid == guid);

            if (defaultDetail != null)
            {
                _defaultDetail.Remove(defaultDetail);
                return Ok(_defaultDetail);
            }

            if (_foodDetails != null)
            {
                _foodDetails.Remove(foodDetail);
                return Ok(_foodDetails);
            }
            
            return BadRequest("查無此筆明細");
        }

        [HttpPut]
        [Route("item")]
        public IHttpActionResult Update(Detail detail)
        {
            var defaultDetail = _defaultDetail.FirstOrDefault(d => d.Guid == detail.Guid);
            var foodDetail = _foodDetails.FirstOrDefault(d => d.Guid == detail.Guid);
            
            if (defaultDetail != null)
            {
                if (defaultDetail.CardGuid != detail.CardGuid)
                {
                    _defaultDetail.Remove(defaultDetail);
                    _foodDetails.Add(new Detail
                    {
                        Name = detail.Name,
                        Count = detail.Count,
                        Date = detail.Date,
                        CardGuid = detail.CardGuid
                    });
                }
                else
                {
                    defaultDetail.Name = detail.Name;
                    defaultDetail.Count = detail.Count;
                    defaultDetail.Date = detail.Date;
                    defaultDetail.CardGuid = detail.CardGuid;
                }

                return Ok(_defaultDetail);
            }

            if (_foodDetails != null)
            {
                if (foodDetail.CardGuid != detail.CardGuid)
                {
                    _foodDetails.Remove(foodDetail);
                    _defaultDetail.Add(new Detail()
                    {
                        Name = detail.Name,
                        Count = detail.Count,
                        Date = detail.Date,
                        CardGuid = detail.CardGuid
                    });
                }
                else
                {
                    foodDetail.Name = detail.Name;
                    foodDetail.Count = detail.Count;
                    foodDetail.Date = detail.Date;
                    foodDetail.CardGuid = detail.CardGuid;
                }

                return Ok(_foodDetails);
            }
            
            return BadRequest("查無此筆明細");
        }
    }

    public class DetailList
    {
        public Guid CardGuid { get; set; }
        public List<Detail> Details { get; set; }
    }

    public class Detail
    {
        /// <summary> 紀錄卡片類別的唯一碼 </summary>
        public Guid CardGuid { get; set; }
        
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}