using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.Point_Student;
using QLNH2.Models.DTOs.Student;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointStudentController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public PointStudentController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }

        [HttpPost]
        [Route("get-all-pointstudent")]
        public async Task<IActionResult> GetAllPointStudent([FromBody] PaginationClass search)
        {
            var query = db.PointStudents.AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query.Where(x =>
                x.Evaluate.ToLower().Trim().Contains(search.SearchTerm));
            }

            if (query.Count() == 0)
            {
                return Ok(new { msg = "không tìm thấy", success = false });
            }

            var point = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new
                {
                    x.Point,
                    x.Evaluate,
                    x.IdSv,
                    x.IdMh,
                    x.IdQt
                }).ToListAsync();

            var totalcount = point.Count();
            if (totalcount == 0)
            {
                return Ok(new { msg = "hết dữ liệu", success = false });
            }

            return Ok(new
            {
                data = point,
                currentPage = search.Page,
                totalcount = totalcount,
                totalPage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });
        }

        [HttpPost]
        [Route("create-pointstudent")]
        public async Task<IActionResult> CreatePointStudent([FromBody] CreatePoint_Student point)
        {
            var check_id = await db.PointStudents.FirstOrDefaultAsync(x => x.Id == point.IdSv);


            var check_sb = await db.Subjects.AnyAsync(x => x.Id == point.IdMh);
            if (!check_sb)
            {
                return Ok(new { msg = "không có môn này", success = false });
            }

            var check_qt = await db.Progresses.AnyAsync(x => x.Id == point.IdQt);
            if (!check_qt)
            {
                return Ok(new { msg = "không có quá trình này", success = false });
            }
            var newpoint = new PointStudent
            {
                Point = point.Point,
                IdMh = point.IdMh,
                IdSv = point.IdSv,
                IdQt = point.IdQt
            };
            db.Add(newpoint);
            var check_F = await db.PointStudents.AnyAsync(x => (point.Point >= 0 && point.Point <= 4.9));
            if (check_F)
            {
                newpoint.Evaluate = "F";
            }

            var check_D = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5));
            if (check_D)
            {
                newpoint.Evaluate = "D";

            }

            var check_C = await db.PointStudents.AnyAsync(x => (point.Point >= 6.6 && point.Point <= 7));
            if (check_C)
            {
                newpoint.Evaluate = "C";
            }

            var check_B = await db.PointStudents.AnyAsync(x => (point.Point >= 7.1 && point.Point <= 7.9));
            if (check_B)
            {
                newpoint.Evaluate = "B";
            }

            var check_A = await db.PointStudents.AnyAsync(x => (point.Point >= 8 && point.Point <= 8.9));
            if (check_A)
            {
                newpoint.Evaluate = "A";
            }

            var check_Aplus = await db.PointStudents.AnyAsync(x => (point.Point >= 9 && point.Point <= 10));
            if (check_Aplus)
            {
                newpoint.Evaluate = "A+";
            }

            
            await db.SaveChangesAsync();
            return Ok(new { mmsg = "thêm thành công", success = true });
        }

        [HttpPut]
        [Route("update-pointstudent")]
        public async Task<IActionResult> UpdatePointStudent(int id, [FromBody] UpdatePoint_Student point)
        {
            var check_id = await db.PointStudents.FirstOrDefaultAsync(x => x.Id == id);
            if (check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }

            var check_0 = await db.PointStudents.AnyAsync(x => point.Point < 0);
            if (check_0)
            {
                return Ok(new { msg = "không thể nhập điểm dưới 0", success = false });
            }

            var check_10 = await db.PointStudents.AnyAsync(x => point.Point > 10);
            if (check_10)
            {
                return Ok(new { msg = "không thể nhập điểm trên 10", success = false });
            }

            if (point.Evaluate != "F" &&
                point.Evaluate != "D" &&
                point.Evaluate != "C" &&
                point.Evaluate != "B" &&
                point.Evaluate != "A" &&
                point.Evaluate != "A+"
               )
            {
                return Ok(new { msg = "đánh giá chỉ có thể là F, D, C ,B ,A ,A+ thôi", success = false });
            }

            check_id.Point = point.Point;
            check_id.IdSv = point.IdSv;
            check_id.IdMh = point.IdMh;
            check_id.IdQt = point.IdQt;

            var check_F = await db.PointStudents.AnyAsync(x => (point.Point >= 0 && point.Point <= 4.9));
            if (check_F)
            {
                check_id.Evaluate = "F";
            }

            var check_D = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) );
            if (check_D)
            {
                check_id.Evaluate = "D";

            }

            var check_C = await db.PointStudents.AnyAsync(x => (point.Point >= 6.6 && point.Point <= 7));
            if (check_C)
            {
                check_id.Evaluate = "C";
            }

            var check_B = await db.PointStudents.AnyAsync(x => (point.Point >= 7.1 && point.Point <= 7.9));
            if (check_B)
            {
                check_id.Evaluate = "B";
            }

            var check_A = await db.PointStudents.AnyAsync(x => (point.Point >= 8 && point.Point <= 8.9));
            if (check_A)
            {
                check_id.Evaluate = "A";
            }

            var check_Aplus = await db.PointStudents.AnyAsync(x => (point.Point >= 9 && point.Point <= 10));
            if (check_Aplus)
            {
                check_id.Evaluate = "A+";
            }

            var check_sv = await db.Hocsinhs.AnyAsync(x => x.Id == point.IdSv);
            if (!check_sv)
            {
                return Ok(new { msg = "không có sinh viên này", success = false });
            }

            var check_mon = await db.Subjects.AnyAsync(x => x.Id == point.IdMh);
            if (!check_mon)
            {
                return Ok(new { msg = "không có môn đó", success = false });
            }

            var check_qt = await db.Progresses.AnyAsync(x => x.Id == point.IdQt);
            if (!check_qt)
            {
                return Ok(new { msg = "không có quá trình đó", success = false });
            }

            

            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });
        }

        [HttpDelete]
        [Route("delete-pointstudent")]
        public async Task<IActionResult> DeletePointStudent(int id)
        {
            var check_id = await db.PointStudents.FirstOrDefaultAsync(x => x.Id == id);
            if(check_id == null)
            {
                return Ok(new { msg = "không có id này", success = false });
            }


            db.Remove(check_id);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });
        }
    }
}
