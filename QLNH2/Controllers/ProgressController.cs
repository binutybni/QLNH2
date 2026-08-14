using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models.DTOs.Progress;
using QLNH2.Models.DTOs.Student;
using QLNH2.Models;


namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public ProgressController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpPost]
        [Route("get-all-progress")]
        public async Task<IActionResult> GetAllProgress([FromBody] PaginationClass search)
        {
            var query = db.Progresses.AsQueryable();
            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query.Where(x => x.NameProgress.ToLower().Trim().Contains(search.SearchTerm));
            }

            if (!query.Any())
            {
                return Ok(new { msg = "không tìm thấy", success = false });
            }

            var qt = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new
                {
                    x.NameProgress
                }).ToListAsync();

            var totalcount = qt.Count();
            return Ok(new
            {
                data = qt,
                currentPage = search.Page,
                totalcount = totalcount,
                totalPage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });

        }

        [HttpPost]
        [Route("create-progress")]
        public async Task<IActionResult> CreateProgress([FromBody] CreateProgress qt)
        {
            var check_qt = await db.Progresses.AnyAsync(x => x.NameProgress == qt.NameProgress);
            if (check_qt)
            {
                return Ok(new { msg = "đã có quá trình đó rồi", success = false });
            }

            var newqt = new Progress
            {
                NameProgress = qt.NameProgress
            };
            await db.AddAsync(newqt);
            await db.SaveChangesAsync();
            return Ok(new { msg = "thêm thành công", success = true });
        }

        [HttpPut]
        [Route("update-progress")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateProgress qt)
        {
            var check_qt = await db.Progresses.FirstOrDefaultAsync(x => x.Id == id);
            if (check_qt == null)
            {
                return Ok(new { msg = "không có tên quá trình đó", success = false });
            }

            var check_nameqt = await db.Progresses.AnyAsync(x => x.NameProgress == qt.NameProgress);
            if (check_nameqt)
            {
                return Ok(new { msg = "quá trình bị trùng rồi", success = false });
            }
            check_qt.NameProgress = qt.NameProgress;
            check_qt.TimeUp = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });
        }

        [HttpDelete]
        [Route("delete-progress")]
        public async Task<IActionResult> DeleteProgress(int id)
        {
            var check_qt = await db.Progresses.FirstOrDefaultAsync(x => x.Id == id);
            if (check_qt == null)
            {
                return Ok(new { msg = "không có id đó", success = false });
            }

            var check_idqt = await db.PointStudents.AnyAsync(x => x.IdQt == id);
            if (check_idqt)
            {
                return Ok(new { msg = "học kỳ này vẫn còn sinh viên đang học không xóa được", success = false });
            }

            db.Remove(check_qt);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });

        }

    }
}
