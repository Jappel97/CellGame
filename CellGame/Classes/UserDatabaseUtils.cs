using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace CellGame.Classes
{
    /*
     * This class contains all the functions that interact with the users table in the database
     */
    public static class UserDatabaseUtils
    {
        /*
         * This function is called when a user logs in. It sets the username and ID in the Singleton.
         */
        public static void setUserNameId(string email, SqlConnection connection)
        {
            try
            {
                string userIdString;
                string userNameString;
                var setUserNameCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT AspNetUsers.Id, STUDENTS.STUDENT_NAME " +
                                  "FROM AspNetUsers JOIN STUDENTS ON AspNetUsers.Id = STUDENTS.STUDENT_ID " +
                                  "WHERE Email = @email"
                };

                var paramEmail = new SqlParameter("@email", SqlDbType.VarChar, 40)
                {
                    Value = email
                };
                setUserNameCommand.Parameters.Add(paramEmail);

                connection.Close(); connection.Open();
                setUserNameCommand.Prepare();

                bool found = false;
                using (var reader = setUserNameCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        found = true;
                        userIdString = $"{reader["Id"]}";
                        userNameString = $"{reader["STUDENT_NAME"]}";
                        HttpContext.Current.Session["userId"] = userIdString;
                        HttpContext.Current.Session["userName"] = userNameString;
                    }
                }

                if (!found)
                {
                    var command1 = new SqlCommand(null, connection)
                    {
                        CommandText = "SELECT AspNetUsers.Id " +
                                        "FROM AspNetUsers " +
                                        "WHERE Email = @email"
                    };

                    var param2 = new SqlParameter("@email", SqlDbType.VarChar, 40)
                    {
                        Value = email
                    };
                    command1.Parameters.Add(param2);

                    command1.Prepare();

                    using (var reader = command1.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Current.Session["userId"] = $"{reader["Id"]}";
                        }
                    }
                    setNewUserName(email, connection);

                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

        }

        /*
         * When a user registers, split the email into strings for their first name and last name. Concatenate
         * these and store them in the users table as the UserName.
         */

        public static void setNewUserName(string email, SqlConnection connection)
        {
            try
            {
                connection.Close(); connection.Open();

                string namePortion = email.Split(new[] { '@' })[0];
                string[] nameSplit = namePortion.Split(new[] { '.' });
                string firstName = nameSplit[0];
                string lastName = "";
                if (nameSplit.Length == 2)
                {
                    lastName = nameSplit[1];
                }

                string fullName = firstName + " " + lastName;
                fullName = fullName.Trim();
                HttpContext.Current.Session["userName"] = fullName;

                //we need to ensure that we're not writing in null values to the database.
                var commandAddName = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO STUDENTS " +
                                    "(STUDENT_ID, STUDENT_NAME) " +
                                    "VALUES (@studentId, @studentName);"
                };

                if (String.IsNullOrEmpty((string)HttpContext.Current.Session["userId"]))
                {
                    //This seems to be causing issues with some foreign keys in the database.
                    //Figure out why it's attempting to write a NULL.
                    var studentId = new SqlParameter("@studentId", SqlDbType.NVarChar, 128)
                    {
                        Value = " ***ERROR*** "
                    };
                    commandAddName.Parameters.Add(studentId);
                }
                else
                {
                    var studentId = new SqlParameter("@studentId", SqlDbType.NVarChar, 128)
                    {
                        Value = (string)HttpContext.Current.Session["userId"]
                    };
                    commandAddName.Parameters.Add(studentId);
                }

                if (String.IsNullOrEmpty((string)HttpContext.Current.Session["userName"]))
                {
                    var studentName = new SqlParameter("@studentName", SqlDbType.VarChar, 40)
                    {
                        Value = " ***ERROR*** "
                    };
                    commandAddName.Parameters.Add(studentName);
                }
                else
                {
                    var studentName = new SqlParameter("@studentName", SqlDbType.VarChar, 40)
                    {
                        Value = fullName
                    };
                    commandAddName.Parameters.Add(studentName);
                }

                commandAddName.Prepare();
                commandAddName.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a userID and roleID and gives the user that role.
         */
        public static void changeUserRole(String roleId, String userId, SqlConnection connection)
        {
            try
            {
                var changeUserRoleCommand = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE AspNetUserRoles " +
                                    "SET RoleId = @roleId " +
                                    "WHERE UserId = @userId;"
                };

                var parameterRole = new SqlParameter("@roleId", SqlDbType.NVarChar, 128)
                {
                    Value = roleId
                };
                changeUserRoleCommand.Parameters.Add(parameterRole);

                var parameterUserID = new SqlParameter("@userId", SqlDbType.VarChar, 40)
                {
                    Value = userId
                };
                changeUserRoleCommand.Parameters.Add(parameterUserID);

                connection.Close(); connection.Open();
                changeUserRoleCommand.Prepare();
                changeUserRoleCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function will return a list of all the scenarios. Teachers and Admins can edit any scenario but 
         * students are restricted to editing their own scenarios.
         */
        public static Scenarios getAdminScenariosToEdit(SqlConnection connection)
        {
            var scenarios = new Scenarios { ScenarioList = new List<Scenario>() };
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * " +
                                  "FROM SCENARIOS JOIN STUDENTS ON SCENARIOS.SCENARIO_MADE_BY = STUDENTS.STUDENT_ID " +
                                  "WHERE SCENARIO_ID != 0"
                };

                connection.Close(); connection.Open();
                command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var scenario = new Scenario();
                        scenario.scenarioName = $"{reader["SCENARIO_NAME"]}";
                        scenario.scenarioID = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        scenario.description = $"{reader["SCENARIO_DESCRIPTION"]}";
                        scenario.isActive = Convert.ToBoolean($"{reader["SCENARIO_IS_ACTIVE"]}".ToLower());
                        scenario.madeBy = $"{reader["STUDENT_NAME"]}";
                        scenario.cellFunction = $"{reader["SCENARIO_CELL_FUNCTION"]}";
                        scenario.cellShapeAndFeatures = $"{reader["SCENARIO_CELL_SHAPE_AND_FEATURES"]}";
                        scenario.cellLifespan = $"{reader["SCENARIO_CELL_LIFESPAN"]}";
                        scenario.cellNutrition = $"{reader["SCENARIO_CELL_NUTRITION"]}";
                        if ($"{reader["SCENARIO_FIRST_QUESTION"]}".Equals(""))
                            scenario.currentQuestion = 0;
                        else
                            scenario.currentQuestion = Convert.ToInt32($"{reader["SCENARIO_FIRST_QUESTION"]}");
                        scenarios.ScenarioList.Add(scenario);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return scenarios;
        }

        /*
         * This will return a list of scenarios created by the student who's ID is stored in the Singleton
         */
        public static Scenarios getStudentScenariosToEdit(SqlConnection connection)
        {
            var scenarios = new Scenarios { ScenarioList = new List<Scenario>() };
            try
            {
                var scenariosToEditCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * " +
                                  "FROM SCENARIOS " +
                                  "WHERE SCENARIO_ID != 0 AND SCENARIO_APPROVED = 0 AND SCENARIO_MADE_BY = @id"
                };

                var studentIDParam = new SqlParameter("@id", SqlDbType.NVarChar, 128)
                {
                    Value = (string)HttpContext.Current.Session["userId"]
                };
                scenariosToEditCommand.Parameters.Add(studentIDParam);

                connection.Close(); connection.Open();
                scenariosToEditCommand.Prepare();
                using (var reader = scenariosToEditCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var scenario = new Scenario();
                        scenario.scenarioName = $"{reader["SCENARIO_NAME"]}";
                        scenario.scenarioID = Convert.ToInt32($"{reader["SCENARIO_ID"]}");
                        scenario.description = $"{reader["SCENARIO_DESCRIPTION"]}";
                        scenario.isActive = Convert.ToBoolean($"{reader["SCENARIO_IS_ACTIVE"]}".ToLower());
                        scenario.cellFunction = $"{reader["SCENARIO_CELL_FUNCTION"]}";
                        scenario.cellShapeAndFeatures = $"{reader["SCENARIO_CELL_SHAPE_AND_FEATURES"]}";
                        scenario.cellLifespan = $"{reader["SCENARIO_CELL_LIFESPAN"]}";
                        scenario.cellNutrition = $"{reader["SCENARIO_CELL_NUTRITION"]}";
                        if ($"{reader["SCENARIO_FIRST_QUESTION"]}".Equals(""))
                            scenario.currentQuestion = 0;
                        else
                            scenario.currentQuestion = Convert.ToInt32($"{reader["SCENARIO_FIRST_QUESTION"]}");

                        scenarios.ScenarioList.Add(scenario);
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return scenarios;
        }

        public static void getUserRoles(SqlConnection connection)
        {
            Singleton.roles = new Dictionary<string, string>();
            try
            {
                var getUserRolesCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT * FROM AspNetRoles;"
                };

                connection.Close(); connection.Open();
                getUserRolesCommand.Prepare();
                using (var reader = getUserRolesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Singleton.roles.Add($"{reader["Id"]}", $"{reader["Name"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "USR06";
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