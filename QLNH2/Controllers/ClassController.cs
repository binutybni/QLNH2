using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNH2.Data;
using QLNH2.Models.DTOs.Class;
using QLNH2.Models;
using QLNH2.Models.DTOs.Student;

namespace QLNH2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public ClassController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        [HttpPost]
        [Route("get-all-class")]
        public async Task<IActionResult> GetAllCLass([FromBody] PaginationClass search)
        {
            var totalcount = await db.Classes.CountAsync();
            var query = db.Classes.AsQueryable();

            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query.Where(x =>
                x.NameClass.ToLower().Trim().Contains(search.SearchTerm) );
            }

            var lop = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(x => new
                {
                    x.NameClass
                }).ToListAsync();

            var totalRecord = lop.Count();
            if (totalRecord == 0)
            {
                return Ok(new { msg = "hết dữ liệu", success = false });
            }
            return Ok(new
            {
                data = lop,
                currentPage = search.Page,
                totalcount = totalcount,
                totalPage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] CreateClass lop)
        {
            var check_namelop = await db.Classes.AnyAsync(x => x.NameClass == lop.NameClass);
            if (check_namelop)
            {
                return Ok(new { msg = $"đã có lớp {lop.NameClass} rồi", success = false });
            }
            ;

            var check_idschool = await db.Schools.FirstOrDefaultAsync(x => x.Id == lop.Schoolid);
            if (check_idschool == null)
            {
                return Ok(new { msg = $"không tìm thấy trường có id là {lop.Schoolid}", success = false });
            }

            var newclass = new Class
            {
                NameClass = lop.NameClass,
                Schoolid = lop.Schoolid,
                TimeCreate = unixTimestamp,
                TimeUpdate = unixTimestamp,
            };
            db.Add(newclass);
            await db.SaveChangesAsync();
            return Ok(new { msg = "tạo mới thành công", success = true });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClass lop)
        {
            var check_idclass = await db.Classes.FirstOrDefaultAsync(x => x.Id == id);
            if (check_idclass == null)
            {
                return Ok(new { msg = $"không tìm thấy lớp có {id}", success = false });
            }

            var check_idschool = await db.Schools.FirstOrDefaultAsync(x => x.Id == lop.Schoolid);
            if (check_idschool == null)
            {
                return Ok(new { msg = $"không tìm thấy trường có id là {lop.Schoolid}", success = false });
            }

            var check_namelop = await db.Classes.AnyAsync(x => x.NameClass == lop.NameClass && x.Schoolid == lop.Schoolid && x.Id != id);
            if (check_namelop)
            {
                return Ok(new { msg = $"đã có lớp {lop.NameClass} rồi", success = false });
            }

            check_idclass.NameClass = lop.NameClass;
            check_idclass.Schoolid = lop.Schoolid;
            check_idclass.TimeUpdate = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành cộng", success = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Deleteclass(int id)
        {
            var checkid = await db.Classes.FirstOrDefaultAsync(x => x.Id == id);
            if (checkid == null)
            {
                return Ok(new { msg = $"không tìm thấy lớp có {id}", success = false });
            }

            db.Remove(checkid);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });
        }
        [HttpGet]
        [Route("get-student-by-id-class")]
        public async Task<IActionResult> GetStudentByIDClass(int idClass)
        {
            //Cách bỏ vòng lặp
            var checklop = await db.Classes
                .Where(x => x.Id == idClass)
                .Select(x => new
                {
                    x.Id,
                    x.NameClass
                })
                .FirstOrDefaultAsync();
            var ListStudent = new List<object>();
            var getstudentbylop = await db.Hocsinhs
                .Where(x => x.Classid == idClass)
                .Select(x => new
                {
                    x.Id,
                    x.NameStudent,
                    x.Class.NameClass
                })
                .ToListAsync();
            ListStudent.Add(new
            {
                checklop.NameClass,
                getstudentbylop
            });
            return Ok(new { data = ListStudent });
        }

    }
}
