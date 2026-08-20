using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.Progress;
using QLNH2.Models.DTOs.Student;
using Microsoft.EntityFrameworkCore;
using QLNH2.Models.DTOs.Subject;


namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public SubjectController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpPost]
        [Route("get-all-subject")]
        public async Task<IActionResult> GetAllProgress([FromBody] PaginationClass search)
        {
            var query = db.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query.Where(x =>
                x.NameSub.ToLower().Trim().Contains(search.SearchTerm) ||
                x.MaMh.ToLower().Trim().Contains(search.SearchTerm));
            }

            if (!query.Any())
            {
                return Ok(new { msg = "không tìm thấy", success = false });
            }

            var sb = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new
                {
                    x.Id,
                    x.MaMh,
                    x.NameSub
                }).ToListAsync();

            var totalcount = sb.Count();
            return Ok(new
            {
                data = sb,
                currentPage = search.Page,
                totalcount = totalcount,
                totalPage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });

        }

        [HttpPost]    // thêm Time-cre và Time-up
        [Route("create-subject")]
        public async Task<IActionResult> CreateProgress([FromBody] CreateSubject sb)
        {
            var check_sb = await db.Subjects.AnyAsync(x => x.NameSub == sb.NameSub);
            if (check_sb)
            {
                return Ok(new { msg = "đã có môn đó rồi", success = false });
            }

            var check_codemh = await db.Subjects.AnyAsync(x => x.MaMh == sb.MaMh);
            if (check_sb)
            {
                return Ok(new { msg = "đã có mã môn đó rồi", success = false });
            }

            var newqt = new Subject
            {
                NameSub = sb.NameSub,
                MaMh = sb.MaMh

            };
            await db.AddAsync(newqt);
            await db.SaveChangesAsync();
            return Ok(new { msg = "thêm thành công", success = true });
        }

        [HttpPut]
        [Route("update-progress")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateSubject sb)
        {
            var check_sb = await db.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (check_sb == null)
            {
                return Ok(new { msg = "không có môn đó", success = false });
            }

            var check_maMh_namesb = await db.Subjects.AnyAsync(x => x.MaMh == sb.MaMh && x.NameSub == sb.NameSub);
            if (check_maMh_namesb)
            {
                return Ok(new { msg = "trùng mã môn học với tên môn học rồi", success = false });
            }

            var check_codemh = await db.Subjects.AnyAsync(x => x.MaMh == sb.MaMh && x.Id != id);
            if (check_codemh)
            {
                return Ok(new { msg = "trùng mã môn đó rồi", success = false });
            }

            var check_namesb = await db.Subjects.AnyAsync(x => x.NameSub == sb.NameSub && x.Id != id);
            if (check_namesb)
            {
                return Ok(new { msg = "trùng môn rồi", success = false });
            }

            check_sb.NameSub = sb.NameSub;
            check_sb.MaMh = sb.MaMh;
            check_sb.TimeUp = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });
        }

        [HttpDelete]
        [Route("delete-progress")]
        public async Task<IActionResult> DeleteProgress(int id) // hỏi xóa 1 môn mà vẫn có sinh viên đang học thì sao
        {
            var check_sb = await db.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (check_sb == null)
            {
                return Ok(new { msg = "không có id đó", success = false });
            }

            var check_idMh = await db.Subjects.AnyAsync(x => x.Id == id);
            if (check_idMh)
            {
                return Ok(new { msg = "không thể xóa môn này vì vẫn còn sinh viên đang học", success = false });
            }

            db.Remove(check_sb);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });

        }
    }
}
