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
    public class DKMHController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public DKMHController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        [HttpGet]
        [Route("get-all-dkmh")]
        public async Task<IActionResult> GetAllDKMH()
        {
            var dk = await db.Dkmhs
                .Select(x => new
                {
                    x.IdGvgdmh,
                    x.IdHs
                }).ToListAsync();
            return Ok(dk);
        }

        [HttpPost]
        [Route("create-DKMH")]
        public async Task<IActionResult> CreateDKMH([FromBody] CreateDKMH dk)
        {
            var check_gv = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == dk.IdGvgdmh);
            if(check_gv == null)
            {
                return Ok(new { msg = "không có giáo viên này", success = false });
            }

            var check_hs = await db.Hocsinhs.FirstOrDefaultAsync(x => x.Id == dk.IdHs);
            if(check_hs == null)
            {
                return Ok(new { msg = "không có hs này", success = false });
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

        [HttpPut]
        [Route("update-dkmh")]
        public async Task<IActionResult> UpdateDKMH(int id, [FromBody] UpdateDKMH dk)
        {
            var check_id = await db.Dkmhs.FirstOrDefaultAsync(x => x.Id == id);
            if(check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            var check_pcgv = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == dk.IdGvgdmh);
            if(check_pcgv == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            var check_hs = await db.Hocsinhs.FirstOrDefaultAsync(x => x.Id == dk.IdHs);
            if(check_hs == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            // trùng
            var check_pcgv_hs = await db.Dkmhs.FirstOrDefaultAsync(x => x.IdGvgdmh == dk.IdGvgdmh && x.IdHs == dk.IdHs && x.Id != id);
            if (check_pcgv_hs == null)
            {
                return Ok(new { msg = "bị trùng rồi", success = false });
            }

            check_id.IdGvgdmh = dk.IdGvgdmh;
            check_id.IdHs = dk.IdHs;
            await db.SaveChangesAsync();
            return Ok(new {msg="cập nhật thành công", success = true});
        }

        [HttpDelete]
        [Route("delete-dkmh")]
        public async Task<IActionResult> DeleteDKMH(int id)
        {
            var check_id = await db.Dkmhs.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            db.Remove(check_id);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });
        }
    }

}
