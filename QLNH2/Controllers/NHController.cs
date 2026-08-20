using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.NH;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NHController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public NHController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpGet]
        [Route("get-all-NH")]
        public async Task<IActionResult> GetAllNH()
        {
            var nh = await db.Nhs
                .Select(x => new
                {
                    x.MaNh,
                    x.TenNh
                }).ToListAsync();

            return Ok(nh);
        }

        [HttpPost]
        [Route("create-NH")]
        public async Task<IActionResult> CreateNH([FromBody] CreateNH nh)
        {
            var check_ma = await db.Nhs.FirstOrDefaultAsync(x => x.MaNh == nh.MaNh);
            if (check_ma != null)
            {
                return Ok(new { msg = "đã có mã NH này rồi", success = false });        
            }

            var check_name = await db.Nhs.FirstOrDefaultAsync(x => x.TenNh.ToLower().Trim() == nh.TenNh.ToLower().Trim());
            if(check_name != null)
            {
                return Ok(new { msg = "đã có năm học này rồi", success = false });
            }

            var createNH = new Nh
            {
                MaNh = nh.MaNh,
                TenNh = nh.TenNh,
                TimeCre = unixTimestamp
            };
            await db.AddAsync(createNH);
            await db.SaveChangesAsync();
            return Ok(new {msg="thêm thành  công", success = true});
        }

        [HttpPut]
        [Route("update-NH")]
        public async Task<IActionResult> UpdateNH(int id,[FromBody] UpdateNH nh)
        {
            var check_id = await db.Nhs.FirstOrDefaultAsync(x => x.Id == id);
            if(check_id == null)
            {
                return Ok(new { msg = "không có id này ", success = false });
            }

            var check_ma = await db.Nhs.FirstOrDefaultAsync(x => (x.MaNh.ToLower().Trim() == nh.MaNh.ToLower().Trim()) && x.Id != id);
            if (check_ma != null)
            {
                return Ok(new { msg = "đã có mã NH này rồi", success = false });
            }

            var check_name = await db.Nhs.FirstOrDefaultAsync(x => x.TenNh.ToLower().Trim() == nh.TenNh.ToLower().Trim() && x.Id != id);
            if (check_name != null)
            {
                return Ok(new { msg = "đã có năm học này rồi", success = false });
            }

            check_id.MaNh = nh.MaNh;
            check_id.TenNh = nh.TenNh;
            check_id.TimeUp = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new {msg="cập nhật thành công", success =true});
        }

        [HttpDelete]
        [Route("delete-NH")]
        public async Task<IActionResult> DeleteNH(int id)
        {
            var check_id = await db.Nhs.FirstOrDefaultAsync(x => x.Id == id);
            if(check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            db.Remove(check_id);
            await db.SaveChangesAsync();
            return Ok(new {msg="xóa thành công", success = true});
        }
    }
}
