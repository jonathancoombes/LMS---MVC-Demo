using LMS.Data;
using LMS.Models;
using LMS.Models.Course;
using LMS.Models.Learning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Extentions
{

    public static class CanDeleteUnitOrChild
    {

          public static bool CanDel(CourseUnit unit, ApplicationDbContext db)
        {

            List<FormativeSession> formSessions = new List<FormativeSession>();
            List<SummativeSession> summSessions = new List<SummativeSession>();

            List<ClassEnrolment> ClassesWithSessions = new List<ClassEnrolment>();

            summSessions.AddRange(db.SummativeSessions.Where(p => p.UnitId == unit.Id));

            var result = false;
            var inActiveClass = false;
            if (unit.CourseTopicIds != null && unit.CourseTopicIds != "")
            {
                var topicIds = OrderList.ItemIdOrder(unit.CourseTopicIds);

                foreach (var item in topicIds)
                {
                    formSessions.AddRange(db.FormativeSessions.Where(p => p.TopicId == item).ToList());
                };

               
                if (summSessions.Count() > 0)
                {
                    foreach (var sess in summSessions)
                    {
                        var classenrol = db.ClassEnrolments.Include(p => p.Class).Where(p => p.Id == sess.ClassEnrolId).SingleOrDefault();

                        if (classenrol != null && classenrol.Class.EndDate < DateTime.Now)
                        {
                            inActiveClass = true;
                        }

                    }

                }

                if (formSessions.Count() > 0)
                {

                    foreach (var sess in formSessions)
                    {

                        var classenrol = db.ClassEnrolments.Include(p => p.Class).Where(p => p.Id == sess.ClassEnrolId).SingleOrDefault();
                        if (classenrol != null && classenrol.Class.EndDate > DateTime.Now)
                        {
                            inActiveClass = true;
                        }

                    }

                }

                result = (formSessions.Count() == 0 && summSessions.Count() == 0 && inActiveClass == false) ? true : false;

                return result;



            }


            return result;

        }
    }
}
