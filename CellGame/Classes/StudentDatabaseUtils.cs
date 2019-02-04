using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace CellGame.Classes
{
    /*
     * This class contains all the functions dealing with the STUDENTS table.
     */
    public static class StudentDatabaseUtils
    {

        /*
         * This function takes in all the info to identify a student, scenario, which grade it belongs to, 
         * the 
         * , answer, and any comments the student had. These are inserted into the STUDENT_ANSWERS table
         * so teachers and admins can see them and enter a grade.
         */
        public static void insertStudentAnswer(string studentId, int scenarioId, int scenarioToGrade, int scenarioQuestion,
            int scenarioAnswer, string studentComment, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO STUDENT_ANSWERS " +
                                  "(" +
                                  "SA_STUDENT_ID, " +
                                  "SA_SCENARIO_ID, " +
                                  "SA_SCENARIO_TO_GRADE, " +
                                  "SA_QUESTION_ID, " +
                                  "SA_ANSWER_ID, " +
                                  "SA_STUDENT_ANSWER_TEXT" +
                                  ") " +
                                  "VALUES " +
                                  "(" +
                                  "@studentID, " +
                                  "@scenarioID, " +
                                  "@scenarioToGrade, " +
                                  "@scenarioQuestion, " +
                                  "@scenarioAnswer, " +
                                  "@studentComment" +
                                  ")"
                };

                var param = new SqlParameter("@studentID", SqlDbType.NVarChar, 128)
                {
                    Value = studentId
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@scenarioID", SqlDbType.Int)
                {
                    Value = scenarioId
                };
                command.Parameters.Add(param2);

                var param3 = new SqlParameter("@scenarioToGrade", SqlDbType.Int)
                {
                    Value = scenarioToGrade
                };
                command.Parameters.Add(param3);

                var param4 = new SqlParameter("@scenarioQuestion", SqlDbType.Int)
                {
                    Value = scenarioQuestion
                };
                command.Parameters.Add(param4);

                var param5 = new SqlParameter("@scenarioAnswer", SqlDbType.Int)
                {
                    Value = scenarioAnswer
                };
                command.Parameters.Add(param5);

                var param6 = new SqlParameter("@studentComment", SqlDbType.VarChar, 500)
                {
                    Value = studentComment
                };
                command.Parameters.Add(param6);

                connection.Close(); connection.Open();
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STU01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a string with the student's grade and returns a list of grades
         * for all the scenarios the student played aond comments made by the teacher/admin.
         */
        public static StudentGrades getStudentGrades(String studentId, SqlConnection connection)
        {
            var studentGrades = new StudentGrades();
            studentGrades.studentGrades = new List<StudentGrade>();
            try
            {
                var command = new SqlCommand(null, connection);

                command.CommandText = "SELECT SCENARIOS_TO_GRADE.STG_ID, " +
                                              "SCENARIOS_TO_GRADE.STG_GRADE," +
                                              "SCENARIOS_TO_GRADE.STG_COMMENTS," +
                                              "SCENARIOS.SCENARIO_ID, " +
                                              "SCENARIOS.SCENARIO_NAME, " +
                                              "STUDENTS.STUDENT_ID, " +
                                              "STUDENTS.STUDENT_NAME " +
                                              "FROM SCENARIOS_TO_GRADE " +
                                              "JOIN STUDENTS ON SCENARIOS_TO_GRADE.STG_STUDENT_ID = STUDENTS.STUDENT_ID " +
                                              "JOIN SCENARIOS ON SCENARIOS_TO_GRADE.STG_SCENARIO_ID = SCENARIOS.SCENARIO_ID " +
                                              "WHERE SCENARIOS_TO_GRADE.STG_GRADE IS NOT NULL AND STUDENTS.STUDENT_ID = @studentId;";

                var param1 = new SqlParameter("@studentId", SqlDbType.NVarChar, 128);
                param1.Value = studentId;
                command.Parameters.Add(param1);

                connection.Close(); connection.Open();
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var studentGrade = new StudentGrade();
                        studentGrade.stgId = Convert.ToInt32($"{reader["STG_ID"]}");
                        var stringGrade = $"{reader["STG_GRADE"]}";
                        if (stringGrade != null || !stringGrade.Equals(""))
                            studentGrade.grade = Convert.ToInt32(stringGrade);
                        else
                            studentGrade.grade = 0;
                        studentGrade.gradeComments = $"{reader["STG_COMMENTS"]}";
                        studentGrade.scenarioName = $"{reader["SCENARIO_NAME"]}";
                        studentGrade.studentId = $"{reader["STUDENT_ID"]}";
                        studentGrade.scenarioId = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        studentGrades.studentName = $"{reader["STUDENT_NAME"]}";
                        studentGrades.studentGrades.Add(studentGrade);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STU02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return studentGrades;
        }

        /*
         * This function will set the list of students in the Singleton.  
         */
        public static void getStudents(SqlConnection connection)
        {
            try
            {
                var userRolesCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT AspNetUserRoles.UserId, STUDENTS.STUDENT_NAME, AspNetRoles.Name " +
                                    "FROM AspNetUserRoles " +
                                    "JOIN STUDENTS ON AspNetUserRoles.UserId = STUDENTS.STUDENT_ID " +
                                    "JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id;"
                };
                connection.Close(); connection.Open();
                userRolesCommand.Prepare();


                var studentsList = new StudentsList();
                studentsList.studentsList = new List<Student>();
                using (var reader = userRolesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student();
                        student.id = $"{reader["UserId"]}";
                        student.name = $"{reader["STUDENT_NAME"]}";
                        student.role = $"{reader["Name"]}";
                        studentsList.studentsList.Add(student);
                    }
                }

                var getRolesCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * " +
                        "FROM AspNetRoles;"
                };
                getRolesCommand.Prepare();

                List<String> roleId = new List<String>();
                List<String> roleName = new List<String>();

                using (var reader = getRolesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roleId.Add($"{reader["Id"]}");
                        roleName.Add($"{reader["Name"]}");
                    }
                }
                studentsList.roleKeys = roleId;
                studentsList.roleNames = roleName;

                HttpContext.Current.Session["studentList"] = studentsList;
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "STU03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}