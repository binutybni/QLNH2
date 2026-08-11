using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models.DTOs.School;
using QLNH2.Models;
namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;
        public SchoolController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchool()
        {
            var school = await db.Schools.Select(x => new
            {
                x.Id,
                x.NameSchool,
                x.Address,
                x.Phone,
                x.TimeCreate,
                x.TimeUpdate
            }).ToListAsync();
            return Ok(school);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSchool([FromBody] CreateSchool trg)
        {
            var school_name = await db.Schools.AnyAsync(x => x.NameSchool == trg.NameSchool);
            if (school_name)
            {
                return Ok(new { msg = "trùng tên trường", success = false });
            }

            var school_phone = await db.Schools.AnyAsync(x => x.Phone == trg.Phone);
            if (school_phone)
            {
                return Ok(new { msg = "trùng số điện thoại", success = false });
            }

            var school = new School
            {
                NameSchool = trg.NameSchool,
                Address = trg.Address,
                Phone = trg.Phone,
                TimeCreate = unixTimestamp,
                TimeUpdate = unixTimestamp
            };
            db.Schools.Add(school);
            await db.SaveChangesAsync();
            return Ok(new { msg = "thêm thành công", success = true });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSchool(int id, [FromBody] UpdateSchool trg)
        {
            var school_id = await db.Schools.FirstOrDefaultAsync(x => x.Id == id);
            if (school_id == null)
            {
                return Ok(new { msg = $"không tìm thấy trường có id là {id}", success = false });
            }

            var school = await db.Schools.AnyAsync(x => x.NameSchool == trg.NameSchool && x.Phone == trg.Phone && x.Id != id);
            if (school)
            {
                return Ok(new { msg = "có thể tên hoặc số điện đã tồn tại", success = false });
            }

            school_id.NameSchool = trg.NameSchool;
            school_id.Address = trg.Address;
            school_id.Phone = trg.Phone;
            school_id.TimeUpdate = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", succes = true });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            var check_schoolid = await db.Schools.FirstOrDefaultAsync(x=> x.Id == id);
            if (check_schoolid == null)
            {
                return Ok(new { msg = $"không tìm thấy trường có id là {id}", success = false });
            }

            db.Remove(check_schoolid);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });
        }
    }
}
