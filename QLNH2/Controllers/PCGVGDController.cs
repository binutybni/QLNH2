using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.PCGVGD;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCGVGDController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public PCGVGDController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpGet]
        [Route("get-all-PCGVGD")]
        public async Task<IActionResult> GetAllPCGVGD()
        {
            var pcgv = await db.Pcgvgds
                .Select(x => new
                {
                    x.IdGvgd,
                    x.IdClass,
                    x.IdMh,
                    x.IdQt,
                    x.IdNh
                }).ToListAsync();
            return Ok(pcgv);
        }

        [HttpPost]
        [Route("create-PCGVGD")]
        public async Task<IActionResult> CreatePCGVGD([FromBody] CreatePCGVGD gv)
        {
            var check_gv = await db.Gvgds.FirstOrDefaultAsync(x => x.Id == gv.IdGvgd);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của gv này", success = false });
            }

            var check_class = await db.Classes.FirstOrDefaultAsync(x => x.Id == gv.IdClass);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của lớp này", success = false });
            }

            var check_mh = await db.Subjects.FirstOrDefaultAsync(x => x.Id == gv.IdMh);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của môn học này", success = false });
            }

            var check_qt = await db.Progresses.FirstOrDefaultAsync(x => x.Id == gv.IdQt);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của quá trình này", success = false });
            }

            var check_nh = await db.Nhs.FirstOrDefaultAsync(x => x.Id == gv.IdNh);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của năm học này", success = false });
            }

            var pcgv = new Pcgvgd
            {
                IdClass = gv.IdClass,
                IdMh = gv.IdMh,
                IdQt = gv.IdQt,
                IdNh = gv.IdNh,
                IdGvgd = gv.IdGvgd
            };

            await db.AddAsync(pcgv);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("update-PCGVGD")]
        public async Task<IActionResult> UpdatePCGVGD(int id, [FromBody] UpdatePCGVGD gv)
        {
            var check_id = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            var check_gv = await db.Gvgds.FirstOrDefaultAsync(x => x.Id == gv.IdGvgd);
            if (check_gv == null)
            {
                return Ok(new { msg = "không có id của gv này", success = false });
            }

            var check_class = await db.Classes.FirstOrDefaultAsync(x => x.Id == gv.IdClass);
            if (check_class == null)
            {
                return Ok(new { msg = "không có id của lớp này", success = false });
            }

            var check_mh = await db.Subjects.FirstOrDefaultAsync(x => x.Id == gv.IdMh);
            if (check_mh == null)
            {
                return Ok(new { msg = "không có id của môn học này", success = false });
            }

            var check_qt = await db.Progresses.FirstOrDefaultAsync(x => x.Id == gv.IdQt);
            if (check_qt == null)
            {
                return Ok(new { msg = "không có id của quá trình này", success = false });
            }

            var check_nh = await db.Nhs.FirstOrDefaultAsync(x => x.Id == gv.IdNh);
            if (check_nh == null)
            {
                return Ok(new { msg = "không có id của năm học này", success = false });
            }

            // trùng
            var check_gv2 = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == gv.IdGvgd && x.Id != id);
            if (check_gv2 == null)
            {
                return Ok(new { msg = "đã có id của gv này", success = false });
            }

            var check_class2 = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == gv.IdClass && x.Id != id);
            if (check_class2 == null)
            {
                return Ok(new { msg = "đã có id của lớp này", success = false });
            }

            var check_mh2 = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == gv.IdMh && x.Id != id);
            if (check_mh2 == null)
            {
                return Ok(new { msg = "đã có id của môn học này", success = false });
            }

            var check_qt2 = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == gv.IdQt && x.Id != id);
            if (check_qt2 == null)
            {
                return Ok(new { msg = "đã có id của quá trình này", success = false });
            }

            var check_nh2 = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == gv.IdNh && x.Id != id);
            if (check_nh2 == null)
            {
                return Ok(new { msg = "đã có id của năm học này", success = false });
            }

            check_id.IdGvgd = gv.IdGvgd;
            check_id.IdClass = gv.IdClass;
            check_id.IdQt = gv.IdQt;
            check_id.IdMh = gv.IdMh;
            check_id.IdNh = gv.IdNh;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });
        }

        [HttpDelete]
        [Route("delete-PCGVGD")]
        public async Task<IActionResult> DeletePCGVGD(int id)
        {
            var check_id = await db.Pcgvgds.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            db.Remove(check_id);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
