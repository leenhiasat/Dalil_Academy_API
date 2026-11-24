//using Dalil_Academy.Core.Entities;
//using Dalil_Academy.Core.Repository;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Dalil_Academy.Infra.Repository
//{
//    public class InMemoryCourseRepository : ICourseRepository
//    {
//        private readonly List<Course> _courses = [];

//        public List<Course> GetAll() => _courses;

//        public Course? GetById(int id) =>
//            _courses.FirstOrDefault(c => c.Id == id);

//        public Course Add(Course course)
//        {
//            course.Id = _courses.Count == 0
//                ? 1
//                : _courses.Max(c => c.Id) + 1;

//            _courses.Add(course);

//            return course;
//        }

//        public Course? Update(int id, Course updatedCourse)
//        {
//            throw new NotImplementedException();
//        }

//        public bool Delete(int id)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
