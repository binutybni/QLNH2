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
            var check_sv = await db.Hocsinhs.AnyAsync(x => x.Id == point.IdSv);
            if (!check_sv)
            {
                return Ok(new { msg = "không có sinh này", success = false });
            }

            var check_sb = await db.Subjects.AnyAsync(x => x.Id == point.IdSv);
            if (!check_sb)
            {
                return Ok(new { msg = "không có môn này", success = false });
            }

            var check_qt = await db.Progresses.AnyAsync(x => x.Id == point.IdSv);
            if (!check_qt)
            {
                return Ok(new { msg = "không có quá trình này", success = false });
            }

            var check_dau = await db.PointStudents.AnyAsync(x => point.Point < 5 && (x.Evaluate.ToLower().Trim() == "khá" || point.Evaluate.ToLower().Trim() == "giỏi" || x.Evaluate.ToLower().Trim() == "trung bình"));
            if (check_dau)
            {
                return Ok(new { msg = $"điểm dưới 5 là rớt, không thể {point.Evaluate}", success = false });
            }

            var check_trungbinh = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6) && (point.Evaluate.ToLower().Trim() == "khá" || x.Evaluate.ToLower().Trim() == "giỏi"));
            if (check_trungbinh)
            {
                return Ok(new { msg = "điểm từ 5 tới 6 chỉ có thể là Trung bình", success = false });
            }

            var check_kha = await db.PointStudents.AnyAsync(x => (point.Point >= 7 && point.Point <= 8) && (point.Evaluate.ToLower().Trim() == "trung bình" || x.Evaluate == "giỏi"));
            if (check_kha)
            {
                return Ok(new { msg = "điểm từ 7 tới 8 chỉ có thể là Khá", success = false });
            }

            var check_gioi = await db.PointStudents.AnyAsync(x => (point.Point >= 9 && point.Point <= 10) && (point.Evaluate.ToLower().Trim() == "Khá" || x.Evaluate.ToLower().Trim() == "trung bình"));
            if (check_gioi)
            {
                return Ok(new { msg = "điểm từ 9 tới 10 chỉ có thể là Giỏi", success = false });
            }

            var newpoint = new PointStudent
            {
                Point = point.Point,
                Evaluate = point.Evaluate,
                IdMh = point.IdMh,
                IdSv = point.IdSv,
                IdQt = point.IdQt
            };
            db.Add(newpoint);
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

            var check_F = await db.PointStudents.AnyAsync(x => (point.Point >= 0 && point.Point <= 4.9) && (point.Evaluate.ToLower().Trim() == "D" || point.Evaluate.ToLower().Trim() == "C" || point.Evaluate.ToLower().Trim() == "B" || point.Evaluate.ToLower().Trim() == "A" || point.Evaluate.ToLower().Trim() == "A+"));
            if (check_F)
            {
                return Ok(new { msg = $"điểm dưới 5 là rớt, không thể {point.Evaluate}", success = false });
            }

            var check_D = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) && (point.Evaluate.ToLower().Trim() == "F" || x.Evaluate.ToLower().Trim() == "C" || x.Evaluate.ToLower().Trim() == "B" || x.Evaluate.ToLower().Trim() == "A" || x.Evaluate.ToLower().Trim() == "A+"));
            if (check_D)
            {
                return Ok(new { msg = "điểm từ 5 tới 6.5 chỉ có thể là D", success = false });
            }

            var check_C = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) && (point.Evaluate.ToLower().Trim() == "F" || x.Evaluate.ToLower().Trim() == "D" || x.Evaluate.ToLower().Trim() == "B" || x.Evaluate.ToLower().Trim() == "A" || x.Evaluate.ToLower().Trim() == "A+"));
            if (check_C)
            {
                return Ok(new { msg = "điểm từ 6.6 tới 7 chỉ có thể là C", success = false });
            }

            var check_B = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) && (point.Evaluate.ToLower().Trim() == "F" || x.Evaluate.ToLower().Trim() == "C" || x.Evaluate.ToLower().Trim() == "D" || x.Evaluate.ToLower().Trim() == "A" || x.Evaluate.ToLower().Trim() == "A+"));
            if (check_B)
            {
                return Ok(new { msg = "điểm từ 7.1 tới 7.9 chỉ có thể là B", success = false });
            }

            var check_A = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) && (point.Evaluate.ToLower().Trim() == "F" || x.Evaluate.ToLower().Trim() == "C" || x.Evaluate.ToLower().Trim() == "B" || x.Evaluate.ToLower().Trim() == "D" || x.Evaluate.ToLower().Trim() == "A+"));
            if (check_A)
            {
                return Ok(new { msg = "điểm từ 8 tới 8.9 chỉ có thể là A", success = false });
            }

            var check_Aplus = await db.PointStudents.AnyAsync(x => (point.Point >= 5 && point.Point <= 6.5) && (point.Evaluate.ToLower().Trim() == "F" || x.Evaluate.ToLower().Trim() == "C" || x.Evaluate.ToLower().Trim() == "B" || x.Evaluate.ToLower().Trim() == "A" || x.Evaluate.ToLower().Trim() == "D"));
            if (check_Aplus)
            {
                return Ok(new { msg = "điểm từ 9 trở lên chỉ có thể là A+", success = false });
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

            check_id.Point = point.Point;
            check_id.Evaluate = point.Evaluate;
            check_id.IdSv = point.IdSv;
            check_id.IdMh = point.IdMh;
            check_id.IdQt = point.IdQt;

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
