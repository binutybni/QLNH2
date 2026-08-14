using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using QLNH2.Data;
using QLNH2.Models;
using QLNH2.Models.DTOs.Student;

namespace QLNH2.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly QLNHDbContext db;
        private readonly int unixTimestamp;

        public StudentController(QLNHDbContext _db)
        {
            this.db = _db;
            DateTime now = DateTime.UtcNow;
            unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        }


        [HttpPost]
        [Route("get-all-student")]
        public async Task<IActionResult> GetALlStudents([FromBody] PaginationClass search)
        {
            var query = db.Hocsinhs.AsQueryable();   // xài AsQueryable thì cần gõ trực tiếp giống như gõ code trên sql, và xài nhiều lần , và khái niệm(Inumberal, IQuerytable)

            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query
                    .Where(x => x.NameStudent.ToLower().Trim().Contains(search.SearchTerm) ||
                                x.CodeStudent.ToLower().Trim().Contains(search.SearchTerm));
            }
            if (query.Count() == 0)
            {
                return Ok(new { msg = "không tìm thấy ", success = false });
            }

            var hs2 = query
                .Skip((search.Page - 1) * search.PageSize)
                .AsEnumerable()
                .Take(search.PageSize)
                .Select((x, index) => new
                {
                    STT = (search.Page - 1) * search.PageSize + index + 1,
                    x.NameStudent,
                    x.CodeStudent
                });
            var totalcount = hs2.Count();
            var totalrecord = hs2.Count();
            if (totalrecord == 0)
            {
                return Ok(new { msg = "hết dữ liệu", success = false });
            }
            return Ok(new
            {
                data = hs2,
                currentPage = search.Page,
                totalcount = totalcount,
                totalpage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });

        }
        [HttpPost]
        public async Task<IActionResult> CreatStudent([FromBody] CreateStudent hs)
        {
            var check_student = await db.Hocsinhs.FirstOrDefaultAsync(x => x.CodeStudent == hs.CodeStudent);
            if (check_student != null)
            {
                return Ok(new { msg = "trùng code hs", success = false });
            }

            var check_class = await db.Classes.FirstOrDefaultAsync(x => x.Id == hs.Classid);
            if (check_class == null)
            {
                return Ok(new { msg = "không tìm thấy lớp ", success = false });
            }

            var new_student = new Hocsinh
            {
                NameStudent = hs.NameStudent,
                CodeStudent = hs.CodeStudent,
                Classid = hs.Classid,
                TimeCreate = unixTimestamp,
                TimeUpdate = unixTimestamp
            };
            db.Hocsinhs.Add(new_student);
            await db.SaveChangesAsync();
            return Ok(new { msg = "tạo mới thành công", success = true });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudent hs)
        {
            var check_student = await db.Hocsinhs.FirstOrDefaultAsync(x => x.Id == id);
            if ((check_student == null))
            {
                return Ok(new { msg = $"không tìm thấy học sinh có id là {id}", success = false });
            }

            var check_code = await db.Hocsinhs.AnyAsync(x => x.CodeStudent == hs.CodeStudent && x.Id != id);
            if (check_code)
            {
                return Ok(new { msg = "trùng code học sinh", success = false });
            }

            check_student.NameStudent = hs.NameStudent;
            check_student.CodeStudent = hs.CodeStudent;
            check_student.Classid = hs.Classid;
            check_student.TimeUpdate = unixTimestamp;
            await db.SaveChangesAsync();
            return Ok(new { msg = "cập nhật thành công", success = true });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var check_student = await db.Hocsinhs.FirstOrDefaultAsync(x => x.Id == id);
            if (check_student == null)
            {
                return Ok(new { msg = $"không tìm thấy học sinh có id là {id}", success = false });
            }
            db.Hocsinhs.Remove(check_student);
            await db.SaveChangesAsync();
            return Ok(new { msg = "xóa thành công", success = true });
        }

        [HttpGet]
        [Route("get-all-class")]
        public async Task<IActionResult> GetAllClass()
        {
            var lop = await db.Classes
                .Select(x => new
                {
                    x.Id,
                    x.NameClass,
                })
                .ToListAsync();
            return Ok(new { data = lop, success = true });
        }

        [HttpGet]
        [Route("get-all-student-by-id-class/{id}")]
        public async Task<IActionResult> GetAllStudentByIdClass(int id)
        {
            var check_class = await db.Classes.FirstOrDefaultAsync(x => x.Id == id);
            if (check_class == null)
            {
                return Ok(new { msg = "không tìm thấy class có id", success = false });
            }

            var new_hs = await db.Hocsinhs
                .Where(x => x.Classid == id)
                .Select(x => new
                {
                    x.NameStudent,
                    x.CodeStudent,
                    x.Class.NameClass

                }).ToListAsync();


            return Ok(new { data = new_hs, success = true });
        }

        [HttpPost]
        [Route("get-all-class-by-id-student")]
        public async Task<IActionResult> GetAllClassByIdStudent(int idlop)
        {
            var lop = await db.Classes
                .Where(x => x.Id == idlop)
                .Select(x => new
                {
                    x.Id,
                    x.NameClass,
                })
                .ToListAsync();
            var newList = new List<object>();
            foreach (var item in lop)
            {
                var hs = await db.Hocsinhs
                    .Where(x => x.Classid == item.Id)
                    .Select(x => new
                    {
                        x.NameStudent,
                        x.CodeStudent,

                    })
                    .ToListAsync();
                newList.Add(new
                {
                    item.NameClass,
                    hs
                });
            }
            return Ok(new { data = newList });
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

        // lớp của 1 trường, tất cả hs của 1 lớp, phân trang để hiển thị ra 7 dữ liệu
        [HttpPost]
        [Route("get-all-student-in-class-by-id-school")]
        public async Task<IActionResult> GetAllStudentInClassByIdSchool(int idtrg)
        {
            var school = await db.Schools
                .Where(x => x.Id == idtrg)
                .Select(x => new
                {
                    x.Id,
                    x.NameSchool,
                    x.Phone,
                }).ToListAsync();

            var newlist = new List<object>();
            foreach (var item in school)
            {
                var lop = await db.Classes
                .Where(x => x.Schoolid == item.Id)
                .Select(x => new
                {
                    x.Id,
                    x.NameClass,
                    x.NameSubject
                })
                .ToListAsync();

                var new2list = new List<object>();
                foreach (var item2 in lop)
                {
                    var hs = await db.Hocsinhs
                       .Where(x => x.Classid == item2.Id)
                       .Select(x => new
                       {
                           x.NameStudent,
                           x.CodeStudent
                       })
                       .ToListAsync();
                    new2list.Add(new
                    {
                        item2.NameClass,
                        hs
                    });
                }
                newlist.Add(new
                {
                    item.NameSchool,
                    new2list

                });
            }
            return Ok(new { data = newlist });
        }

        // chỉ hiện thị ra lớp không có sinh viên
        [HttpPost]
        [Route("get-all-student-not-in-class-by-id-school")]
        public async Task<IActionResult> GetAllStudentNotInClassByIdSchool(int idtrg)
        {
            var school = await db.Schools
                .Where(x => x.Id == idtrg)
                .Select(x => new
                {
                    x.Id,
                    x.NameSchool,
                    x.Phone,
                }).ToListAsync();

            var newlist = new List<object>();
            foreach (var item in school)
            {
                var lop = await db.Classes
                .Where(x => x.Schoolid == item.Id)
                .Select(x => new
                {
                    x.Id,
                    x.NameClass,
                    x.NameSubject
                })
                .ToListAsync();

                var new2list = new List<object>();
                foreach (var item2 in lop)
                {
                    var hs = await db.Hocsinhs
                       .Where(x => x.Classid == item2.Id)
                       .Select(x => new
                       {
                           x.NameStudent,
                           x.CodeStudent
                       })
                       .ToListAsync();

                    if (hs.Any())
                    {
                        new2list.Add(new
                        {
                            item2.NameClass,
                            hs
                        });

                    }



                }
                newlist.Add(new
                {
                    item.NameSchool,
                    new2list

                });

            }
            return Ok(new { data = newlist });
        }

        //nếu như lớp đó có sinh viên thì hiển thị danh sách sinh viên của lớp đó, nếu ko có thì hiển thị dòng thông báo "ko có sv" của lớp đó
        [HttpPost]
        [Route("get-all-student-if-student-in-class-if-not-show-not-in-class")]
        public async Task<IActionResult> bt3(int idtrg)
        {
            var school = await db.Schools
                .Where(x => x.Id == idtrg)
                .Select(x => new
                {
                    x.Id,
                    x.NameSchool,
                    x.Phone,
                }).ToListAsync();

            var newlist = new List<object>();
            foreach (var item in school)
            {
                var lop = await db.Classes
                .Where(x => x.Schoolid == item.Id)
                .Select(x => new
                {
                    x.Id,
                    x.NameClass,
                    x.NameSubject
                })
                .ToListAsync();

                var new2list = new List<object>();
                var List0sv = new List<object>();
                var Listcosv = new List<object>();
                foreach (var item2 in lop)
                {
                    var hs = await db.Hocsinhs
                       .Where(x => x.Classid == item2.Id)
                       .Select(x => new
                       {
                           x.NameStudent,
                           x.CodeStudent
                       })
                       .ToListAsync();

                    if (hs.Any())
                    {
                        Listcosv.Add(new
                        {
                            item2.NameClass,
                            hs
                        });

                    }
                    else
                    {
                        List0sv.Add(new
                        {
                            item2.NameClass,
                            mgs = "ko có sv"


                        });
                    }

                }
                new2list.AddRange(Listcosv);
                new2list.AddRange(List0sv);
                newlist.Add(new
                {
                    item.NameSchool,
                    new2list

                });
            }
            return Ok(new { data = newlist });
        }

        [HttpPost] // 3 tiếng
        [Route("bt-1-buoi-2")]
        public async Task<IActionResult> GetAllStudentAndClassAndSchool(int idtrg, [FromBody] PaginationClass search)
        {
            var query = db.Hocsinhs.Include(x => x.Class).AsQueryable(); // xài AsQueryable thì cần gõ trực tiếp giống như gõ code trên sql, và xài nhiều lần , và khái niệm(Inumberal, IQuerytable)
            if (!string.IsNullOrEmpty(search.SearchTerm))
            {
                query = query.Where(x =>
                x.NameStudent.ToLower().Trim().Contains(search.SearchTerm) ||
                x.CodeStudent.ToLower().Trim().Contains(search.SearchTerm) ||
                x.Class.NameClass.ToLower().Trim().Contains(search.SearchTerm));
            }

            if (!query.Any())
            {
                return Ok(new { msg = "không tìm thấy dữ liệu bạn đang tìm", success = false });
            }

            var school = await db.Schools
                .Where(x => x.Id == idtrg)
                .Select(x => new
                {
                    x.NameSchool
                })
                .FirstOrDefaultAsync();
            if (school == null)
            {
                return Ok(new { msg = "không có id trường đó", success = false });
            }

            var list = new List<object>();
            
            var lop = await db.Classes
                .Where(x => x.Schoolid == idtrg)
                .Select(x => x.NameClass)
                .FirstOrDefaultAsync();

            var list2 = new List<object>();

            var student = query
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .AsEnumerable()     // coi cái này là ranh giới, bình thường thì EF vẫn xứ lý trên IsQueryTable vì nó tự hiểu, nên sau khi dùng AsEnumerable thì nó chỉ xữ lý trên C# nên chỉ có thể dùng dữ liệu đã cung cấp thôi. Ví dụ lúc đầu db.Hocsinh thì về sau chỉ có thể lấy dữ liệu từ bảng học sinh nếu muôn lấy thêm dữ liệu của bảng khác thì phải Include(x => x.(bảng) trước đó) 
            .Select((x, Index) => new
            {
                STT = (search.Page - 1) * search.PageSize + Index + 1,
                x.NameStudent,
                x.CodeStudent,
            }).ToList();

            list.Add(new
            {
                school.NameSchool,
                lop
            });

            list2.Add(new
            {
                list,
                student
            });

            var totalcount = query.Count(); 
            return Ok(new
            {
                data = list2,
                currentPage = search.Page,
                totalcount = totalcount,
                totalPage = (int)Math.Ceiling((double)totalcount / search.PageSize)
            });



        }
    }
}
