using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.GVGD;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GVGDController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public GVGDController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpGet]
        [Route("get-all-GVGD")]
        public async Task<IActionResult> GetAllGVGD()
        {
            var gv = await db.Gvgds
                .Select(x => new
                {
                    x.MaGvgd,
                    x.TenGvgd
                }).ToListAsync();
            return Ok(gv);
        }

        [HttpPost]
        [Route("create-GVGD")]
        public async Task<IActionResult> CreateGVGD([FromBody] CreateGVGD gv)
        {
            var check_ma = await db.Gvgds.FirstOrDefaultAsync(x => x.MaGvgd == gv.MaGvgd);
            if (check_ma != null)
            {
                return Ok(new { msg = "đã có mã gv này", success = false });
            }

            var check_name = await db.Gvgds.FirstOrDefaultAsync(x => x.TenGvgd == gv.TenGvgd);
            if (check_name != null)
            {
                return Ok(new { msg = "đã có mã gv này", success = false });
            }

            var teacher = new Gvgd
            {
                TenGvgd = gv.TenGvgd,
                MaGvgd = gv.MaGvgd
            };
            await db.AddAsync(teacher);
            await db.SaveChangesAsync();
            return Ok(new { msg = "thêm thành công", success = true });
        }

        [HttpPut]
        [Route("Update-GVGD")]
        public async Task<IActionResult> UpdateGVGD(int id, [FromBody] UpdateGVGD gv)
        {
            var check_id = await db.Gvgds.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }
            var check_ma = await db.Gvgds.FirstOrDefaultAsync(x => x.MaGvgd == gv.MaGvgd && x.Id != id);
            if (check_ma != null)
            {
                return Ok(new { msg = "đã có mã gv này", success = false });
            }

            var check_name = await db.Gvgds.FirstOrDefaultAsync(x => x.TenGvgd == gv.TenGvgd && x.Id != id);
            if (check_name != null)
            {
                return Ok(new { msg = "đã có mã gv này", success = false });
            }

            check_id.TenGvgd = gv.TenGvgd;
            check_id.MaGvgd = gv.MaGvgd;
            check_id.TimeUp = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });
        }

        [HttpDelete]
        [Route("Delete-GVGD")]
        public async Task<IActionResult> DeleteGVGD(int id)
        {
            var check_id = await db.Gvgds.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            db.Remove(check_id);
            return Ok(new { msg = "xóa thành công", success = true });
        }
    }
}
