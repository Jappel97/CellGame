using CellGame.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CellGame.Classes
{
    /*
     * This class contains all the functions dealing with questions in the database.
     */
    public static class QuestionDatabaseUtils
    {

        /*
         * This function takes a Question as an input. This will set this question as the first question
         * for the scenario it is contained in.
         */
        public static void changeFirstQuestion(Question question, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE SCENARIOS " +
                                    "SET SCENARIO_FIRST_QUESTION = @questionId " +
                                    "WHERE SCENARIO_ID = @questionScenario"
                };

                var param = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = question.questionId
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@questionScenario", SqlDbType.Int)
                {
                    Value = question.questionScenario
                };
                command.Parameters.Add(param2);

                connection.Close(); connection.Open();
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE01";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a question and adds its information to the database.
         */
        public static int addQuestion(Question question, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "INSERT INTO QUESTIONS (QUESTION_TITLE, QUESTION_DESCRIPTION, QUESTION_SCENARIO, QUESTION_PICTURE_URL, QUESTION_IS_ACTIVE)" +
                                    "VALUES (@questionTitle, @questionDescription, @scenarioNumber, @pictureURL, 1); SELECT SCOPE_IDENTITY();"
                };

                var param = new SqlParameter("@questionTitle", SqlDbType.VarChar, 140)
                {
                    Value = question.questionTitle
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@questionDescription", SqlDbType.VarChar, 1000)
                {
                    Value = question.questionDescription
                };
                command.Parameters.Add(param2);

                if (question.questionPicture == null)
                {
                    question.questionPicture = "";
                }
                var param3 = new SqlParameter("@pictureURL", SqlDbType.VarChar, 500)
                {
                    Value = question.questionPicture
                };
                command.Parameters.Add(param3);

                var param4 = new SqlParameter("@scenarioNumber", SqlDbType.Int)
                {
                    Value = question.questionScenario
                };
                command.Parameters.Add(param4);

                connection.Close(); connection.Open();
                command.Prepare();
                question.questionId = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE02";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return question.questionId;
        }

        /*
         * This function takes in a Question and updates its values in the database.
         */
        public static void editQuestion(Question question, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE QUESTIONS SET " +
                                    "QUESTION_TITLE = @questionTitle, " +
                                    "QUESTION_DESCRIPTION = @questionDescription, " +
                                    "QUESTION_PICTURE_URL = @pictureURL " +
                                    "WHERE QUESTION_ID = @questionId"
                };

                var param = new SqlParameter("@questionTitle", SqlDbType.VarChar, 140)
                {
                    Value = question.questionTitle
                };
                command.Parameters.Add(param);

                var param2 = new SqlParameter("@questionDescription", SqlDbType.VarChar, 1000)
                {
                    Value = question.questionDescription
                };
                command.Parameters.Add(param2);

                var param3 = new SqlParameter("@pictureURL", SqlDbType.VarChar, 200)
                {
                    Value = question.questionPicture
                };
                command.Parameters.Add(param3);

                var param4 = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = question.questionId
                };
                command.Parameters.Add(param4);

                connection.Close(); connection.Open();
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE03";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes a questionID and returns the number of times it is referenced in the STUDENT_ANSWERS table.
         */
        public static int getQuestionUsage(int questionId, SqlConnection connection)
        {
            int count;
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM STUDENT_ANSWERS WHERE SA_QUESTION_ID = @questionId"
                };

                var param = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = questionId
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                count = (Int32)command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE04";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return count;
        }

        /*
         * This function takes in a questionID and makes that question inactive.
         */
        public static void makeQuestionInactive(int questionId, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE QUESTIONS SET QUESTION_IS_ACTIVE = 0 WHERE QUESTION_ID = @questionId; " +
                                        "UPDATE ANSWERS SET ANSWER_IS_ACTIVE = 0 WHERE " +
                                        "ANSWER_ID IN (SELECT ANSWER_QUESTIONS.ANSWER_ID FROM ANSWER_QUESTIONS WHERE QUESTION_REFERENCE_ID = @questionId);"

                };

                var param = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = questionId
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE05";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a questionID and makes that question active.
         */
        public static void makeQuestionActive(int questionId, SqlConnection connection)
        {
            try
            {
                var command = new SqlCommand(null, connection)
                {
                    CommandText = "UPDATE QUESTIONS SET QUESTION_IS_ACTIVE = 1 WHERE QUESTION_ID = @questionId"
                };

                var param = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = questionId
                };
                command.Parameters.Add(param);

                connection.Close(); connection.Open();
                command.Prepare();

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE06";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        /*
         * This function takes in a questionID and deletes that question from the database.
         */
        public static bool deleteQuestion(int questionId, SqlConnection connection)
        {
            ((EditScenarioModel)HttpContext.Current.Session["editScenario"]).warningMsg = "";
            bool returnValue = true;
            try
            {
                var checkQuestionReferenceCommand = new SqlCommand(null, connection)
                {
                    CommandText = "SELECT COUNT(*) FROM STUDENT_ANSWERS WHERE SA_QUESTION_ID = @questionId"
                };
                var questionIDParam = new SqlParameter("@questionId", SqlDbType.Int)
                {
                    Value = questionId
                };

                checkQuestionReferenceCommand.Parameters.Add(questionIDParam);
                connection.Close(); connection.Open();
                checkQuestionReferenceCommand.Prepare();

                if ((Int32) checkQuestionReferenceCommand.ExecuteScalar() > 0)
                {

                    Singleton.warningMsg  += "\nThis question is being used in scenarios that are being graded. ";
                    returnValue = false;
                }

                checkQuestionReferenceCommand.CommandText =
                    "SELECT COUNT(*) FROM ANSWER_QUESTIONS WHERE QUESTION_REFERENCE_ID = @questionId";

                connection.Close(); connection.Open();
                checkQuestionReferenceCommand.Prepare();

                if ((Int32) checkQuestionReferenceCommand.ExecuteScalar() > 0)
                {
                    Singleton.warningMsg += "\nThis question is referenced by one or more answers. ";
                    returnValue = false;
                }

                checkQuestionReferenceCommand.CommandText =
                    "SELECT COUNT(*) FROM ANSWERS WHERE ANSWER_FOR_QUESTION = @questionId";

                connection.Close(); connection.Open();
                checkQuestionReferenceCommand.Prepare();

                if ((Int32)checkQuestionReferenceCommand.ExecuteScalar() > 0)
                {
                    Singleton.warningMsg += "\nA question cannot be deleted when it contains answers. ";
                    returnValue = false;
                }
                checkQuestionReferenceCommand.Parameters.Clear();


                if (!returnValue)
                {
                    Singleton.warningMsg += "\nThis question has been deactivated. ";
                    Singleton.isWarning = true;
                    return returnValue;
                }


                var deleteQuestionCommand = new SqlCommand(null, connection)
                {
                    CommandText = "DELETE FROM QUESTIONS WHERE QUESTION_ID = @questionId"
                };

                deleteQuestionCommand.Parameters.Add(questionIDParam);

                connection.Close(); connection.Open();
                deleteQuestionCommand.Prepare();

                deleteQuestionCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Singleton.errorCode = "QUE07";
                Singleton.writeErrorToFile(Singleton.errorCode, ex.Message, ex.StackTrace);
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return returnValue;
        }

    }
}
