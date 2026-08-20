using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.DKMH;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaitapController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public BaitapController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        [HttpPost]
        [Route("bài tập 1")] // hỏi làm sao mà đệ hiện đúng học phần đó đúng học kỳ đó trong năm đó mà các học phần không bị trùng lịch với nhau
        public async Task<IActionResult> Baitap1([FromBody] CreateDKMH dk)
        {
            var check_gv = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == dk.IdGvgdmh);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có giáo viên này", success = false });
            }

            var check_hs = await db.Hocsinhs.FirstOrDefaultAsync(x => x.Id == dk.IdHs);
            if (check_hs == null)
            {
                return Ok(new { msg = "không có hs này", success = false });
            }

            // trùng
            var check_trung = await db.Dkmhs
                .FirstOrDefaultAsync(x => x.IdGvgdmhNavigation.IdMh == check_gv.IdMh && x.IdGvgdmhNavigation.IdNh == check_gv.IdNh && x.IdHs == dk.IdHs);

            if (check_trung != null)
            {
                return Ok(new { msg = "đã đăng ký môn này rồi", success = false });
            }

            var dkmh = new Dkmh
            {
                IdGvgdmh = dk.IdGvgdmh,
                IdHs = dk.IdHs
            };
            db.Add(dkmh);
            await db.SaveChangesAsync();
            return Ok(new { msg = "thêm mới thành công", success = true });
        }

        [HttpGet]
        [Route("bài tập 2")]
        public async Task<IActionResult> Baitap2()
        {
            var dk = await db.Dkmhs
                .Select(x => new
                {
                    x.IdHsNavigation.NameStudent,
                    x.IdGvgdmhNavigation.IdGvgdNavigation.TenGvgd,
                    x.IdHsNavigation.Class.NameClass,
                    x.IdGvgdmhNavigation.IdMhNavigation.NameSub,
                    x.IdGvgdmhNavigation.IdQtNavigation.NameProgress,
                    x.IdGvgdmhNavigation.IdNhNavigation.TenNh

                }).ToListAsync();
            return Ok(dk);
        }


        [HttpPost]
        [Route("bài tập 3")]
        public async Task<IActionResult> Baitap3()
        {
            var nh = await db.Nhs
                .Select(x => new
                {
                    x.Id,
                    x.MaNh,
                    x.TenNh
                }).ToListAsync();

            var newlist = new List<object>();
            foreach (var item in nh)
            {
                var sb = await db.Pcgvgds
                    .Where(x => x.IdNh == item.Id)
                    .Select(x => new
                    {
                        namesub = x.IdMhNavigation.NameSub,
                    })
                    .ToListAsync();

                newlist.Add(new
                {
                    item.MaNh,
                    item.TenNh,
                    total = sb.Count()
                });
            }
            return Ok(newlist);
        }

        [HttpPost]
        [Route("bài tập 4")]
        public async Task<IActionResult> Baitap4()
        {
            var nh = await db.Nhs
                .Select(x => new
                {
                    x.Id,
                    x.MaNh,
                    x.TenNh
                }).ToListAsync();

            var newlist = new List<object>();
            foreach (var item in nh)
            {
                var gv = await db.Pcgvgds
                    .Where(x => x.IdNh == item.Id && x.IdGvgdNavigation.Id == x.IdGvgd)
                    .Select(x => new
                    {
                        x.IdGvgdNavigation.TenGvgd,
                        x.IdMhNavigation.NameSub
                    }).ToListAsync();
                newlist.Add(new
                {
                    item.MaNh,
                    item.TenNh,
                    gv

                });
            }
            return Ok(newlist);
        }

        [HttpPost]
        [Route("bài tập 5")]
        public async Task<IActionResult> Baitap5()
        {
            var nh = await db.Nhs
                .Select(x => new
                {
                    x.Id,
                    x.MaNh,
                    x.TenNh
                }).ToListAsync();

            var newlist = new List<object>();

            foreach (var item in nh)
            {
                var sb = await db.Pcgvgds
                    .Where(x => x.IdNhNavigation.Id == item.Id)
                    .Select(x => new
                    {
                        x.Id,
                        sub = x.IdMhNavigation.NameSub,
                    }).ToListAsync();

                foreach (var item2 in sb)
                {
                    var hs = await db.Dkmhs
                        .Where(x => x.IdGvgdmh == item2.Id)
                        .Select(x => new
                        {
                            x.IdHsNavigation.NameStudent
                        }).ToListAsync();
                    newlist.Add(new
                    {
                        item.MaNh,
                        item.TenNh,
                        item2.sub,
                        total = hs.Count()
                    });
                }
            }
            return Ok(newlist);
        }
    }
}

